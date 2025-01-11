using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Ensure this namespace is included for TMP_Text

public class LevelExpBarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text LevelNumber; // TMP_Text for displaying the level number
    private CentralHall centralHall;

    private void Start()
    {
        // Assign centralHall by finding the CentralHall component in the scene
        centralHall = FindObjectOfType<CentralHall>();
        if (centralHall == null)
        {
            Debug.LogError("CentralHall not found in the scene.");
            return;
        }

        Debug.Log("CentralHall found.");
        // Subscribe to the OnPlayerLevelUp event
        centralHall.OnPlayerLevelUp += UpdateExpLvUI;
        UpdateExpLvUI(centralHall.Level); // Initialize UI with the current level
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event to prevent memory leaks
        if (centralHall != null)
        {
            centralHall.OnPlayerLevelUp -= UpdateExpLvUI;
        }
    }

    private void UpdateExpLvUI(int level)
    {
        Debug.Log("UpdateExpLvUI activated");
        if (LevelNumber != null)
        {
            LevelNumber.text = level.ToString();
            Debug.Log("Level Loaded Successfully");
        }
    }
}
