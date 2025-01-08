using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpTween : MonoBehaviour
{
    [SerializeField] GameObject SoftWheel, HardWheel, HUD;

    public void OnEnable()
    {
        LeanTween.reset();
        HUD.SetActive(false);
        LeanTween.rotateAround(SoftWheel, Vector3.forward, -360, 10f).setLoopClamp();
        LeanTween.rotateAround(HardWheel, Vector3.forward, 360, 10f).setLoopClamp();
    }

    public void CloseLevel()
    {
        OnComplete();
    }

    public void OnComplete()
    {
        SoftWheel.SetActive(false);
        HardWheel.SetActive(false);
        gameObject.SetActive(false);
        HUD.SetActive(true);
    }
}
