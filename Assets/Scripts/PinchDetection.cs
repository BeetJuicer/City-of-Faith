using System.Collections;
using UnityEngine;

public class PinchToZoomAndPan : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 0.1f;  // How fast the camera zooms in and out
    [SerializeField] private float panSpeed = 0.5f;   // How fast the camera moves (panning)
    [SerializeField] private float minZoom = 2f;      // Minimum zoom (orthographic size)
    [SerializeField] private float maxZoom = 10f;     // Maximum zoom (orthographic size)

    private Camera mainCamera; // Reference to the main camera
    private TouchControls controls; // Reference to the new input system

    private Coroutine zoomCoroutine;

    private Vector2 lastPrimaryFingerPosition;
    private Vector2 lastSecondaryFingerPosition;

    private bool isZooming = false;

    private void Awake()
    {
        controls = new TouchControls(); // Initialize the new input system controls
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

        // Store initial finger positions for panning and zooming
        lastPrimaryFingerPosition = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
        lastSecondaryFingerPosition = controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();

        while (true)
        {
            // Read current positions of both fingers using the New Input System
            Vector2 primaryFingerPosition = controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>();
            Vector2 secondaryFingerPosition = controls.Touch.SecondaryFingerPosition.ReadValue<Vector2>();

            // Check if the secondary finger is touching (i.e., zoom gesture is happening)
            if (controls.Touch.SecondaryTouchContact.ReadValue<float>() > 0)
            {
                // Handle zooming
                float currentDistance = Vector2.Distance(primaryFingerPosition, secondaryFingerPosition);
                Vector2 pinchCenter = (primaryFingerPosition + secondaryFingerPosition) / 2f;
                Vector3 pinchWorldPoint = mainCamera.ScreenToWorldPoint(new Vector3(pinchCenter.x, pinchCenter.y, mainCamera.nearClipPlane));

                if (previousDistance == 0f)
                {
                    previousDistance = currentDistance;
                    yield return null;
                }

                float distanceDelta = currentDistance - previousDistance;
                float newSize = mainCamera.orthographicSize - (distanceDelta * zoomSpeed);
                newSize = Mathf.Clamp(newSize, minZoom, maxZoom);

                float zoomFactor = mainCamera.orthographicSize / newSize;
                Vector3 directionToPinch = pinchWorldPoint - mainCamera.transform.position;
                mainCamera.transform.position += directionToPinch * (1 - 1 / zoomFactor);

                mainCamera.orthographicSize = newSize;
                previousDistance = currentDistance;

                isZooming = true;
            }
            else if (controls.Touch.PrimaryFingerPosition.ReadValue<Vector2>() != Vector2.zero && !isZooming)
            {
                // Handle panning when only one finger is active and zooming isn't happening
                Vector2 primaryFingerDelta = primaryFingerPosition - lastPrimaryFingerPosition;

                Vector3 worldDelta = mainCamera.ScreenToWorldPoint(new Vector3(primaryFingerDelta.x, primaryFingerDelta.y, 0))
                                     - mainCamera.ScreenToWorldPoint(Vector3.zero);

                mainCamera.transform.position -= worldDelta * panSpeed;
            }

            // Reset zoom flag when there are no two fingers
            if (controls.Touch.SecondaryTouchContact.ReadValue<float>() == 0)
            {
                isZooming = false;
            }

            // Store the current finger positions for the next iteration
            lastPrimaryFingerPosition = primaryFingerPosition;
            lastSecondaryFingerPosition = secondaryFingerPosition;

            yield return null; // Continue on the next frame
        }
    }
}
