using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenHUDTween : MonoBehaviour
{
    [SerializeField] GameObject ExperienceBar;
    [SerializeField] GameObject OptionButton;
    [SerializeField] GameObject FriendButton;
    [SerializeField] GameObject QuestButton;
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
        QuestButton.transform.localScale = Vector3.zero;
        ShopButton.transform.localScale = Vector3.zero;
        GloryBar.transform.localScale = Vector3.zero;
        CoinBar.transform.localScale = Vector3.zero;

        //Animations
        LeanTween.scale(ExperienceBar, new Vector3(1.172214f, 1.088488f, 0.8076155f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(OptionButton, new Vector3(0.7672241f, 0.7672241f, 0.7672241f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(FriendButton, new Vector3(0.7458504f, 0.7458504f, 0.7458504f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(QuestButton, new Vector3(0.7458504f, 0.7458504f, 0.7458504f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(ShopButton, new Vector3(0.7558168f, 0.7699736f, 2.421002f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(GloryBar, new Vector3(0.9331497f, 0.9331497f, 0.9331497f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
        LeanTween.scale(CoinBar, new Vector3(0.9591717f, 0.9591717f, 0.9591717f), 0.8f).setEase(LeanTweenType.easeOutExpo).setDelay(0.2f);
    }
}
