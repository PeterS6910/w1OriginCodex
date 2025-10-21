using System;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    public class DllWinmm
    {
        public const string DLL_PATH = "winmm.dll";

        [DllImport(DLL_PATH)]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport(DLL_PATH)]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
    }
}
