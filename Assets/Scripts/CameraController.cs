using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Configurações do Alvo")]
    [Tooltip("Arraste o objeto do Player para cá")]
    public Transform target; 
    
    [Tooltip("Distância da câmera em relação ao player. O Z deve ser negativo!")]
    public Vector3 offset = new Vector3(0f, 1.5f, -10f);

    [Header("Suavidade da Câmera")]
    [Tooltip("Quanto menor o número, mais lenta e 'cinematográfica' é a perseguição")]
    public float smoothSpeed = 5f;

    // Usamos LateUpdate em vez de Update para evitar tremedeiras (Jitter) na física
    void LateUpdate()
    {
        // Se o player for destruído ou não estiver linkado, a câmera não faz nada
        if (target == null) return;

        // Calcula onde a câmera DEVERIA estar (Posição do Player + a distância do Offset)
        Vector3 desiredPosition = target.position + offset;

        // Vector3.Lerp faz a câmera deslizar suavemente do ponto atual até o ponto desejado
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
        
        // Aplica a nova posição na câmera
        transform.position = smoothedPosition;
    }
}