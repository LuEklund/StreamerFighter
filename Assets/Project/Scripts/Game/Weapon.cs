using System;
using UnityEngine;
namespace Game {
    public class Weapon : MonoBehaviour {
        [SerializeField] LayerMask m_layerMask;
        
        [Header("Weapon Settings")]
        public int m_damage = 1;
        public float m_attackRange = 1f;
        public float m_attackRate = 1f;

        bool m_useLayerMask;
        public string GUID { get; set; }

        void Awake() {
            m_useLayerMask = m_layerMask != 0;
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