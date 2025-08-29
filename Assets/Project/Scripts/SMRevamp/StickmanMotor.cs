using System;
using System.Collections;
using Stickman;
using TCS.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
namespace SMRevamp {
    public class StickmanMotor : MonoBehaviour {
        // Stored in "N" format (32 hex chars, no dashes) for compactness and easy copy/paste.
        [ReadOnly] public string m_guid;
        public GameObject m_torso;
        [Header( "Components" )]
        public MovementKeys m_movementKeys = new();
        public Movement m_movement = new();
        public ControlledArms m_controlledArms = new();
        public ControlledKnees m_controlledKnees = new();
        public ControlledLegs m_controlledLegs = new();
        public ControlledLean m_controlledLean = new();

        Coroutine m_leftRoutine;
        Coroutine m_rightRoutine;

        void OnValidate() {
            if ( string.IsNullOrEmpty( m_guid ) ) {
                m_guid = Guid.NewGuid().ToString( "N" );
            }
        }

        public void Awake() {
            m_movement.Init( m_movementKeys );
            m_controlledArms.Init( m_movementKeys );

            m_controlledKnees.Init( m_movementKeys );
            m_controlledLegs.Init( m_movementKeys );
            m_controlledLean.Init();
        }

        void Update() {
            m_movementKeys.HandleInput(); // input
            m_movement.HandleMovement();
            m_controlledArms.HandleArms();
            m_controlledKnees.HandleKnees();
            m_controlledLegs.HandleLegs();
            m_controlledLean.HandleLean();

            if ( m_movementKeys.m_left ) m_controlledLean.LeanLeft();
            if ( m_movementKeys.m_right ) m_controlledLean.LeanRight();
        }

        void FixedUpdate() {
            if ( m_movementKeys.m_left && m_leftRoutine == null ) {
                m_leftRoutine = StartCoroutine( RunMoveLeft() );
            }

            if ( m_movementKeys.m_right && m_rightRoutine == null ) {
                m_rightRoutine = StartCoroutine( RunMoveRight() );
            }

            m_movementKeys.m_left = false;
            m_movementKeys.m_right = false;
        }

        IEnumerator RunMoveLeft() {
            yield return StartCoroutine( m_controlledLegs.MoveLeft() );
            m_leftRoutine = null;
        }

        IEnumerator RunMoveRight() {
            yield return StartCoroutine( m_controlledLegs.MoveRight() );
            m_rightRoutine = null;
        }

        [Button] public void ToggleKinematic() {
            m_movement.ToggleKinematic();
        }

        void OnDrawGizmosSelected() {
            if ( m_movement != null ) {
                m_movement.OnDrawGizmosSelected();
            }
        }
    }

    [Serializable] public class ControlledArms {
        [SerializeField] LimbSettings m_leftArmLimbSettings;
        [SerializeField] LimbSettings m_rightArmLimbSettings;
        
        MovementKeys m_movementKeys;
        public void Init(MovementKeys movementKeys) {
            m_movementKeys = movementKeys;
            m_leftArmLimbSettings.Init();
            m_rightArmLimbSettings.Init();
        }
        
        public void HandleArms() {
        }
    }

    [Serializable] public class ControlledLean {
        [SerializeField] Balance m_lowerTorso;
        [SerializeField] Balance m_upperTorso;

        public float m_lowerTargetRotation;
        public float m_lowerForce = 25000f;
        public float m_upperTargetRotation;
        public float m_upperForce = 25000f;
        public float m_lerpSpeed = 5f;

        public void Init() {
            if ( m_lowerTorso ) {
                m_lowerTorso.targetRotation = m_lowerTargetRotation;
                m_lowerTorso.force = m_lowerForce;
            }

            if ( m_upperTorso ) {
                m_upperTorso.targetRotation = m_upperTargetRotation;
                m_upperTorso.force = m_upperForce;
            }
        }

        public void HandleLean() {
            if ( m_lowerTorso ) {
                m_lowerTorso.targetRotation = Mathf.Lerp( m_lowerTorso.targetRotation, m_lowerTargetRotation, Time.deltaTime * m_lerpSpeed );
                m_lowerTorso.force = Mathf.Lerp( m_lowerTorso.force, m_lowerForce, Time.deltaTime * m_lerpSpeed );
            }

            if ( m_upperTorso ) {
                m_upperTorso.targetRotation = Mathf.Lerp( m_upperTorso.targetRotation, m_upperTargetRotation, Time.deltaTime * m_lerpSpeed );
                m_upperTorso.force = Mathf.Lerp( m_upperTorso.force, m_upperForce, Time.deltaTime * m_lerpSpeed );
            }
        }

        public void LeanLeft() {
            m_lowerTargetRotation = 20f;
            m_upperTargetRotation = 10f;
        }

        public void LeanRight() {
            m_lowerTargetRotation = -20f;
            m_upperTargetRotation = -10f;
        }
    }

    [Serializable] public class ControlledLegs {
        [Header( "Rigidbody2D References" )]
        [SerializeField] Rigidbody2D m_leftLegRb;
        [SerializeField] Rigidbody2D m_rightLegRb;
        public float m_legForce = 1f;
        public float m_kickDelay = .5f;
        [Header( "Leg Bend Settings" )]
        [SerializeField] HingeJoint2D m_leftHip;
        [SerializeField] HingeJoint2D m_rightHip;
        public float m_bendAngle = 45f;
        public float m_bendGap;
        public bool m_reverseGap;
        MovementKeys m_movementKeys;
        WaitForSeconds m_waitForSeconds;
        float m_currentGap;
        public void Init(MovementKeys movementKeys) {
            m_movementKeys = movementKeys;
            if ( !m_leftHip || !m_rightHip ) {
                Debug.LogError( "One or more required HingeJoint2D references are not set in the inspector." );
                return;
            }

            if ( !m_leftLegRb || !m_rightLegRb ) {
                Debug.LogError( "One or more required Rigidbody2D references are not set in the inspector." );
                return;
            }

            if ( m_reverseGap ) m_currentGap = -m_bendGap;

            SetHipLimits( m_leftHip, -m_bendAngle, m_currentGap );
            SetHipLimits( m_rightHip, m_currentGap, -m_bendAngle );

            m_waitForSeconds = new WaitForSeconds( m_kickDelay );
        }

        public void HandleLegs() {
            if ( !m_leftHip || !m_rightHip ) return;

            if ( m_reverseGap ) m_currentGap = -m_bendGap;
            else m_currentGap = m_bendGap;

            if ( m_movementKeys.m_left ) {
                SetHipLimits( m_leftHip, -m_bendAngle, m_currentGap );
                SetHipLimits( m_rightHip, m_currentGap, -m_bendAngle );
            }
            else if ( m_movementKeys.m_right ) {
                SetHipLimits( m_leftHip, -m_bendAngle, m_currentGap );
                SetHipLimits( m_rightHip, m_currentGap, -m_bendAngle );
            }
            else {
                SetHipLimits( m_leftHip, -m_bendAngle, m_currentGap );
                SetHipLimits( m_rightHip, m_currentGap, -m_bendAngle );
            }
        }

        public IEnumerator MoveRight() {
            while (m_movementKeys.m_right) {
                if ( m_movementKeys.m_left ) yield break;
                ZeroX( m_leftLegRb );
                m_leftLegRb.AddForce( Vector2.right * (m_legForce * 1000 * Time.fixedDeltaTime) );
                yield return m_waitForSeconds;
                if ( !m_movementKeys.m_right ) yield break;
                ZeroX( m_rightLegRb );
                m_rightLegRb.AddForce( Vector2.right * (m_legForce * 1000 * Time.fixedDeltaTime) );
                yield return m_waitForSeconds;
            }
        }

        public IEnumerator MoveLeft() {
            while (m_movementKeys.m_left) {
                if ( m_movementKeys.m_right ) yield break;
                ZeroX( m_rightLegRb );
                m_rightLegRb.AddForce( Vector2.left * (m_legForce * 1000 * Time.fixedDeltaTime) );
                yield return m_waitForSeconds;
                if ( !m_movementKeys.m_left ) yield break;
                ZeroX( m_leftLegRb );
                m_leftLegRb.AddForce( Vector2.left * (m_legForce * 1000 * Time.fixedDeltaTime) );
                yield return m_waitForSeconds;
            }
        }

        void ZeroX(Rigidbody2D rb) {
            var v = rb.linearVelocity;
            v.x = 0f;
            rb.linearVelocity = v;
        }


        void SetHipLimits(HingeJoint2D leftHip, float p1, float currentOffset, bool flip = false) {
            if ( !flip ) {
                var limits = leftHip.limits;
                limits.min = p1;
                limits.max = currentOffset;
                leftHip.limits = limits;
            }
            else {
                var limits = leftHip.limits;
                limits.min = currentOffset;
                limits.max = p1;
                leftHip.limits = limits;
            }

            leftHip.useLimits = true;
        }
    }

    [Serializable] public class ControlledKnees {
        [Header( "Knee Settings" )]
        [SerializeField] HingeJoint2D m_leftKnee;
        [SerializeField] HingeJoint2D m_rightKnee;
        public float m_bendAngle = 45f;
        public float m_bendGap;
        public bool m_reverseGap;
        MovementKeys m_movementKeys;
        float m_currentGap;
        public void Init(MovementKeys movementKeys) {
            m_movementKeys = movementKeys;
            if ( !m_leftKnee || !m_rightKnee ) {
                Debug.LogError( "One or more required HingeJoint2D references are not set in the inspector." );
                return;
            }

            if ( m_reverseGap ) m_currentGap = -m_bendGap;

            SetKneeLimits( m_leftKnee, -m_bendAngle, m_currentGap );
            SetKneeLimits( m_rightKnee, m_currentGap, -m_bendAngle );
        }

        public void HandleKnees() {
            if ( !m_leftKnee || !m_rightKnee ) return;

            if ( m_reverseGap ) m_currentGap = -m_bendGap;
            else m_currentGap = m_bendGap;

            if ( m_movementKeys.m_left ) {
                SetKneeLimits( m_leftKnee, -m_bendAngle, m_currentGap );
                SetKneeLimits( m_rightKnee, m_currentGap, -m_bendAngle );
            }
            else if ( m_movementKeys.m_right ) {
                SetKneeLimits( m_leftKnee, m_bendAngle, m_currentGap );
                SetKneeLimits( m_rightKnee, m_currentGap, m_bendAngle );
            }
            else {
                SetKneeLimits( m_leftKnee, -m_bendAngle, m_currentGap );
                SetKneeLimits( m_rightKnee, m_currentGap, -m_bendAngle );
            }
        }

        void SetKneeLimits(HingeJoint2D rightKnee, float p1, float p2, bool flip = false) {
            if ( !flip ) {
                var limits = rightKnee.limits;
                limits.min = p1;
                limits.max = p2;
                rightKnee.limits = limits;
            }
            else {
                var limits = rightKnee.limits;
                limits.min = p2;
                limits.max = p1;
                rightKnee.limits = limits;
            }

            rightKnee.useLimits = true;
        }
    }

    [Serializable] public class LimbSettings {
        [Header( "Hinge Joint Settings" )]
        public HingeJoint2D m_hingeJoint;
        public bool m_useMotor;
        public float m_motorSpeed = 1000;
        public float m_motorForce = 1000;
        public bool m_useLimits;
        public float m_limitMin = -45;
        public float m_limitMax = 45;
        JointAngleLimits2D m_limits;

        public void Init() {
            if ( m_hingeJoint ) {
                if ( m_useMotor ) {
                    var motor = m_hingeJoint.motor;
                    motor.motorSpeed = m_motorSpeed;
                    motor.maxMotorTorque = m_motorForce;
                    m_hingeJoint.motor = motor;
                    m_hingeJoint.useMotor = true;
                }
                else {
                    m_hingeJoint.useMotor = false;
                }

                if ( m_useLimits ) {
                    m_limits = new JointAngleLimits2D {
                        min = m_limitMin,
                        max = m_limitMax,
                    };
                    m_hingeJoint.limits = m_limits;
                    m_hingeJoint.useLimits = true;
                }
                else {
                    m_hingeJoint.useLimits = false;
                }
            }
        }
    }

    [Serializable] public class MovementKeys {
        [SerializeField] KeyCode m_jumpKey = KeyCode.Space;
        [SerializeField] KeyCode m_leftKey = KeyCode.A;
        [SerializeField] KeyCode m_rightKey = KeyCode.D;

        public bool m_jump;
        public bool m_left;
        public bool m_right;

        public bool m_isPlayerControlled = true;

        public void HandleInput() {
            if ( !m_isPlayerControlled ) return;
            m_jump = Input.GetKey( m_jumpKey );
            m_left = Input.GetKey( m_leftKey );
            m_right = Input.GetKey( m_rightKey );
        }
    }

    [Serializable] public class Movement {
        [SerializeField] Rigidbody2D m_rb;
        [SerializeField] float m_speed = 5f;
        [SerializeField] float m_jumpForce = 10f;
        [SerializeField] float m_jumpCooldown = 1f;
        public bool m_isGrounded;
        public bool m_canJump = true;
        public LayerMask m_groundLayer;
        public float m_groundCheckOffset = 0.1f;
        public float m_groundCheckRadius = 0.2f;

        Transform m_playerPos;
        MovementKeys m_movementKeys;

        public void Init(MovementKeys movementKeys) {
            m_movementKeys = movementKeys;
            if ( !m_rb ) {
                Debug.LogError( "Rigidbody2D reference is not set in the inspector." );
                return;
            }

            m_playerPos = m_rb.transform;
            m_isGrounded = false;
            m_canJump = true;
        }

        // TODO: Spamming left and right builds momentum
        public void HandleMovement() {
            if ( !m_rb ) return;

            if ( m_movementKeys.m_left && m_movementKeys.m_right
                 || !m_movementKeys.m_left && !m_movementKeys.m_right ) {
                m_rb.linearVelocity = new Vector2( 0, m_rb.linearVelocity.y );
            }
            else if ( m_movementKeys.m_left ) {
                m_rb.linearVelocity = new Vector2( -m_speed, m_rb.linearVelocity.y );
            }
            else {
                m_rb.linearVelocity = new Vector2( m_speed, m_rb.linearVelocity.y );
            }

            HandleJump();
        }

        void HandleJump() {
            if ( m_movementKeys.m_jump == false || m_canJump == false ) return;
            m_isGrounded = Physics2D.OverlapCircle(
                m_playerPos.position + Vector3.down * m_groundCheckOffset,
                m_groundCheckRadius,
                m_groundLayer
            );

            if ( !m_isGrounded ) return;
            m_rb.bodyType = RigidbodyType2D.Dynamic;
            m_rb.AddForce( Vector2.up * m_jumpForce );
        }

        public void ToggleKinematic() {
            if ( !m_rb ) return;
            if ( m_rb.bodyType == RigidbodyType2D.Kinematic ) {
                m_rb.bodyType = RigidbodyType2D.Dynamic;
            }
            else {
                m_rb.bodyType = RigidbodyType2D.Kinematic;
                m_rb.linearVelocity = Vector2.zero;
            }
        }

        void ZeroX(Rigidbody2D rb) {
            var v = rb.linearVelocity;
            v.x = 0f;
            rb.linearVelocity = v;
        }

        public void OnDrawGizmosSelected() {
            if ( m_playerPos == null ) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(
                m_playerPos.position + Vector3.down * m_groundCheckOffset,
                m_groundCheckRadius
            );
        }
    }
}