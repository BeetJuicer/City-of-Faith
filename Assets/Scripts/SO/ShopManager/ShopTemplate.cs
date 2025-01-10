using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class ShopTemplate : MonoBehaviour
{
    private Structure_SO siso;
    private ShopManager shopManager;


    [SerializeField] private TMP_Text titleTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text costTxt;
    [SerializeField] private Image itemImg;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockedOverlay;

    public void Init(Structure_SO so, ShopManager sm, bool isLocked)
    {
        this.siso = so;
        this.shopManager = sm;

        titleTxt.text = so.structureName;
        costTxt.text = so.currencyRequired.Values.First().ToString();
        itemImg.sprite = so.displayImage;

        lockedOverlay.SetActive(!isLocked);
        Debug.Log($"Setting lock overlay for {so.structureName} to {!isLocked}");

        GetComponent<Button>().interactable = isLocked;

        GetComponent<Button>().onClick.RemoveAllListeners();
        if (!isLocked)
        {
            GetComponent<Button>().onClick.AddListener(() => sm.PurchaseItem(so));
        }
    }

    public void OnClick()
    {
        Debug.Log("ShopTemplate clicked!");
        shopManager.PurchaseItem(siso);
    }

    //private void OnDestroy()
    //{
    //    button.onClick.RemoveAllListeners();
    //}
}

