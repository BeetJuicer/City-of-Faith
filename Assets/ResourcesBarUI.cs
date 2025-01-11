using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcesBarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText; // Reference to a UI Text component for gold
    [SerializeField] private TMP_Text gloryText; // Reference to a UI Text component for glory

    private ResourceManager resourceManager;

    private void Start()
    {
        // Assuming ResourceManager is a singleton or accessible in the scene
        resourceManager = FindObjectOfType<ResourceManager>();
        if (resourceManager == null)
        {
            Debug.Log("ResourceManager not found in the scene.");
        }
        Debug.Log("ResourceManagerFound");
        UpdateResourcesUI();
    }


    public void UpdateResourcesUI()
    {
        Debug.Log("UpdateResourcesUI activated");

        if (resourceManager != null)
        {
            if (resourceManager.PlayerCurrencies.ContainsKey(Currency.Gold) &&
                resourceManager.PlayerCurrencies.ContainsKey(Currency.Glory))
            {
                int goldValue = resourceManager.PlayerCurrencies[Currency.Gold];
                int gloryValue = resourceManager.PlayerCurrencies[Currency.Glory];

                Debug.Log($"Gold Value: {goldValue}");
                Debug.Log($"Glory Value: {gloryValue}");

                goldText.text = goldValue.ToString();
                gloryText.text = gloryValue.ToString();
            }
            else
            {
                Debug.LogWarning("PlayerCurrencies dictionary does not contain Gold or Glory keys.");
            }
        }
        else
        {
            Debug.LogWarning("ResourceManager is not set.");
        }
    }

}
