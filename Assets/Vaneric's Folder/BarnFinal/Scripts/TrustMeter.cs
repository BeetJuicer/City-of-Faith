using UnityEngine;
using UnityEngine.UI;

public class TrustMeter : MonoBehaviour
{
    public float maxTrust = 100f;
    private float currentTrust = 0f;
    public Slider trustSlider;

    void Start()
    {
        if (trustSlider != null)
        {
            trustSlider.maxValue = maxTrust;
            trustSlider.value = currentTrust;
        }
    }

    public void IncreaseTrust(float amount)
    {
        currentTrust += amount;
        currentTrust = Mathf.Clamp(currentTrust, 0, maxTrust);

        if (trustSlider != null)
        {
            trustSlider.value = currentTrust;
        }

        Debug.Log($"Trust increased! Current Trust: {currentTrust}/{maxTrust}");

        if (currentTrust >= maxTrust)
        {
            FindObjectOfType<GameManagerSheep>().EndGame();
        }
    }

    public float GetTrustPercentage()
    {
        return (currentTrust / maxTrust) * 100f;
    }
    public void ResetTrust()
    {
        currentTrust = 0f;
        if (trustSlider != null)
        {
            trustSlider.value = currentTrust;
        }
        Debug.Log("Trust meter reset!");
    }

}
