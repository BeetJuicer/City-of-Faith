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
    [SerializeField] protected Structure_SO structure_so;
    [SerializeField] private Structure_SO[] decorations;
    [SerializeField] private Structure_SO[] buildings;


    //Building State
    public enum BuildingState
    {
        InProgress,
        Built
    }

    public BuildingState buildingState { get; private set; }

    // In progress and built components
    private GameObject inProgressVisual;
    private GameObject builtVisual;
    private const string IN_PROGRESS_VISUAL_NAME = "InProgressVisual";
    private const string BUILT_VISUAL_NAME = "BuiltVisual";

    [Tooltip("The exact time the building was instantiated.")]//TODO: problems if user changes date and time of device.
    private DateTime timeInstantiated;
    private DateTime buildFinishedTime;

    private void Awake()
    {
        buildingState = BuildingState.InProgress;
    }

    private void Start()
    {
        // ENTER IN-PROGRESS STATE
        inProgressVisual = transform.Find(IN_PROGRESS_VISUAL_NAME).gameObject;
        builtVisual = transform.Find(BUILT_VISUAL_NAME).gameObject;

        Debug.Assert(inProgressVisual != null, "In Progress Visual not found. If the Visual exists, check the spelling.");
        Debug.Assert(builtVisual != null, "Built Visual not found. If the Visual exists, check the spelling.");

        //TODO: change to online method
        timeInstantiated = DateTime.Now;
        buildFinishedTime = timeInstantiated.AddDays(structure_so.BuildDays)
                                            .AddHours(structure_so.BuildHours)
                                            .AddMinutes(structure_so.BuildMinutes)
                                            .AddSeconds(structure_so.BuildSeconds);
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

    //UIManager.cs

    //DisplayStructureDetails(Structure_SO so)
    // setactive(UI ng display)
    // structueNametext.value = so.structureName;
    // description = description
    // cost. text.value = so.cost


    //scriptableObjects[]

    //DisplayScriptables(scriptbale[] arr)
    //foreach(structure in arr)
    // display card for each structure
    // structueNametext.value = so.structureName;
    // description = description
    // cost. text.value = so.cost

    // Update is called once per frame
    void Update()
    {
        switch (buildingState)
        {
            case BuildingState.Built:
                {
                    // shit here
                    // decor does nothing
                    // plants 
                    break;
                }
            case BuildingState.InProgress:
                {

                    // EXIT STATE.
                    if (DateTime.Now > buildFinishedTime)
                    {
                        // -- Resource Changes
                        print("TODO: Add " + structure_so.expGivenOnBuild + " exp!!");
                        print("TODO: Add " + structure_so.numberOfCitizensAdded + " citizens to population!!");

                        // -- Visual Changes
                        //TODO: VFX: Add vfx from object pool here.
                        inProgressVisual.SetActive(false);
                        builtVisual.SetActive(true);

                        // Exit state.
                        buildingState = BuildingState.Built;
                    }
                    break;
                }
            default:
                print("unhandled state");
                break;
        }
    }

    public void DisplayBuildingState()
    {
        //TODO: replace with UI
        print("Remaining Time: " + buildFinishedTime.Subtract(DateTime.Now));
    }

    private void OnApplicationQuit()
    {

    }
}

public class StructureSaveData
{
    Vector3 position;
    string prefabName;
    Structure.BuildingState structureState;
    DateTime buildFinishedTime;
}   