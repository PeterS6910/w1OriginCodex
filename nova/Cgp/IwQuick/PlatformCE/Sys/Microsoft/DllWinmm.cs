using System;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    public class DllWinmm
    {
        [DllImport("winmm.dll")]
        public static extern int waveOutGetVolume(IntPtr hwo, out uint dwVolume);

        [DllImport("winmm.dll")]
        public static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
    }
}
