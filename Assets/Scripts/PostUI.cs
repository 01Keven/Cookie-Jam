using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PostUI : MonoBehaviour
{
    [Header("Referências do Post")]
    [SerializeField] private Image visualImage;
    [SerializeField] private TextMeshProUGUI authorText;
    [SerializeField] private TextMeshProUGUI captionText;

    [Header("Sistema de Curtidas")]
    [SerializeField] private Button likeButton;
    [SerializeField] private Image heartIcon; 
    [SerializeField] private TextMeshProUGUI likeCountText;
    [SerializeField] private Color unlikedColor = Color.white;
    [SerializeField] private Color likedColor = Color.red;

    private PostData myData;
    private bool isLiked = false;
    private int currentLikes;

    public void Setup(PostData data)
    {
        myData = data;
        authorText.text = "@" + data.authorName;
        captionText.text = data.caption;
        visualImage.sprite = data.postVisual;

        // Gera um número inicial falso de curtidas
        currentLikes = Random.Range(15, 8900);
        UpdateLikeUI();

        // Configura o botão de curtir
        likeButton.onClick.RemoveAllListeners();
        likeButton.onClick.AddListener(ToggleLike);
    }

    private void ToggleLike()
    {
        isLiked = !isLiked;
        
        // Aumenta ou diminui o número falso de curtidas
        currentLikes += isLiked ? 1 : -1;
        UpdateLikeUI();
    }

    private void UpdateLikeUI()
    {
        // heartIcon.color = isLiked ? likedColor : unlikedColor;
        likeCountText.text = currentLikes.ToString();
    }

    public PostData GetData()
    {
        return myData;
    }
}