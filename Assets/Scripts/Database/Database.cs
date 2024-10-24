using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SQLite;

public class Database : MonoBehaviour
{
    private readonly int USERNAME_CHARACTER_LIMIT = 50;
    private readonly int MINIMUM_PASSWORD_LENGTH = 8;

    private string username = "Carl";
    private string password = "CarlPass";

    public class PlayerData
    {
        [PrimaryKey, AutoIncrement]
        public int Player_id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    private void Start()
    {
        var db = new SQLiteConnection($"{Application.persistentDataPath}/MyDb.db");
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
    }

    private void OnApplicationFocus(bool focus)
    {
        //all Idatabasepersistence objects.save
    }

    private void OnApplicationQuit()
    {
        
    }
}
