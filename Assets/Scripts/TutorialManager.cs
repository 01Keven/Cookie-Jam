using UnityEngine;
using TMPro; // Necessário se estiver usando TextMeshPro para os textos

public class TutorialManager : MonoBehaviour
{
    [Header("Referências da UI")]
    public GameObject tutorialPanel;
    public TextMeshProUGUI tutorialText; 

    [Header("Texto desta Fase")]
    [TextArea(4, 8)]
    public string textoDoTutorial;

    void Start()
    {
        // Exibe o painel e aplica o texto configurado no Inspector
        if (tutorialPanel != null && tutorialText != null)
        {
            tutorialText.text = textoDoTutorial;
            tutorialPanel.SetActive(true);
            
            // Congela o tempo do jogo (para a preguiça e os inimigos não avançarem)
            Time.timeScale = 0f; 
        }
    }

    // Função que será chamada pelo Botão de Fechar
    public void FecharTutorial()
    {
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
        
        // Faz o jogo voltar a rodar normalmente
        Time.timeScale = 1f; 
    }
}