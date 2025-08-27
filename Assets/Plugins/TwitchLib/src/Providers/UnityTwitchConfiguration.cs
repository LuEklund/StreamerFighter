using System;
using TwitchSharp.Interfaces;

namespace TwitchSharp.Providers
{
    public class UnityTwitchConfiguration : ITwitchConfiguration {
        public string Username { get; }
        public string AccessToken { get; }
        public string RefreshToken { get; }
        public string ClientId { get; }
        public string ClientSecret { get; }
        public string DefaultChannel { get; }
        public int MessagesAllowedInPeriod { get; }
        public int ThrottlingPeriodSeconds { get; }
        public string LogLevel { get; }
        public bool AutoJoinChannel { get; }

        public UnityTwitchConfiguration(
            string username,
            string accessToken,
            string refreshToken = "",
            string clientId = "",
            string clientSecret = "",
            string defaultChannel = "",
            int messagesAllowedInPeriod = 750,
            int throttlingPeriodSeconds = 30,
            string logLevel = "Info",
            bool autoJoinChannel = true) {
        
            Username = username ?? throw new ArgumentNullException(nameof(username));
            AccessToken = accessToken ?? throw new ArgumentNullException(nameof(accessToken));
            RefreshToken = refreshToken;
            ClientId = clientId;
            ClientSecret = clientSecret;
            DefaultChannel = defaultChannel;
            MessagesAllowedInPeriod = messagesAllowedInPeriod;
            ThrottlingPeriodSeconds = throttlingPeriodSeconds;
            LogLevel = logLevel;
            AutoJoinChannel = autoJoinChannel;
        }
    }
}