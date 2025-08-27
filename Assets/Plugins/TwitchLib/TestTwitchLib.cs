using TwitchLib.Client.Events;
using TwitchSharp;
using UnityEngine;

public class TestTwitchLib : MonoBehaviour {
    TwitchUserBot bot;

    void Awake() {
        bot = TwitchBotFactory.CreateForUnity(
            username: "your_twitch_username",
            accessToken: "access_token_here", // Get this from https://twitchtokengenerator.com/
            defaultChannel: "", // normally your username with a # prefix
            true // turn on logging
        );
    }

    void Start() {
        SendChatMessage("Crack Cocaine");
        bot.OnLog += SendLogMessage;
    }
    void OnDestroy() {
        bot.OnLog -= SendLogMessage;
    }
    
    void SendLogMessage(object sender, OnLogArgs e) {
        if (sender == null) return;
        Debug.Log( e.Data );
    }
    void SendChatMessage(string message = "Hello from Unity!") {
        bot.SendMessageToChannel( message );
    }
}