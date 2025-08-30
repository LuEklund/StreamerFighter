using UnityEngine;
namespace Character {
    /// <summary>
    /// Lives next to a Collider2D and pings the owning IgnoreCollision when enabled.
    /// Keeps ignore pairs valid after colliders are toggled.
    /// </summary>
    [DisallowMultipleComponent]
    sealed class Collider2DEnableWatcher : MonoBehaviour {
        IgnoreCollision _owner;
        Collider2D _col;

        public void Initialize(IgnoreCollision owner, Collider2D col) {
            _owner = owner;
            _col = col;
        }

        void Awake() {
            // In case added via AddComponent at edit/runtime without Initialize
            if ( !_owner ) _owner = GetComponentInParent<IgnoreCollision>( true );
            if ( !_col ) _col = GetComponent<Collider2D>();
        }

        void OnEnable() {
            if ( _owner && _col ) _owner.OnChildColliderEnabled( _col );
        }
    }
}