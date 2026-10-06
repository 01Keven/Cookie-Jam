using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FeedManager : MonoBehaviour
{
    [Header("Referências")]
    public Transform contentContainer; 
    public GameObject postPrefab;
    
    [Header("Dados")]
    public List<PostData> availablePosts; 
    
    [Header("Configurações do Feed")]
    public float postHeight = 800f; 
    public float scrollSpeed = 15f; // Aumentado levemente para um snap mais rápido e moderno

    [Header("Configurações de Swipe (Arrastar)")]
    public float dragThreshold = 50f; // Distância mínima do mouse para considerar um arrasto
    public float inputCooldown = 0.4f; // Tempo de bloqueio (em segundos) antes de poder passar de post de novo

    private List<PostUI> spawnedPosts = new List<PostUI>();
    private int currentIndex = 0;
    private float targetYPosition = 0f;

    // Variáveis de controle de input
    private float lastInputTime = 0f;
    private Vector2 startDragPos;
    private bool isDragging;

    void Start()
    {
        GenerateFeed();
    }

    void Update()
    {
        HandleInput();
        SmoothScroll();
        ApplyEffectOfCurrentPost();
    }

    private void GenerateFeed()
    {
        foreach (var data in availablePosts)
        {
            GameObject newPostObj = Instantiate(postPrefab, contentContainer);
            PostUI postUI = newPostObj.GetComponent<PostUI>();
            postUI.Setup(data);
            spawnedPosts.Add(postUI);
        }
    }

    private void HandleInput()
    {
        // Impede que o jogador role a tela rápido demais (cooldown)
        if (Time.time - lastInputTime < inputCooldown) return;

        bool moveNext = false;
        bool movePrev = false;

        // 1. Controle por Teclado (Mantido apenas para facilitar seus testes no editor)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.downArrowKey.wasPressedThisFrame) moveNext = true;
            if (Keyboard.current.upArrowKey.wasPressedThisFrame) movePrev = true;
        }

        // 2. Controle EXCLUSIVO por Arrastar (Swipe) - Rodinha do mouse removida
        if (Mouse.current != null)
        {
            // Quando aperta o botão do mouse
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                startDragPos = Mouse.current.position.ReadValue();
                isDragging = true;
            }
            
            // Quando solta o botão do mouse
            if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
            {
                isDragging = false;
                Vector2 endDragPos = Mouse.current.position.ReadValue();
                
                // Calcula a diferença de altura entre onde clicou e onde soltou
                float deltaY = endDragPos.y - startDragPos.y;

                // No Unity, Y positivo significa que o mouse foi para CIMA.
                if (deltaY > dragThreshold) 
                {
                    moveNext = true; // Arrastou para cima = Próximo post
                }
                else if (deltaY < -dragThreshold) 
                {
                    movePrev = true; // Arrastou para baixo = Post anterior
                }
            }
        }

        // Executa a transição se algum comando foi validado
        if (moveNext)
        {
            NextPost();
            lastInputTime = Time.time;
        }
        else if (movePrev)
        {
            PreviousPost();
            lastInputTime = Time.time;
        }
    }

    public void NextPost()
    {
        if (currentIndex < spawnedPosts.Count - 1)
        {
            currentIndex++;
            targetYPosition = currentIndex * postHeight;
        }
    }

    public void PreviousPost()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            targetYPosition = currentIndex * postHeight;
        }
    }

    private void SmoothScroll()
    {
        Vector3 newPos = contentContainer.localPosition;
        newPos.y = Mathf.Lerp(newPos.y, targetYPosition, Time.deltaTime * scrollSpeed);
        contentContainer.localPosition = newPos;
    }

    private void ApplyEffectOfCurrentPost()
    {
        if (spawnedPosts.Count == 0) return;
        PostData currentData = spawnedPosts[currentIndex].GetData();
        // A lógica da barra de preguiça vai aqui futuramente
    }
}