using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Resource Producer_SO")]
public class ResourceProducer_SO : ScriptableObject
{
    public FoodResource_SO resource_SO;

    public int baseAmountPerClaim;
    [Header("Time Needed Per Claim")]
    public int daysToClaim;
    public int hoursToClaim;
    public int minutesToClaim;
    public int secondsToClaim;

    public int expGivenPerClaim;

    public TimeSpan baseTimeNeededPerClaim { get => new TimeSpan(daysToClaim, hoursToClaim, minutesToClaim, secondsToClaim); set => baseTimeNeededPerClaim = value; }
}
