using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class Structure : MonoBehaviour, IClickableObject
{
    [SerializeField] protected Structure_SO structure_so;

    //Building State
    private enum BuildingState
    {
        InProgress,
        Built
    }

    private BuildingState buildingState;

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
                    if (DateTime.Now.Equals(buildFinishedTime))
                    {
                        buildingState = BuildingState.Built;
                        //TODO: VFX: Add vfx from object pool here.
                        //change to normal prefab
                        inProgressVisual.SetActive(false);
                        builtVisual.SetActive(true);
                    }
                    break;
                }
            default:
                print("unhandled state");
                break;
        }
    }


    private void DisplayUI()
    {
        //UIManager.HUD.DisplayStructureUI(structure_so, buildState);
        //TODO: Use UI manager.
        switch (buildingState)
        {
            case BuildingState.Built:
                {

                    break;
                }
            case BuildingState.InProgress:
                {
                    print("Time left until built: " + buildFinishedTime.Subtract(timeInstantiated).ToString());
                    break;
                }
        }

        print("Display UI\n" + structure_so.structureName + ": " + structure_so.description);
    }

    //temporary naughtyattributes button
    [Button]
    public void OnStructureClicked()
    {
        DisplayUI();
    }

    [Button]
    public void OnDestroy()
    {
        //print("Destroyed " + structure_so.structureName + ". Returning " + structure_so.resellValue + " gold coins.");
    }

    public void OnObjectClicked()
    {
        DisplayUI();
    }
}
