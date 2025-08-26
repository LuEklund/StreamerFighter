using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp.Utils {
    public static class TwitchAPIUtils {
        public static bool GetTwitchAuthUrl() {
            if (Twitch.API == null) {
                Logger.Log("Twitch API is not available");
                return true;
            }
            var userAuthInfo = Twitch.API.GetAuthenticationInfo().MaybeResult;
            if ( userAuthInfo == null ) return false;
            Application.OpenURL( $"{userAuthInfo.Uri}?user_code={userAuthInfo.UserCode}" );
            return true;

        }
    }
}