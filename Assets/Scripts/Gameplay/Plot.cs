using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

[RequireComponent(typeof(Structure))]
public class Plot : MonoBehaviour, IClickableObject, IBoostableObject
{
    private Structure structure;
    public enum PlotState
    {
        // BE CAREFUL WITH CHANGING. ENUM IS STORED AS INT IN DATABASE. DON'T CHANGE ORDER UNLESS ABSOLUTELY NEEDED !!!

        WAITING = 1,
        EMPTY = 2,
        GROWING = 3,
        RIPE = 4,
    }


    //TODO: Temporary serialize field for debugging
    [SerializeField] Crop_SO tempCropSO;

    private bool hasPopped = false;
    private CropManager cropManager;
    private CropVisual cropVisual;
    private CropImageDisplay cropImageDisplay;
    private Crop_SO crop_SO;
    private UIManager uiManager;
    [SerializeField] private Transform cropVisualPos;

    // Database
    private Database db;
    private Database.PlotData plotData = null;

    //debugging
    private PlotState currentPlotState = PlotState.WAITING;
    public PlotState CurrentPlotState
    {
        get => currentPlotState;

        private set
        {
            currentPlotState = value;
            plotData.plot_state = (int)currentPlotState;
            if (db == null) print("db_logs: db in plot is null!!");
            db.UpdateRecord(plotData);
            //UI
            if (cropVisual != null) cropVisual.UpdateVisual(currentPlotState);
            if (cropImageDisplay != null) cropImageDisplay.UpdateVisual(currentPlotState, Crop_SO1);
        }
    }

    public Crop_SO Crop_SO { get { return Crop_SO1; } set { Crop_SO1 = value; } }
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

    public Crop_SO Crop_SO1 { get => Crop_SO2; set => Crop_SO2 = value; }
    public Crop_SO Crop_SO2 { get => crop_SO; set => crop_SO = value; }

    // Timer
    private float updateTimer;

    CentralHall central;

    void Start()
    {
        central = FindObjectOfType<CentralHall>();
        structure = GetComponent<Structure>();
        cropImageDisplay = GetComponentInChildren<CropImageDisplay>();
        cropManager = FindObjectOfType<CropManager>();
        Debug.Assert(cropImageDisplay != null, "Cannot find crop image display in children!");

        if (plotData == null)
        {
            Crop_SO = null;
            db = FindFirstObjectByType<Database>();

            plotData = new Database.PlotData
            {
                structure_id = structure.StructureID,
                growth_finish_time = GrowthFinishTime,
                plot_state = (int)currentPlotState,
                crop_so_name = null
            };

            db.AddNewRecord(plotData);
            print($"db_logs: Initial added plot# {plotData.structure_id} with crop_so_name: null to database.");
        }     
        else
        {
            print($"db_logs: not adding new plot to database!");
        }

        cropImageDisplay.UpdateVisual(CurrentPlotState, Crop_SO1);
        uiManager = FindObjectOfType<UIManager>();
    }

    public void LoadData(Database.PlotData data, Database db)
    {
        print("db_logs: Loading crop......");
        //not using the property so that I don't have to call the update database event. Just loading data. 
        plotData = data;
        this.db = db;

        currentPlotState = (PlotState)data.plot_state;
        growthFinishTime = data.growth_finish_time;

        if (plotData.crop_so_name == null)
            return;

        string path = $"Crops/SO_Crops/{data.crop_so_name}";
        print($"db_logs: Attempting to look for {path}");
        Crop_SO cropSO = (Crop_SO)Resources.Load(path);
        Debug.Assert(cropSO != null, $"{data.crop_so_name} does not exist in {path}!");

        if (cropSO == null)
            print($"db_logs: {path} could not be found!");
        else
            print($"db_logs: {path} found!");

        Crop_SO1 = cropSO;
        print($"db_logs: Loaded {Crop_SO1.name} from database!");


        if (currentPlotState == PlotState.GROWING)
        {
            print("db_logs: last saved is " + currentPlotState + ".");
            TimeSpan timeLeftToClaim = GrowthFinishTime - DateTime.Now;
            if (timeLeftToClaim <= TimeSpan.Zero)
            {
                print("db_logs: moving to ripe state");
                CurrentPlotState = (PlotState.RIPE);
            }

            InstantiateCrop(cropSO);
        }
        else if (currentPlotState == PlotState.RIPE)
        {
            InstantiateCrop(cropSO);
        }
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

    // Calls update visual on the crop as well.
    private void InstantiateCrop(Crop_SO crop_SO)
    {
        print($"db_logs: instantiating {crop_SO.name}");
        var cropGO = Instantiate(crop_SO.cropPrefab, cropVisualPos);
        if (cropGO.TryGetComponent(out cropVisual))
        {
            cropVisual.UpdateVisual(CurrentPlotState);
        }
        else
        {
            Debug.LogWarning($"{crop_SO.cropPrefab} does not have a CropVisual component.");
        }
    }

    // TODO: Temporary button for testing only.
    [Button]
    public void Plant()
    {
        Plant(tempCropSO);
    }

    /// <summary>
    ///  Method to be used when a plant is chosen in the shop menu.\
    /// </summary>
    /// <param name="crop_SO"></param>
    public void Plant(Crop_SO crop_SO)
    {
        // Asserts
        Debug.Assert(crop_SO != null, "crop SO cannot be null!");
        Debug.Assert(CurrentPlotState == PlotState.EMPTY, "Cannot plant while plot is not empty!");
        Debug.Assert(crop_SO.baseTimeNeededPerClaim >= TimeSpan.Zero, "Time needed cannot be negative!");

        this.Crop_SO1 = crop_SO;

        // Success
        //GrowthFinishTime = DateTime.Now.Add(crop_SO.baseTimeNeededPerClaim);
        TimeSpan plotCodeDuration = new TimeSpan(crop_SO.daysToClaim, crop_SO.hoursToClaim, crop_SO.minutesToClaim, crop_SO.daysToClaim);
        TimeSpan propertyTest = Crop_SO.baseTimeNeededPerClaim;

        GrowthFinishTime = DateTime.Now.Add(plotCodeDuration);

        print($"planted at time: {DateTime.Now.ToString()} will finish at: {GrowthFinishTime}. From {crop_SO.name} with durationPerClaim {crop_SO.baseTimeNeededPerClaim} \n " +
                $"before: {crop_SO.baseTimeNeededPerClaim} + {DateTime.Now} = {DateTime.Now.Add(crop_SO.baseTimeNeededPerClaim)}, \n" +
                $"separated: {crop_SO.daysToClaim}, {crop_SO.hoursToClaim}, {crop_SO.minutesToClaim}, {crop_SO.secondsToClaim}. \n" +
                $"test combination: {crop_SO.baseTimeNeededPerClaim} \n" +
                $"compare: plottest: {plotCodeDuration} vs. propertytest: {propertyTest}");
        CurrentPlotState = PlotState.GROWING;

        //Visual update
        //TODO: Possible optimization, use crop pools.
        InstantiateCrop(crop_SO);

        //update db when planted
        plotData.crop_so_name = crop_SO.name;
        db.UpdateRecord(plotData);
        print($"db_logs: Updating database of plot #{plotData.structure_id} with {crop_SO.name}. ");
    }

    /// <summary>
    /// Goes through states to decide what to do when the object is clicked.
    /// </summary>
    // TODO: Temporary button for testing only.
    [Button]
    public void OnObjectClicked()
    {

        print("clicked. state is: " + currentPlotState);
        AudioManager.Instance.PlaySFX("Plot");

        if (hasPopped == false)
        {
            PopOnClick();
        }

        // State handling
        switch (CurrentPlotState)
        {
            case PlotState.WAITING:
                {
                    //do nothing
                    break;
                }
            case PlotState.EMPTY:
                {
                    //if(cropselection)
                    //    //add to crops selected.
                    //pagkapili ng crop, for all crops selected, Plant(napilingCrop)

                    cropManager.OpenCropSelection(this);
                    print("Empty plot clicked." + "Opening crop selection");
                    break;
                }
            case PlotState.GROWING:
                {
                    //UIManager.OpenInfoButton(cropSO);
                    //UIManager.OpenSellButton(this.GetComponent<Structure>())
                    uiManager.ActivateBoostButton(this);
                    break;
                }
            case PlotState.RIPE:
                {
                    //SoundEffectForRipe
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

    public void PopOnClick()
    {
        hasPopped = true;
        structure.PopOnClick();
    }

    public void ResetPopState()
    {
        hasPopped = false;
        structure.ResetPopState();
    }

    //private void OnMouseDown()
    //{
    //    OnObjectClicked();
    //}

    public void ClaimHarvest()
    {
        ResourceManager.Instance.AdjustPlayerCurrency(Currency.Gold, Crop_SO1.amountPerClaim);
        ResourceManager.Instance.AdjustPlayerResources(FoodResource.Plant, Crop_SO1.amountPerClaim);
        central.AddToCentralExp(Crop_SO1.expPerClaim);

        //TODO: Possible optimization, use crop pools. May be temporary depending on UI
        Destroy(cropVisualPos.GetChild(0).gameObject);

        CurrentPlotState = PlotState.EMPTY;
    }

    public void BoostProgress()
    {
        GrowthFinishTime = DateTime.Now;
    }

    public bool IsInBoostableState()
    {
        return CurrentPlotState == PlotState.GROWING;
    }

    public DateTime GetTimeFinished()
    {
        return GrowthFinishTime;
    }

    public TimeSpan GetTotalDuration()
    {
        return new TimeSpan(Crop_SO.daysToClaim, Crop_SO.hoursToClaim, Crop_SO.minutesToClaim, Crop_SO.secondsToClaim);
    }
}
