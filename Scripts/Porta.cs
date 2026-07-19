using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement; 
using UnityEngine; 
using System; 

public class Porta : MonoBehaviour
{
    [SerializeField] private string nomeProximaFase; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se o objeto que colidiu com a porta tem a tag "Player"
        if (collision.CompareTag("Player"))
        {
            // Chama o método para carregar a próxima fase, se o jogador colidir com a porta
            IrProximaFase();
        }
    }

    private void IrProximaFase()
    {
        // Carrega a próxima cena usando o nome da fase fornecido na variável 'nomeProximaFase'
        SceneManager.LoadScene(this.nomeProximaFase);
    }
}
