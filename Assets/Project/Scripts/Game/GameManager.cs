using System.Collections.Concurrent;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game {
    public class GameManager : MonoBehaviour
    {
        public Dictionary<string, GameObject> m_players = new();
        public GameObject m_player;
        ConcurrentQueue<System.Action> _mainThreadActions = new ConcurrentQueue<System.Action>();

        void Awake() {
            AddPlayer( "Damon" );
            AddPlayer( "NotDamon");
        }

        public void AddPlayer(string id) {
            // if ( !m_players.ContainsKey( id ) ) {
            _mainThreadActions.Enqueue(() => {
                var go = Instantiate(m_player);
                go.GetComponentInChildren<TextMeshProUGUI>().text = id;
                
                go.transform.position = new Vector3(Random.Range(-8f, 8f), 5f, 0f);
                // m_players.Add( id, go );
            });

                
            // }
        }
        void Update() {
            while (_mainThreadActions.TryDequeue(out var action)) {
                action.Invoke();
            }
        }


    }
}
