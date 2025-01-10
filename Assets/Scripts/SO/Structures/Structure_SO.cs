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
    public Sprite displayImage;
    public string structurePrefabName;
    public string minigameSceneName;

    [Header("Category")]
    public ItemCategory Category;

    //TODO: maybe change the datatype. temporary for now.
    public DateTime buildDuration;
    public int expGivenOnBuild;

    [Header("Town Stats")]
    public int townHallLevelRequirement;
    public int numberOfCitizensAdded;

    [Header("Build Cost")]

    [SerializedDictionary]
    public SerializedDictionary<Currency, int> currencyRequired;

    [SerializedDictionary]
    public SerializedDictionary<FoodResource, int> resourcesRequired;

    [Tooltip("The resell value must be less than the cost.")]
    [SerializedDictionary]
    public SerializedDictionary<FoodResource, int> resourcesReturnedOnResell;

    [Header("Time Needed")]
    public int BuildDays;
    public int BuildHours;
    public int BuildMinutes;
    public int BuildSeconds;

}

public enum ItemCategory
{
    Buildings,
    Decorations
}