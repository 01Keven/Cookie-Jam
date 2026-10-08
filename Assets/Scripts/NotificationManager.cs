using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Para os textos da UI

[System.Serializable]
public class PhoneMessage
{
    public string senderName;
    public string messageText;
    public bool isMomEvent; // Marca como verdadeiro se for a mãe
}

public class NotificationManager : MonoBehaviour
{
    [Header("Configurações de Tempo")]
    public float minWaitTime = 10f;
    public float maxWaitTime = 25f;
    public int totalMessagesToSpawn = 5; // Limite de mensagens por fase

    [Header("Evento da Mãe")]
    [Tooltip("Tempo que o player tem para desligar a tela depois que a notificação aparece")]
    public float reactionTime = 3f; 
    public GameObject momCharacterVisual; // O sprite da mãe no mundo 2D que vai aparecer

    [Header("Interface da Notificação")]
    public GameObject notificationPanel;
    public TextMeshProUGUI senderText;
    public TextMeshProUGUI contentText;
    public float popupDuration = 4f; // Tempo que a notificação fica na tela

    [Header("Banco de Mensagens")]
    public List<PhoneMessage> possibleMessages;

    private int messagesShown = 0;

    void Start()
    {
        notificationPanel.SetActive(false);
        if (momCharacterVisual != null) momCharacterVisual.SetActive(false);
        
        StartCoroutine(NotificationRoutine());
    }

    private IEnumerator NotificationRoutine()
    {
        while (messagesShown < totalMessagesToSpawn)
        {
            // Sorteia o tempo de espera até a próxima notificação
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            yield return new WaitForSeconds(waitTime);

            // Escolhe uma mensagem aleatória
            PhoneMessage randomMsg = possibleMessages[Random.Range(0, possibleMessages.Count)];
            
            // Mostra a notificação na tela
            ShowNotification(randomMsg);
            messagesShown++;

            // Se for a mãe, inicia a mecânica de "Stealth"
            if (randomMsg.isMomEvent)
            {
                yield return StartCoroutine(MomEventRoutine());
            }
            else
            {
                // Se for uma mensagem normal, só espera o popup sumir
                yield return new WaitForSeconds(popupDuration);
                notificationPanel.SetActive(false);
            }
        }
    }

    private void ShowNotification(PhoneMessage msg)
    {
        senderText.text = msg.senderName;
        contentText.text = msg.messageText;
        notificationPanel.SetActive(true);
        // Opcional: Tocar um som de notificação ("Bzz Bzz") aqui
    }

    private IEnumerator MomEventRoutine()
    {
        // 1. A notificação aparece e a mãe "chega" no cenário
        if (momCharacterVisual != null) momCharacterVisual.SetActive(true);
        
        // 2. O jogador tem X segundos para reagir
        yield return new WaitForSeconds(reactionTime);
        notificationPanel.SetActive(false); // Esconde a notificação

        // 3. O momento da verdade: A mãe checa se o celular está ligado
        if (PhoneController.Instance.isPhoneOn)
        {
            // O JOGADOR FOI PEGO!
            GameManager.Instance.LosingLife();
            Debug.Log("GAME OVER ou PENALIDADE! A mãe pegou você no celular.");
            // Exemplo de penalidade: GameManager.Instance.ModifyLazy(50f);
        }
        else
        {
            // O jogador conseguiu esconder a tempo.
            Debug.Log("Ufa! A mãe achou que você estava dormindo.");
        }

        // 4. A mãe vai embora, independentemente do resultado
        yield return new WaitForSeconds(2f); // Ela fica um tempinho encarando
        if (momCharacterVisual != null) momCharacterVisual.SetActive(false);
    }
}