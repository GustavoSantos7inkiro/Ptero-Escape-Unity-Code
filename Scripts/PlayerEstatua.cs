using System.Collections;
using System.Collections.Generic;
  using UnityEngine;


public class PlayerEstatua : MonoBehaviour
{
    public string[] estatuaAnimNames = { "estatua1", "estatua2", "estatua3", "estatua4" }; // Nomes das animações de estátua
    public bool isEstatua = false; // Indica se o player está em modo estátua

    private Rigidbody2D rig; // Rigidbody do player
    private Animator animator; // Animator para trocar animações
    private bool aguardandoPouso = false; // Espera o player pousar para ativar estátua
    private Coroutine estatuaCoroutine; // Referência da coroutine ativa

    [SerializeField] private LayerMask layerChao; // Layer que representa o chão
    [SerializeField] private Transform checagemChao; // Posição para verificar se está no chão
    [SerializeField] private float raioChao = 0.2f; // Raio para checagem de chão

    private Player player; // Referência ao script Player principal

    void Start()
    {
        rig = GetComponent<Rigidbody2D>(); // Pega Rigidbody2D
        animator = GetComponent<Animator>(); // Pega Animator
        player = GetComponent<Player>(); // Pega script Player
        isEstatua = false; // Começa fora do modo estátua
        rig.bodyType = RigidbodyType2D.Dynamic; // Define corpo dinâmico para movimentação
    }

    void Update()
    {
        // Verifica se está no chão usando OverlapCircle na posição checagemChao
        bool estaNoChao = Physics2D.OverlapCircle(checagemChao.position, raioChao, layerChao);

        if (InputModoEstatua()) // Se o botão para virar estátua foi pressionado
        {
            
            if (!isEstatua && !player.IsPendurado)
            {
                //  Impede virar estátua no ar
                if (!estaNoChao)
                {
                    Debug.Log("Não pode virar estátua no ar.");
                    return;
                }

                TocarAnimacaoEstatuaAleatoria();
                gameObject.layer = LayerMask.NameToLayer("Estatua");
                EntrarModoEstatua();
            }
            else if (isEstatua)
            {
                SairModoEstatua(); // Sai manualmente do modo estátua se já estiver ativo
            }
        }
    }
    // Ativa o modo estátua
    private void EntrarModoEstatua()
    {
        rig.velocity = Vector2.zero; // Para o movimento zerando a velocidade
        rig.bodyType = RigidbodyType2D.Static; // Torna o corpo estático (sem física)
        isEstatua = true; // Define que está em modo estátua
        estatuaCoroutine = StartCoroutine(SairModoEstatuaDepoisDe5Segundos()); // Coroutine para sair do modo estátua automaticamente
    }

    // Coroutine que sai do modo estátua após 5,5 segundos
    private IEnumerator SairModoEstatuaDepoisDe5Segundos()
    {
        animator.speed = 1f; // Garante velocidade normal da animação
        yield return new WaitForSeconds(5.5f); // Espera 5,5 segundos
        SairModoEstatua(); // Sai do modo estátua automaticamente
    }

    // Sai do modo estátua, manual ou automático
    private void SairModoEstatua()
    {
        if (estatuaCoroutine != null)
        {
            StopCoroutine(estatuaCoroutine); // Para a coroutine ativa, se houver
            estatuaCoroutine = null;
        }

        rig.bodyType = RigidbodyType2D.Dynamic; // Volta a ser dinâmico para poder se mover
        isEstatua = false; // Sai do estado de estátua
        aguardandoPouso = false; // Cancela espera por pouso, se ainda estivesse esperando
        gameObject.layer = LayerMask.NameToLayer("Player"); // Retorna à layer padrão do player
        animator.Play("player_idle"); // Volta para animação idle
        Debug.Log("Saiu do modo estátua (automático ou manual)");
    }

    // Toca uma animação de estátua aleatória da lista
    private void TocarAnimacaoEstatuaAleatoria()
    {
        int poseIndex = Random.Range(0, estatuaAnimNames.Length); // Escolhe pose aleatória
        animator.Play(estatuaAnimNames[poseIndex], 0, 0f); // Toca animação escolhida
    }

    
    private bool InputModoEstatua()
    {
        return Input.GetKeyDown(KeyCode.S) || Input.GetButtonDown("EstatuaJoystick");
    }

    // Desenha no editor a área de checagem do chão
    private void OnDrawGizmosSelected()
    {
        if (checagemChao != null)
        {
            Gizmos.color = Color.red; // Cor vermelha para o gizmo
            Gizmos.DrawWireSphere(checagemChao.position, raioChao); // Desenha a esfera de verificação
        }
    }
}
