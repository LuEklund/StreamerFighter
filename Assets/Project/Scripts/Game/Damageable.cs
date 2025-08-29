using UnityEngine;
using Logger = TCS.Utils.Logger;

namespace Game {
    public class Damageable : MonoBehaviour {
        [SerializeField] Stickman.Stickman m_player;

        void Awake() {
            if ( m_player == null ) {
                Logger.LogError( "Damageable: No Health component found!", this );
                enabled = false;
            }
        }

        public void TakeDamage(int damage, string id) {
            if ( m_player != null  && m_player.IsMe(id) == false) {
                m_player.TakeDamage( damage, transform.TransformPoint( transform.position ) );
            }
        }
    }
}