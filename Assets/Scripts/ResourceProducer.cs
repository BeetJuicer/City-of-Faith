using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;

[RequireComponent(typeof(Structure))]
public class ResourceProducer : MonoBehaviour, IClickableObject
{
    private Structure structure;

    private enum ResourceState
    {
        Producing,
        ReadyToClaim,
    }

    private ResourceState resourceState;

    [SerializeField] private ResourceProducer_SO resourceProducer_SO;
    private int amountPerClaim;

    private TimeSpan timePerClaim;
    private DateTime startTime;
    private DateTime finishTime;

    //temp maybe
    private bool structureReady = false;

    // Timer
    private float updateTimer;

    private void Awake()
    {
        resourceState = ResourceState.Producing;
    }

    private void Start()
    {
        structure = GetComponent<Structure>();

        amountPerClaim = resourceProducer_SO.baseAmountPerClaim;
        timePerClaim = resourceProducer_SO.baseTimeNeededPerClaim;
    }

    private void Update()
    {
        // Checking only twice per second.
        if (Time.time <= (updateTimer + 0.5f))
        {
            return;
        }

        updateTimer = Time.time;

        // Only start production when built.
        if (structure.buildingState == Structure.BuildingState.InProgress)
        {
            return;
        }
        else if (!structureReady)
        {
            structureReady = true;
            startTime = DateTime.Now;
            finishTime = DateTime.Now.Add(timePerClaim);
        }

        // State handling
        switch (resourceState)
        {
            case ResourceState.ReadyToClaim:
                {
                    // nothing here.
                    print("Ready to Claim!!");
                    break;
                }
            case ResourceState.Producing:
                {
                    TimeSpan timeLeftToClaim = finishTime - DateTime.Now;
                    print("Producing... " + timeLeftToClaim + " seconds left");
                    if (timeLeftToClaim <= TimeSpan.Zero)
                    {
                        resourceState = ResourceState.ReadyToClaim;
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

    public void ClaimResources()
    {
        print("Claimed " + amountPerClaim + resourceProducer_SO.resource_SO.resourceType + "!");
        ResourceManager.Instance.AddToPlayerResources(FoodResource.Fish, amountPerClaim);

        startTime = DateTime.Now;
        finishTime = startTime.Add(timePerClaim);

        resourceState = ResourceState.Producing;
    }

    private void OnApplicationQuit()
    {
        //Store the time here.
    }

    [Button]
    public void OnObjectClicked()
    {
        switch (resourceState)
        {
            case ResourceState.ReadyToClaim:
                {
                    ClaimResources();
                    break;
                }
            case ResourceState.Producing:
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
