using System;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    public class DllUser32
    {
        public const int CDS_TEST = 0x02;
        public const int CDS_UPDATEREGISTRY = 0x01;
        public const int DISP_CHANGE_FAILED = -1;
        public const int DISP_CHANGE_SUCCESSFUL = 0;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;

            public short dmOrientation;
            public short dmPaperSize;
            public short dmPaperLength;
            public short dmPaperWidth;

            public short dmScale;
            public short dmCopies;
            public short dmDefaultSource;
            public short dmPrintQuality;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            public short dmLogPixels;
            public short dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;

            public int dmDisplayFlags;
            public int dmDisplayFrequency;

            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;

            public int dmPanningWidth;
            public int dmPanningHeight;
        };

        [DllImport("user32.dll")]//, CharSet=CharSet.Auto, SetLastError=true)]
        public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int ChangeDisplaySettingsEx(
            string lpszDeviceName, 
            [In] ref DEVMODE lpDevMode,
            IntPtr hwnd, 
            int dwFlags,
            IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);

        private static short _tDeviceModeSize = (short)Marshal.SizeOf(typeof(DEVMODE));
        public static void CreateDEVMODE(out DEVMODE o_aStruct)
        {
            o_aStruct = new DEVMODE();
            o_aStruct.dmSize =  _tDeviceModeSize;
            o_aStruct.dmDriverExtra = 0;
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);



        public const int SC_MONITORPOWER = 0xF170;
        public const int WM_SYSCOMMAND = 0x0112;

        public const int MONITORON = -1;
        public const int MONITOROFF = 2;
        public const int MONITORSTANBY = 1;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(Int32 hWnd, uint Msg, Int32 wParam, Int32 lParam);

        // handling for hiding the close button
        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        public const uint SC_CLOSE = 0xF060;
        public const uint MF_GRAYED = 0x00000001;
        public const uint MF_BYCOMMAND = 0x00000000;

        [DllImport("user32.dll")]
        public static extern IntPtr RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);
    }
}
