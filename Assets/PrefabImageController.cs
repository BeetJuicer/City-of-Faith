using UnityEngine;

public class PrefabImageController : MonoBehaviour
{
    [SerializeField] private GameObject imageObject;  // Assign this in prefab
    [SerializeField] private string buildingID;  // Unique identifier for this type (like "House", "Farm")

    private void Start()
    {
        if (IsFirstTimeBuildingPlaced())
        {
            EnableImage();
            MarkBuildingAsSeen();
        }
        else
        {
            imageObject.SetActive(false);  // If not first time, make sure image stays off
        }
    }

    private bool IsFirstTimeBuildingPlaced()
    {
        // Check PlayerPrefs if this building type has been placed before
        return PlayerPrefs.GetInt($"BuildingPlaced_{buildingID}", 0) == 0;
    }

    private void MarkBuildingAsSeen()
    {
        // Mark this building type as placed (so image won't show next time)
        PlayerPrefs.SetInt($"BuildingPlaced_{buildingID}", 1);
        PlayerPrefs.Save();
    }

    public void EnableImage()
    {
        if (imageObject != null)
        {
            imageObject.SetActive(true);
        }
    }
}
