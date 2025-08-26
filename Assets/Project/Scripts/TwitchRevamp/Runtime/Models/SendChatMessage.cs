using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchRevamp.Models {
    [Serializable]
    public class SendChatMessageRequest {
        [JsonProperty("broadcaster_id")]
        public string BroadcasterId { get; set; }
        
        [JsonProperty("sender_id")]
        public string SenderId { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
        
        [JsonProperty("reply_parent_message_id")]
        public string ReplyParentMessageId { get; set; }
        
        [JsonProperty("for_source_only")]
        public bool? ForSourceOnly { get; set; }
    }
    
    [Serializable]
    public class SendChatMessageResponse {
        [JsonProperty("data")]
        public List<SendChatMessageData> Data { get; set; }
    }
    
    [Serializable]
    public class SendChatMessageData {
        [JsonProperty("message_id")]
        public string MessageId { get; set; }
        
        [JsonProperty("is_sent")]
        public bool IsSent { get; set; }
        
        [JsonProperty("drop_reason")]
        public DropReason DropReason { get; set; }
    }
    
    [Serializable]
    public class DropReason {
        [JsonProperty("code")]
        public string Code { get; set; }
        
        [JsonProperty("message")]
        public string Message { get; set; }
    }
}