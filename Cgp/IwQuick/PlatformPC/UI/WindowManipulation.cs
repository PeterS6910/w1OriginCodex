using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.UI
{
    public static class WindowManipulation
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        private const int SW_HIDE = 0;
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;
        private const int SW_SHOWNOACTIVATE = 4;
        private const int SW_RESTORE = 9;
        private const int SW_SHOWDEFAULT = 10;

        public static void BringProcessWindowToFront(Process process)
        {
            if (IsIconic(process.MainWindowHandle))
                ShowWindowAsync(process.MainWindowHandle, SW_RESTORE);

            SetForegroundWindow(process.MainWindowHandle);
        }
    }
}
