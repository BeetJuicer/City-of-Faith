using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FishCatching : MonoBehaviour
{
    [Header("UI Progress Bar (Slider)")]
    public Slider circularSlider;
    public Transform redOutlineCaptureBox;
    public RectTransform sliderRectTransform;

    private Dictionary<GameObject, Coroutine> activeCoroutines = new Dictionary<GameObject, Coroutine>();
    private List<GameObject> fishInsideBox = new List<GameObject>(); // Tracks fish inside capture box
    private FishingController fishingController;
    private FishSpawner fishSpawner;

    private void Start()
    {
        fishingController = FindObjectOfType<FishingController>();
        fishSpawner = FindObjectOfType<FishSpawner>();

        if (fishingController == null)
            Debug.LogError("❌ FishingController not found in the scene!");

        if (fishSpawner == null)
            Debug.LogError("❌ FishSpawnerManager not found in the scene!");

        if (circularSlider == null)
        {
            Debug.LogError("❌ Circular Slider reference is missing in Inspector!");
        }
        else
        {
            circularSlider.value = 0f;
            circularSlider.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (circularSlider.gameObject.activeSelf && redOutlineCaptureBox != null)
        {
            sliderRectTransform.position = redOutlineCaptureBox.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BigFish") || other.CompareTag("SmallFish"))
        {
            Debug.Log($"🐟 Fish Entered: {other.name}");

            if (!fishInsideBox.Contains(other.gameObject))
            {
                fishInsideBox.Add(other.gameObject);
            }

            if (!activeCoroutines.ContainsKey(other.gameObject))
            {
                float catchTime = (other.CompareTag("SmallFish")) ? 1f : 2f;
                Coroutine catchRoutine = StartCoroutine(CatchFish(other.gameObject, catchTime));
                activeCoroutines.Add(other.gameObject, catchRoutine);
            }

            if (circularSlider != null && !circularSlider.gameObject.activeSelf)
            {
                circularSlider.gameObject.SetActive(true);
                circularSlider.value = 0f;
                sliderRectTransform.position = redOutlineCaptureBox.position;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("BigFish") || other.CompareTag("SmallFish"))
        {
            Debug.Log($"🚫 Fish Left Capture Area: {other.gameObject.name}");

            if (activeCoroutines.ContainsKey(other.gameObject))
            {
                StopCoroutine(activeCoroutines[other.gameObject]);
                activeCoroutines.Remove(other.gameObject);
            }

            if (fishInsideBox.Contains(other.gameObject))
            {
                fishInsideBox.Remove(other.gameObject);
            }

            // Hide slider only if no fish remain in the capture area
            if (fishInsideBox.Count == 0 && circularSlider != null)
            {
                circularSlider.gameObject.SetActive(false);
            }
        }
    }

    IEnumerator CatchFish(GameObject fish, float catchTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < catchTime)
        {
            if (fish == null)
            {
                Debug.LogWarning("Fish was destroyed before being caught!");
                yield break;
            }

            elapsedTime += Time.deltaTime;
            Debug.Log($" {fish.name} Inside Capture Box: {elapsedTime}/{catchTime}");

            if (circularSlider != null)
            {
                circularSlider.value = elapsedTime / catchTime;
            }

            yield return null;
        }

        if (fish != null)
        {
            Debug.Log($"🎉 Caught: {fish.name}!");

            int points = fish.CompareTag("BigFish") ? 10 : 5;
            fishingController?.AddScore(points);
            fishSpawner?.RespawnFish(fish);

            fishInsideBox.Remove(fish);
            activeCoroutines.Remove(fish);
            Destroy(fish);
        }

        // Hide slider only if no fish remain
        if (fishInsideBox.Count == 0 && circularSlider != null)
        {
            circularSlider.gameObject.SetActive(false);
        }
    }
}
