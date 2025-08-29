using System;
using UnityEngine;
namespace Game {
    public class Weapon : MonoBehaviour {
        public int m_damage = 1;
        [SerializeField] LayerMask m_layerMask;
        [SerializeField] Collider2D m_weaponCollider;

        bool m_useLayerMask;
        public string GUID { get; set; }

        void Awake() {
            m_useLayerMask = m_layerMask != 0;

            if ( m_weaponCollider == null ) {
                m_weaponCollider = GetComponent<Collider2D>();
            }
            
            if ( m_weaponCollider == null ) {
                Debug.LogError( "Weapon: No Collider2D found on weapon object!", this );
                enabled = false;
                return;
            }

            
        }

        void OnTriggerEnter2D(Collider2D other) {
            if ( m_useLayerMask && (m_layerMask.value & 1 << other.gameObject.layer) == 0 ) {
                return;
            }

            var damageable = other.GetComponent<Damageable>();
            if ( damageable != null ) {
                damageable.TakeDamage( m_damage, GUID );
            }
        }
    }
}