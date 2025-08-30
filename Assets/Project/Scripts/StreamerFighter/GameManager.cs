using System;
using System.Collections.Generic;
using Character;
using StreamAPI;
using UnityEngine;
using Logger = TCS.Utils.Logger;
using Random = UnityEngine.Random;
namespace StreamerFighter {
    public class GameManager : MonoBehaviour {
        // Instead of storing just a GameObject, lets have the Stickman component.
        // so we just have direct access to all its API.
        public readonly Dictionary<string, Stickman> Players = new(); 
        public Stickman m_player;
        
        public bool m_spamPlayers = false;
        public bool m_addTestDamons = false;

        void Awake() {
            if ( m_player == null ) {
                Logger.LogError( "GameManager: No Player Prefab assigned!", this );
                enabled = false;
                return;
            }
        }

        void Start() {
            if ( m_addTestDamons ) {
                if(TryAddPlayer("Damon") == false) {
                    Logger.LogError( "i didnt spawn, Damon" );
                }

                if(TryAddChat( "Damon", "Hello World!" ) == false) {
                    Logger.LogError( "i didnt chat, Damon" );
                }

                if(TryAddPlayer( "NotDamon" ) == false) {
                    Logger.LogError( "i didnt spawn, NotDamon" );
                }

                if(TryAddChat( "NotDamon", "Hello World!" ) == false) {
                    Logger.LogError( "i didnt chat, NotDamon" );
                }
            }

            // every 10 seconds spawn a new player.
            if ( m_spamPlayers ) {
                InvokeRepeating( nameof( SpawnRandomPlayer ), 10f, 2f );
            }
        }
        
        void SpawnRandomPlayer() {
            var id = $"User{Random.Range(1000, 9999)}";
            if ( TryAddPlayer( id ) ) {
                Logger.Log( $"Spawned {id}" );
            }
        }

        // networking methods should be a bool so we can try/catch and log errors.
        // if we return false, we can log it.
        public bool TryAddPlayer(string id) {
            if (Players.ContainsKey(id) == false) {
                InvokeOnMainThread(() => SpawnPlayer(id)); // cleaner lambda call for network or unity thread safety.
                return true;
            }
            return false;
        }

        public bool TryAddChat(string id, string message) {
            if ( Players.TryGetValue( id, out var player ) ) {
                InvokeOnMainThread(() => player.SendChatMessage(message)); 
                return true;
            }
            return false;
        }
        
        // this is only used by the Stickman when it spawns itself. aka dragged into the scene.
        public void JustAddMeToDict(string id, Stickman player) {
            if ( !Players.TryAdd( id, player ) ) {
                Logger.LogError( "WTF" );
            }
        }
        
        // Helpers
        void SpawnPlayer(string id) {
            var player = Instantiate(m_player);
            Players.Add(id, player);
                        
            player.m_nameText.text = id;
            player.transform.position = new Vector3(Random.Range(-8f, 8f), 5f, 0f);
        }
        
        public void RemoveAndDestroy(string id) {
            if ( Players.ContainsKey( id ) ) {
                Destroy( Players[id].gameObject );
                Players.Remove( id );
                Logger.Log( $"Removed {id} from players" );
            }
        }
        
        /// <summary>
        /// Executes the given action on the main Unity thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <param name="enqueueIfBackground">
        /// If true, the action will be enqueued to the main thread if called from a background thread.
        /// If false, the action will be executed immediately regardless of the thread.
        /// </param>
        static void InvokeOnMainThread(Action action, bool enqueueIfBackground = true) {
            if (UnityThread.CurrentIsMainThread) {
                action();
            }
            else if (enqueueIfBackground) {
                ThreadDispatcher.Enqueue(action);
            }
            else {
                action();
            }
        }

    }
}