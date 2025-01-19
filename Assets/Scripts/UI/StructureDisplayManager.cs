using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class StructureDisplayManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private GameObject card;                 // UI card to display structure details
    [SerializeField] private TextMeshProUGUI nameText;        // UI element for the structure's name
    [SerializeField] private TextMeshProUGUI descriptionText; // UI element for the structure's description
    [SerializeField] private Image artworkImage;             // UI element for the structure's artwork

    // Populates the card UI with the details of the specified structure
    public void DisplayStructureDetails(Structure_SO structure_so)
    {
        if (structure_so != null)
        {
            // Make the card visible and update its fields
            card.SetActive(true);

            // Remove "_SO" from the structure name before displaying
            string formattedName = structure_so.name.Replace("_SO", "");
            nameText.text = formattedName;

            descriptionText.text = structure_so.description;
            artworkImage.sprite = structure_so.displayImage;
        }
        else
        {
            Debug.LogError("Structure data (Structure_SO) is missing!");
        }
    }


    // Hides the structure detail card
    public void CloseCard()
    {
        if (card != null)
        {
            card.SetActive(false);
        }
        else
        {
            Debug.LogError("The 'card' GameObject is not assigned! Please assign it in the Inspector.");
        }
    }
}
