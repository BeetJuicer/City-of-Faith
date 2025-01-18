using UnityEngine;
using UnityEngine.UI;

public class BuildingMoveUIManager : MonoBehaviour
{
    [SerializeField] private RectTransform buildingMoveUIRectTransform;
    [SerializeField] private Vector3 offset;  // Offset from the building's position
    [SerializeField] private Button checkButton;
    [SerializeField] private BuildingOverlay buildingOverlay;  // Reference to the BuildingOverlay script
    [SerializeField] private GameObject buildingMoveUI;  // Reference to the BuildingMoveUI GameObject
    [SerializeField] private GameObject HUDCanvas;

    public Color allowedColor = Color.white;
    public Color disallowedColor = Color.gray;

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
        //if (!buildingOverlay.IsAllowedToPlace)
        //{
        //    GetComponent<Button>().interactable = false;
        //}
        if (checkButton != null && buildingOverlay != null)
        {
            // Get the Image component of the button and update the color
            Image buttonImage = checkButton.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = buildingOverlay.IsAllowedToPlace ? allowedColor : disallowedColor;
            }
        }
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
