using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Linq;
using System;

public class CentralHall : MonoBehaviour
{
    private Database db;
    private Database.CentralData centralData = null;
    public event Action<int> OnPlayerLevelUp;

    [SerializeField] private Unlockables_SO unlockables_SO;
    private int level;
    public int Level
    {
        get => level;
        private set
        {
            level = value;
            centralData.level = level;
            db.UpdateRecord(centralData);
            OnPlayerLevelUp?.Invoke(value);
        }
    }

    private int exp;
    public int Exp
    {
        get => exp;
        private set
        {
            exp = value;
            centralData.exp = exp;
            db.UpdateRecord(centralData);
        }
    }

    [SerializedDictionary("Level", "Exp Required Until Level Up")]
    [SerializeField] private SerializedDictionary<int, int> levelUpExpRequirements = new();

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
        Exp += value;
        if (Exp >= levelUpExpRequirements[Level])
        {
            LevelUp(levelUpExpRequirements[Level] - Exp);
        }
    }

    private void LevelUp(int remainder)
    {
        Level++;
        UnlockedStructures.Concat(unlockables_SO.unclockableStructures[level]);
        UnlockedCrops.Concat(unlockables_SO.unclockableCrops[level]);
        //TODO: optimize and store locked structures only once, not everytime they're fetched.
        //List<car> result = list2.Except(list1).ToList()
        Exp = remainder;
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
