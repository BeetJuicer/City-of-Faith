using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using UnityEngine.EventSystems;

public class FishingController : MonoBehaviour
{
    public static FishingController Instance;

    [Header("UI Elements")]
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

    [Header("End Game UI")]
    public GameObject endGamePanel;
    public TMP_Text quoteText;
    public TMP_Text goldText;
    public TMP_Text expText;
    public Button exitGameButton;

    [Header("References")]
    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private Camera MainCamera3d;
    [SerializeField] private GameObject fishingMinigamePrefab;
    [SerializeField] private GameObject AudioManager;

    private ResourceManager resourceManager;
    private CentralHall centralHall;

    private int score = 0;
    private int goldReward = 0;
    private int expReward = 0;
    private float timer = 70f;
    private bool canCast = false;
    private bool gameEnded = false;
    private bool hasCast = false;
    private Vector3 originalPosition;
    private Coroutine currentMessageCoroutine = null;

    private void Awake()
    {
        resourceManager = FindObjectOfType<ResourceManager>();
        centralHall = FindObjectOfType<CentralHall>();

        if (resourceManager == null)
            Debug.LogError("ResourceManager is not found in the scene!");

        if (centralHall == null)
            Debug.LogError("CentralHall is not found in the scene!");
    }

    void OnEnable()
    {
        HUDCanvas.SetActive(false);
        ResetGameState();
        endGamePanel.SetActive(false);
        fishermanAnimator.SetFloat("Blend", 0f);
        StartCoroutine(StartGameSequence());
        castButton.onClick.AddListener(OnCastButtonPressed);
        exitButton.onClick.AddListener(() => StartCoroutine(WaitThenExit()));
        UpdateScoreText();
        AudioSourceFish.Instance.PlayBackgroundMusic();
    }

    void HideUIElements()
    {
        messageBackground.gameObject.SetActive(false);
        messageText.gameObject.SetActive(false);
        castButton.gameObject.SetActive(false);
        exitButton.gameObject.SetActive(false);
        joystick.SetActive(false);
        joystickCapture.HideCaptureBox();
    }

    public void ResetGameState()
    {
        gameEnded = false;
        timer = 70f;
        score = 0;
        goldReward = 0;
        expReward = 0;
        hasCast = false;
        canCast = false;

        if (endGamePanel != null) endGamePanel.SetActive(false);
        if (castButton != null) castButton.gameObject.SetActive(false);
        if (exitButton != null) exitButton.gameObject.SetActive(false);

        if (joystick != null) joystick.SetActive(false);
        if (joystickCapture != null) joystickCapture.HideCaptureBox();

        if (fishermanAnimator != null)
        {
            fishermanAnimator.gameObject.SetActive(true);
            fishermanAnimator.SetFloat("Blend", 0f);
            fishermanAnimator.enabled = false;
            fishermanAnimator.enabled = true;

            if (originalPosition != Vector3.zero)
            {
                fishermanAnimator.transform.position = originalPosition;
            }
        }

        if (fishSpawner != null)
        {
            fishSpawner.ClearAllFish();
            StartCoroutine(SpawnFishWithDelay());
        }

        ResetJoystick();
        UpdateScoreText();
        StartCoroutine(StartGameSequence());
    }

    private IEnumerator SpawnFishWithDelay()
    {
        yield return new WaitForSeconds(0.5f);
        fishSpawner.SpawnInitialFish();
    }

    private void Update()
    {
        if (!gameEnded)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();

            if (timer <= 0)
            {
                timer = 0;
                EndGame();
            }
        }
    }

    IEnumerator StartGameSequence()
    {
        yield return StartCoroutine(ShowMessage("When God speaks, trust his timing for abundance", 2f));
        yield return new WaitForSeconds(Random.Range(2, 3));
        yield return StartCoroutine(ShowMessage("Go fishing now!", 2f));

        canCast = true;
        castButton.gameObject.SetActive(true);
    }

    public void OnCastButtonPressed()
    {
        if (!canCast || hasCast) return;

        canCast = false;
        hasCast = true;
        castButton.gameObject.SetActive(false);
        AudioSourceFish.Instance.PlayTapSound();
        StartCoroutine(FishingSequence());
    }

    IEnumerator FishingSequence()
    {
        yield return ShowMessage("You cast your net in faith. Trust in the Lord’s timing!", 2f);

        fishermanAnimator.SetFloat("Blend", 0.5f);
        yield return new WaitForSeconds(1.5f);
        fishermanAnimator.SetFloat("Blend", 0.8f);
        yield return new WaitForSeconds(2.5f);

        joystick.SetActive(true);
        joystickCapture.ShowCaptureBox();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreText();
        AudioSourceFish.Instance.PlayFishCatchSound();
    }

    void UpdateScoreText()
    {
        scoreText.text = score.ToString();
    }

    private void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.CeilToInt(timer % 60);

        if (minutes > 0)
        {
            timerText.text = string.Format("{0}:{1:00}", minutes, seconds);
        }
        else
        {
            timerText.text = seconds.ToString();
        }
    }

    void EndGame()
    {
        gameEnded = true;
        messageText.text = $"Fishing Over! You scored {score} points.";
        joystick.SetActive(false);
        joystickCapture.HideCaptureBox();
        fishSpawner.ClearAllFish();

        string quote = "Trust in the Lord’s abundance.";

        if (score >= 200)
        {
            goldReward = 500;
            expReward = 500;
            quote = "With faith, the net overflows.";
        }
        else if (score >= 100)
        {
            goldReward = 300;
            expReward = 300;
            quote = "Those who obey receive in full.";
        }
        else
        {
            goldReward = 150;
            expReward = 150;
        }

        AudioSourceFish.Instance.PlayRewardSound();

        endGamePanel.SetActive(true);
        quoteText.text = quote;
        goldText.text = goldReward.ToString();
        expText.text = expReward.ToString();
        exitButton.gameObject.SetActive(true);
    }

    IEnumerator ShowMessage(string message, float duration)
    {
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }

        // Ensure UI elements are active before fading in
        messageBackground.gameObject.SetActive(true);
        messageText.gameObject.SetActive(true);

        messageText.text = message; // Ensure text updates before fading

        currentMessageCoroutine = StartCoroutine(FadeUI(messageBackground, messageText, true, 0.8f));

        yield return new WaitForSeconds(duration);

        yield return StartCoroutine(FadeUI(messageBackground, messageText, false, 0.8f));

        currentMessageCoroutine = null; // Ensure coroutine reference resets
    }

    IEnumerator FadeUI(Image bg, TMP_Text text, bool fadeIn, float duration)
    {
        // Ensure UI elements are active before fading
        bg.gameObject.SetActive(true);
        text.gameObject.SetActive(true);

        float startAlpha = fadeIn ? 0 : 130f / 255f;
        float endAlpha = fadeIn ? 130f / 255f : 0;
        float startTextAlpha = fadeIn ? 0 : 1;
        float endTextAlpha = fadeIn ? 1 : 0;

        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            float textAlpha = Mathf.Lerp(startTextAlpha, endTextAlpha, elapsed / duration);

            bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, textAlpha);

            yield return null;
        }

        bg.color = new Color(bg.color.r, bg.color.g, bg.color.b, endAlpha);
        text.color = new Color(text.color.r, text.color.g, text.color.b, endTextAlpha);

        // Disable UI after fading out
        if (!fadeIn)
        {
            bg.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
        }
    }

    public void ExitGame()
    {
        fishingMinigamePrefab.SetActive(false);
        HUDCanvas.SetActive(true);
        MainCamera3d.gameObject.SetActive(true);
        AudioManager.gameObject.SetActive(true);

        if (resourceManager != null)
        {
            resourceManager.AdjustPlayerCurrency(Currency.Gold, goldReward);
            Debug.Log("Gold Updated!");
        }
        else
        {
            Debug.LogError("resourceManager is NULL!");
        }

        if (centralHall != null)
        {
            centralHall.AddToCentralExp(expReward);
            Debug.Log("Exp Updated!");
        }
        else
        {
            Debug.LogError("centralHall is NULL!");
        }

        goldReward = 0;
        expReward = 0;
    }

    private void ResetJoystick()
    {
        if (joystick != null)
        {
            StartCoroutine(DelayResetJoystick());
        }
    }
    private IEnumerator DelayResetJoystick()
    {
        yield return new WaitForSeconds(0.1f);

        // Ensure joystick registers as released before disabling
        PointerEventData pointerEvent = new PointerEventData(EventSystem.current);
        ExecuteEvents.Execute(joystick.gameObject, pointerEvent, ExecuteEvents.pointerUpHandler);

        // Small delay to fully clear input before disabling
        yield return new WaitForSeconds(0.05f);

        joystick.SetActive(false);
    }

    IEnumerator WaitThenExit()
    {
        yield return new WaitForSeconds(0.5f);
        ExitGame();
    }

}
