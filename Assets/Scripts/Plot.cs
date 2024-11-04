using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

[RequireComponent(typeof(Structure))]
public class Plot : MonoBehaviour, IClickableObject
{
    private Structure structure;
    public enum PlotState
    {
        WAITING = 1,
        EMPTY = 2,
        GROWING = 3,
        RIPE = 4,
    }


    //TODO: Temporary serialize field for debugging
    [SerializeField] private Crop_SO crop_SO;
    [SerializeField] private Transform cropVisualPos;

    // Database
    private Database db;
    private Database.PlotData plotData = null;

    private PlotState currentPlotState = PlotState.WAITING;
    public PlotState CurrentPlotState
    {
        get => currentPlotState;

        private set
        {
            currentPlotState = value;
            plotData.plot_state = (int)currentPlotState;
            db.UpdateRecord(plotData);
        }
    }

    public Crop_SO Crop_SO { get { return crop_SO; } set { } }
    private DateTime growthFinishTime;
    public DateTime GrowthFinishTime
    {
        get => growthFinishTime;
        private set
        {
            growthFinishTime = value;
            plotData.growth_finish_time = growthFinishTime;
            db.UpdateRecord(plotData);
        }
    }

    // Timer
    private float updateTimer;

    void Start()
    {
        structure = GetComponent<Structure>();

        if (plotData == null)
        {
            db = FindFirstObjectByType<Database>();
            plotData = new Database.PlotData
            {
                structure_id = structure.StructureID,
                growth_finish_time = GrowthFinishTime,
                plot_state = (int)CurrentPlotState,
                crop_so_name = Crop_SO.name
            };

            db.AddNewPlot(plotData);
        }
    }

    public void LoadData(Database.PlotData data, Database db)
    {
        plotData = data;
        this.db = db;

        CurrentPlotState = (PlotState)data.plot_state;
        //not using the property so that I don't have to call the update database event. Just loading data. 
        growthFinishTime = data.growth_finish_time;

        string path = $"Crop/{data.crop_so_name}";
        Crop_SO cropSO = (Crop_SO)Resources.Load(path);
        Debug.Assert(cropSO != null, $"{data.crop_so_name} does not exist in {path}!");

        crop_SO = cropSO;

        if (CurrentPlotState == PlotState.GROWING)
            Instantiate(cropSO.cropPrefab, cropVisualPos);
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
        switch (CurrentPlotState)
        {
            case PlotState.WAITING:
                {
                    if (structure.CurrentBuildingState == Structure.BuildingState.BUILT)
                    {
                        CurrentPlotState = PlotState.EMPTY;
                    }

                    break;
                }
            case PlotState.EMPTY:
                {
                    // Nothing.
                    break;
                }
            case PlotState.GROWING:
                {
                    TimeSpan timeLeftToClaim = GrowthFinishTime - DateTime.Now;
                    print("Growing... " + timeLeftToClaim + " seconds left");
                    if (timeLeftToClaim <= TimeSpan.Zero)
                    {
                        CurrentPlotState = (PlotState.RIPE);
                    }
                    break;
                }
            case PlotState.RIPE:
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
        Debug.Assert(CurrentPlotState == PlotState.EMPTY, "Plot must be empty to plant!");
        Debug.Assert(crop_SO.baseTimeNeededPerClaim >= TimeSpan.Zero, "Time needed cannot be negative!");

        // Success
        GrowthFinishTime = DateTime.Now.Add(crop_SO.baseTimeNeededPerClaim);
        CurrentPlotState = PlotState.GROWING;

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
        switch (CurrentPlotState)
        {
            case PlotState.WAITING:
                {
                    structure.DisplayBuildingState();
                    break;
                }
            case PlotState.EMPTY:
                {
                    //UIManager.DisplayCropChoices();
                    print("TODO: Display Plant choices UI");
                    break;
                }
            case PlotState.GROWING:
                {
                    print("TODO: Display growth details");
                    break;
                }
            case PlotState.RIPE:
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

        CurrentPlotState = PlotState.EMPTY;
    }
}
