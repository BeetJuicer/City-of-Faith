using UnityEngine;
using Unity.Services.Core;
using System.Collections.Generic;
using Unity.Services.CloudSave;
using System.Threading.Tasks;
using System.IO;
using UnityEngine.SceneManagement;

public class CloudSaveDB : MonoBehaviour
{
    private readonly float secondsPerAutosave = 10f;
    private async void Awake()
    {
        await UnityServices.InitializeAsync();
    }

    private void Start()
    {
        //autosave every 5 minutes
        InvokeRepeating(nameof(SavePlayerFile), secondsPerAutosave, secondsPerAutosave);
    }

    private void OnApplicationPause(bool pause)
    {
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
        var bytes = await LoadFileBytes("databaseSave");
        File.WriteAllBytes($"{Application.persistentDataPath}/MyDb.db", bytes);
        Debug.Log("Successfully loaded cloud saved database!");
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
        }

        return null;
    }
}
