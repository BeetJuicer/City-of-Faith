using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

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

    void Awake()
    {
        // Singleton pattern: ensures only one instance of GameController exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Initial setup when the game starts
        AudioSourceMiniGame.instance.sfxSource.Stop();  // Stop lingering sound effects
        AudioSourceMiniGame.instance.PlayStartSound();  // Play the start sound effect
        AudioSourceMiniGame.instance.PlayBackgroundMusic();  // Start playing background music

        // Initialize the UI elements
        timeText.text = "Time Left: " + Mathf.RoundToInt(timer) + "s";
        scoreText.text = "Score: 0";

        // Hide the game-over panel at the start
        gameOverPanel.SetActive(false);
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
    }

    private void SpawnObjects()
    {
        // Spawn objects at random positions within the screen bounds
        for (int i = 0; i < 3; i++)
        {
            float pos_x = Random.Range(-4.0f, 4.0f);  // Random x position for spawning
            float pos_y = 6.0f;                       // Fixed y position for spawning
            float initialVelocity = Random.Range(5.0f, 8.0f);  // Random downward velocity

            GameObject obj;
            // Randomly choose whether to spawn an egg (80% chance) or a bomb (20% chance)
            if (Random.Range(0, 100) < 80)
            {
                obj = Instantiate(gm[0], new Vector3(pos_x, pos_y, 0.1f), Quaternion.identity);
            }
            else
            {
                obj = Instantiate(gm[1], new Vector3(pos_x, pos_y, 0.1f), Quaternion.identity);
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
        AudioSourceMiniGame.instance.StopBackgroundMusic();
        AudioSourceMiniGame.instance.sfxSource.Stop();

        // Play game-over sound effect
        AudioSourceMiniGame.instance.PlayGameOverSound();

        // Display the game-over UI and show the final score
        gameOverPanel.SetActive(true);
        gameOverText.text = "Game Over";
        finalScoreText.text = "Final Score: " + score;

        // Calculate rewards based on score
        CalculateRewards();

        // Display rewards (Gold and EXP)
        goldText.text = "Gold: " + gold;
        expText.text = "EXP: " + exp;

        // Show exit button
        exitButton.gameObject.SetActive(true);

        // Pause the game
        Time.timeScale = 0f;
    }

    private void CalculateRewards()
    {
        // Rewards based on score thresholds
        if (score == 0)
        {
            gold = 0;
            exp = 0;
        }
        else if (score >= 1 && score <= 250)
        {
            gold = 3;
            exp = 3;
        }
        else if (score > 250 && score <= 425)
        {
            gold = 6;
            exp = 6;
        }
        else if (score > 450)
        {
            gold = 10;
            exp = 10;
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
        // Logic to exit the game (e.g., quit or return to the main menu)
    }
}
