using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Structure))]
public class Plot : MonoBehaviour, IClickableObject
{
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

    private TimeSpan timePerClaim;
    private DateTime startTime;
    private DateTime finishTime;

    // Timer
    private float updateTimer;

    private void Awake()
    {
        // TODO: Load status here.
    }

    // Start is called before the first frame update
    void Start()
    {
        structure = GetComponent<Structure>();

        timePerClaim = crop_SO.baseTimeNeededPerClaim;
    }

    // Update is called once per frame
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

    public void Plant(Crop_SO crop_SO)
    {
        this.crop_SO = crop_SO;
    }

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
        print("TODO: Claimed crops!");
        ResourceManager.Instance.AddToPlayerResources(FoodResource.Plant, crop_SO.amountPerClaim);
        plotState = PlotState.Empty;
    }
}
