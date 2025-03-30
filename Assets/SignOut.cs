using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignOut : MonoBehaviour
{
    public void LogOut()
    {
        print("Signing out...");
        AuthenticationService.Instance.SignOut(true);
        UnityEngine.SceneManagement.SceneManager.LoadScene("Cloud");
    }
}
