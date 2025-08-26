using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Message {
        [JsonProperty( "text" )] public string Text { get; set; }
        [JsonProperty( "fragments" )] public List<Fragment> Fragments { get; set; }
    }
}