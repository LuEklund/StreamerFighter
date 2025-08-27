using TwitchLib.Client.Events;
using TwitchSharp.Interfaces;

namespace TwitchSharp.Providers
{
    public class NullLogger : ITwitchLogger {
        public void LogInfo(string message) { }
        public void LogDebug(string message) { }
        public void LogError(string message) { }
        public void LogWarning(string message) { }
        public void LogTwitchEvent(OnLogArgs args) { }
    }
}