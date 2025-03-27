using UnityEngine;
using System.Collections;

public class WolfSpawner : MonoBehaviour
{
    [Header("Wolf Settings")]
    public GameObject wolfPrefab;
    public Transform[] spawnPoints;

    [Header("Spawn Settings")]
    public float initialSpawnInterval = 3f;
    private float spawnInterval;
    private bool isSpawning = false;
    private float difficultyMultiplier = 0.95f;
    private bool stopSpawning = true;
    private float minSpawnInterval = 1.8f;

    void Start()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned in WolfSpawner!");
            return;
        }

        if (wolfPrefab == null)
        {
            Debug.LogError("Wolf prefab is NOT assigned in WolfSpawner!");
            return;
        }

        spawnInterval = initialSpawnInterval;
        stopSpawning = true;
    }

    public void StartSpawning()
    {
        if (!isSpawning && !stopSpawning)
        {
            isSpawning = true;
            Debug.Log("Wolves will start spawning...");
            StartCoroutine(SpawnWolvesRepeatedly());
        }
    }

    private IEnumerator SpawnWolvesRepeatedly()
    {
        while (isSpawning)
        {
            if (stopSpawning) yield break;

            yield return new WaitForSeconds(spawnInterval);
            SpawnWolf();
        }
    }

    private void SpawnWolf()
    {
        if (spawnPoints.Length == 0 || wolfPrefab == null) return;

        int wolvesPerSpawn = Random.Range(2, 4);
        Debug.Log($"Spawning {wolvesPerSpawn} wolves...");

        for (int i = 0; i < wolvesPerSpawn; i++)
        {
            Transform selectedSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(wolfPrefab, selectedSpawnPoint.position, Quaternion.identity);
        }

        spawnInterval = Mathf.Max(minSpawnInterval, spawnInterval * difficultyMultiplier);
    }

    public void StopSpawning()
    {
        stopSpawning = true;
        isSpawning = false;
        Debug.Log("Wolf Spawning Stopped!");
        StopAllCoroutines();
    }

    public void ClearAllWolves()
    {
        foreach (GameObject wolf in GameObject.FindGameObjectsWithTag("Wolf"))
        {
            Destroy(wolf);
        }
        Debug.Log("All wolves cleared!");
    }

    public void ResetSpawning()
    {
        StopSpawning();
        ClearAllWolves();
        StartSpawning();
    }

    public void EnableSpawning()
    {
        stopSpawning = false;
    }
}
