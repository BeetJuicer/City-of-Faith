using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using AYellowpaper.SerializedCollections;


[CreateAssetMenu(menuName = "Scriptable Objects/Structure_SO")]
public class Structure_SO : ScriptableObject
{
    public string structureName;
    [TextArea]
    public string description;
    public GameObject structurePrefab;
    public GameObject inProgressBuildingPrefab;
    public Sprite displayImage;

    //TODO: maybe change the datatype. temporary for now.
    public DateTime buildDuration;
    public int expGivenOnBuild;

    [Header("Town Stats")]
    public int townHallLevelRequirement;
    public int numberOfCitizensAdded;

    //TODO: make a container?manager? that keeps track of each level's unlockables. maybe the townhall.
    [Header("Build Cost")]

    [SerializedDictionary]
    public SerializedDictionary<Resource, int> resourcesRequired;

    [Tooltip("The resell value must be less than the cost.")]
    [SerializedDictionary]
    public SerializedDictionary<Resource, int> resourcesReturnedOnResell;

    [Header("Time Needed")]
    public int BuildDays;
    public int BuildHours;
    public int BuildMinutes;
    public int BuildSeconds;

}
