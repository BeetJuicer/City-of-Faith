using System.Collections;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SignOut : MonoBehaviour
{
    IEnumerator LogOutCoroutine()
    {
        print("Signing out...");
        AuthenticationService.Instance.SignOut(true);
        yield return new WaitForSeconds(1f);
        print("Changing Scene...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Cloud");
    }

    public void LogOut()
    {
        StartCoroutine(nameof(LogOutCoroutine));
    }
}
