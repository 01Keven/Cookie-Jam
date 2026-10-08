using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController2D : MonoBehaviour
{
    public float speed = 2f; 
    private Rigidbody2D rb;
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
    }

    void Update()
    {
        // Lê o input como um número flutuante (-1 para esquerda, 1 para direita, 0 parado)
        moveDirection = moveAction.ReadValue<float>();

        // Verifica se o player está tentando se mover (usamos Abs para ignorar se é esquerda ou direita)
        if (Mathf.Abs(moveDirection) > 0.1f)
        {
            // O pulo do gato: Só pune o jogador se o GameManager existir e NÃO houver tarefa ativa
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