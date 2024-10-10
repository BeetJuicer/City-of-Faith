using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SimpleJuiceAnimation : MonoBehaviour
{
    // Adjust these values to control the squash and stretch effect
    [SerializeField] private float duration = 0.5f;

    private Vector3 originalScale;
    [SerializeField] private Vector3 scaleFrom = new Vector3(0.7f, 0.7f, 0.7f);
    [SerializeField] private Vector3 scaleTo = new Vector3(0.9f, 0.9f, 0.9f);

    void Start()
    {
        // Store the original scale for reference
        originalScale = transform.localScale;

        transform.localScale = scaleFrom;

        OnScale();
    }

    private void OnScale()
    {
        transform.DOScale(scaleTo, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                transform.DOScale(scaleFrom, duration)
                .SetEase(Ease.OutBounce)
                .SetDelay(0.5f)
                .OnComplete(OnScale);
            });
    }
}
