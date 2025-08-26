# Send Chat Message

Sends a message to the broadcaster's chat room.

**NOTE:** When sending messages to a Shared Chat session, behaviors differ depending on your authentication token type:

* When using an App Access Token, messages will only be sent to the source channel (defined by the broadcaster_id parameter) by default starting on May 19, 2025. Messages can be sent to all channels by using the for_source_only parameter and setting it to false.
* When using a User Access Token, messages will be sent to all channels in the shared chat session, including the source channel. This behavior cannot be changed with this token type.

## Authorization

Requires an app access token or user access token that includes the `user:write:chat` scope. If app access token used, then additionally requires `user:bot` scope from chatting user, and either `channel:bot` scope from broadcaster or moderator status.

## URL

`POST https://api.twitch.tv/helix/chat/messages`

## Request Body

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| broadcaster_id | String | Yes | The ID of the broadcaster whose chat room the message will be sent to. |
| sender_id | String | Yes | The ID of the user sending the message. This ID must match the user ID in the user access token. |
| message | String | Yes | The message to send. The message is limited to a maximum of 500 characters. Chat messages can also include emoticons. To include emoticons, use the name of the emote. The names are case sensitive. Don't include colons around the name (e.g., :bleedPurple:). If Twitch recognizes the name, Twitch converts the name to the emote before writing the chat message to the chat room. |
| reply_parent_message_id | String | No | The ID of the chat message being replied to. |
| for_source_only | Bool | No | **NOTE:** This parameter can only be set when utilizing an App Access Token. It cannot be specified when a User Access Token is used, and will instead result in an HTTP 400 error. Determines if the chat message is sent only to the source channel (defined by broadcaster_id) during a shared chat session. This has no effect if the message is sent during a shared chat session. If this parameter is not set, the default value when using an App Access Token is false. On May 19, 2025 the default value for this parameter will be updated to true, and chat messages sent using an App Access Token will only be shared with the source channel by default. If you prefer to send a chat message to both channels in a shared chat session, make sure this parameter is explicitly set to false in your API request before May 19. |

## Response Body

| Field | Type | Description |
|-------|------|-------------|
| data | Object[] |  |
| data[].message_id | String | The message id for the message that was sent. |
| data[].is_sent | Boolean | If the message passed all checks and was sent. |
| data[].drop_reason | Object | The reason the message was dropped, if any. |
| data[].drop_reason.code | String | Code for why the message was dropped. |
| data[].drop_reason.message | String | Message for why the message was dropped. |

## Response Codes

| Code | Description |
|------|-------------|
| 200 | OK - Successfully sent the specified broadcaster a message. |
| 400 | Bad Request - Various parameter validation errors including "Cannot set *for_source_only* if User Access Token is used" |
| 401 | Unauthenticated - Missing or invalid authorization |
| 403 | Forbidden - The sender is not permitted to send chat messages to the broadcaster's chat room |
| 422 | Unprocessable Entity - The message is too large |

## Example Request

```json
{
  "broadcaster_id": "123456",
  "sender_id": "789012",
  "message": "Hello world!",
  "reply_parent_message_id": "abc123def456",
  "for_source_only": false
}
```

## Example Response

```json
{
  "data": [
    {
      "message_id": "abc-123-def-456",
      "is_sent": true,
      "drop_reason": null
    }
  ]
}
```