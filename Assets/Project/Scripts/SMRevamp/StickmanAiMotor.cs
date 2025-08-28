using UnityEngine;

namespace SMRevamp
{
    public class StickmanAiMotor : MonoBehaviour {
        [SerializeField] StickmanMotor stickmanMotor;
        public GameObject target;

        void Awake() {
            if ( stickmanMotor == null ) {
                stickmanMotor = GetComponent<StickmanMotor>();
            }
            
            stickmanMotor.m_movementKeys.m_isPlayerControlled = false;
        }

        void Update()
        {
            if (target == null) return;
            if(target.transform.position.x < stickmanMotor.toroso.transform.position.x)
                stickmanMotor.m_movementKeys.m_left = UnityEngine.Random.value > 0.5f;
            else if (target.transform.position.x > stickmanMotor.toroso.transform.position.x)
                stickmanMotor.m_movementKeys.m_right = UnityEngine.Random.value > 0.5f;
        }
    }
}