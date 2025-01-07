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


    private void Start()
    {
        button.onClick.AddListener(() => finishTime = DateTime.Now);
    }

    public void OpenGlorySpeedUpPanel(IBoostableObject boostableObject, DateTime finishTime, TimeSpan totalDuration)
    {
        container.SetActive(true);
        button.onClick.AddListener(boostableObject.BoostProgress);
        this.finishTime = finishTime;
    }

//    public void OpenGlorySpeedUpPanel(Structure_SO structureSO, Structure structure)
//    {
//        container.SetActive(true);
//        button.onClick.AddListener(structure.SpeedUpBuildingProgress);
//    }

//    public void OpenGlorySpeedUpPanel(Crop_SO cropSO, Plot p)
//    {
//        container.SetActive(true);

////        button.onClick.AddListener(p.SpeedUpBuildingProgress);
//    }

//    public void OpenGlorySpeedUpPanel(ResourceProducer_SO rpSO, ResourceProducer rp)
//    {
//        container.SetActive(true);

//        button.onClick.AddListener(rp.SpeedUpResourceProgress);
//    }

    public void CloseGlorySpeedUpPanel()
    {
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

        print("Received!!");

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
