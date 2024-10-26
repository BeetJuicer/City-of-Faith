using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;
using System;

public class Database : MonoBehaviour
{
    private readonly int USERNAME_CHARACTER_LIMIT = 50;
    private readonly int MINIMUM_PASSWORD_LENGTH = 8;

    private string username = "Carl";
    private string password = "CarlPass";

    SQLiteConnection db;

    public class PlayerData
    {
        [PrimaryKey, AutoIncrement]
        public int Player_id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
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

        // Player Exists. Load data.
        Debug.Log("Player exists.");
        LoadGame();
    }


    // When player reenters game.
    private void OnApplicationFocus(bool focus)
    {
        if (focus)
            LoadGame();
        else
            SaveGame();
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
    }

    public void AddNewStructure(Structure structure, Structure_SO so, DateTime timeBuildFinished, Structure.BuildingState buildingState)
    {
        Vector3 structurePos = structure.gameObject.transform.position;
        string query = $"INSERT INTO tbl_structure('player_id', 'prefab_name', 'time_build_finished', 'buildState', 'posX', 'posY', 'posZ') " +
                                          $"VALUES(?, ?, ?, ?, ?, ?, ?); ";
        var result = db.Execute(query, 1, so.structurePrefab.name, timeBuildFinished.ToString(), buildingState.ToString(), structurePos.x, structurePos.y, structurePos.z);

        //Test errors here.
    }
}
