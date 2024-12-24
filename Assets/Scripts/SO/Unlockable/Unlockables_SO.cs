using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;


[CreateAssetMenu(menuName = "Scriptable Objects/Unlockables_SO")]
public class Unlockables_SO : ScriptableObject
{
    [SerializedDictionary("Level", "Structures")]
    public SerializedDictionary<int, List<Structure_SO>> unclockableStructures = new();

    [SerializedDictionary("Level", "Crops")]
    public SerializedDictionary<int, List<Crop_SO>> unclockableCrops = new();

    [SerializedDictionary("Level", "Population Limit")]
    public SerializedDictionary<int, int> populationLimit = new();
}
