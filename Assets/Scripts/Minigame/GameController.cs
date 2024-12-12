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
    public Button exitButton;

    int score = 0;

    float spawnCooldown = 0.5f;
    float currentCooldown = 0.0f;

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
        timeText.text = "Time Left: " + Mathf.RoundToInt(timer) + "s";
        scoreText.text = "Score: 0";

        // Initially hide the game over panel
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

                    currentCooldown = spawnCooldown;
                }
            }
        }
        else
        {
            // Game Over
            gameOverPanel.SetActive(true);
            gameOverText.text = "Game Over";
            finalScoreText.text = "Final Score: " + score;
            exitButton.gameObject.SetActive(true);

            // Stop the game loop to prevent further updates
            Time.timeScale = 0f;
        }
    }

    public void AddScore(int points)
    {
        score += points;
        score = Mathf.Max(score, 0); // Ensures score doesn't go below 0
        scoreText.text = "Score: " + score;
    }

    public void ExitGame()
    {
        // Implement your exit logic here
        // For example, you might load a specific scene or quit the application
    }
}