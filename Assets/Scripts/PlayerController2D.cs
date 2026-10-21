using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float speed = 2f; 
    private Rigidbody2D rb;
    
    private Animator anim;
    private SpriteRenderer spriteRenderer; 
    
    private float moveDirection;

    [Header("Controles")]
    public InputAction moveAction;

    [Header("Cansaço Físico")]
    [Tooltip("Quanto a barra de preguiça aumenta por segundo enquanto o player caminha sem objetivo")]
    public float walkPenaltyPerSecond = 8f; 

    private void OnEnable()
    {
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true; 
        
        // Pega os componentes que estão no mesmo objeto do Player
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Lê o input como um número flutuante (-1 para esquerda, 1 para direita, 0 parado)
        moveDirection = moveAction.ReadValue<float>();

        // Lógica da Animação
        bool estaAndando = Mathf.Abs(moveDirection) > 0.1f;
        
        // Envia o estado para o Animator (mantive o nome exatamente como você digitou)
        if (anim != null)
        {
            anim.SetBool("isRunnig", estaAndando);
        }

        // Lógica para virar o rosto do personagem
        if (moveDirection > 0.1f)
        {
            spriteRenderer.flipX = false; // Olha para a direita
        }
        else if (moveDirection < -0.1f)
        {
            spriteRenderer.flipX = true; // Olha para a esquerda
        }

        // Sistema de Punição
        if (estaAndando)
        {
            if (GameManager.Instance != null && !GameManager.Instance.isTaskLocked)
            {
                GameManager.Instance.ModifyLazy(walkPenaltyPerSecond * Time.deltaTime);
            }
        }
    }

    void FixedUpdate()
    {
        // Altera a velocidade no eixo X, preservando a gravidade no Y
        rb.linearVelocity = new Vector2(moveDirection * speed, rb.linearVelocity.y);
    }
}