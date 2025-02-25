using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCropTween : MonoBehaviour
{
    [SerializeField] GameObject CropShopInner;
    private Vector3 initialPosition; // Store the initial position of the GameObject
    private Structure structure;

    private void OnEnable()
    {
        AudioManager.Instance.PlaySFX("Canvas");
        LeanTween.reset();
        CropShopInner.transform.localScale = Vector3.zero;
        LeanTween.scale(CropShopInner, Vector3.one, 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
    }

    public void CloseBox()
    {
        LeanTween.scale(CropShopInner, new Vector3(1.15f, 1.15f, 1.15f), 0.3f).setEase(LeanTweenType.easeOutExpo);
        LeanTween.scale(CropShopInner, Vector3.zero, 0.3f).setEase(LeanTweenType.easeOutExpo).setDelay(0.1f).setOnComplete(OnComplete);
    }

    void OnComplete()
    {
        AudioManager.Instance.PlaySFX("Canvas");
        gameObject.SetActive(false);
    }
}
