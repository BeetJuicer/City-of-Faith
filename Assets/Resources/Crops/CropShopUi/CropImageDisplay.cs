using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class CropImageDisplay : MonoBehaviour
{
    [SerializeField] private GameObject harvestCanvas;
    [SerializeField] private Image image;
    private Plot plot;

    private void Start()
    {
        Plot plot = GetComponentInParent<Plot>();
    }

    // Method to put the crop image to Image UI
    public void UpdateVisual(Plot.PlotState state, Crop_SO so)
    {
        image.sprite = so.cropImage;
        harvestCanvas.SetActive(state == Plot.PlotState.RIPE);
    }
}
