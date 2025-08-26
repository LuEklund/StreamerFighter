using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Condition {
        [JsonProperty( "broadcaster_user_id" )] public string BroadcasterUserID { get; set; }
        [JsonProperty( "moderator_user_id" )] public string ModeratorUserID { get; set; }
    }
}