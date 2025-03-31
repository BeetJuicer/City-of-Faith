using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Dialogue : MonoBehaviour
{

    public enum TutorialSection1Steps
    {
        StartDialogue,
        ShowArrowToShop,
        OpenShop,
        NPCDialogue2,
        ShowArrowToItem,
        PlaceBuilding,
        ShowArrowToBuilding,
        ShowArrowToBoost,
        NPCDialogue5,
        Complete,
    }

    public enum TutorialSection2Steps
    {
        ShowArrowToShop,
        ShowArrowToShop2,
        ShowArrowToPlot,
        ShowArrowToBuilding,
        PlaceBuilding2,
        NPCDialogue3,
        NPCDialogue4,
        NPCDialogue5,
        WaitForPlant,
        NPCDialogue6,
        WaitForCropBoost,
        NPCDialogue7,
        WaitForLevel3,
        NPCDialogue8,
        ShowArrowToBarn,
        Complete,
    }

    [SerializeField] private TutorialSection1Steps section1Step;
    [SerializeField] private TutorialSection2Steps section2Step;
    [SerializeField] private Dialogue_SO[] lines;
    [SerializeField] private TMP_Text textComponent;
    [SerializeField] private Button nextButton;
    [SerializeField] private float textSpeed;
    [SerializeField] private GameObject dialogueBox; // Reference to the dialogue box GameObject
    [SerializeField] private Image arrow; // Reference to the Arrow GameObject (not just the Image)
    [SerializeField] private Image arrowInBuilding;
    [SerializeField] private GameObject shopButton;
    [SerializeField] private CentralHall centralHall;
    [SerializeField] private BuildingOverlay buildingOverlay;
    [SerializeField] private HUDController controller;

    private int dialogueIndex = 0;
    private int index = 0;
    private bool isShopTutorialComplete = false;
    private bool isItemClickComplete = false;
    private bool isItemBuild = false;
    private bool isPlotBoosted = false;
    private bool isBuildingClicked = false;
    private bool isPlotBuild = false;
    private bool isCropClickComplete = false;
    private bool isTutorialComplete = false;
    private bool isPlotClickComplete = false;

    private HashSet<TutorialSection1Steps> completedSection1Steps = new HashSet<TutorialSection1Steps>();
    private HashSet<TutorialSection2Steps> completedSection2Steps = new HashSet<TutorialSection2Steps>();



    private Structure structureObservee;

    void Start()
    {
        section1Step = TutorialSection1Steps.StartDialogue;
        HandleTutorialSteps();
    }

    void HandleTutorialSteps()
    {
        Debug.Log("Current Step: " + section1Step); // Log the current step
        switch (section1Step)
        {
            case TutorialSection1Steps.StartDialogue:
                controller.HideAll();
                StartDialogue();
                break;
            case TutorialSection1Steps.ShowArrowToShop:
                controller.HideAllExceptShopButton(); // Call ShowAll() if you want to activate HUD canvas
                ShowArrow(new Vector3(802, -173, 0));
                break;

            case TutorialSection1Steps.NPCDialogue2:
                StartDialogue2();
                break;

            case TutorialSection1Steps.ShowArrowToItem:
                ShowArrow(new Vector3(-300, 200, 0)); // Offset arrow above shop item
                break;

            case TutorialSection1Steps.PlaceBuilding:
                controller.HideAllExceptBoostButton();
                //Sa part na to, activate the arrow image in the villager house
                //
                break;

            case TutorialSection1Steps.ShowArrowToBuilding:
                //wait
                break;

            case TutorialSection1Steps.ShowArrowToBoost:

                break;

            case TutorialSection1Steps.NPCDialogue5:
                StartDialogue2();
                break;

            //case TutorialSection1Steps.Complete:
            //    Debug.Log("Tutorial Complete!!!");
            //    ToggleButtons(true);
            //    break;

            default:
                Debug.LogWarning("Unhandled tutorial step: " + section1Step);
                break;
        }
    }

    void HandleTutorialSteps2()
    {
        switch (section2Step)
        {
            case TutorialSection2Steps.NPCDialogue3: // Player Level Up, Plot Dialogue, Open Shop and Buy a Plot
                StartDialogue2();
                break;

            case TutorialSection2Steps.ShowArrowToShop: //Arrow points the shop, wait for the player to OpenShop
                controller.HideAllExceptShopButton();
                ShowArrow(new Vector3(802, -173, 0));
                break;

            case TutorialSection2Steps.ShowArrowToShop2:
                controller.HideAllExceptShopButton(); // Call ShowAll() if you want to activate HUD canvas
                ShowArrow(new Vector3(802, -173, 0));
                break;

            case TutorialSection2Steps.ShowArrowToPlot: //Arrow points to Plot Shop Item, wait for the player to ClickShopItem
                controller.HideAllExceptBoostButton();
                ShowArrow(new Vector3(15, 200, 0));
                break;

            case TutorialSection2Steps.PlaceBuilding2: //Building Overlay mode, user place the building
                controller.HideAllExceptBoostButton();
                break;

            case TutorialSection2Steps.ShowArrowToBuilding: //Wait for Boost Structure
                controller.HideAllExceptBoostButton(); //Close All HUD except the boost button
                break;

            case TutorialSection2Steps.NPCDialogue4: //NPC will order the player to boost the plot in progress building
                StartDialogue2();
                break;

            case TutorialSection2Steps.NPCDialogue5: //NPC will order player to plant Beet
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

            case TutorialSection2Steps.WaitForLevel3: //Player will play the game to reach level 3
                Debug.Log("End of Plot Tutorial Reached");
                dialogueBox.SetActive(false);
                controller.ShowAll();
                break;

            case TutorialSection2Steps.NPCDialogue8:
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
        Debug.Log("Ending dialogue. Current steps - Section 1: " + section1Step + ", Section 2: " + section2Step);
        dialogueBox.SetActive(false);

        // Handle Section 1 Steps (Only if not already completed)
        if (!completedSection1Steps.Contains(section1Step))
        {
            switch (section1Step)
            {
                case TutorialSection1Steps.StartDialogue:
                    section1Step = TutorialSection1Steps.ShowArrowToShop;
                    HandleTutorialSteps();
                    completedSection1Steps.Add(TutorialSection1Steps.StartDialogue);
                    break;

                case TutorialSection1Steps.NPCDialogue2:
                    section1Step = TutorialSection1Steps.ShowArrowToBuilding;
                    HandleTutorialSteps();
                    completedSection1Steps.Add(TutorialSection1Steps.NPCDialogue2);
                    break;

                //case TutorialSection1Steps.NPCDialogue5:
                //    section1Step = TutorialSection1Steps.Complete;
                //    HandleTutorialSteps();
                //    completedSection1Steps.Add(TutorialSection1Steps.NPCDialogue5);
                //    break;
            }
        }

        // Handle Section 2 Steps (Only if not already completed)
        if (!completedSection2Steps.Contains(section2Step))
        {
            switch (section2Step)
            {
                case TutorialSection2Steps.NPCDialogue3:
                    Debug.Log("Tutorial Plot Scene");
                    section2Step = TutorialSection2Steps.ShowArrowToShop;
                    HandleTutorialSteps2();
                    completedSection2Steps.Add(TutorialSection2Steps.NPCDialogue3);
                    break;

                case TutorialSection2Steps.NPCDialogue4:
                    section2Step = TutorialSection2Steps.ShowArrowToBuilding;
                    HandleTutorialSteps2();
                    completedSection2Steps.Add(TutorialSection2Steps.NPCDialogue4);
                    break;

                case TutorialSection2Steps.NPCDialogue5:
                    section2Step = TutorialSection2Steps.WaitForPlant;
                    HandleTutorialSteps2();
                    completedSection2Steps.Add(TutorialSection2Steps.NPCDialogue5);
                    break;

                case TutorialSection2Steps.NPCDialogue6:
                    section2Step = TutorialSection2Steps.WaitForCropBoost;
                    HandleTutorialSteps2();
                    completedSection2Steps.Add(TutorialSection2Steps.NPCDialogue6);
                    break;

                case TutorialSection2Steps.NPCDialogue7:
                    section2Step = TutorialSection2Steps.WaitForLevel3;
                    HandleTutorialSteps2();
                    completedSection2Steps.Add(TutorialSection2Steps.NPCDialogue7);
                    break;

                case TutorialSection2Steps.NPCDialogue8:
                    section2Step = TutorialSection2Steps.ShowArrowToShop2;
                    HandleTutorialSteps2();
                    completedSection2Steps.Add(TutorialSection2Steps.NPCDialogue7);
                    break;
                //case TutorialSection2Steps.ShowArrowToBarn:
                //    break;
            }
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
        if (isShopTutorialComplete)
        {
            return;
        }
        else if (section1Step == TutorialSection1Steps.ShowArrowToShop)
        {
            section1Step = TutorialSection1Steps.ShowArrowToItem;
            HandleTutorialSteps();
        }
        else if (section2Step == TutorialSection2Steps.ShowArrowToShop)
        {
            section2Step = TutorialSection2Steps.ShowArrowToPlot;
            HandleTutorialSteps2();
            isShopTutorialComplete = true;
        }
        //else if (section2Step == TutorialSection2Steps.ShowArrowToShop2)
        //{
        //    section2Step = TutorialSection2Steps.ShowArrowToBarn;
        //    HandleTutorialSteps2();
        //    isShopTutorialComplete = true;
        //}
        Debug.Log("Shop Button Clicked");
    }

    public void OnShopItemClicked()
    {
        Debug.Log("ShopItem Clicked");

        arrow.gameObject.SetActive(false);

        if (section1Step == TutorialSection1Steps.ShowArrowToItem)
        {

            section1Step = TutorialSection1Steps.PlaceBuilding;
            HandleTutorialSteps();
        }
        else if (section2Step == TutorialSection2Steps.ShowArrowToPlot)
        {

            section2Step = TutorialSection2Steps.PlaceBuilding2;
            HandleTutorialSteps2();
            isPlotClickComplete = true;
        }
        else
        {
            if (isPlotClickComplete)
            {
                //Debug.Log("tutorial done");
                return;
            }
        }

    }

    public void OnCropItemClicked()
    {

        if (isCropClickComplete)
        {
            //Debug.Log("tutorial done");
            return;
        }
        controller.HideAllExceptBoostButton();
        section2Step = TutorialSection2Steps.NPCDialogue6;
        HandleTutorialSteps2();
        isCropClickComplete = true;
    }

    public void OnBuildingClicked()
    {
        if (isBuildingClicked)
        {
            return;
        }

        arrow.gameObject.SetActive(false);
        section1Step = TutorialSection1Steps.ShowArrowToBoost;
        HandleTutorialSteps();

        isBuildingClicked = true;

        Debug.Log("Villager House Clicked");
    }

    public void PlaceBuilding()
    {
        if (section1Step == TutorialSection1Steps.PlaceBuilding)
        {
            Debug.Log("Place Building: Villager House");

            section1Step = TutorialSection1Steps.NPCDialogue2;
            HandleTutorialSteps();
        }
        else if (section2Step == TutorialSection2Steps.PlaceBuilding2)
        {
            Debug.Log("Place Building: Plot");

            section2Step = TutorialSection2Steps.NPCDialogue4;
            HandleTutorialSteps2();

            isPlotBuild = true;
        }
        else
        {
            if (isPlotBuild)
            {
                return;
            }
        }

    }

    public void OnLevelUp()
    {
        if (section2Step == TutorialSection2Steps.WaitForLevel3)
        {
            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue8;
            HandleTutorialSteps2();
        }
        else
        {
            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue3;
            HandleTutorialSteps2();
        }
    }

    public void OnBoostBuilding()
    {
        Debug.Log("Boosted in the Tutorial");

        if (section2Step == TutorialSection2Steps.ShowArrowToBuilding)
        {
            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue5;
            HandleTutorialSteps2();
        }
        else if (section2Step == TutorialSection2Steps.WaitForCropBoost)
        {
            if (isPlotBoosted)
            {
                return;
            }

            controller.HideAll();
            section2Step = TutorialSection2Steps.NPCDialogue7;
            HandleTutorialSteps2();

            isPlotBoosted = true;
        }

    }

    void ToggleButtons(bool state)
    {
        // Find all GameObjects with the "UI_Button" tag
        GameObject[] allButtons = GameObject.FindGameObjectsWithTag("UI_Button");

        foreach (GameObject buttonObj in allButtons)
        {
            // Get the Button component and disable/enable it
            Button button = buttonObj.GetComponent<Button>();
            if (button != null && button != nextButton) // Exclude the Next button
            {
                button.interactable = state;
            }
        }
    }

}
