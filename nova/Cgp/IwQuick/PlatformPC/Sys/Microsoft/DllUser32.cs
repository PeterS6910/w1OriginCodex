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

        public const int ENUM_CURRENT_SETTINGS = -1;
        public const int ENUM_REGISTRY_SETTINGS = -2;

        public const int WM_SIZE = 5;
        public const int WM_SYNCPAINT = 136;
        public const int WM_MOVE = 3;
        public const int WM_ACTIVATE = 6;
        public const int WM_LBUTTONDOWN = 513;
        public const int WM_LBUTTONUP = 514;
        public const int WM_LBUTTONDBLCLK = 515;
        public const int WM_MOUSEMOVE = 512;

        public const int WM_PAINT = 15;
        public const int WM_GETTEXT = 13;

        public const int WM_NCCREATE = 129;
        public const int WM_NCLBUTTONDOWN = 161;
        public const int WM_NCLBUTTONUP = 162;
        public const int WM_NCMOUSEMOVE = 160;
        public const int WM_NCACTIVATE = 134;
        public const int WM_NCPAINT = 133;
        public const int WM_NCHITTEST = 132;
        public const int WM_NCLBUTTONDBLCLK = 163;

        public const int VK_LBUTTON = 1;
        public const int SM_CXSIZE = 30;
        public const int SM_CYSIZE = 31;

        public const int WM_SYSCOMMAND = 274;//0x112;
        public const int SC_MINIMIZE = 61472;//0xF020;
        public const int SC_MAXIMIZE = 61488;//0xF030;


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

        private static readonly short _tDeviceModeSize = (short)Marshal.SizeOf(typeof(DEVMODE));
        public static void CreateDEVMODE(out DEVMODE o_aStruct)
        {
            o_aStruct = new DEVMODE {dmSize = _tDeviceModeSize, dmDriverExtra = 0};
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();


        public const int SC_MONITORPOWER = 0xF170;

        public const int MONITORON = -1;
        public const int MONITOROFF = 2;
        public const int MONITORSTANBY = 1;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, Int32 wParam, Int32 lParam);


        /// <summary>
        /// handling for hiding the close button
        /// </summary>
        /// <param name="hMenu"></param>
        /// <param name="uIDEnableItem"></param>
        /// <param name="uEnable"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool EnableMenuItem(IntPtr hMenu, uint uIDEnableItem, uint uEnable);

        [DllImport("user32.dll")]
        public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        public const uint SC_CLOSE = 0xF060;
        public const uint MF_GRAYED = 0x00000001;
        public const uint MF_BYCOMMAND = 0x00000000;

        [DllImport("user32.dll")]
        public static extern IntPtr RemoveMenu(IntPtr hMenu, uint nPosition, uint wFlags);

        /// <summary>
        /// ability to highlight taskbar menu
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="bInvert"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hwnd, bool bInvert);

        [DllImport("user32")]
        public static extern int GetWindowDC(int hwnd);

        [DllImport("user32")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32")]
        public static extern int SetCapture(int hwnd);

        [DllImport("user32")]
        public static extern int GetCapture();


        [DllImport("user32")]
        public static extern bool ReleaseCapture();

        [DllImport("user32")]
        public static extern int GetSysColor(int nIndex);

        [DllImport("user32")]
        public static extern int GetSystemMetrics(int nIndex);
    }
}
