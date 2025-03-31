using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCHumanManager : MonoBehaviour
{
    public GameObject NPCHuman; // Assign the child object in the Inspector

    private Structure structure;

    void Start()
    {
        structure = GetComponent<Structure>(); // Get Structure component from the same GameObject (Parent)

    }

    void Update()
    {
        CheckState();
    }

    void CheckState()
    {
        if (structure != null)
        {
            NPCHuman.SetActive(structure.CurrentBuildingState == Structure.BuildingState.BUILT);
        }
    }

}
