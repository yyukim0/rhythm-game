using UnityEngine;
using UnityEngine.InputSystem;

public class Amassar : MonoBehaviour
{
    private GameObject massa;
    private bool indoProLixo = false;

    private void OnTriggerEnter(Collider bater)
    {
        if (bater.CompareTag("Massa"))
        {
            massa = bater.gameObject;
            indoProLixo = false;
        }
    }

    private void OnTriggerExit(Collider bater)
    {
        if (bater.CompareTag("Massa"))
        {
            if (bater.gameObject == massa)
            {
                massa = null;
                indoProLixo = false;
            }
        }
    }

    void Update()
    {
        if (indoProLixo && massa != null)
        {
            massa.transform.Translate(0f, 0f, -0.05f);
            return;     
        }
        
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame && massa != null) 
        {
            indoProLixo = true;
            Debug.Log("jogou no lixo!");
        }
        
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && massa != null)
        {
            massa.transform.localScale -= new Vector3(0f, 0.05f, 0f);
            massa.transform.position -= new Vector3(0f, 0f, 0.05f);

            Debug.Log("amassou!");
        }
    }
}