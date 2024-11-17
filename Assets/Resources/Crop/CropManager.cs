using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CropManager : MonoBehaviour
{
    public Crop_SO[] cropItemsSO; // Array of Crop Scriptable Objects
    public GameObject[] cropPanelsGO; // Array of UI panels for each crop
    public ShopTemplate[] cropPanels; // Array of templates for crop UI
    public Button[] cropPurchaseBtns; // Buttons for purchasing crops

    public TMP_Text coinUI; // Reference to coin text
    public int coins; // Player's coins
    public GameObject cropShopUI; // Reference to the crop shop UI
    public GameObject buildingOverlayUI; // Reference to the building overlay UI

    private void Start()
    {
        //coinUI.text = coins.ToString();
        LoadCropPanels();
        //CheckCropPurchaseable();
    }

    public void LoadCropPanels()
    {
        for (int i = 0; i < cropItemsSO.Length; i++)
        {
            cropPanels[i].titleTxt.text = cropItemsSO[i].cropName;
            cropPanels[i].descriptionTxt.text = cropItemsSO[i].cropDetails;
            cropPanels[i].costTxt.text = cropItemsSO[i].cropPrice.ToString();
            cropPanels[i].itemImage.sprite = cropItemsSO[i].cropImage;

        }
    }

    //public void AddCoins(int amount)
    //{
    //    coins += amount;
    //    coinUI.text = coins.ToString();
    //    CheckCropPurchaseable();
    //}

    //private void CheckCropPurchaseable()
    //{
    //    for (int i = 0; i < cropItemsSO.Length; i++)
    //    {
    //        if (coins >= cropItemsSO[i].cropPrice)
    //        {
    //            cropPurchaseBtns[i].interactable = true;
    //        }
    //        else
    //        {
    //            cropPurchaseBtns[i].interactable = false;
    //        }
    //    }
    //}

    public void PurchaseItem(int btnNo)
    {
        // New functionality: Hide Shop UI and show Building Overlay
        cropShopUI.SetActive(false); // Deactivate the shop UI
        buildingOverlayUI.SetActive(true); // Activate the building overlay
    }
}
