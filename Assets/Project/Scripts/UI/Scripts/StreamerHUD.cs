using StreamAPI;
using UnityEngine;
using UnityEngine.UIElements;
namespace UI {
    public class StreamerHUD : MonoBehaviour {
        [SerializeField] UIDocument m_doc;
        [SerializeField] TwitchChatter m_twitchChatter;
    
        TwitchStreamInfo m_twitchStreamInfo;

        public void Awake() {
            if ( m_doc == null ) {
                Debug.LogError( "StreamerHUD: No UIDocument assigned!", this );
                enabled = false;
                return;
            }
            
            if ( m_twitchChatter == null ) {
                m_twitchChatter = FindFirstObjectByType<TwitchChatter>( FindObjectsInactive.Exclude );
            }
            
            if ( m_twitchChatter == null ) {
                Debug.LogError( "StreamerHUD: No TwitchChatter assigned!", this );
                enabled = false;
                return;
            }
        }

        void SetTwitchInfo(string username, string accessToken, string channelName, bool enableLogging) {
            m_twitchStreamInfo = new TwitchStreamInfo {
                m_userName = username,
                m_accessToken = accessToken,
                m_channelName = channelName,
                m_enableLogging = enableLogging
            };
            
            m_twitchChatter.Init( m_twitchStreamInfo );
        }
    }
}