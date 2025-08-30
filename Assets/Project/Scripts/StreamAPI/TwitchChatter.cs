using StreamerFighter;
using TCS.Utils;
using TwitchLib.Client.Events;
using TwitchSharp;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace StreamAPI {
    public class TwitchStreamInfo {
        public string m_userName;
        public string m_accessToken; // Get this from https://twitchtokengenerator.com/ DONT SHOW TO ANYONE
        public string m_channelName = ""; // normally your username with a # prefix
        public bool m_enableLogging = true;
    }

    public class TwitchChatter : MonoBehaviour {
        public GameManager gameManager;
        public GameObject m_player;
        
        public bool m_logChatterInfo = true;

        [TextArea]
        public string m_testMessage = "Test Message";
    
        TwitchUserBot m_bot;
        readonly TwitchStreamInfo m_twitchStreamInfo = new();

        #region Class Construction
        void Awake() {
            var load = Resources.Load("Lucas/secrets") as TextAsset;
            if (load == null) {
                Logger.LogError("Failed to load secrets.json from Resources/Damon/");
                return;
            }
            Logger.Log(load );
            var secrets = JsonUtility.FromJson<UnitySecrets>(load.ToString());
            m_twitchStreamInfo.m_userName = secrets.USERNAME;
            m_twitchStreamInfo.m_accessToken = secrets.ACCESS_TOKEN;
        
        
            m_bot = TwitchBotFactory.CreateForUnity(
                m_twitchStreamInfo.m_userName, m_twitchStreamInfo.m_accessToken, m_twitchStreamInfo.m_channelName, // normally your username with a # prefix
                m_twitchStreamInfo.m_enableLogging // turn on logging
            );
        }

        void Start() => Init();

        void Init() {
            m_bot.OnLog += SendLogMessage;
            m_bot.OnMessageReceived += LogMessageReceived;
            // SendChatMessage("Crack Cocaine");
        }
        
        void OnDestroy() {
            if (m_bot != null) {
                m_bot.OnLog -= SendLogMessage;
                m_bot.OnMessageReceived -= LogMessageReceived;
            }
        }
        #endregion
    
        void LogMessageReceived(object sender, OnMessageReceivedArgs e) {
            if (sender == null) return;
            if ( m_logChatterInfo ) {
                Logger.Log( $"[{e.ChatMessage.Channel}] {e.ChatMessage.Username}: {e.ChatMessage.Message}" );
            }

            // This still adds the player if they don't exist, otherwise just adds the chat message
            // the if statement is just for logging purposes we want the other line below.
            if (!gameManager.TryAddPlayer(e.ChatMessage.Username)) { // if player already exists, just add chat
                if ( m_logChatterInfo ) {
                    Logger.Log( $"Player {e.ChatMessage.Username} already exists." );
                }
            }
            
            //gameManager.TryAddPlayer(e.ChatMessage.Username) // we just want this line for production
            // Hello from Twitch!
            gameManager.TryAddChat( e.ChatMessage.Username, e.ChatMessage.Message );
        }
    
        void SendLogMessage(object sender, OnLogArgs e) {
            if (sender == null) return;
            if ( m_logChatterInfo ) {
                Logger.Log( e.Data );
            }
        }

        [Button] public void SendTestMessage() => SendChatMessage( m_testMessage );
        void SendChatMessage(string message) => m_bot.SendMessageToChannel( message );
    }
}