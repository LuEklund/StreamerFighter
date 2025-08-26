using System.Collections.Generic;
using UnityEngine;

using TwitchSDK;
using TwitchSDK.Interop;

public class AuthManager : MonoBehaviour
{
    GameTask<AuthenticationInfo> authInfoTask;
    GameTask<AuthState> curAuthState;
    
    GameTask<Poll> ourPoll;
    
    GameTask<StreamInfo> StreamInfoTask;
    string GameName;
    bool isMature;
    string Language;
    string Title;
    long ViewerCount;
	
   
    void Start()
    {
        UpdateAuthState();
    }
	
    void Update()
    {


    }
    
    public void UpdateAuthState()
    {
        curAuthState = Twitch.API.GetAuthState();
        switch (curAuthState.MaybeResult.Status)
        {
            
            case AuthStatus.LoggedIn:
                Debug.Log("✅ User is logged in");
                break;

            case AuthStatus.LoggedOut:
                GetAuthInformation();
                Debug.Log("❌ User is logged out");
                break;

            case AuthStatus.WaitingForCode:
                var userAuthInfo = Twitch.API.GetAuthenticationInfo().MaybeResult;
                if (userAuthInfo != null)
                {
                    Debug.Log("🔑 Asking user to log in...");
                    Application.OpenURL($"{userAuthInfo.Uri}?user_code={userAuthInfo.UserCode}");
                }
                break;
        } 
    }

    // Triggered by login button
    public void GetAuthInformation()
    {
        if (authInfoTask == null)
        {
            var scopes = TwitchOAuthScope.Bits.Read.Scope 
                         + " " + TwitchOAuthScope.Channel.ManageBroadcast.Scope 
                         + " " + TwitchOAuthScope.Channel.ManagePolls.Scope 
                         + " " + TwitchOAuthScope.Channel.ManagePredictions.Scope 
                         + " " + TwitchOAuthScope.Channel.ManageRedemptions.Scope 
                         + " " + TwitchOAuthScope.Channel.ReadHypeTrain.Scope 
                         + " " + TwitchOAuthScope.Clips.Edit.Scope 
                         + " " + TwitchOAuthScope.User.ReadSubscriptions.Scope 
                         + " " + TwitchOAuthScope.Channel.ManageBroadcast;
            TwitchOAuthScope tscopes = new TwitchOAuthScope(scopes);
            authInfoTask = Twitch.API.GetAuthenticationInfo(tscopes);
            Debug.Log("DONE");
            GetStreamInformation();
        }
        else
        {
            UpdateAuthState();
        }
        ourPoll = Twitch.API.NewPoll(new PollDefinition { Title = "some title", Choices = new string[] { "a", "b", "c" }, Duration = 20});
        var PollResult = ourPoll?.MaybeResult;
    } 
    
   
	
  
	
    public void GetStreamInformation()
    {
        Debug.Log("0");
        if(StreamInfoTask == null)
        {
            Debug.Log("1");
            StreamInfoTask = Twitch.API.GetMyStreamInfo();
            if(StreamInfoTask?.IsCompleted == true)
            {
                Debug.Log("2");
                if(StreamInfoTask.MaybeResult != null)
                {
                    Debug.Log("3");
                    GameName = "Game Name: " + StreamInfoTask.MaybeResult.GameName;
                    isMature = StreamInfoTask.MaybeResult.IsMature;
                    //DisplayName = "Language: " + StreamInfoTask.MaybeResult.Language;
                    Title = "Title: " + StreamInfoTask.MaybeResult.Title;
                    ViewerCount = StreamInfoTask.MaybeResult.ViewerCount;
                }
            }
        }
    } 
}