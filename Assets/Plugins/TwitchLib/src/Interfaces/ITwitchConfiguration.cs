namespace TwitchSharp.Interfaces
{
    public interface ITwitchConfiguration {
        string Username { get; }
        string AccessToken { get; }
        string RefreshToken { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string DefaultChannel { get; }
        int MessagesAllowedInPeriod { get; }
        int ThrottlingPeriodSeconds { get; }
        string LogLevel { get; }
        bool AutoJoinChannel { get; }
    }
}