using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class BitsUsage {
        [JsonProperty( "user_id" )] public string UserId { get; set; }
        [JsonProperty( "user_login" )] public string UserLogin { get; set; }
        [JsonProperty( "user_name" )] public string UserName { get; set; }
        [JsonProperty( "bits_used" )] public int BitsUsed { get; set; }
        [JsonProperty( "total_bits_used" )] public int TotalBitsUsed { get; set; }
        [JsonProperty( "context" )] public string Context { get; set; }
        [JsonProperty( "message" )] public Message Message { get; set; }
        [JsonProperty( "badge_tier" )] public int BadgeTier { get; set; }
    }
}