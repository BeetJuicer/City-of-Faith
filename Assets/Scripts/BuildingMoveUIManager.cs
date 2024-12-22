using UnityEngine;

public class BuildingMoveUIManager : MonoBehaviour
{
    public RectTransform buildingMoveUIRectTransform;
    [SerializeField] private Vector3 offset;  // Offset from the building's position
    public BuildingOverlay buildingOverlay;  // Reference to the BuildingOverlay script
    public GameObject buildingMoveUI;  // Reference to the BuildingMoveUI GameObject
    public Dialogue dialogue;
    void Start()
    {
        buildingMoveUI.SetActive(false);
    }

    public void OpenMoveUI()
    {
        buildingMoveUI.SetActive(true);
    }

    private void Update()
    {
        buildingMoveUIRectTransform.position = buildingOverlay.transform.position + offset;
    }

    public void MoveUp()
    {
        buildingOverlay.MoveIncrementallyUp();
        Update();
    }

    public void MoveDown()
    {
        buildingOverlay.MoveIncrementallyDown();
        Update();
    }

    public void MoveLeft()
    {
        buildingOverlay.MoveIncrementallyLeft();
        Update();
    }

    public void MoveRight()
    {
        buildingOverlay.MoveIncrementallyRight();
        Update();
    }

    public void ConfirmBuildingPlacement()
    {
        dialogue.placeBuilding();
        buildingOverlay.InstantiateBuilding();
        buildingMoveUI.SetActive(false);
    }

    public void CancelBuildingPlacement()
    {
        buildingOverlay.ExitBuildMode();
        buildingMoveUI.SetActive(false);

    }

    public void RotateBuilding()
    {
        buildingOverlay.RotateClockwise();
        buildingMoveUI.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
