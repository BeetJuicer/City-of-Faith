using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.ProBuilder;
using System;

public class BuildingOverlay : MonoBehaviour, IDraggable
{
    public event Action<Vector3, Structure> OnStructureBuilt;

    [SerializeField] private Structure_SO debugStructureSO;
    private Structure_SO structure_SO;
    private float halfHeight = 0;

    public bool IsAllowedToPlace { get; private set; } = true;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Database db;

    [SerializeField] private Material buildPreviewMaterial;
    private int collidersInRange;
    [SerializeField] private int incrementalMovementUnits = 1;

    [SerializeField] private GameObject overlayPlane;
    private GameObject previewGO;
    public bool isInBuildMode;

    private CentralHall centralHall;

    private void Start()
    {
        centralHall = FindFirstObjectByType<CentralHall>();
    }

    public void EnterBuildMode(Structure_SO structure_SO)
    {
        if (isInBuildMode) return;

        isInBuildMode = true;

        this.structure_SO = structure_SO;

        //Precalculate the sizes we'll use for placement.
        //This REQUIRES that the object's pivot is at the center.
        Vector3 structureSize = structure_SO.structurePrefab.GetComponent<BoxCollider>().size;
        halfHeight = structureSize.y / 2;

        //Adjust the scale accordingly for this collider and visual plane.
        GetComponent<BoxCollider>().size = structureSize;
        overlayPlane.transform.localScale = new Vector3(structureSize.x, overlayPlane.transform.localScale.y, structureSize.z);

        overlayPlane.SetActive(true);
    }

    private void Update()
    {
        if (!isInBuildMode) return;
        //green red visual TODO: isOnground
        overlayPlane.GetComponent<MeshRenderer>().material.color = (collidersInRange == 0) ? Color.green : Color.red;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Counts the number of objects in range. Do not count the ground.
        if ((whatIsGround & (1 << other.gameObject.layer)) != 0)
            return;

        collidersInRange++;
    }

    private void OnTriggerExit(Collider other)
    {
        collidersInRange--;
        Debug.Assert(collidersInRange >= 0, "Negative count of colliders. Something is wrong.");
    }

    private void OnRenderObject()
    {
        if (!isInBuildMode) return;

        print($"{halfHeight} + {transform.position.y}");

        int count = structure_SO.structurePrefab.GetComponentInChildren<MeshFilter>().sharedMesh.subMeshCount;
        for (int i = 0; i < count; i++)
        {
            Graphics.DrawMesh(structure_SO.structurePrefab.GetComponentInChildren<MeshFilter>().sharedMesh, new Vector3(transform.position.x, transform.position.y + halfHeight, transform.position.z), transform.rotation, buildPreviewMaterial, 0, null, i);
        }
    }

    // Naughty Attributes methods.
    [Button]
    public void EnterBuildMode()
    {
        EnterBuildMode(debugStructureSO);
    }

    [Button]
    public void ExitBuildMode()
    {
        Debug.Log("ExitBuildMode natriggered.");
        Destroy(previewGO);
        overlayPlane.SetActive(false);
        isInBuildMode = false;
    }

    [Button]
    public void InstantiateBuilding()
    {
        if (!isInBuildMode)
        {
            Debug.LogWarning("Attempted to instantiate building while not in build mode.");
            return;
        }

        if(ResourceManager.Instance.HasEnoughResources(structure_SO.resourcesRequired))
        {
            Debug.LogError("Building overlay activated but player does not have enough money!");
        }

        if(collidersInRange > 0)
        {
            print("Not allowed! Deactivate the UI button for user's confirmation if not allowed");
        }

        Vector3 spawnPos;
        //TODO: currently it's only shooting a ray when we hit instantiate building. Have the ray part of the checking for IsAllowedToBuild
        Ray ray = new Ray(transform.position, Vector3.down);
        if(Physics.Raycast(ray, out RaycastHit hitInfo, 10f, whatIsGround))
        {
            float groundHeight = hitInfo.point.y;
            spawnPos = new Vector3(transform.position.x, groundHeight + halfHeight, transform.position.z);

            var newBuilding = Instantiate(structure_SO.structurePrefab, spawnPos, transform.rotation);

                    //Subtract currency
            foreach (KeyValuePair<Currency, int> currencyCost in structure_SO.currencyRequired)
            {
                ResourceManager.Instance.AdjustPlayerCurrency(currencyCost.Key, -currencyCost.Value);
            }

            //Add xp.
            centralHall.AddToCentralExp(structure_SO.expGivenOnBuild);

            Debug.Log("Instantiate building end reached.");
            OnStructureBuilt?.Invoke(spawnPos, newBuilding.GetComponent<Structure>());
            Debug.Log("OnStructureBuilt Invoked.");
        }

        ExitBuildMode();
    }

    [Button]
    public void RotateClockwise()
    {
        transform.Rotate(new Vector3(0, 90, 0));
    }

    [Button]
    public void MoveIncrementallyRight()
    {
        // Move the building to the world right
        transform.position += Vector3.right * incrementalMovementUnits;
    }

    [Button]
    public void MoveIncrementallyLeft()
    {
        // Move the building to the world left
        transform.position += Vector3.left * incrementalMovementUnits;
    }

    [Button]
    public void MoveIncrementallyUp()
    {
        // Move the building to the world forward (world up direction)
        transform.position += Vector3.forward * incrementalMovementUnits;
    }

    [Button]
    public void MoveIncrementallyDown()
    {
        // Move the building to the world backward (world down direction)
        transform.position += Vector3.back * incrementalMovementUnits;
    }
}
