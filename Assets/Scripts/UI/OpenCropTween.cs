using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCropTween : MonoBehaviour
{
    [SerializeField] GameObject CropShopInner;
    private Vector3 initialPosition; // Store the initial position of the GameObject

    private void Awake()
    {
        // Save the initial position of CropShopInner when the script starts
        initialPosition = CropShopInner.transform.localPosition;
    }

    private void OnEnable()
    {
        CropShopInner.transform.localPosition = initialPosition;
        CropShopInner.transform.localScale = Vector3.zero;

        AudioManager.Instance.PlaySFX("Canvas");
        LeanTween.reset();
        CropShopInner.transform.localScale = Vector3.zero;
        LeanTween.scale(CropShopInner, Vector3.one, 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
    }

    public void CloseBox()
    {
        CropShopInner.LeanMoveLocalX(Screen.width, 0.5f).setEaseInExpo().setOnComplete(OnComplete);
    }

    void OnComplete()
    {
        AudioManager.Instance.PlaySFX("Canvas");
        gameObject.SetActive(false);
    }
}
