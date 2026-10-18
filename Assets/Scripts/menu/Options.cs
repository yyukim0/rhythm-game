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
{
    public TMP_Text start, options, exit, textOptions, volumeOptions, brightnessOptions, exitOptions, pressAnyKeyText;
    public Slider volumeSlider, brightnessSlider;
    public CanvasGroup brightnessOverlay;
    public AudioMixer masterMixer;

    public int selectedOption = 0;

    public TMP_FontAsset fontRegular;
    public TMP_FontAsset fontLight;

    private bool isOption = false;
    private bool isStarted = false; // flag que diz se o player apertou um botão ou não logo no inicio

    public RectTransform backgroundMenu;
    public float speedTransitionMenu;

    void Start()
    {
        Cursor.visible = false;

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

        SetupInitialState();
        
        if (pressAnyKeyText != null)
        {
            pressAnyKeyText.DOFade(0.2f, 0.8f).SetLoops(-1, LoopType.Yoyo);
        }
    }

    void Update()
    {
        if(!isStarted && Input.anyKeyDown) {
            isStarted = true;

            if(pressAnyKeyText != null) {
                pressAnyKeyText.DOKill();
                pressAnyKeyText.DOFade(0f, 0.3f).OnComplete(() => pressAnyKeyText.gameObject.SetActive(false));
            }
            if (backgroundMenu != null) 
            {
                backgroundMenu.DOAnchorPosY(0f, speedTransitionMenu).SetEase(Ease.InQuart).OnComplete(() => { //background sobe
                    ShowMainMenu(); //e após subir manda o resto dos fade in nos botões
                });
            }
            else {
                ShowMainMenu();
            }
        }
        if(isStarted) {
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

    void SetupInitialState()
    {
        // Desativa tudo do menu no início
        start.gameObject.SetActive(false);
        options.gameObject.SetActive(false);
        exit.gameObject.SetActive(false);

        textOptions.gameObject.SetActive(false);
        volumeOptions.gameObject.SetActive(false);
        brightnessOptions.gameObject.SetActive(false);
        exitOptions.gameObject.SetActive(false);

        if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
        if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(false);
    }

    void ShowMainMenu()
    {
        // Ativa os textos do menu principal com alfa 0 e faz o Fade In de 0.8 segundos
        start.gameObject.SetActive(true);
        options.gameObject.SetActive(true);
        exit.gameObject.SetActive(true);

        start.alpha = 0f;
        options.alpha = 0f;
        exit.alpha = 0f;

        start.DOFade(1f, 0.8f);
        options.DOFade(1f, 0.8f);
        exit.DOFade(1f, 0.8f);

        // Garante que o painel de opções está oculto
        textOptions.gameObject.SetActive(false);
        volumeOptions.gameObject.SetActive(false);
        brightnessOptions.gameObject.SetActive(false);
        exitOptions.gameObject.SetActive(false);

        if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
        if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(false);
    }

    void ShowOptionsScreen()
    {
        // Faz Fade Out do menu principal e depois ativa o menu de opções com Fade In
        start.DOFade(0f, 0.4f);
        options.DOFade(0f, 0.4f);
        exit.DOFade(0f, 0.4f).OnComplete(() => {
            start.gameObject.SetActive(false);
            options.gameObject.SetActive(false);
            exit.gameObject.SetActive(false);

            // Ativa opções
            textOptions.gameObject.SetActive(true);
            volumeOptions.gameObject.SetActive(true);
            brightnessOptions.gameObject.SetActive(true);
            exitOptions.gameObject.SetActive(true);

            textOptions.alpha = 0f;
            volumeOptions.alpha = 0f;
            brightnessOptions.alpha = 0f;
            exitOptions.alpha = 0f;

            textOptions.DOFade(1f, 0.6f);
            volumeOptions.DOFade(1f, 0.6f);
            brightnessOptions.DOFade(1f, 0.6f);
            exitOptions.DOFade(1f, 0.6f);

            if (volumeSlider != null) volumeSlider.gameObject.SetActive(true);
            if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(true);
        });
    }

    void HideOptionsScreen()
    {
        // Faz Fade Out das opções e retorna para o menu principal
        textOptions.DOFade(0f, 0.4f);
        volumeOptions.DOFade(0f, 0.4f);
        brightnessOptions.DOFade(0f, 0.4f);
        exitOptions.DOFade(0f, 0.4f).OnComplete(() => {
            textOptions.gameObject.SetActive(false);
            volumeOptions.gameObject.SetActive(false);
            brightnessOptions.gameObject.SetActive(false);
            exitOptions.gameObject.SetActive(false);

            if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
            if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(false);

            // Reativa o menu principal com Fade In
            start.gameObject.SetActive(true);
            options.gameObject.SetActive(true);
            exit.gameObject.SetActive(true);

            start.alpha = 0f;
            options.alpha = 0f;
            exit.alpha = 0f;

            start.DOFade(1f, 0.6f);
            options.DOFade(1f, 0.6f);
            exit.DOFade(1f, 0.6f);
        });
    }

    void DetectInput() 
    {
        if(!isOption){
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
            selectedOption = 0;
            ShowOptionsScreen();
        }
        else if(selectedOption == 2) 
        {
            Application.Quit();
        }
    }

    void VisualMenu() 
    {
        start.font = fontLight;
        options.font = fontLight;
        exit.font = fontLight;

        if(selectedOption == 0) start.font = fontRegular;
        if(selectedOption == 1) options.font = fontRegular;
        if(selectedOption == 2) exit.font = fontRegular;
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
            isOption = false;
            selectedOption = 1;
            HideOptionsScreen();
        }
    }

    void VisualMenuOptions() 
    {
        volumeOptions.font = fontLight;
        brightnessOptions.font = fontLight;
        exitOptions.font = fontLight;

        if(selectedOption == 0) volumeOptions.font = fontRegular;
        if(selectedOption == 1) brightnessOptions.font = fontRegular;
        if(selectedOption == 2) exitOptions.font = fontRegular;
    }
}   