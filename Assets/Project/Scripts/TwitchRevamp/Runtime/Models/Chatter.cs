using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Chatter {
        [JsonProperty] public string UserID { get; set; }
        [JsonProperty] public string UserLogin { get; set; }
        [JsonProperty] public string UserName { get; set; }
    }

    [Serializable] public class Pagination {
        [JsonProperty] public string Cursor { get; set; }
    }

    [Serializable] public class ChattersResponse {
        [JsonProperty] public List<Chatter> Data { get; set; }
        [JsonProperty] public Pagination Pagination { get; set; }
        [JsonProperty] public int Total { get; set; }
    }
}