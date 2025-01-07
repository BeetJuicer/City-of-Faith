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
    public void OpenGlorySpeedUpPanel(Structure_SO structureSO, Structure structure, DateTime finishTime)
    {
        this.finishTime = finishTime;
        nameText.text = structureSO.structureName;

        container.SetActive(true);
        button.enabled = true;
    }

    private void Update()
    {
        var durationLeft = finishTime.Subtract(DateTime.Now);
        timeLeftText.text = durationLeft.ToString();

        if (durationLeft.Seconds <= 0)
        {
            PriceText.text = "Finished";
            button.enabled = false;
        }
        else
        {
            PriceText.text = CalculateGloryCost(durationLeft).ToString();
        }
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
