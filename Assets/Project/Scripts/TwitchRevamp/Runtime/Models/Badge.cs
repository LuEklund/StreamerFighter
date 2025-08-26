using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Badge {
        [JsonProperty( "set_id" )] public string SetIDNum { get; set; }
        [JsonProperty( "id" )] public string ID { get; set; }
        [JsonProperty( "info" )] public string Info { get; set; }
    }
}