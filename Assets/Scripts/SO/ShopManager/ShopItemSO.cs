using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using AYellowpaper.SerializedCollections;


[CreateAssetMenu(fileName = "ShopMenu", menuName = "Scriptable Objects/New Shop Item", order = 1)]
public class ShopItemSO : ScriptableObject
{
    public string title;
    public string description;
    [SerializedDictionary("Currency", "Price")]
    public SerializedDictionary<Currency, int> baseCost;
    public Sprite itemImage;

    public ItemCategory category;
}

public enum ItemCategory
{
    Buildings,
    Decoration,
    Crop
}
