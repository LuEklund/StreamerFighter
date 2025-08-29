using TCS.Utils;
using TwitchLib.Client.Events;
using TwitchSharp;
using UnityEngine;

[System.Serializable]
public class UnitySecrets {
    public string USERNAME;
    public string ACCESS_TOKEN;
    public string REFRESH_TOKEN;
    public string CLIENT_ID;
    public string CLIENT_SECRET;
}

public class TestTwitchLib : MonoBehaviour {
    public string m_userName;
    string m_accessToken; // Get this from https://twitchtokengenerator.com/ DONT SHOW TO ANYONE
    public string m_channelName = ""; // normally your username with a # prefix
    public bool m_enableLogging = true;
    
    public bool m_usePathForConfig = false;
    public string m_secretsPath = "Damon"; // only used if m_use

    [TextArea]
    public string m_testMessage = "Test Message";
    
    TwitchUserBot m_bot;

    void Awake() {
        //Assets/_Damon/Resources/Damon/secrets.json
        
        if (m_usePathForConfig) {
            // just get the from the path no the factory
            var load = Resources.Load("Lucas/secrets") as TextAsset;
            if (load == null) {
                Debug.LogError("Failed to load secrets.json from Resources/Damon/");
                return;
            }
            Debug.Log(load );
            var secrets = JsonUtility.FromJson<UnitySecrets>(load.ToString());
            m_userName = secrets.USERNAME;
            m_accessToken = secrets.ACCESS_TOKEN;
        }
        
        
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
        // SendChatMessage("Crack Cocaine");
    }
    
    void LogMessageReceived(object sender, OnMessageReceivedArgs e) {
        if (sender == null) return;
        Debug.Log( $"[{e.ChatMessage.Channel}] {e.ChatMessage.Username}: {e.ChatMessage.Message}" );
    }
    
    void OnDestroy() {
        if (m_bot != null) {
            m_bot.OnLog -= SendLogMessage;
        }
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