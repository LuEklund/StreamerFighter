# Get Channel Followers

Gets a list of users that follow the specified broadcaster. You can also use this endpoint to see whether a specific user follows the broadcaster.

## Authorization

Requires a user access token that includes the `moderator:read:followers` scope.
The ID in the broadcaster_id query parameter must match the user ID in the access token or the user ID in the access token must be a moderator for the specified broadcaster.
This endpoint will return specific follower information only if both of the above are true. If a scope is not provided or the user isn't the broadcaster or a moderator for the specified channel, only the total follower count will be included in the response.

## URL

`GET https://api.twitch.tv/helix/channels/followers`

## Request Query Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| user_id | String | No | A user's ID. Use this parameter to see whether the user follows this broadcaster. If specified, the response contains this user if they follow the broadcaster. If not specified, the response contains all users that follow the broadcaster. Using this parameter requires both a user access token with the `moderator:read:followers` scope and the user ID in the access token match the broadcaster_id or be the user ID for a moderator of the specified broadcaster. |
| broadcaster_id | String | Yes | The broadcaster's ID. Returns the list of users that follow this broadcaster. |
| first | Integer | No | The maximum number of items to return per page in the response. The minimum page size is 1 item per page and the maximum is 100. The default is 20. |
| after | String | No | The cursor used to get the next page of results. The Pagination object in the response contains the cursor's value. |

## Response Body

| Field | Type | Description |
|-------|------|-------------|
| data | Object[] | The list of users that follow the specified broadcaster. The list is in descending order by followed_at (with the most recent follower first). The list is empty if nobody follows the broadcaster, the specified user_id isn't in the follower list, the user access token is missing the moderator:read:followers scope, or the user isn't the broadcaster or moderator for the channel. |
| data[].followed_at | String | The UTC timestamp when the user started following the broadcaster. |
| data[].user_id | String | An ID that uniquely identifies the user that's following the broadcaster. |
| data[].user_login | String | The user's login name. |
| data[].user_name | String | The user's display name. |
| pagination | Object | Contains the information used to page through the list of results. The object is empty if there are no more pages left to page through. |
| pagination.cursor | String | The cursor used to get the next page of results. Use the cursor to set the request's after query parameter. |
| total | Integer | The total number of users that follow this broadcaster. As someone pages through the list, the number of users may change as users follow or unfollow the broadcaster. |

## Response Codes

| Code | Description |
|------|-------------|
| 200 | OK - Successfully retrieved the broadcaster's list of followers. |
| 400 | Bad Request - Possible reasons: The broadcaster_id query parameter is required. The broadcaster_id query parameter is not valid. |
| 401 | Unauthorized - Possible reasons: The ID in the broadcaster_id query parameter must match the user ID in the access token or the user must be a moderator for the specified broadcaster. The Authorization header is required and must contain a user access token. The user access token is missing the moderator:read:followers scope. The OAuth token is not valid. The client ID specified in the Client-Id header does not match the client ID specified in the OAuth token. The user_id parameter was specified but either the user access token is missing the moderator:read:followers scope or the user is not the broadcaster or moderator for the specified channel. |

## Example Request

```
GET https://api.twitch.tv/helix/channels/followers?broadcaster_id=123456&first=2
```

## Example Response

```json
{
  "data": [
    {
      "followed_at": "2023-01-15T18:16:11.17106713Z",
      "user_id": "123456789",
      "user_login": "user1",
      "user_name": "User1"
    },
    {
      "followed_at": "2023-01-14T15:23:45.12345678Z",
      "user_id": "987654321",
      "user_login": "user2",
      "user_name": "User2"
    }
  ],
  "pagination": {
    "cursor": "eyJiIjpudWxsLCJhIjp7Ik9mZnNldCI6Mn19"
  },
  "total": 2
}
```