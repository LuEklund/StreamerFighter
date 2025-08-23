using UnityEngine;
using Random = UnityEngine.Random;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    public class PreyController : MonoBehaviour {
        [SerializeField] SimulationConfig config;
        
        Movement movement;
        BoundarySystem boundarySystem;
        
        Vector3 movementDirection;
        float lastDirectionChangeTime;
        
        void Awake() {
            movement = GetComponent<Movement>();
            boundarySystem = GetComponent<BoundarySystem>();
            
            if (movement == null) movement = gameObject.AddComponent<Movement>();
            if (boundarySystem == null) boundarySystem = gameObject.AddComponent<BoundarySystem>();
        }
        
        void Start() {
            SetupFromConfig();
            SetRandomDirection();
            lastDirectionChangeTime = Time.time;
        }
        
        void SetupFromConfig() {
            if (config == null) return;
            
            movement.Speed = config.preySpeed;
            
            if (boundarySystem != null) {
                var bounds = boundarySystem;
                bounds.enabled = config.preyBouncesOffWalls;
            }
        }
        
        void FixedUpdate() {
            HandleMovement();
            HandleBoundaries();
            HandleDirectionChange();
        }
        
        void HandleMovement() {
            Vector3 targetPosition = transform.localPosition + movementDirection * (movement.Speed * Time.fixedDeltaTime);
            movement.MoveToPosition(targetPosition);
        }
        
        void HandleBoundaries() {
            if (config == null) return;
            
            Vector3 pos = transform.localPosition;
            bool hitBoundary = false;
            
            if (pos.x <= config.arenaMinBounds.x || pos.x >= config.arenaMaxBounds.x) {
                movementDirection.x = -movementDirection.x;
                hitBoundary = true;
            }
            
            if (pos.z <= config.arenaMinBounds.z || pos.z >= config.arenaMaxBounds.z) {
                movementDirection.z = -movementDirection.z;
                hitBoundary = true;
            }
            
            if (hitBoundary && config.preyBouncesOffWalls) {
                movementDirection = movementDirection.normalized;
            }
        }
        
        void HandleDirectionChange() {
            if (config == null) return;
            
            if (Time.time - lastDirectionChangeTime >= config.preyDirectionChangeInterval) {
                SetRandomDirection();
                lastDirectionChangeTime = Time.time;
            }
        }
        
        public void Respawn() {
            if (config != null) {
                movement.SetPosition(config.GetRandomArenaPosition());
            }
            
            movement.StopMovement();
            SetRandomDirection();
            lastDirectionChangeTime = Time.time;
        }
        
        void SetRandomDirection() {
            movementDirection = new Vector3(
                Random.Range(-1f, 1f),
                0,
                Random.Range(-1f, 1f)
            ).normalized;
        }
        
        public void SetDirection(Vector3 direction) {
            movementDirection = direction.normalized;
        }
        
        public Vector3 GetDirection() {
            return movementDirection;
        }
    }
}