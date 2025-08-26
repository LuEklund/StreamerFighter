using System;
using Newtonsoft.Json;

namespace TwitchRevamp.Models {
    [Serializable]
    public class ChannelFollowEvent {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        
        [JsonProperty("user_login")]
        public string UserLogin { get; set; }
        
        [JsonProperty("user_name")]
        public string UserName { get; set; }
        
        [JsonProperty("broadcaster_user_id")]
        public string BroadcasterUserID { get; set; }
        
        [JsonProperty("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; set; }
        
        [JsonProperty("broadcaster_user_name")]
        public string BroadcasterUserName { get; set; }
        
        [JsonProperty("followed_at")]
        public string FollowedAt { get; set; }
    }
}