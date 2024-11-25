using UnityEngine;
using UnityEngine.UI;
using TMPro;
using NaughtyAttributes;

// Manages the display of structure details in a UI card when a structure is clicked.
// Uses data from a ScriptableObject (Structure_SO) to populate the UI elements.

public class StructureDisplayManager : MonoBehaviour
{
    // Reference to the structure's ScriptableObject (Structure_SO) containing its data.

    public Structure_SO structure_so;

    [Header("UI Elements")]
    [SerializeField] private GameObject card;                 // UI card to display structure details
    [SerializeField] private TextMeshProUGUI nameText;        // UI element for the structure's name
    [SerializeField] private TextMeshProUGUI descriptionText; // UI element for the structure's description
    [SerializeField] private Image artworkImage;             // UI element for the structure's artwork

    // Opens the UI card and displays the details of the associated structure.
    // Triggered by a button press or structure selection.

    [Button] // NaughtyAttributes button for quick testing in the Unity Inspector
    public void OpenCard()
    {
        DisplayStructureDetails(structure_so); // Display the details of the current structure
    }

    [Button] // NaughtyAttributes button for quick testing in the Unity Inspector
    public void CloseCard()
    {
        card.SetActive(false); // Hides the card UI
    }

    //Populates the card UI with the details of the specified structure.
    // Activates the card and updates its fields.

    public void DisplayStructureDetails(Structure_SO structure_so)
    {
        // Make the card visible
        card.SetActive(true);

        if (structure_so != null)
        {
            // Update card UI with structure data
            nameText.text = structure_so.name;
            descriptionText.text = structure_so.description;
            artworkImage.sprite = structure_so.displayImage;
        }
        else
        {
            Debug.LogError("Structure data (Structure_SO) is missing!");
        }
    }
}