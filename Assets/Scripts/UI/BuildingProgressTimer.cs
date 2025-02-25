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


    private DateTime timeBuildStarted;
    private DateTime timeBuildFinished;

    private void Start()
    {
        structure = GetComponentInParent<Structure>();
        structure_SO = structure.structure_so;
        InitializeBuildTimer();
    }

    private void InitializeBuildTimer()
    {
        timeBuildStarted = DateTime.Now;
        timeBuildFinished = timeBuildStarted
                            .AddDays(structure_SO.BuildDays)
                            .AddHours(structure_SO.BuildHours)
                            .AddMinutes(structure_SO.BuildMinutes)
                            .AddSeconds(structure_SO.BuildSeconds);
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
            float totalBuildTime = (float)(timeBuildFinished - timeBuildStarted).TotalSeconds;
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