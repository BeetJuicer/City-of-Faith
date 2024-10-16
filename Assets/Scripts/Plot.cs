using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

[RequireComponent(typeof(Structure))]
public class Plot : MonoBehaviour, IClickableObject
{
    private void OnApplicationPause(bool pause)
    {
        
    }

    private void OnApplicationQuit()
    {
        
    }

    private Structure structure;

    private enum PlotState
    {
        Waiting,
        Empty,
        Growing,
        Ripe,
    }

    private PlotState plotState = PlotState.Waiting;

    //TODO: Temporary serialize field for debugging
    [SerializeField] private Crop_SO crop_SO;

    [SerializeField] private Transform cropVisualPos;

    private DateTime finishTime;

    // Timer
    private float updateTimer;

    private void Awake()
    {
        // TODO: Load status here.
    }

    void Start()
    {
        structure = GetComponent<Structure>();
    }

    void Update()
    {
        // Checking only twice per second.
        if (Time.time <= (updateTimer))
        {
            return;
        }

        updateTimer = Time.time + 0.5f;

        // State handling
        switch (plotState)
        {
            case PlotState.Waiting:
                {
                    if (structure.buildingState == Structure.BuildingState.Built)
                    {
                        plotState = PlotState.Empty;
                    }

                    break;
                }
            case PlotState.Empty:
                {
                    // Nothing.
                    break;
                }
            case PlotState.Growing:
                {
                    TimeSpan timeLeftToClaim = finishTime - DateTime.Now;
                    print("Growing... " + timeLeftToClaim + " seconds left");
                    if (timeLeftToClaim <= TimeSpan.Zero)
                    {
                        plotState = PlotState.Ripe;
                    }
                    break;
                }
            case PlotState.Ripe:
                {
                    // Nothing.
                    break;
                }
            default:
                {
                    Debug.LogWarning("Unhandled Plot State case.");
                    break;
                }
        }

    }

    /// <summary>
    ///  Method to be used when a plant is chosen in the shop menu.\
    /// </summary>
    /// <param name="crop_SO"></param>
    public void Plant(Crop_SO crop_SO)
    {
        this.crop_SO = crop_SO;

        // Asserts
        Debug.Assert(crop_SO == null, "crop SO cannot be null!");
        Debug.Assert(plotState == PlotState.Empty, "Plot must be empty to plant!");
        Debug.Assert(crop_SO.baseTimeNeededPerClaim >= TimeSpan.Zero, "Time needed cannot be negative!");

        // Success
        finishTime = DateTime.Now.Add(crop_SO.baseTimeNeededPerClaim);
        plotState = PlotState.Growing;

        //Visual update
        //TODO: Possible optimization, use crop pools.
        Instantiate(crop_SO.cropPrefab, cropVisualPos);
    }

    // TODO: Temporary button for testing only.
    [Button]
    public void Plant()
    {
        Plant(crop_SO);
    }

    /// <summary>
    /// Goes through states to decide what to do when the object is clicked.
    /// </summary>
    // TODO: Temporary button for testing only.
    [Button]
    public void OnObjectClicked()
    {
        // State handling
        switch (plotState)
        {
            case PlotState.Waiting:
                {
                    structure.DisplayBuildingState();
                    break;
                }
            case PlotState.Empty:
                {
                    //UIManager.DisplayCropChoices();
                    print("TODO: Display Plant choices UI");
                    break;
                }
            case PlotState.Growing:
                {
                    print("TODO: Display growth details");
                    break;
                }
            case PlotState.Ripe:
                {
                    ClaimHarvest();
                    break;
                }
            default:
                {
                    Debug.LogWarning("Unhandled Plot State case.");
                    break;
                }
        }
    }

    public void ClaimHarvest()
    {
        ResourceManager.Instance.AddToPlayerResources(FoodResource.Plant, crop_SO.amountPerClaim);

        //TODO: Possible optimization, use crop pools. May be temporary depending on UI
        Destroy(cropVisualPos.GetChild(0).gameObject);

        plotState = PlotState.Empty;
    }
}
