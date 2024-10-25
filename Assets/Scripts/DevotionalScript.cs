using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DevotionalScript : MonoBehaviour
{
    public GameObject DevotionalUI;
    public GameObject DevotionalNextButton;

    void Start()
    {
        ClearPreviousDate();
        // Get the current system date
        DateTime currentDate = DateTime.Now.Date;

        // Display the current date in the console
        Debug.Log("Current Date: " + currentDate.ToString("yyyy-MM-dd"));

        // Check if it's a new day
        if (IsNewDay(currentDate))
        {
            OpenDevotional();
            SaveCurrentDate(currentDate);
        }
    }

    void Update()
    {
        // Optional: You can add code here to continuously monitor for a new day, if required
    }

    // Method to open the devotional UI
    public void OpenDevotional()
    {
        DevotionalUI.SetActive(true);
        DevotionalNextButton.SetActive(false);
        Debug.Log("Devotional UI opened.");
    }

    // Method to check if it's a new day
    private bool IsNewDay(DateTime currentDate)
    {
        // Get the previously stored date from PlayerPrefs
        string previousDateStr = PlayerPrefs.GetString("PreviousDate", "");

        if (string.IsNullOrEmpty(previousDateStr))
        {
            // If no previous date was stored, consider it a new day
            return true;
        }

        // Convert the stored string to a DateTime object
        DateTime previousDate = DateTime.Parse(previousDateStr);

        // Compare the previous date with the current date
        return currentDate > previousDate;
    }

    // Method to save the current date to PlayerPrefs
    private void SaveCurrentDate(DateTime currentDate)
    {
        // Save the current date as a string in PlayerPrefs
        PlayerPrefs.SetString("PreviousDate", currentDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();
        Debug.Log("Date saved: " + currentDate.ToString("yyyy-MM-dd"));
    }

    public void ClearPreviousDate()
    {
        PlayerPrefs.DeleteKey("PreviousDate");
        PlayerPrefs.Save();
        Debug.Log("Previous date cleared from PlayerPrefs.");
    }
}
