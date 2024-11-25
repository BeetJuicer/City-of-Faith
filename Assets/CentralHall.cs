using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Linq;

public class CentralHall : MonoBehaviour
{
    private Database db;
    private Database.CentralData centralData = null;

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
    private SerializedDictionary<int, int> levelUpExpRequirements = new();

    private List<Structure_SO> unlockedStructures;
    public List<Structure_SO> UnlockedStructures { get => unlockedStructures; private set { unlockedStructures = value; } }

    private List<Crop_SO> unlockedCrops;
    public List<Crop_SO> UnlockedCrops { get => unlockedCrops; private set { unlockedCrops = value; } }

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
        if(centralData == null)
            LoadDefault();
        else
        {
            //do nothing. the database handles loading.
        }
    }

    private void LoadDefault()
    {
        Level = 1;
        UnlockedCrops.Concat(unlockables_SO.unclockableCrops[level]);
        UnlockedStructures.Concat(unlockables_SO.unclockableStructures[level]);
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
        Exp = remainder;
    }

    private List<Structure_SO> GetLockedStructureSOs()
    {
        List<Structure_SO> locked = new();
        for (int i = Level + 1; i < unlockables_SO.unclockableStructures.Count; i++)
        {
            locked.Concat(unlockables_SO.unclockableStructures[i]);
        }

        return locked;
    }

    private List<Crop_SO> GetLockedCropSOs()
    {
        List<Crop_SO> locked = new();
        for (int i = Level + 1; i < unlockables_SO.unclockableCrops.Count; i++)
        {
            locked.Concat(unlockables_SO.unclockableCrops[i]);
        }

        return locked;
    }

}
