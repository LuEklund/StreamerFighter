using System;
using System.Collections.Generic;
namespace TwitchRevamp.Models {
    [Serializable] public class ChannelUpdateRequest {
        public string GameID { get; set; }
        public string Title { get; set; }
        public string BroadcasterLanguage { get; set; }
        public List<string> Tags { get; set; }
        public List<ContentClassificationLabel> ContentClassificationLabels { get; set; }
        public bool? IsBrandedContent { get; set; }
    }

    [Serializable] public class ContentClassificationLabel {
        public string ID { get; set; }
        public bool IsEnabled { get; set; }
    }

    [Serializable] public class ChannelUpdateResponse {
        // This endpoint typically returns an empty response body
    }
}