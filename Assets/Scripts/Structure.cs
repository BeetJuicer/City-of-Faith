using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class Structure : MonoBehaviour, IClickableObject
{
    private enum BuildingState
    {
        InProgress,
        Built
    }

    private BuildingState buildingState;
    [SerializeField] protected Structure_SO structure_so;
    [Tooltip("The exact time the building was instantiated.")]// problems if user changes date and time of device.
    private DateTime timeInstantiated;
    private DateTime buildFinishedTime;

    private void Awake()
    {
        buildingState = BuildingState.InProgress;
    }

    private void Start()
    {
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
                    if (DateTime.Now.Equals(buildFinishedTime))
                    {
                        buildingState = BuildingState.Built;
                        //change to normal prefab

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
