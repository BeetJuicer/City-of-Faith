using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading;

public class FishGameController : MonoBehaviour
{
    public static FishGameController instance;
    // Prefabs for fish and trash objects to spawn
    public GameObject[] fishPrefabs;
    public GameObject[] trashPrefabs;

    // Game settings
    public float jumpForce = 5f;            // Force for fish jumping
    public float trashJumpForce = 5f;       // Force for trash jumping
    public float gameTime = 30f;            // Total game time in seconds
    public int score = 0;                   // Current score
    private int goldReward = 0;
    private int expReward = 0;
    public TextMeshProUGUI scoreText;       // UI text to display score
    public TextMeshProUGUI timeText;        // UI text to display remaining time
    public GameObject gameOverPanel;       // UI panel to display at game over
    public TextMeshProUGUI gameOverText;    // UI text for game over message
    public TextMeshProUGUI finalScoreText;  // UI text for final score display
    public TextMeshProUGUI goldText;        // UI text for gold reward
    public TextMeshProUGUI expText;         // UI text for experience points
    public FishAudioSource fishAudioSource; // Audio source for game sounds
    public Button exitButton;

    // Private variables
    private float timeLeft;                 // Remaining game time
    private bool isGameOver = false;        // Flag to check if the game is over
    private Camera mainCamera;              // Camera reference
    private readonly Vector2[] spawnPositions = new Vector2[3]; // Spawn positions on screen
    private int lastSpawnIndex = -1;        // Prevent consecutive spawns at the same position
    private ResourceManager resourceManager;   // Reference to ResourceManager
    private CentralHall centralHall;           // Reference to CentralHall

    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private Camera MainCamera3d;
    [SerializeField] private GameObject fishingMinigamePrefab;
    [SerializeField] private GameObject audioManager;

    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Get references to ResourceManager and CentralHall
        resourceManager = ResourceManager.Instance;
        centralHall = FindObjectOfType<CentralHall>();
        if (resourceManager == null || centralHall == null)
        {
            Debug.LogError("ResourceManager or CentralHall not found!");
        }
    }

    void OnEnable()
    {
        if (isGameOver)
        {
            ResetGameState();
        }

        // Initialize variables and set up game
        mainCamera = Camera.main;
        timeLeft = gameTime;

        // Start background music and game start sound
        fishAudioSource.PlayBackgroundMusic();
        fishAudioSource.PlayStartSFX();

        // Calculate spawn positions based on camera view
        CalculateSpawnPositions();

        // Start spawning fish and trash continuously
        StartCoroutine(SpawnObjectsContinuously());

        // Update score display
        UpdateScoreText();

    }

    private void ResetGameState()
    {
        isGameOver = false;               // Reset the game over flag
        timeLeft = gameTime;              // Reset the timer
        score = 0;                        // Reset the score
        goldReward = 0;                   // Reset gold reward
        expReward = 0;                    // Reset experience points

        // Reset UI elements
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false); // Hide the game over panel
        UpdateScoreText();                  // Reset score text
        UpdateTimerText();                  // Reset timer text

        // Re-enable gameplay-related objects if necessary
        fishingMinigamePrefab.SetActive(true);
    }


    void Update()
    {
        // Only update if the game is not over
        if (!isGameOver)
        {
            timeLeft -= Time.deltaTime;  // Decrease the time left
            UpdateTimerText();           // Update the timer display

            // Check if time is up
            if (timeLeft <= 0)
                HandleGameOver();  // Call HandleGameOver instead of GameOver
        }
    }

    private void CalculateSpawnPositions()
    {
        // Calculate the spawn positions based on the camera view
        float screenLeft = mainCamera.ViewportToWorldPoint(new Vector3(0.2f, 0, 0)).x;
        float screenMiddle = mainCamera.ViewportToWorldPoint(new Vector3(0.5f, 0, 0)).x;
        float screenRight = mainCamera.ViewportToWorldPoint(new Vector3(0.8f, 0, 0)).x;
        float screenBottom = mainCamera.ViewportToWorldPoint(new Vector3(0, 0.1f, 0)).y;

        // Assign calculated positions to spawnPositions array
        spawnPositions[0] = new Vector2(screenLeft, screenBottom);
        spawnPositions[1] = new Vector2(screenMiddle, screenBottom);
        spawnPositions[2] = new Vector2(screenRight, screenBottom);
    }

    // Coroutine to spawn fish and trash continuously
    IEnumerator SpawnObjectsContinuously()
    {
        while (!isGameOver)
        {
            // Randomly select a spawn position, avoiding the last position
            int spawnIndex;
            do
            {
                spawnIndex = Random.Range(0, spawnPositions.Length);
            } while (spawnIndex == lastSpawnIndex);

            lastSpawnIndex = spawnIndex;  // Set last spawn position
            SpawnObjectAtPosition(spawnPositions[spawnIndex]); // Spawn object

            // Wait for a short interval before spawning the next object
            yield return new WaitForSeconds(0.8f);
        }
    }

    // Spawn either a fish or trash at the given position
    private void SpawnObjectAtPosition(Vector2 spawnPosition)
    {
        GameObject entity;

        // 80% chance to spawn a fish, 20% chance to spawn trash
        if (Random.value < 0.8f)
        {
            int randomFishIndex = Random.Range(0, fishPrefabs.Length);
            entity = Instantiate(fishPrefabs[randomFishIndex], spawnPosition, Quaternion.identity);
            entity.GetComponent<Fish>().SetAudioSource(fishAudioSource);
        }
        else
        {
            int randomTrashIndex = Random.Range(0, trashPrefabs.Length);
            entity = Instantiate(trashPrefabs[randomTrashIndex], spawnPosition, Quaternion.identity);
            entity.GetComponent<Trash>().SetAudioSource(fishAudioSource);
        }

        // Add force to make the object jump
        Rigidbody2D rb = entity.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float adjustedJumpForce = entity.tag == "Fish" ? jumpForce : trashJumpForce;
            rb.AddForce(Vector2.up * adjustedJumpForce, ForceMode2D.Impulse);
        }
    }

    // Add points to the score and update the UI
    public void AddScore(int points)
    {
        score += points;
        score = Mathf.Max(score, 0); // Prevent negative scores
        UpdateScoreText();
    }

    // Update the score UI text
    void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }

    // Update the timer UI text
    void UpdateTimerText()
    {
        if (timeText != null)
            timeText.text = "Time Left: " + Mathf.RoundToInt(timeLeft);
    }

    // Handle game over logic
    private void HandleGameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        gameOverText.text = "Game Over";
        finalScoreText.text = "Final Score: " + score;

        // Calculate rewards based on the score
        CalculateRewards();

        // Update UI with rewards
        goldText.text = "Gold: " + goldReward;   // Use goldReward
        expText.text = "EXP: " + expReward;      // Use expReward

        // Update player resources and central hall data
        if (resourceManager != null)
        {
            Debug.Log("Adding Gold");
            resourceManager.AdjustPlayerCurrency(Currency.Gold, goldReward);
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

        //// Show exit button and assign the event listener
        //exitButton.gameObject.SetActive(true);
        //exitButton.onClick.RemoveAllListeners();
        //exitButton.onClick.AddListener(ExitToMainScene);

        // Pause the game
        Time.timeScale = 0f;
        fishAudioSource.StopBackgroundMusic();
        fishAudioSource.PlayGameOverSFX(); 
    }

    private void CalculateRewards()
    {
        int lowAverageScore = 55;
        int averageScore = 115;
        int highAverageScore = 230;

        int baseGold = 5;
        int baseExp = 5;

        // Define reward variables
        int goldReward = 0;
        int expReward = 0;

        if (score < lowAverageScore)
        {
            goldReward = Mathf.FloorToInt(baseGold * 0.5f);  // 50% of base gold reward
            expReward = Mathf.FloorToInt(baseExp * 0.5f);    // 50% of base exp reward
        }
        else if (score >= lowAverageScore && score < averageScore)
        {
            goldReward = Mathf.FloorToInt(baseGold * 1.0f);  // 100% of base gold reward
            expReward = Mathf.FloorToInt(baseExp * 1.0f);    // 100% of base exp reward
        }
        else if (score >= averageScore && score < highAverageScore)
        {
            goldReward = Mathf.FloorToInt(baseGold * 1.5f);  // 150% of base gold reward
            expReward = Mathf.FloorToInt(baseExp * 1.5f);    // 150% of base exp reward
        }
        else
        {
            goldReward = Mathf.FloorToInt(baseGold * 2.0f);  // 200% of base gold reward
            expReward = Mathf.FloorToInt(baseExp * 2.0f);    // 200% of base exp reward
        }

        if (score == 0)
        {
            goldReward = 0;  // No reward for score 0
            expReward = 0;   // No reward for score 0
        }

        // Assign calculated rewards to the instance variables
        this.goldReward = goldReward;
        this.expReward = expReward;
    }

    public void ExitToMainScene()
    {
        Time.timeScale = 1f;
        fishingMinigamePrefab.SetActive(false);
        HUDCanvas.SetActive(true);
        MainCamera3d.gameObject.SetActive(true);
        audioManager.gameObject.SetActive(true);

        Debug.Log("Exiting mini-game and saving rewards.");
    }

}