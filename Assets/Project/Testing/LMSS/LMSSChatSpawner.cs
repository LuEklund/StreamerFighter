using LmSharp;
using UnityEngine;

public class LMSSChatSpawner : MonoBehaviour {
    public GameObject chatPrefab;
    public Transform spawnPoint;

    private void Update() {
        if ( Input.GetKeyDown( KeyCode.C ) ) {
            SpawnChatMessage( "Hello, this is a test message!" );
        }
    }

    public void SpawnChatMessage(string message) {
        GameObject chatInstance = Instantiate( chatPrefab, spawnPoint.position, Quaternion.identity );
        // LMSSChatMessage chatMessage = chatInstance.GetComponent<LMSSChatMessage>();
        // if (chatMessage != null)
        // {
        //     chatMessage.SetMessage(message);
        // }
        // else
        // {
        //     Debug.LogError("LMSSChatMessage component not found on the chat prefab.");
        // }
    }
}