
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Database;

public class Tracker : MonoBehaviour
{
    private List<Plot> plots;
    private BuildingOverlay buildingOverlay;
    private UIManager uiManager;
    private GameManagerSheep barnGame;
    private FishingController fishingGame;
    private int harvestCount;
    private int buildingCount;
    private int minigameCount;

    [SerializeField] private TMP_Text harvestText;
    [SerializeField] private TMP_Text builtText;
    [SerializeField] private TMP_Text minigameText;

    [Obsolete]
    private void Start()
    {
        PlayerData playerData = Database.Instance.CurrentPlayerData;
        harvestCount = playerData.HarvestCount;
        buildingCount = playerData.BuildingCount;
        minigameCount = playerData.MinigameCount;

        harvestText.text = "Harvested Crop: " + harvestCount.ToString() + " / 20";
        builtText.text = "Structure Built: " + buildingCount.ToString() + " / 10";
        minigameText.text = "Minigames Played: " + minigameCount.ToString() + " / 10";

        plots = new List<Plot>(UnityEngine.Object.FindObjectsOfType<Plot>());
        foreach (Plot plot in plots)
        {
            plot.OnHarvest += TrackHarvest;
        }

        buildingOverlay = FindObjectOfType<BuildingOverlay>();
        buildingOverlay.OnBuilt += TrackBuilt;

        if (barnGame == null)
        { Debug.Log("Nawawala si Barn Game na script di madetect"); }
        barnGame = FindObjectOfType<GameManagerSheep>();
        barnGame.finishBarnGame += TrackMinigame;

        fishingGame = FindObjectOfType<FishingController>();
        fishingGame.finishFishingGame += TrackMinigame;

        uiManager = FindObjectOfType<UIManager>();
        uiManager.OnRemove += DecreaseTrack;
    }

    private void TrackHarvest()
    {
        harvestCount++;
        harvestText.text = "Harvested Crop: " + harvestCount.ToString() + " / 20";
        PlayerData playerData = Database.Instance.CurrentPlayerData;
        SaveTrackHarvest(playerData);
        Debug.Log("Tracked Harvest: " + harvestCount);
    }

    private void TrackBuilt(Structure structure)
    {
        if (structure.TryGetComponent<Plot>(out Plot newPlot))
        {
            plots.Add(newPlot);
            newPlot.OnHarvest += TrackHarvest;
        }
        buildingCount++;
        builtText.text = "Structure Built: " + buildingCount.ToString() + " / 10";
        PlayerData playerData = Database.Instance.CurrentPlayerData;
        SaveTrackBuilt(playerData);
        Debug.Log("Tracked Building: " + buildingCount);
    }
    private void TrackMinigame()
    {
        minigameCount++;
        minigameText.text = "Minigames Played: " + minigameCount.ToString() + " / 10";
        PlayerData playerData = Database.Instance.CurrentPlayerData;
        SaveTrackMinigame(playerData);
        Debug.Log("Tracked Minigame: " + minigameCount);
    }

    private void DecreaseTrack(Structure removeStructure)
    {
        if (removeStructure.TryGetComponent<Plot>(out Plot newPlot))
        {
            plots.Remove(newPlot);
            Debug.Log("One Plot removed from the list");
        }
        buildingCount--;
        PlayerData playerData = Database.Instance.CurrentPlayerData;
        SaveTrackBuilt(playerData);
        Debug.Log("Remaining Structure: " + buildingCount);

    }

    public void SaveTrackHarvest(PlayerData playerData)
    {
        playerData.HarvestCount = harvestCount;
        Database.Instance.UpdateRecord(playerData);
    }
    public void SaveTrackBuilt(PlayerData playerData)
    {
        playerData.BuildingCount = buildingCount;
        Database.Instance.UpdateRecord(playerData);
    }
    public void SaveTrackMinigame(PlayerData playerData)
    {
        playerData.BuildingCount = minigameCount;
        Database.Instance.UpdateRecord(playerData);
    }
    private void OnDestroy()
    {
        foreach (Plot plot in plots)
        {
            plot.OnHarvest -= TrackHarvest;
        }

        if (buildingOverlay != null)
        {
            buildingOverlay.OnBuilt -= TrackBuilt;
        }

        if (barnGame != null)
        {
            barnGame.finishBarnGame -= TrackMinigame;
        }
        if (fishingGame != null)
        {
            fishingGame.finishFishingGame -= TrackMinigame;
        }
    }
}
