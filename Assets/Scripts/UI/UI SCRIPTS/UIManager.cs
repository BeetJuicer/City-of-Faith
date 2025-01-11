using UnityEngine;
using UnityEngine.UI;
using System;
using System.Resources;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class UIManager : MonoBehaviour
{

    [SerializeField] private StructureDisplayManager structureDisplayManager; // Reference to the StructureDisplayManager

    [SerializeField] private Button infoButton; // Reference to the Info button
    [SerializeField] private Button sellButton; // Reference to the Sell button
    [SerializeField] private Button miniGameButton; // Reference to the Mini Game button

    private Structure_SO selectedStructure; // The structure that is currently selected/clicked

    [SerializeField] private GlorySpeedUp glorySpeedUp;

    // Constants for Minigame Scene Names
    private const string BARN_GAME_SCENE = "BarnGame";
    private const string FISHING_GAME_SCENE = "FishingQuickCatch";

    void Start()
    {
        // Check if the mini-game was completed
        int miniGameCompleted = PlayerPrefs.GetInt("MinigameCompleted", 0); // Default to 0 (not completed)

        if (miniGameCompleted == 1)
        {
            // Fetch the gold and exp earned from the previous scene (BarnGame or FishingQuickCatch)
            int goldEarned = PlayerPrefs.GetInt("GoldEarned", 0);
            int expEarned = PlayerPrefs.GetInt("ExpEarned", 0);
            string miniGameName = PlayerPrefs.GetString("MinigameName", "Unknown");

            // Log the reward message with the mini-game name
            Debug.Log($"Congrats! You earned {goldEarned} gold and {expEarned} experience from playing {miniGameName}.");

            // Optionally clear the PlayerPrefs if you don't need them anymore
            PlayerPrefs.DeleteKey("GoldEarned");
            PlayerPrefs.DeleteKey("ExpEarned");

            // Reset the flag so it doesn't log again next time
            PlayerPrefs.SetInt("MinigameCompleted", 0);
        }
    }

    public void LoadMinigameScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.Log($"Loading Minigame Scene: {sceneName}");
            DOTween.KillAll(); // Kill all active tweens before loading the new scene
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Scene name is null or empty!");
        }
    }


    public void HideActionPanel()
    {
        miniGameButton.onClick.RemoveAllListeners(); // Clear listeners to prevent duplicate calls
    }

    public void ShowStructureInfo(Structure_SO structure)
    {
        if (structure != null)
        {
            structureDisplayManager.DisplayStructureDetails(structure);
        }
        else
        {
            Debug.LogError("Selected structure is null.");
        }
    }

    // Function that gets called when the Info button is clicked
    private void OnInfoButtonClick()
    {
        structureDisplayManager.DisplayStructureDetails(selectedStructure);

        // Hide the Info and Sell buttons when info is clicked
        infoButton.gameObject.SetActive(false);
        sellButton?.gameObject.SetActive(false);

        // Optionally hide the action panel as well
        HideActionPanel();
    }

    // Function that gets called when the Sell button is clicked
    private void OnSellButtonClick()
    {
        HideActionPanel(); // Only hides the panel
        // Add logic for selling the structure here
    }

    // Function that gets called when the Mini Game button is clicked
    private void OnMiniGameButtonClick(string structureName)
    {
        Debug.Log($"Starting mini game for {structureName}");

        switch (structureName)
        {
            case "Structure_BarnHouse":
                SceneManager.LoadScene(BARN_GAME_SCENE);
                break;
            case "Structure_FishingPort":
                SceneManager.LoadScene(FISHING_GAME_SCENE);
                break;
            default:
                Debug.LogWarning("No minigame associated with this structure.");
                break;
        }
    }

    public void OpenBoostButton(IBoostableObject boostableObject, DateTime finishTime, TimeSpan totalDuration)
    {
        glorySpeedUp.OpenGlorySpeedUpPanel(boostableObject, finishTime, totalDuration);
    }

    public void ActivateInfoButton(Structure structure)
    {
        if (structure != null)
        {
            infoButton.gameObject.SetActive(true);
            infoButton.onClick.RemoveAllListeners();
            infoButton.onClick.AddListener(() =>
            {
                ShowStructureInfo(structure.GetStructureSO());
                HideActionPanel();
            });
        }
        else
        {
            Debug.LogError("Selected structure is null.");
        }
    }

    // Method to activate the Sell button dynamically
    public void ActivateSellButton(Structure structure)
    {
        Debug.LogWarning("Structure Clicked");
        // Show the action panel

        // Ensure the Sell button is visible
        sellButton.gameObject.SetActive(true);

        // Set up the Sell button to handle selling the structure
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() =>
        {
            SellStructure(structure);
            // Optionally hide the panel and buttons after selling
            infoButton.gameObject.SetActive(false);
            sellButton?.gameObject.SetActive(false);
            HideActionPanel();
        });

    }

    public void ActivateMinigameButton(Structure structure)
    {
        // Get the structure name
        string structureName = structure.structure_so.structurePrefab.name;

        // Determine the minigame scene to load based on the structure name
        string sceneName = structureName switch
        {
            "Structure_BarnHouse" => BARN_GAME_SCENE,
            "Structure_FishingPort" => FISHING_GAME_SCENE,
            _ => null
        };

        // If a valid scene is found, configure the miniGameButton
        if (!string.IsNullOrEmpty(sceneName))
        {
            miniGameButton.gameObject.SetActive(true);
            miniGameButton.onClick.RemoveAllListeners(); // Clear previous listeners
            miniGameButton.onClick.AddListener(() => LoadMinigameScene(sceneName)); // Add the correct listener
            Debug.Log($"Minigame Button is set to load: {sceneName}");
        }
        else
        {
            miniGameButton.gameObject.SetActive(false);
            Debug.LogWarning("No minigame associated with the selected structure.");
        }
    }



    private void SellStructure(Structure structure)
    {
        if (structure != null)
        {
            // Implement your selling logic here, e.g., award gold or resources

            Debug.Log($"Sold structure: {structure.GetStructureSO().name}");
            // Optional: Destroy the structure GameObject if necessary
            Destroy(structure.gameObject);
        }
        else
        {
            Debug.LogWarning("Structure is null. Cannot sell.");
        }
    }

}