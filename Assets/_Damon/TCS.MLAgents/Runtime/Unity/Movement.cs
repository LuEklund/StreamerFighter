using UnityEngine;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    public class Movement : MonoBehaviour {
        [SerializeField] float speed = 5f;
        [SerializeField] bool useGravity = false;
        [SerializeField] RigidbodyConstraints constraints = RigidbodyConstraints.FreezeRotation;
        
        Rigidbody rigidBody;
        
        public float Speed {
            get => speed;
            set => speed = value;
        }
        
        public Vector3 Velocity => rigidBody.linearVelocity;
        public Vector3 Position => transform.localPosition;
        
        void Awake() {
            rigidBody = GetComponent<Rigidbody>();
            if (rigidBody == null) {
                rigidBody = gameObject.AddComponent<Rigidbody>();
            }
            
            rigidBody.useGravity = useGravity;
            rigidBody.constraints = constraints;
        }
        
        public void ApplyForce(Vector3 force) {
            rigidBody.AddForce(force * speed);
        }
        
        public void SetVelocity(Vector3 velocity) {
            rigidBody.linearVelocity = velocity;
        }
        
        public void SetPosition(Vector3 position) {
            transform.localPosition = position;
        }
        
        public void MoveToPosition(Vector3 position) {
            rigidBody.MovePosition(position);
        }
        
        public void StopMovement() {
            rigidBody.linearVelocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
        }
    }
}