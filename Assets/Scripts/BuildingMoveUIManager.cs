using UnityEngine;
using UnityEngine.UI;

public class BuildingMoveUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform buildingMoveUIRectTransform;
    [SerializeField] private Vector3 offset;  // Offset from the building's position
    [SerializeField] private Button checkBtn;
    [SerializeField] private BuildingOverlay buildingOverlay;  // Reference to the BuildingOverlay script
    [SerializeField] private GameObject buildingMoveUI;  // Reference to the BuildingMoveUI GameObject
    [SerializeField] private GameObject HUDCanvas;

    public Color allowedColor = Color.green;
    public Color disallowedColor = Color.black;

    public void OpenMoveUI()
    {
        buildingMoveUI.SetActive(true);
    }

    private void Update()
    {
        buildingMoveUIRectTransform.position = buildingOverlay.transform.position + offset;
        UpdateButtonColor();
    }

    private void UpdateButtonColor()
    {
        if (!buildingOverlay.IsAllowedToPlace)
        {
            GetComponent<Button>().interactable = false;
        }
        //if (indicatorImage != null && buildingOverlay != null)
        //{
        //    checkBtn.gameObject.SetActive(false);
        //    indicatorImage.gameObject.SetActive(!buildingOverlay.IsAllowedToPlace);
        //}
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
        //dialogue.PlaceBuilding();
        HUDCanvas.SetActive(true);
        buildingMoveUI.SetActive(false);
        buildingOverlay.InstantiateBuilding();
    }

    public void CancelBuildingPlacement()
    {
        buildingOverlay.ExitBuildMode();
        buildingMoveUI.SetActive(false);
        HUDCanvas.SetActive(true);
    }

    public void RotateBuilding()
    {
        buildingMoveUIRectTransform.rotation = Quaternion.identity;
        buildingOverlay.RotateClockwise();
    }
}
