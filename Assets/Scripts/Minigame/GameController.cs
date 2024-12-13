using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController instance;

    float timer = 30;
    public GameObject[] gm;
    public TextMeshProUGUI timeText;
    public TextMeshProUGUI scoreText;

    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI expText;
    public Button exitButton;

    int score = 0;
    int gold = 0;
    int exp = 0;

    float spawnCooldown = 0.5f;
    float currentCooldown = 0.0f;
    bool isGameOver = false; // Flag to control game-over logic

    void Awake()
    {
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
        // Stop all current sounds before starting
        AudioSourceMiniGame.instance.sfxSource.Stop();  // Stop any lingering sound effects

        // Play the start sound effect once when the game begins
        AudioSourceMiniGame.instance.PlayStartSound();

        // Start playing background music
        AudioSourceMiniGame.instance.PlayBackgroundMusic();

        // Initialize UI
        timeText.text = "Time Left: " + Mathf.RoundToInt(timer) + "s";
        scoreText.text = "Score: 0";

        // Initially hide the game-over panel
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            timeText.text = "Time Left: " + Mathf.RoundToInt(timer) + "s";

            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0)
            {
                SpawnObjects();
                currentCooldown = spawnCooldown;
            }
        }
        else if (!isGameOver) // Trigger game-over logic only once
        {
            HandleGameOver();
        }
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < 3; i++)
        {
            float pos_x = Random.Range(-4.0f, 4.0f);
            float pos_y = 6.0f;
            float initialVelocity = Random.Range(5.0f, 8.0f);

            GameObject obj;
            if (Random.Range(0, 100) < 80)
            {
                obj = Instantiate(gm[0], new Vector3(pos_x, pos_y, 0.1f), Quaternion.identity);
            }
            else
            {
                obj = Instantiate(gm[1], new Vector3(pos_x, pos_y, 0.1f), Quaternion.identity);
            }

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(Vector2.down * initialVelocity, ForceMode2D.Impulse);
            }
        }
    }

    private void HandleGameOver()
    {
        isGameOver = true; // Set the game-over flag

        // Stop background music
        AudioSourceMiniGame.instance.StopBackgroundMusic();

        // Stop any other sound effects to avoid overlap
        AudioSourceMiniGame.instance.sfxSource.Stop();

        // Play the game-over sound effect once
        AudioSourceMiniGame.instance.PlayGameOverSound();

        // Display the game-over UI
        gameOverPanel.SetActive(true);
        gameOverText.text = "Game Over";
        finalScoreText.text = "Final Score: " + score;

        // Calculate rewards based on score
        CalculateRewards();

        // Display rewards
        goldText.text = "Gold: " + gold;
        expText.text = "EXP: " + exp;

        exitButton.gameObject.SetActive(true);

        // Pause the game loop
        Time.timeScale = 0f;
    }

    private void CalculateRewards()
    {
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
        score += points;
        score = Mathf.Max(score, 0); // Ensure score doesn't drop below 0
        scoreText.text = "Score: " + score;
    }

    public void ExitGame()
    {
        // Implement your exit logic here (e.g., quit the game or return to the main menu)
    }
}
