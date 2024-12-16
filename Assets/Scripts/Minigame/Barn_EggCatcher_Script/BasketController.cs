using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketController : MonoBehaviour
{
    public Camera cam;                 // Reference to the camera used to calculate screen bounds
    private float maxWidth;            // The maximum horizontal position the basket can move
    private Rigidbody2D rb;            // Rigidbody2D component for smooth movement
    private float yOffset = -3.5f;     // Vertical offset to keep the basket at a fixed height

    void Start()
    {
        // Assign the main camera if none is set in the Inspector
        if (cam == null)
        {
            cam = Camera.main;
        }

        // Calculate the maximum width of the screen in world coordinates
        Vector3 upperCorner = new Vector3(Screen.width, Screen.height, 0.0f); // Top-right corner of the screen
        Vector3 targetWidth = cam.ScreenToWorldPoint(upperCorner);           // Convert screen position to world position
        maxWidth = targetWidth.x;                                           // Set the maximum horizontal width

        // Get the Rigidbody2D component attached to this GameObject
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Get the mouse position in world coordinates
        Vector3 rawPosition = cam.ScreenToWorldPoint(Input.mousePosition);

        // Set the basket's target position (horizontal movement only, fixed vertical position)
        Vector3 targetPosition = new Vector3(rawPosition.x, yOffset, 0.0f);

        // Clamp the horizontal position to prevent the basket from going off-screen
        float targetWidth = Mathf.Clamp(targetPosition.x, -maxWidth, maxWidth);
        targetPosition = new Vector3(targetWidth, yOffset, targetPosition.z);

        // Smoothly move the basket to the target position using Rigidbody2D
        rb.MovePosition(targetPosition);
    }
}
