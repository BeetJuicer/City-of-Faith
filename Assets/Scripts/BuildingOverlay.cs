using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class BuildingOverlay : MonoBehaviour, IDraggable
{
    public bool IsAllowedToPlace { get; private set; }
    [SerializeField] private LayerMask whatIsGround;

    [SerializeField] private Structure_SO structure_SO;
    [SerializeField] private Material buildPreviewMaterial;
    private int collidersInRange;
    [SerializeField] private int incrementalMovementUnits = 1;

    private void Awake()
    {
        IsAllowedToPlace = true;
    }

    private void Start()
    {
        //get the collision x and z of the structure and apply it to the collider of this thing.
        Vector3 structureSize = structure_SO.structurePrefab.GetComponent<BoxCollider>().size;
        //Vector3 detectorSize = GetComponent<BoxCollider>().size;
        //GetComponent<BoxCollider>().size = new Vector3(structureSize.x, detectorSize.y, structureSize.z);
        //also adjust the scale x and z accordingly.
        transform.localScale = new Vector3(structureSize.x, transform.localScale.y, structureSize.z);
    }

    private void Update()
    {
        //update color,
        gameObject.GetComponent<MeshRenderer>().material.color = (IsAllowedToPlace) ? Color.green : Color.red;
    }

    private void OnRenderObject()
    {
        if (GameManager.Instance.CurrentGameState == GameState.Edit_Mode)
            Graphics.DrawMesh(structure_SO.structurePrefab.GetComponentInChildren<MeshFilter>().sharedMesh, transform.position, transform.rotation, buildPreviewMaterial, 0);
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
    public void ToggleBuildMode()
    {
        GameManager.Instance.ChangeGameState(GameState.Edit_Mode);
    }

    [Button]
    public void InstantiateBuilding()
    {
        //also a bit temporary. Final will be to spawn a prefab that's in the growth phase. or in the building phase.
        //enough time passes by, it'll replace itself with a finished building prefab.

        if (GameManager.Instance.CurrentGameState == GameState.Edit_Mode &&
            IsAllowedToPlace &&
            ResourceManager.Instance.HasEnoughResources(structure_SO.resourcesRequired))
        {
            Instantiate(structure_SO.structurePrefab, transform.position, transform.rotation);
            //add xp and subtract gold.
            print("Built " + structure_SO.structureName + ". Subtracted todo continue this"); //+ structure_SO.goldCost + " gold coins and added " + structure_SO.expValue + " exp.");
        }
        //else
            //feedback that it's not allowed.

    }

    [Button]
    public void RotateClockwise()
    {
        transform.Rotate(new Vector3(0,45,0));
    }

    [Button]
    public void MoveIncrementallyRight()
    {
        transform.position += transform.right * incrementalMovementUnits;
    }

    [Button]
    public void MoveIncrementallyLeft()
    {
        transform.position -= transform.right * incrementalMovementUnits;
    }

    [Button]
    public void MoveIncrementallyUp()
    {
        transform.position += transform.forward * incrementalMovementUnits;
    }

    [Button]
    public void MoveIncrementallyDown()
    {
        transform.position -= transform.forward * incrementalMovementUnits;
    }
}
