using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;
using System.Reflection;

public class HarvestUI : MonoBehaviour
{
    [SerializeField] private GameObject harvestCanvas;
    [SerializeField] private Image image;
    private ResourceProducer Rp;

    private void Start()
    {
        Rp = GetComponentInParent<ResourceProducer>();

        if (Rp != null)
        {
            // Use Reflection to access the private field 'rp_SO'
            FieldInfo fieldInfo = typeof(ResourceProducer).GetField("rp_SO", BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo != null)
            {
                var rp_SO = fieldInfo.GetValue(Rp) as ResourceProducer_SO;
                if (rp_SO?.resource_SO?.resourceImage != null)
                {
                    image.sprite = rp_SO.resource_SO.resourceImage;
                }
                else
                {
                    Debug.LogWarning("Resource image or dependencies are missing!");
                }
            }
            else
            {
                Debug.LogError("Could not find 'rp_SO' field on ResourceProducer!");
            }
        }
        else
        {
            Debug.LogError("ResourceProducer component is missing on parent!");
        }
    }

    private void Update()
    {
        if (Rp != null && Rp.CurrentProducerState == ProducerState.Ready_To_Claim)
        {
            harvestCanvas.SetActive(true);
        }
        else
        {
            harvestCanvas.SetActive(false);
        }
    }
}
