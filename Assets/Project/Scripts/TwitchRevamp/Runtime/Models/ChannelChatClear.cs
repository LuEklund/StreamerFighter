using System;
namespace TwitchRevamp.Models {
    [Serializable] public class ChannelChatClearRequest {
        public string Type { get; set; }
        public string Version { get; set; }
        public Condition Condition { get; set; }
        public Transport Transport { get; set; }
    }

    [Serializable] public class Condition {
        public string BroadcasterUserID { get; set; }
        public string UserID { get; set; }
    }

    [Serializable] public class Transport {
        public string Method { get; set; }
        public string Callback { get; set; }
        public string Secret { get; set; }
    }

    [Serializable] public class ChannelChatClearResponse {
        // This endpoint typically returns an empty response body or subscription details
    }
}