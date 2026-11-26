using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems; // necessário para seleção

public class MenuPrincipalManager : MonoBehaviour
{
    [SerializeField] private string nomeDoLevelDeJogo;
    [SerializeField] private GameObject painelMenuInicial;
    [SerializeField] private GameObject painelCreditos;
    [SerializeField] private GameObject painelControles;

    [Header("Botões padrão para seleção")]
    [SerializeField] private GameObject botaoInicialMenu; // ex: botão Jogar
    [SerializeField] private GameObject botaoVoltarCreditos;
    [SerializeField] private GameObject botaoVoltarControles;

    void Start()
    {
        // Seleciona o botão padrão no menu principal ao iniciar
        EventSystem.current.SetSelectedGameObject(botaoInicialMenu);
    }

    public void Jogar()
    {
        SceneManager.LoadScene(nomeDoLevelDeJogo);
    }

    public void AbrirCreditos()
    {
        painelMenuInicial.SetActive(false);
        painelCreditos.SetActive(true);

        EventSystem.current.SetSelectedGameObject(botaoVoltarCreditos);
    }

    public void FecharCreditos()
    {
        painelCreditos.SetActive(false);
        painelMenuInicial.SetActive(true);

        EventSystem.current.SetSelectedGameObject(botaoInicialMenu);
    }

    public void AbrirControles()
    {
        painelMenuInicial.SetActive(false);
        painelControles.SetActive(true);

        EventSystem.current.SetSelectedGameObject(botaoVoltarControles);
    }

    public void FecharControles()
    {
        painelControles.SetActive(false);
        painelMenuInicial.SetActive(true);

        EventSystem.current.SetSelectedGameObject(botaoInicialMenu);
    }
}
