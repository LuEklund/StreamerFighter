using Plugins;
using TwitchSDK;
using TwitchSDK.Interop;
using UnityEngine;
namespace TwitchRevamp {
    public class AuthManager : MonoBehaviour {
        GameTask<AuthenticationInfo> m_authInfoTask;
        GameTask<AuthState> m_authState;

        void Start() { UpdateAuthState(); }

        void UpdateAuthState() {
            m_authState = Twitch.API.GetAuthState();
            var state = m_authState.MaybeResult;
            if ( state == null ) return;

            if ( state.Status == AuthStatus.WaitingForCode ) {
                // request scopes you actually need:
                var infoTask = Twitch.API.GetAuthenticationInfo(
                    TwitchOAuthScope.Channel.ManagePolls,
                    TwitchOAuthScope.Channel.ManagePredictions,
                    TwitchOAuthScope.Channel.ManageRedemptions
                );
                var info = infoTask.MaybeResult;
                if ( info != null ) {
                    Application.OpenURL( info.Uri + info.UserCode );
                }
            }
        }
    }
}