using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Linq;
using System;
public enum Currency
{
    // BE CAREFUL WITH CHANGING. ENUM IS STORED AS INT IN DATABASE. DON'T CHANGE ORDER UNLESS ABSOLUTELY NEEDED !!!
    Gold = 1,
    Glory = 2,
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
    public static ResourceManager Instance { get { return _instance; } private set { _instance = value; } }

    private Database db;

    // Set as a dictionary so that we can update each database record individually when set instead of updating the entire list of currencies.
    private Dictionary<Currency, Database.CurrencyData> currencyData = null;

    public event Action<Currency, int> OnCurrencyUpdated;
    public event Action<int> OnCurrencyLoaded;

    // serialize for now for debug purposes.
    [SerializedDictionary]
    [SerializeField] private SerializedDictionary<FoodResource, int> playerFoodResources = new();
    private SerializedDictionary<FoodResource, int> PlayerFoodResources
    {
        get => playerFoodResources;
        set
        {
            playerFoodResources = value;
            UpdateFoodStashTotal();
        }
    }

    private Dictionary<Currency, int> playerCurrencies = new SerializedDictionary<Currency, int>();
    public Dictionary<Currency, int> PlayerCurrencies
    {
        get => playerCurrencies;
        private set
        {
            playerCurrencies = value;
            // AddToPlayerCurrencies handles updating the single record in database.
        }
    }

    [SerializedDictionary]
    [SerializeField] private SerializedDictionary<Currency, int> startingPlayerCurrencies = new();

    public int PlayerExp { get; private set; }

    [Tooltip("Citizen Satisfaction is 0 -> 1 -> 2. 1 is the baseline.")]
    public int CitizenSatisfaction { get; private set; }
    public int Population { get; private set; }
    public int FoodStashTotalAmount { get; private set; }

    [SerializeField] private int foodUnitsRequiredPerCitizen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null && Instance != this)
        {
            Destroy(this);
        }

    }

    private void Start()
    {
        db = FindFirstObjectByType<Database>();
        Debug.Assert(db != null, "No gameobject with Database component found in scene!");

        currencyData = db.GetCurrencyData().ToDictionary(record => (Currency)record.currency_type);

        // No data found. Must be new player.
        if (currencyData.Count <= 0)
        {
            InitializeDefaultCurrencyAndData();
            print("Initializing default currency");
        }
        else
        {
            // Load player currencies from data.
            print("Initializing old player currency");
            PlayerCurrencies = currencyData.ToDictionary(pair => pair.Key, pair => pair.Value.amount);
        }


        print("Currencies: " + PlayerCurrencies);
        foreach (KeyValuePair<Currency, int> kv in PlayerCurrencies)
        {
            print(kv.Key + ": " + kv.Value);
        }

        OnCurrencyLoaded?.Invoke(1);
    }

    private void InitializeDefaultCurrencyAndData()
    {
        //curency data.
        foreach (Currency type in Enum.GetValues(typeof(Currency)))
        {
            Database.CurrencyData typeData = new()
            {
                player_id = db.PlayerId,
                currency_type = (int)type,
                amount = startingPlayerCurrencies[type]
            };
            currencyData.Add(type, typeData);
            db.AddNewRecord(typeData);
        }

        //actual currency
        PlayerCurrencies = startingPlayerCurrencies;
    }

    // Always use this to set player currency.
    private void SetPlayerCurrency(Currency type, int newValue)
    {
        if (newValue < 0)
            Debug.LogError("Negative Currency detected!!");

        print($"Oncurrencyupdated is invoked with newValue: {newValue}, oldValue is:  {playerCurrencies[type]}");
        playerCurrencies[type] = newValue;
        currencyData[type].amount = newValue;

        OnCurrencyUpdated?.Invoke(type, newValue);
        //ResourceManager.Instance.OnCurrencyUpdated +=

        db.UpdateRecord(currencyData[type]);
    }

    public void AdjustPlayerCurrency(Currency type, int amount)
    {
        SetPlayerCurrency(type,
            PlayerCurrencies[type] + amount);
    }

    public void AdjustPlayerCurrency(Dictionary<Currency, int> amounts)
    {
        foreach (KeyValuePair<Currency, int> p in amounts)
        {
            SetPlayerCurrency(p.Key, PlayerCurrencies[p.Key] + p.Value);
        }
    }

    public void AdjustPlayerResources(FoodResource type, int amount)
    {
        //print("Added " + amount + " units of " + type.ToString() + " to storage!");

        PlayerFoodResources[type] += amount;
    }

    public bool HasEnoughCurrency(Currency currency, int amount)
    {
        //print($"Checking player:{PlayerCurrencies[currency]} is greater than {amount}?: {PlayerCurrencies[currency] > amount}");
        return PlayerCurrencies[currency] > amount;
    }

    public bool HasEnough<T>(Dictionary<T, int> cost, Dictionary<T, int> playerInventory)
    {
        foreach (var keyValue in cost)
        {
            //print($"Checking {keyValue.Key}.... player: {playerInventory[keyValue.Key]}, cost: {keyValue.Value} ");
            if (playerInventory[keyValue.Key] < keyValue.Value)
            {
                //print("Returning false");
                return false;
            }
        }
        //print("Returning true");
        return true;
    }

    public bool HasEnoughCurrency(Dictionary<Currency, int> cost)
    {
        return HasEnough(cost, PlayerCurrencies);
    }

    public bool HasEnoughResources(Dictionary<FoodResource, int> cost)
    {
        return HasEnough(cost, PlayerFoodResources);
    }

    //Necessary for calculating how much TOTAL food we have.
    public void UpdateFoodStashTotal()
    {
        int sum = 0;
        foreach(FoodResource type in Enum.GetValues(typeof(FoodResource)))
        {
            sum += PlayerFoodResources[type];
        }

        FoodStashTotalAmount = sum;
    }

    public void CalculateCitizenSatisfaction()
    {
        if (FoodStashTotalAmount / foodUnitsRequiredPerCitizen < 1)
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

    public void TriggerCurrencyUpdated(Currency currencyType, int newAmount)
    {
        OnCurrencyUpdated?.Invoke(currencyType, newAmount);
    }

}
