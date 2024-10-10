using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ResourceProducer : Structure
{
    //timesinceproductionstarted
    //timeleft
    //on exit, store time left. or

    //store time when production starts, check for time when it will end.
    //resource scriptable object.
    [SerializeField] private ResourceProducer_SO resourceProducer_SO;
    private int amountPerClaim;

    public bool IsReadyToClaim { get; private set; }

    private TimeSpan timePerClaim;
    private DateTime startTime;
    private DateTime finishTime;

    private TimeSpan timeLeftToClaim;

    //Timer
    private float updateTimer;


    private void Start()
    {
        amountPerClaim = resourceProducer_SO.baseAmountPerClaim;
        timePerClaim = resourceProducer_SO.baseTimeNeededPerClaim;
    }

    private void Update()
    {
        // Checking only once per second.
        if(Time.time > (updateTimer + 1))
        {
            updateTimer = Time.time;
            timeLeftToClaim = finishTime - DateTime.Now;
            IsReadyToClaim = timeLeftToClaim <= TimeSpan.Zero;
        }
    }

    //
    public void ClaimResources()
    {
        ResourceManager.Instance.AddToPlayerResources(resourceProducer_SO.resource_SO.resourceType, amountPerClaim);
        startTime = DateTime.Now;
        finishTime = startTime.Add(timePerClaim);
    }

    private void OnApplicationQuit()
    {
        //Store the time here.
    }
}
