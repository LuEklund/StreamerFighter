using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Transport {
        [JsonProperty( "method" )] public string Method { get; set; }
        [JsonProperty( "callback" )] public string Callback { get; set; }
        [JsonProperty( "secret" )] public string Secret { get; set; }
        [JsonProperty( "session_id" )] public string SessionID { get; set; }
        [JsonProperty( "connected_at" )] public string ConnectedAt { get; set; }
        [JsonProperty( "disconnected_at" )] public string DisconnectedAt { get; set; }
    }
}