using System;
using System.Collections;
using Stickman;
using TCS.Utils;
using UnityEngine;
using UnityEngine.Serialization;
namespace SMRevamp {
    public class StickmanMotor : MonoBehaviour {
        [Header( "Components" )]
        public MovementKeys m_movementKeys = new();
        public Movement m_movement = new();
        public ControlledKnees m_controlledKnees = new();
        public ControlledLegs m_controlledLegs = new();
        public ControlledLean m_controlledLean = new();

        [Header( "Limb Settings" )] // most likely going to remove this
        public bool m_updateChanges;
        public LimbSettings m_headSettings;
        //public LimbSettings m_torsoSettings;
        public LimbSettings m_leftLegSettings;
        public LimbSettings m_lowerLeftLegSettings;
        public LimbSettings m_rightLegSettings;
        public LimbSettings m_lowerRightLegSettings;

        public void Awake() {
            // if ( !m_legMovements.Init( m_movementKeys ) ) {
            //     Debug.LogError( "LegMovements initialization failed. Please check the Rigidbody2D references in the inspector." );
            //     enabled = false; // Disable this component if initialization fails
            // }
            //
            // LimbSettings[] settingsArray = LimbSettingsArray();
            // foreach (var balanceSetting in settingsArray) {
            //     balanceSetting.Init();
            // }

            m_movement.Init( m_movementKeys );

            //m_proceduralWalker.Init( m_movementKeys );
            
            m_controlledKnees.Init( m_movementKeys );
            m_controlledLegs.Init( m_movementKeys );
            m_controlledLean.Init();
        }

        void Update() {
            m_movement.HandleMovement();
            m_controlledKnees.HandleKnees();
            m_controlledLegs.HandleLegs();
            m_controlledLean.HandleLean();
            
            // TODO: Holding both left and right stops kick routines, until all keys are released
            if ( Input.GetKeyDown( m_movementKeys.m_leftKey ) ) {
                StartCoroutine( m_controlledLegs.MoveLeft() );
                m_controlledLean.LeanLeft();
            }
            else if ( Input.GetKeyDown( m_movementKeys.m_rightKey ) ) {
                StartCoroutine( m_controlledLegs.MoveRight() );
                m_controlledLean.LeanRight();
            }
        }

        // LimbSettings[] LimbSettingsArray() {
        //     LimbSettings[] settingsArray = {
        //         m_headSettings,
        //         //m_torsoSettings,
        //         m_leftLegSettings,
        //         m_lowerLeftLegSettings,
        //         m_rightLegSettings,
        //         m_lowerRightLegSettings
        //     };
        //     return settingsArray;
        // }
        
        // [Button] public void TestLegForce() {
        //     m_controlledLegs.AddWalkForce();
        // }
        
        [Button] public void ToggleKinematic( ) {
            m_movement.ToggleKinematic();
        }

        void OnDrawGizmosSelected() {
            if ( m_movement != null ) {
                m_movement.OnDrawGizmosSelected();
            }
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
        
        // public void HandleLean() {
        //     if ( m_lowerTorso ) {
        //         m_lowerTorso.targetRotation = m_lowerTargetRotation;
        //         m_lowerTorso.force = m_lowerForce;
        //     }
        //
        //     if ( m_upperTorso ) {
        //         m_upperTorso.targetRotation = m_upperTargetRotation;
        //         m_upperTorso.force = m_upperForce;
        //     }
        // }
        
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
        [Header("Leg Bend Settings")]
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
            SetHipLimits( m_rightHip, m_currentGap, -m_bendAngle);
            
            m_waitForSeconds = new WaitForSeconds(m_kickDelay);
        }
        
        public void HandleLegs() {
            if ( !m_leftHip || !m_rightHip ) return;
            
            if ( m_reverseGap ) m_currentGap = -m_bendGap;
            else m_currentGap = m_bendGap;
            
            if ( Input.GetKey( m_movementKeys.m_leftKey ) ) {
                SetHipLimits( m_leftHip, -m_bendAngle, m_currentGap );
                SetHipLimits( m_rightHip, m_currentGap, -m_bendAngle);
            }
            else if ( Input.GetKey( m_movementKeys.m_rightKey ) ) {
                SetHipLimits( m_leftHip, -m_bendAngle, m_currentGap );
                SetHipLimits( m_rightHip, m_currentGap, -m_bendAngle);
            }
            else {
                SetHipLimits( m_leftHip, -m_bendAngle, m_currentGap );
                SetHipLimits( m_rightHip, m_currentGap, -m_bendAngle);
            }
        }

        // public void AddWalkForce(bool flip = false) {
        //     if ( !flip ) {
        //         m_leftLegRb.AddForce( Vector2.left * (1000 * Time.deltaTime) );
        //         m_rightLegRb.AddForce( Vector2.left * (1000 * Time.deltaTime) );
        //     }
        //     else {
        //         m_leftLegRb.AddForce( Vector2.right * (1000 * Time.deltaTime) );
        //         m_rightLegRb.AddForce( Vector2.right * (1000 * Time.deltaTime) );
        //     }
        // }

        public IEnumerator MoveRight() {
            while (Input.GetKey(m_movementKeys.m_rightKey)) {
                if (Input.GetKey(m_movementKeys.m_leftKey)) yield break;
                ZeroX(m_leftLegRb);
                m_leftLegRb.AddForce(Vector2.right * (m_legForce * 1000 * Time.deltaTime));
                yield return m_waitForSeconds;
                if (!Input.GetKey(m_movementKeys.m_rightKey)) yield break;
                ZeroX(m_rightLegRb);
                m_rightLegRb.AddForce(Vector2.right * (m_legForce * 1000 * Time.deltaTime));
                yield return m_waitForSeconds;
            }
        }

        public IEnumerator MoveLeft() {
            while (Input.GetKey(m_movementKeys.m_leftKey)) {
                if (Input.GetKey(m_movementKeys.m_rightKey)) yield break;
                ZeroX(m_rightLegRb);
                m_rightLegRb.AddForce(Vector2.left * (m_legForce * 1000 * Time.deltaTime));
                yield return m_waitForSeconds;
                if (!Input.GetKey(m_movementKeys.m_leftKey)) yield break;
                ZeroX(m_leftLegRb);
                m_leftLegRb.AddForce(Vector2.left * (m_legForce * 1000 * Time.deltaTime));
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
            SetKneeLimits( m_rightKnee, m_currentGap, -m_bendAngle);
        }
        
        public void HandleKnees() {
            if ( !m_leftKnee || !m_rightKnee ) return;
            
            if ( m_reverseGap ) m_currentGap = -m_bendGap;
            else m_currentGap = m_bendGap;
            
            if ( Input.GetKey( m_movementKeys.m_leftKey ) ) {
                SetKneeLimits( m_leftKnee, -m_bendAngle, m_currentGap );
                SetKneeLimits( m_rightKnee, m_currentGap, -m_bendAngle);
            }
            else if ( Input.GetKey( m_movementKeys.m_rightKey ) ) {
                SetKneeLimits( m_leftKnee, -m_bendAngle, m_currentGap );
                SetKneeLimits( m_rightKnee, m_currentGap, -m_bendAngle);
            }
            else {
                SetKneeLimits( m_leftKnee, -m_bendAngle, m_currentGap );
                SetKneeLimits( m_rightKnee, m_currentGap, -m_bendAngle);
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
        [Header( "Balance Component" )]
        public Balance m_balance;
        public float m_targetRotation;
        public float m_force;
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
            m_balance.targetRotation = m_targetRotation;
            m_balance.force = m_force;

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
        public KeyCode m_jumpKey = KeyCode.Space;
        public KeyCode m_leftKey = KeyCode.A;
        public KeyCode m_rightKey = KeyCode.D;
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

        KeyCode m_jumpKey;
        KeyCode m_leftKey;
        KeyCode m_rightKey;
        Transform m_playerPos;

        public void Init(MovementKeys movementKeys) {
            m_jumpKey = movementKeys.m_jumpKey;
            m_leftKey = movementKeys.m_leftKey;
            m_rightKey = movementKeys.m_rightKey;
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

            bool left = Input.GetKey( m_leftKey );
            bool right = Input.GetKey( m_rightKey );

            if ( ( left && right ) || ( !left && !right ) ) {
                m_rb.linearVelocity = new Vector2( 0, m_rb.linearVelocity.y );
            }
            else if ( left ) {
                m_rb.linearVelocity = new Vector2( -m_speed, m_rb.linearVelocity.y );
            }
            else {
                m_rb.linearVelocity = new Vector2( m_speed, m_rb.linearVelocity.y );
            }

            HandleJump();
        }

        void HandleJump() {
            if ( !Input.GetKeyDown( m_jumpKey ) && m_canJump ) return;
            m_isGrounded = Physics2D.OverlapCircle(
                m_playerPos.position + Vector3.down * m_groundCheckOffset,
                m_groundCheckRadius,
                m_groundLayer
            );
            if ( m_isGrounded && Input.GetKeyDown( m_jumpKey ) ) {
                m_rb.bodyType = RigidbodyType2D.Dynamic;
                m_rb.AddForce( Vector2.up * m_jumpForce );
            }
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