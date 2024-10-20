using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

public class StructureDisplayManager : MonoBehaviour
{
    // Ensure this is visible in the Inspector
    public Structure_SO structure_so;

    [SerializeField] private GameObject card;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image artworkImage;

    [Button]
    public void OpenCard()
    {
        DisplayStructureDetails(structure_so);
    }

    [Button]
    public void CloseCard()
    {
        card.SetActive(false);
    }

    // Function to update the UI with card details (called when building is clicked)
    public void DisplayStructureDetails(Structure_SO structure_so)
    {
        card.SetActive(true);

        if (structure_so != null)
        {
            nameText.text = structure_so.name;
            descriptionText.text = structure_so.description;
            artworkImage.sprite = structure_so.displayImage;
        }
        else
        {
            Debug.LogError("Card reference is missing!");
        }
    }
}

