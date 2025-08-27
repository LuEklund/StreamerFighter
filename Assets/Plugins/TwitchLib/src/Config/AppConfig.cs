// using System;
// using System.IO;
// using Newtonsoft.Json;
//
// namespace TwitchSharp.Config
// {
//     public class AppConfig {
//         public int MessagesAllowedInPeriod { get; set; } = 750;
//         public int ThrottlingPeriodSeconds { get; set; } = 30;
//         public string LogLevel { get; set; } = "Info";
//         public bool AutoJoinChannel { get; set; } = true;
//         public string DefaultChannel { get; set; } = string.Empty;
//
//         public static AppConfig LoadFromFile(string filePath = "config.json") {
//             if ( !File.Exists( filePath ) ) {
//                 var defaultConfig = new AppConfig();
//                 defaultConfig.SaveToFile( filePath );
//                 return defaultConfig;
//             }
//
//             string json = File.ReadAllText( filePath );
//             var config = JsonSerializer.Deserialize<AppConfig>( json );
//
//             if ( config == null ) {
//                 throw new InvalidOperationException( "Failed to deserialize config file" );
//             }
//
//             return config;
//         }
//
//         public void SaveToFile(string filePath = "config.json") {
//             var options = new JsonSerializerOptions {
//                 WriteIndented = true,
//             };
//             string json = JsonSerializer.Serialize( this, options );
//             File.WriteAllText( filePath, json );
//         }
//     }
// }