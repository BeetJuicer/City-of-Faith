using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Linq;
using System;
using NaughtyAttributes;

public class CentralHall : MonoBehaviour
{
    private Database db;
    private Database.CentralData centralData = null;
    public event Action<int> OnPlayerLevelUp;
    public event Action<int> OnExpChanged;

    [SerializeField] private Unlockables_SO unlockables_SO;
    bool maxLevel;
    private int level;
    public int Level
    {
        get => level;
        private set
        {
            if (maxLevel) return;

            if (value >= unlockables_SO.unclockableCrops.Count ||
               value >= unlockables_SO.unclockableStructures.Count)
            {
                Debug.LogWarning("You've reached max level! Have UI here.");
                Exp = levelUpExpRequirements[Level];
                maxLevel = true;
                return;
            }

            level = value;
            centralData.level = level;
            if (db == null)
                db = FindObjectOfType<Database>();

            db.UpdateRecord(centralData);
            OnPlayerLevelUp?.Invoke(value);

        }
    }

    [Button]
    public void LevelUp()
    {
        if (maxLevel) return;

        Level++;
        Debug.Log("Current Level: " + Level);
    }

    private int exp;
    public int Exp
    {
        get => exp;
        private set
        {
            if (maxLevel) return;

            exp = value;
            centralData.exp = exp;
            if (db == null)
                db = FindObjectOfType<Database>();

            db.UpdateRecord(centralData);
            OnExpChanged?.Invoke(value);
        }
    }

    CentralHall ch;
    //text.text = ch.Exp + "/" + ch.levelUpRequirements[ch.Level]

    [SerializedDictionary("Level", "Exp Required Until Level Up")]
    [SerializeField] private SerializedDictionary<int, int> levelUpExpRequirements = new();
    public SerializedDictionary<int, int> LevelUpExpRequirements { get => levelUpExpRequirements; private set => levelUpExpRequirements = value; }

    private List<Structure_SO> unlockedStructures = new();
    public List<Structure_SO> UnlockedStructures { get => GetUnlockedStructureSOs(); private set { unlockedStructures = value; } }

    private List<Crop_SO> unlockedCrops = new();
    public List<Crop_SO> UnlockedCrops { get => GetUnlockedCropSOs(); private set { unlockedCrops = value; } }

    public List<Crop_SO> LockedCrops { get => GetLockedCropSOs(); private set { LockedCrops = value; } }
    public List<Structure_SO> LockedStructures { get => GetLockedStructureSOs(); private set { LockedStructures = value; } }


    private void Start()
    {
        db = FindFirstObjectByType<Database>();
        LoadDatabaseOrDefault();
    }

    public void LoadData(Database.CentralData data)
    {
        this.centralData = data;
        //not using property since this is initialization
        level = data.level; 
        exp = data.exp;
    }

    private void LoadDatabaseOrDefault()
    {
        if (centralData == null)
            LoadDefault();
        else
        {
            //do nothing. the database handles loading.
        }
    }

    private void LoadDefault()
    {
        level = 1;
        UnlockedCrops.Concat(unlockables_SO.unclockableCrops[level]);
        UnlockedStructures.Concat(unlockables_SO.unclockableStructures[level]);
        centralData = new Database.CentralData
        {
            player_id = db.PlayerId,
            level = level,
            exp = exp,
        };
        db.AddNewRecord(centralData);
    }

    public void AddToCentralExp(int value)
    {
        //83 + 25 : 100
        Exp += value;

        //108 : 100
        if (Exp >= LevelUpExpRequirements[Level])
        {
            LevelUp(Exp - LevelUpExpRequirements[Level]);
        }
    }

    private void LevelUp(int remainder)
    {
        if (maxLevel) return;

        Level++;
        Exp = remainder;
        //TODO: optimize and store locked structures only once, not everytime they're fetched.
        UnlockedStructures.Concat(unlockables_SO.unclockableStructures[level]);
        UnlockedCrops.Concat(unlockables_SO.unclockableCrops[level]);

        //if exp is still greater, call level up again
        if (Exp >= levelUpExpRequirements[Level])
        {
            LevelUp(Exp - LevelUpExpRequirements[Level]);
        }

    }

    private List<Structure_SO> GetLockedStructureSOs()
    {
        List<Structure_SO> locked = new();
        for (int i = Level + 1; i < unlockables_SO.unclockableStructures.Count; i++)
        {
            locked = locked.Concat(unlockables_SO.unclockableStructures[i]).ToList();
        }

        return locked;
    }

    private List<Structure_SO> GetUnlockedStructureSOs()
    {
        List<Structure_SO> unlocked = new();
        for (int i = 0; i <= Level; i++)
        {
            unlocked = unlocked.Concat(unlockables_SO.unclockableStructures[i]).ToList();
        }
        return unlocked;
    }

    private List<Crop_SO> GetUnlockedCropSOs()
    {
        List<Crop_SO> unlocked = new();
        for (int i = 0; i <= Level; i++)
        {
            unlocked = unlocked.Concat(unlockables_SO.unclockableCrops[i]).ToList();
        }
        return unlocked;
    }

    private List<Crop_SO> GetLockedCropSOs()
    {
        List<Crop_SO> locked = new();
        for (int i = Level + 1; i < unlockables_SO.unclockableCrops.Count; i++)
        {
            locked = locked.Concat(unlockables_SO.unclockableCrops[i]).ToList();
        }

        return locked;
    }

}
