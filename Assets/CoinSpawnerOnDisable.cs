using UnityEngine;
using PrimeTween;
using System.Collections;

public class CoinSpawnerOnDisable : MonoBehaviour
{
    [SerializeField] private Transform coinPrefab; // Coin prefab (UI Image)
    [SerializeField] private RectTransform targetUI; // Target UI element where coins should move to
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private TweenSettings tweenSettings;
    [SerializeField] private Camera mainCamera;

    public void SpawnCoins(Vector3 startPosition, int coinCount = 10, float spreadRadius = 20f, float spawnDelay = 0.1f, float moveDelay = 0.3f)
    {
        StartCoroutine(SpawnCoinsRoutine(startPosition, coinCount, spreadRadius, spawnDelay, moveDelay));
    }

    private IEnumerator SpawnCoinsRoutine(Vector3 startPosition, int coinCount, float spreadRadius, float spawnDelay, float moveDelay)
    {
        Vector3 endPosition = GetTargetPosition();

        for (int i = 0; i < coinCount; i++)
        {
            // Generate random offset
            float randomX = Random.Range(-spreadRadius, spreadRadius);
            float randomY = Random.Range(-spreadRadius, spreadRadius);
            Vector3 randomOffset = new Vector3(randomX, randomY, 0);
            Vector3 randomizedStart = startPosition + randomOffset;

            // Instantiate the coin
            var coin = Instantiate(coinPrefab, randomizedStart, Quaternion.identity, transform);

            // Wait before spawning the next coin
            yield return new WaitForSeconds(spawnDelay);

            // Wait before all coins start moving at once (after last coin spawns)
            if (i == coinCount)
            {
                yield return new WaitForSeconds(moveDelay);
            }

            // Animate coin movement
            Tween.Position(coin, endPosition, tweenSettings).OnComplete(() => Destroy(coin.gameObject));
        }
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 worldPosition;
        if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(targetUI, targetUI.position, mainCamera, out worldPosition);
        }
        else
        {
            Vector3 screenPosition = mainCamera.WorldToScreenPoint(targetUI.position);
            screenPosition.z = Mathf.Abs(mainCamera.transform.position.z);
            worldPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        }
        return worldPosition;
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = GetTargetPosition();

        Gizmos.color = Color.red;
        Gizmos.DrawLine(startPosition, endPosition);
    }
}