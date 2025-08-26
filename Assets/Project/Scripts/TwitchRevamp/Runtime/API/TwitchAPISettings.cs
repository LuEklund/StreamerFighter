using UnityEngine;
namespace TwitchRevamp.API {
    public class TwitchAPISettings : ScriptableObject {
        public const string INITIAL_CLIENT_ID = "Go to dev.twitch.tv to get a client-id";

        public const string SETTINGS_PATH = "Project/TwitchAPISettings";

        [SerializeField]
        public string m_clientId = "";
        [SerializeField]
        public bool m_useEventSubProxy = false;

        static TwitchAPISettings s_instance;

        public static TwitchAPISettings Instance {
            get {
                s_instance = NullableInstance;
                if ( s_instance == null ) {
                    s_instance = CreateInstance<TwitchAPISettings>();
                }

                return s_instance;
            }

            set => s_instance = value;
        }
        public static TwitchAPISettings NullableInstance {
            get {
                if ( s_instance == null ) {
                    s_instance = Resources.Load( nameof(TwitchAPISettings) ) as TwitchAPISettings;
                }

                return s_instance;
            }
        }
    }
}