using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "Structure Scriptable Object")]
public class Structure_SO : ScriptableObject
{
    public string structureName;
    [TextArea]
    public string description;
    public GameObject structurePrefab;
    public Sprite displayImage;

    //TODO: maybe change the datatype. temporary for now.
    public DateTime buildDuration;
    public int goldCost;
    public int townHallLevelRequirement;
    
    //TODO: make a container?manager? that keeps track of each level's unlockables. maybe the townhall.
}
