using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Xml.Linq;

public class GameControllerSheep : MonoBehaviour
{
    public SheepSpawner sheepSpawner;
    public TextMeshProUGUI sheepCounterText, timerText, waveText, waveNameText, messageText;
    public Button callSheepButton;
    public Image waveBackground, messageBackground;

    public GameObject endGamePanel;
    public TextMeshProUGUI quoteText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI expText;
    public Button exitButton;

    [SerializeField] private GameObject AudioManager;

    private int totalSheep = 99;
    private int caughtSheep = 0;
    private int wave = 1;
    private float gameDuration = 120f;
    private int callSheepTaps = 0;
    private bool lostSheepSpawned = false;
    private bool gameEnded = false;
    private Coroutine currentMessageCoroutine; // Track the active message coroutine

    void Start()
    {
        AudioSourceSheep.Instance.PlayBackgroundMusic();
        // Hide UI elements at start
        HideUIElements();

        endGamePanel.SetActive(false);

        // Ensure SheepSpawner is assigned
        if (!sheepSpawner)
        {
            sheepSpawner = FindObjectOfType<SheepSpawner>();
            if (!sheepSpawner)
            {
                Debug.LogError("SheepSpawner is missing in the scene!");
                return;
            }
        }

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
        if (gameEnded) return; // Prevents multiple triggers
        gameEnded = true;

        Debug.Log("EndGame Triggered!");

        // Stop all coroutines to prevent overlapping behaviors
        StopAllCoroutines();

        // Ensure the End Game Panel is active immediately
        endGamePanel.SetActive(true);
        Time.timeScale = 0;

        Debug.Log("EndGamePanel should now be visible.");

        // Calculate Rewards
        int goldReward = Mathf.Min(wave * 100, 500);
        int expReward = Mathf.Min(wave * 100, 500);

        Debug.Log($"Rewards Calculated -> Gold: {goldReward}, Exp: {expReward}");

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

        // Display rewards
        goldText.text = $"{goldReward}";
        expText.text = $"{expReward}";

        // Display the panel with a delay for polish
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

        currentMessageCoroutine = null; // Reset after completion
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
}
