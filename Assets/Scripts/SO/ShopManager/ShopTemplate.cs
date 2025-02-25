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
    [SerializeField] private ResourceManager rm;

    [SerializeField] private TMP_Text titleTxt;
    [SerializeField] private TMP_Text costTxt;
    [SerializeField] private TMP_Text expTxt;
    [SerializeField] private Image itemImg;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockedOverlay;


    public void Init(Structure_SO so, ShopManager sm, bool isLocked)
    {
        this.siso = so;
        this.shopManager = sm;

        titleTxt.text = so.structureName;
        costTxt.text = so.currencyRequired.Values.First().ToString();
        expTxt.text = so.expGivenOnBuild.ToString();
        itemImg.sprite = so.displayImage;

        lockedOverlay.SetActive(isLocked);
        //Debug.Log($"Setting lock overlay for {so.structureName} to {!isLocked}");

        GetComponent<Button>().interactable = !isLocked;
        //ResourceManager.Instance.HasEnoughCurrency(so.currencyRequired

        GetComponent<Button>().onClick.RemoveAllListeners();
        print("Init!");

        //foreach (var kv in so.currencyRequired)
        //{
        //    print($"Currency required of {so.name}: {so.currencyRequired[Currency.Gold]}");
        //}
        if (ResourceManager.Instance.HasEnoughCurrency(so.currencyRequired))
        {
            //Debug.Log("Resource Manager: " + ResourceManager.Instance.HasEnoughCurrency(so.currencyRequired));
            GetComponent<Button>().onClick.AddListener(() => sm.PurchaseItem(so));
            costTxt.color = Color.black;
        }
        else
        {
            GetComponent<Button>().onClick.AddListener(() => Feedback());
            costTxt.color = Color.red;
            //other indicators na bawal bilhin
            //bg color ng card or logo ng coin na may strikethrough
        }
    }


    private void Feedback()
    {
        //error sound
        //shake card
    }



    //public void OnClick()
    //{
    //    Debug.Log("ShopTemplate clicked!");
    //    shopManager.PurchaseItem(siso);
    //}

    //private void OnDestroy()
    //{
    //    button.onClick.RemoveAllListeners();
    //}
}

