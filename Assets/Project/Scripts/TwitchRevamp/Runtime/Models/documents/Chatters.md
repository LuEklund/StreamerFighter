# Get Chatters

Gets the list of users that are connected to the broadcaster's chat session.

**NOTE:** There is a delay between when users join and leave a chat and when the list is updated accordingly.

To determine whether a user is a moderator or VIP, use the Get Moderators and Get VIPs endpoints. You can check the roles of up to 100 users.

## Authorization

Requires a user access token that includes the `moderator:read:chatters` scope.

## URL

`GET https://api.twitch.tv/helix/chat/chatters`

## Request Query Parameters

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| broadcaster_id | String | Yes | The ID of the broadcaster whose list of chatters you want to get. |
| moderator_id | String | Yes | The ID of the broadcaster or one of the broadcaster's moderators. This ID must match the user ID in the user access token. |
| first | Integer | No | The maximum number of items to return per page in the response. The minimum page size is 1 item per page and the maximum is 1,000. The default is 100. |
| after | String | No | The cursor used to get the next page of results. The Pagination object in the response contains the cursor's value. |

## Response Body

| Field | Type | Description |
|-------|------|-------------|
| data | Object[] | The list of users that are connected to the broadcaster's chat room. The list is empty if no users are connected to the chat room. |
| data[].user_id | String | The ID of a user that's connected to the broadcaster's chat room. |
| data[].user_login | String | The user's login name. |
| data[].user_name | String | The user's display name. |
| pagination | Object | Contains the information used to page through the list of results. The object is empty if there are no more pages left to page through. |
| pagination.cursor | String | The cursor used to get the next page of results. Use the cursor to set the request's after query parameter. |
| total | Integer | The total number of users that are connected to the broadcaster's chat room. As you page through the list, the number of users may change as users join and leave the chat room. |

## Response Codes

| Code | Description |
|------|-------------|
| 200 | OK - Successfully retrieved the broadcaster's list of chatters. |
| 400 | Bad Request - Various parameter validation errors |
| 401 | Unauthorized - Missing or invalid authorization |
| 403 | Forbidden - Moderator ID does not match broadcaster's moderators |

## Example Request

```
GET https://api.twitch.tv/helix/chat/chatters?broadcaster_id=123456&moderator_id=123456&first=100
```

## Example Response

```json
{
  "data": [
    {
      "user_id": "123456789",
      "user_login": "user1",
      "user_name": "User1"
    },
    {
      "user_id": "987654321",
      "user_login": "user2",
      "user_name": "User2"
    }
  ],
  "pagination": {
    "cursor": "eyJiIjpudWxsLCJhIjp7Ik9mZnNldCI6NX19"
  },
  "total": 2
}
```