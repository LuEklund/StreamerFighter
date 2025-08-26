using TwitchRevamp.API;
using TwitchSDK;
using TwitchSDK.Interop;
using UnityEngine;
using Logger = TCS.Utils.Logger;
namespace TwitchRevamp {
    public class TwitchEventExamples : MonoBehaviour {
        GameTask<EventStream<ChannelFollowEvent>> m_followEvents;
        GameTask<EventStream<HypeTrainEvent>> m_hypeEvents;
        GameTask<EventStream<ChannelRaidEvent>> m_raidEvents;
        GameTask<EventStream<CustomRewardEvent>> m_rewardEvents;
        GameTask<EventStream<ChannelSubscribeEvent>> m_subEvents;

        void Start() {
            if (TwitchAPI.API == null) {
                Logger.Log("TwitchAPI API is not available");
                return;
            }
            
            m_rewardEvents = TwitchAPI.API.SubscribeToCustomRewardEvents();
            m_followEvents = TwitchAPI.API.SubscribeToChannelFollowEvents();
            m_subEvents = TwitchAPI.API.SubscribeToChannelSubscribeEvents();
            m_hypeEvents = TwitchAPI.API.SubscribeToHypeTrainEvents();
            m_raidEvents = TwitchAPI.API.SubscribeToChannelRaidEvents();
        }

        void Update() {
            if ( m_rewardEvents?.MaybeResult != null &&
                 m_rewardEvents.MaybeResult.TryGetNextEvent( out var reward ) && reward != null ) {
                Logger.Log( $"{reward.RedeemerName} redeemed {reward.CustomRewardTitle} for {reward.CustomRewardCost}" );
            }

            if ( m_followEvents?.MaybeResult != null &&
                 m_followEvents.MaybeResult.TryGetNextEvent( out var follow ) && follow != null ) {
                Logger.Log( $"{follow.UserDisplayName} is now following!" );
            }

            if ( m_subEvents?.MaybeResult != null &&
                 m_subEvents.MaybeResult.TryGetNextEvent( out var sub ) && sub != null ) {
                Logger.Log( $"{sub.UserDisplayName} subscribed!" );
            }

            if ( m_hypeEvents?.MaybeResult != null &&
                 m_hypeEvents.MaybeResult.TryGetNextEvent( out var hype ) && hype != null ) {
                Logger.Log( $"Hype Train level: {hype.Level}" );
            }

            if ( m_raidEvents?.MaybeResult != null &&
                 m_raidEvents.MaybeResult.TryGetNextEvent( out var raid ) && raid != null ) {
                Logger.Log( $"{raid.FromBroadcasterName} raided with {raid.Viewers} viewers!" );
            }
        }
    }
}