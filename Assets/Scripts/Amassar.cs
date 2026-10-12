using UnityEngine;
using UnityEngine.InputSystem;

public class Amassar : MonoBehaviour
{
    private GameObject massa;
    private bool indoProLixo = false;
    private bool jaAmassou = false; // Flag para controlar se já foi amassado

    private void OnTriggerEnter(Collider bater)
    {
        if (bater.CompareTag("Massa"))
        {
            massa = bater.gameObject;
            indoProLixo = false;
            jaAmassou = false; // Reseta quando entra em contato com um novo item
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
                jaAmassou = false;
            }
        }
    }

    void Update()
    {
        if (indoProLixo && massa != null)
        {
            massa.transform.Translate(0f, 0f, -0.3f); 
            return;     
        }
        
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame && massa != null) 
        {
            indoProLixo = true;
            Debug.Log("jogou no lixo!");
        }
        
        // Só amassa se a massa existir E ainda não tiver sido amassada nesta interação
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && massa != null && !jaAmassou)
        {
            massa.transform.localScale -= new Vector3(0f, 0.05f, 0f);
            massa.transform.position -= new Vector3(0f, 0f, 0.05f);

            jaAmassou = true; // Trava para não poder amassar de novo
            Debug.Log("amassou!");
        }
    }
}