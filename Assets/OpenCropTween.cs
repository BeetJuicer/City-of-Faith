using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenCropTween : MonoBehaviour
{
    [SerializeField] GameObject CropShop;

    public void OnEnable()
    {
        LeanTween.reset();
        CropShop.transform.localScale = Vector3.zero;
        LeanTween.scale(CropShop, Vector3.one, 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
    }

    public void CloseBox()
    {
        CropShop.LeanMoveLocalX(Screen.width, 0.5f).setEaseInExpo().setOnComplete(OnComplete);
    }

    void OnComplete()
    {
        gameObject.SetActive(false);
    }
}
