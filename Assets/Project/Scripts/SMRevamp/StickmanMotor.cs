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
        public ProceduralWalker2D m_proceduralWalker = new(); // gpt example
        public ControlledKnees m_controlledKnees = new();
        public ControlledLegs m_controlledLegs = new();
        public LegMovements m_legMovements;

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
        }

        void Update() {
            //StartCoroutine( m_legMovements.Update() );

            // if ( m_updateChanges ) {
            //     LimbSettings[] settingsArray = LimbSettingsArray();
            //     foreach (var balanceSetting in settingsArray) {
            //         balanceSetting.Init();
            //     }
            // }

            m_movement.HandleMovement();
            m_controlledKnees.HandleKnees();
            m_controlledLegs.HandleLegs();
            
            // TODO: Holding both left and right stops kick routines, until all keys are released
            if ( Input.GetKeyDown( m_movementKeys.m_leftKey ) ) {
                StartCoroutine( m_controlledLegs.MoveLeft() );
            }
            else if ( Input.GetKeyDown( m_movementKeys.m_rightKey ) ) {
                StartCoroutine( m_controlledLegs.MoveRight() );
            }
        }

        // void FixedUpdate() {
        //     m_proceduralWalker.FixedUpdate();
        // }

        LimbSettings[] LimbSettingsArray() {
            LimbSettings[] settingsArray = {
                m_headSettings,
                //m_torsoSettings,
                m_leftLegSettings,
                m_lowerLeftLegSettings,
                m_rightLegSettings,
                m_lowerRightLegSettings
            };
            return settingsArray;
        }
        
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

    [Serializable] public class LegMovements {
        [Header( "Rigidbody2D References" )]
        public Rigidbody2D m_leftLegRb;
        public Rigidbody2D m_lowerLeftLegRb;
        public Rigidbody2D m_rightLegRb;
        public Rigidbody2D m_lowerRightLegRb;
        [Header( "LegMovements Settings" )]
        [SerializeField] Animator m_anim;
        [SerializeField] float m_speed = 2f;
        [SerializeField] float m_lowerSpeed = 2f;
        [SerializeField] float m_legWait = .5f;

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

            yield return null;
        }

        // TODO: Figure out foot movement.
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

    // Procedural 2D Walker by ChatGPT
    [Serializable] public class ProceduralWalker2D {
        [Header( "Rigidbodies" )]
        [SerializeField] Rigidbody2D pelvis;

        [Header( "Joints (set limits in Inspector)" )]
        [SerializeField] HingeJoint2D leftHip;
        [SerializeField] HingeJoint2D leftKnee;
        [SerializeField] HingeJoint2D rightHip;
        [SerializeField] HingeJoint2D rightKnee;

        [Header( "Facing / Input" )]
        [Tooltip( "If false, script derives facing from input keys" )]
        public bool useFaceBool = true;
        [Tooltip( "True = facing right, False = facing left" )]
        public bool faceRight = true;
        MovementKeys movementKeys;

        [Header( "Gait" )]
        [SerializeField, Tooltip( "Steps per second at full input" )]
        float stepFrequency = 2.2f;
        [SerializeField, Tooltip( "Hip swing amplitude (deg)" )]
        float hipAmplitude = 18f;
        [SerializeField, Tooltip( "Extra knee bend during swing (deg)" )]
        float kneeAmplitude = 25f;
        [SerializeField, Tooltip( "Baseline knee bend (deg)" )]
        float neutralKnee = 8f;

        [Header( "Motor PD" )]
        [SerializeField] float kp = 8f; // proportional gain (lowered)
        [SerializeField] float kd = 1.0f; // damping gain
        [SerializeField] float maxMotorSpeed = 420f; // deg/s clamp
        [SerializeField] float maxMotorTorque = 600f; // tune to masses
        [SerializeField, Tooltip( "How fast the commanded motor speed can change (deg/s^2)" )]
        float motorAccel = 3000f;

        [Header( "Per-joint orientation (flip if it moves the wrong way)" )]
        [SerializeField] int leftHipSign = 1;
        [SerializeField] int rightHipSign = 1;
        [SerializeField] int leftKneeSign = 1;
        [SerializeField] int rightKneeSign = 1;

        float phase; // 0..2π
        float lHipZero, rHipZero, lKneeZero, rKneeZero; // neutral angles (deg)
        float spLHip, spRHip, spLKnee, spRKnee; // last motor speeds for smoothing

        public void Init(MovementKeys movementKeys1) {
            movementKeys = movementKeys1;
            // Capture each joint's neutral angle at startup so targets are relative to YOUR rig.
            lHipZero = leftHip.jointAngle;
            rHipZero = rightHip.jointAngle;
            lKneeZero = leftKnee.jointAngle;
            rKneeZero = rightKnee.jointAngle;

            // Ensure motors are enabled and torques set.
            PrepareMotor( leftHip );
            PrepareMotor( rightHip );
            PrepareMotor( leftKnee );
            PrepareMotor( rightKnee );
        }

        void PrepareMotor(HingeJoint2D j) {
            j.useLimits = true; // set sensible limits in Inspector per joint
            var m = j.motor;
            m.maxMotorTorque = maxMotorTorque;
            m.motorSpeed = 0f;
            j.motor = m;
            j.useMotor = true;
        }

        public void FixedUpdate() {
            // input (-1..1)
            float move = 0f;
            if ( movementKeys != null ) {
                if ( Input.GetKey( movementKeys.m_leftKey ) ) move -= 1f;
                if ( Input.GetKey( movementKeys.m_rightKey ) ) move += 1f;
            }

            float absMove = Mathf.Abs( move );

            // decide facing
            if ( !useFaceBool )
                faceRight = move >= 0f; // default if using input

            int facingSign = faceRight ? 1 : -1;

            if ( absMove > 0.01f ) {
                // advance phase; scale with input so slow press = slow walk
                float freq = Mathf.Lerp( 0.8f * stepFrequency, stepFrequency, absMove );
                phase += freq * Time.fixedDeltaTime * Mathf.PI * 2f; // in radians
                if ( phase > Mathf.PI * 2f ) phase -= Mathf.PI * 2f;

                // Hip swing (legs 180° out of phase)
                float hipL = facingSign * hipAmplitude * Mathf.Sin( phase );
                float hipR = -hipL;

                // Knee bend mostly on swing (half-wave rectified)
                float swingL = Mathf.Max( 0f, Mathf.Sin( phase + Mathf.PI * 0.5f ) );
                float swingR = Mathf.Max( 0f, Mathf.Sin( phase + Mathf.PI * 0.5f + Mathf.PI ) );
                float kneeL = neutralKnee + kneeAmplitude * swingL;
                float kneeR = neutralKnee + kneeAmplitude * swingR;

                DriveRelative( leftHip, lHipZero, hipL, leftHipSign, ref spLHip );
                DriveRelative( rightHip, rHipZero, hipR, rightHipSign, ref spRHip );
                DriveRelative( leftKnee, lKneeZero, kneeL, leftKneeSign, ref spLKnee );
                DriveRelative( rightKnee, rKneeZero, kneeR, rightKneeSign, ref spRKnee );
            }
            else {
                // Idle: smoothly hold neutral pose
                phase = 0f; // so the first step is consistent
                HoldNeutral( leftHip, lHipZero, ref spLHip );
                HoldNeutral( rightHip, rHipZero, ref spRHip );
                HoldNeutral( leftKnee, lKneeZero + leftKneeSign * neutralKnee, ref spLKnee );
                HoldNeutral( rightKnee, rKneeZero + rightKneeSign * neutralKnee, ref spRKnee );
            }
        }

        void HoldNeutral(HingeJoint2D j, float targetAbsDeg, ref float lastSpd) {
            DriveToAbsolute( j, targetAbsDeg, ref lastSpd, kp * 0.6f );
        }

        void DriveRelative(HingeJoint2D j, float zeroDeg, float targetRelDeg, int sign, ref float lastSpd) {
            float targetAbs = zeroDeg + sign * targetRelDeg;
            DriveToAbsolute( j, targetAbs, ref lastSpd, kp );
        }

        void DriveToAbsolute(HingeJoint2D j, float targetAbsDeg, ref float lastSpd, float Pgain) {
            // error in degrees (-180..180)
            float err = Mathf.DeltaAngle( j.jointAngle, targetAbsDeg );
            // PD -> desired speed
            float desired = Mathf.Clamp( err * Pgain - j.jointSpeed * kd, -maxMotorSpeed, maxMotorSpeed );
            // ramp to avoid instant jolts
            float maxStep = motorAccel * Time.fixedDeltaTime;
            float cmd = Mathf.MoveTowards( lastSpd, desired, maxStep );
            lastSpd = cmd;

            var m = j.motor;
            m.motorSpeed = cmd;
            m.maxMotorTorque = maxMotorTorque;
            j.motor = m;
            j.useMotor = true;
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