using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using UnityEngine;
namespace StreamAPI {
    public class ThreadDispatcher : MonoBehaviour {
        static ThreadDispatcher s_instance;

        static readonly ConcurrentQueue<Action> ExecutionQueue = new();

        void Awake() {
            if ( s_instance == null ) {
                s_instance = this;

                // If we're a child, unparent before making us persistent
                if ( transform.parent != null ) {
                    transform.SetParent( null, true );
                }

                DontDestroyOnLoad( gameObject );
            }
            else if ( s_instance != this ) {
                Destroy( gameObject );
            }
        }


        /// <summary>
        /// Ensures a thread dispatcher is created if there is none.
        /// </summary>
        public static void EnsureCreated([CallerMemberName] string callerMemberName = null) {
            if ( !Application.isPlaying ) {
                return;
            }


            if ( s_instance == null || s_instance.gameObject == null ) {
                s_instance = CreateThreadDispatcherSingleton( callerMemberName );
            }
        }

        /// <summary>
        /// Locks the queue and adds the Action to the queue
        /// </summary>
        /// <param name="action">function that will be executed from the main thread.</param>
        public static void Enqueue(Action action) {
            ExecutionQueue.Enqueue( action );
        }

        void Update() {
            while (!ExecutionQueue.IsEmpty) {
                if ( ExecutionQueue.TryDequeue( out var action ) ) {
                    action.Invoke();
                }
            }
        }

        static ThreadDispatcher CreateThreadDispatcherSingleton(string callerMemberName) {
            if ( !UnityThread.CurrentIsMainThread ) {
                throw new InvalidOperationException( $"The {callerMemberName} can only be created from the main thread. Did you accidentally delete the " + nameof(ThreadDispatcher) + " in your scene?" );
            }

            var threadDispatcher = new GameObject( nameof(ThreadDispatcher) ).AddComponent<ThreadDispatcher>();
            DontDestroyOnLoad( threadDispatcher );
            return threadDispatcher;
        }
    }
}