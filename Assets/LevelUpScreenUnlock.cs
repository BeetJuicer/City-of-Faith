using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpScreenUnlock : MonoBehaviour
{
    [SerializeField] private TMP_Text LevelSNumber;
    [SerializeField] private Unlockables_SO unlockablesSO;
    [SerializeField] private GameObject newTemplate;
    [SerializeField] private GameObject newBContent;
    [SerializeField] private GameObject newCContent;

    private CentralHall centralHall;

    private void OnEnable()
    {
        Debug.Log("LevelUpScreenUnlock enabled.");

        // Assign centralHall by finding the CentralHall component in the scene
        centralHall = FindObjectOfType<CentralHall>();
        if (centralHall == null)
        {
            Debug.LogError("CentralHall not found in the scene.");
            return;
        }

        Debug.Log("CentralHall found: " + centralHall.name);
        LevelScreenFetchNumber(centralHall.Level);
        PopulateUnlockables(centralHall.Level);
    }

    private void LevelScreenFetchNumber(int level)
    {
        Debug.Log("LevelScreenFetchNumber method called with level: " + level);

        if (LevelSNumber != null)
        {
            LevelSNumber.text = level.ToString();
            Debug.Log("Level text updated to: " + level);
        }
        else
        {
            Debug.LogWarning("LevelSNumber is null.");
        }
    }

    private void PopulateUnlockables(int level)
    {
        Debug.Log("PopulateUnlockables method called for level: " + level);

        // Clear existing children in content containers
        ClearContent(newBContent);
        ClearContent(newCContent);

        // Populate Structures
        if (unlockablesSO.unclockableStructures.TryGetValue(level, out var structures))
        {
            Debug.Log("Found " + structures.Count + " structures to unlock for level " + level);
            foreach (var structure in structures)
            {
                Debug.Log("Instantiating structure: " + structure.name);
                var structureCard = Instantiate(newTemplate, newBContent.transform);
                structureCard.GetComponent<NewContentTemplateScript>().Init(structure, this);
                structureCard.SetActive(true);
            }
        }
        else
        {
            Debug.Log("No unlockable structures for level " + level);
        }

        // Populate Crops
        if (unlockablesSO.unclockableCrops.TryGetValue(level, out var crops))
        {
            Debug.Log("Found " + crops.Count + " crops to unlock for level " + level);
            foreach (var crop in crops)
            {
                Debug.Log("Instantiating crop: " + crop.name);
                var cropCard = Instantiate(newTemplate, newCContent.transform);
                cropCard.GetComponent<NewContentTemplateScript>().Init(crop, this);
                cropCard.SetActive(true);
            }
        }
        else
        {
            Debug.Log("No unlockable crops for level " + level);
        }
    }

    private void ClearContent(GameObject content)
    {
        Debug.Log("Clearing content for: " + content.name);
        foreach (Transform child in content.transform)
        {
            Debug.Log("Destroying child: " + child.name);
            Destroy(child.gameObject);
        }
    }
}
