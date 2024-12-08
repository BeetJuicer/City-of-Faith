using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class CropImageDisplay : MonoBehaviour
{
    [SerializeField] private GameObject harvestCanvas;
    private Plot plot;

    private void Start()
    {
        Plot plot = GetComponentInParent<Plot>();
    }

    // Method to put the crop image to Image UI
    public void UpdateVisual(Plot.PlotState state)
    {
        harvestCanvas.SetActive(state == Plot.PlotState.RIPE);
    }
}
