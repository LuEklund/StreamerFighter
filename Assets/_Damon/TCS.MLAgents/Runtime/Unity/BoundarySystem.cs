using UnityEngine;
using Random = UnityEngine.Random;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    public class BoundarySystem : MonoBehaviour {
        [SerializeField] Vector3 minBounds = new Vector3(-4f, 0f, -4f);
        [SerializeField] Vector3 maxBounds = new Vector3(4f, 1f, 4f);
        [SerializeField] bool bounceOnHit = false;
        [SerializeField] bool teleportOnExit = false;
        
        public Vector3 MinBounds => minBounds;
        public Vector3 MaxBounds => maxBounds;
        
        public bool IsOutOfBounds(Vector3 position) {
            return position.x < minBounds.x || position.x > maxBounds.x ||
                   position.z < minBounds.z || position.z > maxBounds.z;
        }
        
        public Vector3 ClampToBounds(Vector3 position) {
            return new Vector3(
                Mathf.Clamp(position.x, minBounds.x, maxBounds.x),
                Mathf.Clamp(position.y, minBounds.y, maxBounds.y),
                Mathf.Clamp(position.z, minBounds.z, maxBounds.z)
            );
        }
        
        public Vector3 GetRandomPositionInBounds() {
            return new Vector3(
                Random.Range(minBounds.x, maxBounds.x),
                Random.Range(minBounds.y, maxBounds.y),
                Random.Range(minBounds.z, maxBounds.z)
            );
        }
        
        public Vector3 HandleBoundaryCollision(Vector3 position, Vector3 velocity) {
            Vector3 newVelocity = velocity;
            
            if (bounceOnHit) {
                if (position.x <= minBounds.x || position.x >= maxBounds.x) {
                    newVelocity.x = -newVelocity.x;
                }
                if (position.z <= minBounds.z || position.z >= maxBounds.z) {
                    newVelocity.z = -newVelocity.z;
                }
            }
            
            return newVelocity;
        }
        
        public void EnforceBoundaries(Movement movement) {
            Vector3 currentPos = movement.Position;
            
            if (IsOutOfBounds(currentPos)) {
                if (teleportOnExit) {
                    movement.SetPosition(GetRandomPositionInBounds());
                } else {
                    movement.SetPosition(ClampToBounds(currentPos));
                }
                
                if (bounceOnHit) {
                    Vector3 newVelocity = HandleBoundaryCollision(currentPos, movement.Velocity);
                    movement.SetVelocity(newVelocity);
                }
            }
        }
    }
}