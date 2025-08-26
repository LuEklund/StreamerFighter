using System;
using System.Collections.Generic;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
    [Serializable] public class Condition {
        [JsonProperty( "broadcaster_user_id" )] public string BroadcasterUserID { get; set; }
        [JsonProperty( "moderator_user_id" )] public string ModeratorUserID { get; set; }
    }

    [Serializable] public class Transport {
        [JsonProperty( "method" )] public string Method { get; set; }
        [JsonProperty( "callback" )] public string Callback { get; set; }
        [JsonProperty( "secret" )] public string Secret { get; set; }
        [JsonProperty( "session_id" )] public string SessionID { get; set; }
        [JsonProperty( "connected_at" )] public string ConnectedAt { get; set; }
        [JsonProperty( "disconnected_at" )] public string DisconnectedAt { get; set; }
    }

    [Serializable] public class ChannelChatNotificationRequest {
        [JsonProperty( "type" )] public string Type { get; set; }
        [JsonProperty( "version" )] public string Version { get; set; }
        [JsonProperty( "condition" )] public Condition Condition { get; set; }
        [JsonProperty( "transport" )] public Transport Transport { get; set; }
    }

    [Serializable] public class ChannelChatNotificationResponse {
        [JsonProperty( "subscription" )] public Subscription Subscription { get; set; }
        [JsonProperty( "event" )] public Event Event { get; set; }
    }

    // Subscription
    // Name | Type | Description
    // id | string | Your client ID.
    //     type | string | The notification’s subscription type.
    //     version | string | The version of the subscription.
    //     status | string | The status of the subscription.
    //     cost | integer | How much the subscription counts against your limit. See Subscription Limits for more information.
    //     condition | condition | Subscription-specific parameters.
    //     created_at | string | The time the notification was created.
    [Serializable] public class Subscription {
        [JsonProperty( "id" )] public string ID { get; set; }
        [JsonProperty( "status" )] public string Status { get; set; }
        [JsonProperty( "type" )] public string Type { get; set; }
        [JsonProperty( "version" )] public string Version { get; set; }
        [JsonProperty( "condition" )] public Condition Condition { get; set; }
        [JsonProperty( "transport" )] public Transport Transport { get; set; }
        [JsonProperty( "created_at" )] public string CreatedAt { get; set; }
        [JsonProperty( "cost" )] public int Cost { get; set; }
    }

    [Serializable] public class Event {
        [JsonProperty( "broadcaster_user_id" )] public string BroadcasterUserID { get; set; }
        [JsonProperty( "broadcaster_user_login" )] public string BroadcasterUserLogin { get; set; }
        [JsonProperty( "broadcaster_user_name" )] public string BroadcasterUserName { get; set; }
        [JsonProperty( "chatter_user_id" )] public string ChatterUserID { get; set; }
        [JsonProperty( "chatter_user_login" )] public string ChatterUserLogin { get; set; }
        [JsonProperty( "chatter_user_name" )] public string ChatterUserName { get; set; }
        [JsonProperty( "chatter_is_anonymous" )] public bool ChatterIsAnonymous { get; set; }
        [JsonProperty( "color" )] public string Color { get; set; }
        [JsonProperty( "badges" )] public List<Badge> Badges { get; set; }
        [JsonProperty( "system_message" )] public string SystemMessage { get; set; }
        [JsonProperty( "message_id" )] public string MessageID { get; set; }
        [JsonProperty( "message" )] public Message Message { get; set; }
        [JsonProperty( "notice_type" )] public string NoticeType { get; set; }
        [JsonProperty( "sub" )] public Sub Sub { get; set; }
        [JsonProperty( "resub" )] public Resub Resub { get; set; }
        [JsonProperty( "sub_gift" )] public SubGift SubGift { get; set; }
        [JsonProperty( "community_sub_gift" )] public CommunitySubGift CommunitySubGift { get; set; }
        [JsonProperty( "gift_paid_upgrade" )] public GiftPaidUpgrade GiftPaidUpgrade { get; set; }
        [JsonProperty( "prime_paid_upgrade" )] public PrimePaidUpgrade PrimePaidUpgrade { get; set; }
        [JsonProperty( "pay_it_forward" )] public PayItForward PayItForward { get; set; }
        [JsonProperty( "raid" )] public Raid Raid { get; set; }
        [JsonProperty( "unraid" )] public Unraid Unraid { get; set; }
        [JsonProperty( "announcement" )] public Announcement Announcement { get; set; }
        [JsonProperty( "bits_badge_tier" )] public BitsBadgeTier BitsBadgeTier { get; set; }
        [JsonProperty( "charity_donation" )] public CharityDonation CharityDonation { get; set; }
        [JsonProperty( "shared_chat_sub" )] public SharedChatSub SharedChatSub { get; set; }
        [JsonProperty( "shared_chat_resub" )] public SharedChatResub SharedChatResub { get; set; }
        [JsonProperty( "shared_chat_sub_gift" )] public SharedChatSubGift SharedChatSubGift { get; set; }
        [JsonProperty( "shared_chat_community_sub_gift" )] public SharedChatCommunitySubGift SharedChatCommunitySubGift { get; set; }
        [JsonProperty( "shared_chat_gift_paid_upgrade" )] public SharedChatGiftPaidUpgrade SharedChatGiftPaidUpgrade { get; set; }
        [JsonProperty( "shared_chat_prime_paid_upgrade" )] public SharedChatPrimePaidUpgrade SharedChatPrimePaidUpgrade { get; set; }
        [JsonProperty( "shared_chat_pay_it_forward" )] public SharedChatPayItForward SharedChatPayItForward { get; set; }
        [JsonProperty( "shared_chat_raid" )] public SharedChatRaid SharedChatRaid { get; set; }
        [JsonProperty( "shared_chat_unraid" )] public SharedChatUnraid SharedChatUnraid { get; set; }
        [JsonProperty( "shared_chat_announcement" )] public SharedChatAnnouncement SharedChatAnnouncement { get; set; }
        [JsonProperty( "shared_chat_bits_badge_tier" )] public SharedChatBitsBadgeTier SharedChatBitsBadgeTier { get; set; }
        [JsonProperty( "shared_chat_charity_donation" )] public SharedChatCharityDonation SharedChatCharityDonation { get; set; }
        [JsonProperty( "source_broadcaster_user_id" )] public string SourceBroadcasterUserID { get; set; }
        [JsonProperty( "source_broadcaster_user_login" )] public string SourceBroadcasterUserLogin { get; set; }
        [JsonProperty( "source_broadcaster_user_name" )] public string SourceBroadcasterUserName { get; set; }
        [JsonProperty( "source_message_id" )] public string SourceMessageID { get; set; }
        [JsonProperty( "source_badges" )] public List<Badge> SourceBadges { get; set; }

        // Automod Message Hold V1 Properties
        [JsonProperty( "user_id" )] public string UserId { get; set; }
        [JsonProperty( "user_login" )] public string UserLogin { get; set; }
        [JsonProperty( "user_name" )] public string UserName { get; set; }
        [JsonProperty( "level" )] public int? Level { get; set; }
        [JsonProperty( "category" )] public string Category { get; set; }
        [JsonProperty( "held_at" )] public string HeldAt { get; set; }
        [JsonProperty( "fragments" )] public AutomodFragments Fragments { get; set; }

        // Automod Message Hold V2 Properties
        [JsonProperty( "reason" )] public string Reason { get; set; }
        [JsonProperty( "automod" )] public AutomodData Automod { get; set; }
        [JsonProperty( "blocked_term" )] public object BlockedTerm { get; set; }

        // Automod Message Update Properties
        [JsonProperty( "moderator_user_id" )] public string ModeratorUserId { get; set; }
        [JsonProperty( "moderator_user_login" )] public string ModeratorUserLogin { get; set; }
        [JsonProperty( "moderator_user_name" )] public string ModeratorUserName { get; set; }
        [JsonProperty( "status" )] public string Status { get; set; }
    }

    [Serializable] public class Badge {
        [JsonProperty( "set_id" )] public string SetIDNum { get; set; }
        [JsonProperty( "id" )] public string ID { get; set; }
        [JsonProperty( "info" )] public string Info { get; set; }
    }

    [Serializable] public class Message {
        [JsonProperty( "text" )] public string Text { get; set; }
        [JsonProperty( "fragments" )] public List<Fragment> Fragments { get; set; }
    }

    [Serializable] public class Fragment {
        [JsonProperty( "type" )] public string Type { get; set; }
        [JsonProperty( "text" )] public string Text { get; set; }
        [JsonProperty( "cheer" )] public string? Cheer { get; set; }
        [JsonProperty( "emote" )] public Emote? Emote { get; set; }
        [JsonProperty( "mention" )] public Mention? Mention { get; set; }
        // V2 properties
        [JsonProperty( "cheermote" )] public Cheermote? Cheermote { get; set; }
    }

    [Serializable] public class Cheermote {
        [JsonProperty( "prefix" )] public string Prefix { get; set; }
        [JsonProperty( "bits" )] public int Bits { get; set; }
        [JsonProperty( "tier" )] public int Tier { get; set; }
    }

    [Serializable] public class Emote {
        [JsonProperty( "id" )] public string ID { get; set; }
        [JsonProperty( "emote_set_id" )] public string EmoteSetID { get; set; }
        [JsonProperty( "owner_id" )] public int? OwnerID { get; set; }
        [JsonProperty( "format" )] public string Format { get; set; }
    }

    [Serializable] public class Mention {
        [JsonProperty( "user_id" )] public string UserID { get; set; }
        [JsonProperty( "user_name" )] public string UserName { get; set; }
        [JsonProperty( "user_login" )] public string UserLogin { get; set; }
    }

    [Serializable] public class Sub {
        [JsonProperty( "sub_plan" )] public int SubPlan { get; set; }
        [JsonProperty( "cumulative_months" )] public int CumulativeMonths { get; set; }
        [JsonProperty( "streak_months" )] public int? StreakMonths { get; set; }
        [JsonProperty( "is_gift" )] public bool IsGift { get; set; }
        [JsonProperty( "gifter_is_anonymous" )] public bool GifterIsAnonymous { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class Resub {
        [JsonProperty( "cumulative_months" )] public int CumulativeMonths { get; set; }
        [JsonProperty( "duration_months" )] public int DurationMonths { get; set; }
        [JsonProperty( "streak_months" )] public int? StreakMonths { get; set; }
        [JsonProperty( "sub_plan" )] public string SubPlan { get; set; }
        [JsonProperty( "is_gift" )] public bool IsGift { get; set; }
        [JsonProperty( "gifter_is_anonymous" )] public bool? GifterIsAnonymous { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class SubGift {
        [JsonProperty( "cumulative_months" )] public int CumulativeMonths { get; set; }
        [JsonProperty( "duration_months" )] public int DurationMonths { get; set; }
        [JsonProperty( "streak_months" )] public int? StreakMonths { get; set; }
        [JsonProperty( "sub_plan" )] public string SubPlan { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class CommunitySubGift {
        [JsonProperty( "count" )] public int Count { get; set; }
        [JsonProperty( "sub_plan" )] public string SubPlan { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class GiftPaidUpgrade {
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class PrimePaidUpgrade {
        [JsonProperty( "sub_plan" )] public string SubPlan { get; set; }
    }

    [Serializable] public class PayItForward {
        [JsonProperty( "gifter_is_anonymous" )] public bool GifterIsAnonymous { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class Raid {
        [JsonProperty( "user_id" )] public string UserID { get; set; }
        [JsonProperty( "user_name" )] public string UserName { get; set; }
        [JsonProperty( "user_login" )] public string UserLogin { get; set; }
        [JsonProperty( "viewer_count" )] public int ViewerCount { get; set; }
        [JsonProperty( "profile_image_url" )] public string ProfileImageURL { get; set; }
    }

    [Serializable] public class Unraid { }

    [Serializable] public class Announcement {
        [JsonProperty( "color" )] public string Color { get; set; }
    }

    [Serializable] public class BitsBadgeTier {
        [JsonProperty( "tier" )] public int Tier { get; set; }
    }

    [Serializable] public class CharityDonation {
        [JsonProperty( "charity_name" )] public string CharityName { get; set; }
        [JsonProperty( "donated_amount" )] public int DonatedAmount { get; set; }
    }

    [Serializable] public class SharedChatSub {
        [JsonProperty( "sub_plan" )] public int SubPlan { get; set; }
        [JsonProperty( "cumulative_months" )] public int CumulativeMonths { get; set; }
        [JsonProperty( "streak_months" )] public int? StreakMonths { get; set; }
        [JsonProperty( "is_gift" )] public bool IsGift { get; set; }
        [JsonProperty( "gifter_is_anonymous" )] public bool GifterIsAnonymous { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class SharedChatResub {
        [JsonProperty( "cumulative_months" )] public int CumulativeMonths { get; set; }
        [JsonProperty( "duration_months" )] public int DurationMonths { get; set; }
        [JsonProperty( "streak_months" )] public int? StreakMonths { get; set; }
        [JsonProperty( "sub_plan" )] public string SubPlan { get; set; }
        [JsonProperty( "is_gift" )] public bool IsGift { get; set; }
        [JsonProperty( "gifter_is_anonymous" )] public bool? GifterIsAnonymous { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class SharedChatSubGift {
        [JsonProperty( "cumulative_months" )] public int CumulativeMonths { get; set; }
        [JsonProperty( "duration_months" )] public int DurationMonths { get; set; }
        [JsonProperty( "streak_months" )] public int? StreakMonths { get; set; }
        [JsonProperty( "sub_plan" )] public string SubPlan { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class SharedChatCommunitySubGift {
        [JsonProperty( "count" )] public int Count { get; set; }
        [JsonProperty( "sub_plan" )] public string SubPlan { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class SharedChatGiftPaidUpgrade {
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class SharedChatPrimePaidUpgrade {
        [JsonProperty( "sub_plan" )] public string SubPlan { get; set; }
    }

    [Serializable] public class SharedChatPayItForward {
        [JsonProperty( "gifter_is_anonymous" )] public bool GifterIsAnonymous { get; set; }
        [JsonProperty( "gifter_user_id" )] public string GifterUserID { get; set; }
        [JsonProperty( "gifter_user_name" )] public string GifterUserName { get; set; }
        [JsonProperty( "gifter_user_login" )] public string GifterUserLogin { get; set; }
    }

    [Serializable] public class SharedChatRaid {
        [JsonProperty( "user_id" )] public string UserID { get; set; }
        [JsonProperty( "user_name" )] public string UserName { get; set; }
        [JsonProperty( "user_login" )] public string UserLogin { get; set; }
        [JsonProperty( "viewer_count" )] public int ViewerCount { get; set; }
        [JsonProperty( "profile_image_url" )] public string ProfileImageURL { get; set; }
    }

    [Serializable] public class SharedChatUnraid { }

    [Serializable] public class SharedChatAnnouncement {
        [JsonProperty( "color" )] public string Color { get; set; }
    }

    [Serializable] public class SharedChatBitsBadgeTier {
        [JsonProperty( "tier" )] public int Tier { get; set; }
    }

    [Serializable] public class SharedChatCharityDonation {
        [JsonProperty( "charity_name" )] public string CharityName { get; set; }
        [JsonProperty( "donated_amount" )] public int DonatedAmount { get; set; }
    }

    // Automod Classes
    [Serializable] public class AutomodFragments {
        [JsonProperty( "emotes" )] public List<AutomodEmote> Emotes { get; set; }
        [JsonProperty( "cheermotes" )] public List<AutomodCheermote> Cheermotes { get; set; }
    }

    [Serializable] public class AutomodEmote {
        [JsonProperty( "text" )] public string Text { get; set; }
        [JsonProperty( "id" )] public string Id { get; set; }
        [JsonProperty( "set-id" )] public string SetId { get; set; }
    }

    [Serializable] public class AutomodCheermote {
        [JsonProperty( "text" )] public string Text { get; set; }
        [JsonProperty( "amount" )] public int Amount { get; set; }
        [JsonProperty( "prefix" )] public string Prefix { get; set; }
        [JsonProperty( "tier" )] public int Tier { get; set; }
    }

    [Serializable] public class AutomodData {
        [JsonProperty( "category" )] public string Category { get; set; }
        [JsonProperty( "level" )] public int Level { get; set; }
        [JsonProperty( "boundaries" )] public List<AutomodBoundary> Boundaries { get; set; }
    }

    [Serializable] public class AutomodBoundary {
        [JsonProperty( "start_pos" )] public int StartPos { get; set; }
        [JsonProperty( "end_pos" )] public int EndPos { get; set; }
    }
}