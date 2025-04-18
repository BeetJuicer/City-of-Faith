using System;
using UnityEngine;
using UnityEngine.UI;

public class FriendHallCanvas : MonoBehaviour, ICanvasView
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button donateBtn;
    [SerializeField] private Button exitBtn;

    public event Action Canvas_Deactivated;
    public event Action Donate_Clicked;

    private void Start()
    {
        exitBtn.onClick.AddListener(Deactivate);
        donateBtn.onClick.AddListener(() => Donate_Clicked?.Invoke());
    }


    private void OnDestroy()
    {
        exitBtn.onClick.RemoveAllListeners();
        donateBtn.onClick.RemoveAllListeners();
    }


    public void Activate()
    {
        panel.SetActive(true);
    }

    public void Deactivate()
    {
        panel.SetActive(false);
        Canvas_Deactivated?.Invoke();
    }
}
