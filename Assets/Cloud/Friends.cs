using UnityEngine;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Friends;
using System;
using System.Collections.Generic;

public class Friends : MonoBehaviour
{
    public IReadOnlyList<Unity.Services.Friends.Models.Relationship> friends { get; private set; }

    private void Start()
    {
        AuthenticationService.Instance.SignedIn += async () => await GetFriends();
        DisplayFriends();
    }

    //This task will be called when we log in.
    private async Task GetFriends()
    {
        // Initialize friends service
        await FriendsService.Instance.InitializeAsync();

        // Start using the Friends SDK functionalities.
        friends = FriendsService.Instance.Friends;
    }

    //temporary method.
    public void DisplayFriends()
    {
        print("No UI for friends yet.");
    }
}
