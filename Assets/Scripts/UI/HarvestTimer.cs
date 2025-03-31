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
    [SerializeField] private GameObject timerUI;
    [SerializeField] private Image image;
    private Crop_SO crop_so;

    private void Start()
    {
        plot = GetComponentInParent<Plot>();
        timerSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (plot == null || timerSlider == null)
            return;

        if (plot.CurrentPlotState == Plot.PlotState.GROWING)
        {
            timerSlider.gameObject.SetActive(true); // Show the slider when growing
            UpdateSlider();
            UpdateCropImage(plot.Crop_SO1);
            timerUI.SetActive(true);

        }
        else
        {
            timerSlider.gameObject.SetActive(false); // Hide when not growing
            timerUI.SetActive(false);
        }
    }

    private void UpdateSlider()
    {
        TimeSpan totalDuration = plot.GetTotalDuration();
        TimeSpan remainingTime = plot.GrowthFinishTime - DateTime.Now;

        float progress = 1f - (float)(remainingTime.TotalSeconds / totalDuration.TotalSeconds);
        timerSlider.value = Mathf.Clamp01(progress); // Ensures value stays between 0 and 1
        //timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", remainingTime.Hours, remainingTime.Minutes, remainingTime.Seconds);
    }
    public void UpdateCropImage(Crop_SO so)
    {
        if (so == null)
        {
            print("null so");
            return;
        }
        image.sprite = so.cropImage;

    }
}
