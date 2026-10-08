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

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        lazyBar.maxValue = maxLazy;
        lazyBar.value = currentLazy;
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