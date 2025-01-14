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
    [SerializeField] private Button miniGameButtonOpen;// Reference to the Mini Game button

    private Structure_SO selectedStructure; // The structure that is currently selected/clicked

    [SerializeField] private Camera MainCamera3d;
    [SerializeField] private GameObject fishingScene;
    [SerializeField] private GlorySpeedUp glorySpeedUp;
    VolumeSettings volumeSettings;

    private const string BARN_GAME = "BarnGame";
    private const string FISHING_GAME = "FishingGame";

    public void LoadFishingMinigame()
    {
        DOTween.KillAll();
        MainCamera3d.gameObject.SetActive(false);
        fishingScene.SetActive(true);
        gameObject.SetActive(false);
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

    }

    // Function that gets called when the Sell button is clicked
    private void OnSellButtonClick()
    {
    }

    // Function that gets called when the Mini Game button is clicked
    public void OnMiniGameButtonClick(string structureName)
    {
        Debug.Log($"Starting mini game for {structureName}");
        switch (structureName)
        {
            case "Structure_BarnHouse":
                //SceneManager.LoadScene(BARN_GAME_SCENE);
                break;
            case "Structure_FishingPort":
                LoadFishingMinigame();
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
        Debug.Log("Structure Clicked");
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
        });

    }

    public void ActivateMinigameButton(Structure structure)
    {
        Debug.Log("minigame button activated");
        string structureName = structure.structure_so.structurePrefab.name;

        miniGameButtonOpen.gameObject.SetActive(true);
        miniGameButtonOpen.onClick.RemoveAllListeners();
        miniGameButtonOpen.onClick.AddListener(() =>
        {
            Debug.Log($"Starting mini game for {structureName}");
            switch (structureName)
            {
                case "Structure_BarnHouse":
                    //SceneManager.LoadScene(BARN_GAME_SCENE);
                    miniGameButtonOpen.onClick.RemoveAllListeners();
                    break;
                case "Structure_FishingPort":
                    LoadFishingMinigame();
                    miniGameButtonOpen.onClick.RemoveAllListeners();
                    break;
                default:
                    Debug.LogWarning("No minigame associated with this structure.");
                    break;
            }
        });

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