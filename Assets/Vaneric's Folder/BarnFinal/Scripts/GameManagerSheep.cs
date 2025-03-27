using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManagerSheep : MonoBehaviour
{
    public static GameManagerSheep Instance { get; private set; }

    [Header("UI Elements")]
    public TMP_Text trustPercentageText;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TMP_Text quoteText;
    public TMP_Text goldText;
    public TMP_Text expText;
    public Button exitGameButton;

    [Header("Game Settings")]
    private int baseGoldPerSegment = 100;
    private int baseExpPerSegment = 100;
    private int totalGoldEarned = 0;
    private int totalExpEarned = 0;
    private CameraMovement cameraMovement;
    private LoopingBackground loopingBackground;
    private Coroutine currentMessageCoroutine;

    public WolfSpawner wolfSpawner;
    public TrustMeter trustMeter;
    public GameObject sheepAnimator;

    [Header("References")]
    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private Camera MainCamera3d;
    [SerializeField] private GameObject BarnMinigamePrefab;
    [SerializeField] private GameObject AudioManager;

    private ResourceManager resourceManager;
    private CentralHall centralHall;

    private bool gameEnded = false;

    private void Awake()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        centralHall = FindObjectOfType<CentralHall>();
        cameraMovement = FindObjectOfType<CameraMovement>();
        loopingBackground = FindObjectOfType<LoopingBackground>();

        if (resourceManager == null)
            Debug.LogError("ResourceManager is not found in the scene!");

        if (centralHall == null)
            Debug.LogError("CentralHall is not found in the scene!");

        if (cameraMovement == null)
            Debug.LogError("CameraMovement not found in scene!");

        if (loopingBackground == null)
            Debug.LogError("LoopingBackground not found in scene!");
    }

    void OnEnable()
    {
        HUDCanvas.SetActive(false);
        ResetGameState();

        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(ExitGame);
        }
        UpdateTrustText();

        FlowerSpawner flowerSpawner = FindObjectOfType<FlowerSpawner>();
        if (flowerSpawner == null)
        {
            Debug.LogError("FlowerSpawner not found! Make sure it is attached to a GameObject in the scene.");
        }
        else
        {
            flowerSpawner.ResetFlowers();
        }

        AudioSourceBarn.Instance.PlayBackgroundMusic();
    }
    public void OnSheepEatsFlower()
    {
        if (gameEnded) return;

        Debug.Log("Sheep ate a flower! Trust increased!");

        trustMeter.IncreaseTrust(20f);
        UpdateTrustText();

        if (wolfSpawner != null && !gameEnded)
        {
            wolfSpawner.EnableSpawning();
            wolfSpawner.StartSpawning(); // Ensure it starts properly
        }
        else
        {
            Debug.LogError("WolfSpawner not found or game ended!");
        }

        FlowerSpawner flowerSpawner = FindObjectOfType<FlowerSpawner>();
        if (flowerSpawner != null)
        {
            flowerSpawner.ResetFlowers(); // Ensure flowers respawn
        }
        else
        {
            Debug.LogError("FlowerSpawner is missing!");
        }

        if (trustMeter.GetTrustPercentage() >= 100f)
        {
            EndGame();
        }
    }

    private void UpdateTrustText()
    {
        if (trustPercentageText != null)
        {
            trustPercentageText.text = $"{trustMeter.GetTrustPercentage()}%";
        }
    }

    public void EndGame()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("Trust Meter Full! Ending game...");
        SetBackgroundState(false);

        if (wolfSpawner != null)
        {
            wolfSpawner.StopSpawning();
            wolfSpawner.ClearAllWolves();
        }

        totalGoldEarned = (int)(trustMeter.GetTrustPercentage() / 20f) * baseGoldPerSegment;
        totalExpEarned = (int)(trustMeter.GetTrustPercentage() / 20f) * baseExpPerSegment;

        StartCoroutine(ShowEndGameScreen());
    }


    private void SetBackgroundState(bool state)
    {
        if (cameraMovement != null)
        {
            cameraMovement.enabled = state;
            Debug.Log($"Camera Movement set to {state}");
        }
        else
        {
            Debug.LogError("CameraMovement is NULL!");
        }

        if (loopingBackground != null)
        {
            loopingBackground.enabled = state;
            Debug.Log($"Background Looping set to {state}");
        }
        else
        {
            Debug.LogError("LoopingBackground is NULL!");
        }
    }


    private IEnumerator ShowEndGameScreen(bool playSound = true)
    {
        SetBackgroundState(false);
        yield return new WaitForSeconds(1f);

        if (endGamePanel != null) endGamePanel.SetActive(true);

        string trustQuote = "The sheep followed, but trust takes time.";
        float trustPercentage = trustMeter.GetTrustPercentage();

        if (trustPercentage >= 100)
        {
            trustQuote = "The sheep fully trusts you now. The journey is complete.";
        }
        else if (trustPercentage >= 80)
        {
            trustQuote = "Trust grows strong in faith and care.";
        }
        else if (trustPercentage >= 50)
        {
            trustQuote = "Trust is built step by step.";
        }

        if (playSound)
        {
            AudioSourceBarn.Instance.PlayRewardSound();
        }

        if (quoteText != null) quoteText.text = trustQuote;
        if (goldText != null) goldText.text = $"{totalGoldEarned}";
        if (expText != null) expText.text = $"{totalExpEarned}";
        if (exitGameButton != null) exitGameButton.gameObject.SetActive(true);
    }

    public void ResetGameState()
    {
        gameEnded = false;
        trustMeter.ResetTrust();

        totalGoldEarned = 0;
        totalExpEarned = 0;
        UpdateTrustText();

        if (endGamePanel != null) endGamePanel.SetActive(false);
        if (sheepAnimator != null) sheepAnimator.SetActive(true);

        if (wolfSpawner != null)
        {
            wolfSpawner.ResetSpawning(); // Restart wolves properly
        }

        FlowerSpawner flowerSpawner = FindObjectOfType<FlowerSpawner>();
        if (flowerSpawner != null)
        {
            flowerSpawner.ResetFlowers(); // Reset flowers
        }

        SetBackgroundState(true);
    }


    public void OnWolfCollidesWithSheep()
    {
        if (gameEnded) return;
        gameEnded = true;

        Debug.Log("The sheep was caught by a wolf! Game Over!");

        SetBackgroundState(false);
        if (sheepAnimator != null) sheepAnimator.SetActive(false);

        if (wolfSpawner != null)
        {
            wolfSpawner.StopSpawning();
            wolfSpawner.ClearAllWolves();
        }

        totalGoldEarned = (int)(trustMeter.GetTrustPercentage() / 20f) * baseGoldPerSegment;
        totalExpEarned = (int)(trustMeter.GetTrustPercentage() / 20f) * baseExpPerSegment;

        StartCoroutine(ShowEndGameScreen());
    }
    public void ExitGame()
    {
        BarnMinigamePrefab.SetActive(false);
        HUDCanvas.SetActive(true);
        MainCamera3d.gameObject.SetActive(true);
        AudioManager.gameObject.SetActive(true);

        if (resourceManager != null)
        {
            resourceManager.AdjustPlayerCurrency(Currency.Gold, totalGoldEarned);
            Debug.Log("Gold Updated!");
        }
        else
        {
            Debug.LogError("resourceManager is NULL!");
        }

        if (centralHall != null)
        {
            centralHall.AddToCentralExp(totalExpEarned);
            Debug.Log("Exp Updated!");
        }
        else
        {
            Debug.LogError("centralHall is NULL!");
        }

        totalGoldEarned = 0;
        totalExpEarned = 0;
    }

}