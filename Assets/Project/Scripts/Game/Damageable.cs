using Stickman;
using UnityEngine;

namespace Game {
    // Attach this to any character or object that can deal/receive damage.
    public class TeamMember : MonoBehaviour {
        [Tooltip( "Players/actors with the same TeamId won't hurt each other." )]
        public int m_teamId;
    }

    public class Weapon : MonoBehaviour {
        [Header( "Damage" )]
        [SerializeField] int m_damage = 1;

        [Header( "Target Filtering" )]
        [SerializeField] LayerMask m_layerMask;
        [Tooltip( "If null, defaults to transform.root" )]
        [SerializeField] Transform m_ownerRoot;

        [Header( "Physics" )]
        [Tooltip( "If null, tries to GetComponent<Collider2D>()" )]
        [SerializeField] Collider2D m_weaponCollider;

        bool m_useLayerMask;
        TeamMember m_ownerTeam;

        void Awake() {
            m_useLayerMask = m_layerMask != 0;

            if ( m_ownerRoot == null ) {
                m_ownerRoot = transform.root;
            }

            if ( m_weaponCollider == null ) {
                m_weaponCollider = GetComponent<Collider2D>();
            }

            // Cache owner's team (if any)
            if ( m_ownerRoot != null ) {
                m_ownerTeam = m_ownerRoot.GetComponentInChildren<TeamMember>();
            }

            // Hard-ignore collisions with ALL colliders on the owner
            if ( m_weaponCollider != null && m_ownerRoot != null ) {
                var ownerCols = m_ownerRoot.GetComponentsInChildren<Collider2D>( includeInactive: true );
                foreach (var c in ownerCols) {
                    if ( c != null && c != m_weaponCollider ) {
                        Physics2D.IgnoreCollision( m_weaponCollider, c, true );
                    }
                }
            }
        }

        void OnTriggerEnter2D(Collider2D other) {
            // 1) Self check: same root? (extra safety even though we ignore collisions above)
            if ( m_ownerRoot != null && other.transform.root == m_ownerRoot ) {
                return;
            }

            // 2) LayerMask check
            if ( m_useLayerMask && (m_layerMask.value & 1 << other.gameObject.layer) == 0 ) {
                return;
            }

            // 3) Team check: if both have teams and they match, skip
            var otherTeam = other.GetComponentInParent<TeamMember>();
            if ( m_ownerTeam != null && otherTeam != null && m_ownerTeam.m_teamId == otherTeam.m_teamId ) {
                return;
            }

            // 4) Apply damage (look up Damageable in parent so hitboxes work)
            var health = other.GetComponentInParent<Damageable>();
            if ( health != null ) {
                health.TakeDamage( m_damage );
            }
        }
    }

    public class Damageable : MonoBehaviour {
        [SerializeField] Health m_health;

        void Awake() {
            if ( m_health == null ) {
                Debug.LogError( "Damageable: No Health component found!" );
            }
        }

        public void TakeDamage(int damage) {
            if ( m_health != null ) {
                m_health.TakeDamage( damage );
            }
        }
    }
}