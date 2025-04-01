using UnityEngine;

public class PrefabImageController : MonoBehaviour
{
    [SerializeField] private GameObject imageObject;  // Assign this in prefab
    private bool imageEnabled =false;

    // Call this method to enable the image child
    public void EnableImage()
    {
        if (imageEnabled)
        {
            return;
        }
        imageObject.SetActive(true);
        imageEnabled = true;
    }

    public void DisableImage()
    {
        if (imageObject != null)
        {
            imageObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Image child not assigned in the inspector!");
        }
    }
}
