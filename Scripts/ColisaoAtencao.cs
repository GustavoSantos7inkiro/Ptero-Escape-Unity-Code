using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class ColisaoAtencao : MonoBehaviour
{
    [Header("Referências")]
    public Inimigo inimigo; // Referência ao script do inimigo
    public PlayerEstatua playerEstatua; // Para verificar se o jogador está em modo estátua

    [Header("Configurações")]
    [SerializeField] private float tempoMinimoAtencao = 6f; // Tempo mínimo que o inimigo fica atento

    private float tempoRestante = 0f; // Contador regressivo para desativar o modo atento
    private bool jogadorDentro = false; // Flag para saber se o jogador está dentro da zona de atenção

    private void Update()
    {
        // Se o jogador está dentro da zona e o inimigo não está hostil
        if (jogadorDentro && !inimigo.estaHostil)
        {
            AtivarAtencao(); // Ativa o modo atento e reinicia o tempo
        }
        // Se inimigo está atento, mas o jogador saiu ou está em estátua, decrementa tempo para desativar
        else if (inimigo.estaAtento)
        {
            tempoRestante -= Time.deltaTime; // Decrementa o tempo restante
            if (tempoRestante <= 0f)
            {
                DesativarAtencao(); // Desativa o modo atento após o tempo acabar
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se quem entrou é o jogador
        if (other.CompareTag("Player"))
        {
            playerEstatua = other.GetComponent<PlayerEstatua>(); // Obtém o componente PlayerEstatua
            jogadorDentro = true; // Marca que o jogador entrou na área

            // Se o jogador não está em modo estátua e o inimigo não está atento ou hostil, ativa atenção imediatamente
            if (playerEstatua != null && !playerEstatua.isEstatua && !inimigo.estaAtento && !inimigo.estaHostil)
            {
                inimigo.AtivarModoAtento();
                Debug.Log("Entrou no modo atento");
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

    // Método para ativar o modo atento
    private void AtivarAtencao()
    {
        tempoRestante = tempoMinimoAtencao; // Reinicia o timer do modo atento
        if (!inimigo.estaAtento)
        {
            inimigo.AtivarModoAtento(); // Ativa o modo atento no inimigo
            Debug.Log("Entrou no modo atento"); // Log para debug
        }
    }

    // Método para desativar o modo atento
    private void DesativarAtencao()
    {
        inimigo.DesativarModoAtento(); // Desativa o modo atento no inimigo
        Debug.Log("Saiu do modo atento"); // Log para debug
    }
}