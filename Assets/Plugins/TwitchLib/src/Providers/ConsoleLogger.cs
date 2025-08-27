using System;
using System.Globalization;
using TwitchLib.Client.Events;
using TwitchSharp.Interfaces;

namespace TwitchSharp.Providers
{
    public class ConsoleLogger : ITwitchLogger {
        public void LogInfo(string message) {
            Console.WriteLine($"[INFO] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
        }

        public void LogDebug(string message) {
            Console.WriteLine($"[DEBUG] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
        }

        public void LogError(string message) {
            Console.WriteLine($"[ERROR] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
        }

        public void LogWarning(string message) {
            Console.WriteLine($"[WARNING] {DateTime.Now:yyyy-MM-dd HH:mm:ss}: {message}");
        }

        public void LogTwitchEvent(OnLogArgs args) {
            Console.WriteLine($"{args.DateTime.ToString(CultureInfo.InvariantCulture)}: {args.BotUsername} - {args.Data}");
        }
    }
}