using System;
using System.Collections.Generic;
namespace TwitchRevamp.Models {
    [Serializable] public class Follower {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string UserLogin { get; set; }
        public string FollowedAt { get; set; }
    }

    [Serializable] public class FollowersResponse {
        public int Total { get; set; }
        public List<Follower> Data { get; set; }
        public Pagination Pagination { get; set; }
    }
}