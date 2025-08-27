using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace TwitchRevamp.Models {
    [Serializable]
    public class ChannelFollowersResponse {
        [JsonProperty("data")]
        public List<ChannelFollower> Data { get; set; }
       /* "pagination": {
    "cursor": "eyJiIjpudWxsLCJhIjp7Ik9mZnNldCI6Mn19"
  },*/
        // [JsonProperty("pagination")]
        // public Pagination Pagination { get; set; }
        
        [JsonProperty("total")]
        public int Total { get; set; }
    }
    
    [Serializable]
    public class ChannelFollower {
        [JsonProperty("followed_at")]
        public string FollowedAt { get; set; }
        
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        
        [JsonProperty("user_login")]
        public string UserLogin { get; set; }
        
        [JsonProperty("user_name")]
        public string UserName { get; set; }
    }

    // [Serializable]
    // public class Pagination {
    //     [JsonProperty("cursor")]
    //     public string Cursor { get; set; }
    // }
}