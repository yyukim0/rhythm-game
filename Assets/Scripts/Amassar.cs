using UnityEngine;
using UnityEngine.InputSystem;

public class Amassar : MonoBehaviour
{
    private GameObject massa;

    private void OnTriggerEnter(Collider bater)
    {
        if (bater.CompareTag("Massa"))
        {
            massa = bater.gameObject;
        }
    }

    private void OnTriggerExit(Collider bater)
    {
        if (bater.CompareTag("Massa"))
        {
            if (bater.gameObject == massa)
            {
                massa = null;
            }
        }
    }

    void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && massa != null)
        {
            massa.transform.localScale -= new Vector3(0f, 0.05f, 0f);
            massa.transform.position -= new Vector3(0f, 0f, 0.05f);

            Debug.Log("amassou!");
        }
    }
}