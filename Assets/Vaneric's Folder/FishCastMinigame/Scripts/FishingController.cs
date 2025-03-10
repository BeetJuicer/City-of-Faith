using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FishingController : MonoBehaviour
{
    public Image messageBackground;
    public TMP_Text messageText;
    public TMP_Text timerText;
    public TMP_Text scoreText;
    public Button castButton;
    public Button exitButton;
    public Animator fishermanAnimator;
    public GameObject joystick;
    public JoystickCapture joystickCapture;
    public FishSpawner fishSpawner;

    public GameObject endGamePanel;
    public TMP_Text quoteText;
    public TMP_Text goldText;
    public TMP_Text expText;
    public Button exitGameButton;

    [SerializeField] private GameObject AudioManager;

    private int score = 0;
    private float timer = 60f;
    private bool canCast = false;
    private bool gameEnded = false;
    private bool hasCast = false;

    private Coroutine currentMessageCoroutine = null;

    void Start()
    {
        messageBackground.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
        fishermanAnimator.SetFloat("Blend", 0f);
        StartCoroutine(StartGameSequence());
        castButton.onClick.AddListener(OnCastButtonPressed);
        exitGameButton.onClick.AddListener(ExitGame);
        castButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        endGamePanel.SetActive(false);
        joystick.SetActive(false);
        joystickCapture.HideCaptureBox();
        UpdateScoreText();
        AudioSourceFish.Instance.PlayBackgroundMusic();
    }

    void Update()
    {
        if (!gameEnded)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();

            if (timer <= 0)
            {
                EndGame();
            }
        }
    }

    IEnumerator StartGameSequence()
    {
        yield return ShowMessage("When God speaks, trust his timing for abundance", 3f);

        yield return new WaitForSeconds(Random.Range(3, 5));

        yield return ShowMessage("Go fishing now!", 2f);
        canCast = true;
        castButton.gameObject.SetActive(true);
    }

    public void OnCastButtonPressed()
    {
        if (!canCast || hasCast) return;

        canCast = false;
        hasCast = true;
        castButton.gameObject.SetActive(false);

        AudioSourceFish.Instance.PlayTapSound(); // Play tap sound
        StartCoroutine(FishingSequence());
    }

    IEnumerator FishingSequence()
    {
        yield return ShowMessage("You cast your net in faith. Trust in the Lord’s timing!", 2f);

        // Set animation to casting state
        fishermanAnimator.SetFloat("Blend", 0.5f);
        yield return new WaitForSeconds(1.5f);

        // Stay in waiting animation (prevents looping)
        fishermanAnimator.SetFloat("Blend", 0.8f);

        joystick.SetActive(true);
        joystickCapture.ShowCaptureBox();
    }


    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();

        // Correct message based on fish points
        if (points == 10)  // Big Fish
        {
            StartCoroutine(ShowMessage("Your faith has brought you abundance!", 2f));
        }
        else if (points == 5)  // Small Fish
        {
            StartCoroutine(ShowMessage("You obeyed Jesus and received a great blessing!", 2f));
        }

        // Play fish catch sound when score increases
        AudioSourceFish.Instance.PlayFishCatchSound();
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    void EndGame()
    {
        gameEnded = true;
        messageText.text = $"Fishing Over! You scored {score} points.";

        // Hide UI elements
        joystick.SetActive(false);
        joystickCapture.HideCaptureBox();

        // Clear all fish to reduce lag
        fishSpawner.ClearAllFish();

        // Calculate Gold & EXP Rewards
        int gold, exp;
        string quote = "Trust in the Lord’s abundance.";

        if (score >= 200)
        {
            gold = 500;
            exp = 500;
            quote = "With faith, the net overflows.";
        }
        else if (score >= 100)
        {
            gold = 300;
            exp = 300;
            quote = "Those who obey receive in full.";
        }
        else
        {
            gold = 150;
            exp = 150;
            quote = "Small faith brings small blessings.";
        }

        // Play Reward Sound
        AudioSourceFish.Instance.PlayRewardSound();

        // Show EndGame Panel
        endGamePanel.SetActive(true);
        quoteText.text = quote;
        goldText.text = gold.ToString();
        expText.text = exp.ToString();
        exitButton.gameObject.SetActive(true);
    }

    void ExitGame()
    {
        Debug.Log("Exiting game...");
    }

    IEnumerator ShowMessage(string message, float duration)
    {
        if (currentMessageCoroutine != null)
            StopCoroutine(currentMessageCoroutine);

        currentMessageCoroutine = StartCoroutine(FadeUI(messageBackground, messageText, true, 0.8f)); // Fade in
        messageText.text = message;

        yield return new WaitForSeconds(duration);

        currentMessageCoroutine = StartCoroutine(FadeUI(messageBackground, messageText, false, 0.8f)); // Fade out
    }

    IEnumerator FadeUI(Image bg, TMP_Text text, bool fadeIn, float duration)
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
