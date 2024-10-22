using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    public int coins;
    public TMP_Text coinUI;
    public ShopItemSO[] ShopItemsSO;
    public GameObject[] ShopPanelsGO;
    public ShopTemplate[] shopPanels;
    public Button[] myPurchaseBtns;

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
        checkPurchaseable();
    }



    public void addCoins()
    {
        coins = coins + 100;
        coinUI.text = coins.ToString();
        checkPurchaseable();

    }

    public void checkPurchaseable()
    {
        for (int i = 0; i < ShopItemsSO.Length; i++)
        {
            if (coins >= ShopItemsSO[i].baseCost)
                myPurchaseBtns[i].interactable = true;
            else
                myPurchaseBtns[i].interactable = false;
        }
    }

    public void PurchaseItem(int btnNo)
    {
        if (coins >= ShopItemsSO[btnNo].baseCost)
        {
            coins = coins - ShopItemsSO[btnNo].baseCost;
            coinUI.text = coins.ToString();
            checkPurchaseable();
        }
    }

    public void FilterItemsByCategory(ItemCategory category)
    {
        currentCategory = category;  // Set the current category

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
