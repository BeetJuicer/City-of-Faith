using UnityEngine;

public class CircularSliderPosition : MonoBehaviour
{
    public Transform redOutlineCaptureBox;  // Assign in Inspector
    public Vector2 offset = new Vector2(0.5f, 0.5f);  // Adjust for correct placement

    void Update()
    {
        if (redOutlineCaptureBox)
        {
            // Set CircularSlider position relative to RedOutlineCaptureBox
            Vector3 worldPosition = redOutlineCaptureBox.position + (Vector3)offset;
            transform.position = worldPosition;
        }
    }
}
