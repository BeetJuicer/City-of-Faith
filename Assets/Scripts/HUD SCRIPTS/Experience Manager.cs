using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Manages the player's experience, level progression, and updates the UI elements related to experience.
// The experience points and levels are determined by an AnimationCurve for custom leveling dynamics.

public class ExperienceManager : MonoBehaviour
{
    // Experience properties
    [Header("Experience")]
    [SerializeField] AnimationCurve experienceCurve; // Curve to define experience points needed per level

    int currentLevel = 0; // The player's current level
    int totalExperience = 0; // Total accumulated experience points
    int previousLevelsExperience; // Experience needed for the previous level
    int nextLevelsExperience; // Experience needed for the next level

    // UI properties
    [Header("Interface")]
    [SerializeField] TextMeshProUGUI levelText; // UI element to display the current level
    [SerializeField] TextMeshProUGUI experienceText; // UI element to display current experience as "current / required"
    [SerializeField] Image experienceFill; // UI element to show experience progress as a filled bar


    // Initializes the experience display and calculates initial experience thresholds.
    void Start()
    {
        UpdateLevel();
    }

    // Checks for mouse input to add experience points for testing purposes.
    // take note na for testing lang yan pwede nyo e comment out or tanggalin na lang.

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AddExperience(5); // Adds 5 experience points on mouse click
        }
    }

    // Adds a specified amount of experience to the total and updates the level if necessary.
    public void AddExperience(int amount)
    {
       // totalExperience += amount;
       // CheckForLevelUp();
       // UpdateInterface();
    }


    // Checks if the total experience has reached the threshold for the next level.
    // If so, increments the player's level and updates experience requirements.
    void CheckForLevelUp()
    {
        if (totalExperience >= nextLevelsExperience)
        {
            currentLevel++; // Increase level
            UpdateLevel();

            // Trigger any level-up effects (e.g., sound effects) here
        }
    }

    // Updates the experience thresholds for the current level and the next level.
    // Also updates the UI display.
    void UpdateLevel()
    {
        previousLevelsExperience = (int)experienceCurve.Evaluate(currentLevel); // Exp required for current level
        nextLevelsExperience = (int)experienceCurve.Evaluate(currentLevel + 1); // Exp required for next level
        UpdateInterface();
    }

    // Updates the level and experience display on the UI.
    // Calculates the fill amount of the experience bar based on current progress.
    void UpdateInterface()
    {
        // Calculate experience progress within the current level
        int start = totalExperience - previousLevelsExperience;
        int end = nextLevelsExperience - previousLevelsExperience;

        levelText.text = currentLevel.ToString();
        experienceText.text = start + " / " + end;

        // Avoid division by zero and clamp fill amount between 0 and 1
        experienceFill.fillAmount = end == 0 ? 0 : Mathf.Clamp01((float)start / end);
    }
}
