using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class HarvestUI : MonoBehaviour
{
    [SerializeField] private GameObject harvestCanvas;
    [SerializeField] private Image image;
    private ResourceProducer Rp;

    private void Start()
    {
        ResourceProducer Rp = GetComponentInParent<ResourceProducer>();
    }

    // Method to put the crop image to Image UI
    public void Update()
    {
        if (Rp.CurrentProducerState == ProducerState.Ready_To_Claim)
        {
            harvestCanvas.SetActive(true);
        }
        else { harvestCanvas.SetActive(false); }

    }
}
