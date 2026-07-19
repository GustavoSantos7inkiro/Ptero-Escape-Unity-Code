using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MonoBehaviour
{


    [Header("Stamina UI")]
    public Image staminaFillImage;
    // Velocidade padrão do jogador
    [SerializeField] private float speed = 3.2f;

    // Força do pulo
    [SerializeField] private float jumpForce = 10f;

    // Raio de checagem para saber se está no chão
    [SerializeField] private float groundCheckRadius = 0.2f;

    // Camada que representa o chão
    [SerializeField] private LayerMask groundLayer;

    // Referência ao script PlayerEstatua
    [SerializeField] private PlayerEstatua playerEstatua;

    // Ponto onde o jogador será posicionado após subir na estante
    [SerializeField] private Transform pontoTopoEstante;

    // Fonte de áudio e efeitos sonoros
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip runFastClip;
    [SerializeField] private AudioClip gritoClip;

    // Componentes principais
    private Rigidbody2D rb;
    private Animator anim;

    // Flags de controle
    private bool isJumping;
    private bool isPendurado;

    // Propriedade pública para acessar isPendurado externamente
    public bool IsPendurado => isPendurado;

    private bool runFast;
    private bool runFastSoundPlayed;

    

    // Valores auxiliares
    private float speedOriginal;
    private float runFastStartTime;
    private float lastRunKeyTime;
    private float lastRightArrowTime;
    private float lastLeftArrowTime;

    // Limite de tempo para detectar duplo clique
    private float doubleTapThreshold = 0.3f;

    // Direção no momento do pulo
    private float jumpDirection;

    // Flags para controle de duplo toque no joystick
    private bool wasJoystickRight;
    private bool wasJoystickLeft;

    private bool isGrounded;


    [Header("Estamina")]
    public float maxStamina = 100f;
    public float currentStamina;
    public float staminaDrainDuration = 3f;  // segundos para drenar tudo
    public float staminaRecoverDuration = 5f; // segundos para recuperar tudo

    private float staminaDrainRate;
    private float staminaRecoverRate;

    private bool canRunFast = true; // controlará se o jogador pode correr



    public RectTransform staminaBarRectTransform; // Referência à barra no Canvas UI
    public Vector3 offset = new Vector3(0, 50f, 0); // Offset em pixels na tela
    private Camera mainCamera;


    public GameObject staminaBarObject;

    private void Awake()
    {


        

        mainCamera = Camera.main;

        currentStamina = maxStamina;
        staminaDrainRate = maxStamina / staminaDrainDuration;
        staminaRecoverRate = maxStamina / staminaRecoverDuration;
        // Inicializa componentes
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        speedOriginal = speed;


        staminaDrainRate = maxStamina / staminaDrainDuration;     // quanto stamina perde por segundo
        staminaRecoverRate = maxStamina / staminaRecoverDuration; // quanto stamina recupera por segundo
        currentStamina = maxStamina;

        // Garante referência ao script PlayerEstatua
        if (playerEstatua == null)
            playerEstatua = GetComponent<PlayerEstatua>();

        // Garante referência ao AudioSource
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }



    private void Update()
    {
         UpdateStaminaBarPosition();

        if (staminaBarRectTransform != null && mainCamera != null)
        {
            Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);
            staminaBarRectTransform.position = screenPos + offset;
        }

        staminaFillImage.fillAmount = currentStamina / maxStamina;

        HandleStamina();
        // Verifica se está no chão
        CheckGround();

        // Se está pendurado, executa o comportamento de pendurado e retorna
        if (isPendurado)
        {
            HandlePendurado();
            return;
        }

        // Se não está em modo estátua, permite pular
        if (!playerEstatua.isEstatua)
            HandleJumpInput();

        // Captura entrada de movimento
        HandleMovementInput();

        // Atualiza animações
        HandleAnimations();
        
        
    }

    

    private void FixedUpdate()
    {
        // Movimento aplicado na física (FixedUpdate)
        if (!playerEstatua.isEstatua && !isPendurado)
            Move();
    }

    private void CheckGround()
    {
        // Verifica se o jogador está tocando o chão usando OverlapCircle
        isGrounded = Physics2D.OverlapCircle(transform.position, groundCheckRadius, groundLayer);

        // Reseta isJumping caso esteja no chão
        if (isGrounded && isJumping)
            isJumping = false;
    }

   void UpdateStaminaBarPosition()
{
    if (staminaBarRectTransform == null || mainCamera == null)
        return;

    Vector3 screenPos = mainCamera.WorldToScreenPoint(transform.position);

    // Se o player estiver atrás da câmera, escondemos a barra
    if (screenPos.z < 0)
    {
        staminaBarRectTransform.gameObject.SetActive(false);
        return;
    }
    else
    {
        staminaBarRectTransform.gameObject.SetActive(true);
    }

    staminaBarRectTransform.position = screenPos + offset;
}




    private void HandlePendurado()
    {
        // Anula o movimento e a gravidade ao ficar pendurado
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0f;

        // Define animações
        anim.SetBool("run", false);
        anim.SetBool("run_fast", false);
        anim.SetBool("jump", false);
        anim.SetBool("isPendurado", true);

        // Verifica se a animação "Pendurado" terminou
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Pendurado") && stateInfo.normalizedTime >= 1f)
        {
            isPendurado = false;
            anim.SetBool("isPendurado", false);

            // Muda para o topo da estante, mas levemente acima para cair naturalmente
            Vector3 pos = pontoTopoEstante.position;
            transform.position = new Vector3(pos.x, pos.y + 0.2f, pos.z);

            // Restaura a gravidade e permite que o corpo caia suavemente
            rb.gravityScale = 1.6f;
            rb.velocity = new Vector2(0f, -1f); // movimento levemente para baixo
            isJumping = false;
        }
    }
    

    private void HandleStamina()
{
    if (runFast && currentStamina > 0f)
    {
        currentStamina -= staminaDrainRate * Time.deltaTime;
        if (currentStamina <= 0f)
        {
            currentStamina = 0f;
            canRunFast = false;
            runFast = false;  // força parar corrida
            Debug.Log("[STAMINA] Estamina zerada, corrida parada.");
        }
    }
    else
    {
        currentStamina += staminaRecoverRate * Time.deltaTime;
        if (currentStamina >= maxStamina)
        {
            currentStamina = maxStamina;
            canRunFast = true;
        }
    }
}

    private void HandleJumpInput()
    {
        // Pula ao pressionar o botão, se não estiver pulando
        if ((Input.GetButtonDown("JumpJoystick") || Input.GetKeyDown(KeyCode.Space)) && !isJumping)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isJumping = true;

            // Captura direção horizontal no momento do pulo
            float inputJoystick = Input.GetAxisRaw("HorizontalJoystick");
            float inputKeyboard = Input.GetKey(KeyCode.RightArrow) ? 1f : (Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f);
            float inputX = inputJoystick != 0f ? inputJoystick : inputKeyboard;

            jumpDirection = (inputX != 0f) ? Mathf.Sign(inputX) : 999f;
        }
    }

  private void HandleMovementInput()
{
    float inputJoystick = Input.GetAxisRaw("HorizontalJoystick");
    float inputKeyboard = Input.GetAxisRaw("Horizontal");

    // Se o teclado estiver sendo pressionado, usa ele. Caso contrário, usa o joystick
    float inputX = inputKeyboard != 0f ? inputKeyboard : inputJoystick;

    bool isMoving = Mathf.Abs(inputX) > 0.1f;

    Debug.Log($"[MOVIMENTO] InputJoystick: {inputJoystick}, InputKeyboard: {inputKeyboard}, Usado: {inputX}");

    bool isRunButtonHeld = Input.GetButton("RunJoystick") || Input.GetKey(KeyCode.Z);

    // 👉 Mostra ou oculta a barra de estamina com base no botão de correr
    if (staminaBarObject != null)
    staminaBarObject.SetActive(isRunButtonHeld && !playerEstatua.isEstatua && !isPendurado);

    // Só ativa corrida se puder correr
    if (isRunButtonHeld && isMoving && canRunFast)
    {
        if (!runFast)
        {
            runFast = true;
            runFastStartTime = Time.time;
            Debug.Log("[RUN FAST] Corrida rápida iniciada (tecla segurada).");
        }
        else if (Time.time - runFastStartTime > 3f)
        {
            runFast = false;
            Debug.Log("[RUN FAST] Corrida rápida encerrada (tempo esgotado).");
        }
    }
    else
    {
        if (runFast)
            Debug.Log("[RUN FAST] Corrida rápida desativada (tecla solta, parado ou sem estamina).");

        runFast = false;
    }

    speed = runFast ? speedOriginal * 1.5f : speedOriginal;
}

    private void CheckDoubleTapJoystick(ref float lastTapTime, ref bool wasPressed)
    {
        // Verifica duplo toque com joystick (direita ou esquerda)
        if (!wasPressed)
        {
            if (Time.time - lastTapTime < doubleTapThreshold)
            {
                runFast = true;
                runFastStartTime = Time.time;
            }
            lastTapTime = Time.time;
        }
        wasPressed = true;
    }

 private void Move()
{
    float inputJoystick = Input.GetAxisRaw("HorizontalJoystick");
    float inputKeyboard = Input.GetKey(KeyCode.RightArrow) ? 1f : (Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f);
    float inputX = inputJoystick != 0f ? inputJoystick : inputKeyboard;

    Debug.Log($"[MOVE] inputJoystick: {inputJoystick}, inputKeyboard: {inputKeyboard}, usado: {inputX}");

    if (isJumping && jumpDirection == 999f)
        inputX = 0f;
    else if (isJumping && Mathf.Sign(inputX) != jumpDirection && inputX != 0f)
        inputX = 0f;

    if (inputX != 0f)
    {
        rb.velocity = new Vector2(inputX * speed, rb.velocity.y);
        Debug.Log($"[MOVE] Movendo com velocidade {rb.velocity.x}");

        if (!isJumping)
            transform.eulerAngles = inputX > 0 ? Vector3.zero : new Vector3(0f, 180f, 0f);
    }
    else
    {
        rb.velocity = new Vector2(0f, rb.velocity.y);
        Debug.Log("[MOVE] Parado.");

        if (!isJumping)
        {
            anim.SetBool("run", false);
            anim.SetBool("run_fast", false);
        }
    }
}
    
  private void HandleAnimations()
{
    anim.SetBool("jump", isJumping);

    if (isJumping)
    {
        anim.SetBool("run", false);
        anim.SetBool("run_fast", false);
        runFastSoundPlayed = false;

        if (audioSource.isPlaying)
            audioSource.Stop();

        Debug.Log("[ANIMAÇÃO] Pulo ativado.");
        return;
    }

    // Corrigido: entrada combinada de teclado e joystick
    float inputJoystick = Input.GetAxisRaw("HorizontalJoystick");
    float inputKeyboard = Input.GetKey(KeyCode.RightArrow) ? 1f : (Input.GetKey(KeyCode.LeftArrow) ? -1f : 0f);
    float inputX = inputKeyboard != 0f ? inputKeyboard : inputJoystick;

    bool isMoving = Mathf.Abs(inputX) > 0.1f;

    if (runFast && isMoving)
    {
        anim.SetBool("run_fast", true);
        anim.SetBool("run", false);

        if (!audioSource.isPlaying && !runFastSoundPlayed)
        {
            audioSource.clip = runFastClip;
            audioSource.Play();
            runFastSoundPlayed = true;

            Debug.Log("[SOM] Corrida rápida som tocado.");
        }

        Debug.Log("[ANIMAÇÃO] Correndo rápido.");
    }
    else
    {
        anim.SetBool("run_fast", false);

        if (audioSource.isPlaying)
            audioSource.Stop();

        runFastSoundPlayed = false;
        anim.SetBool("run", isMoving);

        Debug.Log($"[ANIMAÇÃO] Correndo normal: {isMoving}");
    }

    anim.SetBool("walk_slow", isMoving && !runFast && !isJumping);
}

   private void OnTriggerEnter2D(Collider2D collision)
{
    // Se entrou na área de pendurar
    if (collision.CompareTag("Pendurado"))
    {
        TriggerPendurado trigger = collision.GetComponent<TriggerPendurado>();
        if (trigger != null)
        {
            pontoTopoEstante = trigger.topoEstante;
            isPendurado = true;
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
            anim.SetBool("isPendurado", true);

            // Esconde a barra de estamina
            if (staminaBarObject != null)
                staminaBarObject.SetActive(false);
        }
    }
}

    private void OnTriggerExit2D(Collider2D collision)
{
    // Ao sair da área de pendurar, restaura a gravidade
    if (collision.CompareTag("Pendurado"))
    {
        rb.gravityScale = 1.6f;
        isPendurado = false;
        anim.SetBool("isPendurado", false);

        // Mostra a barra de estamina de volta
        if (staminaBarObject != null)
            staminaBarObject.SetActive(true);
    }
}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Se bateu no teto da estante durante o pulo
        if (collision.gameObject.CompareTag("TetoEstante") && isJumping)
        {
            isJumping = false;
            anim.SetBool("jump", false);
            jumpDirection = 0f;
            rb.velocity = Vector2.zero;
        }

        // Se colidiu com parede sem estar pulando
        if (collision.gameObject.CompareTag("Wall") && !isJumping)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            anim.SetBool("run", false);
            anim.SetBool("run_fast", false);
        }

        // Se colidiu com o chão
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isJumping = false;
            anim.SetBool("jump", false);

            // Define se deve correr ou ficar parado
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                anim.SetBool("run", true);
                anim.SetBool("run_fast", runFast);
            }
            else
            {
                anim.SetBool("run", false);
            }
            jumpDirection = 0f;
        }

        // Se colidiu com inimigo, recarrega a cena
        if (collision.gameObject.CompareTag("Enemy"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            
        }
    }
}