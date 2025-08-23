using Random = UnityEngine.Random;

namespace TCS.MLAgents._Damon.TCS.MLAgents.Runtime.Unity {
    /// <summary>
    ///     Simple prey behaviour that moves randomly within bounds and respawns when caught.
    /// </summary>
    [RequireComponent( typeof(Rigidbody) )]
    public class Prey : MonoBehaviour {
        public float m_moveSpeed = 3f;
        Vector3 m_movementDirection;
        Rigidbody m_rb;

        void Start() {
            m_rb = GetComponent<Rigidbody>();
            m_rb.useGravity = false;
            m_rb.constraints = RigidbodyConstraints.FreezeRotation;
            SetRandomDirection();
        }

        void FixedUpdate() {
            // Move the prey continuously in its current direction
            m_rb.MovePosition( transform.localPosition + m_movementDirection * (m_moveSpeed * Time.fixedDeltaTime) );
            // Bounce off the environment bounds (assumes a 2D play area of ±4 units)
            if ( Mathf.Abs( transform.localPosition.x ) > 4f ) {
                m_movementDirection.x = -m_movementDirection.x;
            }

            if ( Mathf.Abs( transform.localPosition.z ) > 4f ) {
                m_movementDirection.z = -m_movementDirection.z;
            }
        }

        public void Respawn() {
            // Place prey at a random location and choose a new random direction
            transform.localPosition = new Vector3( Random.Range( -4f, 4f ), 0.5f, Random.Range( -4f, 4f ) );
            SetRandomDirection();
        }

        void SetRandomDirection() {
            m_movementDirection = new Vector3( Random.Range( -1f, 1f ), 0, Random.Range( -1f, 1f ) ).normalized;
        }
    }
}