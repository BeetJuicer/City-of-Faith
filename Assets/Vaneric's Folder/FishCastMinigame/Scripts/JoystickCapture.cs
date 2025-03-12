using UnityEngine;
using UnityEngine.EventSystems;

public class JoystickCapture : MonoBehaviour
{
    [Header("References")]
    public bl_Joystick joystick;  // Reference to the bl_Joystick script
    public Transform redOutline;  // The capture box that moves
    public Camera mainCamera;     // Main game camera

    [Header("Movement Settings")]
    public float moveSpeed = 60f; // Speed of the red outline movement

    private Vector3 minBounds;
    private Vector3 maxBounds;
    private Vector3 initialPosition;
    private bool isMoving = false;

    void Start()
    {
        if (redOutline == null || mainCamera == null || joystick == null)
        {
            Debug.LogError("JoystickCapture: Missing references!");
            return;
        }

        SetWorldBoundaries();
        SetInitialPosition();
    }

    void Update()
    {
        MoveCaptureBox();
    }

    /// <summary>
    /// Moves the capture box based on joystick input while staying within screen boundaries.
    /// </summary>
    private void MoveCaptureBox()
    {
        Vector2 moveDirection = new Vector2(joystick.Horizontal, joystick.Vertical);

        if (moveDirection.magnitude > 0.01f)
        {
            isMoving = true;
            Vector3 move = new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed * Time.unscaledDeltaTime;
            redOutline.position += move;

            redOutline.position = new Vector3(
                Mathf.Clamp(redOutline.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(redOutline.position.y, minBounds.y, maxBounds.y),
                redOutline.position.z
            );
        }
        else if (isMoving)
        {
            isMoving = false;
            Debug.Log("Joystick stopped moving");
        }
    }

    /// <summary>
    /// Sets movement boundaries based on screen size in world space.
    /// </summary>
    private void SetWorldBoundaries()
    {
        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

        float padding = 0.5f; // Optional padding to avoid edges

        minBounds = new Vector3(screenBottomLeft.x + padding, screenBottomLeft.y + padding, 0);
        maxBounds = new Vector3(screenTopRight.x - padding, screenTopRight.y - padding, 0);
    }

    /// <summary>
    /// Sets the initial position of the capture box.
    /// </summary>
    private void SetInitialPosition()
    {
        initialPosition = Vector3.zero; // Default position
        redOutline.position = initialPosition;
    }

    /// <summary>
    /// Shows and resets the capture box.
    /// </summary>
    public void ShowCaptureBox()
    {
        redOutline.position = initialPosition;
        redOutline.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hides the capture box.
    /// </summary>
    public void HideCaptureBox()
    {
        redOutline.gameObject.SetActive(false);
    }
}
