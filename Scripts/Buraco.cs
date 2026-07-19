using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 


public class Buraco : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se o objeto que entrou na trigger tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // Recarrega a cena atual (reinicia a fase)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
