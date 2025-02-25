using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenTransitionTween : MonoBehaviour
{
    public Transform box;
    public CanvasGroup background;

    private void OnEnable()
    {
        AudioManager.Instance.PlaySFX("Canvas");
        LeanTween.reset();
        background.alpha = 0;
        background.LeanAlpha(1, 0.5f);

        box.localPosition = new Vector2(0, -Screen.height);
        box.LeanMoveLocalY(0, 0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public void CloseBox()
    {
        background.LeanAlpha(0, 0.5f);
        box.LeanMoveLocalY(-Screen.height, 0.5f).setEaseInExpo().setOnComplete(OnComplete);
    }

    void OnComplete()
    {
        AudioManager.Instance.PlaySFX("Canvas");
        gameObject.SetActive(false);
    }
}
