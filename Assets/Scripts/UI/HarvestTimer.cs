using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using static Database;


public class HarvestTimer : MonoBehaviour
{
    private Crop_SO crop_so;
    private Plot plot;
    [SerializeField] private Slider progressSlider;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject timerUI;


    private DateTime timeBuildStarted;
    private DateTime timeBuildFinished;

    private void Start()
    {
        plot = GetComponentInParent<Plot>();
        crop_so = plot.Crop_SO2;
        if (crop_so != null)
        {
            InitializeBuildTimer();
        }
        else
        {
            Debug.LogError("Crop_SO2 is null. Timer will not work!");
        }
    }

    private void InitializeBuildTimer()
    {
        timeBuildStarted = DateTime.Now;
        timeBuildFinished = timeBuildStarted
                            .AddDays(crop_so.daysToClaim)
                            .AddHours(crop_so.hoursToClaim)
                            .AddMinutes(crop_so.minutesToClaim)
                            .AddSeconds(crop_so.secondsToClaim);
        timerUI.SetActive(true);
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