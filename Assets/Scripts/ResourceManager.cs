using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Linq;

public enum Resource
{
    Gold, 
    Glory,
    Wood,
    Stone,
    Food
}

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager Instance { get { return _instance; } }

    [SerializedDictionary] // serialize for now for debug purposes.
    private SerializedDictionary<Resource, int> playerResources;
    public int PlayerExp { get; private set; }

    private void Awake()
    {
        if(_instance != null)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool HasEnoughResources(Dictionary<Resource, int> cost)
    {
        foreach (var resourceCost in cost)
        {
            //check each resource cost against playerResources. return false if not enough.
            if(playerResources[resourceCost.Key] < resourceCost.Value)
            {
                return false;
            }
        }

        return true;
    }
}
