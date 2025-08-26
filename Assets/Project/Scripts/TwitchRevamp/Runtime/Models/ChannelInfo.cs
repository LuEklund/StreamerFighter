using System;
using System.Collections.Generic;
namespace TwitchRevamp.Models {
    [Serializable] public class ChannelInfo {
        public string BroadcasterID { get; set; }
        public string BroadcasterLogin { get; set; }
        public string BroadcasterName { get; set; }
        public string BroadcasterLanguage { get; set; }
        public string GameID { get; set; }
        public string GameName { get; set; }
        public string Title { get; set; }
        public int Delay { get; set; }
        public List<string> Tags { get; set; }
        public List<string> ContentClassificationLabels { get; set; }
        public bool IsBrandedContent { get; set; }
    }

    [Serializable] public class ChannelInfoResponse {
        public List<ChannelInfo> Data { get; set; }
    }
}