/*
 * Class for DBS Creator
 * */

using System;
using System.Runtime.InteropServices;

namespace Contal.Cgp.DBSCreator
{
    public class Support
    {
        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "ShowWindowAsync")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        public const int WS_SHOWNORMAL = 1;
    }
}
