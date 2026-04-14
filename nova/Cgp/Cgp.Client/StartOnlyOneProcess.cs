using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace Contal.Cgp.Client
{
    public static class StartOnlyOneProcess
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

        public static bool ForegroundStartedProcess(string globalMutexName)
        {
            Process current = Process.GetCurrentProcess();
            
            Process[] proceses = Process.GetProcessesByName(current.ProcessName);
            if (proceses != null && proceses.Length > 0)
            {
                foreach (Process process in proceses)
                {
                    if (process.Id != current.Id &&
                        process.SessionId == current.SessionId)
                    {
                        if (IsIconic(process.MainWindowHandle))
                        {
                            ShowWindowAsync(process.MainWindowHandle, SW_RESTORE);
                        }

                        SetForegroundWindow(process.MainWindowHandle);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
