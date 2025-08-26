using System;
using TwitchSDK;
using UnityEngine;
namespace TwitchRevamp.API {
    public class TwitchAPI : MonoBehaviour {
        protected static readonly object Lock = new();

        protected static TwitchAPI Instance;

        public static TwitchSDKApi API {
            get {
                lock (Lock) {
                    if ( Instance && Instance.APIInstance != null ) {
                        return Instance.APIInstance;
                    }

                    try {
                        Instance = FindFirstObjectByType<TwitchAPI>( FindObjectsInactive.Include );
                    }
                    catch (UnityException e) when (e.HResult == -2147467261) {
                        throw new Exception( "The Twitch API can only be initialized on the main thread. Make sure the first invocation of Twitch.API happens in the Unity Main thread (e.g. the Start or Update method, and not a constructor)" );
                    }

                    if ( Instance && Instance.APIInstance != null ) {
                        Destroy( Instance.gameObject );
                    }

                    if ( Instance ) {
                        return Instance.APIInstance;
                    }

                    var singletonObject = new GameObject();
                    Instance = singletonObject.AddComponent<TwitchAPI>();
                    Instance.CreateInstance();
                    singletonObject.name = "TwitchApi (Singleton)";

                    // Make instance persistent.
                    DontDestroyOnLoad( singletonObject );

                    return Instance.APIInstance;
                }
            }
        }
        protected TwitchSDKApi APIInstance;

        public TwitchAPI() { }

        /*void Awake() {
            lock (Lock) {
                if ( Instance == null ) {
                    Instance = this;
                    CreateInstance();
                    DontDestroyOnLoad( gameObject );
                }
                else if ( Instance != this ) {
                    Destroy( gameObject );
                }
            }
        }*/

        void CreateInstance() {
            var settings = TwitchAPISettings.Instance;

            if ( settings.m_clientId == TwitchAPISettings.INITIAL_CLIENT_ID ) {
                Debug.LogError( "Twitch: No OAuth ClientId set. Please open the Twitch settings at Twitch->Edit Settings." );
            }

            APIInstance = new UnityTwitchAPI( settings.m_clientId, settings.m_useEventSubProxy );
            ((UnityTwitchAPI)APIInstance).InitializeInternally();
        }


        void OnApplicationQuit() {
            if ( APIInstance == null ) return;
            Debug.Log( "OnApplicationQuit Twitch API" );
            APIInstance.Dispose();
            APIInstance = null;
        }

        void OnDestroy() {
            if ( APIInstance == null ) return;
            Debug.Log( "OnDestroy Twitch API" );
            APIInstance.Dispose();
            APIInstance = null;
        }
    }
}