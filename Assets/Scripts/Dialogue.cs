using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static Database;
using static Dialogue;

public class Dialogue : MonoBehaviour
{


    public enum TutorialSection2Steps
    {
        StartDialogue,
        NPCDialogue2,
        ShowArrowToItem,
        PlaceBuilding,
        ShowArrowToBuilding,
        NPCDialogue5,
        ShowArrowToShop,
        ShowArrowToShop2,
        ShowArrowToPlot,
        ShowArrowToBuilding2,
        ShowArrowToQuest,
        PlaceBuilding2,
        NPCDialogue3,
        NPCDialogue4,
        NPCDialogue52,
        WaitForPlant,
        NPCDialogue6,
        WaitForCropBoost,
        NPCDialogue7,
        WaitForLevel3,
        WaitForLevel4,
        NPCDialogue8,
        WaitQuestClose,
        ShowArrowToBarn,
        NPCDialogue9,
        NPCDialogue10,
        ShowArrowToShop3,
        PlaceBuilding3,
        NPCDialogue11,
        NPCDialogue12,
        NPCDialogue13,
        PlaceBuilding4,
        NPCDialogue14,
        NPCDialogue15,
        WaitForLevel5,
        NPCDialogue16,
        PlaceBuilding5,
        NPCDialogue17,
        Complete,
    }

    [SerializeField] private TutorialSection2Steps section2Step;
    [SerializeField] private Dialogue_SO[] lines;
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private Button nextButton;
    [SerializeField] private float textSpeed;
    [SerializeField] private GameObject dialogueBox; // Reference to the dialogue box GameObject
    [SerializeField] private Image arrow; // Reference to the Arrow GameObject (not just the Image)
    [SerializeField] private PrefabImageController arrowInBuilding;
    [SerializeField] private ArrowPlot arrowPlot;
    [SerializeField] private GameObject shopButton;
    [SerializeField] private CentralHall centralHall;
    [SerializeField] private BuildingOverlay buildingOverlay;
    [SerializeField] private HUDController controller;



    private int dialogueIndex = 0;
    private int index = 0;
    private bool isCropClickComplete = false;

    void Start()
    {
        PlayerData playerData = Database.Instance.CurrentPlayerData;
        print(playerData);
        LoadTutorialProgressFromDatabase(playerData);

    }
    public void LoadTutorialProgressFromDatabase(PlayerData playerData)
    {
        section2Step = (TutorialSection2Steps)playerData.TutorialStep2;
        print(section2Step);
        dialogueBox.SetActive(false);
        HandleTutorialSteps2();
    }

    public void SaveTutorialProgressToDatabase(PlayerData playerData)
    {
        playerData.TutorialStep2 = (int)section2Step;


        Database.Instance.UpdateRecord(playerData);
    }

    void HandleTutorialSteps2()
    {
        PlayerData playerData = Database.Instance.CurrentPlayerData;
        SaveTutorialProgressToDatabase(playerData);

        print(section2Step);
        switch (section2Step)
        {
            case TutorialSection2Steps.StartDialogue:
                controller.HideAll();
                StartDialogue();
                break;
            case TutorialSection2Steps.ShowArrowToShop:
                controller.HideAllExceptShopButton(); // Call ShowAll() if you want to activate HUD canvas
                ShowArrow(new Vector3(802, -173, 0));
                Debug.Log("Show Arrow Section 1 tutorial");
                break;

            case TutorialSection2Steps.NPCDialogue2:
                StartDialogue2();
                break;

            case TutorialSection2Steps.ShowArrowToItem:
                ShowArrow(new Vector3(-350, 200, 0)); // Offset arrow above shop item
                break;

            case TutorialSection2Steps.PlaceBuilding:
                controller.HideAllExceptBoostButton();
                //Sa part na to, activate the arrow image in the villager house
                //
                break;

            case TutorialSection2Steps.ShowArrowToBuilding:
                arrowInBuilding.EnableImage();
                break;

            case TutorialSection2Steps.NPCDialogue5:
                StartDialogue2();
                break;
            case TutorialSection2Steps.NPCDialogue3: // Player Level Up, Plot Dialogue, Open Shop and Buy a Plot
                StartDialogue2();
                break;

            case TutorialSection2Steps.ShowArrowToShop3:
                controller.HideAllExceptShopButton(); // Call ShowAll() if you want to activate HUD canvas
                ShowArrow(new Vector3(802, -173, 0));
                Debug.Log("Show Arrow Barn tutorial");
                break;

            case TutorialSection2Steps.ShowArrowToQuest:
                controller.HideAllExceptQuestButton(); // Call ShowAll() if you want to activate HUD canvas
                ShowArrow(new Vector3(-565, -237, 0));
                break;

            case TutorialSection2Steps.ShowArrowToPlot: //Arrow points to Plot Shop Item, wait for the player to ClickShopItem (SHOW ARROW TO VILLAGER HOUSE)
                controller.HideAllExceptBoostButton();
                ShowArrow(new Vector3(15, 200, 0));
                break;

            case TutorialSection2Steps.PlaceBuilding2: //Building Overlay mode, user place the building
                controller.HideAllExceptBoostButton();
                break;

            case TutorialSection2Steps.ShowArrowToBuilding2: //Wait for Boost Structure
                arrowPlot.EnableImage();
                controller.HideAllExceptBoostButton(); //Close All HUD except the boost button
                break;

            case TutorialSection2Steps.NPCDialogue4: //NPC will order the player to boost the plot in progress building
                StartDialogue2();
                break;

            case TutorialSection2Steps.NPCDialogue52: //NPC will order player to plant Beet
                StartDialogue2();
                break;

            case TutorialSection2Steps.WaitForPlant:

                break;

            case TutorialSection2Steps.NPCDialogue6:
                StartDialogue2();
                break;

            case TutorialSection2Steps.WaitForCropBoost:
                break;

            case TutorialSection2Steps.NPCDialogue7:
                StartDialogue2();
                break;

            case TutorialSection2Steps.NPCDialogue8:
                StartDialogue2();
                break;

            case TutorialSection2Steps.WaitQuestClose:
                break;

            case TutorialSection2Steps.NPCDialogue9:
                StartDialogue2();
                break;

            case TutorialSection2Steps.WaitForLevel3: //Player will play the game to reach level 3
                Debug.Log("End of Plot Tutorial Reached");
                dialogueBox.SetActive(false);
                controller.ShowAll();
                break;

            case TutorialSection2Steps.NPCDialogue10:
                StartDialogue2();
                break;

            case TutorialSection2Steps.ShowArrowToShop2:
                controller.HideAllExceptShopButton();
                ShowArrow(new Vector3(802, -173, 0));
                break;

            case TutorialSection2Steps.ShowArrowToBarn:
                ShowArrow(new Vector3(250, 200, 0));
                break;

            case TutorialSection2Steps.PlaceBuilding3:
                controller.HideAllExceptBoostButton();
                break;

            case TutorialSection2Steps.NPCDialogue11:
                StartDialogue2();
                break;

            case TutorialSection2Steps.NPCDialogue12:
                StartDialogue2();
                break;

            case TutorialSection2Steps.NPCDialogue13:
                StartDialogue2();
                break;

            case TutorialSection2Steps.NPCDialogue14:
                StartDialogue2();
                break;

            case TutorialSection2Steps.NPCDialogue15:
                StartDialogue2();
                break;

            case TutorialSection2Steps.WaitForLevel4:
                Debug.Log("End of Barn Tutorial Reached");
                dialogueBox.SetActive(false);
                controller.ShowAll();
                break;

            case TutorialSection2Steps.WaitForLevel5:
                Debug.Log("End of Fishing Tutorial Reached");
                dialogueBox.SetActive(false);
                controller.ShowAll();
                break;

            case TutorialSection2Steps.NPCDialogue16:
                StartDialogue2();
                break;

            case TutorialSection2Steps.NPCDialogue17:
                StartDialogue2();
                break;
        }
    }

    void StartDialogue()
    {
        Debug.Log("Starting dialogue at index 0");
        textComponent.text = string.Empty;
        nextButton.onClick.AddListener(OnNextButtonClick);
        arrow.gameObject.SetActive(false);
        dialogueBox.SetActive(true);
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine()
    {
        Debug.Log("Typing line at index " + index);

        nextButton.interactable = true;
        bool isTypingSkipped = false;
        nextButton.onClick.AddListener(() => isTypingSkipped = true);

        foreach (char c in lines[dialogueIndex].dialogueLines[index].ToCharArray())
        {
            // Check if typing has been skipped
            if (isTypingSkipped)
            {
                // Display the full sentence and break the loop
                textComponent.text = lines[dialogueIndex].dialogueLines[index];
                break;
            }

            // Add the character and wait
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }

        Debug.Log("Line typed: " + lines[dialogueIndex].dialogueLines[index]);

        // Ensure the listener is removed to avoid issues when re-adding
        nextButton.onClick.RemoveListener(() => isTypingSkipped = true);
    }

    void OnNextButtonClick()
    {
        nextButton.interactable = false;

        if (textComponent.text == lines[dialogueIndex].dialogueLines[index])
        {
            NextLine();
        }
        else
        {
            StopAllCoroutines();
            textComponent.text = lines[dialogueIndex].dialogueLines[index];
            nextButton.interactable = true;
        }
    }

    void NextLine()
    {
        Debug.Log("NextLine called. Current index: " + index);
        if (index < lines[dialogueIndex].dialogueLines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            Debug.Log("Next line: " + index);
            StartCoroutine(TypeLine());
        }
        else
        {
            EndDialogue();
        }
    }
    void StartDialogue2()
    {
        Debug.Log("Starting dialogue at Dialogue index" + dialogueIndex);
        dialogueBox.SetActive(true);
        textComponent.text = string.Empty;
        index = 0; // this index is for every Lines in 1 scriptable object, sa loob ng so maraming lines
        dialogueIndex++; //this dialogueIndex is for every Scriptable Objects of Dialogue, to change the topic of tutorial
        StartCoroutine(TypeLine()); // Start typing...
    }

    void EndDialogue()
    {
        Debug.Log("Ending dialogue. Current steps - Section 2: " + section2Step);
        dialogueBox.SetActive(false);

        // Handle Section 2 Steps (Only if not already completed)     
        switch (section2Step)
        {
            case TutorialSection2Steps.StartDialogue:
                section2Step = TutorialSection2Steps.ShowArrowToShop;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue2:
                section2Step = TutorialSection2Steps.ShowArrowToBuilding;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue3:
                Debug.Log("Tutorial Plot Scene");
                section2Step = TutorialSection2Steps.ShowArrowToShop2;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue4:
                section2Step = TutorialSection2Steps.ShowArrowToBuilding2;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue5:
                section2Step = TutorialSection2Steps.WaitForPlant;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue6:
                section2Step = TutorialSection2Steps.WaitForCropBoost;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue7:
                arrowPlot.DisableImageBuilt();
                section2Step = TutorialSection2Steps.ShowArrowToQuest;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue8:
                section2Step = TutorialSection2Steps.WaitQuestClose;
                HandleTutorialSteps2();
                break;
            case TutorialSection2Steps.NPCDialogue9:
                section2Step = TutorialSection2Steps.WaitForLevel3;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue10:
                section2Step = TutorialSection2Steps.ShowArrowToShop3;
                HandleTutorialSteps2();
                break;

            //Skip NPC 11

            case TutorialSection2Steps.NPCDialogue12:
                section2Step = TutorialSection2Steps.WaitForLevel4;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue13:
                Debug.Log("End of Barn Tutorial Reached");
                dialogueBox.SetActive(false);
                controller.ShowAll();
                section2Step = TutorialSection2Steps.PlaceBuilding4;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue15:
                section2Step = TutorialSection2Steps.WaitForLevel5;
                HandleTutorialSteps2();
                break;

            case TutorialSection2Steps.NPCDialogue16:
                dialogueBox.SetActive(false);
                controller.ShowAll();
                section2Step = TutorialSection2Steps.PlaceBuilding5;
                HandleTutorialSteps2();
                break;

            default:
                Debug.Log("Steps Over");
                break;
        }
    }

    void ShowArrow(Vector3 newPosition)
    {
        RectTransform rectTransform = arrow.rectTransform; // Get RectTransform of the Image
        rectTransform.anchoredPosition = new Vector2(newPosition.x, newPosition.y); // Update position
        arrow.gameObject.SetActive(true);
        Debug.Log($"Arrow Position Updated: {newPosition}");
    }

    public void OnShopButtonClicked()
    {
        arrow.gameObject.SetActive(false);

        if (section2Step == TutorialSection2Steps.ShowArrowToShop)
        {
            Debug.Log("Show Arrow to Villager item Section 1 tutorial");
            section2Step = TutorialSection2Steps.ShowArrowToItem;
            HandleTutorialSteps2();

        }
        else if (section2Step == TutorialSection2Steps.ShowArrowToShop2)
        {
            Debug.Log("Show Arrow to Plot item Section 2 tutorial");
            section2Step = TutorialSection2Steps.ShowArrowToPlot;
            HandleTutorialSteps2();
        }

        else if (section2Step == TutorialSection2Steps.ShowArrowToShop3)
        {
            section2Step = TutorialSection2Steps.ShowArrowToBarn;
            HandleTutorialSteps2();
        }
        else
        {
            return;
        }
        Debug.Log("Shop Button Clicked");
    }

    public void OnShopItemClicked()
    {
        Debug.Log("ShopItem Clicked");

        arrow.gameObject.SetActive(false);

        if (section2Step == TutorialSection2Steps.ShowArrowToItem)
        {

            section2Step = TutorialSection2Steps.PlaceBuilding;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.ShowArrowToPlot)
        {

            section2Step = TutorialSection2Steps.PlaceBuilding2;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.ShowArrowToBarn)
        {
            section2Step = TutorialSection2Steps.PlaceBuilding3;
            HandleTutorialSteps2();
        }
        else
        {
            return;
        }

    }

    public void OnCropItemClicked()
    {

        if (isCropClickComplete)
        {
            return;
        }
        controller.HideAllExceptBoostButton();
        section2Step = TutorialSection2Steps.NPCDialogue6;
        HandleTutorialSteps2();
        isCropClickComplete = true;
    }

    public void PlaceBuilding()
    {
        if (section2Step == TutorialSection2Steps.PlaceBuilding)
        {
            Debug.Log("Place Building: Villager House");

            section2Step = TutorialSection2Steps.NPCDialogue2;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.PlaceBuilding2)
        {
            Debug.Log("Place Building: Plot");

            section2Step = TutorialSection2Steps.NPCDialogue4;
            HandleTutorialSteps2();

            //isPlotBuild = true;
        }
        else if (section2Step == TutorialSection2Steps.PlaceBuilding3)
        {
            Debug.Log("Place Building: Barn");
            section2Step = TutorialSection2Steps.NPCDialogue11;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.PlaceBuilding4)
        {
            Debug.Log("Place Building: Fishport");
            section2Step = TutorialSection2Steps.NPCDialogue14;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.PlaceBuilding5)
        {
            Debug.Log("Place Building: NPC");
            section2Step = TutorialSection2Steps.NPCDialogue17;
            HandleTutorialSteps2();
        }
        else
        {
            return;
        }

    }

    public void OnLevelUp()
    {

        if (section2Step == TutorialSection2Steps.WaitForLevel3)
        {
            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue10;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.WaitForLevel4)
        {
            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue13;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.WaitForLevel5)
        {
            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue16;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.ShowArrowToBuilding)
        {
            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue3;
            HandleTutorialSteps2();
        }
        else
        {
            return;
        }
    }

    public void OnBoostBuilding()
    {
        Debug.Log("Boosted in the Tutorial");

        if (section2Step == TutorialSection2Steps.ShowArrowToBuilding2)
        {
            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue5;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.WaitForCropBoost)
        {
            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue7;
            HandleTutorialSteps2();
        }
        else
        {
            return;
        }

    }

    public void OnClickQuest()
    {
        arrow.gameObject.SetActive(false);
        if (section2Step == TutorialSection2Steps.ShowArrowToQuest)
        {
            section2Step = TutorialSection2Steps.NPCDialogue8;
            HandleTutorialSteps2();
        }
        else
        {
            return;
        }
    }

    public void CloseQuest()
    {
        Debug.Log("Close Quest Box!!!");
        if (section2Step == TutorialSection2Steps.WaitQuestClose)
        {
            section2Step = TutorialSection2Steps.NPCDialogue9;
            HandleTutorialSteps2();
        }
        else
        {
            return;
        }
    }

    public void OnExitMiniGame()
    {
        Debug.Log("Barn Game Finish!");
        if (section2Step == TutorialSection2Steps.NPCDialogue11)
        {
            section2Step = TutorialSection2Steps.NPCDialogue12;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.NPCDialogue14)
        {
            section2Step = TutorialSection2Steps.NPCDialogue15;
            HandleTutorialSteps2();
        }
        else
        {
            return;
        }
    }

    //void ToggleButtons(bool state)
    //{
    //    // Find all GameObjects with the "UI_Button" tag
    //    GameObject[] allButtons = GameObject.FindGameObjectsWithTag("UI_Button");

    //    foreach (GameObject buttonObj in allButtons)
    //    {
    //        // Get the Button component and disable/enable it
    //        Button button = buttonObj.GetComponent<Button>();
    //        if (button != null && button != nextButton) // Exclude the Next button
    //        {
    //            button.interactable = state;
    //        }
    //    }
    //}

}
