using System;
using UnityEngine;
using TMPro;

public class DateManager : MonoBehaviour
{
    public TextMeshProUGUI dateText;  // Reference to the TMP Text component

    void Update()
    {
        // Get the current time in UTC
        DateTime utcNow = DateTime.UtcNow;

        // Convert UTC time to Philippine Standard Time (UTC +8)
        DateTime philippinesTime = utcNow.AddHours(8);  // UTC +8 for the Philippines

        // Update the TMP text to include the current time in day/month/year hour:minute:second format
        dateText.text = "Date: " + philippinesTime.ToString("dd/MM/yyyy HH:mm:ss");
    }
}
