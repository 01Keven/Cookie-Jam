using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance;

    [Header("Referências")]
    public GameObject popupPrefab;
    public Transform popupContainer; 
    public RectTransform phoneRect;  
    public FeedManager feedManager;

    [Header("Configurações do Evento")]
    public float adDuration = 8f; 
    public int minPopups = 4;
    public int maxPopups = 9;

    [Header("Área de Spawn (Ajuste no Inspector)")]
    [Tooltip("Ajuste esses limites para espalhar pela tela toda (ex: X -800 a 800, Y -500 a 500)")]
    public float minSpawnX = -800f;
    public float maxSpawnX = 800f;
    public float minSpawnY = -500f;
    public float maxSpawnY = 500f;

    private List<GameObject> activePopups = new List<GameObject>();

    void Awake()
    {
        Instance = this;
    }

    public void StartAdEvent(string eventID)
    {
        StartCoroutine(AdRoutine(eventID));
    }

    private IEnumerator AdRoutine(string eventID)
    {
        // 1. AVISA O GAME MANAGER QUE O AD COMEÇOU (Ativa a punição se desligar a tela)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.temAdNaTela = true;
        }

        int amount = Random.Range(minPopups, maxPopups + 1);

        for (int i = 0; i < amount; i++)
        {
            SpawnPopup();
            yield return new WaitForSeconds(0.1f); 
        }

        yield return new WaitForSeconds(adDuration);

        foreach (var p in activePopups)
        {
            if (p != null) Destroy(p);
        }
        activePopups.Clear();

        // 2. AVISA O GAME MANAGER QUE O AD ACABOU PELO TEMPO (Remove a punição)
        if (GameManager.Instance != null)
        {
            GameManager.Instance.temAdNaTela = false;
        }

        feedManager.DestravarFeed(eventID);
    }

    private void SpawnPopup()
    {
        GameObject newPopup = Instantiate(popupPrefab, popupContainer);
        
        RectTransform rect = newPopup.GetComponent<RectTransform>();
        
        // Agora o espalhamento obedece aos limites que você definir no Inspector
        float randomX = Random.Range(minSpawnX, maxSpawnX); 
        float randomY = Random.Range(minSpawnY, maxSpawnY);
        rect.anchoredPosition = new Vector2(randomX, randomY);

        newPopup.GetComponent<PopupAd>().Setup(phoneRect);
        activePopups.Add(newPopup);
    }

    // 3. NOVA FUNÇÃO: Chame esta função a partir do script do próprio botão de fechar do Ad (PopupAd)
    public void RemoverPopupFechado(GameObject popup)
    {
        activePopups.Remove(popup);
        
        // Se o jogador foi rápido e fechou todos antes do tempo acabar, tira a punição!
        if (activePopups.Count == 0 && GameManager.Instance != null)
        {
            GameManager.Instance.temAdNaTela = false;
        }
    }
}