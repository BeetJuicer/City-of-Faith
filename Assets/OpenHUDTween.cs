using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenHUDTween : MonoBehaviour
{
    [SerializeField] GameObject ExperienceBar;
    [SerializeField] GameObject OptionButton;
    [SerializeField] GameObject FriendButton;
    [SerializeField] GameObject ShopButton;
    [SerializeField] GameObject GloryBar;
    [SerializeField] GameObject CoinBar;

    private void OnEnable()
    {
        LeanTween.reset();
        //Settings to Zero
        ExperienceBar.transform.localScale = Vector3.zero;
        OptionButton.transform.localScale = Vector3.zero;
        FriendButton.transform.localScale = Vector3.zero;
        ShopButton.transform.localScale = Vector3.zero;
        GloryBar.transform.localScale = Vector3.zero;
        CoinBar.transform.localScale = Vector3.zero;

        //Animations
        LeanTween.scale(ExperienceBar, new Vector3(1.562077f, 1.450505f, 1.076218f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(OptionButton, Vector3.one, 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(FriendButton, Vector3.one, 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(ShopButton, new Vector3(0.9990134f, 1.017725f, 3.2f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(GloryBar, new Vector3(1.233764f, 1.233764f, 1.233764f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(CoinBar, new Vector3(1.233764f, 1.233764f, 1.233764f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
    }
}
