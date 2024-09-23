using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class BuildingOverlay : MonoBehaviour
{
    public bool IsAllowedToPlace { get; private set; }
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private GameObject structurePrefab;
    [SerializeField] private Material buildMaterial;
    private int collidersInRange;

    //temporary value for testing with naughtyattributes
    private bool buildMode;

    private void Awake()
    {
        IsAllowedToPlace = true;
    }

    private void Update()
    {
        //update color
        gameObject.GetComponent<MeshRenderer>().material.color = (IsAllowedToPlace) ? Color.green : Color.red;
    }

    private void OnRenderObject()
    {
        if (buildMode)
            Graphics.DrawMesh(structurePrefab.GetComponent<MeshFilter>().sharedMesh, transform.position, transform.rotation, buildMaterial, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Counts the number of objects in range. Do not count the ground.
        if ((whatIsGround & (1 << other.gameObject.layer)) != 0)
            return;

        print("something here: " + other.gameObject);
        collidersInRange++;
        IsAllowedToPlace = false;
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInRange--;
        if(collidersInRange == 0)
        {
            IsAllowedToPlace = true;
        }
    }


    // Naughty Attributes methods.
    [Button]
    private void ToggleBuildMode()
    {
        buildMode = !buildMode;
    }

    [Button]
    private void InstantiateBuilding()
    {
        //also a bit temporary. Final will be to spawn a prefab that's in the growth phase. or in the building phase.
        //enough time passes by, it'll replace itself with a finished building prefab.

        if(IsAllowedToPlace)
            Instantiate(structurePrefab, transform.position, transform.rotation);
        //else
            //feedback that it's not allowed.

    }

    [Button]
    private void RotateClockwise()
    {
        gameObject.transform.Rotate(new Vector3(0,45,0));
    }
}
