using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

public class CentralHallClickable : MonoBehaviour, IClickableObject
{
    public GameObject CentralHallCanvas;

    public void OnObjectClicked()
    {

            PopOnClick();
            print("Central Hall Clicked");
    }

    public void PopOnClick()
    {
        CentralHallCanvas.gameObject.SetActive(true);
    }

    public void ResetPopState()
    {
    }

}
