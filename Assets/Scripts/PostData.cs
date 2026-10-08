using UnityEngine;

public enum PostType 
{ 
    Entertainment, // Baixa a preguiça (vídeos idiotas, cookies)
    TaskHint,      // Dá dicas do que fazer no mundo real
    StressfulAd    // Aumenta a preguiça/estresse (opcional)
}

[CreateAssetMenu(fileName = "New Post", menuName = "Doomscroll/Post Data")]
public class PostData : ScriptableObject
{
    public string authorName;
    [TextArea(2, 4)] 
    public string caption;
    public Sprite postVisual; // Pode ser um Sprite, ou um VideoClip se for usar o VideoPlayer
    public PostType type;
    
    [Tooltip("Quanto esse post altera a barra de preguiça por segundo enquanto é assistido")]
    public float lazyEffectPerSecond = -5f; 
    public string taskID;
}