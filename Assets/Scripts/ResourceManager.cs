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

public enum Resource
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

    [SerializedDictionary] // serialize for now for debug purposes.
    private SerializedDictionary<Resource, int> playerResources;
    private SerializedDictionary<Currency, int> playerCurrencies;
    public int PlayerExp { get; private set; }
    
    [Tooltip("Citizen Satisfaction is 0 -> 1 -> 2. 1 is the baseline.")]
    public int CitizenSatisfaction { get; private set; }
    public int Population { get; private set; }
    public int FoodStashAmount { get; private set; }

    [SerializeField] private int foodUnitsRequiredPerCitizen;

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    public void AddToPlayerResources(Resource type, int amount)
    {
        playerResources[type] += amount;
        CalculateFoodStash();
    }

    public bool HasEnough<T>(Dictionary<T, int>cost, Dictionary<T, int>playerInventory)
    {
        foreach(var keyValue in playerInventory)
        {
            if(playerInventory[keyValue.Key] < keyValue.Value)
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

    public bool HasEnoughResources(Dictionary<Resource, int> cost)
    {
        return HasEnough(cost, playerResources);
    }

    public int CalculateFoodStash()
    {
        FoodStashAmount = playerResources[Resource.Fish] +
                          playerResources[Resource.Meat] +
                          playerResources[Resource.Plant];

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
