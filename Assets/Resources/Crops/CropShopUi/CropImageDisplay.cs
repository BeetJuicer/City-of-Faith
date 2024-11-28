using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class CropImageDisplay : MonoBehaviour
{
    public Image cropImage;
    [SerializeField] private Plot plotScript;

    // Method to put the crop image to Image UI
    public void Setup(Plot plot)
    {
        plotScript = plot;

        // Get Crop_SO from the Plot.cs
        if (plotScript != null && plotScript.Crop_SO != null)
        {
            Crop_SO cropData = plotScript.Crop_SO;
            if (cropData != null)
            {
                cropImage.sprite = cropData.cropImage; // Update the Image
                Debug.Log("Crop image updated to: " + cropData.cropImage.name);
            }
        }
    }

    [Button("Test Setup")]
    public void TestSetup()
    {
        if (plotScript != null)
        {
            Setup(plotScript);
        }
        else
        {
            Debug.LogWarning("Plot script not assigned!");
        }
    }
}
