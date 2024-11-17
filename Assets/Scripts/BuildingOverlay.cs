using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;


public class BuildingOverlay : MonoBehaviour, IDraggable
{
    public bool IsAllowedToPlace { get; private set; }
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Database db;

    [SerializeField] private Structure_SO structure_SO;
    [SerializeField] private Material buildPreviewMaterial;
    private int collidersInRange;
    [SerializeField] private int incrementalMovementUnits = 1;

    //debug
    private Vector3 hitPos = Vector3.down;

    public bool useRay;

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
        Ray ray = new Ray(transform.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 10f, whatIsGround))
        {
            hitPos = hitInfo.point;
            print(hitPos);
            Vector3 objSpaceHitPos = transform.InverseTransformPoint(hitInfo.point);
            float groundHeight = objSpaceHitPos.y;
            float halfHeight = (structure_SO.structurePrefab.GetComponent<BoxCollider>().size.y / 2);
            Vector3 spawnPos = new Vector3(transform.position.x, groundHeight, transform.position.z);

            GameObject structure = Instantiate(structure_SO.structurePrefab, spawnPos, transform.rotation);

            //add xp and subtract gold.
            foreach (KeyValuePair<Currency, int> currencyCost in structure_SO.currencyRequired)
            {
                ResourceManager.Instance.AdjustPlayerCurrency(currencyCost.Key, -currencyCost.Value);
            }
        }
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

        //print("something here: " + other.gameObject);
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
        if (GameManager.Instance.CurrentGameState == GameState.Edit_Mode && 
            ResourceManager.Instance.HasEnoughResources(structure_SO.resourcesRequired) &&
            IsAllowedToPlace)
        {
            //TODO: currently it's only shooting a ray when we hit instantiate building. Have the ray part of the checking for IsAllowedToBuild
            if(useRay)
            {
                Ray ray = new Ray(transform.position, Vector3.down);
                if(Physics.Raycast(ray, out RaycastHit hitInfo, 10f, whatIsGround))
                {
                    Vector3 objSpaceHitPos = transform.InverseTransformPoint(hitInfo.point);
                    float groundHeight = objSpaceHitPos.y;
                    float halfHeight = (structure_SO.structurePrefab.GetComponent<BoxCollider>().size.y / 2);
                    Vector3 spawnPos = new Vector3(transform.position.x, groundHeight, transform.position.z);

                    GameObject structure = Instantiate(structure_SO.structurePrefab, hitInfo.point, transform.rotation);

                    //add xp and subtract gold.
                    foreach (KeyValuePair<Currency, int> currencyCost in structure_SO.currencyRequired)
                    {
                        ResourceManager.Instance.AdjustPlayerCurrency(currencyCost.Key, -currencyCost.Value);
                    }
                }
            }
            else
            {
                float height = structure_SO.structurePrefab.GetComponent<BoxCollider>().size.y;
                GameObject structure = Instantiate(structure_SO.structurePrefab, new Vector3(transform.position.x, height, transform.position.z), transform.rotation);

                //add xp and subtract gold.
                foreach (KeyValuePair<Currency, int> currencyCost in structure_SO.currencyRequired)
                {
                    ResourceManager.Instance.AdjustPlayerCurrency(currencyCost.Key, -currencyCost.Value);
                }
            }
            //print("TODO: Built " + structure_SO.structureName + ". NEED TO ADD " + structure_SO.expGivenOnBuild);
        }
        //else
            //feedback that it's not allowed.
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, hitPos);
    }

    [Button]
    public void RotateClockwise()
    {
        transform.Rotate(new Vector3(0, 45, 0));
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
