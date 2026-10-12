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
    public TMP_Text start, options, exit;
    public int selectedOption = 0;

    public TMP_FontAsset fontRegular;
    public TMP_FontAsset fontLight;

    void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        DetectInput();
        VisualMenu();
    }

    void DetectInput() 
    {
        // Garante que o teclado está conectado antes de ler
        if (Keyboard.current == null) return;

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

    void VisualMenu() 
    {
        start.font = fontLight;
        options.font = fontLight;
        exit.font = fontLight;

        if(selectedOption == 0) start.font = fontRegular;
        if(selectedOption == 1) options.font = fontRegular;
        if(selectedOption == 2) exit.font = fontRegular;
    }

    void WhenOptionIsSelected() 
    {
        if(selectedOption == 0) 
        {
            SceneManager.LoadScene("Fase1");
        }
        if(selectedOption == 1)
        {
            Debug.Log("Option selecionado");
        }
        if(selectedOption == 2) 
        {
            Debug.Log("Tentando fechar o jogo...");
            Application.Quit();
        }
    }
}