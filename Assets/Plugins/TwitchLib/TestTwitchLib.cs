using TCS.Utils;
using TwitchLib.Client.Events;
using TwitchSharp;
using UnityEngine;

public class TestTwitchLib : MonoBehaviour {
    public string m_userName;
    public string m_accessToken;

    [TextArea]
    public string m_testMessage = "Test Message";
    
    TwitchUserBot bot;

    void Awake() {
        bot = TwitchBotFactory.CreateForUnity(
            //username: "your_twitch_username",
            m_userName,
            //accessToken: "access_token_here", // Get this from https://twitchtokengenerator.com/ DONT SHOW TO ANYONE
            m_accessToken,
            defaultChannel: "", // normally your username with a # prefix
            true // turn on logging
        );
    }

    void Start() {
        bot.OnLog += SendLogMessage;
        SendChatMessage("Crack Cocaine");
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

    [Button] public void SendTestMessage() {
        SendChatMessage( m_testMessage );
    }
}