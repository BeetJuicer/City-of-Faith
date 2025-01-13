using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpTween : MonoBehaviour
{
    [SerializeField] GameObject SoftWheel, HardWheel, HUD;
    // Store LeanTween IDs
    private int softWheelTweenId, hardWheelTweenId;

    public void OnEnable()
    {
        LeanTween.reset();
        HUD.SetActive(false);

        // Store the LeanTween IDs
        softWheelTweenId = LeanTween.rotateAround(SoftWheel, Vector3.forward, -360, 10f).setLoopClamp().id;
        hardWheelTweenId = LeanTween.rotateAround(HardWheel, Vector3.forward, 360, 10f).setLoopClamp().id;
    }

    public void CloseLevel()
    {
        StopAnimations();
        OnComplete();
    }

    private void StopAnimations()
    {
        // Cancel the animations using the stored IDs
        LeanTween.cancel(softWheelTweenId);
        LeanTween.cancel(hardWheelTweenId);
    }

    public void OnComplete()
    {
        gameObject.SetActive(false);
        HUD.SetActive(true);

        //ResourceManager rs;
        //text.text = rs.PlayerCurrencies[Currency.Glory].ToString();
    }


}
