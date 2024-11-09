using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;
using SQLite;

public enum ProducerState
{
    // BE CAREFUL WITH CHANGING. ENUM IS STORED AS INT IN DATABASE. DON'T CHANGE ORDER UNLESS ABSOLUTELY NEEDED !!!

    Waiting = 1,
    Producing = 2,
    Ready_To_Claim = 3,
}

[RequireComponent(typeof(Structure))]
public class ResourceProducer : MonoBehaviour, IClickableObject
{
    private Structure structure;

    // Database
    private Database db;
    private Database.ResourceProducerData resourceProducerData = null;

    [SerializeField] private ResourceProducer_SO resourceProducer_SO;
    private int amountPerClaim;
    private TimeSpan timePerClaim;

    //temp debug bool
    public bool printTime = false;


    private ProducerState currentProducerState = ProducerState.Waiting;
    public ProducerState CurrentProducerState
    {
        get => currentProducerState;
        private set
        {
            currentProducerState = value;
            resourceProducerData.producer_state = (int)currentProducerState;
            db.UpdateRecord(resourceProducerData);
        }
    }

    private DateTime productionFinishTime;
    public DateTime ProductionFinishTime
    {
        get => productionFinishTime;
        private set
        {
            productionFinishTime = value;
            resourceProducerData.production_finish_time = productionFinishTime;
            db.UpdateRecord(resourceProducerData);
        }
    }

    // Timer
    private float updateTimer;

    private void Start()
    {
        structure = GetComponent<Structure>();
        db = FindFirstObjectByType<Database>();

        if (resourceProducerData == null)
        {
            resourceProducerData = new Database.ResourceProducerData
            {
                structure_id = structure.StructureID,
                production_finish_time = ProductionFinishTime,
                producer_state = (int)CurrentProducerState,
            };

            db.AddNewRecord(resourceProducerData);
        }

        //Keep a base amount per claim in case leveling up buildings is possible.
        amountPerClaim = resourceProducer_SO.baseAmountPerClaim;
        timePerClaim = resourceProducer_SO.baseTimeNeededPerClaim;
    }

    public void LoadData(Database.ResourceProducerData data, Database db)
    {
        this.db = db;
        resourceProducerData = data;

        //Not using the property so I don't waste a call to update database.
        productionFinishTime = data.production_finish_time;
        currentProducerState = (ProducerState)data.producer_state;
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
        switch (CurrentProducerState)
        {
            case ProducerState.Waiting:
                {
                    if (structure.CurrentBuildingState == Structure.BuildingState.BUILT)
                    {
                        StartProduction();
                    }
                    break;
                }
            case ProducerState.Ready_To_Claim:
                {
                        // nothing here.
                    if (printTime)
                        print("Ready to Claim!!");
                    break;
                }
            case ProducerState.Producing:
                {
                    TimeSpan timeLeftToClaim = ProductionFinishTime - DateTime.Now;
                    if (printTime) 
                        print("Producing... " + timeLeftToClaim + " seconds left"); // to be replaced with ui

                    if (timeLeftToClaim <= TimeSpan.Zero)
                    {
                        CurrentProducerState = ProducerState.Ready_To_Claim;
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
        ProductionFinishTime = DateTime.Now.Add(timePerClaim);
        CurrentProducerState = ProducerState.Producing;
    }

    public void ClaimResources()
    {
        print("Claimed " + amountPerClaim + resourceProducer_SO.resource_SO.resourceType + "!");
        ResourceManager.Instance.AdjustPlayerResources(FoodResource.Fish, amountPerClaim);
        StartProduction();
    }

    [Button]
    public void OnObjectClicked()
    {
        switch (CurrentProducerState)
        {
            case ProducerState.Waiting:
                {
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
