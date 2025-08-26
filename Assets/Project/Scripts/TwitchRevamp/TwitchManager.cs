using Plugins;
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
            if ( !m_settings ) return;

            TwitchSDKSettings.Instance = m_settings;

            TwitchAPI = new UnityTwitch( m_settings.ClientId, m_settings.UseEventSubProxy );
            Instance = TwitchAPI;
            ((UnityTwitch)Instance).InitializeInternally();
        }
        
        [ContextMenu("Authenticate")] // if you right-click the component in the inspector, you can select "Authenticate" to open the browser for auth
        public void Authenticate() {
            var userAuthInfo = API.GetAuthenticationInfo().MaybeResult;
            if ( userAuthInfo != null ) {
                Application.OpenURL( $"{userAuthInfo.Uri}?user_code={userAuthInfo.UserCode}" );
                return;
            }
            
            Logger.Log("Twitch: Unable to get authentication info");
        }
    }
}