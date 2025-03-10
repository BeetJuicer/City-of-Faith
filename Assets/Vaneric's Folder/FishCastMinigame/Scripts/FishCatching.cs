using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishCatching : MonoBehaviour
{
    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
    private FishingController fishingController;
    private FishSpawner fishSpawner;

    private void Start()
    {
        fishingController = FindObjectOfType<FishingController>();
        fishSpawner = FindObjectOfType<FishSpawner>();

        if (fishingController == null)
            Debug.LogError("FishingController not found in the scene!");

        if (fishSpawner == null)
            Debug.LogError("FishSpawnerManager not found in the scene!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BigFish") || other.CompareTag("SmallFish"))
        {
            Debug.Log($"Fish Entered: {other.name}");

            if (!activeCoroutines.ContainsKey(other.gameObject))
            {
                float catchTime = (other.CompareTag("SmallFish")) ? 1f : 2f; // SmallFish = 1s, BigFish = 2s
                Coroutine catchRoutine = StartCoroutine(CatchFish(other.gameObject, catchTime));
                activeCoroutines.Add(other.gameObject, catchRoutine);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (activeCoroutines.ContainsKey(other.gameObject))
        {
            Debug.Log($"Fish Left Capture Area: {other.gameObject.name}");
            StopCoroutine(activeCoroutines[other.gameObject]);
            activeCoroutines.Remove(other.gameObject);
        }
    }

    IEnumerator CatchFish(GameObject fish, float catchTime)
    {
        float elapsedTime = 0f;
        while (elapsedTime < catchTime)
        {
            if (!fish) yield break;

            elapsedTime += Time.deltaTime;
            Debug.Log($"{fish.name} Inside Capture Box: {elapsedTime}/{catchTime}");

            yield return null;
        }

        if (fish)
        {
            Debug.Log($"Caught: {fish.name}!");

            // Update Score in FishingController
            int points = fish.CompareTag("BigFish") ? 10 : 5;
            if (fishingController != null)
            {
                fishingController.AddScore(points);
            }

            // Spawn new fish after catching
            if (fishSpawner != null)
            {
                fishSpawner.RespawnFish(fish);
            }

            Destroy(fish);
        }

        activeCoroutines.Remove(fish);
    }
}
