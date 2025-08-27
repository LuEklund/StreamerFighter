using System;
using System.IO;
using Newtonsoft.Json;

namespace TwitchSharp.Config
{
    public class Secrets {
        [JsonProperty("USERNAME")]
        public string Username { get; set; } = string.Empty;
    
        [JsonProperty("ACCESS_TOKEN")]
        public string AccessToken { get; set; } = string.Empty;
    
        [JsonProperty("REFRESH_TOKEN")]
        public string RefreshToken { get; set; } = string.Empty;
    
        [JsonProperty("CLIENT_ID")]
        public string ClientId { get; set; } = string.Empty;
    
        [JsonProperty("CLIENT_SECRET")]
        public string ClientSecret { get; set; } = string.Empty;

        // public static Secrets LoadFromFile(string filePath = "secrets.json") {
        //     if ( !File.Exists( filePath ) ) {
        //         throw new FileNotFoundException( $"Secrets file not found: {filePath}" );
        //     }
        //
        //     string json = File.ReadAllText( filePath );
        //     var secrets = JsonSerializer.Deserialize<Secrets>( json );
        //
        //     if ( secrets == null ) {
        //         throw new InvalidOperationException( "Failed to deserialize secrets file" );
        //     }
        //
        //     return secrets;
        // }
    }
}