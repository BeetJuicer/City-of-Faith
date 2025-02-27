using UnityEngine;

public class SheepSpawner : MonoBehaviour
{
    public GameObject sheepPrefab;
    public Transform[] spawnPoints;

    public void SpawnSheep(int count, float speed)
    {
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, spawnPoints.Length);
            GameObject sheep = Instantiate(sheepPrefab, spawnPoints[randomIndex].position, Quaternion.identity);

            SheepMovement sheepMovement = sheep.GetComponent<SheepMovement>();
            if (sheepMovement != null)
            {
                sheepMovement.SetSpeed(speed);
            }
        }
    }

    public Vector3 GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("No spawn points assigned!");
            return Vector3.zero;
        }

        return spawnPoints[Random.Range(0, spawnPoints.Length)].position;
    }

    public GameObject SpawnLostSheep()
    {
        Vector3 spawnPosition = GetRandomSpawnPoint();
        GameObject lostSheep = Instantiate(sheepPrefab, spawnPosition, Quaternion.identity);
        lostSheep.name = "LostSheep";
        return lostSheep;
    }
}
