using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManagerSheep : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text trustPercentageText;

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TMP_Text quoteText;
    public TMP_Text goldText;
    public TMP_Text expText;
    public Button exitGameButton;

    [Header("Game Settings")]
    public int baseGoldPerSegment = 100;
    public int baseExpPerSegment = 100;
    private int totalGoldEarned = 0;
    private int totalExpEarned = 0;
    private CameraMovement cameraMovement;
    private LoopingBackground loopingBackground;
    private Coroutine currentMessageCoroutine;

    public WolfSpawner wolfSpawner;
    public TrustMeter trustMeter;
    public GameObject sheepAnimator;

    private bool gameEnded = false;

    void Start()
    {
        if (exitGameButton != null)
        {
            exitGameButton.onClick.AddListener(ExitGame);
        }
        UpdateTrustText();

        cameraMovement = FindObjectOfType<CameraMovement>();
        loopingBackground = FindObjectOfType<LoopingBackground>();

        if (cameraMovement == null)
            Debug.LogError("CameraMovement not found in scene!");

        if (loopingBackground == null)
            Debug.LogError("LoopingBackground not found in scene!");
    }
    public void OnSheepEatsFlower()
    {
        if (gameEnded) return;

        Debug.Log("Sheep ate a flower! Trust increased!");

        trustMeter.IncreaseTrust(20f);
        UpdateTrustText();

        if (wolfSpawner != null && !gameEnded)
        {
            wolfSpawner.StartSpawning();
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
        if (cameraMovement != null) cameraMovement.enabled = state;
        if (loopingBackground != null) loopingBackground.enabled = state;
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
    private void ExitGame()
    {
        Debug.Log("Game Exiting...");
    }
}
