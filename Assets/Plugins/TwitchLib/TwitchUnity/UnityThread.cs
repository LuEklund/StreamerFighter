using System.Threading;
using UnityEngine;
namespace TwitchLib.Unity {
    public class UnityThread : MonoBehaviour {
        public static Thread Main { get; private set; }
        public static bool CurrentIsMainThread => Main == null || Main == Thread.CurrentThread;
        [RuntimeInitializeOnLoadMethod( RuntimeInitializeLoadType.BeforeSceneLoad )]
        static void SetMainUnityThread() => Main = Thread.CurrentThread;
    }
}