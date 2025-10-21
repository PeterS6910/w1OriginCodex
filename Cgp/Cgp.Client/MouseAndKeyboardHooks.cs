using System;
using System.Runtime.InteropServices;

namespace Contal.Cgp.Client
{
    public class MouseActivityHook : IDisposable
    {
        private static int hHook;

        private const int WH_MOUSE = 7;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;

        private readonly bool isX86 = IntPtr.Size == 4;
        private delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        private HookProc MouseHookProcedure;

        public event Action<int, int> MouseDown;
        public event Action<int, int> MouseUp;
        public event Action<int> MouseWheel;
        public event Action<int, int> MouseMove;

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Explicit)]
        public struct MouseHookStruct
        {
            [FieldOffset(0x00)]
            public POINT position;

            [FieldOffset(0x16)]
            public Int16 MouseDataX86;

            [FieldOffset(0x22)]
            public Int16 MouseDataX64;
        }

        //This is the Import for the SetWindowsHookEx function.
        //Use this function to install a thread-specific Hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        private static extern int SetWindowsHookEx(int idHook, HookProc lpfn,
        IntPtr hInstance, int threadId);

        //This is the Import for the UnhookWindowsHookEx function.
        //Call this function to uninstall the Hook.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        private static extern bool UnhookWindowsHookEx(int idHook);

        //This is the Import for the CallNextHookEx function.
        //Use this function to pass the Hook information to the next Hook procedure in chain.
        [DllImport("user32.dll", CharSet = CharSet.Auto,
         CallingConvention = CallingConvention.StdCall)]
        private static extern int CallNextHookEx(int idHook, int nCode,
        IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32")]
        internal static extern int GetCurrentThreadId();

        public void Hook()
        {
            if (hHook == 0)
            {
                MouseHookProcedure = MouseHookProc;

                hHook = SetWindowsHookEx(WH_MOUSE,
                    MouseHookProcedure,
                    IntPtr.Zero,
                    GetCurrentThreadId());
            }
        }

        public void UnHook()
        {
            if (hHook != 0)
            {
                UnhookWindowsHookEx(hHook);
                hHook = 0;
            }
        }

        private int MouseHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var mouseHookStruct = (MouseHookStruct) Marshal.PtrToStructure(lParam, typeof (MouseHookStruct));

                switch (wParam.ToInt32())
                {
                    case WM_MOUSEWHEEL:

                        if (MouseWheel != null)
                        {
                            MouseWheel(isX86
                                ? mouseHookStruct.MouseDataX86
                                : mouseHookStruct.MouseDataX64);
                        }

                        break;

                    case WM_MOUSEMOVE:

                        if (MouseMove != null)
                            MouseMove(mouseHookStruct.position.X, mouseHookStruct.position.Y);

                        break;

                    case WM_LBUTTONDOWN:

                        if (MouseDown != null)
                            MouseDown(mouseHookStruct.position.X, mouseHookStruct.position.Y);

                        break;

                    case WM_LBUTTONUP:

                        if (MouseUp != null)
                            MouseUp(mouseHookStruct.position.X, mouseHookStruct.position.Y);

                        break;
                }
            }

            return CallNextHookEx(hHook, nCode, wParam, lParam);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            UnHook();

            if (disposing)
                GC.SuppressFinalize(this);
        }

        ~MouseActivityHook()
		{
			Dispose(false);
		}
    }
}
