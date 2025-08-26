using System;
using System.Collections;
using System.Linq;
using TCS.Utils;
using TwitchRevamp.API;
using TwitchRevamp.Utils;
using TwitchSDK;
using TwitchSDK.Interop;
using UnityEngine;
using UnityEngine.Networking;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp {
    public static class TwitchScopes {
        // EventSub-specific scopes not in SDK enums
        public static TwitchOAuthScope Followers => new( "moderator:read:followers" );
        /// <summary>
        /// View a list of all subscribers to a channel and check if a user is subscribed to a channel.
        /// API
        /// Get Broadcaster Subscriptions
        /// EventSub
        /// Channel Subscribe
        /// Channel Subscription End
        /// Channel Subscription Gift
        /// Channel Subscription Message
        /// </summary>
        public static TwitchOAuthScope Subscriptions => new( "channel:read:subscriptions" );
        public static TwitchOAuthScope UserReadChat => new( "moderator:read:chat_messages" );
        public static TwitchOAuthScope ChannelBot => new( "channel:bot" );
        public static TwitchOAuthScope WriteChat => new( "user:write:chat" );
        public static TwitchOAuthScope ReadChat => new( "user:read:chat" );
    }

    public class TwitchAuthManager : MonoBehaviour {
        // IMPORTANT: ONLY for scopes you need. Apparently asking for the wrong ones can get you into trouble.
        // TODO: review these scopes and remove any you don't need,
        // TODO: figure out how to add scopes to our Http requests
        // Current Scopes to get i ask the https://id.twitch.tv/oauth2/authorize
        // you need to Re-authorize if you change these Any Scope (Add or Remove)
        // find all scopes here https://dev.twitch.tv/docs/authentication/scopes/#twitch-access-token-scopes
        static readonly TwitchOAuthScope RequiredScopes = new(
            string.Join(
                " ", 
                TwitchOAuthScope.Channel.ManagePolls.Scope,
                TwitchOAuthScope.Channel.ReadHypeTrain.Scope,
                TwitchScopes.Followers.Scope,
                TwitchScopes.Subscriptions.Scope,
                TwitchScopes.ChannelBot.Scope,
                TwitchScopes.UserReadChat.Scope
            )
        );
        
        GameTask<AuthenticationInfo> m_authInfoTask;
        GameTask<AuthState> m_authStateTask;

        void Start() => UpdateAuthState();

        void PromptLogin() {
            // Set scopes before calling GetAuthenticationInfo
            // so the auth info is generated with the correct scopes
            m_authInfoTask ??= TwitchAPI.API.GetAuthenticationInfo( RequiredScopes );
            TwitchAPIUtils.GetTwitchAuthUrl();
        }

        public void UpdateAuthState() {
            if (TwitchAPI.API == null) {
                Logger.Log("TwitchAPI API is not available");
                return;
            }
            m_authStateTask = TwitchAPI.API.GetAuthState();
            var state = m_authStateTask?.MaybeResult;
            if ( state == null ) {
                return;
            }

            switch (state.Status) {
                case AuthStatus.LoggedIn:
                    Logger.Log( "TwitchAPI: logged in" );
                    LogTokenInfo();
                    break;

                case AuthStatus.LoggedOut:
                    PromptLogin();
                    Logger.Log( "TwitchAPI: logged out" );
                    // Optionally call PromptLogin() here
                    break;

                case AuthStatus.WaitingForCode:
                    var info = m_authInfoTask?.MaybeResult;
                    if ( info == null ) {
                        return; // still loading
                    }

                    Logger.Log( $"Authorize at: {info.Uri} with code: {info.UserCode}" );
                    PromptLogin();
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
        
        [Button] public void LogTokenInfo() {
            var state = m_authStateTask?.MaybeResult;
            if (state == null) {
                Logger.Log("No auth state available");
                return;
            }
            
            Logger.Log($"Auth Status: {state.Status}");
            if (state.Scopes != null && state.Scopes.Length > 0) {
                Logger.Log($"Current Scopes: [{string.Join(", ", state.Scopes)}]");
            } else {
                Logger.Log("No scopes available");
            }
        }

    }
}