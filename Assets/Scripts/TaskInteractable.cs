using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider2D))]
public class TaskInteractable : MonoBehaviour
{
    [Tooltip("Deve ser EXATAMENTE igual ao taskID colocado no PostData (ex: RegarPlanta)")]
    public string requiredTaskID;
    
    [Header("Controles")]
    public InputAction interactAction; 

    private bool isPlayerInRange = false;
    
    // 1. NOVA VARIÁVEL: Para guardar o Animator do Player
    private Animator playerAnim; 

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
        if (isPlayerInRange && interactAction.WasPressedThisFrame())
        {
            if (GameManager.Instance.activeTaskID == requiredTaskID)
            {
                // 2. TOCA A ANIMAÇÃO AQUI!
                if (playerAnim != null) 
                {
                    playerAnim.SetTrigger("Interact");
                }

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
            // 3. Pega o Animator do player quando ele entra na área
            playerAnim = collision.GetComponent<Animator>(); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            // 4. Limpa a referência quando o player vai embora
            playerAnim = null; 
        }
    }
}