using UnityEngine;

public class FixRotation : MonoBehaviour
{
    [SerializeField] private new Transform transform; // Optional: Specific transform to control
    private Quaternion fixedRotation;

    private void Awake()
    {

        // Store the initial rotation
        fixedRotation = transform.rotation;
    }

    private void LateUpdate()
    {
        // Reapply the stored rotation in LateUpdate
        transform.rotation = fixedRotation;
    }
}
