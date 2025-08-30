using System;
using StreamAPI;
using UnityEngine.UIElements;

namespace UI {
    [UxmlElement] public partial class TwitchloginElement : VisualElement {
        public static readonly string ClassNameUSS = "twitchlogin-element";

        public static readonly string LoginPanelUSS = ClassNameUSS + "_login-panel"; // twitchlogin-element_login-panel 
        public static readonly string TwitchHeaderUSS = ClassNameUSS + "_twitch-header"; // twitchlogin-element_twitch-header 
        public static readonly string TwitchHeaderLabelUSS = ClassNameUSS + "_twitch-header-label"; // twitchlogin-element_twitch-header-label 
        public static readonly string AuthKeyUSS = ClassNameUSS + "_auth-key"; // twitchlogin-element_auth-key 
        public static readonly string UserNameUSS = ClassNameUSS + "_user-name"; // twitchlogin-element_user-name 
        public static readonly string ChannelNameUSS = ClassNameUSS + "_channel-name"; // twitchlogin-element_channel-name 
        public static readonly string EnableLoggingUSS = ClassNameUSS + "_enable-logging"; // twitchlogin-element_enable-logging 
        public static readonly string StartTwitchButtonUSS = ClassNameUSS + "_start-twitch-button"; // twitchlogin-element_start-twitch-button 

        #region UI Elements
        readonly VisualElement m_loginPanel = new() { name = "login-panel" };
        readonly VisualElement m_twitchHeader = new() { name = "TwitchHeader" };
        readonly Label m_twitchHeaderLabel = new() { name = "TwitchHeader_label" };
        readonly TextField m_authKey = new() { name = "AuthKey" };
        readonly TextField m_userName = new() { name = "UserName" };
        readonly TextField m_channelName = new() { name = "ChannelName" };
        readonly Toggle m_enableLogging = new() { name = "EnableLogging" };
        readonly Button m_startTwitchButton = new() { name = "StartTwitchButton" };
        #endregion
        
        public event Action OnStartTwitchButtonClicked {
            add => m_startTwitchButton.clicked += value;
            remove => m_startTwitchButton.clicked -= value;
        }
        
        public TwitchStreamInfo GetTwitchStreamInfo() {
            return new TwitchStreamInfo {
                m_accessToken = m_authKey.value,
                m_userName = m_userName.value,
                m_channelName = m_channelName.value,
                m_enableLogging = m_enableLogging.value,
            };
        }

        public TwitchloginElement() {
            SetElementClassNames();

            // Set Text Fields
            m_twitchHeaderLabel.text = "Twitch Login";
            m_authKey.label = "Auth Key";
            m_userName.label = "User Name";
            m_channelName.label = "Channel Name";
            m_enableLogging.label = "EnableLogging";
            m_startTwitchButton.text = "Start Twitch";

            // Build Hierarchy
            hierarchy.Add( m_loginPanel );
            m_loginPanel.Add( m_twitchHeader );
            m_twitchHeader.Add( m_twitchHeaderLabel );
            m_loginPanel.Add( m_authKey );
            m_loginPanel.Add( m_userName );
            m_loginPanel.Add( m_channelName );
            m_loginPanel.Add( m_enableLogging );
            m_loginPanel.Add( m_startTwitchButton );
        }

        public void SetElementClassNames() {
            AddToClassList( ClassNameUSS );
            m_loginPanel.AddToClassList( LoginPanelUSS );
            m_twitchHeader.AddToClassList( TwitchHeaderUSS );
            m_twitchHeaderLabel.AddToClassList( TwitchHeaderLabelUSS );
            m_authKey.AddToClassList( AuthKeyUSS );
            m_userName.AddToClassList( UserNameUSS );
            m_channelName.AddToClassList( ChannelNameUSS );
            m_enableLogging.AddToClassList( EnableLoggingUSS );
            m_startTwitchButton.AddToClassList( StartTwitchButtonUSS );
        }
    }
}