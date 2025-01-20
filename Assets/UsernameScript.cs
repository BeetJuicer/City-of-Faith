using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UsernameScript : MonoBehaviour
{
    [SerializeField] private Button confirmBtn;
    [SerializeField] private TMP_Text confirmText;
    [SerializeField] private TMP_InputField usernameInput;

    private RectTransform inputFieldRectTransform; // To manipulate the position of the input field
    private bool isInputFieldMoved = false; // To track if the field has already moved

    private void Start()
    {
        // Ensure the button is initially disabled and text is gray
        UpdateButtonState();

        // Cache the RectTransform of the input field
        inputFieldRectTransform = usernameInput.GetComponent<RectTransform>();

        // Add listeners to the input field and button
        usernameInput.onValueChanged.AddListener(OnUsernameValueChanged);
        confirmBtn.onClick.AddListener(OnConfirmButtonClicked);
    }

    private void OnUsernameValueChanged(string inputValue)
    {
        // Update the button state when the input value changes
        UpdateButtonState();

        // If there's a value and the input field hasn't moved yet, move it
        if (!string.IsNullOrEmpty(inputValue) && !isInputFieldMoved)
        {
            SlideInputFieldUp();
            isInputFieldMoved = true;
        }
        else if (string.IsNullOrEmpty(inputValue) && isInputFieldMoved)
        {
            SlideInputFieldDown();
            isInputFieldMoved = false;
        }
    }

    private void SlideInputFieldUp()
    {
        // Slide the input field up by 200 on the Y-axis
        LeanTween.moveY(inputFieldRectTransform, inputFieldRectTransform.anchoredPosition.y + 200, 0.5f)
                 .setEase(LeanTweenType.easeInOutQuad);
    }

    private void SlideInputFieldDown()
    {
        // Slide the input field back to its original position
        LeanTween.moveY(inputFieldRectTransform, inputFieldRectTransform.anchoredPosition.y - 200, 0.5f)
                 .setEase(LeanTweenType.easeInOutQuad);
    }

    private void UpdateButtonState()
    {
        bool hasValue = !string.IsNullOrEmpty(usernameInput.text);

        // Enable/disable the button
        confirmBtn.interactable = hasValue;

        // Change the text color
        confirmText.color = hasValue ? Color.white : Color.gray;
    }

    private void OnConfirmButtonClicked()
    {
        // Call the SetUser method from the Database script with the input value
        string username = usernameInput.text;

        if (!string.IsNullOrEmpty(username))
        {
            Database.Instance.SetUser(username);
            Debug.Log($"Username {username} passed to Database.SetUser");
            OnDestroy();
        }
        else
        {
            Debug.LogWarning("Input field is empty. No username provided.");
        }
    }

    private void OnDestroy()
    {
        // Remove listeners to avoid memory leaks
        Debug.Log("username listeners destroyed");
        usernameInput.onValueChanged.RemoveListener(OnUsernameValueChanged);
        confirmBtn.onClick.RemoveListener(OnConfirmButtonClicked);
    }
}
