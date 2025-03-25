using System.Collections;
using UnityEngine;

public class FlowerSpawner : MonoBehaviour
{
    public GameObject flowerPrefab;
    public Transform[] spawnPoints;
    public float spawnCooldown = 5f;

    void Start()
    {
        StartCoroutine(SpawnFlowers());
    }

    IEnumerator SpawnFlowers()
    {
        SpawnFlower();

        while (true)
        {
            yield return new WaitForSeconds(spawnCooldown);
            SpawnFlower();
        }
    }

    void SpawnFlower()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("No spawn points assigned!");
            return;
        }

        Transform randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        Instantiate(flowerPrefab, randomSpawnPoint.position, Quaternion.identity);
    }
}
