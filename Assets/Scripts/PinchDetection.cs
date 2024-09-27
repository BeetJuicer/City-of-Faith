using System.Collections;
using UnityEngine;

public class PinchToZoomAndPan : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 0.1f;  // How fast the camera zooms in and out
    [SerializeField] private float panSpeed = 0.5f;   // How fast the camera moves (panning)
    [SerializeField] private float minZoom = 2f;      // Minimum zoom (orthographic size)
    [SerializeField] private float maxZoom = 10f;     // Maximum zoom (orthographic size)

    private Camera mainCamera; // Reference to the main camera
    private float previousDistance; // Stores the previous frame's pinch distance
    private TouchControls controls; // Reference to the new input system

    private Coroutine zoomCoroutine;

    private void Awake()
    {
        controls = new TouchControls();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        controls.Enable();
        controls.Touch.SecondaryTouchContact.started += _ => ZoomStart();
        controls.Touch.SecondaryTouchContact.canceled += _ => ZoomEnd();
    }

    private void OnDisable()
    {
        controls.Disable();
        controls.Touch.SecondaryTouchContact.started -= _ => ZoomStart();
        controls.Touch.SecondaryTouchContact.canceled -= _ => ZoomEnd();
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

    IEnumerator ZoomDetection()
    {
        float previousDistance = 0f;

        // Store initial finger positions for panning
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
            float newSize = mainCamera.orthographicSize - (distanceDelta * zoomSpeed);

            // Clamp the camera zoom to stay within min and max bounds
            newSize = Mathf.Clamp(newSize, minZoom, maxZoom);

            // Calculate the zoom factor (how much we're zooming in or out)
            float zoomFactor = mainCamera.orthographicSize / newSize;

            // Move the camera to zoom in toward the pinch center
            Vector3 directionToPinch = pinchWorldPoint - mainCamera.transform.position;
            mainCamera.transform.position += directionToPinch * (1 - 1 / zoomFactor);

            // Apply the new orthographic size
            mainCamera.orthographicSize = newSize;

            // Panning (move the camera based on finger drag)
            Vector2 primaryFingerDelta = primaryFingerPosition - lastPrimaryFingerPosition;
            Vector2 secondaryFingerDelta = secondaryFingerPosition - lastSecondaryFingerPosition;

            // Calculate the average movement of both fingers (delta)
            Vector2 averageDelta = (primaryFingerDelta + secondaryFingerDelta) / 2f;

            // Convert screen delta to world delta
            Vector3 worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(averageDelta.x, averageDelta.y, 0))
                                 - mainCamera.ScreenToWorldPoint(Vector3.zero);

            // Apply the movement to the camera position (panning)
            mainCamera.transform.position -= worldDelta * panSpeed;

            // Update previousDistance for the next loop iteration
            previousDistance = currentDistance;

            // Store current finger positions for next panning calculation
            lastPrimaryFingerPosition = primaryFingerPosition;
            lastSecondaryFingerPosition = secondaryFingerPosition;

            yield return null; // Continue on the next frame
        }
    }
}
