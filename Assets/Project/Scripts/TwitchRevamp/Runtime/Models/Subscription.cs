using System;
using Newtonsoft.Json;
namespace TwitchRevamp.Models {
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
}