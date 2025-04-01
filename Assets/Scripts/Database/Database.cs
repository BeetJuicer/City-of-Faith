using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using SQLite;
using System;
using System.Linq;
using System.Xml.Schema;

public class Database : MonoBehaviour
{
    public static Database Instance { get; private set; }

    private readonly int USERNAME_CHARACTER_LIMIT = 50;
    private readonly int MINIMUM_PASSWORD_LENGTH = 8;

    [SerializeField] private int mainGameIndex;
    [SerializeField] private string testingpo;

    [HideInInspector]
    public string Username { get; private set; }
    //temp for prototype

    private string password = "";
    public int PlayerId { get; private set; }

    SQLiteConnection db;
    [SerializeField] private bool IsGameplayScene;

    #region Publicly Accessible Data (For Visits)

    [System.Serializable]
    public struct PublicStructureData
    {
        public string name;
        public Vector3 position;
    }

    [System.Serializable]
    public class PublicVillageData
    {
        public List<PublicStructureData> structures;
        public int level;
    }

    #endregion


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
    private string path;

    [NaughtyAttributes.Button]
    public void EnterTest()
    {
        SetUser(testingpo);
    }
    private void Awake()
    {
        SceneManager.DontDestroyOnLoad(this);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        CreateDatabaseFileIfNotExists();
    }

    public void SetUser(string s)
    {
        //CloudSaveDB.LoadData(); load data to path
        // db(json now. OR cloud save directly. dictionary dictionary) = path
        // decrypt json, load world, encrypt json


        //SaveData()
        //CloudSave.SaveAsync(playerData, PlayerData)

        /*
         PlayerData{
            <id, structure> structureData,
            <id, rp> rpData,
            <id, p> plotData,
            centralData(one.),
            currency sa unity economy
            resources>? economy?
            
        }
         
         */

        //Changed. Only one player is in the mydb.db
        print("User set with username: " + s);
        var db = new SQLiteConnection(path);

        Username = s;

        //Query for user
        TableQuery<PlayerData> query = db.Table<PlayerData>().Where(row => row.Username.Equals(Username));

        // No player. Start new game.
        //TODO: set flags for tutorials.
        if (query.Count() == 0)
        {
            // Create a new player.
            var newPlayer = new PlayerData()
            {
                Username = Username,
                Password = password
            };
            db.Insert(newPlayer);
            PlayerId = newPlayer.Player_id;
            CurrentPlayerData = newPlayer;
            print($"User {Username} not found. Creating new game with id:{newPlayer.Player_id}");
            return;
        }
        else // not new player
        {
            CurrentPlayerData = query.ToList<PlayerData>().First();
            PlayerId = CurrentPlayerData.Player_id;
            //increase play sessions count
            CurrentPlayerData.Play_sessions += 1;

            db.Update(CurrentPlayerData);
        }

        db.Close();
    }

    public void CreateDatabaseFileIfNotExists()
    {
        path = $"{Application.persistentDataPath}/MyDb.db";

        if (System.IO.File.Exists(path))
            return;

        //TODO FOR DATABASE: PASSWORD CHECKING

        db = new SQLiteConnection(path);

        //print("db_logs: printing all!");
        //var allstructures = db.Table<StructureData>();
        //foreach (var item in allstructures)
        //{
        //    print($"{item.structure_id} owned by {item.player_id}");
        //}


        //activate foreign key constraints
        db.Execute("PRAGMA foreign_keys = ON");

        db.CreateTable<PlayerData>();
        db.CreateTable<StructureData>();
        db.CreateTable<ResourceProducerData>();
        db.CreateTable<PlotData>();
        db.CreateTable<CurrencyData>();
        db.CreateTable<CentralData>();

        db.Close();
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    // When player reenters game.
    //private void OnApplicationFocus(bool focus)
    //{
    //    if (focus)
    //        LoadGame();
    //}

    private void Start()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (CurrentPlayerData == null)
        {
            Debug.LogWarning("Player data is null!");
            return;
        }
        if (scene.buildIndex == mainGameIndex)
        {
            LoadGame();
        }

    }

    private void LoadGame()
    {
        var db = new SQLiteConnection(path);

        var structures = GetStructureData();
        print("db_logs: printing result of GetStructureData()");
        foreach (var item in structures)
        {
            print($"{item.structure_id} owned by {item.player_id}");
        }

        if (structures.Count == 0)
        {
            print($"db_logs: no strsucture found. returning.");
            return;
        }

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
            print($"db_logs: loop passes through {s_data.structure_id}. {s_data.prefab_name}");
            object prefab = Resources.Load(path);
            Debug.Assert(prefab != null, $"{s_data.prefab_name} does not exist in {path}!");
            print($"db_logs: loop found {path}!");

            Vector3 pos = new Vector3(s_data.posX, s_data.posY, s_data.posZ);
            Quaternion rot = new Quaternion(s_data.rotX, s_data.rotY, s_data.rotZ, s_data.rotW);

            print($"db_logs: loop attempting to instantiate prefab {path}!");

            GameObject structure = Instantiate(prefab as GameObject, pos, rot);

            print($"db_logs: Structure is {structure}");
            structure.GetComponent<Structure>().LoadData(s_data, this);

            print($"db_logs: Instantiating structure: {s_data.structure_id}. {s_data.prefab_name}, from player {s_data.player_id}");

            // I don't like this checking each type of structure. Might refactor.
            // Loading ResourceProducerData
            if (structure.TryGetComponent(out ResourceProducer rp))
            {
                print($"Instantiating rp: {resourceProducers[s_data.structure_id].structure_id}, from player {s_data.player_id}");

                rp.LoadData(resourceProducers[s_data.structure_id], this);
            }
            else if (structure.TryGetComponent(out Plot plot))
            {
                // Loading Plot
                print($"Instantiating plot: {plots[s_data.structure_id].structure_id}, from player {s_data.player_id}");
                plot.LoadData(plots[s_data.structure_id], this);
            }
        }

        db.Close();
    }


    public List<StructureData> GetStructureData()
    {
        var db = new SQLiteConnection(path);
        var result = db.Table<StructureData>().Where((row) => row.player_id == PlayerId).ToList();
        db.Close();
        return result;
    }

    public List<PublicStructureData> GetPublicStructureData()
    {
        var db = new SQLiteConnection(path);
        var result = db.Table<StructureData>()
                        .Where((row) => row.player_id == PlayerId && row.building_state == 2) //building_state == 2 is BUILT. Refer to Structure.cs BuildingState
                        .Select(row => new PublicStructureData
                                        {
                                            name = row.prefab_name,
                                            position = new Vector3(row.posX, row.posY, row.posZ)
                                        })
                        .ToList();

        //for testing.
        //foreach (var row in result)
        //{
        //    print($"pb_data: {row.name} at {row.position}");
        //}

        db.Close();

        return result;
    }

    public List<CurrencyData> GetCurrencyData()
    {
        var db = new SQLiteConnection(path);
        var result = db.Table<CurrencyData>().Where((row) => row.player_id == PlayerId).ToList();
        db.Close();
        return result;
    }

    public CentralData GetCentralData()
    {
        var db = new SQLiteConnection(path);
        var centralTable = db.Table<CentralData>().ToList().Where(row => row.player_id == PlayerId);
        db.Close();
        if (centralTable.Count() == 0)
            return null;
        else
            return centralTable.First();

    }

    public void AddNewRecord(IDatabaseData newRecord)
    {
        var db = new SQLiteConnection(path);
        db.Insert(newRecord);
        print("new record added");
        db.Close();
    }

    //debug
    public void AddNewRecord(PlotData newRecord)
    {
        var db = new SQLiteConnection(path);
        db.Insert(newRecord);
        print("new record added");
        db.Close();
    }

    public void UpdateRecord(IDatabaseData newRecord)
    {
        var db = new SQLiteConnection(path);
        db.Update(newRecord);
        db.Close();
    }

    public void UpdateRecords(IEnumerable<IDatabaseData> newRecords)
    {
        var db = new SQLiteConnection(path);
        foreach (var record in newRecords)
        {
            db.Update(record);
        }
        db.Close();
    }

    public void DeleteRecord(IDatabaseData recordToDelete)
    {
        var db = new SQLiteConnection(path);
        db.Delete(recordToDelete);
       db.Close();
    }

}