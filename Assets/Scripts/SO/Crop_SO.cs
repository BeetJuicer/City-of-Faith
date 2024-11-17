using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Scriptable Objects/Crop_SO")]
public class Crop_SO : ScriptableObject
{
    public string cropName;
    public string cropDetails;
    public int cropPrice;
    public Sprite cropImage;
    public GameObject cropPrefab;

    public int amountPerClaim;

    [Header("Time Needed Per Claim")]
    public int daysToClaim;
    public int hoursToClaim;
    public int minutesToClaim;
    public int secondsToClaim;

    public TimeSpan baseTimeNeededPerClaim;

    private void OnValidate()
    {
        baseTimeNeededPerClaim = new TimeSpan(daysToClaim, hoursToClaim, minutesToClaim, secondsToClaim);
    }
}
