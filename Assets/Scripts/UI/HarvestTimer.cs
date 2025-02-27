using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using static Database;


public class HarvestTimer : MonoBehaviour
{
    private Plot plot;
    [SerializeField] private Slider timerSlider;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject timerUI;


    private void Start()
    {
        plot = GetComponentInParent<Plot>(); // Assumes UI is a child of the plot
        timerSlider.gameObject.SetActive(false); // Hide initially
    }

    private void Update()
    {
        if (plot == null || timerSlider == null)
            return;

        if (plot.CurrentPlotState == Plot.PlotState.GROWING)
        {
            timerSlider.gameObject.SetActive(true); // Show the slider when growing
            timerText.gameObject.SetActive(true);
            UpdateSlider();
        }
        else
        {
            timerSlider.gameObject.SetActive(false); // Hide when not growing
            timerText.gameObject.SetActive(false);
        }
    }

    private void UpdateSlider()
    {
        TimeSpan totalDuration = plot.GetTotalDuration();
        TimeSpan remainingTime = plot.GrowthFinishTime - DateTime.Now;

        float progress = 1f - (float)(remainingTime.TotalSeconds / totalDuration.TotalSeconds);
        timerSlider.value = Mathf.Clamp01(progress); // Ensures value stays between 0 and 1
        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
    }
}
