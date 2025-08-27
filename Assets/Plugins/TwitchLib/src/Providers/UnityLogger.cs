using TwitchLib.Client.Events;
using TwitchSharp.Interfaces;

namespace TwitchSharp.Providers
{
    public class UnityLogger : ITwitchLogger {
        public void LogInfo(string message) {
#if UNITY_2019_1_OR_NEWER
            UnityEngine.Debug.Log($"[TwitchBot INFO] {message}");
#else
        Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
#endif
        }

        public void LogDebug(string message) {
#if UNITY_2019_1_OR_NEWER
            UnityEngine.Debug.Log($"[TwitchBot DEBUG] {message}");
#else
        Console.WriteLine($"[DEBUG] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
#endif
        }

        public void LogError(string message) {
#if UNITY_2019_1_OR_NEWER
            UnityEngine.Debug.LogError($"[TwitchBot ERROR] {message}");
#else
        Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
#endif
        }

        public void LogWarning(string message) {
#if UNITY_2019_1_OR_NEWER
            UnityEngine.Debug.LogWarning($"[TwitchBot WARNING] {message}");
#else
        Console.WriteLine($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
#endif
        }

        public void LogTwitchEvent(OnLogArgs args) {
#if UNITY_2019_1_OR_NEWER
            UnityEngine.Debug.Log($"[TwitchLib] {args.DateTime}: {args.BotUsername} - {args.Data}");
#else
        Console.WriteLine($"[TwitchLib] {args.DateTime}: {args.BotUsername} - {args.Data}");
#endif
        }
    }
}