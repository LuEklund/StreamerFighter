using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Cheermote {
        [JsonProperty( "prefix" )] public string Prefix { get; set; }
        [JsonProperty( "bits" )] public int Bits { get; set; }
        [JsonProperty( "tier" )] public int Tier { get; set; }
    }
}