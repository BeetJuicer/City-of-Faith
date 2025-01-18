using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.UI;

public class GlorySpeedUp : MonoBehaviour
{
    [SerializeField] private GameObject container;
    [SerializeField] private TextMeshProUGUI timeLeftText;
    [SerializeField] private TextMeshProUGUI PriceText;

    [SerializeField] private int costLow;
    [SerializeField] private int costMedium;
    [SerializeField] private int costHigh;
    [SerializeField] private Button button;

    private DateTime finishTime;
    public Dialogue dialogue;
    IBoostableObject boostableObject;

    private void Start()
    {
        //dialogue.boostBuilding();
    }

    public void OpenGlorySpeedUpPanel(IBoostableObject boostableObject)
    {
        container.SetActive(true);
        this.boostableObject = boostableObject;
        this.finishTime = boostableObject.GetTimeFinished();
        button.onClick.AddListener(Boost);
    }

    private void Boost()
    {
        boostableObject.BoostProgress();
        ResourceManager.Instance.AdjustPlayerCurrency(Currency.Glory, -CalculateGloryCost(finishTime.Subtract(DateTime.Now)));
        if (boostableObject.IsInBoostableState())
        {
            OpenGlorySpeedUpPanel(boostableObject);
        }
        else
        {
            CloseGlorySpeedUpPanel();
        }
        //refresh:
    }

    public void CloseGlorySpeedUpPanel()
    {
        finishTime = DateTime.Now;
        button.onClick.RemoveAllListeners();
        container.SetActive(false);
    }

    private void Update()
    {
        if (!container.activeInHierarchy)
            return;

        var durationLeft = finishTime.Subtract(DateTime.Now);

        if (durationLeft.Seconds <= 0)
        {
            CloseGlorySpeedUpPanel();
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
