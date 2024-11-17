using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    //ShopItem
    public int coins;
    public TMP_Text coinUI;
    public ShopItemSO[] ShopItemsSO;
    public GameObject[] ShopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;

    public GameObject ShopManagerUi; // Reference para ma-deactivate Shop UI onclick
    public GameObject BuildingOverlay; // Reference para ma-activate Building overlay onclick


    private ItemCategory currentCategory = ItemCategory.Buildings;

    private void Start()
    {
        FilterItemsByCategory(currentCategory);

        for (int i = 0; i < ShopItemsSO.Length; i++)
        {
            ShopPanelsGO[i].SetActive(true);
        }

        coinUI.text = coins.ToString();
        LoadPanels();
    }

    public void addCoins()
    {
        coins = coins + 100;
        coinUI.text = coins.ToString();
    }
    public void PurchaseItem(int btnNo)
    {
        // New functionality: Hide Shop UI and show Building Overlay
        ShopManagerUi.SetActive(false); // Deactivate the shop UI
        BuildingOverlay.SetActive(true); // Activate the building overlay
    }


    public void FilterItemsByCategory(ItemCategory category)
    {
        currentCategory = category; // Set the current category

        // Loop through the shop items and only display items that match the selected category
        for (int i = 0; i < ShopItemsSO.Length; i++)
        {
            if (ShopItemsSO[i].category == category)
            {
                ShopPanelsGO[i].SetActive(true);
                shopPanels[i].titleTxt.text = ShopItemsSO[i].title;
                shopPanels[i].descriptionTxt.text = ShopItemsSO[i].description;
                shopPanels[i].costTxt.text = ShopItemsSO[i].baseCost.ToString();
                shopPanels[i].itemImage.sprite = ShopItemsSO[i].itemImage;
            }
            else
            {
                ShopPanelsGO[i].SetActive(false);
            }
        }
    }

    public void ShowBuildings()
    {
        FilterItemsByCategory(ItemCategory.Buildings);
    }

    public void ShowDecoration()
    {
        FilterItemsByCategory(ItemCategory.Decoration);
    }

    public void LoadPanels()
    {
        for (int i = 0; i < ShopItemsSO.Length; i++)
        {
            shopPanels[i].titleTxt.text = ShopItemsSO[i].title;
            shopPanels[i].descriptionTxt.text = ShopItemsSO[i].description;
            shopPanels[i].costTxt.text = ShopItemsSO[i].baseCost.ToString();
            shopPanels[i].itemImage.sprite = ShopItemsSO[i].itemImage;
        }

    }
}
