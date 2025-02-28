using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using Unity.Services.Core;

public class TestHello : MonoBehaviour
{
    // Start is called before the first frame update
    private async void Start()
    {
        // Initialize the Unity Services Core SDK
        await UnityServices.InitializeAsync();

        // Authenticate by logging into an anonymous account
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        try
        {
            // Call the function within the module and provide the parameters we defined in there
            var module = new HelloWorldBindings(CloudCodeService.Instance);
            var result = await module.SayHello("World");

            Debug.Log(result);
        }
        catch (CloudCodeException exception)
        {
            Debug.LogException(exception);
        }
    }

// Update is called once per frame
void Update()
    {
        
    }
}
