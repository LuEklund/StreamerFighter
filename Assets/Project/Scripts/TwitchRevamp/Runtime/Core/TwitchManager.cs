using TCS.Utils;
using TwitchRevamp.Utils;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp {
    [DefaultExecutionOrder( -1000 )]
    public class TwitchManager : Twitch {
        [SerializeField]
        TwitchSDKSettings m_settings;

        UnityTwitch TwitchAPI;
        TwitchManager() { }
        void Awake() => Init();

        void Init() {
            lock (Lock) {
                if ( _Twitch == null ) {
                    _Twitch = this;
                    CreateSettings();
                    DontDestroyOnLoad( gameObject );
                    Logger.Log( "TwitchManager initialized" );
                }
                else if ( _Twitch != this ) {
                    Destroy( gameObject );
                }
            }
        }

        void CreateSettings() {
            if ( !m_settings ) {
                return;
            }

            TwitchSDKSettings.Instance = m_settings;

            TwitchAPI = new UnityTwitch( m_settings.ClientId, m_settings.UseEventSubProxy );
            Instance = TwitchAPI;
            ((UnityTwitch)Instance).InitializeInternally();
        }
        
        [Button] public void Authenticate() {
            if ( TwitchAPIUtils.GetTwitchAuthUrl() ) return;
            Logger.Log("Twitch: Unable to get authentication info");
        }
    }
}