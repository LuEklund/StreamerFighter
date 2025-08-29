using Stickman;
using UnityEngine;
namespace Game {
    public class PlayerBorderTrigger : MonoBehaviour {
        public LayerMask m_layerMask;
        void OnTriggerEnter2D(Collider2D other) {
            Debug.Log( "OnTriggerEnter2D" );
            if ( (m_layerMask.value & 1 << other.gameObject.layer) > 0 ) {
                var player = other.GetComponentInParent<Stickman.Stickman>();
                if ( player != null ) {
                    Destroy(player.gameObject);
                }
            }
        }
    }
}