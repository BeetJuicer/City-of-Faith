using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopManager : MonoBehaviour
{
    //ShopItem
    public Structure_SO[] Shop_SO;
    //public Button[] myPurchaseBtns;

    public GameObject ShopManagerUi; // Reference para ma-deactivate Shop UI onclick
    public BuildingOverlay buildingOverlay; // Reference para ma-activate Building overlay onclick
    public Dialogue dialogue;
    public BuildingMoveUIManager moveUI;

    [SerializeField] private GameObject ShopTemplate;
    [SerializeField] private GameObject shopContent;



    //private ItemCategory currentCategory = ItemCategory.Buildings;

    private void Start()
    {
        LoadPanels();
    }

    public void LoadPanels()
    {
        for (int i = 0; i < Shop_SO.Length; i++)
        {
            Debug.Log("Instantiating card for: " + Shop_SO[i].name);
            var card = Instantiate(ShopTemplate, shopContent.transform);
            card.GetComponent<ShopTemplate>().Init(Shop_SO[i], this);
            card.SetActive(true);
            Debug.Log("Card instantiated and initialized.");
        }
    }

    public void PurchaseItem(Structure_SO so)
    {
        ShopManagerUi.SetActive(false);
        dialogue.OnShopItemClicked();
        moveUI.OpenMoveUI();
        buildingOverlay.EnterBuildMode(so);
        ResourceManager.Instance.AdjustPlayerCurrency(so.currencyRequired);
    }

    //public void FilterItemsByCategory(ItemCategory category)
    //{
    //    currentCategory = category; // Set the current category

    //    // Loop through the shop items and only display items that match the selected category
    //    for (int i = 0; i < ShopItemsSO.Length; i++)
    //    {
    //        if (ShopItemsSO[i].category == category)
    //        {
    //            var card = Instantiate(ShopTemplate, transform.position, Quaternion.identity, csContent.transform);
    //            card.GetComponent<ShopTemplate>().Init(ShopItemsSO[i], this);
    //            card.SetActive(true);
    //        }
    //        else
    //        {
    //            ShopPanelsGO[i].SetActive(false);
    //        }
    //    }
    //}

    //public void ShowBuildings()
    //{
    //    FilterItemsByCategory(ItemCategory.Buildings);
    //}

    //public void ShowDecoration()
    //{
    //    FilterItemsByCategory(ItemCategory.Decoration);
    //}


}
