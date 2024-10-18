using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    // Ensure this is visible in the Inspector
    public Card card;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public Image artworkImage;

    // Function to update the UI with card details (called when building is clicked)
    void Start()
    {
        if (card != null)
        {
            nameText.text = card.name;
            descriptionText.text = card.description;
            artworkImage.sprite = card.artwork;
        }
        else
        {
            Debug.LogError("Card reference is missing!");
        }
    }
}

