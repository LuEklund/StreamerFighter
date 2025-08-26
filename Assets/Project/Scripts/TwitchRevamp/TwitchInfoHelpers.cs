using Plugins;
using TwitchSDK;
using TwitchSDK.Interop;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp {
    public class TwitchInfoHelpers : MonoBehaviour {
        GameTask<StreamInfo> m_streamInfoTask;
        GameTask<UserInfo> m_userInfoTask;

        public void GetUserInformation() {
            if ( m_userInfoTask != null ) return;
            m_userInfoTask = Twitch.API.GetMyUserInfo();

            if ( m_userInfoTask?.IsCompleted == true && m_userInfoTask.MaybeResult != null ) {
                var u = m_userInfoTask.MaybeResult;
                Logger.Log( $"Login: {u.DisplayName} | BroadcasterType: {u.BroadcasterType} | ViewCount: {u.ViewCount}" );
            }
        }

        public void GetStreamInformation() {
            if ( m_streamInfoTask != null ) return;
            m_streamInfoTask = Twitch.API.GetMyStreamInfo();

            if ( m_streamInfoTask?.IsCompleted == true && m_streamInfoTask.MaybeResult != null ) {
                var s = m_streamInfoTask.MaybeResult;
                Logger.Log( $"Game: {s.GameName} | Language: {s.Language} | Title: {s.Title} | Viewers: {s.ViewerCount} | Mature: {s.IsMature}" );
            }
        }
    }
}