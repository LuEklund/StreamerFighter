using System;
using StreamerFighter;
using TCS.Utils;
using TwitchLib.Client.Events;
using TwitchSharp;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace StreamAPI {
    [Serializable] public class TwitchStreamInfo {
        public string m_userName;
        public string m_accessToken; // Get this from https://twitchtokengenerator.com/quick/jX8swSi9rN DONT SHOW TO ANYONE
        public string m_channelName = ""; // normally your username with a # prefix
        public bool m_enableLogging = true;
    }

    public class TwitchChatter : MonoBehaviour {
        public GameManager m_gameManager;
        
        public bool m_logChatterInfo = true;
        
        TwitchStreamInfo m_twitchStreamInfo = new();
        TwitchUserBot m_bot;

        #region Class Construction
        void Awake() {
            if ( m_gameManager == null ) {
                m_gameManager = FindFirstObjectByType<GameManager>( FindObjectsInactive.Exclude );
            }

            if ( m_gameManager == null ) {
                Logger.LogError( "TwitchChatter: No GameManager assigned!", this );
            }
        }

        // UI Construction
        public void Init(TwitchStreamInfo twitchStreamInfo) {
            m_twitchStreamInfo = twitchStreamInfo;
            Init();
        }
        
        void Init() {
            m_bot = TwitchBotFactory.CreateForUnity(
                m_twitchStreamInfo.m_userName,
                m_twitchStreamInfo.m_accessToken, 
                m_twitchStreamInfo.m_channelName, // normally your username with a # prefix
                m_twitchStreamInfo.m_enableLogging // turn on logging
            );
            m_bot.OnLog += SendLogMessage;
            m_bot.OnMessageReceived += LogMessageReceived;
        }
        
        void OnDestroy() {
            if (m_bot != null) {
                m_bot.OnLog -= SendLogMessage;
                m_bot.OnMessageReceived -= LogMessageReceived;
            }
        }
        #endregion
    
        void LogMessageReceived(object sender, OnMessageReceivedArgs e) {
            if (sender == null) {
                return;
            }

            if ( m_logChatterInfo ) {
                Logger.Log( $"[{e.ChatMessage.Channel}] {e.ChatMessage.Username}: {e.ChatMessage.Message}" );
            }

            // This still adds the player if they don't exist, otherwise just adds the chat message
            // the if statement is just for logging purposes we want the other line below.
            if (!m_gameManager.TryAddPlayer(e.ChatMessage.Username)) { // if player already exists, just add chat
                if ( m_logChatterInfo ) {
                    Logger.Log( $"Player {e.ChatMessage.Username} already exists." );
                }
            }
            
            //gameManager.TryAddPlayer(e.ChatMessage.Username) // we just want this line for production
            // Hello from Twitch!
            m_gameManager.TryAddChat( e.ChatMessage.Username, e.ChatMessage.Message );
        }
    
        void SendLogMessage(object sender, OnLogArgs e) {
            if (sender == null) {
                return;
            }

            if ( m_logChatterInfo ) {
                Logger.Log( e.Data );
            }
        }

        [Button] public void SendTestMessage() => SendChatMessage( "Test Message" );
        void SendChatMessage(string message) => m_bot.SendMessageToChannel( message );
    }
}