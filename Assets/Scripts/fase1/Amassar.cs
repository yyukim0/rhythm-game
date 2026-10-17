using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections.Generic;

public class Amassar : MonoBehaviour
{

    // GERENCIAMENTO DAS MASSAS PARA CONTABILIZAR A PERDA DE PONTOS CASO UMA NÃO SEJA AMASSADA
    private List<GameObject> massasNoTrigger = new List<GameObject>();
    private List<GameObject> massasIndoProLixo= new List<GameObject>();
    private HashSet<GameObject> massasAmassadas = new HashSet<GameObject>();

    public TMP_Text txtPontuacao;

    public int pontuacao = 0;

    void Start() {
        pontuacao = 0;
    }

    private void OnTriggerEnter(Collider bater)
    {
        if (bater.CompareTag("MassaVermelha") || bater.CompareTag("MassaAzul"))
        {
            if(!massasNoTrigger.Contains(bater.gameObject)) {
                massasNoTrigger.Add(bater.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider bater)
    {
        if (bater.CompareTag("MassaVermelha") || bater.CompareTag("MassaAzul"))
        {
            GameObject massaSaindo = bater.gameObject;

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
        txtPontuacao.text = pontuacao.ToString();
        
        for (int i = massasIndoProLixo.Count - 1; i >= 0; i--) {
            GameObject massas = massasIndoProLixo[i];
            if (massas != null) {
                massas.transform.Translate(0.01f, 0f, -0.05f);
            }
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
                
                massasIndoProLixo.Add(massaAtual);
                massasNoTrigger.Remove(massaAtual);
                
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
}