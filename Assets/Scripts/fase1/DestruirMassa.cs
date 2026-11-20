using UnityEngine;

public class DestruirMassa : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("MassaVermelha") ||
            collision.gameObject.CompareTag("MassaAzul"))
        {
            Destroy(collision.gameObject);
        }
    }
}