using System;
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

    // randomly send a chat message every m_minTime to m_maxTime seconds
    void Start() => ResetTimer();

    void Update() {
        m_timer -= Time.deltaTime;
        if ( m_timer <= 0f ) {
            SendChatMessage();
            ResetTimer();
        }
    }
    void ResetTimer() {
        m_timer = Random.Range( m_minTime, m_maxTime );
    }
    async void SendChatMessage() {
        try {
            string message = await GetChatMessage( "Say something funny" );
            if ( string.IsNullOrEmpty( message ) ) {
                return;
            }
        
            m_chatBubble.ShowMessage( message );
        }
        catch (Exception e) {
            // NOOP
        }
    }
    
    
    
    public Task<string> GetChatMessage(string message) {
        #if LMSS
        return m_lmssClient.SendChatMessage( message );
        #else
        return Task.FromResult( "Hello World" );
        #endif
    }
}