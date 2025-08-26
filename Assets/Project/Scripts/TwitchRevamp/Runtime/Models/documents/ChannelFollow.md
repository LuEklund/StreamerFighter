# Channel Follow Event

The `channel.follow` subscription type sends a notification when a specified channel receives a follow.

## Authorization

Must have `moderator:read:followers` scope.

## Request Body

| Name | Type | Required? | Description |
|------|------|-----------|-------------|
| type | string | yes | The subscription type name: `channel.follow`. |
| version | string | yes | The subscription type version: `2`. |
| condition | condition | yes | Subscription-specific parameters. |
| transport | transport | yes | Transport-specific parameters. |

### Condition

| Name | Type | Required? | Description |
|------|------|-----------|-------------|
| broadcaster_user_id | string | yes | The user ID of the channel to receive follow notifications for. |
| moderator_user_id | string | yes | The user ID of the moderator of the channel. |

### Transport

| Name | Type | Required? | Description |
|------|------|-----------|-------------|
| method | string | yes | The transport method: `webhook`. |
| callback | string | yes | The callback URL where the notification should be sent. |
| secret | string | yes | A secret string used to verify the authenticity of the notification. |

## Example Request

```json
{
    "type": "channel.follow",
    "version": "2",
    "condition": {
        "broadcaster_user_id": "1337",
        "moderator_user_id": "1337"
    },
    "transport": {
        "method": "webhook",
        "callback": "https://example.com/webhooks/callback",
        "secret": "s3cRe7"
    }
}
```

## Notification Payload

| Name | Type | Description |
|------|------|-------------|
| subscription | subscription | Metadata about the subscription. |
| event | event | The event information. Contains the user ID and user name of the follower and the broadcaster user ID and broadcaster user name. |

### Event

| Name | Type | Description |
|------|------|-------------|
| user_id | string | The user ID of the follower. |
| user_login | string | The user login of the follower. |
| user_name | string | The user display name of the follower. |
| broadcaster_user_id | string | The user ID of the channel being followed. |
| broadcaster_user_login | string | The user login of the channel being followed. |
| broadcaster_user_name | string | The user display name of the channel being followed. |
| followed_at | string | RFC3339 timestamp of when the follow occurred. |

## Example Notification

```json
{
    "subscription": {
        "id": "f1c2a387-161a-49f9-a165-0f21d7a4e1c4",
        "type": "channel.follow",
        "version": "2",
        "status": "enabled",
        "cost": 0,
        "condition": {
           "broadcaster_user_id": "1337",
           "moderator_user_id": "1337"
        },
         "transport": {
            "method": "webhook",
            "callback": "https://example.com/webhooks/callback"
        },
        "created_at": "2019-11-16T10:11:12.634234626Z"
    },
    "event": {
        "user_id": "1234",
        "user_login": "cool_user",
        "user_name": "Cool_User",
        "broadcaster_user_id": "1337",
        "broadcaster_user_login": "cooler_user",
        "broadcaster_user_name": "Cooler_User",
        "followed_at": "2020-07-15T18:16:11.17106713Z"
    }
}
```