using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FeedManager : MonoBehaviour
{
    [Header("Referências da UI")]
    [Tooltip("Arraste o objeto TelaDoCelular (que tem a máscara) aqui")]
    public RectTransform phoneScreen; 
    
    [Tooltip("Arraste o objeto ContentContainer aqui")]
    public RectTransform contentContainer; 
    
    public GameObject postPrefab;

    [Header("Dados do Feed")]
    public List<PostData> availablePosts;

    [Header("Configurações do Swipe")]
    public float swipeThreshold = 50f; // Distância do dedo/mouse para validar o pulo
    public float snapSpeed = 15f; // Velocidade da transição de tela

    private int currentIndex = 0;
    private float postHeight;
    private float targetY = 0f;
    
    private Vector2 startDragPos;
    private bool isDragging;
    public bool isLocked = false; // Se o feed está travado por uma tarefa
    private HashSet<string> completedTasks = new HashSet<string>();
    

    void Start()
    {
        // Pega a altura real da máscara do celular. 
        // Essa será a distância exata de cada post e de cada pulo.
        postHeight = phoneScreen.rect.height;
        GenerateFeed();
        
    }

    private void GenerateFeed()
    {
        // Lê a largura exata da sua máscara do celular
        float postWidth = phoneScreen.rect.width;

        for (int i = 0; i < availablePosts.Count; i++)
        {
            GameObject newPost = Instantiate(postPrefab, contentContainer);
            RectTransform rect = newPost.GetComponent<RectTransform>();

            // Força a largura e a altura absolutas, ignorando qualquer âncora bugada
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, postWidth);
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, postHeight);
            
            // Posiciona um post perfeitamente embaixo do outro (-i * altura)
            rect.anchoredPosition = new Vector2(0, -i * postHeight);

            // Injeta os dados
            newPost.GetComponent<PostUI>().Setup(availablePosts[i]);
        }
    }


    private void HandleSwipe()
    {
        if (isLocked) return; // Se o feed estiver travado, não permite swipe

        if (Mouse.current == null) return;

        // Quando clica/toca na tela
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            startDragPos = Mouse.current.position.ReadValue();
            isDragging = true;
        }

        // Quando solta o clique/toque
        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            isDragging = false;
            float deltaY = Mouse.current.position.ReadValue().y - startDragPos.y;

            // Arrastou para CIMA (deltaY positivo) -> Próximo vídeo (desce o container)
            if (deltaY > swipeThreshold && currentIndex < availablePosts.Count - 1)
            {
                currentIndex++;
            }
            // Arrastou para BAIXO (deltaY negativo) -> Vídeo anterior (sobe o container)
            else if (deltaY < -swipeThreshold && currentIndex > 0)
            {
                currentIndex--;
            }

            // O novo Y alvo do container é o índice atual multiplicado pela altura da tela
            targetY = currentIndex * postHeight;
        }
    }

    private void SnapToCurrentPost()
    {
        // Move o container até travar perfeitamente no targetY
        Vector2 currentPos = contentContainer.anchoredPosition;
        currentPos.y = Mathf.Lerp(currentPos.y, targetY, Time.deltaTime * snapSpeed);
        contentContainer.anchoredPosition = currentPos;
    }

    void Update()
    {
        HandleSwipe();
        SnapToCurrentPost();
        CheckCurrentPostForTask();
    }

    private void CheckCurrentPostForTask()
    {
        if (isLocked) return;

        PostData currentData = availablePosts[currentIndex];

        // 1. Checa se é uma Tarefa de Mundo Real
        if (currentData.type == PostType.TaskHint && !completedTasks.Contains(currentData.taskID))
        {
            isLocked = true;
            GameManager.Instance.AtivarTarefa(currentData.taskID);
        }
        // 2. NOVO: Checa se é um Anúncio Estressante
        // Usamos a palavra "Ad_" + o índice numérico para criar um ID único (ex: Ad_4) e não repetir se o jogador voltar o scroll
        else if (currentData.type == PostType.StressfulAd && !completedTasks.Contains("Ad_" + currentIndex))
        {
            isLocked = true;
            AdManager.Instance.StartAdEvent("Ad_" + currentIndex);
        }
        // 3. Checa se é Entretenimento
        else if (currentData.type == PostType.Entertainment)
        {
            GameManager.Instance.ModifyLazy(currentData.lazyEffectPerSecond * Time.deltaTime);
        }
    }

    public void DestravarFeed(string taskID)
    {
        completedTasks.Add(taskID);
        isLocked = false;
    }
}