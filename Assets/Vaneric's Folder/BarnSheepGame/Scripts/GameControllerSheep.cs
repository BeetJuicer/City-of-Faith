using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;
using System.Resources;

public class GameControllerSheep : MonoBehaviour
{
    public static GameControllerSheep Instance;
    public SheepSpawner sheepSpawner;
    public TextMeshProUGUI sheepCounterText, timerText, waveText, waveNameText, messageText;
    public Button callSheepButton;
    public Image waveBackground, messageBackground;

    public GameObject endGamePanel;
    public TextMeshProUGUI quoteText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI expText;
    public Button exitButton;

    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private Camera MainCamera3d;
    [SerializeField] private GameObject barnMinigamePrefab;
    [SerializeField] private GameObject AudioManager;

    private ResourceManager resourceManager;
    private CentralHall centralHall;
    private int goldReward = 0;
    private int expReward = 0;
    private int totalSheep = 99;
    private int caughtSheep = 0;
    private int wave = 1;
    private float gameDuration = 90f;
    private int callSheepTaps = 0;
    private bool lostSheepSpawned = false;
    private bool gameEnded = false;
    private Coroutine currentMessageCoroutine;
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable()
    {
        HideUIElements();
        HUDCanvas.SetActive(false);
        ResetGameState();
        endGamePanel.SetActive(false);

        if (!sheepSpawner)
        {
            sheepSpawner = FindObjectOfType<SheepSpawner>();
            if (!sheepSpawner)
            {
                Debug.LogError("SheepSpawner is missing in the scene!");
                return;
            }
        }
        AudioSourceSheep.Instance.PlayBackgroundMusic();
        StartCoroutine(StartWaves());
        StartCoroutine(GameTimer());
        callSheepButton.gameObject.SetActive(false);
        UpdateSheepCounter();
        UpdateWaveText();
    }

    void HideUIElements()
    {
        waveBackground.gameObject.SetActive(false);
        waveNameText.gameObject.SetActive(false);
        messageBackground.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
    }

    public void ResetGameState()
    {
        gameEnded = false;
        gameDuration = 90f;
        caughtSheep = 0;
        goldReward = 0;
        expReward = 0;
        wave = 1;
        callSheepTaps = 0;
        lostSheepSpawned = false;

        if (endGamePanel != null)
            endGamePanel.SetActive(false);

        UpdateSheepCounter();
        UpdateWaveText();

        if (barnMinigamePrefab != null)
            barnMinigamePrefab.SetActive(true);

        Debug.Log("Game state has been reset.");
    }

    IEnumerator StartWaves()
    {
        for (wave = 1; wave <= 5; wave++)
        {
            UpdateWaveText();
            int sheepToSpawn = (wave == 5) ? 19 : 20;
            sheepSpawner.SpawnSheep(sheepToSpawn, GetSheepSpeedForWave());

            int targetSheep = Mathf.Min(caughtSheep + sheepToSpawn, totalSheep);
            yield return new WaitUntil(() => caughtSheep >= targetSheep);
        }

        if (caughtSheep == totalSheep)
        {
            StartCoroutine(ShowMessageSequence());
        }
    }

    IEnumerator ShowMessageSequence()
    {
        yield return ShowMessage("Congratulations! You've gathered 99 sheep! But one is missing! Where could it be?", 3);
        yield return ShowMessage("Would you not leave the ninety-nine and go after the one that is lost? - Luke 15:4", 3);
        callSheepButton.gameObject.SetActive(true);
        callSheepButton.onClick.RemoveAllListeners();
        callSheepButton.onClick.AddListener(CallLostSheep);
    }

    public void CatchSheep(GameObject sheep)
    {
        if (gameEnded) return;

        caughtSheep++;
        UpdateSheepCounter();
        AudioSourceSheep.Instance.PlaySheepTapSound();

        Debug.Log($"Caught Sheep: {caughtSheep} / 100");
        if (caughtSheep == 100)
        {
            Debug.Log("Last Sheep Caught! Triggering EndGame...");
            StartCoroutine(ShrinkAndEndGame(sheep));
        }
        else
        {
            StartCoroutine(ShrinkAndDestroySheep(sheep));
        }
    }


    // Shrinks the sheep before destroying it
    IEnumerator ShrinkAndDestroySheep(GameObject sheep)
    {
        yield return ShrinkSheep(sheep);
        Destroy(sheep);
    }

    // Shrink first before end the game
    IEnumerator ShrinkAndEndGame(GameObject sheep)
    {
        Debug.Log("Shrinking last sheep...");

        yield return ShrinkSheep(sheep);
        Destroy(sheep);

        yield return new WaitForSeconds(1f); // Small delay before ending

        Debug.Log("Calling EndGame...");
        EndGame();
    }

    // Shrink animation
    IEnumerator ShrinkSheep(GameObject sheep)
    {
        sheep.GetComponent<Collider2D>().enabled = false; // Prevents multiple taps
        float shrinkDuration = 0.2f;
        float timer = 0;
        Vector3 originalScale = sheep.transform.localScale;

        while (timer < shrinkDuration)
        {
            sheep.transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, timer / shrinkDuration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

    void CallLostSheep()
    {
        if (gameEnded) return;

        callSheepTaps++;
        AudioSourceSheep.Instance.PlayTapSound();

        // Ensure only one message coroutine runs at a time
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }

        switch (callSheepTaps)
        {
            case 1:
                currentMessageCoroutine = StartCoroutine(ShowMessage("This verse reminds us how important every single one of us is to God.", 3));
                break;

            case 2:
                currentMessageCoroutine = StartCoroutine(ShowMessage("The shepherd never gave up until he found his lost sheep. Keep calling!", 3));
                break;

            case 3:
                if (!lostSheepSpawned)
                {
                    lostSheepSpawned = true;
                    callSheepButton.gameObject.SetActive(false);
                    currentMessageCoroutine = StartCoroutine(ShowMessage("Listen carefully... meh... Is that it?", 2));
                    StartCoroutine(ShowLostSheep());
                }
                break;
        }
    }

    IEnumerator ShowLostSheep()
    {
        yield return new WaitForSeconds(1f);
        sheepSpawner.SpawnSheep(1, 20);
        AudioSourceSheep.Instance.PlayLostSheepBaaSound();
    }

    IEnumerator GameTimer()
    {
        while (gameDuration > 0)
        {
            if (gameEnded) yield break;
            if (Time.timeScale == 0) yield return null; // Paused, so wait instead of decreasing time

            gameDuration -= Time.deltaTime;
            gameDuration = Mathf.Max(gameDuration, 0);

            int minutes = Mathf.FloorToInt(gameDuration / 60);
            int seconds = Mathf.FloorToInt(gameDuration % 60);
            timerText.text = $"{minutes}:{seconds:00}";

            yield return null;
        }

        EndGame();
    }


    void UpdateSheepCounter()
    {
        sheepCounterText.text = $"{caughtSheep} / 100";
    }

    void UpdateWaveText()
    {
        waveText.text = $"{wave} / 5";
        UpdateWaveName();
    }

    void EndGame()
    {
        if (gameEnded) return; // prevent unnecessary execution of code when the game is already ended
        gameEnded = true;

        // Stop all coroutines to prevent overlapping behaviors
        StopAllCoroutines();

        // Ensure the End Game Panel is active immediately
        endGamePanel.SetActive(true);
        Time.timeScale = 0;

        // Calculate Rewards limit of 500
        goldReward = Mathf.Min(wave * 100, 500);
        expReward = Mathf.Min(wave * 100, 500);

        // Set End Game Message based on progress
        if (caughtSheep == 100)
        {
            AudioSourceSheep.Instance.PlayRewardSound();
            quoteText.text = "Rejoice! The lost sheep has been found. (Luke 15:6)";
        }
        else if (wave == 1)
        {
            quoteText.text = "You’ve started the journey. Next time, go further!";
        }
        else if (wave == 2)
        {
            quoteText.text = "Great effort! \nThe flock awaits your return.";
        }
        else if (wave == 3)
        {
            quoteText.text = "You’ve come far! Next time, finish the journey.";
        }
        else if (wave == 4)
        {
            quoteText.text = "So close! The lost sheep still waits for you.";
        }

        goldText.text = $"{goldReward} ";
        expText.text = $"{expReward} ";
        StartCoroutine(ShowEndGamePanel());
    }

    IEnumerator ShowEndGamePanel()
    {
        Debug.Log("Showing End Game Panel...");

        endGamePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.5f);

        Debug.Log("End Game Panel Fully Visible!");
    }

    int GetSheepSpeedForWave()
    {
        return wave * 2;
    }

    void UpdateWaveName()
    {
        string[] waveNames = { "First Wave", "Second Wave", "Third Wave", "Fourth Wave", "Final Wave" };
        waveNameText.text = waveNames[wave - 1];
        StartCoroutine(ShowWaveUI());
    }

    IEnumerator ShowWaveUI()
    {
        AudioSourceSheep.Instance.PlayNextWaveSound();
        yield return FadeUI(waveBackground, waveNameText, true, 0.8f);
        yield return new WaitForSeconds(2f);
        yield return FadeUI(waveBackground, waveNameText, false, 0.8f);
    }

    IEnumerator ShowMessage(string message, float duration)
    {
        messageText.text = message;
        yield return FadeUI(messageBackground, messageText, true, 0.8f); // Fade in

        yield return new WaitForSeconds(duration);

        yield return FadeUI(messageBackground, messageText, false, 0.8f); // Fade out

        currentMessageCoroutine = null;
    }


    IEnumerator FadeUI(Image bg, TextMeshProUGUI text, bool fadeIn, float duration)
    {
        bg.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        float startAlpha = fadeIn ? 0 : 130f / 255f;
        float endAlpha = fadeIn ? 130f / 255f : 0;
        float startTextAlpha = fadeIn ? 0 : 1;
        float endTextAlpha = fadeIn ? 1 : 0;

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t / duration);
            float textAlpha = Mathf.Lerp(startTextAlpha, endTextAlpha, t / duration);
            bg.color = new Color(0, 0, 0, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, textAlpha);
            yield return null;
        }

        bg.color = new Color(0, 0, 0, endAlpha);
        text.color = new Color(text.color.r, text.color.g, text.color.b, endTextAlpha);

        if (!fadeIn)
        {
            bg.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }
    }
    public void ExitToMainScene()
    {
        Time.timeScale = 1f;
        barnMinigamePrefab.SetActive(false);
        HUDCanvas.SetActive(true);
        MainCamera3d.gameObject.SetActive(true);
        AudioManager.gameObject.SetActive(true);

        // Update player resources and central hall data
        if (resourceManager != null)
        {
            Debug.Log("Adding Gold");
            ResourceManager.Instance.AdjustPlayerCurrency(Currency.Gold, goldReward);
        }
        else
        {
            Debug.LogError("ResourceManager is null!");
        }

        if (centralHall != null)
        {
            Debug.Log("Adding Exp");
            centralHall.AddToCentralExp(expReward);
        }
        else
        {
            Debug.LogError("CentralHall is null!");
        }

        // Reset rewards after granting
        goldReward = 0;
        expReward = 0;

        Debug.Log("Exiting mini-game and saving rewards.");
    }
}
