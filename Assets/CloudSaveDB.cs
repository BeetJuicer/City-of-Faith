using UnityEngine;
using Unity.Services.Core;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.SceneManagement;
using DG.Tweening.Plugins.Core.PathCore;
using SQLite;

public class CloudSaveDB : MonoBehaviour
{
    private readonly float secondsPerAutosave = 10f;
    UsernamePasswordAuth UPA;

    private async void Awake()
    {
        await UnityServices.InitializeAsync();
    }

    private void Start()
    {
        UPA = FindAnyObjectByType<UsernamePasswordAuth>();
        if (UPA == null)
            Debug.LogWarning("UPA not found!");

        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        //We do not want to call autosave if we're not in the game scene. This causes file access issues to the save file because Auth also has the file open.
        if (scene.buildIndex <= 1)
            return;

        print("Not in main menu. Starting Autosave");
        //autosave every 5 minutes
        InvokeRepeating(nameof(SavePlayerFile), secondsPerAutosave, secondsPerAutosave);
    }

    private void OnApplicationPause(bool pause)
    {
        //Temporary Solution. We don't want to save anything if we're in the main menu screen.
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.buildIndex <= 1)
            return;

        SavePlayerFile();
    }

    private void OnApplicationQuit()
    {
        //Temporary Solution. We don't want to save anything if we're in the main menu screen.
        Scene scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.buildIndex <= 1)
            return;

        SavePlayerFile();
    }

    private async Task SaveFileBytes(string key, byte[] bytes)
    {
        try
        {
            await CloudSaveService.Instance.Files.Player.SaveAsync(key, bytes);

            Debug.Log("File saved!");
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e.Reason );
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
        }
    }

    public async void SavePlayerFile()
    {
        print($"Saving path: { Application.persistentDataPath}/MyDb.db");
        byte[] file = System.IO.File.ReadAllBytes($"{Application.persistentDataPath}/MyDb.db");

        await SaveFileBytes("databaseSave", file);
    }

    public async void SaveData()
    {
        Database.ResourceProducerData rp = new();
        var rpData = new Dictionary<int, object> {
            { 1, rp },
            { 2, rp },
            { 3, rp },
            { 4, rp }
        };

        var structureData =  new Dictionary<int, object>();
        var plotData =  new Dictionary<int, object>();
        string centralData = "centralData here.";

        var playerData = new Dictionary<string, object>{
          {"username", "carlipooo"},
          {"resourceProducerData", rpData},
          {"structureData", structureData},
          {"plotData", plotData},
          {"centralData", centralData},
        };
        await CloudSaveService.Instance.Data.Player.SaveAsync(playerData);
        Debug.Log($"Saved data {string.Join(',', playerData)}");
    }

    public async void LoadData()
    {
        var playerData = await CloudSaveService.Instance.Data.Player.LoadAsync(new HashSet<string> {
          "username", "money"
        });

        if (playerData.TryGetValue("username", out var firstKey))
        {
            Debug.Log($"username value: {firstKey.Value.GetAs<string>()}");
        }

        if (playerData.TryGetValue("money", out var secondKey))
        {
            Debug.Log($"money value: {secondKey.Value.GetAs<int>()}");
        }
    }

    public async void LoadDatabase()
    {
        Database.Instance?.CreateDatabaseFileIfNotExists();

        var bytes = await LoadFileBytes("databaseSave");

        // No cloud save file found. Initialize new scene.
        if (bytes == null)
        {
            print("No cloud save found. Initializing new scene.");
            Database.Instance?.SetUser(UPA._USERNAME);
            UnityEngine.SceneManagement.SceneManager.LoadScene("CloudWorld");
            return;
        }

        File.WriteAllBytes($"{Application.persistentDataPath}/MyDb.db", bytes);
        Debug.Log("Successfully loaded cloud saved database!");
        Database.Instance?.SetUser(UPA._USERNAME);
        UnityEngine.SceneManagement.SceneManager.LoadScene("CloudWorld");
    }

    private async Task<byte[]> LoadFileBytes(string key)
    {
        try
        {
            var results = await CloudSaveService.Instance.Files.Player.LoadBytesAsync(key);

            Debug.Log("File loaded!");

            return results;
        }
        catch (CloudSaveValidationException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveRateLimitedException e)
        {
            Debug.LogError(e);
        }
        catch (CloudSaveException e)
        {
            Debug.LogError(e);
            Debug.Log("Player does not exist yet.");
            return null;
        }

        return null;
    }
}
