using UnityEngine;
using TMPro;
using System.Collections;

public class FishGameController : MonoBehaviour
{
    // Prefabs for fish and trash objects to spawn
    public GameObject[] fishPrefabs;
    public GameObject[] trashPrefabs;

    // Game settings
    public float jumpForce = 5f;            // Force for fish jumping
    public float trashJumpForce = 5f;       // Force for trash jumping
    public float gameTime = 30f;            // Total game time in seconds
    public int score = 0;                   // Current score
    public TextMeshProUGUI scoreText;       // UI text to display score
    public TextMeshProUGUI timeText;        // UI text to display remaining time
    public GameObject gameOverPanel;       // UI panel to display at game over
    public TextMeshProUGUI gameOverText;    // UI text for game over message
    public TextMeshProUGUI finalScoreText;  // UI text for final score display
    public TextMeshProUGUI goldText;        // UI text for gold reward
    public TextMeshProUGUI expText;         // UI text for experience points
    public FishAudioSource fishAudioSource; // Audio source for game sounds

    // Private variables
    private float timeLeft;                 // Remaining game time
    private bool isGameOver = false;        // Flag to check if the game is over
    private Camera mainCamera;              // Camera reference
    private readonly Vector2[] spawnPositions = new Vector2[3]; // Spawn positions on screen
    private int lastSpawnIndex = -1;        // Prevent consecutive spawns at the same position

    void Start()
    {
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

    void Update()
    {
        // Only update if the game is not over
        if (!isGameOver)
        {
            timeLeft -= Time.deltaTime;  // Decrease the time left
            UpdateTimerText();           // Update the timer display

            // Check if time is up
            if (timeLeft <= 0)
                GameOver();
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
    void GameOver()
    {
        isGameOver = true;  // Set the game over flag
        gameOverPanel.SetActive(true); // Show game over panel
        gameOverText.text = "Game Over"; // Display game over message
        finalScoreText.text = "Final Score: " + score; // Show final score

        // Determine rewards based on the final score
        int goldReward = score >= 300 ? 10 : score >= 150 ? 6 : 3;
        int expReward = score >= 300 ? 10 : score >= 150 ? 6 : 3;

        // Display rewards on the UI
        goldText.text = "Gold: " + goldReward;
        expText.text = "EXP: " + expReward;

        // Pause the game and stop background music
        Time.timeScale = 0f;
        fishAudioSource.StopBackgroundMusic();
        fishAudioSource.PlayGameOverSFX(); // Play game over sound
    }
}
