// using TwitchSharp.Config;
// using TwitchSharp.Interfaces;
//
// namespace TwitchSharp.Providers
// {
//     public class FileBasedTwitchConfiguration : ITwitchConfiguration {
//         readonly Secrets m_secrets;
//         // readonly AppConfig m_config;
//         //
//         // public FileBasedTwitchConfiguration(string secretsPath = "secrets.json", string configPath = "config.json") {
//         //     m_secrets = Secrets.LoadFromFile(secretsPath);
//         //     m_config = AppConfig.LoadFromFile(configPath);
//         // }
//
//         public string Username => m_secrets.Username;
//         public string AccessToken => m_secrets.AccessToken;
//         public string RefreshToken => m_secrets.RefreshToken;
//         public string ClientId => m_secrets.ClientId;
//         public string ClientSecret => m_secrets.ClientSecret;
//         public string DefaultChannel => m_config.DefaultChannel;
//         public int MessagesAllowedInPeriod => m_config.MessagesAllowedInPeriod;
//         public int ThrottlingPeriodSeconds => m_config.ThrottlingPeriodSeconds;
//         public string LogLevel => m_config.LogLevel;
//         public bool AutoJoinChannel => m_config.AutoJoinChannel;
//     }
// }