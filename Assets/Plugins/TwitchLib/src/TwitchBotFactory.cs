using TwitchSharp.Interfaces;
using TwitchSharp.Providers;

namespace TwitchSharp
{
    public static class TwitchBotFactory {
        // public static TwitchUserBot CreateForCLI(string secretsPath = "secrets.json", string configPath = "config.json") {
        //     // var configuration = new FileBasedTwitchConfiguration(secretsPath, configPath);
        //     var logger = new ConsoleLogger();
        //     return new TwitchUserBot(configuration, logger);
        // }

        public static TwitchUserBot CreateForUnity(
            string username, 
            string accessToken, 
            string defaultChannel = "", 
            bool enableLogging = true) {
        
            var configuration = new UnityTwitchConfiguration(
                username: username,
                accessToken: accessToken,
                defaultChannel: defaultChannel
            );
        
            var logger = enableLogging ? (ITwitchLogger)new UnityLogger() : new NullLogger();
            return new TwitchUserBot(configuration, logger);
        }

        public static TwitchUserBot CreateCustom(ITwitchConfiguration configuration, ITwitchLogger logger) {
            return new TwitchUserBot(configuration, logger);
        }
    }
}