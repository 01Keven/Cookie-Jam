using UnityEngine;
using UnityEngine.EventSystems;

public class PopupAd : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private RectTransform rectTransform;
    private RectTransform parentRect; 
    private RectTransform phoneRect;
    private Vector2 pointerOffset; 

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = transform.parent.GetComponent<RectTransform>();
    }

    public void Setup(RectTransform phoneArea)
    {
        phoneRect = phoneArea;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(-10f, 10f));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Joga o pop-up para a frente dos outros
        transform.SetAsLastSibling();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition);
        pointerOffset = rectTransform.anchoredPosition - localPointerPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 1. Salva a posição exata antes de tentar mover
        Vector2 oldPosition = rectTransform.anchoredPosition;

        // 2. Tenta mover a imagem para onde o mouse está
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, eventData.position, eventData.pressEventCamera, out Vector2 localPointerPosition))
        {
            rectTransform.anchoredPosition = localPointerPosition + pointerOffset;
        }

        // 3. Verifica se a nova posição fez a imagem bater no celular
        if (phoneRect != null && CheckOverlap(rectTransform, phoneRect))
        {
            // Se bateu, desfaz o movimento (isso cria a sensação de colisão física sólida)
            rectTransform.anchoredPosition = oldPosition;
        }
    }

    // Função matemática que cria dois quadrados precisos e checa se eles estão se tocando
    private bool CheckOverlap(RectTransform rect1, RectTransform rect2)
    {
        Vector3[] corners1 = new Vector3[4];
        rect1.GetWorldCorners(corners1);
        Rect r1 = new Rect(corners1[0].x, corners1[0].y, corners1[2].x - corners1[0].x, corners1[2].y - corners1[0].y);

        Vector3[] corners2 = new Vector3[4];
        rect2.GetWorldCorners(corners2);
        Rect r2 = new Rect(corners2[0].x, corners2[0].y, corners2[2].x - corners2[0].x, corners2[2].y - corners2[0].y);

        return r1.Overlaps(r2);
    }
}