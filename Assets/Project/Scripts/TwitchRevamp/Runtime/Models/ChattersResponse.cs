using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchRevamp.Models {
    [Serializable]
    public class ChattersResponse {
        [JsonProperty("data")]
        public List<Chatter> Data { get; set; }
        
        [JsonProperty("pagination")]
        public Pagination Pagination { get; set; }
        
        [JsonProperty("total")]
        public int Total { get; set; }
    }
    
    [Serializable]
    public record Chatter {
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        
        [JsonProperty("user_login")]
        public string UserLogin { get; set; }
        
        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }
    
    [Serializable]
    public class Pagination {
        [JsonProperty("cursor")]
        public string Cursor { get; set; }
    }
}