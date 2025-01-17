using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcesBarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText; // Reference to a UI Text component for gold
    [SerializeField] private TMP_Text gloryText; // Reference to a UI Text component for glory

    private void Start()
    {
        ResourceManager.Instance.OnCurrencyLoaded += Initialize;
    }

    private void Initialize()
    {
        ResourceManager resourceManager = ResourceManager.Instance;

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
                Debug.LogError("PlayerCurrencies dictionary does not contain Gold or Glory keys.");
            }
        }
        else
        {
            Debug.LogWarning("ResourceManager is not set.");
        }

        ResourceManager.Instance.OnCurrencyUpdated += ResourceManager_OnCurrencyUpdated;
    }

    private void OnDisable()
    {
        ResourceManager.Instance.OnCurrencyLoaded -= Initialize;
        ResourceManager.Instance.OnCurrencyUpdated -= ResourceManager_OnCurrencyUpdated;
    }

    private void ResourceManager_OnCurrencyUpdated(Currency type, int amount)
    {
        print($"Should be setting UI to {amount}.");

        if (type == Currency.Gold)
            goldText.text = amount.ToString();
        else if (type == Currency.Glory)
            gloryText.text = amount.ToString();
        else
            Debug.LogWarning("Currency type not handled by resources UI!");
    }

}
