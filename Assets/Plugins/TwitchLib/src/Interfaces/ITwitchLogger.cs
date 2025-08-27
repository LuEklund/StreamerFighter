using TwitchLib.Client.Events;

namespace TwitchSharp.Interfaces
{
    public interface ITwitchLogger {
        void LogInfo(string message);
        void LogDebug(string message);
        void LogError(string message);
        void LogWarning(string message);
        void LogTwitchEvent(OnLogArgs args);
    }
}