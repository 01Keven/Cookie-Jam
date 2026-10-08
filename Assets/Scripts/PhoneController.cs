using UnityEngine;
using UnityEngine.UI;

public class PhoneController : MonoBehaviour
{
    public static PhoneController Instance;

    [Header("Visuais do Celular")]
    [Tooltip("Arraste o objeto que contém a máscara e o feed do celular. Ele será desativado.")]
    public GameObject screenContent; 
    
    [Tooltip("Imagem preta que simula a tela desligada (opcional)")]
    public GameObject blackScreen;

    public bool isPhoneOn = true;

    void Awake()
    {
        Instance = this;
    }

    public void TogglePhonePower()
    {
        isPhoneOn = !isPhoneOn;
        
        if (screenContent != null) screenContent.SetActive(isPhoneOn);
        
        if (blackScreen != null) blackScreen.SetActive(!isPhoneOn);

        Debug.Log(isPhoneOn ? "Celular LIGADO." : "Celular DESLIGADO.");
    }
}