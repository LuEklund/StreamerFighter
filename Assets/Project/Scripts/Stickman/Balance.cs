using UnityEngine;
using UnityEngine.Serialization;
namespace Stickman {
    public class Balance : MonoBehaviour
    {
        [FormerlySerializedAs( "targetRotation" )] public float m_targetRotation;
        Rigidbody2D m_rb;
        [FormerlySerializedAs( "force" )] public float m_force;

        private void Start()
        {
            m_rb = gameObject.GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            m_rb.MoveRotation(Mathf.LerpAngle(m_rb.rotation, m_targetRotation, m_force * 10000 * Time.deltaTime));
        }

    }
}
