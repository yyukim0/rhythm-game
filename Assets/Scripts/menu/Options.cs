using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour
{                   //start é o "groovar"
    public TMP_Text start, options, exit, textOptions, volumeOptions, brightnessOptions, exitOptions, pressAnyKeyText;
    public Slider volumeSlider, brightnessSlider;
    public CanvasGroup volumeSliderGroup, brightnessSliderGroup;
    public CanvasGroup brightnessOverlay;
    public AudioMixer masterMixer;

    public int selectedOption = 0;

    public TMP_FontAsset fontRegular;
    public TMP_FontAsset fontLight;

    private bool isOption = false; // flag que diz se a parte de opções está ativa no menu
    private bool isStarted = false; // flag que diz se o player apertou um botão ou não logo no inicio
    private bool isMenuReady = false; // trava para o menu só aceitar inputs e atualizar o visual após o fade acabar

    public RectTransform backgroundMenu, backgroundOptions, telaAmarela;
    public float speedTransitionMenu;
    public Image menuTop, menuBottom, deSelectedGroove, deSelectedOptions, deSelectedExit, selectedGroove, selectedOptions, selectedExit, adjust1, 
                 adjust2, adjust3, adjust4;
    public Image greenLine, yellowLine, blackLine, pinkBackground;

    private RectTransform[] alimentos;
    private Vector2 deslocamentoMouse; //matriz que pega a posição do mouse
    private Vector2[] posicoesOriginais; 

    //isso previne que o dotween faça merda e coloque varias animações simultaneas para o mesmo alimento
    private Tween[] foodTweens; //pedi ajuda da IA porque estava dando errado
    private Vector2 ultimoDeslocamento; //guarda o ultimo deslocamento aplicado

    void Start()
    {
        Cursor.visible = false;

        //recupera o valor salvo do brilho e já deixa iniciado como 50%
        float savedBrightness = PlayerPrefs.GetFloat("BrightnessValue", 50f);
        if (brightnessSlider != null) {
            brightnessSlider.value = savedBrightness;
        }
        SetBrightness(savedBrightness);

        float savedVolume = PlayerPrefs.GetFloat("VolumeValue", 50f);
        if (volumeSlider != null) {
            volumeSlider.value = savedVolume;
        }
        SetAudioVolume(savedVolume);
        
        //esconde os elementos na tela no inicioj
        SetupInitialState();

        GameObject[] objetos = GameObject.FindGameObjectsWithTag("Alimento"); //encontra objetos com a tag "alimento"

        alimentos = new RectTransform[objetos.Length];
        foodTweens = new Tween[alimentos.Length];

        for (int i = 0; i < objetos.Length; i++)
        {
            alimentos[i] = objetos[i].GetComponent<RectTransform>(); //temos uma lista com todos os objetos com a tag "alimento"
        }

        posicoesOriginais = new Vector2[alimentos.Length];

        for (int i = 0; i < alimentos.Length; i++)
        {
            if (alimentos[i] != null)
            {
                posicoesOriginais[i] = alimentos[i].anchoredPosition; //guarda as posições dos objetos
            }
        }

        //faz o texto de apertar qualquer tecla piscar
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.DOFade(0.2f, 0.8f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    void Update()
    {
        if (!isStarted)
        {
            CalculateMouseMovement();

            if (Vector2.Distance(deslocamentoMouse, ultimoDeslocamento) > 0.1f)
            {
                ultimoDeslocamento = deslocamentoMouse;
                MoveFoods();
            }
        }

        if(!isStarted && Input.anyKeyDown) {
            isStarted = true;

            if(pressAnyKeyText != null) {
                pressAnyKeyText.DOKill();
                pressAnyKeyText.DOFade(0f, 0.3f).OnComplete(() => pressAnyKeyText.gameObject.SetActive(false));
            }
            if (telaAmarela != null) 
            {
                DOVirtual.DelayedCall(1f, () => //faz os alimentos desaparecerem um tempinho depois da tela amarela
                {
                    foreach (RectTransform alimento in alimentos)
                    {
                        if (alimento != null)
                        {
                            Image imagem = alimento.GetComponent<Image>();

                            if (imagem != null)
                            {
                                imagem.DOFade(0f, 0.5f);
                            }
                        }
                    }
                });
                telaAmarela.DOAnchorPosY(0f, speedTransitionMenu).SetEase(Ease.InQuart).OnComplete(() => { //background sobe
                    DOVirtual.DelayedCall(2f, () => {
                        telaAmarela.DOAnchorPosY(-1200f, speedTransitionMenu).SetEase(Ease.InQuart);
                        ShowMainMenu(); //e após subir manda o resto dos fade in nos botões
                    });
                });
            }
            else {
                ShowMainMenu();
            }
        }
        if(isStarted && isMenuReady) {
            DetectInput();
            if (!isOption)
            {
                VisualMenu();
            }
            else {
                VisualMenuOptions();
            }
        }
    }

    Vector2 GetMouseDirection() //vai pegar a direção do mouse para os objetos na tela de press a button
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float mouseX = (mousePosition.x / Screen.width) * 2f - 1f;
        float mouseY = (mousePosition.y / Screen.height) * 2f - 1f;

        return new Vector2(mouseX, mouseY);
    }

    void CalculateMouseMovement()
    {
        Vector2 direcaoMouse = GetMouseDirection();

        deslocamentoMouse = direcaoMouse * 15f; //intensidade que o objeto vai se mover de acordo com a direção do mouse
    }

    void MoveFoods()
    {
        for (int i = 0; i < alimentos.Length; i++)
        {
            if (alimentos[i] != null)
            {
                Vector2 novaPosicao = posicoesOriginais[i] + deslocamentoMouse; //calcula onde vai ser a nova posição da comida

                foodTweens[i]?.Kill(); //mata a animação anterior desse elemento se uma nova foi feita

                foodTweens[i] = alimentos[i].DOAnchorPos(novaPosicao, 0.35f).SetEase(Ease.OutQuad); //o dotween faz o efeito de levar a comida até lá
            }
        }
    }

    void SetupInitialState()
    {
        // Desativa tudo do menu no início
        start.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        exit.gameObject.SetActive(false);

        if (menuTop != null) menuTop.gameObject.SetActive(false);
        if (menuBottom != null) menuBottom.gameObject.SetActive(false);

        if (deSelectedGroove != null) deSelectedGroove.gameObject.SetActive(false);
        if (deSelectedOptions != null) deSelectedOptions.gameObject.SetActive(false);
        if (deSelectedExit != null) deSelectedExit.gameObject.SetActive(false);

        if (selectedGroove != null) selectedGroove.gameObject.SetActive(false);
        if (selectedOptions != null) selectedOptions.gameObject.SetActive(false);
        if (selectedExit != null) selectedExit.gameObject.SetActive(false);

        textOptions.gameObject.SetActive(false);
        volumeOptions.gameObject.SetActive(false);
        brightnessOptions.gameObject.SetActive(false);
        exitOptions.gameObject.SetActive(false);

        if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
        if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(false);

        adjust1.gameObject.SetActive(false);
        adjust2.gameObject.SetActive(false);
        adjust3.gameObject.SetActive(false);
        adjust4.gameObject.SetActive(false);
    }

    void ShowMainMenu()
    {
        isMenuReady = false;

        // Ativa os textos do menu principal com alfa 0 e faz o Fade In de 0.8 segundos

        backgroundMenu.gameObject.SetActive(true);

        start.gameObject.SetActive(true);
        options.gameObject.SetActive(true);
        exit.gameObject.SetActive(true);
        menuTop.gameObject.SetActive(true);
        menuBottom.gameObject.SetActive(true);

        selectedGroove.gameObject.SetActive(true);
        selectedOptions.gameObject.SetActive(false);
        selectedExit.gameObject.SetActive(false);

        deSelectedGroove.gameObject.SetActive(false);
        deSelectedOptions.gameObject.SetActive(true);
        deSelectedExit.gameObject.SetActive(true);

        selectedOption = 0;
        
        start.alpha = 0f;
        options.alpha = 0f;
        exit.alpha = 0f;

        if (menuTop != null) { Color c = menuTop.color; c.a = 0f; menuTop.color = c; }
        if (menuBottom != null) { Color c = menuBottom.color; c.a = 0f; menuBottom.color = c; }
        
        if (selectedGroove != null) { Color c = selectedGroove.color; c.a = 0f; selectedGroove.color = c; }
        if (deSelectedOptions != null) { Color c = deSelectedOptions.color; c.a = 0f; deSelectedOptions.color = c; }
        if (deSelectedExit != null) { Color c = deSelectedExit.color; c.a = 0f; deSelectedExit.color = c; }
        if (deSelectedGroove != null) { Color c = deSelectedGroove.color; c.a = 1f; deSelectedGroove.color = c; }
        if (selectedOptions != null) { Color c = selectedOptions.color; c.a = 1f; selectedOptions.color = c; }
        if (selectedExit != null) { Color c = selectedExit.color; c.a = 1f; selectedExit.color = c; }
        

        start.DOFade(1f, 1f);
        options.DOFade(1f, 1f);
        exit.DOFade(1f, 1f);
        menuTop.DOFade(1f, 1f);
        menuBottom.DOFade(1f, 1f);
        selectedGroove.DOFade(1f, 1f);
        deSelectedOptions.DOFade(1f, 1f);
        deSelectedExit.DOFade(1f, 1f).OnComplete(() => {
            isMenuReady = true; 
            VisualMenu();
        });

        // garante que o painel de opções está oculto
        textOptions.gameObject.SetActive(false);
        volumeOptions.gameObject.SetActive(false);
        brightnessOptions.gameObject.SetActive(false);
        exitOptions.gameObject.SetActive(false);

        if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
        if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(false);
    }

    void ShowOptionsScreen()
    {
        backgroundMenu.gameObject.SetActive(false);
        backgroundOptions.gameObject.SetActive(true);

        greenLine.rectTransform.DOAnchorPosX(0f, 2f).SetEase(Ease.InQuart).SetDelay(0.3f);
        yellowLine.rectTransform.DOAnchorPosX(0f, 2f).SetEase(Ease.InQuart).SetDelay(0.5f);
        blackLine.rectTransform.DOAnchorPosX(0f, 2f).SetEase(Ease.InQuart).SetDelay(0.7f);
        pinkBackground.rectTransform.DOAnchorPosX(0f, 2f).SetEase(Ease.InQuart).SetDelay(0.9f);

        backgroundOptions.gameObject.SetActive(true);
        // Ativa opções
        textOptions.gameObject.SetActive(true);
        volumeOptions.gameObject.SetActive(true);
        brightnessOptions.gameObject.SetActive(true);
        exitOptions.gameObject.SetActive(true);
        volumeSlider.gameObject.SetActive(true);
        brightnessSlider.gameObject.SetActive(true);

        textOptions.alpha = 0f;
        volumeOptions.alpha = 0f;
        brightnessOptions.alpha = 0f;
        exitOptions.alpha = 0f;
        volumeSliderGroup.alpha = 0f;
        brightnessSliderGroup.alpha = 0f;

        adjust1.color = new Color(adjust1.color.r, adjust1.color.g, adjust1.color.b, 0f);
        adjust2.color = new Color(adjust2.color.r, adjust2.color.g, adjust2.color.b, 0f);
        adjust3.color = new Color(adjust3.color.r, adjust3.color.g, adjust3.color.b, 0f);
        adjust4.color = new Color(adjust4.color.r, adjust4.color.g, adjust4.color.b, 0f);

        adjust1.gameObject.SetActive(true);
        adjust2.gameObject.SetActive(true);
        adjust3.gameObject.SetActive(true);
        adjust4.gameObject.SetActive(true);

        textOptions.DOFade(1f, 1f).SetDelay(3f);
        volumeOptions.DOFade(1f, 1f).SetDelay(3f);
        brightnessOptions.DOFade(1f, 1f).SetDelay(3f);
        exitOptions.DOFade(1f, 1f).SetDelay(3f);
        menuTop.DOFade(1f, 1f).SetDelay(3f);
        //bota aqui
        adjust1.DOFade(1f, 1f).SetDelay(2.8f);
        adjust2.DOFade(1f, 1f).SetDelay(2.8f);
        adjust3.DOFade(1f, 1f).SetDelay(2.8f);
        adjust4.DOFade(1f, 1f).SetDelay(2.8f);
        volumeSliderGroup.DOFade(1f, 1f).SetDelay(3.2f);
        brightnessSliderGroup.DOFade(1f, 1f).SetDelay(3.2f).OnComplete(() =>
        {
            isMenuReady = true;
        });

        if (volumeSlider != null) volumeSlider.gameObject.SetActive(true);
        if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(true);        
    }

    void HideOptionsScreen()
    {
        // Trava o menu durante a transição de retorno
        isMenuReady = false;

        // Faz Fade Out das opções e retorna para o menu principal
        textOptions.DOFade(0f, 0.4f).SetDelay(0.8f);
        volumeOptions.DOFade(0f, 0.4f).SetDelay(0.8f);
        brightnessOptions.DOFade(0f, 0.4f).SetDelay(0.8f);
        exitOptions.DOFade(0f, 0.4f).SetDelay(0.8f);

        adjust1.DOFade(0f, 0.4f).SetDelay(0.8f);
        adjust2.DOFade(0f, 0.4f).SetDelay(0.8f);
        adjust3.DOFade(0f, 0.4f).SetDelay(0.8f);
        adjust4.DOFade(0f, 0.4f).SetDelay(0.8f);

        volumeSliderGroup.DOFade(0f, 0.4f).SetDelay(0.8f);
        brightnessSliderGroup.DOFade(0f, 0.4f).SetDelay(0.8f);

        greenLine.rectTransform.DOAnchorPosX(1633f, 2f).SetEase(Ease.InQuart).SetDelay(0.3f);
        yellowLine.rectTransform.DOAnchorPosX(1633f, 2f).SetEase(Ease.InQuart).SetDelay(0.5f);
        blackLine.rectTransform.DOAnchorPosX(1633f, 2f).SetEase(Ease.InQuart).SetDelay(0.7f);
        pinkBackground.rectTransform.DOAnchorPosX(1633f, 2f).SetEase(Ease.InQuart).SetDelay(0.9f).OnComplete(() => {
            backgroundOptions.gameObject.SetActive(false);
            
            // Desativa os elementos de opções
            textOptions.gameObject.SetActive(false);
            volumeOptions.gameObject.SetActive(false);
            brightnessOptions.gameObject.SetActive(false);
            exitOptions.gameObject.SetActive(false);

            adjust1.gameObject.SetActive(false);
            adjust2.gameObject.SetActive(false);
            adjust3.gameObject.SetActive(false);
            adjust4.gameObject.SetActive(false);

            if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
            if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(false);

            // Só agora sai do modo opções, depois que o fade out acabou
            isOption = false;

            ShowMainMenu();
        });
    }

    void DetectInput() 
    {
        if(!isOption){
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.dKey.wasPressedThisFrame) 
            {
                selectedOption++;
                if(selectedOption > 2) selectedOption = 0;
            } 
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame) 
            {
                selectedOption--;
                if(selectedOption < 0) selectedOption = 2;
            }
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame) 
            {
                WhenOptionIsSelected();
            }
        }
        else {
            //Navegação vertical
            if (Keyboard.current.downArrowKey.wasPressedThisFrame || Keyboard.current.sKey.wasPressedThisFrame) 
            {
                selectedOption++;
                if(selectedOption > 2) selectedOption = 0;
            } 
            if (Keyboard.current.upArrowKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame) 
            {
                selectedOption--;
                if(selectedOption < 0) selectedOption = 2;
            }
            if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame) 
            {
                WhenOptionIsSelectedOptions();
            }

            //Navegação dos sliders
            float moveValue = 0f;
            if(Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) {
                moveValue = 1.5f;
            }
            if(Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) {
                moveValue = -1.5f;
            }

            if(moveValue != 0f) {
                ControlSlider(moveValue);
            }
        }
    }

    void ControlSlider(float valueSlider) {
        if (selectedOption == 0 && volumeSlider != null) {
            volumeSlider.value += valueSlider;
            SetAudioVolume(volumeSlider.value);
        }
        if (selectedOption == 1 && brightnessSlider != null) {
            brightnessSlider.value += valueSlider;
            SetBrightness(brightnessSlider.value);
        }
    }

    void SetAudioVolume(float sliderValue) {
        float normalizedValue = sliderValue / 50;
        float clampedValue = Mathf.Clamp(normalizedValue, 0.0001f, 10f);
        float dB = Mathf.Log10(clampedValue) * 20;

        if (masterMixer != null) {
            masterMixer.SetFloat("GameVolume", dB);
        }

        PlayerPrefs.SetFloat("VolumeValue", sliderValue);
        PlayerPrefs.Save();
    }

    void SetBrightness(float sliderValue) {
        float offset = (sliderValue - 50f) / 50f;

        Image overlayImage = brightnessOverlay.GetComponent<Image>();
        if(overlayImage != null) {
            if(offset < 0f) {
                overlayImage.color = Color.black;
                brightnessOverlay.alpha = -offset * 0.5f;
            }
            else {
                overlayImage.color = Color.white;
                brightnessOverlay.alpha = offset * 0.5f;
            }
        }    
        PlayerPrefs.SetFloat("BrightnessValue", sliderValue);
        PlayerPrefs.Save();
    }

    // PARTE DO MENU PRINCIPAL
    void WhenOptionIsSelected() 
    {
        if(selectedOption == 0) 
        {
            SceneManager.LoadScene("Fase1");
        }
        else if(selectedOption == 1)
        {
            isOption = true;
            isMenuReady = false;
            selectedOption = 0;

            ShowYellowScreen(() =>
            {
                ShowOptionsScreen();
            });

            // Fade Out do menu principal, simultaneamente à tela amarela.
            start.DOFade(0f, 0.4f);
            options.DOFade(0f, 0.4f);
            menuTop.DOFade(0f, 0.4f);
            menuBottom.DOFade(0f, 0.4f);
            deSelectedGroove.DOFade(0f, 0.4f);
            selectedOptions.DOFade(0f, 0.4f);
            deSelectedExit.DOFade(0f, 0.4f);

            exit.DOFade(0f, 0.4f).OnComplete(() =>
            {
                start.gameObject.SetActive(false);
                options.gameObject.SetActive(false);
                exit.gameObject.SetActive(false);
                menuTop.gameObject.SetActive(false);
                menuBottom.gameObject.SetActive(false);

                selectedGroove.gameObject.SetActive(false);
                selectedOptions.gameObject.SetActive(false);
                selectedExit.gameObject.SetActive(false);

                deSelectedGroove.gameObject.SetActive(false);
                deSelectedOptions.gameObject.SetActive(false);
                deSelectedExit.gameObject.SetActive(false);
            });
        }
        else if(selectedOption == 2) 
        {
            Application.Quit();
        }
    }

    void VisualMenu() 
    {
        if(selectedOption == 0) {
            selectedGroove.gameObject.SetActive(true);
            selectedOptions.gameObject.SetActive(false);
            selectedExit.gameObject.SetActive(false);
        
            deSelectedGroove.gameObject.SetActive(false);
            deSelectedOptions.gameObject.SetActive(true);
            deSelectedExit.gameObject.SetActive(true);
        }

        if(selectedOption == 1) {
            selectedGroove.gameObject.SetActive(false);
            selectedOptions.gameObject.SetActive(true);
            selectedExit.gameObject.SetActive(false);
        
            deSelectedGroove.gameObject.SetActive(true);
            deSelectedOptions.gameObject.SetActive(false);
            deSelectedExit.gameObject.SetActive(true);
        }

        if(selectedOption == 2) {
            selectedGroove.gameObject.SetActive(false);
            selectedOptions.gameObject.SetActive(false);
            selectedExit.gameObject.SetActive(true);
        
            deSelectedGroove.gameObject.SetActive(true);
            deSelectedOptions.gameObject.SetActive(true);
            deSelectedExit.gameObject.SetActive(false);
        }

        //não usar isso, essa parte era apenas para mudar a fonte se selecionado, porém estamos usando imagens agora
        // start.font = fontLight;
        // options.font = fontLight;
        // exit.font = fontLight;

        // if(selectedOption == 0) start.font = fontRegular;
        // if(selectedOption == 1) options.font = fontRegular;
        // if(selectedOption == 2) exit.font = fontRegular;
    }

    void InsideMenu() {
        start.gameObject.SetActive(true); 
        options.gameObject.SetActive(true); 
        exit.gameObject.SetActive(true); 

        start.DOFade(1f, 0.5f).SetDelay(0.3f);
        options.DOFade(1f, 0.5f).SetDelay(0.3f);
        exit.DOFade(1f, 0.5f).SetDelay(0.3f);

        textOptions.gameObject.SetActive(false); 
        volumeOptions.gameObject.SetActive(false); 
        brightnessOptions.gameObject.SetActive(false); 
        exitOptions.gameObject.SetActive(false); 

        if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
        if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(false);   
    }

    // TELA DO OPTIONS

    void WhenOptionIsSelectedOptions() 
    {
        if(selectedOption == 2) 
        {
            selectedOption = 1;
            HideOptionsScreen();
        }
    }

    void VisualMenuOptions() 
    {
        //não usar isso, essa parte era apenas para mudar a fonte se selecionado, porém estamos usando imagens agora
        // volumeOptions.font = fontLight;
        // brightnessOptions.font = fontLight;
        // exitOptions.font = fontLight;

        // if(selectedOption == 0) volumeOptions.font = fontRegular;
        // if(selectedOption == 1) brightnessOptions.font = fontRegular;
        // if(selectedOption == 2) exitOptions.font = fontRegular;
    }

    void ShowYellowScreen(System.Action onComplete)
    {
        telaAmarela.DOAnchorPosY(0f, speedTransitionMenu)
            .SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                // Troca o background enquanto a tela amarela cobre tudo.
                backgroundMenu.gameObject.SetActive(false);
                backgroundOptions.gameObject.SetActive(true);

                DOVirtual.DelayedCall(2f, () =>
                {
                    // Faz a tela amarela descer.
                    telaAmarela.DOAnchorPosY(-1200f, speedTransitionMenu)
                        .SetEase(Ease.InQuart)
                        .OnComplete(() =>
                        {
                            // Só depois que a tela amarela sair,
                            // começa a animação das opções.
                            onComplete?.Invoke();
                        });
                });
            });
    }
}   