using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CropManager : MonoBehaviour
{
    public Crop_SO[] cropItemsSO; // Array of Crop Scriptable Objects
    public Button[] cropPurchaseBtns; // Buttons for purchasing crops

    public TMP_Text coinUI; // Reference to coin text
    public int coins; // Player's coins
    public GameObject cropShopUI; // Reference to the crop shop UI
    public GameObject buildingOverlayUI; // Reference to the building overlay UI
    public Canvas CropSelectionUI; // Para maopen sa plot, may public method



    //carl
    private Plot selectedPlot;
    [SerializeField] private GameObject cropCardTemplate;
    [SerializeField] private GameObject csContent;


    private void Start()
    {
        //coinUI.text = coins.ToString();
        //CheckCropPurchaseable();
        LoadCropPanels();
    }

    public void Purchase(Crop_SO so)
    {
        // this is only called when the button is enabled anyway, so no checking needed.
        selectedPlot.Plant(so);
        ResourceManager.Instance.AdjustPlayerCurrency(so.cropPrice);
    }

    public void LoadCropPanels()
    {
        for (int i = 0; i < cropItemsSO.Length; i++)
        {
            print(i);
            var card = Instantiate(cropCardTemplate, transform.position, Quaternion.identity, csContent.transform);
            card.GetComponent<CropShopTemplate>().Init(cropItemsSO[i], this);
            card.SetActive(true);
        }
    }

    public void OpenCropSelection(Plot plot) //Open the Crop Shop
    {
        selectedPlot = plot;

        if (CropSelectionUI != null)
        {
            CropSelectionUI.enabled = true;
        }
        else
        {
            Debug.LogError("Error in OpenCropSelection");
        }
    }

    public void CloseCropSelection() //Close the Crop Shop
    {
        if (CropSelectionUI != null)
        {
            CropSelectionUI.enabled = false;
        }
        else
        {
            Debug.LogError("Error in OpenCropSelection");
        }
    }

    public void PurchaseItem(int btnNo)
    {
        cropShopUI.SetActive(false); // Deactivate the shop UI
        buildingOverlayUI.SetActive(true); // Activate the building overlay
    }
}
