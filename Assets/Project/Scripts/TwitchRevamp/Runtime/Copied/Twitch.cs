using System;
using TwitchSDK;
using UnityEngine;
namespace TwitchRevamp {
    public class Twitch : MonoBehaviour {
        protected static object Lock = new object();

        protected static Twitch _Twitch;

        public static TwitchSDKApi API {
            get {
                lock (Lock) {
                    if ( _Twitch != null && _Twitch.Instance != null )
                        return _Twitch.Instance;

                    try {
                        _Twitch = FindObjectOfType<Twitch>();
                    }
                    catch (UnityException e) when (e.HResult == -2147467261) {
                        throw new Exception( "The Twitch API can only be initialized on the main thread. Make sure the first invocation of Twitch.API happens in the Unity Main thread (e.g. the Start or Update method, and not a constructor)" );
                    }

                    if ( _Twitch != null && _Twitch.Instance != null )
                        Destroy( _Twitch.gameObject );

                    if ( _Twitch == null ) {
                        var singletonObject = new GameObject();
                        _Twitch = singletonObject.AddComponent<Twitch>();
                        _Twitch.CreateInstance();
                        singletonObject.name = "TwitchApi (Singleton)";

                        // Make instance persistent.
                        DontDestroyOnLoad( singletonObject );
                    }

                    return _Twitch.Instance;
                }
            }
        }
        protected TwitchSDKApi Instance;

        public Twitch() { }

        void Awake() {
            lock (Lock) {
                if ( _Twitch == null ) {
                    _Twitch = this;
                    CreateInstance();
                    DontDestroyOnLoad( gameObject );
                }
                else if ( _Twitch != this ) {
                    Destroy( gameObject );
                }
            }
        }

        public void CreateInstance() {
            var settings = TwitchSDKSettings.Instance;

            if ( settings.ClientId == TwitchSDKSettings.InitialClientId ) {
                Debug.LogError( "Twitch: No OAuth ClientId set. Please open the Twitch settings at Twitch->Edit Settings." );
            }

            Instance = new UnityTwitch( settings.ClientId, settings.UseEventSubProxy );
            ((UnityTwitch)Instance).InitializeInternally();
        }


        private void OnApplicationQuit() {
            if ( Instance != null ) {
                Debug.Log( "OnApplicationQuit Twitch API" );
                Instance.Dispose();
                Instance = null;
            }
        }

        private void OnDestroy() {
            if ( Instance != null ) {
                Debug.Log( "OnDestroy Twitch API" );
                Instance.Dispose();
                Instance = null;
            }
        }
    }
}