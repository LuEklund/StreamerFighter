using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
namespace OverlayCore.Window {
    public class TransparentWindowTwo : MonoBehaviour {
        // =============================
        // WINDOWS (user32 + dwmapi)
        // =============================
        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern int SetLayeredWindowAttributes(IntPtr hWnd, uint crKey, byte bAlpha, uint dwFlags);

        struct MARGINS
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("Dwmapi.dll")]
        static extern uint DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS margins);

        const int GWL_EXSTYLE = -20;
        const uint WS_EX_LAYERED = 0x00080000;
        const uint WS_EX_TRANSPARENT = 0x00000020;

        static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        const uint LWA_COLORKEY = 0x00000001;
        #endif

        // =============================
        // LINUX (X11 + XFixes)
        // =============================
        #if UNITY_STANDALONE_LINUX && !UNITY_EDITOR
        // libX11
        [DllImport("libX11")]
        static extern IntPtr XOpenDisplay(IntPtr display_name);

        [DllImport("libX11")]
        static extern int XCloseDisplay(IntPtr display);

        [DllImport("libX11")]
        static extern IntPtr XDefaultRootWindow(IntPtr display);

        [DllImport("libX11")]
        static extern IntPtr XInternAtom(IntPtr display, string atom_name, bool only_if_exists);

        [DllImport("libX11")]
        static extern int XGetWindowProperty(
            IntPtr display,
            IntPtr w,
            IntPtr property,
            IntPtr long_offset,
            IntPtr long_length,
            bool delete,
            IntPtr req_type,
            out IntPtr actual_type_return,
            out int actual_format_return,
            out IntPtr nitems_return,
            out IntPtr bytes_after_return,
            out IntPtr prop_return);

        [DllImport("libX11")]
        static extern int XFree(IntPtr data);

        [DllImport("libX11")]
        static extern int XMoveWindow(IntPtr display, IntPtr w, int x, int y);

        [DllImport("libX11")]
        static extern int XFlush(IntPtr display);

        // libXfixes (for input shape region – true click-through)
        [DllImport("libXfixes")]
        static extern IntPtr XFixesCreateRegion(IntPtr display, IntPtr rectangles, int nrectangles);

        [DllImport("libXfixes")]
        static extern void XFixesDestroyRegion(IntPtr display, IntPtr region);

        [DllImport("libXfixes")]
        static extern void XFixesSetWindowShapeRegion(IntPtr display, IntPtr w, int shape_kind, int x, int y, IntPtr region);

        // libc – to get our PID
        [DllImport("libc")]
        static extern int getpid();

        const int ShapeInput = 2; // X11 shape kind for input region
        static readonly IntPtr XA_WINDOW = (IntPtr)33; // well-known atom id for WINDOW type
        #endif

        // =============================
        // COMMON
        // =============================
        bool m_clickthrough;
        void ForceNotClickthrough() => m_clickthrough = false;
        void ForceClickthrough() => m_clickthrough = true;

        IntPtr m_hWndOrXWindow; // Windows: HWND; Linux: X11 Window (XID)
        Camera m_camera;

        #if UNITY_STANDALONE_LINUX && !UNITY_EDITOR
        IntPtr xDisplay = IntPtr.Zero;
        #endif

        void Awake() {
            m_camera = Camera.main;

            TransparentWindowEvents.OnForceClickThrough += ForceClickthrough;
            TransparentWindowEvents.OnForceNotClickThrough += ForceNotClickthrough;
        }

        void Start() {
            #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            hWndOrXWindow = GetActiveWindow();

            // Extend the frame so per-pixel alpha from the backbuffer is honored.
            var margins = new MARGINS { cxLeftWidth = -1 };
            DwmExtendFrameIntoClientArea(hWndOrXWindow, ref margins);

            // Layered + Transparent (click-through) initially; we'll toggle each frame below.
            SetWindowLong(hWndOrXWindow, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);

            SetWindowToCorrectScreen_Windows();
            #endif

            #if UNITY_STANDALONE_LINUX && !UNITY_EDITOR
            // Try to open an X11 display (works on Xorg and XWayland).
            xDisplay = XOpenDisplay(IntPtr.Zero);
            if (xDisplay == IntPtr.Zero)
            {
                Debug.LogWarning("[TransparentWindow] XOpenDisplay failed. Click-through disabled (likely pure Wayland).");
            }
            else
            {
                hWndOrXWindow = FindUnityTopLevelWindowByPid(xDisplay);
                if (hWndOrXWindow == IntPtr.Zero)
                {
                    Debug.LogWarning("[TransparentWindow] Could not locate Unity window via _NET_CLIENT_LIST/_NET_WM_PID. Click-through disabled.");
                }
                else
                {
                    // Start in click-through; we'll toggle based on pointer hits below.
                    SetClickthrough_Linux(true);
                    SetWindowToCorrectScreen_Linux();
                }
            }
            #endif

            Application.runInBackground = true;
        }

        void Update() {
            // Click-through when the pointer is NOT over UI/3D; capture input when it IS.
            SetClickthrough( !IsPointerOverUIOr3DObject() );
        }

        void OnDestroy() {
            TransparentWindowEvents.OnForceClickThrough -= ForceClickthrough;
            TransparentWindowEvents.OnForceNotClickThrough -= ForceNotClickthrough;

            #if UNITY_STANDALONE_LINUX && !UNITY_EDITOR
            if (xDisplay != IntPtr.Zero)
            {
                XCloseDisplay(xDisplay);
                xDisplay = IntPtr.Zero;
            }
            #endif
        }

        public bool IsPointerOverUIOr3DObject() {
            if ( !m_clickthrough )
                return false;

            // UI first
            if ( EventSystem.current != null ) {
                if ( EventSystem.current.IsPointerOverGameObject() )
                    return true;

                var pe = new PointerEventData( EventSystem.current ) { position = Input.mousePosition };
                List<RaycastResult> hits = new();
                EventSystem.current.RaycastAll( pe, hits );
                if ( hits.Count > 0 )
                    return true;
            }

            // Then 3D
            if ( m_camera != null ) {
                var ray = m_camera.ScreenPointToRay( Input.mousePosition );
                return Physics.Raycast( ray, out _ );
            }

            return false;
        }

        void SetClickthrough(bool clickthrough) {
            #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (hWndOrXWindow != IntPtr.Zero)
            {
                if (clickthrough)
                    SetWindowLong(hWndOrXWindow, GWL_EXSTYLE, WS_EX_LAYERED | WS_EX_TRANSPARENT);
                else
                    SetWindowLong(hWndOrXWindow, GWL_EXSTYLE, WS_EX_LAYERED);
            }
            #endif

            #if UNITY_STANDALONE_LINUX && !UNITY_EDITOR
            SetClickthrough_Linux(clickthrough);
            #endif
        }

        // =============================
        // WINDOWS helpers
        // =============================
        #if UNITY_STANDALONE_WIN && !UNITY_EDITOR
        void SetWindowToCorrectScreen_Windows()
        {
            // Determine which Unity display the mouse is currently over.
            Vector2 mouse = Input.mousePosition;

            int targetDisplay = 0;
            int cumulativeX = 0;
            for (int i = 0; i < Display.displays.Length; i++)
            {
                int w = Display.displays[i].systemWidth;
                if (mouse.x >= cumulativeX && mouse.x < cumulativeX + w)
                {
                    targetDisplay = i;
                    break;
                }
                cumulativeX += w;
            }

            // Position on that display; keep it topmost.
            int posX = 0;
            for (int i = 0; i < targetDisplay; i++) posX += Display.displays[i].systemWidth;
            int posY = 0;

            SetWindowPos(hWndOrXWindow, HWND_TOPMOST, posX, posY, 0, 0, 0);
        }
        #endif

        // =============================
        // LINUX helpers (X11)
        // =============================
        #if UNITY_STANDALONE_LINUX && !UNITY_EDITOR
        void SetClickthrough_Linux(bool clickthrough)
        {
            if (xDisplay == IntPtr.Zero || hWndOrXWindow == IntPtr.Zero)
                return;

            if (clickthrough)
            {
                // Empty input region = let clicks pass through to whatever is underneath.
                IntPtr empty = XFixesCreateRegion(xDisplay, IntPtr.Zero, 0);
                XFixesSetWindowShapeRegion(xDisplay, hWndOrXWindow, ShapeInput, 0, 0, empty);
                XFixesDestroyRegion(xDisplay, empty);
            }
            else
            {
                // Null = restore default input region (window receives events again).
                XFixesSetWindowShapeRegion(xDisplay, hWndOrXWindow, ShapeInput, 0, 0, IntPtr.Zero);
            }
            XFlush(xDisplay);
        }

        void SetWindowToCorrectScreen_Linux()
        {
            if (xDisplay == IntPtr.Zero || hWndOrXWindow == IntPtr.Zero)
                return;

            Vector2 mouse = Input.mousePosition;

            int targetDisplay = 0;
            int cumulativeX = 0;
            for (int i = 0; i < Display.displays.Length; i++)
            {
                int w = Display.displays[i].systemWidth;
                if (mouse.x >= cumulativeX && mouse.x < cumulativeX + w)
                {
                    targetDisplay = i;
                    break;
                }
                cumulativeX += w;
            }

            int posX = 0;
            for (int i = 0; i < targetDisplay; i++) posX += Display.displays[i].systemWidth;
            int posY = 0;

            XMoveWindow(xDisplay, hWndOrXWindow, posX, posY);
            XFlush(xDisplay);
        }

        IntPtr FindUnityTopLevelWindowByPid(IntPtr display)
        {
            // Root window
            IntPtr root = XDefaultRootWindow(display);
            if (root == IntPtr.Zero) return IntPtr.Zero;

            // Get the list of client windows from the WM (EWMH)
            IntPtr atomClientList = XInternAtom(display, "_NET_CLIENT_LIST", false);
            if (atomClientList == IntPtr.Zero) return IntPtr.Zero;

            IntPtr actualType, nitems, bytesAfter, prop;
            int actualFormat;
            int status = XGetWindowProperty(
                display, root, atomClientList,
                IntPtr.Zero, (IntPtr)2048, false, XA_WINDOW,
                out actualType, out actualFormat, out nitems, out bytesAfter, out prop);

            if (status != 0 || prop == IntPtr.Zero) return IntPtr.Zero;

            try
            {
                int count = nitems.ToInt32();
                IntPtr[] wins = new IntPtr[count];
                Marshal.Copy(prop, wins, 0, count);

                IntPtr atomPid = XInternAtom(display, "_NET_WM_PID", false);
                if (atomPid == IntPtr.Zero) return IntPtr.Zero;

                int myPid = getpid();

                for (int i = 0; i < wins.Length; i++)
                {
                    if (TryGetWindowPid(display, wins[i], atomPid, out int winPid) && winPid == myPid)
                    {
                        return wins[i];
                    }
                }
            }
            finally
            {
                XFree(prop);
            }

            return IntPtr.Zero;
        }

        bool TryGetWindowPid(IntPtr display, IntPtr window, IntPtr atomPid, out int pid)
        {
            pid = -1;
            IntPtr actualType, nitems, bytesAfter, prop;
            int actualFormat;
            int status = XGetWindowProperty(
                display, window, atomPid,
                IntPtr.Zero, (IntPtr)1, false, (IntPtr)0, // Any type
                out actualType, out actualFormat, out nitems, out bytesAfter, out prop);

            if (status != 0 || prop == IntPtr.Zero || nitems.ToInt32() == 0)
                return false;

            try
            {
                // _NET_WM_PID is a 32-bit CARDINAL
                pid = Marshal.ReadInt32(prop);
                return pid > 0;
            }
            finally
            {
                XFree(prop);
            }
        }
        #endif
    }
}