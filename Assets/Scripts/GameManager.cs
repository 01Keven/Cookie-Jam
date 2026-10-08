using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Barra de Preguiça (UI)")]
    public Slider lazyBar;
    public float maxLazy = 100f;
    public float currentLazy = 50f; // Começa na metade

    [Header("Drenagem da Energia")]
    public float normalLazyIncrease = 2f; // Aumenta devagar se não estiver em tarefa
    public float taskPenaltyIncrease = 10f; // Aumenta rápido quando o feed trava!

    [Header("Estado Atual")]
    public string activeTaskID = "";
    public bool isTaskLocked = false;

    [Header("Sistema de Vidas (Mãe)")]
    public int currentLives = 3;
    public GameObject[] heartIcons;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lazyBar.maxValue = maxLazy;
        lazyBar.value = currentLazy;
        currentLives = 3;
        UpdateHeartsUI();
    }

    void Update()
    {
        // Calcula a taxa de preguiça. Se estiver travado na tarefa, a preguiça sobe rápido.
        float currentRate = isTaskLocked ? taskPenaltyIncrease : normalLazyIncrease;
        
        currentLazy += currentRate * Time.deltaTime;
        lazyBar.value = currentLazy;

        if (currentLazy >= maxLazy)
        {
            GameOver();
        }
    }

    public void LosingLife()
    {
        currentLives--; // Tira uma vida
        UpdateHeartsUI(); // Apaga um coração da tela

        Debug.Log("Você foi pego pela mãe! Vidas restantes: " + currentLives);

        if (currentLives <= 0)
        {
            GameOverForLifes();
        }
    }
    private void UpdateHeartsUI()
    {
        // Previne erros caso você não tenha colocado as imagens no Inspector ainda
        if (heartIcons == null || heartIcons.Length == 0) return;

        for (int i = 0; i < heartIcons.Length; i++)
        {
            // Se o índice do coração for menor que a quantidade de vidas, ele fica ligado.
            // Ex: Se tem 2 vidas, o heartIcons[0] e [1] ficam ativos. O [2] é desativado.
            heartIcons[i].SetActive(i < currentLives);
        }
    }

    private void GameOverForLifes()
    {
        Debug.Log("GAME OVER: A mãe confiscou o celular! Você está de castigo.");
        // SceneManager.LoadScene("GameOverScene"); // Descomente quando criar a cena de derrota
    }

    

    public void ModifyLazy(float amount)
    {
        currentLazy += amount;
        currentLazy = Mathf.Clamp(currentLazy, 0, maxLazy);
    }

    public void AtivarTarefa(string taskID)
    {
        activeTaskID = taskID;
        isTaskLocked = true;
        Debug.Log("Feed Travado! Tarefa necessária: " + taskID);
    }

    public void CompletarTarefaAtual()
    {
        Debug.Log("Tarefa Concluída: " + activeTaskID);
        
        // Destrava o feed
        FindAnyObjectByType<FeedManager>().DestravarFeed(activeTaskID);
        
        isTaskLocked = false;
        activeTaskID = "";
        
        // Bônus: completar tarefa abaixa um pouco a preguiça
        ModifyLazy(-20f); 
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER: Preguiça atingiu o máximo. O personagem foi dormir.");
        // SceneManager.LoadScene("GameOverScene");
    }
}