using UnityEngine;
using System.Collections.Generic;

public class FishSpawner : MonoBehaviour
{
    public GameObject bigFishPrefab;
    public GameObject smallFishPrefab;
    public BoxCollider2D boundaryArea;

    public int initialBigFishCount = 9;
    public int initialSmallFishCount = 9;

    private List<GameObject> activeFishes = new List<GameObject>();
    private bool hasSpawned = false; // Prevents multiple spawns on first run

    void Start()
    {
        if (!hasSpawned)
        {
            hasSpawned = true; // Mark as spawned
            ClearAllFish();  // Ensure no extra fish exist
            SpawnInitialFish();
        }
    }

    public void SpawnInitialFish()
    {
        if (activeFishes.Count > 0) return; // Prevent double spawning
        SpawnFish(bigFishPrefab, initialBigFishCount);
        SpawnFish(smallFishPrefab, initialSmallFishCount);
    }

    void SpawnFish(GameObject fishPrefab, int fishCount)
    {
        if (boundaryArea == null)
        {
            Debug.LogError("Fish Boundary Area is not set in FishSpawner!");
            return;
        }

        for (int i = 0; i < fishCount; i++)
        {
            Vector2 spawnPosition = GetRandomPositionInsideBoundary();
            GameObject fishInstance = Instantiate(fishPrefab, spawnPosition, Quaternion.identity);
            activeFishes.Add(fishInstance);

            if (fishInstance.TryGetComponent(out FishMovement fishMovement))
            {
                fishMovement.boundaryArea = boundaryArea;
            }
            else
            {
                Debug.LogError($"FishPrefab ({fishPrefab.name}) is missing a FishMovement script!");
            }
        }
    }

    Vector2 GetRandomPositionInsideBoundary()
    {
        Bounds bounds = boundaryArea.bounds;
        return new Vector2(Random.Range(bounds.min.x, bounds.max.x), Random.Range(bounds.min.y, bounds.max.y));
    }

    public void RespawnFish(GameObject caughtFish)
    {
        activeFishes.Remove(caughtFish);
        Destroy(caughtFish);

        GameObject newFishPrefab = (Random.value > 0.5f) ? bigFishPrefab : smallFishPrefab;
        Vector2 spawnPosition = GetRandomPositionInsideBoundary();
        GameObject newFish = Instantiate(newFishPrefab, spawnPosition, Quaternion.identity);
        activeFishes.Add(newFish);

        if (newFish.TryGetComponent(out FishMovement fishMovement))
        {
            fishMovement.boundaryArea = boundaryArea;
        }

        Debug.Log("1 new fish spawned!");
    }

    public void ClearAllFish()
    {
        foreach (GameObject fish in activeFishes)
        {
            if (fish != null) Destroy(fish);
        }
        activeFishes.Clear();
        Debug.Log("All fish cleared.");
    }
}
