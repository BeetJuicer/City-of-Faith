using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using TMPro;
using System;
using Unity.Services.Authentication.PlayerAccounts;

public class UsernamePasswordAuth : MonoBehaviour
{
    [SerializeField] private TMP_InputField username;
    [SerializeField] private TMP_InputField password;
    CloudSaveDB cloudSave;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private GameObject loginCanvas;

    public string _USERNAME { get; private set; }
    async void Awake()
    {
        try
        {
            await UnityServices.InitializeAsync();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private async void Start()
    {
        // await PlayerAccountService.Instance.StartSignInAsync(); for third party logins.
        cloudSave = FindAnyObjectByType<CloudSaveDB>();
        if (AuthenticationService.Instance.SessionTokenExists)
        {
            Debug.Log("Cached session exists. Attempting to restore session...");
            try
            {
                // SignInAnonymouslyAsync isn't just for guest accounts. This is how session token is used apparently.
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                cloudSave.LoadDatabase();
            }
            catch (RequestFailedException ex)
            {
                Debug.LogException(ex);
                text.text = "Failed to restore session.";
            }
        }
        else
        {
            text.text = "";
            loginCanvas.SetActive(true);
        }
    }

    public async void SignUp()
    {
        await SignUpWithUsernamePasswordAsync(username.text, password.text);
    }
    public async void SignIn()
    {
        await SignInWithUsernamePasswordAsync(username.text, password.text);
    }

    public async void UpdatePassword()
    {
        Debug.LogError("UpdatePassword is still not in use. No UI available yet.");
        return;
        await UpdatePasswordAsync("hello", "test");
    }


    async Task SignUpWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            await AuthenticationService.Instance.UpdatePlayerNameAsync(username);
            _USERNAME = username;
            Debug.Log("SignUp is successful for username: " + _USERNAME);
            cloudSave.LoadDatabase();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    async Task SignInWithUsernamePasswordAsync(string username, string password)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            cloudSave.LoadDatabase();
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }

    async Task UpdatePasswordAsync(string currentPassword, string newPassword)
    {
        try
        {
            await AuthenticationService.Instance.UpdatePasswordAsync(currentPassword, newPassword);
            Debug.Log("Password updated.");
        }
        catch (AuthenticationException ex)
        {
            // Compare error code to AuthenticationErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
        catch (RequestFailedException ex)
        {
            // Compare error code to CommonErrorCodes
            // Notify the player with the proper error message
            Debug.LogException(ex);
        }
    }
}
