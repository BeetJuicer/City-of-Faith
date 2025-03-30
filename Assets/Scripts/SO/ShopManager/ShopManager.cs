using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ShopManager : MonoBehaviour
{

    public GameObject ShopManagerUi; // Reference para ma-deactivate Shop UI onclick
    public BuildingOverlay buildingOverlay; // Reference para ma-activate Building overlay onclick
    public Dialogue dialogue;
    public BuildingMoveUIManager moveUI;

    [SerializeField] private CentralHall centralhall;
    [SerializeField] private GameObject ShopTemplate;
    [SerializeField] private GameObject shopContent;
    [SerializeField] private GameObject buildingOverlayObject;

    private ItemCategory currentCategory = ItemCategory.Buildings;


    private void Start()
    {
        centralhall.OnPlayerLevelUp += RefreshShop;
        FilterItemsByCategory(ItemCategory.Buildings);
        buildingOverlayObject.SetActive(true);
    }

    private void RefreshShop(int level)
    {
        FilterItemsByCategory(currentCategory);
    }

    private void FilterItems(ItemCategory category)
    {
        currentCategory = category;
        FilterItemsByCategory(ItemCategory.Buildings);
    }

    private void ClearShopContent()
    {
        foreach (Transform child in shopContent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    //public void LoadPanels()
    //{
    //    if (centralhall == null)
    //    {
    //        Debug.LogError("UnlockManager reference is missing in ShopManager!");
    //        return;
    //    }

    //     if (shopContent == null)
    //    {
    //        Debug.LogError("ShopContent reference is missing in ShopManager!");
    //        return;
    //    }

    //    List<Structure_SO> unlockedStructures = centralhall.UnlockedStructures;
    //    List<Structure_SO> lockedStructures = centralhall.LockedStructures;

    //    Debug.Log($"Unlocked structures count: {unlockedStructures.Count}");
    //    Debug.Log($"Locked structures count: {lockedStructures.Count}");

    //    foreach (var structure in unlockedStructures)
    //    {
    //        Debug.Log("Instantiating unlocked card for: " + structure.name);
    //        var card = Instantiate(ShopTemplate, shopContent.transform);
    //        card.GetComponent<ShopTemplate>().Init(structure, this, isLocked: true);
    //        card.SetActive(true);
    //        Debug.Log("Unlocked card instantiated and initialized.");
    //    }

    //    foreach (var structure in lockedStructures)
    //    {
    //        Debug.Log("Instantiating locked card for: " + structure.name);
    //        var card = Instantiate(ShopTemplate, shopContent.transform);
    //        card.GetComponent<ShopTemplate>().Init(structure, this, isLocked: false);
    //        card.SetActive(true);
    //        Debug.Log("Locked card instantiated and initialized.");
    //    }
    //}

    public void PurchaseItem(Structure_SO so)
    {
        ShopManagerUi.SetActive(false);
        //dialogue.OnShopItemClicked();
        buildingOverlayObject.SetActive(true);
        buildingOverlay.EnterBuildMode(so);
        moveUI.OpenMoveUI();
    }

    public void FilterItemsByCategory(ItemCategory category)
    {
        currentCategory = category; // Set the current category

        // Clear existing shop content
        ClearShopContent();

        List<Structure_SO> unlockedStructures = centralhall.UnlockedStructures;
        List<Structure_SO> lockedStructures = centralhall.LockedStructures;

        // Loop through the shop items and display items that match the selected category
        foreach (var structure in centralhall.UnlockedStructures)
        {
            if (structure.Category == category)
            {
                var card = Instantiate(ShopTemplate, transform.position, Quaternion.identity, shopContent.transform);
                card.GetComponent<ShopTemplate>().Init(structure, this, isLocked: false);
                //print($"Initialized {structure.name}");
                card.SetActive(true);
            }
        }

        foreach (var structure in centralhall.LockedStructures)
        {
            if (structure.Category == category)
            {
                var card = Instantiate(ShopTemplate, transform.position, Quaternion.identity, shopContent.transform);
                card.GetComponent<ShopTemplate>().Init(structure, this, isLocked: true);
                //print($"Initialized {structure.name}");
                card.SetActive(true);
            }
        }
    }

    // Optional methods for UI buttons
    public void ShowBuildings()
    {
        FilterItemsByCategory(ItemCategory.Buildings);
    }

    public void ShowDecorations()
    {
        FilterItemsByCategory(ItemCategory.Decorations);
    }


    //public void ShowBuildings()
    //{
    //    FilterItemsByCategory(ItemCategory.Buildings);
    //}

    //public void ShowDecoration()
    //{
    //    FilterItemsByCategory(ItemCategory.Decoration);
    //}


}
