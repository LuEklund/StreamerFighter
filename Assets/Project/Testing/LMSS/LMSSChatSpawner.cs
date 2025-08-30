using System;
using System.Threading;
using System.Threading.Tasks;
using StreamAPI;
using UnityEngine;
using Random = UnityEngine.Random;

public class LMSSChatSpawner : MonoBehaviour {
    #if LMSS
    LMSharp.LMSSClient m_lmssClient;
    #endif

    [SerializeField] ChatBubble m_chatBubble;
    public float m_minTime = 25f;
    public float m_maxTime = 100f;
    float m_timer;

    CancellationTokenSource m_cts = new();

    void Awake() {
        #if LMSS
        m_lmssClient = FindFirstObjectByType<LMSharp.LMSSClient>( FindObjectsInactive.Include );
        if ( m_lmssClient == null ) {
            Debug.LogError( "LMSSChatSpawner: No LMSSClient found in scene!", this );
            enabled = false;
            return;
        }
        #endif
    }

    void Start() => ResetTimer();

    void Update() {
        m_timer -= Time.deltaTime;
        if ( m_timer <= 0f ) {
            // fire-and-forget is okay; cancellation is handled below
            _ = SendChatMessageAsync( m_cts.Token );
            ResetTimer();
        }
    }

    void ResetTimer() {
        m_timer = Random.Range( m_minTime, m_maxTime );
    }

    // IMPORTANT: cancel the token when this object is destroyed
    void OnDestroy() {
        try {
            m_cts.Cancel();
        }
        catch {
            /* ignore */
        }
        finally {
            m_cts.Dispose();
        }
    }

    // Prefer async Task over async void so exceptions/cancellation can flow
    async Task SendChatMessageAsync(CancellationToken token) {
        try {
            string message = await GetChatMessage("Hello", token);

            if (token.IsCancellationRequested || this == null || m_chatBubble == null) return;

            m_chatBubble.ShowMessage(message);
        }
        catch (OperationCanceledException) { /* expected on destroy */ }
        catch (Exception e) { /*Debug.LogException(e, this);*/ }
    }


    public Task<string> GetChatMessage(string message, CancellationToken token) {
        #if LMSS
        return m_lmssClient.GetChatMessage( message, token );
        #else
        return Task.FromResult( "Hello World" );
        #endif
    }
}