using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColisaoHostil : MonoBehaviour
{
    [Header("Referências")]
    public Inimigo inimigo; // Referência ao script do inimigo
    public PlayerEstatua playerEstatua; // Referência para checar se o jogador está em modo estátua

    [Header("Configurações")]
    [SerializeField] private float tempoMinimoHostil = 9f; // Tempo mínimo em segundos para o modo hostil ficar ativo

    private float tempoRestante = 0f; // Contador regressivo para desativar o modo hostil
    private bool jogadorDentro = false; // Flag para saber se o jogador está dentro da área de colisão

    private void Update()
    {
        // Se o jogador está dentro da área e o inimigo ainda não está hostil
        if (jogadorDentro && !inimigo.estaHostil)
        {
            AtivarHostilidade(); // Ativa o modo hostil e reinicia o tempo
        }
        // Se o inimigo já está hostil, decrementa o tempo restante para desativar
        else if (inimigo.estaHostil)
        {
            tempoRestante -= Time.deltaTime; // Decrementa o tempo com o delta do frame
            if (tempoRestante <= 0f)
            {
                DesativarHostilidade(); // Desativa o modo hostil quando o tempo acaba
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem entrou é o jogador
        if (other.CompareTag("Player"))
        {
            playerEstatua = other.GetComponent<PlayerEstatua>(); // Pega o componente PlayerEstatua
            // Só marca que o jogador está dentro se ele não estiver no modo estátua
            if (playerEstatua != null && !playerEstatua.isEstatua)
            {
                jogadorDentro = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Quando o jogador sai da área, marca que não está mais dentro
        if (other.CompareTag("Player"))
        {
            jogadorDentro = false;
        }
    }

    // Método para ativar o modo hostil no inimigo
    private void AtivarHostilidade()
    {
        tempoRestante = tempoMinimoHostil; // Reinicia o timer do modo hostil
        inimigo.AtivarModoHostil(); // Ativa o modo hostil no script Inimigo
        Debug.Log("Inimigo entrou em modo hostil"); // Log para debug
    }

    // Método para desativar o modo hostil no inimigo
    private void DesativarHostilidade()
    {
        inimigo.DesativarModoHostil(); // Desativa o modo hostil no script Inimigo
        Debug.Log("Inimigo saiu do modo hostil"); // Log para debug
    }
}
