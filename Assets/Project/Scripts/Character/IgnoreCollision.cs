using System.Collections.Generic;
using UnityEngine;

namespace Character {
    // make sure we catch colliders before other scripts run
    [DefaultExecutionOrder( -999 )]
    public class IgnoreCollision : MonoBehaviour {
        // Cache of all colliders under this character (including inactive children)
        readonly List<Collider2D> _colliders = new( 32 );
        readonly List<Collider2D> _scratch = new( 32 ); // temp to filter nulls

        void Awake() {
            RebuildCache();
            ApplyAllIgnores();
        }

        void OnEnable() {
            // Safety: if this component itself was toggled, re-apply
            ApplyAllIgnores();
        }

        void OnTransformChildrenChanged() {
            // Children added/removed at runtime
            RebuildCache();
            ApplyAllIgnores();
        }

        /// <summary>
        /// Called by Collider2DEnableWatcher on a child collider's OnEnable.
        /// Re-applies ignores for just that collider.
        /// </summary>
        public void OnChildColliderEnabled(Collider2D col) {
            if ( !col ) return;

            // Clean nulls lazily
            Compact();

            // Ensure cache knows about it (in case it was added at runtime)
            if ( !_colliders.Contains( col ) ) {
                _colliders.Add( col );
                EnsureWatcher( col );
            }

            ApplyIgnoresFor( col );
        }

        void RebuildCache() {
            _colliders.Clear();
            _colliders.AddRange( GetComponentsInChildren<Collider2D>( true ) );

            // Add a watcher beside each collider so we know when it gets re-enabled
            for (int i = 0; i < _colliders.Count; i++)
                EnsureWatcher( _colliders[i] );

            Compact(); // drop any nulls just in case
        }

        void EnsureWatcher(Collider2D col) {
            if ( !col ) return;
            if ( !col.TryGetComponent<Collider2DEnableWatcher>( out _ ) )
                col.gameObject.AddComponent<Collider2DEnableWatcher>().Initialize( this, col );
        }

        void ApplyAllIgnores() {
            Compact();
            for (int i = 0; i < _colliders.Count; i++) {
                var a = _colliders[i];
                if ( !a || !a.enabled ) continue;

                for (int k = i + 1; k < _colliders.Count; k++) {
                    var b = _colliders[k];
                    if ( !b || !b.enabled ) continue;

                    Physics2D.IgnoreCollision( a, b, true );
                }
            }
        }

        void ApplyIgnoresFor(Collider2D target) {
            if ( !target || !target.enabled ) return;

            // Ignore target against every other enabled collider we track
            for (int i = 0; i < _colliders.Count; i++) {
                var other = _colliders[i];
                if ( !other || other == target || !other.enabled ) continue;

                Physics2D.IgnoreCollision( target, other, true );
            }
        }

        void Compact() {
            _scratch.Clear();
            for (int i = 0; i < _colliders.Count; i++) {
                var c = _colliders[i];
                if ( c ) _scratch.Add( c );
            }

            _colliders.Clear();
            _colliders.AddRange( _scratch );
        }
    }
}