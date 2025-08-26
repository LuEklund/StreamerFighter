Automod Terms Update Request Body
Name	Type	Required?	Description
type	String	Yes	The subscription type name: automod.terms.update.
version	String	Yes	The subscription type version: 1.
condition	 condition 	Yes	Subscription-specific parameters.
transport	 transport 	Yes	Transport-specific parameters.

Automod Terms Update Webhook Example
```json
{
    "type": "automod.terms.update",
    "version": "1",
    "condition": {
        "broadcaster_user_id": "1337",
        "moderator_user_id": "9001"
    },
    "transport": {
        "method": "webhook",
        "callback": "https://example.com/webhooks/callback",
        "secret": "s3cRe7"
    }
}
```

Automod Terms Update Notification Payload
Name	Type	Description
subscription	 subscription 	Metadata about the subscription.
event	 event 	The event information.
```json
{
  "subscription": {
    "id": "f1c2a387-161a-49f9-a165-0f21d7a4e1c4",
    "type": "automod.terms.update",
    "version": "1",
    "status": "enabled",
    "cost": 0,
    "condition": {
      "broadcaster_user_id": "1337",
      "moderator_user_id": "9001"
    },
    "transport": {
      "method": "webhook",
      "callback": "https://example.com/webhooks/callback"
    },
    "created_at": "2023-04-11T10:11:12.123Z"
  },
  "event": {
    "broadcaster_user_id": "1337",
    "broadcaster_user_name": "blah",
    "broadcaster_user_login": "blahblah",
    "moderator_user_id": "9001",
    "moderator_user_login": "the_mod",
    "moderator_user_name": "The_Mod",
    "action": "bad-message-id",
    "from_automod": true,
    "terms": ["automodterm1", "automodterm2", "automodterm3"]
  }
}
```