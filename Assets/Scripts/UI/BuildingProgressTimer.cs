using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using static Database;


public class BuildingProgressTimer : MonoBehaviour
{
    private Structure_SO structure_SO;
    private Structure structure;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject timerUI;


    //private DateTime timeBuildStarted; unnecessary
    private DateTime timeBuildFinished;
    float totalBuildTime;

    private void Start()
    {
        structure = GetComponentInParent<Structure>();
        structure_SO = structure.structure_so;
        InitializeBuildTimer();
    }

    private void InitializeBuildTimer()
    {
        //build started is always when the app is opened, even if the building was started yesterday.
        /* old code.
        timeBuildStarted = DateTime.Now;
        timeBuildFinished = timeBuildStarted
                            .AddDays(structure_SO.BuildDays)
                            .AddHours(structure_SO.BuildHours)
                            .AddMinutes(structure_SO.BuildMinutes)
                            .AddSeconds(structure_SO.BuildSeconds);
        */

        //no need to calculate timeBuildFinished here. Structure.cs handles that
        timeBuildFinished = structure.TimeBuildFinished;
        totalBuildTime = (float)new TimeSpan(structure_SO.BuildDays, structure_SO.BuildHours, structure_SO.BuildMinutes, structure_SO.BuildSeconds).TotalSeconds;

    }

    private void Update()
    {
        UpdateTimerUI();
    }

    private void UpdateTimerUI()
    {
        TimeSpan remainingTime = timeBuildFinished - DateTime.Now;

        if (remainingTime.TotalSeconds > 0)
        {
            timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
            float remainingBuildTime = (float)remainingTime.TotalSeconds;
            progressSlider.value = Mathf.Clamp01(1 - (remainingBuildTime / totalBuildTime));
        }
        else
        {
            progressSlider.value = 1f;

            // Hide the timer UI once the build is complete
            timerUI.SetActive(false);
            enabled = false; // Stop updating the script
        }
    }
}