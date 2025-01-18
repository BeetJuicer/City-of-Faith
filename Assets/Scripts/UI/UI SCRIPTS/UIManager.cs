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
    [SerializeField] private GameObject fishingMinigamePrefab;
    [SerializeField] private GameObject BarnMinigamePrefab;
    [SerializeField] private GlorySpeedUp glorySpeedUp;

    [SerializeField] private GameObject audioManager;

    private const string BARN_GAME = "BarnGame";
    private const string FISHING_GAME = "FishingGame";


    public void LoadFishingMinigame()
    {
        audioManager.gameObject.SetActive(false);
        DOTween.KillAll();
        MainCamera3d.gameObject.SetActive(false);
        fishingMinigamePrefab.SetActive(true);
        gameObject.SetActive(false);
    }

    public void LoadBarnMinigame()
    {
        audioManager.gameObject.SetActive(false);
        DOTween.KillAll();
        MainCamera3d.gameObject.SetActive(false);
        BarnMinigamePrefab.SetActive(true);
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

        DisableOnStructureClickButtons();
    }

    // Function that gets called when the Mini Game button is clicked
    public void OnMiniGameButtonClick(string structureName)
    {
        Debug.Log($"Starting mini game for {structureName}");
        switch (structureName)
        {
            case "Structure_BarnHouse":
                LoadBarnMinigame();
                break;
            case "Structure_FishingPort":
                LoadFishingMinigame();
                break;
            default:
                Debug.LogWarning("No minigame associated with this structure.");
                break;
        }
    }

    public void DisableOnStructureClickButtons()
    {
        infoButton.gameObject.SetActive(false);
        sellButton.gameObject.SetActive(false);
        miniGameButtonOpen.gameObject.SetActive(false);
        glorySpeedUp.CloseGlorySpeedUpPanel();
    }

    #region structureClickButtons
    public void ActivateInfoButton(Structure structure)
    {
        if (GameManager.Instance.CurrentGameState == GameState.Edit_Mode)
            return;

        GetComponent<CropManager>().CloseCropSelection();

        if (structure != null)
        {
            infoButton.gameObject.SetActive(true);
            infoButton.onClick.RemoveAllListeners();
            infoButton.onClick.AddListener(() =>
            {
                ShowStructureInfo(structure.structure_so);
                DisableOnStructureClickButtons();
            });
        }
        else
        {
            Debug.LogError("Selected structure is null.");
        }
    }

    public void ActivateBoostButton(IBoostableObject boostableObject)
    {
        if (GameManager.Instance.CurrentGameState == GameState.Edit_Mode)
            return;

        //GetComponent<CropManager>().CloseCropSelection();
        glorySpeedUp.OpenGlorySpeedUpPanel(boostableObject);
    }



    // Method to activate the Sell button dynamically
    public void ActivateSellButton(Structure structure)
    {
        if (GameManager.Instance.CurrentGameState == GameState.Edit_Mode)
            return;

        Debug.Log("Structure Clicked");
        //GetComponent<CropManager>().CloseCropSelection();
        // Show the action panel

        // Ensure the Sell button is visible
        sellButton.gameObject.SetActive(true);

        // Set up the Sell button to handle selling the structure
        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() =>
        {
            SellStructure(structure);
            DisableOnStructureClickButtons();
        });

    }

    public void ActivateMinigameButton(Structure structure)
    {
        if (GameManager.Instance.CurrentGameState == GameState.Edit_Mode)
            return;

        Debug.Log("minigame button activated");
        //GetComponent<CropManager>().CloseCropSelection();
        string structureName = structure.structure_so.structurePrefab.name;

        miniGameButtonOpen.gameObject.SetActive(true);
        miniGameButtonOpen.onClick.RemoveAllListeners();
        miniGameButtonOpen.onClick.AddListener(() =>
        {
            DisableOnStructureClickButtons();

            Debug.Log($"Starting mini game for {structureName}");
            switch (structureName)
            {
                case "Structure_BarnHouse":
                    LoadBarnMinigame();
                    break;
                case "Structure_FishingPort":
                    LoadFishingMinigame();
                    break;
                default:
                    Debug.LogWarning("No minigame associated with this structure.");
                    break;
            }
        });

    }

    #endregion

    private void SellStructure(Structure structure)
    {
        DisableOnStructureClickButtons();

        if (structure != null)
        {
            structure.GetComponent<Structure>().DestroyStructure();
        }
        else
        {
            Debug.LogWarning("Structure is null. Cannot sell.");
        }
    }

}