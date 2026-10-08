using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class TaskInteractable : MonoBehaviour
{
    [Tooltip("Deve ser EXATAMENTE igual ao taskID colocado no PostData (ex: RegarPlanta)")]
    public string requiredTaskID;
    
    [Header("Controles")]
    public InputAction interactAction; // Expõe a ação no Inspector

    private bool isPlayerInRange = false;

    private void OnEnable()
    {
        interactAction.Enable();
    }

    private void OnDisable()
    {
        interactAction.Disable();
    }

    void Update()
    {
        // Se o player estiver perto e a tecla for pressionada neste exato frame
        if (isPlayerInRange && interactAction.WasPressedThisFrame())
        {
            // Verifica se esta é a tarefa que o jogo está pedindo agora
            if (GameManager.Instance.activeTaskID == requiredTaskID)
            {
                GameManager.Instance.CompletarTarefaAtual();
                
                Debug.Log("Interação concluída!");
            }
            else
            {
                Debug.Log("Não é hora de fazer isso.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}