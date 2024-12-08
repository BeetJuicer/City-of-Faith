using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CropShopTemplate : MonoBehaviour
{
    private Crop_SO so;
    private CropManager cropManager;

    [SerializeField] private TMP_Text titleTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text costTxt;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button button;

    public void Init(Crop_SO so, CropManager cm)
    {
        this.so = so;
        this.cropManager = cm;

        titleTxt.text = so.cropName;
        descriptionTxt.text = so.cropDetails;
        itemImage.sprite = so.cropImage;
        costTxt.text = so.cropPrice.Values.First().ToString(); //First for now. a bit hacky.

        // check price, enable button if enough money.
        button.enabled = (ResourceManager.Instance.HasEnoughCurrency(so.cropPrice));
        // call cm.Purchase when button is clicked.
        button.onClick.AddListener(() => cm.Purchase(so));
    }

    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
