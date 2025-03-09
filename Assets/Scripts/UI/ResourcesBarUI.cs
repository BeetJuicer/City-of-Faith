using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourcesBarUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText; // Reference to a UI Text component for gold
    [SerializeField] private TMP_Text gloryText; // Reference to a UI Text component for glory
    [SerializeField] private TMP_Text shopGoldText; // Reference to a UI Text component for gold
    [SerializeField] private TMP_Text shopGloryText;
    [SerializeField] GameObject CoinBar;
    public static RectTransform coinBarRect;
    public static Vector3 coinBarPosition;
    /*
     Note: Make sure that ResourceManager is above this script in execution order.
     */

    private void Start()
    {
        coinBarRect = CoinBar.GetComponent<RectTransform>();
        coinBarPosition = coinBarRect.position;
        Initialize();
    }

    private void Initialize()
    {
        print("initializing resource bar UI ");
        ResourceManager rm = ResourceManager.Instance;

        int goldValue = rm.PlayerCurrencies[Currency.Gold];
        int gloryValue = rm.PlayerCurrencies[Currency.Glory];

        Debug.Log($"Gold Value: {goldValue}");
        Debug.Log($"Glory Value: {gloryValue}");

        goldText.text = goldValue.ToString();
        gloryText.text = gloryValue.ToString();
        shopGoldText.text = goldValue.ToString();
        shopGloryText.text = gloryValue.ToString();

        ResourceManager.Instance.OnCurrencyUpdated += ResourceManager_OnCurrencyUpdated;
    }

    private void OnDisable()
    {
        ResourceManager.Instance.OnCurrencyUpdated -= ResourceManager_OnCurrencyUpdated;
    }

    private void ResourceManager_OnCurrencyUpdated(Currency type, int amount)
    {
        print($"Should be setting UI to {amount}.");

        if (type == Currency.Gold)
        {
            goldText.text = amount.ToString();
            shopGoldText.text = amount.ToString();
        }
        else if (type == Currency.Glory)
        {
            gloryText.text = amount.ToString();
            shopGloryText.text = amount.ToString();
        }
        else
            Debug.LogWarning("Currency type not handled by resources UI!");
    }

}
