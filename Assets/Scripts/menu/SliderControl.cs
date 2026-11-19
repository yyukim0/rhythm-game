using UnityEngine;
using UnityEngine.UI;

public class SliderControl : MonoBehaviour
{
    public Slider slider;
    public RectTransform fill;
    public RectTransform area; // arraste o Fill Area aqui

    void Start()
    {
        slider.onValueChanged.AddListener(Atualizar);
        Atualizar(slider.normalizedValue);
    }

    void Atualizar(float _)
    {
        float t = Mathf.Clamp01(slider.normalizedValue);

        float larguraBarra = area.rect.width;
        float larguraFill = fill.rect.width;
        float diagonal = larguraFill - larguraBarra;

        // 0%: ponta direita do verde (a diagonal) está na borda esquerda da barra
        float posInicial = -larguraBarra;
        // 100%: ponta direita do verde está na borda direita da barra
        float posFinal = 0f;

        float x = Mathf.Lerp(posInicial, posFinal, t);
        fill.anchoredPosition = new Vector2(x, 0f);
    }
}