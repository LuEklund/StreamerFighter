using System.Collections.Concurrent;
using System.Collections.Generic;
using Stickman;
using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour
    {
        public Dictionary<string, GameObject> m_players = new();
        public GameObject m_player;
        ConcurrentQueue<System.Action> _mainThreadActions = new ConcurrentQueue<System.Action>();
        void Awake() {
            AddPlayer( "Damon", "DAMON!" );
            AddPlayer( "NotDamon", "WHo is DAMON!?");
        }
        
        void OnEnable()
        {
            Health.OnDeath += HandleDeath;
        }

        void OnDisable()
        {
            Health.OnDeath -= HandleDeath;
        }

        public void AddPlayer(string id, string msg)
        {
            GameObject player = null;
            if ( m_players.ContainsKey( id ) == false) {
                _mainThreadActions.Enqueue(() => {
                    player = Instantiate(m_player);
                    player.GetComponentInChildren<StickmanAi>().m_nameText.text = id;
                    
                    player.transform.position = new Vector3(Random.Range(-8f, 8f), 5f, 0f);
                    m_players.Add( id, player );
                });
            }
            else
            {
                player = m_players[id];
            }

            _mainThreadActions.Enqueue(() => { player.GetComponentInChildren<StickmanAi>().m_chatText.text = msg; });

        }
        void Update() {
            while (_mainThreadActions.TryDequeue(out var action)) {
                action.Invoke();
            }
        }

        void HandleDeath(string id)
        {
            if (m_players.ContainsKey(id))
            {
                Destroy( m_players[id] );
                m_players.Remove(id);
                Debug.Log($"Removed {id} from players");
            }
        }

    }
}
