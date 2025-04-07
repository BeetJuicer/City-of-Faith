using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.CloudSave.Models.Data.Player;
using Unity.Services.Friends;

public class VisitCloudLoader : MonoBehaviour
{
    public static VisitCloudLoader Instance { get; private set; }
    public Database.PublicVillageData VillageData { get; private set; }
    public string friendUsername { get; private set; }

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

    //private void Start()
    //{
    //    LoadVillageDebug();
    //}

    public void SetVillageToVisit(Database.PublicVillageData villageData, string username)
    {
        this.friendUsername = username;
        this.VillageData = villageData;
    }

    public void LoadVillage(Scene scene, LoadSceneMode mode)
    {
        print("Entered loadvill");
        if (scene.buildIndex != 3) //scene MUST be 3 in build index. Magic number. Refactor.
            return;

        if (VillageData == null)
        {
            Debug.LogWarning("Village Data is null!");
            return;
        }

        print("village level: " + VillageData.level);

        foreach (var structure in VillageData.structures)
        {
            print("Instantiating: " + structure.name);
            string path = $"Structures/{structure.name}";
            Instantiate(Resources.Load(path), structure.position, Quaternion.identity);
        }
    }

}
