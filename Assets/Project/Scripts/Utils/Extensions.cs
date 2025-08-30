using TMPro;
using UnityEngine;
namespace Utils {
    public static class Extensions {
        public static Rigidbody2D ZeroX(this Rigidbody2D rb) {
            var v = rb.linearVelocity;
            v.x = 0f;
            rb.linearVelocity = v;
            
            return rb;
        }
        
        public static HingeJoint2D SetKneeLimits(this HingeJoint2D rightKnee, float p1, float p2, bool flip = false) {
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
            
            return rightKnee;
        }
        
        public static TextMeshProUGUI SetText(this TextMeshProUGUI textMesh, string text) {
            textMesh.text = text;
            return textMesh;
        }
    }
}