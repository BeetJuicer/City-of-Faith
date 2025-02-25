using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BlockOtherUIClicks : MonoBehaviour
{
    [SerializeField] private GameObject buildingMoveUI;  // Assign your World Space UI
    private bool isBlocking = false;

    private void Update()
    {
        if (isBlocking && Input.GetMouseButtonDown(0)) // Left Click
        {
            if (!IsPointerOverMoveUI())
            {
                // Prevent interaction with other UI by consuming the event
                Debug.Log("Click blocked outside BuildingMoveUI");
            }
        }
    }

    public void EnableBlocker()
    {
        isBlocking = true;
    }

    public void DisableBlocker()
    {
        isBlocking = false;
    }

    private bool IsPointerOverMoveUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == buildingMoveUI || result.gameObject.transform.IsChildOf(buildingMoveUI.transform))
            {
                return true; // Click is on the move UI
            }
        }

        return false; // Click is outside
    }
}
