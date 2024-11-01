using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;
using System;
using System.Linq;

public class Database : MonoBehaviour
{
    private readonly int USERNAME_CHARACTER_LIMIT = 50;
    private readonly int MINIMUM_PASSWORD_LENGTH = 8;

    private string username = "New";
    private string password = "Player";
    private int playerId = 3;

    SQLiteConnection db;

    [Table("tbl_player")]
    public class PlayerData
    {
        [PrimaryKey, AutoIncrement]
        public int Player_id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [Table("tbl_structure")]
    public class StructureData
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
    public class ResourceProducerData
    {
        [PrimaryKey, NotNull]
        public int structure_id { get; set; }
        [NotNull]
        public int producer_state { get; set; }
        public DateTime production_finish_time { get; set; }
    }
    
    [Table("tbl_plot")]
    public class PlotData
    {
        [PrimaryKey, NotNull]
        public int structure_id { get; set; }
        [NotNull]
        public int plot_state { get; set; }
        public DateTime growth_finish_time { get; set; }
        public string crop_so_name { get; set; }
    }

    private void Awake()
    {
        db = new SQLiteConnection($"{Application.persistentDataPath}/MyDb.db");
        db.CreateTable<PlayerData>();

        //TODO FOR DATABASE: PASSWORD CHECKING

        // CREATE A STATEMENT CHECKING IF USER EXISTS
        var query = db.Table<PlayerData>().Where(row => row.Username.Equals(username));

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
            return;
        }
        db.CreateTable<StructureData>();
        db.CreateTable<ResourceProducerData>();

        // Player Exists. Load data.
        Debug.Log("Player exists.");
        LoadGame();
    }

    // When player reenters game.
    private void OnApplicationFocus(bool focus)
    {
        //if (focus)
        //    LoadGame();
        //else
        //    SaveGame();
    }

    // When player goes to home screen.
    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveGame();
    }

    private void SaveGame()
    {

    }

    private void LoadGame()
    {
        var structures = DatabaseGetStructureData();

        // Keeping structureIds for multiple queries.
        var structureIds = structures.Select(s => s.structure_id).ToList();

        // Resource Producer Data from Database
        // All resource producer data with structure_id in 'structureIds'.
        // Each structure id is unique, so we're sure that everything in 'structureIds' is owned by this current player.
        Dictionary<int, ResourceProducerData> resourceProducers = db.Table<ResourceProducerData>()
                          .Where(row => structureIds.Contains(row.structure_id))
                          .ToList().ToDictionary(rp => rp.structure_id);

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
            structure.GetComponent<Structure>().LoadData(s_data);

            // Loading ResourceProducerData
            if (structure.TryGetComponent(out ResourceProducer rp))
            {
                rp.LoadData(resourceProducers[s_data.structure_id]);
            }
        }
    }

    private List<StructureData> DatabaseGetStructureData()
    {
        return db.Table<StructureData>().Where((row) => row.player_id == playerId).ToList();
    }

    public void AddNewStructure(Structure structure, Structure_SO so, DateTime timeBuildFinished, Structure.BuildingState buildingState)
    {
        Vector3 structurePos = structure.gameObject.transform.position;
        Quaternion rotation = structure.gameObject.transform.rotation;

        var structureData = new StructureData {
            prefab_name = so.structurePrefab.name,
            player_id = playerId,
            time_build_finished = timeBuildFinished,
            building_state = (int)buildingState,
            posX = structurePos.x,
            posY = structurePos.y,
            posZ = structurePos.z,
            rotW = rotation.w,
            rotX = rotation.x,
            rotY = rotation.y,
            rotZ = rotation.z,
        };

        var result = db.Insert(structureData);
        //Test errors here.
        print("result: " + result + ", struc_id: " + structureData.structure_id + ", player_id: " + structureData.player_id);

        // Resource Producer
        if (structure.TryGetComponent(out ResourceProducer rp))
        {
            var rpData = new ResourceProducerData
            {
                structure_id = structureData.structure_id,
                production_finish_time = rp.productionFinishTime,
                producer_state = (int)rp.producerState,
            };

            result = db.Insert(rpData);
        }
        else if (structure.TryGetComponent(out Plot plot))
        {
            // Plot
            var plotData = new PlotData
            {
                structure_id = structureData.structure_id,
                growth_finish_time = plot.growthFinishTime,
                plot_state = (int)plot.plotState,
                crop_so_name = plot.Crop_SO.name,
            };

            result = db.Insert(plotData);
        }
    }
}
