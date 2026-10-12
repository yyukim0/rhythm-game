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
        public TMP_Text start, options, exit, textOptions, volumeOptions, brightnessOptions, exitOptions;
        public Slider volumeSlider, brightnessSlider;
        public CanvasGroup brightnessOverlay;
        public AudioMixer masterMixer;

        public int selectedOption = 0;

        public TMP_FontAsset fontRegular;
        public TMP_FontAsset fontLight;

        private bool isOption = false;

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
        }

        void Update()
        {
            DetectInput();

            if (!isOption)
            {
                InsideMenu();
                VisualMenu();
            }
            else {
                InsideOptions();
                VisualMenuOptions();
            }
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
                if(Keyboard.current.dKey.wasPressedThisFrame || Keyboard.current.rightArrowKey.wasPressedThisFrame) {
                    moveValue = 5f;
                }
                if(Keyboard.current.aKey.wasPressedThisFrame || Keyboard.current.leftArrowKey.wasPressedThisFrame) {
                    moveValue = -5f;
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

            textOptions.gameObject.SetActive(false); 
            volumeOptions.gameObject.SetActive(false); 
            brightnessOptions.gameObject.SetActive(false); 
            exitOptions.gameObject.SetActive(false); 

            if (volumeSlider != null) volumeSlider.gameObject.SetActive(false);
            if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(false);   
        }

        // TELA DO OPTIONS

        void WhenOptionIsSelectedOptions() {
            if(selectedOption == 0) 
            {
                Debug.Log("aa");
            }
            else if(selectedOption == 1)
            {
                Debug.Log("aa");
            }
            else if(selectedOption == 2) 
            {
                isOption = false;
                selectedOption = 1;
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

        void InsideOptions() 
        {
            start.gameObject.SetActive(false); 
            options.gameObject.SetActive(false); 
            exit.gameObject.SetActive(false); 

            textOptions.gameObject.SetActive(true); 
            volumeOptions.gameObject.SetActive(true); 
            brightnessOptions.gameObject.SetActive(true); 
            exitOptions.gameObject.SetActive(true); 

            if (volumeSlider != null) volumeSlider.gameObject.SetActive(true);
            if (brightnessSlider != null) brightnessSlider.gameObject.SetActive(true);
        }
    }   