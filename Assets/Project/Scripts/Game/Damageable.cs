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

        public bool TryDealDamage(int damage, string id) {
            if ( m_player != null  && m_player.IsMe(id) == false) {
                return m_player.TryTakeDamage( damage, transform.position );
            }
            
            return false;
        }
    }
}