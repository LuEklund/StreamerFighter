using System;
using System.Linq;
using Plugins;
using TwitchSDK;
using TwitchSDK.Interop;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp {
    public class TwitchAuthManager : MonoBehaviour {
        // Ask ONLY for scopes you need. Example set:
        static readonly TwitchOAuthScope RequiredScopes = new(
            string.Join(
                " ", 
                TwitchOAuthScope.Channel.ManagePolls.Scope, 
                TwitchOAuthScope.Channel.ManagePredictions.Scope, 
                TwitchOAuthScope.Channel.ManageRedemptions.Scope, 
                TwitchOAuthScope.Clips.Edit.Scope, 
                TwitchOAuthScope.User.ReadSubscriptions.Scope
            )
        );
        GameTask<AuthenticationInfo> m_authInfoTask;
        GameTask<AuthState> m_authStateTask;

        void Start() => UpdateAuthState();

        // Hook this to a "Log in" button
        public void PromptLogin() {
            if (Twitch.API == null) {
                Logger.Log("Twitch API is not available");
                return;
            }
            
            if ( m_authInfoTask == null )
                m_authInfoTask = Twitch.API.GetAuthenticationInfo( RequiredScopes );
        }

        public void UpdateAuthState() {
            if (Twitch.API == null) {
                Logger.Log("Twitch API is not available");
                return;
            }
            
            m_authStateTask = Twitch.API.GetAuthState();
            var state = m_authStateTask?.MaybeResult;
            if ( state == null ) return;

            switch (state.Status) {
                case AuthStatus.LoggedIn:
                    Logger.Log( "Twitch: logged in" );
                    break;

                case AuthStatus.LoggedOut:
                    PromptLogin();
                    Logger.Log( "Twitch: logged out" );
                    // Optionally call PromptLogin() here
                    break;

                case AuthStatus.WaitingForCode:
                    var info = m_authInfoTask?.MaybeResult;
                    if ( info == null ) return; // still loading
                    Logger.Log( $"Authorize at: {info.Uri} with code: {info.UserCode}" );
                    // The SDK samples concatenate the code; if your SDK expects a query param, adapt accordingly.
                    Application.OpenURL( $"{info.Uri}{info.UserCode}" );
                    break;
                
                case AuthStatus.Loading:
                    // still loading
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public bool HasScope(string scope) =>
            m_authStateTask?.MaybeResult?.Scopes?.Any( s => s == scope ) == true;
    }
}