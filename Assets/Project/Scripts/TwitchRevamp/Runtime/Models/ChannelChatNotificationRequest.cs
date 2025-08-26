using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class ChannelChatNotificationRequest {
        [JsonProperty( "type" )] public string Type { get; set; }
        [JsonProperty( "version" )] public string Version { get; set; }
        [JsonProperty( "condition" )] public Condition Condition { get; set; }
        [JsonProperty( "transport" )] public Transport Transport { get; set; }
    }
}