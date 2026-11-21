using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class Amassar : MonoBehaviour
{

    // GERENCIAMENTO DAS MASSAS PARA CONTABILIZAR A PERDA DE PONTOS CASO UMA NÃO SEJA AMASSADA
    private List<GameObject> massasNoTrigger = new List<GameObject>();
    private List<GameObject> massasIndoProLixo= new List<GameObject>();
    private HashSet<GameObject> massasAmassadas = new HashSet<GameObject>();

    public TMP_Text txtPontuacao;

    public int pontuacao = 0;
    private float tempoDeFase = 0f;
    private bool faseEncerrada = false;

    private Animator animator;

    void Start() {
        pontuacao = 0;

        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MassaVermelha") || other.CompareTag("MassaAzul"))
        {
            if(!massasNoTrigger.Contains(other.gameObject)) {
                massasNoTrigger.Add(other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MassaVermelha") || other.CompareTag("MassaAzul"))
        {
            GameObject massaSaindo = other.gameObject;

            if(massasNoTrigger.Contains(massaSaindo)) {
                bool foiAmassada = massasAmassadas.Contains(massaSaindo);
                bool foiProLixo = massasIndoProLixo.Contains(massaSaindo);
                if (!foiAmassada && !foiProLixo)
                {
                    pontuacao -= 50;
                }

            massasNoTrigger.Remove(massaSaindo);
            massasAmassadas.Remove(massaSaindo);
            massasIndoProLixo.Remove(massaSaindo);
            }
        }
    }

    void Update()
    {
        if (!faseEncerrada)
        {
            tempoDeFase += Time.deltaTime;
        }
        if (tempoDeFase >= 10f && !faseEncerrada)
        {
            StartCoroutine(EncerrarFase());
        }
     
        txtPontuacao.text = pontuacao.ToString();
        
        for (int i = massasIndoProLixo.Count - 1; i >= 0; i--)
        {
            GameObject massas = massasIndoProLixo[i];

            if (massas != null)
            {
                Rigidbody rb = massas.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.MovePosition(rb.position + Vector3.back * 0.07f);
                }
            }
        }

        if (faseEncerrada)
        {
            return;
        }

        GameObject massaAtual = GetMassaAtiva();
        
        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame && massaAtual != null) 
        {
            if(massaAtual != null) {

                if(massaAtual.CompareTag("MassaAzul")) {
                    pontuacao += 100;   
                }
                if(massaAtual.CompareTag("MassaVermelha")) {
                    pontuacao -= 50;
                }
                
                if (!massasIndoProLixo.Contains(massaAtual))
                {
                    massasIndoProLixo.Add(massaAtual);
                    massasNoTrigger.Remove(massaAtual);
                }
                
                Debug.Log("jogou no lixo!");
            }
            else { //verifica se clicou no vazio
                pontuacao -= 50;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && massaAtual == null) // clicou com o esquerdo no vazio
        {
            pontuacao -= 50;
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && massaAtual != null)
        {
            if (!massasAmassadas.Contains(massaAtual)) {
                animator.SetTrigger("Amassar");
                massaAtual.transform.localScale -= new Vector3(0f, 0.05f, 0f);
                massaAtual.transform.position -= new Vector3(0f, 0f, 0.05f);

                massasAmassadas.Add(massaAtual);

                if(massaAtual.CompareTag("MassaVermelha")) {
                    pontuacao += 100;   
                }
                if(massaAtual.CompareTag("MassaAzul")) {
                    pontuacao -= 50;
                }

                Debug.Log("amassou!");
            }
        }
    }

    private GameObject GetMassaAtiva() {
        foreach (GameObject m in massasNoTrigger) {
            if (m != null && !massasIndoProLixo.Contains(m)) {
                return m;
            }
        }
        return null;
    }

    private IEnumerator EncerrarFase()
    {
        faseEncerrada = true;

        yield return new WaitForSeconds(5f);

        if (pontuacao >= 1000)
        {
            animator.SetTrigger("Winning");
        }
        else
        {
            animator.SetTrigger("Losing");
        }
    }
}