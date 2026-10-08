using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Referências")]
    public Transform player;

    [Header("Configurações da Tela")]
    [Tooltip("A largura exata da visão da sua câmera em unidades da Unity. Altere até bater com as bordas.")]
    public float roomWidth = 18f; 
    
    [Tooltip("Quão rápido a câmera desliza para a nova tela. Coloque um número muito alto (ex: 100) para teleporte instantâneo.")]
    public float transitionSpeed = 8f;

    private float targetX;

    void Start()
    {
        // Salva a posição inicial X da câmera como o primeiro alvo
        targetX = transform.position.x;
    }

    void Update()
    {
        if (player == null) return;

        // Se o player passou da borda DIREITA da tela atual
        if (player.position.x > targetX + (roomWidth / 2f))
        {
            targetX += roomWidth; // Atualiza o alvo da câmera para a próxima tela à direita
        }
        // Se o player passou da borda ESQUERDA da tela atual
        else if (player.position.x < targetX - (roomWidth / 2f))
        {
            targetX -= roomWidth; // Atualiza o alvo da câmera para a tela à esquerda
        }

        // Desliza a câmera suavemente até o novo targetX, mantendo o Y e Z originais
        Vector3 targetPosition = new Vector3(targetX, transform.position.y, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * transitionSpeed);
    }
}