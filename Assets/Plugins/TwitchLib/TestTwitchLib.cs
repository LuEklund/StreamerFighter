using TCS.Utils;
using TwitchLib.Client.Events;
using TwitchSharp;
using UnityEngine;

public class TestTwitchLib : MonoBehaviour {
    public string m_userName;
    public string m_accessToken; // Get this from https://twitchtokengenerator.com/ DONT SHOW TO ANYONE
    public string m_channelName = ""; // normally your username with a # prefix
    public bool m_enableLogging = true;

    [TextArea]
    public string m_testMessage = "Test Message";
    
    TwitchUserBot m_bot;

    void Awake() {
        m_bot = TwitchBotFactory.CreateForUnity(
            m_userName,
            m_accessToken,
            m_channelName, // normally your username with a # prefix
            m_enableLogging // turn on logging
        );
    }

    void Start() {
        Init();
    }
    [Button] public void Init() {
        m_bot.OnLog += SendLogMessage;
        m_bot.OnMessageReceived += LogMessageReceived;
        SendChatMessage("Crack Cocaine");
    }
    
    void LogMessageReceived(object sender, OnMessageReceivedArgs e) {
        if (sender == null) return;
        Debug.Log( $"[{e.ChatMessage.Channel}] {e.ChatMessage.Username}: {e.ChatMessage.Message}" );
    }
    
    void OnDestroy() {
        m_bot.OnLog -= SendLogMessage;
    }
    
    void SendLogMessage(object sender, OnLogArgs e) {
        if (sender == null) return;
        Debug.Log( e.Data );
    }
    
    void SendChatMessage(string message = "Hello from Unity!") {
        m_bot.SendMessageToChannel( message );
    }

    [Button] public void SendTestMessage() {
        SendChatMessage( m_testMessage );
    }
}