using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private GameObject hudCanvas;
    [SerializeField] private GameObject shopButton;
    [SerializeField] private GameObject boostButton;
    [SerializeField] private GameObject questButton;

    // This Script is for the Tutorial so that HUD canvas can be disable and enable

    public void HideAllExceptShopButton()
    {
        if (hudCanvas == null || shopButton == null)
        {
            Debug.LogWarning("HUD Canvas or Shop Button is not assigned.");
            return;
        }

        // Loop through all children of the HUD Canvas
        foreach (Transform child in hudCanvas.transform)
        {
            // Set active false for everything except the ShopButton
            if (child.gameObject != shopButton)
            {
                child.gameObject.SetActive(false);
            }
        }

        // Ensure the shop button stays visible
        shopButton.SetActive(true);
    }
    public void HideAllExceptQuestButton()
    {
        if (hudCanvas == null || questButton == null)
        {
            Debug.LogWarning("HUD Canvas or Quest Button is not assigned.");
            return;
        }
        // Loop through all children of the HUD Canvas
        foreach (Transform child in hudCanvas.transform)
        {
            // Set active false for everything except the ShopButton
            if (child.gameObject != questButton)
            {
                child.gameObject.SetActive(false);
            }
        }

        // Ensure the shop button stays visible
        questButton.SetActive(true);
    }
    public void HideAllExceptBoostButton()
    {

        // Loop through all children of the HUD Canvas
        foreach (Transform child in hudCanvas.transform)
        {
            // Set active false for everything except the ShopButton
            if (child.gameObject != boostButton)
            {
                child.gameObject.SetActive(false);
            }
        }

        // Ensure the shop button stays visible
        boostButton.SetActive(true);
    }

    public void ShowAll()
    {
        if (hudCanvas == null)
        {
            Debug.LogWarning("HUD Canvas is not assigned.");
            return;
        }

        // Reactivate all children
        foreach (Transform child in hudCanvas.transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void HideAll()
    {
        if (hudCanvas == null)
        {
            Debug.LogWarning("HUD Canvas is not assigned.");
            return;
        }

        // Reactivate all children
        foreach (Transform child in hudCanvas.transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
