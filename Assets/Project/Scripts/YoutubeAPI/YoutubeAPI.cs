using System.IO;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using System.Threading;
using System.Threading.Tasks;
using StreamerFighter;


[System.Serializable]
public class YouTubeCredentials
{
    public string YT_CLIENT;
    public string YT_SECRET;
}

public class YoutubeAPI : MonoBehaviour
{
   public GameManager m_gameManager;
    
    private YouTubeService youtubeService;
    async void Start()
    {
        string relativePath = "Lucas/secrets";
        TextAsset textAsset = Resources.Load<TextAsset>(relativePath);

        if (textAsset == null)
        {
            Debug.LogError($"Credentials file not found: {relativePath}");
            return;
        }

        string json = textAsset.text;
        YouTubeCredentials creds = JsonUtility.FromJson<YouTubeCredentials>(json);
    
    
        UserCredential credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            new ClientSecrets
            {
                ClientId = creds.YT_CLIENT,
                ClientSecret = creds.YT_SECRET
            },
            new[] { YouTubeService.Scope.YoutubeReadonly },
            "user", // this is the stored token folder name
            CancellationToken.None
        );

        youtubeService = new YouTubeService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Unity YouTube Chat Listener"
        });

        Debug.Log("OAuth authentication complete!");
    
        string liveChatId = await GetLiveChatId();
    
        // Poll messages
        await PollLiveChat(liveChatId);
    }

    async Task<string> GetLiveChatId()
    {
        var liveBroadcastRequest = youtubeService.LiveBroadcasts.List("snippet");
        liveBroadcastRequest.BroadcastStatus = LiveBroadcastsResource.ListRequest.BroadcastStatusEnum.Active;
        var response = await liveBroadcastRequest.ExecuteAsync();

        if (response.Items.Count > 0)
        {
            return response.Items[0].Snippet.LiveChatId;
        }

        Debug.LogError("No active live broadcasts found");
        return null;
    }

    async Task PollLiveChat(string liveChatId)
    {
        string nextPageToken = null;

        while (true)
        {
            var request = youtubeService.LiveChatMessages.List(liveChatId, "snippet,authorDetails");
            if (!string.IsNullOrEmpty(nextPageToken))
                request.PageToken = nextPageToken;

            var response = await request.ExecuteAsync();

            // Print only new messages
            foreach (var msg in response.Items)
            {
                if (msg == null
                    || msg.AuthorDetails == null
                    || msg.AuthorDetails.DisplayName == null
                    || msg.Snippet == null) continue;
         
                // This still adds the player if they don't exist, otherwise just adds the chat message
                // the if statement is just for logging purposes we want the other line below.
                if (!m_gameManager.TryAddPlayer(msg.AuthorDetails.DisplayName)) { // if player already exists, just add chat
                }
            
                m_gameManager.TryAddChat( msg.AuthorDetails.DisplayName, msg.Snippet.DisplayMessage );

            }

            // Save the token for the next poll
            nextPageToken = response.NextPageToken;

            // Use the recommended polling interval
            long delay = response.PollingIntervalMillis ?? 5000; // default 5s
            await Task.Delay((int)delay);
        }
    }
}
