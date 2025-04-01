using UnityEngine;
using UnityEngine.UI;

public class ArrowPlot : MonoBehaviour
{
    [SerializeField] private GameObject imageObject;  // Assign this in prefab

    // Call this method to enable the image child
    public void EnableImage()
    {
        imageObject.SetActive(true);
        Debug.Log("Arrow Image enabled successfully");
    }


    public void DisableImageBuilt()
    {
        imageObject.SetActive(false);
        Debug.Log("Arrow Image disabled successfully");

    }
}
