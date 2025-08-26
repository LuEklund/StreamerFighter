using System;
using System.Collections;
using Stickman;
using UnityEngine;
namespace SMRevamp {
    [Serializable] public class LimbSettings {
        [Header( "Balance Component" )]
        public Balance m_balance;
        public float m_targetRotation;
        public float m_force;
        public float m_targetMultiplier = 10;
        [Header( "Hinge Joint Settings" )]
        public HingeJoint2D m_hingeJoint;
        public bool m_useMotor;
        public float m_motorSpeed = 1000;
        public float m_motorForce = 1000;
        public bool m_useLimits;
        public JointAngleLimits2D Limits = new() { min = -45, max = 45 };

        public void Init() {
            m_balance.targetRotation = m_targetRotation;
            m_balance.force = m_force;
            m_balance.targetMultiplier = m_targetMultiplier;

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
                    m_hingeJoint.limits = Limits;
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

    public class StickmanMotor : MonoBehaviour {
        public MovementKeys m_movementKeys = new();
        public Movement m_movement;

        [Header( "Limb Settings" )]
        public bool m_updateChanges;
        public LimbSettings m_headSettings;
        public LimbSettings m_torsoSettings;
        public LimbSettings m_leftLegSettings;
        public LimbSettings m_lowerLeftLegSettings;
        public LimbSettings m_rightLegSettings;
        public LimbSettings m_lowerRightLegSettings;

        public void Awake() {
            if ( !m_movement.Init( m_movementKeys ) ) {
                Debug.LogError( "Movement initialization failed. Please check the Rigidbody2D references in the inspector." );
                enabled = false; // Disable this component if initialization fails
            }

            LimbSettings[] settingsArray = LimbSettingsArray();
            foreach (var balanceSetting in settingsArray) {
                balanceSetting.Init();
            }
        }

        void Update() {
            StartCoroutine( m_movement.Update() );

            if ( m_updateChanges ) {
                LimbSettings[] settingsArray = LimbSettingsArray();
                foreach (var balanceSetting in settingsArray) {
                    balanceSetting.Init();
                }
            }
        }

        LimbSettings[] LimbSettingsArray() {
            LimbSettings[] settingsArray = {
                m_headSettings,
                m_torsoSettings,
                m_leftLegSettings,
                m_lowerLeftLegSettings,
                m_rightLegSettings,
                m_lowerRightLegSettings
            };
            return settingsArray;
        }
    }

    [Serializable] public class Movement {
        [Header( "Rigidbody2D References" )]
        public Rigidbody2D m_leftLegRb;
        public Rigidbody2D m_lowerLeftLegRb;
        public Rigidbody2D m_rightLegRb;
        public Rigidbody2D m_lowerRightLegRb;
        [Header( "Movement Settings" )]
        [SerializeField] Animator m_anim;
        [SerializeField] float m_speed = 2f;
        [SerializeField] float m_lowerSpeed = 2f;
        [SerializeField] float m_jumpHeight = 2f;
        [SerializeField] float m_legWait = .5f;
        [Header( "Jump Settings" )]
        [SerializeField] Rigidbody2D m_rb;
        [SerializeField] float m_jumpForce = 10f;
        public bool m_isGrounded = false;
        public float m_positionRadius;
        public LayerMask m_groundLayer;
        public Transform m_playerPos;

        MovementKeys m_movementKeys;

        public bool Init(MovementKeys movementKeys) {
            if ( AreRBsNull() ) {
                Debug.LogError( "One or more required Rigidbody2D references are not set in the inspector." );
                return false;
            }

            m_movementKeys = movementKeys;
            return true;
        }

        public IEnumerator Update() {
            if ( Input.GetKey( m_movementKeys.m_leftKey ) ) {
                m_anim.Play( "WalkLeft" );
                yield return MoveRight( m_legWait );
            }
            else if ( Input.GetKey( m_movementKeys.m_rightKey ) ) {
                m_anim.Play( "WalkRight" );
                yield return MoveLeft( m_legWait );
            }
            else {
                m_anim.Play( "Idle" );
            }

            HandleJump();
            yield return null;
        }

        void HandleJump() {
            m_isGrounded = Physics2D.OverlapCircle( m_playerPos.position, m_positionRadius, m_groundLayer );
            if ( m_isGrounded && Input.GetKeyDown( m_movementKeys.m_jumpKey ) ) {
                m_rb.AddForce( Vector2.up * m_jumpForce );
            }
        }

        IEnumerator MoveLeft(float seconds) {
            m_leftLegRb.AddForce( Vector2.right * (m_speed * 1000 * Time.deltaTime) );
            m_lowerLeftLegRb.AddForce( Vector2.right * (m_lowerSpeed * 1000 * Time.deltaTime) );
            yield return new WaitForSeconds( seconds );
            m_rightLegRb.AddForce( Vector2.right * (m_speed * 1000 * Time.deltaTime) );
            m_lowerRightLegRb.AddForce( Vector2.right * (m_lowerSpeed * 1000 * Time.deltaTime) );
        }

        IEnumerator MoveRight(float seconds) {
            m_rightLegRb.AddForce( Vector2.left * (m_speed * 1000 * Time.deltaTime) );
            m_lowerRightLegRb.AddForce( Vector2.left * (m_lowerSpeed * 1000 * Time.deltaTime) );
            yield return new WaitForSeconds( seconds );
            m_lowerLeftLegRb.AddForce( Vector2.left * (m_lowerSpeed * 1000 * Time.deltaTime) );
            m_leftLegRb.AddForce( Vector2.left * (m_speed * 1000 * Time.deltaTime) );
        }

        bool AreRBsNull() => !m_leftLegRb || !m_lowerLeftLegRb || !m_rightLegRb || !m_lowerRightLegRb;
    }
}