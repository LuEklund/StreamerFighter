using UnityEngine;
namespace Stickman {
    public class IgnoreCollision : MonoBehaviour
    {
        void Start()
        {
            var colliders = GetComponentsInChildren<Collider2D>();
            for (var i = 0; i < colliders.Length; i++)
            {
                for(int k = i + 1; k < colliders.Length; k++)
                {
                    Physics2D.IgnoreCollision(colliders[i], colliders[k]);
                }
            }
        }

        private void OnCollisionEnter2D(Collision2D coll)
        {
            return;
            if (coll.gameObject.tag == "Player")
            {
                Physics2D.IgnoreCollision(this.gameObject.GetComponent<Collider2D>(), coll.gameObject.GetComponent<Collider2D>());
            }
        }
    }
}