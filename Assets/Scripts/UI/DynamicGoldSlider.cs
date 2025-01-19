using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DynamicGoldSlider : MonoBehaviour
{
    public Slider slider;              // Reference to the Slider
    public RectTransform sliderRect;   // Reference to the Slider's RectTransform (for resizing)
    public TMPro.TMP_Text goldText;              // Reference to the Text component that shows the gold amount
    public float baseWidth = 200;        // Base width for the slider
    public int widthPerDigit = 100;     // How much to increase the width per digit

    // Function to update the slider's value and width based on current gold
    public void UpdateSlider(int currentGold)
    {
        slider.value = currentGold;    // Set the slider's value

        // Update the gold text to display the current amount of gold
        goldText.text = currentGold.ToString();

        // Calculate the number of digits in the current gold amount
        int digitCount = currentGold.ToString().Length;

        // Adjust the slider's width dynamically based on the digit count
        sliderRect.sizeDelta = new Vector2(baseWidth + (widthPerDigit * digitCount), sliderRect.sizeDelta.x);
    }
}