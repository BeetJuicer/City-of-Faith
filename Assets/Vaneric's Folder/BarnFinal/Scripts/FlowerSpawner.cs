using System.Collections;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    public GameObject flowerPrefab;
    public Transform[] spawnPoints;
    public float spawnCooldown = 5f;
    private GameObject[] spawnedFlowers;

    void Start()
    {
        if (flowerPrefab == null)
        {
            Debug.LogError("Flower Prefab is missing!");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned to FlowerSpawner!");
            return;
        }

        spawnedFlowers = new GameObject[spawnPoints.Length];
        SpawnInitialFlowers();
    }

    void SpawnInitialFlowers()
    {
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            spawnedFlowers[i] = Instantiate(flowerPrefab, spawnPoints[i].position, Quaternion.identity);
        }
    }

    public void OnFlowerEaten(Transform eatenFlowerPosition)
    {
        Debug.Log("Flower eaten! Scheduling respawn...");
        StartCoroutine(RespawnFlower(eatenFlowerPosition));
    }

    private IEnumerator RespawnFlower(Transform flowerPosition)
    {
        // Find the eaten flower and remove it first
        for (int i = 0; i < spawnedFlowers.Length; i++)
        {
            if (spawnedFlowers[i] != null && spawnedFlowers[i].transform.position == flowerPosition.position)
            {
                Destroy(spawnedFlowers[i]); // Remove the eaten flower
                spawnedFlowers[i] = null;
                break;
            }
        }

        yield return new WaitForSeconds(spawnCooldown); // Wait before respawning

        // Spawn a new flower at the correct position
        for (int i = 0; i < spawnedFlowers.Length; i++)
        {
            if (spawnedFlowers[i] == null)
            {
                spawnedFlowers[i] = Instantiate(flowerPrefab, flowerPosition.position, Quaternion.identity);
                break;
            }
        }

        Debug.Log("Flower respawned!");
    }

    public void ResetFlowers()
    {
        if (spawnPoints == null || flowerPrefab == null)
        {
            Debug.LogError("Cannot reset flowers: Missing spawn points or flowerPrefab!");
            return;
        }

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            if (spawnedFlowers[i] != null)
            {
                Destroy(spawnedFlowers[i]); // Destroy existing flowers
            }

            spawnedFlowers[i] = Instantiate(flowerPrefab, spawnPoints[i].position, Quaternion.identity);
        }

        Debug.Log("Flowers have been reset!");
    }
}
