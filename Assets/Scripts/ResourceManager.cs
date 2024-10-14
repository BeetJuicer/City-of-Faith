using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Linq;

public enum Currency
{
    Gold,
    Glory,
}

public enum FoodResource
{
    Fish,
    Meat,
    Plant,
    //pelts, stone, wood
}


public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager Instance { get { return _instance; } }


    // serialize for now for debug purposes.
    [SerializedDictionary] 
    [SerializeField] private SerializedDictionary<FoodResource, int> playerFoodResources = new SerializedDictionary<FoodResource, int>();

    // serialize for now for debug purposes.
    [SerializedDictionary]
    [SerializeField] private SerializedDictionary<Currency, int> playerCurrencies = new SerializedDictionary<Currency, int>();


    public int PlayerExp { get; private set; }

    [Tooltip("Citizen Satisfaction is 0 -> 1 -> 2. 1 is the baseline.")]
    public int CitizenSatisfaction { get; private set; }
    public int Population { get; private set; }
    public int FoodStashAmount { get; private set; }

    [SerializeField] private int foodUnitsRequiredPerCitizen;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    public void AddToPlayerResources(FoodResource type, int amount)
    {
        print("Added " + amount + " units of " + type + " to storage!");

        playerFoodResources[type] += amount;
        CalculateFoodStash();
    }

    public bool HasEnough<T>(Dictionary<T, int> cost, Dictionary<T, int> playerInventory)
    {
        print(playerInventory);
        foreach (var keyValue in playerInventory)
        {
            if (playerInventory[keyValue.Key] < keyValue.Value)
            {
                return false;
            }
        }
        return true;
    }

    public bool HasEnoughCurrency(Dictionary<Currency, int> cost)
    {
        return HasEnough(cost, playerCurrencies);
    }

    public bool HasEnoughResources(Dictionary<FoodResource, int> cost)
    {
        return HasEnough(cost, playerFoodResources);
    }

    public int CalculateFoodStash()
    {
        FoodStashAmount = playerFoodResources[FoodResource.Fish] +
                          playerFoodResources[FoodResource.Meat] +
                          playerFoodResources[FoodResource.Plant];

        return FoodStashAmount;
    }

    public void CalculateCitizenSatisfaction()
    {
        if(FoodStashAmount / foodUnitsRequiredPerCitizen < 1)
        {
            // not enough food per citizen
            // prosperous
            // full
            // satisfaction state = satisfied
            // satisfaction state = hungry
            // satisfaction state = starving
            // satisfaction state = famine
        }
    }
}
