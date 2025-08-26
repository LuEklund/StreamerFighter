using TwitchRevamp.API;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp.Utils {
    public static class TwitchAPIUtils {
        public static bool GetTwitchAuthUrl() {
            if (TwitchAPI.API == null) {
                Logger.Log("TwitchAPI API is not available");
                return true;
            }
            var userAuthInfo = TwitchAPI.API.GetAuthenticationInfo().MaybeResult;
            if ( userAuthInfo == null ) return false;
            Application.OpenURL( $"{userAuthInfo.Uri}?user_code={userAuthInfo.UserCode}" );
            return true;

        }
    }
}