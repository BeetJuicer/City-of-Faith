using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    private List<Plot> plots;
    private List<BuildingOverlay> buildingOverlays;
    private int harvestCount = 0;
    private int minigameCount = 0;
    private int buildingCount = 0;

    private void Start()
    {
        // Find the Plot and subscribe to its OnCropHarvested event
        Debug.Log("Tracker Script started!");
        plots = new List<Plot>(FindObjectsOfType<Plot>());
        foreach (Plot plot in plots)
        {
            plot.OnHarvest += TrackHarvest;
        }

        //buildingOverlays = new List<BuildingOverlay>(FindObjectOfType<BuildingOverlay>());
        //foreach (BuildingOverlay buildingOverlay in buildingOverlays)
        //{
        //    buildingOverlay.OnBuilt += TrackBuilding;
        //}
        //buildingOverlay = FindObjectOfType<BuildingOverlay>();
        //buildingOverlay.OnBuilt += TrackBuilding;
    }


    // Method to handle the harvest and display the subscription count
    private void TrackHarvest()
    {
        harvestCount++;
        Debug.Log($"Harvest event received! Current subscriber count:" + harvestCount);

    }
    private void TrackBuilding(Structure s)
    {
        buildingCount++;
        Debug.Log($"Building event received! Current subscriber count:" + buildingCount);

        //if (s.TryGetComponent<Plot>)
        //{
        //    plots.Add(s);
        //}

    }
    private void OnDestroy()
    {
        foreach (Plot plot in plots)
        {
            if (plot != null) // Check if plot still exists
            {
                plot.OnHarvest -= TrackHarvest;
            }
        }
        //buildingOverlay.OnBuilt -= TrackBuilding;
    }

}
