using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Emote {
        [JsonProperty( "id" )] public string ID { get; set; }
        [JsonProperty( "emote_set_id" )] public string EmoteSetID { get; set; }
        [JsonProperty( "owner_id" )] public int? OwnerID { get; set; }
        [JsonProperty( "format" )] public string Format { get; set; }
    }
}