using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

/*
 This class handles the building progress of a structure.
 */
public class Structure : MonoBehaviour, IClickableObject, IBoostableObject
{
    [SerializeField] public Structure_SO structure_so;

    #region Database 
    private Database db;
    private Database.StructureData structureData = null;
    public int StructureID { get; private set; }
    #endregion

    #region Building State

    public enum BuildingState
    {
        // BE CAREFUL WITH CHANGING. ENUM IS STORED AS INT IN DATABASE. DON'T CHANGE ORDER UNLESS ABSOLUTELY NEEDED !!!

        IN_PROGRESS = 1,
        BUILT = 2
    }
    private BuildingState currentBuildingState = BuildingState.IN_PROGRESS;
    public BuildingState CurrentBuildingState
    {
        get => currentBuildingState;
        private set
        {
            currentBuildingState = value;
            structureData.building_state = (int)currentBuildingState;
            db.UpdateRecord(structureData);
        }
    }

    //getcomponentinparent<structure>().TimeBuildFinished

    private DateTime timeInstantiated;
    public DateTime TimeBuildFinished { get; private set; }
    //TODO: problems if user changes date and time of device.
    #endregion

    // In progress and built components
    private GameObject inProgressVisual;
    private GameObject builtVisual;
    private GameObject timerUI;
    private const string IN_PROGRESS_VISUAL_NAME = "InProgressVisual";
    private const string BUILT_VISUAL_NAME = "BuiltVisual";
    private const string TIMER_NAME = "BuildingTimerUI";

    private UIManager uiManager;

    public event Action OnStructureInProgressClicked;
    CentralHall centralHall;

    private void Awake()
    {
        // Dynamically find the UIManager in the scene if not assigned in the Inspector
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }

        GetChildrenVisuals();
    }

    private void GetChildrenVisuals()
    {
        // ENTER IN-PROGRESS STATE
        inProgressVisual = transform.Find(IN_PROGRESS_VISUAL_NAME).gameObject;
        builtVisual = transform.Find(BUILT_VISUAL_NAME).gameObject;
        timerUI = transform.Find(TIMER_NAME).gameObject;

        Debug.Assert(inProgressVisual != null, "In Progress Visual not found. If the Visual exists, check the spelling.");
        Debug.Assert(builtVisual != null, "Built Visual not found. If the Visual exists, check the spelling.");
        Debug.Assert(timerUI != null, "Timer UI not found. If the object exists, check the spelling.");
    }

    private void Start()
    {
        // Load default values if not in database.
        if (structureData == null)
        {
            db = FindFirstObjectByType<Database>();
            currentBuildingState = BuildingState.IN_PROGRESS;

            //TODO: change to online method
            timeInstantiated = DateTime.Now;
            TimeBuildFinished = timeInstantiated.AddDays(structure_so.BuildDays)
                                                .AddHours(structure_so.BuildHours)
                                                .AddMinutes(structure_so.BuildMinutes)
                                                .AddSeconds(structure_so.BuildSeconds);

            Vector3 structurePos = transform.position;
            Quaternion rotation = transform.rotation;

            structureData = new Database.StructureData
            {
                prefab_name = structure_so.structurePrefab.name,
                player_id = db.PlayerId,
                time_build_finished = TimeBuildFinished,
                building_state = (int)currentBuildingState,
                posX = structurePos.x,
                posY = structurePos.y,
                posZ = structurePos.z,
                rotW = rotation.w,
                rotX = rotation.x,
                rotY = rotation.y,
                rotZ = rotation.z,
            };

            db.AddNewRecord(structureData);
            print($"Added {structureData.structure_id}. {structureData.prefab_name} to player {structureData.player_id}'s database.");
            StructureID = structureData.structure_id;

            Debug.Assert(StructureID != 0, "Structure ID is 0!");
        }

        uiManager = FindObjectOfType<UIManager>();
        Debug.Assert(uiManager != null, "UI Manager not assigned!");
        centralHall = FindObjectOfType<CentralHall>();
    }

    /// Called by database to initialize needed values.
    public void LoadData(Database.StructureData data, Database db)
    {
        this.db = db;
        structureData = data;

        TimeBuildFinished = data.time_build_finished;

        //not using the property so that I don't have to call the update database event. Just loading data. 
        currentBuildingState = (BuildingState)data.building_state;

        if (currentBuildingState == BuildingState.BUILT)
            EnterBuiltState();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentBuildingState)
        {
            case BuildingState.BUILT:
                {
                    break;
                }
            case BuildingState.IN_PROGRESS:
                {
                    //print("In progress: " + gameObject.name + " Time finished: " + timeBuildFinished);
                    // EXIT STATE.
                    if (DateTime.Now > TimeBuildFinished)
                    {
                        // -- Resource Changes
                        print("TODO: Add " + structure_so.expGivenOnBuild + " exp!!");
                        print("TODO: Add " + structure_so.numberOfCitizensAdded + " citizens to population!!");

                        EnterBuiltState();
                    }
                    break;
                }
            default:
                Debug.LogError("Unhandled state! State is: " + currentBuildingState);
                break;
        }
    }

    [Button]
    public void DestroyStructure()
    {
        ResourceManager.Instance.AdjustPlayerCurrency(structure_so.currencyRequired);
        db.DeleteRecord(structureData);
        Destroy(gameObject);
    }

    private void EnterBuiltState()
    {
        if (centralHall == null)
            centralHall = FindObjectOfType<CentralHall>();

        //Add xp.
        centralHall.AddToCentralExp(structure_so.expGivenOnBuild);
        // -- Visual Changes
        //TODO: VFX: Add vfx from object pool here.
        inProgressVisual.SetActive(false);
        timerUI.SetActive(false);
        builtVisual.SetActive(true);

        // Exit state.
        CurrentBuildingState = BuildingState.BUILT;
    }

    public void DisplayBuildingState()
    {
        //TODO: replace with UI
        print("Remaining Time: " + TimeBuildFinished.Subtract(DateTime.Now));
    }

    [Button]
    public void OnObjectClicked()
    {
        // Ensure UIManager is assigned before proceeding
        if (uiManager == null)
        {
            Debug.LogError("UIManager is not assigned or found!");
            return;
        }

        switch (CurrentBuildingState)
        {
            case BuildingState.IN_PROGRESS:
                Debug.LogWarning("InProgress Working");
                uiManager.ActivateBoostButton(this);
                uiManager.ActivateInfoButton(this);
                uiManager.ActivateSellButton(this);
                OnStructureInProgressClicked?.Invoke();

                break;

            case BuildingState.BUILT:
                if (TryGetComponent<ResourceProducer>(out ResourceProducer rp) || 
                    TryGetComponent<Plot>(out Plot p))
                {
                    return;// plot and resource producer handle built clicks
                }

                uiManager.ActivateInfoButton(this);
                uiManager.ActivateSellButton(this);
                break;

            default:
                Debug.LogWarning("Unhandled building state case!");
                break;
        }
    }


    //for pc
    private void OnMouseDown()
    {
        OnObjectClicked();
    }

    public void BoostProgress()
    {
        TimeBuildFinished = DateTime.Now;
    }

    public bool IsInBoostableState()
    {
        return CurrentBuildingState == BuildingState.IN_PROGRESS;
    }

    public DateTime GetTimeFinished()
    {
        return TimeBuildFinished;
    }

    public TimeSpan GetTotalDuration()
    {
        return new TimeSpan(structure_so.BuildDays, structure_so.BuildHours, structure_so.BuildMinutes, structure_so.BuildSeconds);
    }
}