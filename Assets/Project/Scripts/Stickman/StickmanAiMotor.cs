using UnityEngine;

namespace Stickman {
    public class StickmanAiMotor : MonoBehaviour {
        [SerializeField] StickmanMotor stickmanMotor;
        public StickmanMotor target;

        ControlledArms m_controlledArms;

        [Header("AI Ranges")]
        public float m_detectionRange = 10f;
        public float m_attackRange = 2f;
        public float m_stopBuffer = 0.2f;

        [Header("Combat")]
        public float m_attackCooldown = 1f;

        bool m_isAtTarget = false;
        float m_lastAttackTime = 0f;

        void Awake() {
            if (stickmanMotor == null) {
                stickmanMotor = GetComponent<StickmanMotor>();
                enabled = false;
                return;
            }
            m_controlledArms = stickmanMotor.m_controlledArms;
            stickmanMotor.IsPlayerControlled = false;
        }

        void Update() {
            // Refresh/validate target if none or too far
            if (target == null || Vector2.Distance(target.transform.position, stickmanMotor.m_torso.transform.position) > m_detectionRange) {
                FindTarget();
            }

            if (target == null) {
                // No target -> don't move, don't attack
                StopMoving();
                return;
            }

            Vector2 myPos  = stickmanMotor.m_torso.transform.position;
            Vector2 tgtPos = target.transform.position;
            float distanceToTarget = Vector2.Distance(tgtPos, myPos);
            float dx = tgtPos.x - myPos.x;

            // Decide whether to move
            bool withinAttackRange = distanceToTarget <= m_attackRange;
            bool shouldChase       = distanceToTarget > (m_attackRange + m_stopBuffer);

            // Clear previous inputs
            StopMoving();

            if (shouldChase) {
                // Move horizontally toward target until we hit the stop band
                if (dx < -0.05f) stickmanMotor.m_movementKeys.m_left  = true;
                else if (dx > 0.05f) stickmanMotor.m_movementKeys.m_right = true;
                m_isAtTarget = false;
            } else {
                // We're in the desired band -> stand still
                m_isAtTarget = true;
            }

            // --- Attack gating ---
            // 1) Do not attack if we're moving toward the target.
            // 2) Only attack if in range AND off cooldown.
            bool isMoving = stickmanMotor.m_movementKeys.m_left || stickmanMotor.m_movementKeys.m_right;
            if (!isMoving && withinAttackRange && (Time.time - m_lastAttackTime) > m_attackCooldown) {
                // Choose hand based on target's horizontal relation, not movement keys
                var direction = (dx < 0f) ? HandSelection.Left : HandSelection.Right;
                m_controlledArms.Attack(direction);
                m_lastAttackTime = Time.time;
            }
        }

        void StopMoving() {
            stickmanMotor.m_movementKeys.m_left = false;
            stickmanMotor.m_movementKeys.m_right = false;
        }

        void FindTarget() {
            // Pick the closest other StickmanMotor within detection range
            StickmanMotor[] motors = FindObjectsByType<StickmanMotor>(FindObjectsSortMode.None);
            float closest = Mathf.Infinity;
            StickmanMotor best = null;
            Vector2 myPos = stickmanMotor.m_torso.transform.position;

            foreach (var m in motors) {
                if (m == stickmanMotor) continue; // don't target self
                float d = Vector2.Distance(m.transform.position, myPos);
                if (d < closest && d <= m_detectionRange) {
                    closest = d;
                    best = m;
                }
            }
            target = best;
        }
    }
}