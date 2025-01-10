using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopManager : MonoBehaviour
{
    //ShopItem
    //public Structure_SO[] Shop_SO;
    //public Button[] myPurchaseBtns;

    public GameObject ShopManagerUi; // Reference para ma-deactivate Shop UI onclick
    public BuildingOverlay buildingOverlay; // Reference para ma-activate Building overlay onclick
    public Dialogue dialogue;
    public BuildingMoveUIManager moveUI;

    [SerializeField] private CentralHall centralhall;
    [SerializeField] private GameObject ShopTemplate;
    [SerializeField] private GameObject shopContent;



    //private ItemCategory currentCategory = ItemCategory.Buildings;

    private void Start()
    {
        LoadPanels();
    }

    public void LoadPanels()
    {
        if (centralhall == null)
        {
            Debug.LogError("UnlockManager reference is missing in ShopManager!");
            return;
        }

         if (shopContent == null)
        {
            Debug.LogError("ShopContent reference is missing in ShopManager!");
            return;
        }

        List<Structure_SO> unlockedStructures = centralhall.UnlockedStructures;
        List<Structure_SO> lockedStructures = centralhall.LockedStructures;

        Debug.Log($"Unlocked structures count: {unlockedStructures.Count}");
        Debug.Log($"Locked structures count: {lockedStructures.Count}");

        foreach (var structure in unlockedStructures)
        {
            Debug.Log("Instantiating unlocked card for: " + structure.name);
            var card = Instantiate(ShopTemplate, shopContent.transform);
            card.GetComponent<ShopTemplate>().Init(structure, this, isLocked: true);
            card.SetActive(true);
            Debug.Log("Unlocked card instantiated and initialized.");
        }

        foreach (var structure in lockedStructures)
        {
            Debug.Log("Instantiating locked card for: " + structure.name);
            var card = Instantiate(ShopTemplate, shopContent.transform);
            card.GetComponent<ShopTemplate>().Init(structure, this, isLocked: false);
            card.SetActive(true);
            Debug.Log("Locked card instantiated and initialized.");
        }
    }

    public void PurchaseItem(Structure_SO so)
    {
        ShopManagerUi.SetActive(false);
        dialogue.OnShopItemClicked();
        buildingOverlay.EnterBuildMode(so);
        moveUI.OpenMoveUI();
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
