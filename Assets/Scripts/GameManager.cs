using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

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

    [Header("Progressão e Fim de Jogo")]
    [Tooltip("Coloque aqui o ID exato da última tarefa que finaliza a fase")]
    public string taskIDDeVitoria; 
    
    [Tooltip("Painel de UI que vai aparecer quando a barra encher")]
    public GameObject gameOverPanel;
    
    public float maxPreguica = 100f; // Defina o valor máximo da sua barra
    private bool jogoAcabou = false; // Evita que as funções rodem várias vezes

    [Header("Transição de Fase (Fade e Cutscene)")]
    public Image fadeImage; 
    public float fadeDuration = 1f; 
    
    [Tooltip("O painel de UI contendo a imagem/texto da cutscene")]
    public GameObject cutscenePanel; 
    
    [Tooltip("O componente AudioSource que vai tocar o som")]
    public AudioSource audioDaCutscene; 
    
    [Tooltip("Tempo em segundos que a cutscene fica na tela antes de ir para a próxima fase")]
    public float tempoDaCutscene = 4f;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (lazyBar != null)
        {
            StartCoroutine(FadeInRoutine());
        }
        ;

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
        if (jogoAcabou) return; // Evita que a preguiça continue aumentando após o fim do jogo

        if (currentLazy >= maxPreguica)
        {
            AtivarGameOver();
        }
        
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
        if (jogoAcabou) return; // Evita que a função rode várias vezes
        Debug.Log("Tarefa Concluída: " + activeTaskID);

        if (activeTaskID == taskIDDeVitoria)
        {
            Debug.Log("Parabéns! Você completou a fase!");
            // Aqui você pode adicionar lógica para avançar para a próxima fase ou mostrar uma tela de vitória
            VencerLevel();
            return; // Sai da função para não continuar com a lógica de destravar o feed
        }
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

    private void AtivarGameOver()
    {
        jogoAcabou = true;
        Time.timeScale = 0f; // Pausa o jogo
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true); // Mostra a tela de Game Over
        }
    }

    private void VencerLevel()
    {
        jogoAcabou = true;
        int proximaCenaIndex = SceneManager.GetActiveScene().buildIndex + 1;
        
        if (proximaCenaIndex < SceneManager.sceneCountInBuildSettings)
        {
            if (fadeImage != null)
                StartCoroutine(FadeOutAndLoad(proximaCenaIndex));
            else
                SceneManager.LoadScene(proximaCenaIndex);
        }
        else
        {
            Debug.Log("Fim de jogo!");
        }
    }

    public void BotaoRestart()
    {
        Time.timeScale = 1f; // Despausa o jogo
        // Recarrega a cena atual do zero
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BotaoMenuPrincipal()
    {
        Time.timeScale = 1f; // Despausa o jogo
        Debug.Log("Indo para o Menu Principal... (Cena ainda não existe)");
        // Quando o menu existir, descomente a linha abaixo e coloque o nome exato da cena
        // SceneManager.LoadScene("MainMenuScene"); 
    }

    private IEnumerator FadeInRoutine()
    {
        fadeImage.gameObject.SetActive(true);
        Color cor = fadeImage.color;
        float tempo = 0f;

        while (tempo < fadeDuration)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(1f, 0f, tempo / fadeDuration); // 1 = Opaco, 0 = Transparente
            fadeImage.color = cor;
            yield return null; // Espera até o próximo frame para continuar
        }
        
        fadeImage.gameObject.SetActive(false); // Desativa no fim para segurança
    }

    private IEnumerator FadeOutAndLoad(int indexCena)
    {
        // 1. ESCURECE A TELA (FADE PARA PRETO)
        fadeImage.gameObject.SetActive(true);
        Color cor = fadeImage.color;
        float tempo = 0f;

        while (tempo < fadeDuration)
        {
            tempo += Time.deltaTime;
            cor.a = Mathf.Lerp(0f, 1f, tempo / fadeDuration); 
            fadeImage.color = cor;
            yield return null;
        }

        // 2. ATIVA A CUTSCENE E TOCA O SOM
        // Esconde a tela preta para não tapar a cutscene
        fadeImage.gameObject.SetActive(false); 
        
        if (cutscenePanel != null) cutscenePanel.SetActive(true);
        if (audioDaCutscene != null) audioDaCutscene.Play();

        // 3. ESPERA A CUTSCENE ACABAR
        yield return new WaitForSeconds(tempoDaCutscene);

        // 4. ESCURECE RAPIDAMENTE DE NOVO ANTES DE MUDAR DE CENA (Para não ser um corte seco)
        if (cutscenePanel != null) cutscenePanel.SetActive(false);
        fadeImage.gameObject.SetActive(true);

        // 5. CARREGA A PRÓXIMA FASE
        SceneManager.LoadScene(indexCena);
    }

    
}