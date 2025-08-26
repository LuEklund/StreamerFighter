# Channel Chat Notification Event

The `channel.chat.notification` subscription type sends a notification when an event that appears in chat occurs, such as someone subscribing to the channel or a subscription is gifted.

## Authorization

Requires `user:read:chat` scope from chatting user. If app access token used, then additionally requires `user:bot` scope from chatting user, and either `channel:bot` scope from broadcaster or moderator status.

## Request Body

| Name | Type | Required | Description |
|------|------|----------|-------------|
| type | String | Yes | The subscription type name: `channel.chat.notification` |
| version | String | Yes | The subscription type version: `1` |
| condition | condition | Yes | Subscription-specific parameters |
| transport | transport | Yes | Transport-specific parameters |

## Webhook Example

```json
{
    "type": "channel.chat.notification",
    "version": "1",
    "condition": {
        "broadcaster_user_id": "1971641",
        "user_id": "2914196"
    },
    "transport": {
        "method": "webhook",
        "callback": "https://example.com/webhooks/callback",
        "secret": "s3cRe7"
    }
}
```

## Regular Chat Notification Example

```json
{
  "subscription": {
    "id": "dc1a3cfc-a930-4972-bf9e-0ffc4e7a8996",
    "status": "enabled",
    "type": "channel.chat.notification",
    "version": "1",
    "condition": {
      "broadcaster_user_id": "1971641",
      "user_id": "2914196"
    },
    "transport": {
      "method": "websocket",
      "session_id": "AgoQOtgGkFvXRlSkij343CndhIGY2VsbC1h"
    },
    "created_at": "2023-10-06T18:04:38.807682738Z",
    "cost": 0
  },
  "event": {
    "broadcaster_user_id": "1971641",
    "broadcaster_user_login": "streamer",
    "broadcaster_user_name": "streamer",
    "chatter_user_id": "49912639",
    "chatter_user_login": "viewer23",
    "chatter_user_name": "viewer23",
    "chatter_is_anonymous": false,
    "color": "",
    "badges": [],
    "system_message": "viewer23 subscribed at Tier 1. They've subscribed for 10 months!",
    "message_id": "d62235c8-47ff-a4f4--84e8-5a29a65a9c03",
    "message": {
      "text": "",
      "fragments": []
    },
    "notice_type": "resub",
    "sub": null,
    "resub": {
      "cumulative_months": 10,
      "duration_months": 0,
      "streak_months": null,
      "sub_plan": "1000",
      "is_gift": false,
      "gifter_is_anonymous": null,
      "gifter_user_id": null,
      "gifter_user_name": null,
      "gifter_user_login": null
    },
    "sub_gift": null,
    "community_sub_gift": null,
    "gift_paid_upgrade": null,
    "prime_paid_upgrade": null,
    "pay_it_forward": null,
    "raid": null,
    "unraid": null,
    "announcement": null,
    "bits_badge_tier": null,
    "charity_donation": null,
    "shared_chat_sub": null,
    "shared_chat_resub": null,
    "shared_chat_sub_gift": null,
    "shared_chat_community_sub_gift": null,
    "shared_chat_gift_paid_upgrade": null,
    "shared_chat_prime_paid_upgrade": null,
    "shared_chat_pay_it_forward": null,
    "shared_chat_raid": null,
    "shared_chat_announcement": null,
    "source_broadcaster_user_id": null,
    "source_broadcaster_user_login": null,
    "source_broadcaster_user_name": null,
    "source_message_id": null,
    "source_badges": null
  }
}
```

## Shared Chat Notification Example

```json
{
  "subscription": {
    "id": "dc1a3cfc-a930-4972-bf9e-0ffc4e7a8996",
    "status": "enabled",
    "type": "channel.chat.notification",
    "version": "1",
    "condition": {
      "broadcaster_user_id": "1971641",
      "user_id": "2914196"
    },
    "transport": {
      "method": "websocket",
      "session_id": "AgoQOtgGkFvXRlSkij343CndhIGY2VsbC1h"
    },
    "created_at": "2023-10-06T18:04:38.807682738Z",
    "cost": 0
  },
  "event": {
    "broadcaster_user_id": "1971641",
    "broadcaster_user_login": "streamer",
    "broadcaster_user_name": "streamer",
    "chatter_user_id": "49912639",
    "chatter_user_login": "viewer23",
    "chatter_user_name": "viewer23",
    "chatter_is_anonymous": false,
    "color": "",
    "badges": [],
    "system_message": "viewer23 subscribed at Tier 1. They've subscribed for 10 months!",
    "message_id": "d62235c8-47ff-a4f4--84e8-5a29a65a9c03",
    "message": {
      "text": "",
      "fragments": []
    },
    "notice_type": "shared_chat_resub",
    "sub": null,
    "resub": null,
    "sub_gift": null,
    "community_sub_gift": null,
    "gift_paid_upgrade": null,
    "prime_paid_upgrade": null,
    "pay_it_forward": null,
    "raid": null,
    "unraid": null,
    "announcement": null,
    "bits_badge_tier": null,
    "charity_donation": null,
    "shared_chat_sub": null,
    "shared_chat_resub": {
      "cumulative_months": 10,
      "duration_months": 0,
      "streak_months": null,
      "sub_plan": "1000",
      "is_gift": false,
      "gifter_is_anonymous": null,
      "gifter_user_id": null,
      "gifter_user_name": null,
      "gifter_user_login": null
    },
    "shared_chat_sub_gift": null,
    "shared_chat_community_sub_gift": null,
    "shared_chat_gift_paid_upgrade": null,
    "shared_chat_prime_paid_upgrade": null,
    "shared_chat_pay_it_forward": null,
    "shared_chat_raid": null,
    "shared_chat_unraid": null,
    "shared_chat_announcement": null,
    "shared_chat_bits_badge_tier": null,
    "shared_chat_charity_donation": null,
    "source_broadcaster_user_id": "112233",
    "source_broadcaster_user_login": "streamer33",
    "source_broadcaster_user_name": "streamer33",
    "source_message_id": "2be7193d-0366-4453-b6ec-b288ce9f2c39",
    "source_badges": [{
      "set_id": "subscriber",
      "id": "3",
      "info": "3"
    }]
  }
}
```