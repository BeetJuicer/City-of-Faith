using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Dialogue : MonoBehaviour
{
    public enum TutorialStep
    {
        StartDialogue,
        ShowArrowToShop,
        OpenShop,
        NPCDialogue2,
        ShowArrowToItem,
        NPCDialogue3,
        PlaceBuilding,
        NPCDialogue4,
        ShowArrowToBuilding,
        NPCDialogue5,
        Complete,
    }

    public enum TutorialStep2
    {
        NPCDialogue7,
        Complete,
    }

    public TutorialStep currentStep;
    [SerializeField] private Dialogue_SO[] lines;
    public TMP_Text textComponent;
    public Button nextButton;
    public float textSpeed;
    public GameObject dialogueBox; // Reference to the dialogue box GameObject
    public Image arrow; // Reference to the Arrow GameObject (not just the Image)
    public GameObject shopButton;
    public CentralHall centralHall;
    public BuildingOverlay buildingOverlay;

    private int dialogueIndex = 0;
    private int index = 0;
    private bool isShopTutorialComplete = false;
    private bool isItemClickComplete = false;
    private bool isItemBuild = false;
    private bool isItemBuilding = false;

    //void Start()
    //{
    //    textComponent.text = string.Empty;
    //    nextButton.onClick.AddListener(OnNextButtonClick);
    //    arrow.gameObject.SetActive(false);
    //    currentStep = TutorialStep.StartDialogue; // Initialize to StartDialogue step
    //    HandleTutorialSteps();
    //}

    private void TriggerTutorial(int level)
    {
        if (level == 1)
        {
            textComponent.text = string.Empty;
            nextButton.onClick.AddListener(OnNextButtonClick);
            arrow.gameObject.SetActive(false);
            currentStep = TutorialStep.StartDialogue; // Initialize to StartDialogue step
            HandleTutorialSteps();
        }
        //else if (level == 2)
        //{
        //    textComponent.text = string.Empty;
        //    nextButton.onClick.AddListener(OnNextButtonClick);
        //    arrow.gameObject.SetActive(false);
        //    currentStep = TutorialStep.NPCDialogue7; // Initialize to start the Plot Tutorial (DPlot_Seven)
        //    HandleTutorialSteps();
        //}
    }


    void HandleTutorialSteps()
    {
        Debug.Log("Current Step: " + currentStep); // Log the current step
        switch (currentStep)
        {
            case TutorialStep.StartDialogue:
                ToggleButtons(false);
                StartDialogue();
                break;
            case TutorialStep.ShowArrowToShop:
                shopButton.SetActive(true);
                ToggleButtons(true);
                ShowArrow(new Vector3(285f, -111f, 0));
                break;

            case TutorialStep.NPCDialogue2:
                ToggleButtons(false);
                StartDialogue2();
                break;

            case TutorialStep.ShowArrowToItem:
                ShowArrow(new Vector3(-75f, 57f, 1f)); // Offset arrow above shop item
                break;

            case TutorialStep.NPCDialogue3:
                StartDialogue2();
                break;

            case TutorialStep.PlaceBuilding:
                //Wait for the player to place the structure
                break;

            case TutorialStep.NPCDialogue4:
                StartDialogue2();
                break;

            case TutorialStep.ShowArrowToBuilding:

                if (buildingOverlay != null)
                {
                    Debug.Log("Showing arroooooooow!");
                    buildingOverlay.OnStructureBuilt += ShowArrow;
                }
                break;

            case TutorialStep.NPCDialogue5:
                StartDialogue2();
                break;

            //case TutorialStep.NPCDialogue7:
            //    StartDialogue2();
            //    break;

            case TutorialStep.Complete:
                Debug.Log("Tutorial Complete!!!");
                ToggleButtons(true);
                break;

            default:
                Debug.LogWarning("Unhandled tutorial step: " + currentStep);
                break;
        }
    }
    void StartDialogue()
    {
        Debug.Log("Starting dialogue at index 0");
        StartCoroutine(TypeLine());
    }
    IEnumerator TypeLine()
    {
        Debug.Log("Typing line at index " + index);

        // Make the next button interactable immediately
        nextButton.interactable = true;

        // Initialize a boolean to check if the typing effect should skip
        bool isTypingSkipped = false;

        // Event listener for the next button to skip typing
        nextButton.onClick.AddListener(() => isTypingSkipped = true);

        // Iterate through each character
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
        textComponent.text = string.Empty;
        dialogueBox.SetActive(true);
        index = 0;
        dialogueIndex++;
        StartCoroutine(TypeLine());
    }

    void EndDialogue()
    {
        Debug.Log("Ending dialogue. Current step: " + currentStep);
        dialogueBox.SetActive(false);

        if (currentStep == TutorialStep.StartDialogue)
        {
            currentStep = TutorialStep.ShowArrowToShop;
        }
        else if (currentStep == TutorialStep.NPCDialogue2)
        {
            currentStep = TutorialStep.ShowArrowToItem;
        }
        else if (currentStep == TutorialStep.NPCDialogue3)
        {
            currentStep = TutorialStep.PlaceBuilding;
        }
        else if (currentStep == TutorialStep.NPCDialogue4)
        {
            currentStep = TutorialStep.ShowArrowToBuilding;
        }
        else if (currentStep == TutorialStep.NPCDialogue5)
        {
            currentStep = TutorialStep.Complete;
        }

        HandleTutorialSteps();
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
        if (isShopTutorialComplete)
        {
            return;
        }

        arrow.gameObject.SetActive(false);
        currentStep = TutorialStep.NPCDialogue2;
        HandleTutorialSteps();

        isShopTutorialComplete = true;
    }

    public void OnShopItemClicked()
    {
        Debug.Log("On Shop item clicked is working");
        if (isItemClickComplete)
        {
            Debug.Log("tutorial done");
            return;
        }
        Debug.Log("arrow false item click");
        arrow.gameObject.SetActive(false);
        currentStep = TutorialStep.NPCDialogue3;
        HandleTutorialSteps();

        isItemClickComplete = true;
    }

    public void PlaceBuilding()
    {
        if (isItemBuild)
        {
            return;
        }
        currentStep = TutorialStep.NPCDialogue4;
        HandleTutorialSteps();

        isItemClickComplete = true;
    }

    public void boostBuilding()
    {
        if (isItemBuilding)
        {
            return;
        }
        Debug.Log("Building Boosted");
        currentStep = TutorialStep.NPCDialogue5;
        HandleTutorialSteps();

        isItemBuilding = true;
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

    private void OnEnable()
    {
        centralHall.OnPlayerLevelUp += TriggerTutorial;
    }

    private void OnDisable()
    {
        centralHall.OnPlayerLevelUp -= TriggerTutorial;
    }

    private void OnDestroy()
    {
        buildingOverlay.OnStructureBuilt -= ShowArrow;
    }



}
