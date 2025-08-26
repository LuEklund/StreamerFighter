using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchRevamp.Models {
    [Serializable]
    public class ChannelChatMessageEvent {
        [JsonProperty("broadcaster_user_id")]
        public string BroadcasterUserID { get; set; }
        
        [JsonProperty("broadcaster_user_login")]
        public string BroadcasterUserLogin { get; set; }
        
        [JsonProperty("broadcaster_user_name")]
        public string BroadcasterUserName { get; set; }
        
        [JsonProperty("chatter_user_id")]
        public string ChatterUserID { get; set; }
        
        [JsonProperty("chatter_user_login")]
        public string ChatterUserLogin { get; set; }
        
        [JsonProperty("chatter_user_name")]
        public string ChatterUserName { get; set; }
        
        [JsonProperty("message_id")]
        public string MessageID { get; set; }
        
        [JsonProperty("message")]
        public Message Message { get; set; }
        
        [JsonProperty("color")]
        public string Color { get; set; }
        
        [JsonProperty("badges")]
        public List<Badge> Badges { get; set; }
        
        [JsonProperty("message_type")]
        public string MessageType { get; set; }
        
        [JsonProperty("cheer")]
        public object Cheer { get; set; }
        
        [JsonProperty("reply")]
        public object Reply { get; set; }
        
        [JsonProperty("channel_points_custom_reward_id")]
        public string ChannelPointsCustomRewardID { get; set; }
        
        // Shared chat properties
        [JsonProperty("source_broadcaster_user_id")]
        public string SourceBroadcasterUserID { get; set; }
        
        [JsonProperty("source_broadcaster_user_login")]
        public string SourceBroadcasterUserLogin { get; set; }
        
        [JsonProperty("source_broadcaster_user_name")]
        public string SourceBroadcasterUserName { get; set; }
        
        [JsonProperty("source_message_id")]
        public string SourceMessageID { get; set; }
        
        [JsonProperty("source_badges")]
        public List<Badge> SourceBadges { get; set; }
        
        [JsonProperty("is_source_only")]
        public bool? IsSourceOnly { get; set; }
    }
}