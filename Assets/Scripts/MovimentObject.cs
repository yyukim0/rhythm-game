using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private float tempoDeVida = 0f;
    private bool mudouDeDirecao = false;

    void Update()
    {
        tempoDeVida += Time.deltaTime;

        if (tempoDeVida < 5f)
        {
            transform.Translate(-0.01f, 0f, 0f); 
        }
        else
        {
            transform.Translate(0f, 0f, -0.01f); 
        }
    }
}
