using System;
using TwitchLib.Api.Interfaces;
using TwitchLib.Api.Services;
using TwitchLib.Api.Services.Events;
using TwitchLib.Api.Services.Events.LiveStreamMonitor;
namespace TwitchLib.Unity {
    public class UnityLiveStreamMonitor : LiveStreamMonitorService {
        public UnityLiveStreamMonitor(ITwitchAPI api, int checkIntervalSeconds = 60, int maxStreamRequestCountPerRequest = 100) : base( api, checkIntervalSeconds, maxStreamRequestCountPerRequest ) {
            ThreadDispatcher.EnsureCreated();

            base.OnStreamOnline += (sender, e) => { ThreadDispatcher.Enqueue( () => OnStreamOnline?.Invoke( sender, e ) ); };
            base.OnStreamOffline += (sender, e) => { ThreadDispatcher.Enqueue( () => OnStreamOffline?.Invoke( sender, e ) ); };
            base.OnStreamUpdate += (sender, e) => { ThreadDispatcher.Enqueue( () => OnStreamUpdate?.Invoke( sender, e ) ); };
            base.OnServiceStarted += (sender, e) => { ThreadDispatcher.Enqueue( () => OnServiceStarted?.Invoke( sender, e ) ); };
            base.OnServiceStopped += (sender, e) => { ThreadDispatcher.Enqueue( () => OnServiceStopped?.Invoke( sender, e ) ); };
            base.OnChannelsSet += (sender, e) => { ThreadDispatcher.Enqueue( () => OnChannelsSet?.Invoke( sender, e ) ); };
        }

        #region EVENTS
        /// <summary>Event fires when Stream goes online</summary>
        public new event EventHandler<OnStreamOnlineArgs> OnStreamOnline;
        /// <summary>Event fires when Stream goes offline</summary>
        public new event EventHandler<OnStreamOfflineArgs> OnStreamOffline;
        /// <summary>Event fires when Stream gets updated</summary>
        public new event EventHandler<OnStreamUpdateArgs> OnStreamUpdate;
        /// <summary>Event fires when service stops.</summary>
        public new event EventHandler<OnServiceStartedArgs> OnServiceStarted;
        /// <summary>Event fires when service starts.</summary>
        public new event EventHandler<OnServiceStoppedArgs> OnServiceStopped;
        /// <summary>Event fires when channels to monitor are intitialized.</summary>
        public new event EventHandler<OnChannelsSetArgs> OnChannelsSet;
        #endregion
    }
}