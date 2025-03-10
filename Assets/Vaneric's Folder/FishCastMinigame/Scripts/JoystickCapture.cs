using UnityEngine;

public class JoystickCapture : MonoBehaviour
{
    public bl_Joystick joystick;  // Joystick UI element
    public Transform redOutline;  // Now using Transform (World Space)
    public Camera mainCamera;  // Assign Main Camera
    public float moveSpeed = 1f;

    private Vector3 minBounds;
    private Vector3 maxBounds;
    private Vector3 initialPosition;

    void Start()
    {
        if (redOutline == null || mainCamera == null || joystick == null)
        {
            Debug.LogError("JoystickCapture: Missing references!");
            return;
        }

        SetWorldBoundaries(); // Set movement limits
        SetInitialPosition(); // Start at the correct position
    }

    void Update()
    {
        MoveCaptureBox();
    }

    private void MoveCaptureBox()
    {
        Vector2 moveDirection = new Vector2(joystick.Horizontal, joystick.Vertical);

        if (moveDirection.magnitude > 0.1f)
        {
            Vector3 move = new Vector3(moveDirection.x, moveDirection.y, 0) * moveSpeed * Time.deltaTime;
            redOutline.position += move;

            // Clamp inside the screen boundaries (not FishBoundary)
            redOutline.position = new Vector3(
                Mathf.Clamp(redOutline.position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(redOutline.position.y, minBounds.y, maxBounds.y),
                redOutline.position.z
            );
        }
    }

    private void SetWorldBoundaries()
    {
        Vector3 screenBottomLeft = mainCamera.ScreenToWorldPoint(new Vector3(0, 0, mainCamera.nearClipPlane));
        Vector3 screenTopRight = mainCamera.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, mainCamera.nearClipPlane));

        minBounds = new Vector3(screenBottomLeft.x + 0.5f, screenBottomLeft.y + 0.5f, 0);
        maxBounds = new Vector3(screenTopRight.x - 0.5f, screenTopRight.y - 0.5f, 0);
    }

    private void SetInitialPosition()
    {
        initialPosition = new Vector3(0, 0, 0); // Adjust as needed
        redOutline.position = initialPosition;
    }

    // **Restoring ShowCaptureBox & HideCaptureBox**
    public void ShowCaptureBox()
    {
        redOutline.position = initialPosition; // Reset position
        redOutline.gameObject.SetActive(true);
    }

    public void HideCaptureBox()
    {
        redOutline.gameObject.SetActive(false);
    }
}
