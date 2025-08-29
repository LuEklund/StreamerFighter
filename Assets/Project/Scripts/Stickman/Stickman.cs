using System;
using System.Collections;
using Game;
using TCS.Utils;
using UnityEngine;
using UnityEngine.Serialization;
using Utils;
using Logger = TCS.Utils.Logger;
namespace Stickman {
    public class Stickman : MonoBehaviour {
        [ReadOnly] public string m_guid;
        [SerializeField] Health m_health;
        public GameObject m_torso;

        [Header( "Weapon" )]
        public WeaponManager m_weaponManager = new();

        [Header( "Movement" )]
        public bool m_canMove = true;
        public MovementKeys m_movementKeys = new();
        public Movement m_movement = new();
        public ControlledArms m_controlledArms = new();
        public ControlledKnees m_controlledKnees = new();
        public ControlledLegs m_controlledLegs = new();
        public ControlledLean m_controlledLean = new();

        Coroutine m_leftRoutine;
        Coroutine m_rightRoutine;
        DamageUIManager m_damageUIManager;
        
        public HandSelection Direction {
            get => m_weaponManager.m_direction;
            set => m_weaponManager.m_direction = value;
        }
        
        public HandSelection m_direction = HandSelection.None; // 0=Left, 1=Right, else hide all

        public bool IsMe(string guid) => m_guid == guid; // answers one question, is this me?

        public bool IsPlayerControlled {
            get => m_movementKeys.m_isPlayerControlled;
            set => m_movementKeys.m_isPlayerControlled = value;
        }
        
        bool m_isAttacking;
        [Button] public void ToggleAttack() {
            if ( m_isAttacking != true ) {
                m_controlledArms.Attack( Direction );
                m_isAttacking = true;
                return;
            }
            
            m_controlledArms.Attack( HandSelection.None );
            m_isAttacking = false;
        }


        public void Awake() {
            m_guid = Guid.NewGuid().ToString( "N" ); // This gives us a personal ID.
            m_damageUIManager = FindFirstObjectByType<DamageUIManager>( FindObjectsInactive.Include );
            if ( m_damageUIManager == null ) {
                Logger.LogError( "Stickman: No DamageUIManager found in the scene.", this );
            }
            
            m_weaponManager.Init( m_guid );

            m_movement.Init( m_movementKeys );
            m_controlledKnees.Init( m_movementKeys );
            m_controlledLegs.Init( m_movementKeys );
            m_controlledLean.Init();
        }

        void Update() {
            if ( !m_canMove ) return; // need this for testing.

            m_movementKeys.HandleInput(); // input
            m_movement.HandleMovement();
            m_controlledArms.HandleArms();
            m_controlledKnees.HandleKnees();
            m_controlledLegs.HandleLegs();
            m_controlledLean.HandleLean();

            if ( m_movementKeys.m_left ) {
                m_controlledLean.LeanLeft();
                Direction = HandSelection.Left;
                m_weaponManager.SetDirection( Direction );
            }

            if ( m_movementKeys.m_right ) {
                m_controlledLean.LeanRight();
                Direction = HandSelection.Right;
                m_weaponManager.SetDirection( Direction );
            }
            
            m_direction = Direction;

            m_controlledArms.Attack( m_movementKeys.m_attack ? Direction : HandSelection.None );
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
        public void TakeDamage(int damage, Vector2 hitPoint = default) {
            if ( m_health == null ) return;
            
            if (hitPoint != default && m_health.CanTakeDamage){
                m_damageUIManager?.SpawnDamage( damage, hitPoint );
            }
            m_health.TakeDamage( damage );
        }
    }

    public enum HandSelection { None = -1, Left = 0, Right = 1 }

    [Serializable] public class WeaponManager {
        [SerializeField] Weapon[] m_leftWeapons;
        [SerializeField] Weapon[] m_rightWeapons;

        [Min( 0 )] public int m_currentWeaponIndex;
        public HandSelection m_direction = HandSelection.None; // 0=Left, 1=Right,
        public float AttackRange => GetCurrentWeapon()?.m_attackRange ?? 1f;
        public float AttackRate  => GetCurrentWeapon()?.m_attackRate  ?? 0.6f;

        public void Init(string guid) {
            if ( (m_leftWeapons == null || m_leftWeapons.Length == 0) &&
                 (m_rightWeapons == null || m_rightWeapons.Length == 0) ) {
                Logger.LogWarning( "WeaponManager: No weapons assigned in the inspector." );
                return;
            }

            AssignGuid( m_leftWeapons, guid );
            AssignGuid( m_rightWeapons, guid );

            UpdateActiveWeapons();
        }

        public void EnableWeapon(int index) {
            m_currentWeaponIndex = index;
            UpdateActiveWeapons();
        }

        public void SetDirection(HandSelection direction) {
            m_direction = direction;
            UpdateActiveWeapons();
        }

        void UpdateActiveWeapons() {
            switch (m_direction) {
                case HandSelection.Left:
                    SetOnlyIndexActive( m_rightWeapons, m_currentWeaponIndex );
                    SetAllInactive( m_leftWeapons );
                    break;

                case HandSelection.Right:
                    SetOnlyIndexActive( m_leftWeapons, m_currentWeaponIndex );
                    SetAllInactive( m_rightWeapons );
                    break;
                case HandSelection.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        static void AssignGuid(Weapon[] arr, string guid) {
            if ( arr == null ) return;
            foreach (var t in arr) {
                if ( t != null ) t.GUID = guid;
            }
        }

        static void SetOnlyIndexActive(Weapon[] arr, int index) {
            if ( arr == null || arr.Length == 0 )
                return;

            if ( index < 0 || index >= arr.Length ) {
                SetAllInactive( arr );
                return;
            }

            for (var i = 0; i < arr.Length; i++) {
                if ( arr[i] != null )
                    arr[i].gameObject.SetActive( i == index );
            }
        }

        static void SetAllInactive(Weapon[] arr) {
            if ( arr == null ) return;
            foreach (var t in arr) {
                if ( t != null ) t.gameObject.SetActive( false );
            }
        }
        
        Weapon GetCurrentWeapon() {
            switch (m_direction) {
                case HandSelection.Left:
                    if ( m_leftWeapons != null && m_currentWeaponIndex < m_leftWeapons.Length ) {
                        return m_leftWeapons[m_currentWeaponIndex];
                    }
                    break;
                case HandSelection.Right:
                    if ( m_rightWeapons != null && m_currentWeaponIndex < m_rightWeapons.Length ) {
                        return m_rightWeapons[m_currentWeaponIndex];
                    }
                    break;
                case HandSelection.None:
                    return null;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }
    }
    
    [Serializable] public class ControlledArms {
        [SerializeField] HingeJoint2D m_leftArm;
        [SerializeField] HingeJoint2D m_rightArm;

        public float m_speed = 1000f;
        public float m_torque = 10000f;

        public void HandleArms() {
            // NO-OP for now
        }
        
        public void Attack(HandSelection handSelection) {
            if ( m_leftArm == null || m_rightArm == null ) return;
            
            // TODO: find out why the arms are reversed here, im might be stoned or something. right now its correct lol.
            switch (handSelection) {
                case HandSelection.Right:
                    if ( m_leftArm != null ) {
                        var motor = m_leftArm.motor;
                        motor.motorSpeed = m_speed; // positive for left arm
                        motor.maxMotorTorque = m_torque;
                        m_leftArm.motor = motor;
                        m_leftArm.useLimits = false;
                        m_leftArm.useMotor = true;
                    }
                    break;

                case HandSelection.Left:
                    if ( m_rightArm != null ) {
                        var motor = m_rightArm.motor;
                        motor.motorSpeed = -m_speed; // negative for right arm
                        motor.maxMotorTorque = m_torque;
                        m_rightArm.motor = motor;
                        m_rightArm.useLimits = false;
                        m_rightArm.useMotor = true;
                    }
                    break;
                case HandSelection.None:
                    m_leftArm.useLimits = true;
                    m_leftArm.useMotor = false;
                    
                    m_rightArm.useLimits = true;
                    m_rightArm.useMotor = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        public bool IsAttacking() {
            return m_leftArm.useMotor || m_rightArm.useMotor;
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
                m_lowerTorso.m_targetRotation = m_lowerTargetRotation;
                m_lowerTorso.m_force = m_lowerForce;
            }

            if ( m_upperTorso ) {
                m_upperTorso.m_targetRotation = m_upperTargetRotation;
                m_upperTorso.m_force = m_upperForce;
            }
        }

        public void HandleLean() {
            if ( m_lowerTorso ) {
                m_lowerTorso.m_targetRotation = Mathf.Lerp( m_lowerTorso.m_targetRotation, m_lowerTargetRotation, Time.deltaTime * m_lerpSpeed );
                m_lowerTorso.m_force = Mathf.Lerp( m_lowerTorso.m_force, m_lowerForce, Time.deltaTime * m_lerpSpeed );
            }

            if ( m_upperTorso ) {
                m_upperTorso.m_targetRotation = Mathf.Lerp( m_upperTorso.m_targetRotation, m_upperTargetRotation, Time.deltaTime * m_lerpSpeed );
                m_upperTorso.m_force = Mathf.Lerp( m_upperTorso.m_force, m_upperForce, Time.deltaTime * m_lerpSpeed );
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
                Logger.LogError( "One or more required HingeJoint2D references are not set in the inspector." );
                return;
            }

            if ( !m_leftLegRb || !m_rightLegRb ) {
                Logger.LogError( "One or more required Rigidbody2D references are not set in the inspector." );
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
                Logger.LogError( "One or more required HingeJoint2D references are not set in the inspector." );
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
        [SerializeField] KeyCode m_attackKey = KeyCode.E;

        public bool m_jump;
        public bool m_left;
        public bool m_right;
        public bool m_attack;

        public bool m_isPlayerControlled = true;
        
        public void HandleInput() {
            if ( !m_isPlayerControlled ) return;
            m_jump = Input.GetKey( m_jumpKey );
            m_left = Input.GetKey( m_leftKey );
            m_right = Input.GetKey( m_rightKey );
            m_attack = Input.GetKey( m_attackKey );
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
                Logger.LogError( "Rigidbody2D reference is not set in the inspector." );
                return;
            }

            m_playerPos = m_rb.transform;
            m_isGrounded = false;
            m_canJump = true;
        }

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