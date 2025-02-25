using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Cinemachine;
using System.Collections.Generic;
using System.Linq;

public class PinchToZoomAndPan : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private float zoomSpeed = 0.1f;  // How fast the camera zooms in and out
    [SerializeField] private float panSpeed = 0.5f;   // How fast the camera moves (panning)
    [SerializeField] private float minZoom = 2f;      // Minimum zoom (orthographic size)
    [SerializeField] private float maxZoom = 10f;     // Maximum zoom (orthographic size)
    [SerializeField] private UIManager uiManager; // Reference to the UIManager

    private Camera mainCamera; // Reference to the main camera
    private float previousDistance; // Stores the previous frame's pinch distance
    private TouchControls controls; // Reference to the new input system
    private Coroutine zoomCoroutine;
    private Coroutine dragCoroutine;
    private GameObject draggableObject;
    private IClickableObject lastClickedObject;
    private bool isDraggingBuilding = false; // Flag to check if a building is being dragged

    // Store last known primary finger position for panning
    private Vector2 lastPrimaryFingerPosition;

    private void Awake()
    {
        controls = new TouchControls();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        controls.Enable();
        // Zoom detection
        controls.Touch.SecondaryTouchContact.started += _ => ZoomStart();
        controls.Touch.SecondaryTouchContact.canceled += _ => ZoomEnd();

        // Press or Click detection
        controls.Touch.PrimaryTouchPress.performed += ctx => OnPrimaryTouchPress();

        // Hold detection
        controls.Touch.PrimaryTouchHoldRelease.performed += ctx => OnPrimaryTouchHold();
        controls.Touch.PrimaryTouchHoldRelease.canceled += ctx => OnPrimaryTouchHoldRelease();
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Touch.PrimaryTouchPress.performed -= ctx => OnPrimaryTouchPress();
        controls.Touch.PrimaryTouchHoldRelease.performed -= ctx => OnPrimaryTouchHold();
        controls.Touch.PrimaryTouchHoldRelease.canceled -= ctx => OnPrimaryTouchHoldRelease();
        controls.Touch.SecondaryTouchContact.started -= _ => ZoomStart();
        controls.Touch.SecondaryTouchContact.canceled -= _ => ZoomEnd();
    }


    private void OnPrimaryTouchPress()
    {
        //print("On Primary Touch called.");
        // Get the touch position from the input system
        Vector2 touchPosition = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
        int groundLayerMask = LayerMask.GetMask("Ground");
        //print("Touch position is: " + touchPosition);

        if (IsTouchOverUI(touchPosition))
        {
            print("Touched UI. Not clicking object.");
            return;
        }

        // Convert the touch position to a ray
        Ray ray = mainCamera.ScreenPointToRay(touchPosition);

        // Perform a RaycastAll to get all objects hit by the ray
        RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);

        RaycastHit? groundHit = null;
        RaycastHit? objectHit = null;

        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                groundHit = hit; // Store the first ground hit
            }
            else
            {
                objectHit = hit; // Store the first object hit (structure, draggable, etc.)
            }
        }

        // Prioritize object hit over ground hit
        if (objectHit.HasValue)
        {
            print("Object hitted.");
            IClickableObject[] clickables = objectHit.Value.collider.GetComponents<IClickableObject>();
            foreach (IClickableObject clickable in clickables)
            {

                if (lastClickedObject != null && lastClickedObject != clickable)
                {
                    if (lastClickedObject is IClickableObject clickableComponent)
                    {
                        clickableComponent.ResetPopState();
                    }
                }

                Debug.Log($"Clickable object pressed: {objectHit.Value.collider.gameObject.name} at {objectHit.Value.point}");
                clickable.OnObjectClicked();
                lastClickedObject = clickable;
            }

            if (objectHit.Value.collider.TryGetComponent(out IDraggable draggableObject))
            {
                print("Object is draggable");
                draggableObject = objectHit.Value.collider.gameObject.GetComponent<IDraggable>();

            }
        }
        else if (groundHit.HasValue) // Only process ground if no object was hit
        {
            print("Ground hitted.");
            uiManager.DisableOnStructureClickButtons();
            if (lastClickedObject is IClickableObject clickableComponent)
            {
                clickableComponent.ResetPopState();
                lastClickedObject = null;
            }
        }
    }

    private void OnPrimaryTouchHold()
    {
        if (draggableObject != null && !isDraggingBuilding)
        {
            isDraggingBuilding = true;
            // Start the dragging coroutine if it's not already running
            if (dragCoroutine == null)
            {
                dragCoroutine = StartCoroutine(DragObject());
            }

            //Debug.Log("Primary touch hold detected, object is being dragged.");
        }
    }

    private void OnPrimaryTouchHoldRelease()
    {
        // Stop the dragging coroutine when the touch is released
        if (dragCoroutine != null)
        {
            StopCoroutine(dragCoroutine);
            dragCoroutine = null;
        }

        // Reset draggable object
        draggableObject = null;
        isDraggingBuilding = false;

        //Debug.Log("Primary touch hold released, object settled.");
    }

    // Coroutine to update the object's position while being held
    IEnumerator DragObject()
    {
        while (isDraggingBuilding && draggableObject != null)
        {
            // Get the current touch position
            Vector2 touchPosition = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
            Ray ray = mainCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Move the selected object to follow the touch position on the XZ plane
                Vector3 newPosition = new Vector3(hit.point.x, draggableObject.transform.position.y, hit.point.z);
                draggableObject.transform.position = newPosition; // Update object's position
            }

            // Wait for the next frame to continue the coroutine
            yield return null;
        }
    }


    private void ZoomStart()
    {
        if (zoomCoroutine == null)
        {
            zoomCoroutine = StartCoroutine(ZoomDetection());
        }
    }

    private void ZoomEnd()
    {
        if (zoomCoroutine != null)
        {
            StopCoroutine(zoomCoroutine);
            zoomCoroutine = null;
        }
    }

    private void Update()
    {
        //if(gamemanager is in play mode)
        HandleSingleFingerPan();
        //else buildmode
        //handlemovingbuildingoverlay() or let building overlay handle following finger.
    }

    private bool IsTouchOverUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = new Vector2(touchPosition.x, touchPosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
          //print("touched UI: " + result.gameObject.name);
        }

        return results.Count > 0;
    }

    // Handle single-finger panning (dragging)
    private void HandleSingleFingerPan()
    {
        //TODO: change to checking if currently in build mode.
        // Check if we are currently dragging a building
        if (isDraggingBuilding)
        {
            return; // Skip the panning logic if dragging a building
        }

        if (controls.Touch.PrimaryTouchContact.ReadValue<float>() > 0 && controls.Touch.SecondaryTouchContact.ReadValue<float>() == 0)
        {
            // Get the current position of the primary finger
            Vector2 primaryFingerPosition = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();

            // On first touch, initialize the last primary finger position
            if (lastPrimaryFingerPosition == Vector2.zero)
            {
                lastPrimaryFingerPosition = primaryFingerPosition;
                return;  // Skip this frame
            }

            if (IsTouchOverUI(primaryFingerPosition))
            {
                return;
            }

            // Calculate the delta (change) in position of the primary finger
            Vector2 delta = primaryFingerPosition - lastPrimaryFingerPosition;

            // Convert screen delta to world delta
            Vector3 worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(delta.x, delta.y, 0))
                                    - mainCamera.ScreenToWorldPoint(Vector3.zero);

            // Apply the movement to the camera position (panning)
            virtualCamera.transform.position -= worldDelta * panSpeed;

            // Update the last known primary finger position
            lastPrimaryFingerPosition = primaryFingerPosition;
        }
        else
        {
            // Reset the last known finger position when no touch is detected
            lastPrimaryFingerPosition = Vector2.zero;
        }
    }

    IEnumerator ZoomDetection()
    {
        float previousDistance = 0f;

        // Store initial finger positions for panning and zooming
        Vector2 lastPrimaryFingerPosition = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
        Vector2 lastSecondaryFingerPosition = controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();

        while (true)
        {
            // Read positions of both fingers using the new input system
            Vector2 primaryFingerPosition = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
            Vector2 secondaryFingerPosition = controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();

            // Calculate the current distance between the two fingers (for zooming)
            float currentDistance = Vector2.Distance(primaryFingerPosition, secondaryFingerPosition);

            // Calculate the midpoint between the two fingers (pinch center)
            Vector2 pinchCenter = (primaryFingerPosition + secondaryFingerPosition) / 2f;

            // Convert pinch center to world point (for zooming towards this point)
            Vector3 pinchWorldPoint = mainCamera.ScreenToWorldPoint(new Vector3(pinchCenter.x, pinchCenter.y, mainCamera.nearClipPlane));

            // On first touch or pinch start, initialize previousDistance
            if (previousDistance == 0f)
            {
                previousDistance = currentDistance;
                yield return null; // Skip the rest of this loop iteration
            }

            // Calculate the change in distance between current and previous frame (for zooming)
            float distanceDelta = currentDistance - previousDistance;

            // Adjust the camera's orthographic size based on the pinch movement
            float newSize = virtualCamera.m_Lens.OrthographicSize - (distanceDelta * zoomSpeed);

            // Clamp the camera zoom to stay within min and max bounds
            newSize = Mathf.Clamp(newSize, minZoom, maxZoom);

            // Calculate the zoom factor (how much we're zooming in or out)
            float zoomFactor = virtualCamera.m_Lens.OrthographicSize / newSize;

            // Move the camera to zoom in toward the pinch center
            Vector3 directionToPinch = pinchWorldPoint - mainCamera.transform.position;
            mainCamera.transform.position += directionToPinch * (1 - 1 / zoomFactor);

            // Interpolate near clip plane based on orthographic size
            //float nearClipPlane = Mathf.Lerp(-100, -10000, (newSize - minZoom) / (maxZoom - minZoom));

            // Apply the new orthographic size
            virtualCamera.m_Lens.OrthographicSize = newSize;
            //virtualCamera.m_Lens.NearClipPlane = nearClipPlane;
            //Debug.Log($"Updated Near Clip Plane: {nearClipPlane}");

            // Panning (move the camera based on finger drag)
            Vector2 primaryFingerDelta = primaryFingerPosition - lastPrimaryFingerPosition;
            Vector2 secondaryFingerDelta = secondaryFingerPosition - lastSecondaryFingerPosition;

            // Calculate the average movement of both fingers (delta)
            Vector2 averageDelta = (primaryFingerDelta + secondaryFingerDelta) / 2f;

            // Convert screen delta to world delta
            Vector3 worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(averageDelta.x, averageDelta.y, 0))
                                 - mainCamera.ScreenToWorldPoint(Vector3.zero);

            // Apply the movement to the camera position (panning while zooming)
            virtualCamera.transform.position -= worldDelta * panSpeed;

            // Update previousDistance for the next loop iteration
            previousDistance = currentDistance;

            // Store current finger positions for next panning calculation
            lastPrimaryFingerPosition = primaryFingerPosition;
            lastSecondaryFingerPosition = secondaryFingerPosition;

            yield return null;
        }
    }
}
