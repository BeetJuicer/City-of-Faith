using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.CloudSave.Models.Data.Player;

public class VisitCloudLoader : MonoBehaviour
{
    public static VisitCloudLoader Instance { get; private set; }
    Database.PublicVillageData villageData;
    public Database.PublicVillageData VillageData;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += LoadVillage;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= LoadVillage;
    }

    public void SetVillageToVisit(Database.PublicVillageData villageData)
    {
        this.villageData = villageData;
    }

    public void LoadVillage(Scene scene, LoadSceneMode mode)
    {
        print("Entered loadvill");
        if (scene.buildIndex != 3) //scene MUST be 3 in build index. Magic number. Refactor.
            return;

        if (villageData == null)
        {
            Debug.LogWarning("Village Data is null!");
            return;
        }

        print("village level: " + villageData.level);

        foreach (var structure in villageData.structures)
        {
            print("Instantiating: " + structure.name);
            string path = $"Structures/{structure.name}";
            Instantiate(Resources.Load(path), structure.position, Quaternion.identity);
        }
    }

}
