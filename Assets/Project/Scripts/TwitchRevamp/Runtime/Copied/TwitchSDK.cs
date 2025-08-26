using System.Threading;
using System.Threading.Tasks;
using TwitchSDK;
using TwitchSDK.Interop;
using UnityEngine;

#region Twitch API Singleton
namespace TwitchRevamp {
    public class UnityTwitch : TwitchSDKApi {
        UnityPAL PAL;
        public UnityTwitch(string clientId, bool useESProxy) : base( clientId, useESProxy ) { }

        public void InitializeInternally() {
            PAL.Start();
        }

        protected override PlatformAbstractionLayer CreatePAL() {
            // We need to save this in a variable, so we can call InitializeInternally later.
            return (PAL = new UnityPAL());
        }

        class UnityPAL : ManagedPAL {
            TaskCompletionSource<string> FileIOBasePathTCS = new TaskCompletionSource<string>();

            static UnityPAL() {
                TaskScheduler.UnobservedTaskException += (a, exc) => {
                    if ( exc.Exception.InnerException.GetType() == typeof(CoreLibraryException) ) {
                        Debug.LogWarning( "Unhandled Twitch Exception: " + exc.Exception.InnerException );
                    }
                };
            }

            public void Start() {
                FileIOBasePathTCS.SetResult( Application.persistentDataPath );
            }

            protected override Task Log(LogRequest req) {
                switch (req.Level) {
                    case LogLevel.Debug:
                        // don't show
                        break;
                    case LogLevel.Warning:
                        Debug.LogWarning( req.Message );
                        break;
                    case LogLevel.Error:
                        Debug.LogError( req.Message );
                        break;
                    default:
                        Debug.Log( req.Message );
                        break;
                }

                return Task.CompletedTask;
            }

            // replace FALSE with TRUE to debug / inspect the HTTP requests of the plugin.
#if FALSE
        protected override async Task<WebRequestResult> WebRequest(WebRequestRequest request)
        {
            var isAuthRequest = request.Uri.IndexOf("https://id.twitch.tv/") == 0;
            var displayUri = request.Uri;

            if (isAuthRequest && request.Uri.IndexOf("?") != -1)
                displayUri = request.Uri.Substring(0, request.Uri.IndexOf("?")) + "?redacted.";

            Debug.Log("Getting " + displayUri);
            var res = await base.WebRequest(request);
            Debug.Log("Response to " + displayUri + " is of code " + res.HttpStatus + " and has the body: \r\n" + (isAuthRequest ? "redacted" : res.ResponseBody));
            return res;
        }
#endif

            protected override Task<string> GetFileIOBasePath(CancellationToken _) {
                return FileIOBasePathTCS.Task;
            }

            protected override string HttpUserAgent => "Twitch-Route-66-Unity";
        }
    }
}
#endregion