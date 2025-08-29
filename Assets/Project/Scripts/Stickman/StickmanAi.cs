using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Stickman {
    public class StickmanAi : MonoBehaviour {
        [SerializeField] Stickman m_stickman;
        public Stickman m_target;

        public TextMeshProUGUI m_nameText;
        public TextMeshProUGUI m_chatText;

        ControlledArms m_controlledArms;

        [Header("AI Ranges")]
        public float m_detectionRange = 100f;
        public float m_attackRange = 2f;

        [Header("Combat")]
        public float m_attackCooldown = 1f;

        float m_lastAttackTime = 0f;

        void Awake() {
            if (m_stickman == null) {
                m_stickman = GetComponent<Stickman>();
                enabled = false;
                return;
            }
            m_controlledArms = m_stickman.m_controlledArms;
            m_stickman.IsPlayerControlled = false;
        }

        void Update() {
            if (m_stickman.m_movement.m_isGrounded == false) {
                StopMoving();
                m_stickman.m_movementKeys.m_attack = false;
                return;
            }
            
            if (m_target == null || Vector2.Distance(m_target.m_torso.transform.position, m_stickman.m_torso.transform.position) > m_detectionRange) {
                FindTarget();
            }

            if (m_target == null) {
                StopMoving();
                m_stickman.m_movementKeys.m_attack = false;
                return;
            }

            Vector2 myPos  = m_stickman.m_torso.transform.position;
            Vector2 tgtPos = m_target.m_torso.transform.position;
            float distanceToTarget = Vector2.Distance(tgtPos, myPos);
            float dx = tgtPos.x - myPos.x;

            // Decide whether to move
            m_attackRange = m_stickman.m_weaponManager.AttackRange;
            bool withinAttackRange = distanceToTarget <= m_attackRange;
            bool shouldChase       = distanceToTarget > m_attackRange;

            // Clear previous inputs
            StopMoving();

            if (shouldChase) {
                if (dx < -0.05f) m_stickman.m_movementKeys.m_left  = true;
                else if (dx > 0.05f) m_stickman.m_movementKeys.m_right = true;
            }
            
            bool isMoving = m_stickman.m_movementKeys.m_left || m_stickman.m_movementKeys.m_right;
            
            if (!withinAttackRange && m_controlledArms.IsAttacking()) {
                m_stickman.m_movementKeys.m_attack = false;
            }
            else if (!isMoving && withinAttackRange && Time.time - m_lastAttackTime > m_attackCooldown) {
                m_stickman.m_movementKeys.m_attack = true;
                m_lastAttackTime = Time.time;
            }
        }

        void StopMoving() {
            m_stickman.m_movementKeys.m_left = false;
            m_stickman.m_movementKeys.m_right = false;
        }

        void FindTarget() {
            // Pick the closest other Stickman within detection range
            Stickman[] motors = FindObjectsByType<Stickman>(FindObjectsSortMode.None);
            float closest = Mathf.Infinity;
            Stickman best = null;
            Vector2 myPos = m_stickman.m_torso.transform.position;

            foreach (var m in motors) {
                if (m == m_stickman) continue; // don't target self
                float d = Vector2.Distance(m.m_torso.transform.position, myPos);
                if (d < closest && d <= m_detectionRange) {
                    closest = d;
                    best = m;
                }
            }
            m_target = best;
        }
    }
}