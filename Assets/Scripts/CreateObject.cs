using UnityEngine;

public class CreateObject : MonoBehaviour
{
    public GameObject vermelho, azul;

    public Transform spawn;

    public float[] tempoVermelho;

    public float[] tempoAzul;  
    private float tempo = 0f;
    private int indiceSpawnVermelho, indiceSpawnAzul;

    // Update is called once per frame
    void Update()
    {
        tempo += Time.deltaTime;
        if(indiceSpawnVermelho < tempoVermelho.Length && tempo >= tempoVermelho[indiceSpawnVermelho])
        {
            GameObject novoVermelho = Instantiate(vermelho, spawn.position, spawn.rotation);
            Destroy(novoVermelho, 10f);
            indiceSpawnVermelho++;
        }
        if(indiceSpawnAzul < tempoAzul.Length && tempo >= tempoAzul[indiceSpawnAzul])
        {
            GameObject novoAzul = Instantiate(azul, spawn.position, spawn.rotation);
            Destroy(novoAzul, 10f);
            indiceSpawnAzul++;
        }
    }
}