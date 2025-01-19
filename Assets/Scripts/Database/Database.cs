using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;
using System;
using System.Linq;

public class Database : MonoBehaviour
{
    public static Database Instance { get; private set; }

    private readonly int USERNAME_CHARACTER_LIMIT = 50;
    private readonly int MINIMUM_PASSWORD_LENGTH = 8;

    [SerializeField] private string username = "Test Tickles";
    private string password = "";
    public int PlayerId { get; private set; }

    SQLiteConnection db;

    public interface IDatabaseData { }

    [Table("tbl_player")]
    public class PlayerData : IDatabaseData
    {
        [PrimaryKey, AutoIncrement]
        public int Player_id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Play_sessions { get; set; }

        public bool HasSeenCutscene { get; set; } // Flag to track cutscene viewing
    }

    [Table("tbl_structure")]
    public class StructureData : IDatabaseData
    {
        [PrimaryKey, AutoIncrement]
        public int structure_id { get; set; }

        [NotNull]
        public int player_id { get; set; }
        [NotNull]
        public string prefab_name { get; set; }
        [NotNull]
        public DateTime time_build_finished { get; set; }
        [NotNull]
        public int building_state { get; set; }
        [NotNull]
        public float posX { get; set; }
        [NotNull]
        public float posY { get; set; }
        [NotNull]
        public float posZ { get; set; }
        [NotNull]
        public float rotW { get; set; }
        [NotNull]
        public float rotX { get; set; }
        [NotNull]
        public float rotY { get; set; }
        [NotNull]
        public float rotZ { get; set; }
    }

    [Table("tbl_resourceProducer")]
    public class ResourceProducerData : IDatabaseData
    {
        [PrimaryKey, NotNull]
        public int structure_id { get; set; }
        [NotNull]
        public int producer_state { get; set; }
        public DateTime production_finish_time { get; set; }
    }

    [Table("tbl_plot")]
    public class PlotData : IDatabaseData
    {
        [PrimaryKey, NotNull]
        public int structure_id { get; set; }
        [NotNull]
        public int plot_state { get; set; }
        public DateTime growth_finish_time { get; set; }
        public string crop_so_name { get; set; }
    }

    [Table("tbl_currency")]
    public class CurrencyData : IDatabaseData
    {
        [PrimaryKey, NotNull, AutoIncrement]
        public int currency_id { get; set; }
        [NotNull]
        public int player_id { get; set; }
        [NotNull]
        public int currency_type { get; set; }
        public int amount { get; set; }
    }


    [Table("tbl_central")]
    public class CentralData : IDatabaseData
    {
        [PrimaryKey, NotNull]
        public int player_id { get; set; }
        [NotNull]
        public int level { get; set; }
        [NotNull]
        public int exp { get; set; }
    }

    public PlayerData CurrentPlayerData { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        //TODO FOR DATABASE: PASSWORD CHECKING

        db = new SQLiteConnection($"{Application.persistentDataPath}/MyDb.db");

        //activate foreign key constraints
        db.Execute("PRAGMA foreign_keys = ON");

        db.CreateTable<PlayerData>();
        db.CreateTable<StructureData>();
        db.CreateTable<ResourceProducerData>();
        db.CreateTable<PlotData>();
        db.CreateTable<CurrencyData>();
        db.CreateTable<CentralData>();

        //Query for user
        TableQuery<PlayerData> query = db.Table<PlayerData>().Where(row => row.Username.Equals(username));

        // No player. Start new game.
        //TODO: set flags for tutorials.
        if (query.Count() == 0)
        {
            // Create a new player.
            var newPlayer = new PlayerData()
            {
                Username = username,
                Password = password
            };
            db.Insert(newPlayer);
            PlayerId = newPlayer.Player_id;
            print($"User {username} not found. Creating new game.");
            return;
        }
        else
        {
            CurrentPlayerData = query.ToList<PlayerData>().First();
            PlayerId = CurrentPlayerData.Player_id;
            //increase play sessions count
            CurrentPlayerData.Play_sessions += 1;

            print($"User {username} found. Loading Game.");
            db.Update(CurrentPlayerData);
            LoadGame();
        }
    }

    // When player reenters game.
    //private void OnApplicationFocus(bool focus)
    //{
    //    if (focus)
    //        LoadGame();
    //}

    private void LoadGame()
    {
        var structures = GetStructureData();

        // Keeping structureIds for multiple queries.
        var structureIds = structures.Select(s => s.structure_id).ToList();

        // Resource Producer Data from Database
        // All resource producer data with structure_id in 'structureIds'.
        // Each structure id is unique, so we're sure that everything in 'structureIds' is owned by this current player.
        Dictionary<int, ResourceProducerData> resourceProducers = db.Table<ResourceProducerData>()
                          .Where(row => structureIds.Contains(row.structure_id))
                          .ToList().ToDictionary(rp => rp.structure_id);

        // Same for Plots
        Dictionary<int, PlotData> plots = db.Table<PlotData>()
                  .Where(row => structureIds.Contains(row.structure_id))
                  .ToList().ToDictionary(rp => rp.structure_id);


        // Loading the central hall.
        CentralData centralData = GetCentralData();
        CentralHall central = FindFirstObjectByType<CentralHall>();
        central.LoadData(centralData);

        // Loading the objects. TODO: Calling resources.load is pretty inefficient each time. Use something else.
        foreach (StructureData s_data in structures)
        {
            // Instantiation of GameObject
            string path = $"Structures/{s_data.prefab_name}";
            var prefab = Resources.Load(path);
            Debug.Assert(prefab != null, $"{s_data.prefab_name} does not exist in {path}!");

            Vector3 pos = new Vector3(s_data.posX, s_data.posY, s_data.posZ);
            Quaternion rot = new Quaternion(s_data.rotX, s_data.rotY, s_data.rotZ, s_data.rotW);


            GameObject structure = (GameObject)Instantiate(prefab, pos, rot);
            structure.GetComponent<Structure>().LoadData(s_data, this);

            print($"Instantiating structure: {s_data.structure_id}. {s_data.prefab_name}, from player {s_data.player_id}");

            // I don't like this checking each type of structure. Might refactor.
            // Loading ResourceProducerData
            if (structure.TryGetComponent(out ResourceProducer rp))
            {
                print($"Instantiating rp: {resourceProducers[s_data.structure_id]}, from player {s_data.player_id}");

                rp.LoadData(resourceProducers[s_data.structure_id], this);
            }
            else if (structure.TryGetComponent(out Plot plot))
            {
                // Loading Plot
                print($"Instantiating plot: {plots[s_data.structure_id]}, from player {s_data.player_id}");
                plot.LoadData(plots[s_data.structure_id], this);
            }
        }
    }


    public List<StructureData> GetStructureData()
    {
        return db.Table<StructureData>().Where((row) => row.player_id == PlayerId).ToList();
    }

    public List<CurrencyData> GetCurrencyData()
    {
        return db.Table<CurrencyData>().Where((row) => row.player_id == PlayerId).ToList();
    }

    public CentralData GetCentralData()
    {
        var centralTable = db.Table<CentralData>().ToList().Where(row => row.player_id == PlayerId);
        if (centralTable.Count() == 0)
            return null;
        else
            return centralTable.First();
    }

    public void AddNewRecord(IDatabaseData newRecord)
    {
        print("new record added");
        db.Insert(newRecord);
    }

    public void UpdateRecord(IDatabaseData newRecord)
    {
        db.Update(newRecord);
    }

    public void UpdateRecords(IEnumerable<IDatabaseData> newRecords)
    {
        foreach (var record in newRecords)
        {
            db.Update(record);
        }
    }

    public void DeleteRecord(IDatabaseData recordToDelete)
    {
        db.Delete(recordToDelete);
    }

}