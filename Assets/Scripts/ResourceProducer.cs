using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;
using SQLite;

public enum ProducerState
{
    Waiting = 1,
    Producing = 2,
    Ready_To_Claim = 3,
}

[RequireComponent(typeof(Structure))]
public class ResourceProducer : MonoBehaviour, IClickableObject
{
    private Structure structure;

    private ProducerState resourceState;

    [SerializeField] private ResourceProducer_SO resourceProducer_SO;
    private int amountPerClaim;

    private TimeSpan timePerClaim;
    private DateTime finishTime;

    // Timer
    private float updateTimer;

    private void Awake()
    {
        resourceState = ProducerState.Producing;
    }

    private void Start()
    {
        structure = GetComponent<Structure>();

        //Keep a base amount per claim in case leveling up buildings is possible.
        amountPerClaim = resourceProducer_SO.baseAmountPerClaim;
        timePerClaim = resourceProducer_SO.baseTimeNeededPerClaim;
    }

    public void LoadData(Database.ResourceProducerData data)
    {
        print("Load data here.");
    }

    private void Update()
    {
        // Checking only twice per second.
        if (Time.time <= updateTimer)
        {
            return;
        }

        updateTimer = Time.time + 0.5f;

        // State handling
        switch (resourceState)
        {
            case ProducerState.Waiting:
                {
                    if (structure.buildingState == Structure.BuildingState.BUILT)
                    {
                        StartProduction();
                    }
                    break;
                }
            case ProducerState.Ready_To_Claim:
                {
                    // nothing here.
                    print("Ready to Claim!!");
                    break;
                }
            case ProducerState.Producing:
                {
                    TimeSpan timeLeftToClaim = finishTime - DateTime.Now;
                    print("Producing... " + timeLeftToClaim + " seconds left");
                    if (timeLeftToClaim <= TimeSpan.Zero)
                    {
                        resourceState = ProducerState.Ready_To_Claim;
                    }

                    break;
                }
            default:
                {
                    print("Unhandled Resource State.");
                    break;
                }
        }
    }

    private void StartProduction()
    {
        finishTime = DateTime.Now.Add(timePerClaim);

        resourceState = ProducerState.Producing;
    }

    public void ClaimResources()
    {
        print("Claimed " + amountPerClaim + resourceProducer_SO.resource_SO.resourceType + "!");
        ResourceManager.Instance.AddToPlayerResources(FoodResource.Fish, amountPerClaim);
        StartProduction();
    }


    private void OnApplicationQuit()
    {
        //Save function here.
    }

    [Button]
    public void OnObjectClicked()
    {
        switch (resourceState)
        {
            case ProducerState.Waiting:
                {
                    //UIManager.DisplayDetails(structure_SO);
                    structure.DisplayBuildingState();
                    break;
                }
            case ProducerState.Ready_To_Claim:
                {
                    ClaimResources();
                    break;
                }
            case ProducerState.Producing:
                {
                    print("In progress");
                    break;
                }
            default:
                {
                    print("Unhandled Resource State.");
                    break;
                }
        }
    }
}
