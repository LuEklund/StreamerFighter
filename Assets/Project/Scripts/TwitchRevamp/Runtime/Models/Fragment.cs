#nullable enable
using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Fragment {
        [JsonProperty( "type" )] public string? Type { get; set; }
        [JsonProperty( "text" )] public string? Text { get; set; }
        [JsonProperty( "cheer" )] public string? Cheer { get; set; }
        [JsonProperty( "emote" )] public Emote? Emote { get; set; }
        [JsonProperty( "mention" )] public Mention? Mention { get; set; }
        // V2 properties
        [JsonProperty( "cheermote" )] public Cheermote? Cheermote { get; set; }
    }
}