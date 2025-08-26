using System.Collections.Generic;
using TwitchSDK.Interop;
using UnityEngine;
namespace TwitchRevamp {
    public class TwitchChannelPointManager : MonoBehaviour {
        public void SetSampleRewards() {
            List<CustomRewardDefinition> list = new() {
                new CustomRewardDefinition { Title = "Red!", Cost = 10 },
                new CustomRewardDefinition { Title = "Blue!", Cost = 25 },
                new CustomRewardDefinition { Title = "Octarine!", Cost = 750 },
            };
            Twitch.API.ReplaceCustomRewards( list.ToArray() );
        }

        public void ClearRewards() => Twitch.API.ReplaceCustomRewards();
    }
}