using UnityEngine;
namespace StreamerFighter {
    public enum SpecialEffect { None, Knockback, Poison, Lifesteal, Fire }
    public class Weapon : MonoBehaviour {
        [SerializeField] LayerMask m_layerMask;
        
        [Header("Weapon Settings")]
        public int m_damage = 1;
        public float m_attackRange = 1f;
        public float m_attackRate = 1f;
        
        [Header("Weapon Sounds")]
        public AudioClip m_sound;
        
        [Header("Weapon Special Effects")]
        public SpecialEffect m_specialEffect = SpecialEffect.None;
        public float m_knockbackForce = 0f;
        public int m_poisonDamage= 0; // Damage per second from poison
        public float m_lifestealPercent = 0f; // Percentage of damage dealt
        public float m_fireDamage = 0f; // Duration of fire effect in seconds

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
                var damage = damageable.TryDealDamage( m_damage, GUID );
                if ( m_sound != null && damage ) {
                    AudioSource.PlayClipAtPoint( m_sound, transform.position, 0.5f );
                }
            }
        }
    }
}