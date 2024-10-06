/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using System;


public class PlayfabFriendController : MonoBehaviour
{
    public static Action<List<FriendInfo>> OnFriendListUpdated = delegate { };
    private void Awake()
    {
        UIAddFriend.OnAddFriend += HandleAddPlayfabFriend;
    }


    private void OnDestroy()
    {
        UIAddFriend.OnAddFriend -= HandleAddPlayfabFriend;
    }

    private void Start()
    {

    }

    private void HandleAddPlayfabFriend(string name)
    {
        var request = new AddFriendRequest { FriendTitleDisplayName = name };
        PlayFabClientAPI.AddFriend(request, OnFriendAddedSuccess, OnFailure);
    }

    private void GetPlayfabFriends()
    {
        var request = new GetFriendsListRequest { IncludeSteamFriends = false, IncludeFacebookFriends = false, XboxToken = null };
        PlayFabClientAPI.GetFriendsList(request, OnFriendAddedSuccess, OnFailure);
    }

    private void OnFriendsAddedSuccess(AddFriendResult result)
    {
        GetPlayfabFriends();
    }

    private void OnFriendsListSuccess(GetFriendsListResult result)
    {   
        OnFriendListUpdated?.Invoke(result.Friends);
    }

    private void OnFailure(PlayFabError error)
    {
        Debug.Log($"Playfab Friend Error occured: {error.GeneratedErrorReport()}");
    }

*/