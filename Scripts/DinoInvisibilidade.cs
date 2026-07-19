using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class DinoInvisibilidade : MonoBehaviour
{
    public float visibilidadeForaDaLuz = 0.1f; // Quase invisível fora da luz
    public float visibilidadeNaLuz = 1.0f;     // Visível quando dentro da luz

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetAlpha(visibilidadeNaLuz); // Começa visível
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Luz"))
        {
            // Entrou na luz -> Fica visível
            SetAlpha(visibilidadeNaLuz);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Luz"))
        {
            // Saiu da luz -> Fica quase invisível
            SetAlpha(visibilidadeForaDaLuz);
        }
    }

    void SetAlpha(float alpha)
    {
        Color cor = spriteRenderer.color;
        cor.a = alpha;
        spriteRenderer.color = cor;
    }
}