using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldManager : MonoBehaviour
{
    public int playerGold = 0; // Amount of gold the player has
    public TMPro.TMP_Text goldText; // Reference to the TextMesh component

    void Start()
    {
        UpdateGoldUI();
    }

    // Method to format and update the gold UI
    public void UpdateGoldUI()
    {
        string formattedGold;

        if (playerGold >= 1000000) // If gold is 1 million or more
        {
            formattedGold = (playerGold / 1000000f).ToString("0.000") + " M"; // Show as X.XXX M
        }
        else if (playerGold >= 100000) // If gold is 100,000 or more
        {
            formattedGold = (playerGold / 1000).ToString("0") + "k"; // Show as XXXk
        }
        else
        {
            formattedGold = playerGold.ToString(); // Show the full number for smaller amounts
        }

        goldText.text = formattedGold;
    }

    // Method to add gold
    public void AddGold(int amount)
    {
        playerGold += amount;
        UpdateGoldUI(); // Update the TextMesh to show the new gold amount
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) // Add gold when "G" is pressed
        {
            AddGold(10000); // Example: Add 10,000 gold for testing
        }
    }
}