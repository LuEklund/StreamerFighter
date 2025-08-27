#nullable enable
using System;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;
using TwitchSharp.Interfaces;
namespace TwitchSharp
{
    public class TwitchUserBot {
        readonly string m_channel;
        readonly TwitchClient m_client;
        readonly ITwitchConfiguration m_configuration;
        readonly ITwitchLogger m_logger;
        readonly bool m_logDebug;

        public event EventHandler<OnLogArgs>? OnLog;
        public event EventHandler<OnJoinedChannelArgs>? OnJoinedChannel;
        public event EventHandler<OnMessageReceivedArgs>? OnMessageReceived;
        public event EventHandler<OnWhisperReceivedArgs>? OnWhisperReceived;
        public event EventHandler<OnNewSubscriberArgs>? OnNewSubscriber;
        public event EventHandler<OnConnectedArgs>? OnConnected;

        public TwitchUserBot(ITwitchConfiguration configuration, ITwitchLogger logger) {
            m_configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
            m_channel = string.IsNullOrEmpty(m_configuration.DefaultChannel) ? $"#{m_configuration.Username}" : m_configuration.DefaultChannel;
            m_logDebug = m_configuration.LogLevel.Equals("INFO", StringComparison.OrdinalIgnoreCase);

            var credentials = new ConnectionCredentials(m_configuration.Username, m_configuration.AccessToken);
            var clientOptions = new ClientOptions {
                MessagesAllowedInPeriod = m_configuration.MessagesAllowedInPeriod,
                ThrottlingPeriod = TimeSpan.FromSeconds(m_configuration.ThrottlingPeriodSeconds),
            };
            var customClient = new WebSocketClient(clientOptions);
            m_client = new TwitchClient(customClient);
            m_client.Initialize(credentials, m_channel);

            if (m_logDebug) {
                m_client.OnLog += Client_OnLog;
            }

            m_client.OnJoinedChannel += Client_OnJoinedChannel;
            m_client.OnMessageReceived += Client_OnMessageReceived;
            m_client.OnWhisperReceived += Client_OnWhisperReceived;
            m_client.OnNewSubscriber += Client_OnNewSubscriber;
            m_client.OnConnected += Client_OnConnected;

            m_client.Connect();
        }

        void Client_OnLog(object? sender, OnLogArgs e) {
            if (sender == null) {
                return;
            }

            m_logger.LogTwitchEvent(e);
            OnLog?.Invoke(this, e);
        }

        void Client_OnConnected(object? sender, OnConnectedArgs e) {
            if (sender == null) {
                return;
            }

            m_logger.LogInfo($"Connected to {e.AutoJoinChannel}");
            OnConnected?.Invoke(this, e);
        }

        void Client_OnJoinedChannel(object? sender, OnJoinedChannelArgs e) {
            if (sender == null) {
                return;
            }

            m_logger.LogInfo("Bot joined channel successfully");
            OnJoinedChannel?.Invoke(this, e);
        }

        void Client_OnMessageReceived(object? sender, OnMessageReceivedArgs e) {
            if (sender == null) {
                return;
            }

            m_logger.LogDebug($"Message received: {e.ChatMessage.Message}");
            OnMessageReceived?.Invoke(this, e);
        }

        void Client_OnWhisperReceived(object? sender, OnWhisperReceivedArgs e) {
            if (sender == null) {
                return;
            }

            m_logger.LogDebug($"Whisper received from {e.WhisperMessage.Username}: {e.WhisperMessage.Message}");
            OnWhisperReceived?.Invoke(this, e);
        }

        void Client_OnNewSubscriber(object? sender, OnNewSubscriberArgs e) {
            if (sender == null) {
                return;
            }

            m_logger.LogInfo($"New subscriber: {e.Subscriber.DisplayName}");
            OnNewSubscriber?.Invoke(this, e);
        }

        public void SendMessageToChannel(string message) => m_client.SendMessage(m_channel, message);
    }
}