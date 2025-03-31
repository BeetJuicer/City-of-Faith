using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq; // library para sa enumerables (dictionaries, lists, etc.)
using TMPro;

public class CropShopTemplate : MonoBehaviour
{
    private Crop_SO so;
    private CropManager cropManager;

    [SerializeField] private TMP_Text titleTxt;
    [SerializeField] private TMP_Text descriptionTxt;
    [SerializeField] private TMP_Text costTxt;
    [SerializeField] private TMP_Text goldAmount;
    [SerializeField] private TMP_Text expAmount;
    [SerializeField] private TMP_Text duration;
    [SerializeField] private Image itemImage;
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockedOverlay;

    public void Init(Crop_SO so, CropManager cm, bool isLocked)
    {
        this.so = so;
        this.cropManager = cm;

        titleTxt.text = so.cropName;
        descriptionTxt.text = so.cropDetails;
        itemImage.sprite = so.cropImage;
        costTxt.text = so.cropPrice.Values.First().ToString(); //First for now. a bit hacky.
        goldAmount.text = so.amountPerClaim.ToString();
        expAmount.text = so.expPerClaim.ToString();
        UpdateDurationText();


        lockedOverlay.SetActive(!isLocked);
        //Debug.Log($"Setting lock overlay for {so.cropName} to {!isLocked}");

        button.interactable = isLocked;

        button.onClick.RemoveAllListeners();
        if (!isLocked)
        {
            button.onClick.AddListener(() => cm.Purchase(so));
        }
        // check price, enable button if enough money.
        button.enabled = (ResourceManager.Instance.HasEnoughCurrency(so.cropPrice));
        // ideally, gray yung card.

        // call cm.Purchase when button is clicked.
        button.onClick.AddListener(() =>
        {
            cm.Purchase(so);
            NotifyDialogue();
        });
    }

    private void UpdateDurationText()
    {
        if (so.daysToClaim > 0)
        {
            duration.text = so.daysToClaim + " d";
        }
        else if (so.hoursToClaim > 0)
        {
            duration.text = so.hoursToClaim + " hr";
        }
        else if (so.minutesToClaim > 0)
        {
            duration.text = so.minutesToClaim + " m";
        }
        else
        {
            duration.text = so.secondsToClaim + " s";
        }
    }
    private void NotifyDialogue()
    {
        Debug.Log("On Crop Item Clicked!!!");
        Dialogue dialogue = FindObjectOfType<Dialogue>();
        if (dialogue != null)
        {
            dialogue.OnCropItemClicked();   // Call the method
        }
        else
        {
            Debug.LogWarning("Dialogue script not found in the scene!");
        }
    }


    private void OnDestroy()
    {
        button.onClick.RemoveAllListeners();
    }
}
