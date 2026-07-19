using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inimigo : MonoBehaviour
{
    //  Componentes do inimigo
    private Rigidbody2D rig;                     
    private AudioSource audioSource;             

    //  Movimento
    [SerializeField] private float speed;        // Velocidade de movimentação atual do inimigo
    private float velocidadeOriginal;            // Velocidade base do inimigo, usada para restaurar depois

    //  Colisão
    [SerializeField] private Transform rightCol; 
    [SerializeField] private Transform leftCol;  
    private bool colliding;                      // Booleano que indica se o inimigo colidiu com uma parede
    [SerializeField] private LayerMask layer;    // Layer que representa os obstáculos que o inimigo pode colidir

    //  Áudio
    [SerializeField] private AudioClip grunhido;        
    [SerializeField] private AudioClip grunhidoHostil;  

    //  Estados
    public bool estaAtento = false;              // Indica se o inimigo está no modo "atento"
    public bool estaHostil = false;              // Indica se o inimigo está no modo "hostil"

    //  Referências externas
    [SerializeField] private PlayerEstatua playerEstatua; // Referência ao script do jogador para checar se ele está em modo estátua

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();       // Pega o componente Rigidbody2D no início
        audioSource = GetComponent<AudioSource>(); // Pega o componente AudioSource no início
        velocidadeOriginal = speed;              // Armazena a velocidade original do inimigo
    }

    void Update()
    {
        // Se o jogador estiver em modo estátua, o inimigo não faz nada
        if (playerEstatua != null && playerEstatua.isEstatua)
            return;

        // Move o inimigo na direção atual
        rig.velocity = new Vector2(speed, rig.velocity.y);

        // Verifica colisão entre os pontos leftCol e rightCol com a camada especificada
        colliding = Physics2D.Linecast(rightCol.position, leftCol.position, layer);

        // Se houver colisão, vira o inimigo para o outro lado
        if (colliding)
            Virar();
    }

    public void AtivarModoAtento()
    {
        // Se já estiver atento, não faz nada
        if (estaAtento) return;

        estaAtento = true;           // Ativa o estado atento
        speed *= 0.9f;               // Reduz a velocidade em 10%
        TocarSom(grunhido);          // Toca o som do modo atento
    }

    public void DesativarModoAtento()
    {
        estaAtento = false;          // Desativa o estado atento
        RestaurarVelocidade();       // Restaura a velocidade original

        // Garante que a direção da velocidade e do scale estão coerentes
        if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(speed))
            Virar();
    }

    public void AtivarModoHostil()
    {
        // Se já estiver hostil, não faz nada
        if (estaHostil) return;

        estaHostil = true;           // Ativa o estado hostil
        speed *= 1.3f;               // Aumenta a velocidade em 30%
        TocarSom(grunhidoHostil);    // Toca o som do modo hostil
    }

    public void DesativarModoHostil()
    {
        estaHostil = false;          // Desativa o estado hostil
        RestaurarVelocidade();       // Restaura a velocidade original
    }

    private void Virar()
    {
        // Inverte a escala no eixo X para virar o sprite horizontalmente
        Vector2 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;

        // Inverte o sinal da velocidade para mudar direção
        speed *= -1f;
    }

    private void RestaurarVelocidade()
    {
        // Restaura a velocidade original respeitando a direção atual
        speed = Mathf.Sign(transform.localScale.x) * velocidadeOriginal;
    }

    private void TocarSom(AudioClip clip)
    {
        // Toca o som fornecido, se houver um AudioSource e um clip válido
        if (audioSource && clip)
            audioSource.PlayOneShot(clip);
    }
}