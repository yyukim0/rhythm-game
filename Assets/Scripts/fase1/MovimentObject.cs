using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    private float tempoDeVida = 0f;
    private bool mudouDeDirecao = false;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tempoDeVida += Time.fixedDeltaTime;

        if (tempoDeVida < 6f)
        {
            rb.MovePosition(rb.position + transform.right * -0.01f);
        }
        else
        {
            rb.MovePosition(rb.position + Vector3.back * 0.01f);
        }
    }
}
