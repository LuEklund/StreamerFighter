using System.Threading;
using UnityEngine;
namespace Twitch {
    public class UnityThread : MonoBehaviour {
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void SetMainUnityThread() => s_mainThread = Thread.CurrentThread;

        static Thread s_mainThread;

        public static Thread Main => s_mainThread;
        public static bool CurrentIsMainThread => s_mainThread == null || s_mainThread == Thread.CurrentThread;
    }
}