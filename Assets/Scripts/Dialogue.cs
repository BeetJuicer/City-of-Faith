using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Dialogue : MonoBehaviour
{

    public enum TutorialSection1Steps
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

    public enum TutorialSection2Steps
    {
        NPCDialogue7,
        Complete,
    }

    public TutorialSection1Steps section1Step;
    public TutorialSection2Steps section2Step;
    [SerializeField] private Dialogue_SO[] lines;
    public TMP_Text textComponent;
    public Button nextButton;
    public float textSpeed;
    public GameObject dialogueBox; // Reference to the dialogue box GameObject
    public Image arrow; // Reference to the Arrow GameObject (not just the Image)
    public GameObject shopButton;
    public CentralHall centralHall;
    public BuildingOverlay buildingOverlay;
    [SerializeField] private Camera cam;

    private int dialogueIndex = 0;
    private int index = 0;
    private bool isShopTutorialComplete = false;
    private bool isItemClickComplete = false;
    private bool isItemBuild = false;
    private bool isItemBuilding = false;

    private Structure structureObservee;

    void Start()
    {
        textComponent.text = string.Empty;
        nextButton.onClick.AddListener(OnNextButtonClick);
        arrow.gameObject.SetActive(false);
        section1Step = TutorialSection1Steps.StartDialogue; // Initialize to StartDialogue step
        HandleTutorialSteps();
    }

    private void TriggerTutorial(int level)
    {
        if (level == 1)
        {
            textComponent.text = string.Empty;
            nextButton.onClick.AddListener(OnNextButtonClick);
            arrow.gameObject.SetActive(false);
            section2Step = TutorialSection2Steps.NPCDialogue7; // Initialize to StartDialogue step
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
        Debug.Log("Current Step: " + section1Step); // Log the current step
        switch (section1Step)
        {
            case TutorialSection1Steps.StartDialogue:
                ToggleButtons(false);
                StartDialogue();
                break;
            case TutorialSection1Steps.ShowArrowToShop:
                shopButton.SetActive(true);
                ToggleButtons(true);
                ShowArrow(new Vector3(285f, -111f, 0));
                break;

            case TutorialSection1Steps.NPCDialogue2:
                ToggleButtons(false);
                StartDialogue2();
                break;

            case TutorialSection1Steps.ShowArrowToItem:
                ShowArrow(new Vector3(-75f, 57f, 1f)); // Offset arrow above shop item
                break;

            case TutorialSection1Steps.NPCDialogue3:
                StartDialogue2();
                break;

            case TutorialSection1Steps.PlaceBuilding:
                buildingOverlay.OnStructureBuilt += ShowArrowViewpoint;

                //Wait for the player to place the structure
                break;

            case TutorialSection1Steps.NPCDialogue4:
                StartDialogue2();
                break;

            case TutorialSection1Steps.ShowArrowToBuilding:

                if (buildingOverlay != null)
                {
                    Debug.Log("listening to buildingoverlay!");
                }
                break;

            case TutorialSection1Steps.NPCDialogue5:
                StartDialogue2();
                break;

            /*
             case naclickna, boost naman
                structure.OnStructureInProgressClicked += boostdialoguecucgy   
             
             */

            //case TutorialStep.NPCDialogue7:
            //    StartDialogue2();
            //    break;

            case TutorialSection1Steps.Complete:
                Debug.Log("Tutorial Complete!!!");
                ToggleButtons(true);
                break;

            default:
                Debug.LogWarning("Unhandled tutorial step: " + section1Step);
                break;
        }
    }

    void HandleTutorialSteps2()
    {
        switch (section2Step)
        {
            case TutorialSection2Steps.NPCDialogue7:
                StartDialogue2();
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
        textComponent.text = string.Empty;
        dialogueBox.SetActive(true);
        index = 0;
        dialogueIndex++;
        StartCoroutine(TypeLine());
    }

    void EndDialogue()
    {
        Debug.Log("Ending dialogue. Current step: " + section1Step);
        dialogueBox.SetActive(false);

        if (section1Step == TutorialSection1Steps.StartDialogue)
        {
            section1Step = TutorialSection1Steps.ShowArrowToShop;
        }
        else if (section1Step == TutorialSection1Steps.NPCDialogue2)
        {
            section1Step = TutorialSection1Steps.ShowArrowToItem;
        }
        else if (section1Step == TutorialSection1Steps.NPCDialogue3)
        {
            section1Step = TutorialSection1Steps.PlaceBuilding;
        }
        else if (section1Step == TutorialSection1Steps.NPCDialogue4)
        {
            section1Step = TutorialSection1Steps.ShowArrowToBuilding;
        }
        else if (section1Step == TutorialSection1Steps.NPCDialogue5)
        {
            section1Step = TutorialSection1Steps.Complete;
        }

        HandleTutorialSteps();
    }

    void ShowArrow(Vector3 newPosition)
    {
        Vector2 ScreenPosition = cam.WorldToScreenPoint(newPosition);
        RectTransform rectTransform = arrow.rectTransform; // Get RectTransform of the Image
        rectTransform.anchoredPosition = new Vector2(newPosition.x, newPosition.y); // Update position
        arrow.gameObject.SetActive(true);
        Debug.Log($"Arrow Position Updated: {newPosition}");
    }
    void ShowArrowViewpoint(Vector3 newPosition, Structure structure)
    {
        structureObservee = structure;
        Vector2 viewportPoint = cam.WorldToViewportPoint(newPosition);
        RectTransform rectTransform = arrow.rectTransform; // Get RectTransform of the Image
        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;
        rectTransform.anchoredPosition = viewportPoint;//new Vector2(newPosition.x, newPosition.y); // Update position
        arrow.gameObject.SetActive(true);
        print("Viewportpoint is: " + viewportPoint);
    }

    public void OnShopButtonClicked()
    {
        if (isShopTutorialComplete)
        {
            return;
        }

        arrow.gameObject.SetActive(false);
        section1Step = TutorialSection1Steps.NPCDialogue2;
        HandleTutorialSteps();

        isShopTutorialComplete = true;
    }

    public void OnShopItemClicked()
    {
        //Debug.Log("On Shop item clicked is working");
        if (isItemClickComplete)
        {
            //Debug.Log("tutorial done");
            return;
        }
        //Debug.Log("arrow false item click");
        arrow.gameObject.SetActive(false);
        section1Step = TutorialSection1Steps.NPCDialogue3;
        HandleTutorialSteps();

        isItemClickComplete = true;
    }

    public void PlaceBuilding()
    {
        if (isItemBuild)
        {
            return;
        }
        section1Step = TutorialSection1Steps.NPCDialogue4;
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
        section1Step = TutorialSection1Steps.NPCDialogue5;
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
        buildingOverlay.OnStructureBuilt -= ShowArrowViewpoint;
    }



}
