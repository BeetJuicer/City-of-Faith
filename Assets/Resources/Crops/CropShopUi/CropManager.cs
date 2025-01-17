using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEngine.Experimental.Rendering.Universal.PixelPerfectCamera;

public class CropManager : MonoBehaviour
{
    public GameObject cropShopUI; // Reference to the crop shop UI

    //carl
    private Plot selectedPlot;
    [SerializeField] private CentralHall centralhall;
    [SerializeField] private GameObject cropCardTemplate;
    [SerializeField] private GameObject csContent;


    private void Start()
    {
        LoadCropPanels();
    }

    public void Purchase(Crop_SO so)
    {
        // this is only called when the button is enabled anyway, so no checking needed.
        selectedPlot.Plant(so);
        ResourceManager.Instance.AdjustPlayerCurrency(so.cropPrice); // TODO: negative dapat
        CloseCropSelection();
    }

    public void LoadCropPanels()
    {
        if (centralhall == null)
        {
            Debug.LogError("UnlockManager reference is missing in ShopManager!");
            return;
        }

        if (csContent == null)
        {
            Debug.LogError("ShopContent reference is missing in ShopManager!");
            return;
        }

        List<Crop_SO> unlockedCrops = centralhall.UnlockedCrops;
        List<Crop_SO> lockedCrops = centralhall.LockedCrops;
        Debug.Log($"Unlocked crops count: {unlockedCrops.Count}");
        Debug.Log($"Locked crops count: {lockedCrops.Count}");

        foreach (var cropItemsSO in unlockedCrops)
        {
            var card = Instantiate(cropCardTemplate, transform.position, Quaternion.identity, csContent.transform);
            card.GetComponent<CropShopTemplate>().Init(cropItemsSO, this, isLocked: true);
            card.SetActive(true);
           // Debug.Log("Unlocked card instantiated and initialized.");
        }

        foreach (var cropItemsSO in lockedCrops)
        {
            var card = Instantiate(cropCardTemplate, transform.position, Quaternion.identity, csContent.transform);
            card.GetComponent<CropShopTemplate>().Init(cropItemsSO, this, isLocked: false);
            card.SetActive(true);
           // Debug.Log("Unlocked card instantiated and initialized.");
        }
    }

    public void OpenCropSelection(Plot plot) //Open the Crop Shop
    {
        selectedPlot = plot;

        if (cropShopUI != null)
        {
            cropShopUI.SetActive(true);
        }
        else
        {
            Debug.LogError("Error in OpenCropSelection");
        }
    }

    public void CloseCropSelection() //Close the Crop Shop
    {
        if (cropShopUI != null)
        {
            cropShopUI.SetActive(false);
        }
        else
        {
            Debug.LogError("Error in OpenCropSelection");
        }
    }
}
