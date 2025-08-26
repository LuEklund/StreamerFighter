using TCS.Utils;
using TwitchRevamp.API;
using TwitchRevamp.Utils;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp {
    [DefaultExecutionOrder( -1000 )]
    public class TwitchManager : TwitchAPI {
        [SerializeField]
        TwitchAPISettings m_settings;

        UnityTwitchAPI m_twitchAPIAPI;
        TwitchManager() { }
        void Awake() => Init();

        void Init() {
            lock (Lock) {
                if ( Instance == null ) {
                    Instance = this;
                    CreateSettings();
                    DontDestroyOnLoad( gameObject );
                    Logger.Log( "TwitchManager initialized" );
                }
                else if ( Instance != this ) {
                    Destroy( gameObject );
                }
            }
        }

        void CreateSettings() {
            if ( !m_settings ) {
                return;
            }

            TwitchAPISettings.Instance = m_settings;

            m_twitchAPIAPI = new UnityTwitchAPI( m_settings.m_clientId, m_settings.m_useEventSubProxy );
            APIInstance = m_twitchAPIAPI;
            ((UnityTwitchAPI)APIInstance).InitializeInternally();
        }
        
        [Button] public void Authenticate() {
            if ( TwitchAPIUtils.GetTwitchAuthUrl() ) return;
            Logger.Log("m_twitchAPIAPI: Unable to get authentication info");
        }
    }
}