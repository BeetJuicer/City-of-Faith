using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For UI components like Slider
using TMPro; // Ensure this namespace is included for TMP_Text

public class LevelExpBarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text LevelNumber; // TMP_Text for displaying the level number
    [SerializeField] private Slider expSlider; // Slider for EXP bar
    [SerializeField] private TMP_Text expText; // TMP_Text for displaying EXP progress (optional)

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
        UpdateExpBar(centralHall.Exp); // Pass the current EXP
    }

    public void OnEnable()
    {
        centralHall.OnPlayerLevelUp += UpdateExpLvUI;
        centralHall.OnExpChanged += UpdateExpBar;
    }

    public void OnDisable()
    {
        centralHall.OnPlayerLevelUp -= UpdateExpLvUI;
        centralHall.OnExpChanged -= UpdateExpBar;
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

    private void UpdateExpBar(int exp)
    {
        if (centralHall == null || expSlider == null) return;

        int maxExp = centralHall.LevelUpExpRequirements[centralHall.Level];

        // Update the Slider
        expSlider.maxValue = maxExp;
        expSlider.value = exp;

        // Update optional EXP text
        if (expText != null)
        {
            expText.text = $"{exp} / {maxExp}";
        }

        Debug.Log($"EXP Bar Updated: {exp} / {maxExp}");
    }
}
