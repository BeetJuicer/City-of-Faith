using UnityEngine;

public class PrefabImageController : MonoBehaviour
{
    [SerializeField] private GameObject imageObject;  // Assign this in prefab

    // Call this method to enable the image child
    public void EnableArrowBuilding()
    {
        imageObject.SetActive(true);
    }

    public void DisableArrowBuilding()
    {

        imageObject.SetActive(false);
    }
}
