using UnityEngine;

namespace Stickman {
    public class ColliderTriggerEvent : MonoBehaviour
    {
        public string targetTag;
        public LayerMask layerMask;
    
        bool useLayerMask;

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag(targetTag)) {
                Health health = other.gameObject.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(1);
                }
            }
        }

        public void OnCollisionEnter2D(Collision2D other)
        {
            // Debug.Log(other.gameObject.tag);
            // if ( useLayerMask ) {
            //     if (((1 << other.gameObject.layer) & layerMask) != 0)
            //     {
            //         Health health = other.gameObject.GetComponent<Health>();
            //         if (health != null)
            //         {
            //             health.TakeDamage(1);
            //         }
            //     }
            // }else if (other.gameObject.CompareTag(targetTag)) {
            //     Health health = other.gameObject.GetComponent<Health>();
            //     if (health != null)
            //     {
            //         health.TakeDamage(1);
            //     }
            // }
        }

        public void OnTriggerExit2D(Collider2D other)
        {
            // Debug.Log(other.gameObject.tag);
            // if ( useLayerMask ) {
            //     if (((1 << other.gameObject.layer) & layerMask) != 0)
            //     {
            //         Health health = other.gameObject.GetComponent<Health>();
            //         if (health != null)
            //         {
            //             health.TakeDamage(1);
            //         }
            //     }
            // }else if (other.gameObject.CompareTag(targetTag)) {
            //     Health health = other.gameObject.GetComponent<Health>();
            //     if (health != null)
            //     {
            //         health.TakeDamage(1);
            //     }
            // }
        }

        public void OnTriggerStay2D(Collider2D other)
        {
            // Debug.Log(other.gameObject.tag);
            //     if ( useLayerMask ) {
            //         if (((1 << other.gameObject.layer) & layerMask) != 0)
            //         {
            //             Health health = other.gameObject.GetComponent<Health>();
            //             if (health != null)
            //             {
            //                 health.TakeDamage(1);
            //             }
            //         }
            //     }else if (other.gameObject.CompareTag(targetTag)) {
            //         Health health = other.gameObject.GetComponent<Health>();
            //         if (health != null)
            //         {
            //             health.TakeDamage(1);
            //         }
            //     }
        }
    }
}