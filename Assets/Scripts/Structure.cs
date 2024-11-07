using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

/*
 This class handles the building progress of a structure.
 */
public class Structure : MonoBehaviour
{
    [SerializeField] private Structure_SO structure_so;

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
    private DateTime timeInstantiated;
    private DateTime timeBuildFinished;
    //TODO: problems if user changes date and time of device.
    #endregion

    // In progress and built components
    private GameObject inProgressVisual;
    private GameObject builtVisual;
    private const string IN_PROGRESS_VISUAL_NAME = "InProgressVisual";
    private const string BUILT_VISUAL_NAME = "BuiltVisual";

    private void Awake()
    {
        GetChildrenVisuals();
    }

    private void GetChildrenVisuals()
    {
        // ENTER IN-PROGRESS STATE
        inProgressVisual = transform.Find(IN_PROGRESS_VISUAL_NAME).gameObject;
        builtVisual = transform.Find(BUILT_VISUAL_NAME).gameObject;

        Debug.Assert(inProgressVisual != null, "In Progress Visual not found. If the Visual exists, check the spelling.");
        Debug.Assert(builtVisual != null, "Built Visual not found. If the Visual exists, check the spelling.");
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
            timeBuildFinished = timeInstantiated.AddDays(structure_so.BuildDays)
                                                .AddHours(structure_so.BuildHours)
                                                .AddMinutes(structure_so.BuildMinutes)
                                                .AddSeconds(structure_so.BuildSeconds);

            Vector3 structurePos = transform.position;
            Quaternion rotation = transform.rotation;

            structureData = new Database.StructureData
            {
                prefab_name = structure_so.structurePrefab.name,
                player_id = db.PlayerId,
                time_build_finished = timeBuildFinished,
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
            StructureID = structureData.structure_id;

            Debug.Assert(StructureID != 0, "Structure ID is 0!");
        }
    }

    /// Called by database to initialize needed values.
    public void LoadData(Database.StructureData data, Database db)
    {
        this.db = db;
        structureData = data;

        timeBuildFinished = data.time_build_finished;

        //not using the property so that I don't have to call the update database event. Just loading data. 
        currentBuildingState = (BuildingState)data.building_state;

        if (currentBuildingState == BuildingState.BUILT) 
            EnterBuiltState();
    }

    // UIManager Integration - Detect when this structure is clicked
    private void OnMouseDown()
    {
        // Assuming the UIManager is already set up in the scene.
        UIManager uiManager = FindObjectOfType<UIManager>();

        if (uiManager != null)
        {
            // Call UIManager to show structure details when clicked
            uiManager.OnStructureClick(structure_so);
        }
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
                    if (DateTime.Now > timeBuildFinished)
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
    private void DestroyStructure()
    {
        print("TODO: Sold structure! Add resources returned to resourcemanage");
        db.DeleteRecord(structureData);
        Destroy(gameObject);
    }

    private void EnterBuiltState()
    {
        // -- Visual Changes
        //TODO: VFX: Add vfx from object pool here.
        inProgressVisual.SetActive(false);
        builtVisual.SetActive(true);

        // Exit state.
        CurrentBuildingState = BuildingState.BUILT;
    }

    public void DisplayBuildingState()
    {
        //TODO: replace with UI
        print("Remaining Time: " + timeBuildFinished.Subtract(DateTime.Now));
    }
}