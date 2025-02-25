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
public class ResourceProducer : MonoBehaviour, IClickableObject, IBoostableObject
{
    private Structure structure;
    private GlorySpeedUp glorySpeedUpUI;


    // Database
    private Database db;
    private Database.ResourceProducerData resourceProducerData = null;

    [SerializeField] private ResourceProducer_SO rp_SO;
    private int amountPerClaim;
    private TimeSpan timePerClaim;

    //temp debug bool
    public bool printTime = false;
    private bool hasPopped = false;

    UIManager uiManager;
    CentralHall centralHall;

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

        if (resourceProducerData == null)
        {
            db = FindFirstObjectByType<Database>();
            resourceProducerData = new Database.ResourceProducerData
            {
                structure_id = structure.StructureID,
                production_finish_time = ProductionFinishTime,
                producer_state = (int)CurrentProducerState,
            };

            db.AddNewRecord(resourceProducerData);
            print($"db_logs: Added resource producer {resourceProducerData.structure_id} to database.");
        }
        else
        {
            print($"db_logs: rp data {resourceProducerData.structure_id} not null! should be loaded");
        }

        //Keep a base amount per claim in case leveling up buildings is possible.
        amountPerClaim = rp_SO.baseAmountPerClaim;
        timePerClaim = new TimeSpan(rp_SO.daysToClaim, rp_SO.hoursToClaim, rp_SO.minutesToClaim, rp_SO.secondsToClaim);

        centralHall = FindObjectOfType<CentralHall>();
        uiManager = FindObjectOfType<UIManager>();
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
        print("Claimed " + amountPerClaim + rp_SO.resource_SO.resourceType + "!");
        ResourceManager.Instance.AdjustPlayerResources(FoodResource.Fish, amountPerClaim);
        //TODO: prototype - temporary claiming of gold
        ResourceManager.Instance.AdjustPlayerCurrency(Currency.Gold, amountPerClaim);
        centralHall.AddToCentralExp(rp_SO.expGivenPerClaim);

        StartProduction();
    }

    [Button]
    public void OnObjectClicked()
    {

        if (hasPopped == false)
        {
            PopOnClick();
        }

        switch (CurrentProducerState)
        {
            case ProducerState.Waiting:
                {
                    //do nothing

                    break;
                }
            case ProducerState.Ready_To_Claim:
                {
                    ClaimResources();
                    break;
                }
            case ProducerState.Producing:
                {
                    uiManager.ActivateSellButton(GetComponent<Structure>());
                    uiManager.ActivateInfoButton(GetComponent<Structure>());
                    uiManager.ActivateBoostButton(this);
                    uiManager.ActivateMinigameButton(GetComponent<Structure>());
                    break;
                }
            default:
                {
                    print("Unhandled Resource State.");
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

    public void BoostProgress()
    {
        CurrentProducerState = ProducerState.Ready_To_Claim;
    }

    public bool IsInBoostableState()
    {
        return CurrentProducerState == ProducerState.Producing;
    }

    public DateTime GetTimeFinished()
    {
        return ProductionFinishTime;
    }

    public TimeSpan GetTotalDuration()
    {
        return timePerClaim;
    }
}
