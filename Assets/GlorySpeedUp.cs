using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class GlorySpeedUp : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI PriceText;

    [SerializeField] private int costLow;
    [SerializeField] private int costMedium;
    [SerializeField] private int costHigh;
    [SerializeField] private Button button;

    private DateTime finishTime;
    private DateTime totalDuration;
    private Structure attachedStructure;

    public void OpenGlorySpeedUpPanel(Structure_SO structureSO, Structure structure)
    {
        this.attachedStructure = structure;
        nameText.text = structureSO.structureName;
        container.SetActive(true);

        button.enabled = true;
        button.onClick.AddListener(structure.SpeedUpBuildingProgress);
    }

    public void CloseGlorySpeedUpPanel()
    {
        button.onClick.RemoveAllListeners();
        container.SetActive(false);
    }

    private void Update()
    {
        if (!container.activeInHierarchy) 
            return;


        var durationLeft = attachedStructure.TimeBuildFinished.Subtract(DateTime.Now);

        if (durationLeft.Seconds <= 0)
        {
            CloseGlorySpeedUpPanel();
            print("Open structure display panel here.");
            return;
        }

        timeLeftText.text = durationLeft.ToString(@"dd\.hh\:mm\:ss");
        int cost = CalculateGloryCost(durationLeft);
        PriceText.text = cost.ToString();
        button.enabled = ResourceManager.Instance.HasEnoughCurrency(Currency.Glory, cost);
    }

    public double GetPercentageLeft(TimeSpan part, TimeSpan whole)
    {
        double partInSeconds = part.TotalSeconds;
        double wholeInSeconds = whole.TotalSeconds;

        return (partInSeconds / wholeInSeconds) * 100;
    }

    private int CalculateGloryCost(TimeSpan duration)
    {
        if (duration.Days >= 1)
            return costHigh;
        else if (duration.Hours >= 1)
            return costMedium;
        else if (duration.Minutes >= 1)
            return costLow;
        else
            return 0;
    }
}
