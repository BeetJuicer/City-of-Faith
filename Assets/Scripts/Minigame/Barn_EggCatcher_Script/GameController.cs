using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance;  // Singleton instance for easy access across other scripts

    float timer = 30;                      // Game timer, countdown from 30 seconds
    public GameObject[] gm;                // Array of game objects to spawn (e.g., eggs, bombs)
    public TextMeshProUGUI timeText;       // UI Text for displaying remaining time
    public TextMeshProUGUI scoreText;      // UI Text for displaying current score

    // Game Over UI components
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI expText;
    public Button exitButton;

    // Player data
    int score = 0;                         // Player's score
    int gold = 0;                          // Gold earned at the end of the game
    int exp = 0;                           // Experience points earned

    float spawnCooldown = 0.5f;            // Time interval between object spawns
    float currentCooldown = 0.0f;         // Tracks current cooldown time
    bool isGameOver = false;              // Flag to control game-over state

    private ResourceManager resourceManager;  // Reference to ResourceManager
    private CentralHall centralHall;        // Reference to CentralHall

    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private Camera MainCamera3d;
    [SerializeField] private GameObject BarnMinigamePrefab;
    [SerializeField] private GameObject audioManager;
    [SerializeField] private Transform spawnParent;

    void Awake()
    {
        // Singleton pattern: ensures only one instance of GameController exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        // Get references to ResourceManager and CentralHall
        resourceManager = ResourceManager.Instance;
        if (resourceManager == null)
        {
            Debug.LogError("ResourceManager is not initialized! Please ensure ResourceManager is set up correctly.");
        }

        centralHall = FindObjectOfType<CentralHall>();
        if (centralHall == null)
        {
            Debug.LogError("CentralHall is not found in the scene! Please make sure CentralHall is attached to a game object.");
        }
    }

    void OnEnable()
    {
        // Initialize the game state
        ResetGameState();
        HUDCanvas.SetActive(false);

        // Initialize UI elements
        timeText.text = "Time Left: " + Mathf.RoundToInt(timer) + "s";
        scoreText.text = "Score: 0";
        gameOverPanel.SetActive(false);

        // Handle audio setup
        if (AudioSourceMiniGame.instance != null)
        {
            AudioSourceMiniGame.instance.sfxSource.Stop();
            AudioSourceMiniGame.instance.PlayStartSound();
            AudioSourceMiniGame.instance.PlayBackgroundMusic();
        }
        else
        {
            Debug.LogError("AudioSourceMiniGame instance is not found.");
        }
    }
    public void ResetGameState()
    {
        // Reset game variables
        timer = 30f;
        score = 0;
        gold = 0;
        exp = 0;
        isGameOver = false;
        currentCooldown = spawnCooldown;

        // Update UI to reflect the reset state
        timeText.text = "Time Left: " + Mathf.RoundToInt(timer) + "s";
        scoreText.text = "Score: 0";

        // Hide the game-over panel
        gameOverPanel.SetActive(false);

        // Clear all spawned objects (e.g., eggs and bombs)
        ClearSpawnedObjects();

        // Pause gameplay mechanics and start with a delay
        StartCoroutine(DelayedGameStart());
    }

    private void ClearSpawnedObjects()
    {
        foreach (Transform child in spawnParent)
        {
            Destroy(child.gameObject); // Remove all child objects under the spawn parent
        }
    }

    private IEnumerator DelayedGameStart()
    {
        Debug.Log("Game will start in 1 second...");

        // Optional: Show a "Get Ready!" message or countdown UI here
        timeText.text = "Get Ready!";
        yield return new WaitForSeconds(1f); // Delay for 1 second

        // Begin gameplay after delay
        timeText.text = "Time Left: " + Mathf.RoundToInt(timer) + "s";
        currentCooldown = spawnCooldown; // Allow spawning to start
        Debug.Log("Game started!");
    }

    void Update()
    {
        if (timer > 0)
        {
            // Countdown timer
            timer -= Time.deltaTime;
            timeText.text = "Time Left: " + Mathf.RoundToInt(timer) + "s";

            // Handle object spawning at regular intervals
            currentCooldown -= Time.deltaTime;
            if (currentCooldown <= 0)
            {
                SpawnObjects();  // Call function to spawn new objects
                currentCooldown = spawnCooldown;  // Reset cooldown
            }
        }
        else if (!isGameOver) // Trigger game-over logic once
        {
            HandleGameOver();
        }

        foreach (Transform child in spawnParent)
        {
            if (child.position.y < Camera.main.transform.position.y - 5f) // Adjust threshold as needed
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void SpawnObjects()
    {
        // Spawn objects at random positions within the screen bounds
        for (int i = 0; i < 3; i++)
        {
            float x_offset = Random.Range(-4.0f, 4.0f);  // Random x position for spawning
            float initialVelocity = Random.Range(5.0f, 8.0f);  // Random downward velocity

            GameObject obj;
            // Randomly choose whether to spawn an egg (80% chance) or a bomb (20% chance)
            if (Random.Range(0, 100) < 80)
            {
                obj = Instantiate(gm[0], new Vector3(spawnParent.position.x + x_offset, spawnParent.position.y, 0.1f), Quaternion.identity, spawnParent);
            }
            else
            {
                obj = Instantiate(gm[1], new Vector3(spawnParent.position.x + x_offset, spawnParent.position.y, 0.1f), Quaternion.identity, spawnParent);
            }

            // Add downward force to the object
            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.down * initialVelocity, ForceMode2D.Impulse);
            }
        }
    }

    private void HandleGameOver()
    {
        // Flag the game as over
        isGameOver = true;

        // Stop background music and sound effects
        if (AudioSourceMiniGame.instance != null)
        {
            AudioSourceMiniGame.instance.StopBackgroundMusic();
            AudioSourceMiniGame.instance.sfxSource.Stop();

            // Play game-over sound effect
            AudioSourceMiniGame.instance.PlayGameOverSound();
        }

        // Destroy all remaining spawned objects (eggs and bombs)
        ClearSpawnedObjects();

        // Display the game-over UI and show the final score
        gameOverPanel.SetActive(true);
        gameOverText.text = "Game Over";
        finalScoreText.text = "Final Score: " + score;

        // Calculate rewards based on score
        CalculateRewards();

        // Display rewards (Gold and EXP)
        goldText.text = "Gold: " + gold;
        expText.text = "EXP: " + exp;

        

        // Pause the game
        Time.timeScale = 0f;
    }

    private void CalculateRewards()
    {
        // Define score ranges
        int lowAverageScore = 140;    // Low average range
        int averageScore = 280;       // Average score
        int highAverageScore = 560;   // High average range

        // Initialize rewards
        int baseGold = 5;
        int baseExp = 5;

        // Reward multiplier calculation based on the score range
        if (score < lowAverageScore)
        {
            // Below low average: minimal reward
            gold = Mathf.FloorToInt(baseGold * 0.5f); // 50% of base reward
            exp = Mathf.FloorToInt(baseExp * 0.5f);   // 50% of base reward
        }
        else if (score >= lowAverageScore && score < averageScore)
        {
            // Low average to average: moderate reward
            gold = Mathf.FloorToInt(baseGold * 1.0f); // 100% of base reward
            exp = Mathf.FloorToInt(baseExp * 1.0f);   // 100% of base reward
        }
        else if (score >= averageScore && score < highAverageScore)
        {
            // Average to high average: greater reward
            gold = Mathf.FloorToInt(baseGold * 1.5f); // 150% of base reward
            exp = Mathf.FloorToInt(baseExp * 1.5f);   // 150% of base reward
        }
        else
        {
            // High average: maximum reward
            gold = Mathf.FloorToInt(baseGold * 2.0f); // 200% of base reward
            exp = Mathf.FloorToInt(baseExp * 2.0f);   // 200% of base reward
        }

        // For very low scores, ensure at least a small reward (e.g., 0 score)
        if (score == 0)
        {
            gold = 0;
            exp = 0;
        }
    }

    public void AddScore(int points)
    {
        // Add points to the score and ensure it doesn't go below 0
        score += points;
        score = Mathf.Max(score, 0);
        scoreText.text = "Score: " + score;
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        BarnMinigamePrefab.SetActive(false);
        HUDCanvas.SetActive(true);
        MainCamera3d.gameObject.SetActive(true);
        audioManager.gameObject.SetActive(true);

        // Update player resources and central hall data

        if (resourceManager != null)
        {
            Debug.Log("Adding Gold");
            ResourceManager.Instance.AdjustPlayerCurrency(Currency.Gold, gold);
        }
        if (centralHall != null)
        {
            Debug.Log("Adding Exp");
            centralHall.AddToCentralExp(exp);
        }
        else
        {
            Debug.LogError("CentralHall is null!");
        }

        Debug.Log("Exiting mini-game and saving rewards.");
    }

}
