// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using TwitchLib.Api;
// using TwitchLib.Api.Services;
// using TwitchLib.Api.Services.Events;
// using TwitchLib.Api.Services.Events.LiveStreamMonitor;
// namespace TwitchSharp
// {
//     public class LiveMonitor {
//         TwitchAPI m_api;
//         LiveStreamMonitorService m_monitor;
//
//         public LiveMonitor(TwitchAPI api, LiveStreamMonitorService monitor) {
//             m_api = api;
//             m_monitor = monitor;
//             Task.Run( ConfigLiveMonitorAsync );
//         }
//
//         async Task ConfigLiveMonitorAsync() {
//             m_api = new TwitchAPI {
//                 Settings = {
//                     ClientId = "",
//                     AccessToken = "",
//                 },
//             };
//
//             m_monitor = new LiveStreamMonitorService( m_api );
//
//             List<string> lst = ["ID1", "ID2"];
//             m_monitor.SetChannelsById( lst );
//
//             m_monitor.OnStreamOnline += Monitor_OnStreamOnline;
//             m_monitor.OnStreamOffline += Monitor_OnStreamOffline;
//             m_monitor.OnStreamUpdate += Monitor_OnStreamUpdate;
//
//             m_monitor.OnServiceStarted += Monitor_OnServiceStarted;
//             m_monitor.OnChannelsSet += Monitor_OnChannelsSet;
//
//
//             m_monitor.Start(); //Keep at the end!
//
//             await Task.Delay( -1 );
//
//         }
//
//         void Monitor_OnStreamOnline(object? sender, OnStreamOnlineArgs e) {
//             throw new NotImplementedException();
//         }
//
//         void Monitor_OnStreamUpdate(object? sender, OnStreamUpdateArgs e) {
//             throw new NotImplementedException();
//         }
//
//         void Monitor_OnStreamOffline(object? sender, OnStreamOfflineArgs e) {
//             throw new NotImplementedException();
//         }
//
//         void Monitor_OnChannelsSet(object? sender, OnChannelsSetArgs e) {
//             throw new NotImplementedException();
//         }
//
//         void Monitor_OnServiceStarted(object? sender, OnServiceStartedArgs e) {
//             throw new NotImplementedException();
//         }
//     }
// }