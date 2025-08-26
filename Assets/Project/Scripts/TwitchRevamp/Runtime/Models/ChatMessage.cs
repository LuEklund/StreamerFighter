using System;
using System.Collections.Generic;
namespace TwitchRevamp.Models {
    [Serializable] public class ChatMessageRequest {
        public string BroadcasterID { get; set; }
        public string SenderID { get; set; }
        public string Message { get; set; }
        public bool ForSourceOnly { get; set; }
    }

    [Serializable] public class ChatMessage {
        public string MessageID { get; set; }
        public bool IsSent { get; set; }
    }

    [Serializable] public class ChatMessageResponse {
        public List<ChatMessage> Data { get; set; }
    }
}