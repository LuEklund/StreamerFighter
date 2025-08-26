using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Mention {
        [JsonProperty( "user_id" )] public string UserID { get; set; }
        [JsonProperty( "user_name" )] public string UserName { get; set; }
        [JsonProperty( "user_login" )] public string UserLogin { get; set; }
    }
}