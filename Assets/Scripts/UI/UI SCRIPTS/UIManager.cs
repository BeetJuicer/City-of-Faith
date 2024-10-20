using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject actionPanel; // The panel that holds buttons or structure details
    [SerializeField] private StructureDisplayManager structureDisplayManager; // Reference to the StructureDisplayManager

    [SerializeField] private Button infoButton; // Reference to the Info button
    [SerializeField] private Button sellButton; // Reference to the Sell button (optional)

    private Structure_SO selectedStructure; // The structure that is currently selected/clicked

    // Show the action panel when a structure is clicked
    public void OnStructureClick(Structure_SO structure)
    {
        selectedStructure = structure;

        // Show the panel
        actionPanel.SetActive(true);

        // Reactivate the buttons when clicking a structure
        infoButton.gameObject.SetActive(true);
        if (sellButton != null)
        {
            sellButton.gameObject.SetActive(true);
        }

        // Call the StructureDisplayManager to show the structure details
        structureDisplayManager.DisplayStructureDetails(selectedStructure);
    }

    // Hide the action panel (useful when deselecting or closing)
    public void HideActionPanel()
    {
        actionPanel.SetActive(false);
        structureDisplayManager.CloseCard(); // Optionally close the card when hiding the panel
    }

    // This function can be used when opening a structure's details directly
    public void ShowStructureInfo(Structure_SO structure)
    {
        selectedStructure = structure;

        // Call the StructureDisplayManager to display the structure's details
        structureDisplayManager.DisplayStructureDetails(selectedStructure);

        // Show the action panel
        actionPanel.SetActive(true);
    }

    // Function to handle closing the structure info
    public void CloseStructureInfo()
    {
        structureDisplayManager.CloseCard(); // Close the card
        actionPanel.SetActive(false);        // Hide the action panel
    }

    // Assign button actions during Start or Awake
    void Start()
    {
        infoButton.onClick.AddListener(OnInfoButtonClick);

        // If the sell button is included, assign a listener to it
        if (sellButton != null)
        {
            sellButton.onClick.AddListener(OnSellButtonClick); // Even though it's currently disabled
        }
    }

    // Function that gets called when the Info button is clicked
    private void OnInfoButtonClick()
    {
        structureDisplayManager.DisplayStructureDetails(selectedStructure);

        // Hide the Info and Sell buttons when info is clicked
        infoButton.gameObject.SetActive(false);
        if (sellButton != null)
        {
            sellButton.gameObject.SetActive(false); // Only if sellButton is not null
        }

        // Optionally hide the action panel as well
        HideActionPanel(); // You can keep this if you want to close the panel too
    }

    // Empty function for the Sell button (disabled for now)
    private void OnSellButtonClick()
    {
        HideActionPanel(); // Only hides the panel
        // No additional logic for selling is implemented here.
    }
}
