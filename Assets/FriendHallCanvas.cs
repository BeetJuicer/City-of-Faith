using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FriendHallCanvas : MonoBehaviour, ICanvasView
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button donateBtn;
    [SerializeField] private Button exitBtn;
    [SerializeField] private Button plusBtn;
    [SerializeField] private Button minusBtn;
    [SerializeField] private TextMeshProUGUI goldDonationAmount;

    public event Action Canvas_Deactivated;
    public event Action<int> Donate_Clicked;

    private int[] goldDonationTiers = new int[] { 1, 5, 10, 15 };
    private int currentGoldDonationTier;

    private void Start()
    {
        exitBtn.onClick.AddListener(Deactivate);
        donateBtn.onClick.AddListener(() => Donate_Clicked?.Invoke(goldDonationTiers[currentGoldDonationTier]));
        plusBtn.onClick.AddListener(Add);
        minusBtn.onClick.AddListener(Subtract);

        currentGoldDonationTier = 0;
        goldDonationAmount.text = goldDonationTiers[currentGoldDonationTier].ToString();
        minusBtn.interactable = false;
    }

    private void Add()
    {
        if (currentGoldDonationTier + 1 >= goldDonationTiers.Length)
        {
            plusBtn.interactable = false;
            return;
        }

        currentGoldDonationTier++;
        goldDonationAmount.text = goldDonationTiers[currentGoldDonationTier].ToString();
        minusBtn.interactable = true;

        if (currentGoldDonationTier == goldDonationTiers.Length - 1)
        {
            plusBtn.interactable = false;
        }
    }
    private void Subtract()
    {
        if (currentGoldDonationTier == 0)
        {
            minusBtn.interactable = false;
            return;
        }

        currentGoldDonationTier--;
        goldDonationAmount.text = goldDonationTiers[currentGoldDonationTier].ToString();
        plusBtn.interactable = true;

        if (currentGoldDonationTier == 0)
        {
            minusBtn.interactable = false;
        }
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
