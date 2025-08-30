using UnityEngine;

namespace StreamerFighter {
    public class CameraController : MonoBehaviour
    {
        public Camera cam;
        public float cameraSize = 0.5f;
        public float smoothSpeed = 0.125f;


        void Awake() {
            if ( cam == null ) {
                cam = Camera.main;
            }
            if ( cam == null ) {
                Debug.LogError( "CameraController: No camera found!" );
                return;
            }
        }

        void Update() {
        
        }
    }
}

