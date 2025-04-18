using System;
using UnityEngine;
using NaughtyAttributes;

public class FriendCentralHall : MonoBehaviour, IClickableObject
{

    //variables for resetpopstate
    bool hasPopped;
    GameObject builtHighlight;
    const int HIGHLIGHT_INDEX = 0;

    public event Action FriendHallClicked;

    [Button]
    public void OnObjectClicked()
    {
        FriendHallClicked?.Invoke();
    }

    public void ResetPopState()
    {
        hasPopped = false;
        builtHighlight.SetActive(false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        builtHighlight = transform.GetChild(HIGHLIGHT_INDEX).gameObject;
    }
}
