using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace TCS {
    public class RegisterClicksOffscreen : MonoBehaviour {
        // Import necessary Windows API functions
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetModuleHandle(string lpModuleName);

        delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        LowLevelMouseProc m_proc;
        IntPtr m_hookID = IntPtr.Zero;

        void Start() {
#if !UNITY_EDITOR
            m_proc = HookCallback;
            m_hookID = SetHook(m_proc);
#endif
        }

        void OnDestroy() {
#if !UNITY_EDITOR
            UnhookWindowsHookEx(m_hookID);
#endif
        }

        void Update() {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Mouse0)) {
                HandleOffscreenClick();
            }
#endif
        }

        IntPtr SetHook(LowLevelMouseProc proc) {
            using var curProcess = System.Diagnostics.Process.GetCurrentProcess();
            using var curModule = curProcess.MainModule;
            return SetWindowsHookEx(WH_MOUSE_LL, proc, GetModuleHandle(curModule?.ModuleName), 0);
        }

        const int WH_MOUSE_LL = 14;
        const int WM_LBUTTONDOWN = 0x0201;

        IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) {
            if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN) {
                HandleOffscreenClick();
            }

            return CallNextHookEx(m_hookID, nCode, wParam, lParam);
        }

        void HandleOffscreenClick() {
            Debug.Log("Offscreen click detected!");
            TransparentWindowEvents.OffscreenClick?.Invoke();
        }
    }
}