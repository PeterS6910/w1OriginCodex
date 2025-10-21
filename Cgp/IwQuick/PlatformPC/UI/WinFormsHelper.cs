using System;
using System.Windows.Forms;
using JetBrains.Annotations;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Navigation;

namespace Contal.IwQuick.UI
{
    public enum ScreenDpi
    {
        Unknown = 0,    // Unknown DPI
        Dpi96   = 100,  // 96 = 100% Zoom (Default)
        Dpi120  = 125,  // 120 = 125% Zoom
        Dpi144  = 150   // 144 = 150% Zoom
    }

    public enum DeviceCap
    {
        /// <summary>
        /// Logical pixels inch in X
        /// </summary>
        LogPixelsX = 88,
        /// <summary>
        /// Logical pixels inch in Y
        /// </summary>
        LogPixelsY = 90

        // Other constants may be founded on pinvoke.net: http://pinvoke.net/default.aspx/gdi32/GetDeviceCaps.html
    }

    public static class WinFormsHelper
    {
        public static void ProcessEventAs<T>(
            [NotNull] this Form form, 
            [NotNull] Action<T, EventArgs> lambda, 
            bool checkWhetherControlFocused,
            object senderControl,
            EventArgs eventArgs)
            where T:Control
        {
// ReSharper disable ConditionIsAlwaysTrueOrFalse
            if (ReferenceEquals(form,null) ||
                ReferenceEquals(lambda,null))
// ReSharper disable once HeuristicUnreachableCode
                return;
// ReSharper restore ConditionIsAlwaysTrueOrFalse

            var senderAsT = senderControl as T;
            if (ReferenceEquals(senderAsT,null))
                return;

            if (checkWhetherControlFocused &&
                !senderAsT.Focused)
                return;

            lambda(senderAsT, eventArgs);
        }

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int GetDeviceCaps(IntPtr hDC, int nIndex);

#if PC35

        public static ScreenDpi GetCurrentDPI()
        {
            return GetCurrentDPI(false);
        }

        // Method to get current DPI setting in windows (96 - 100%, 120 - 125%, 144 - 150%, ...)
        public static ScreenDpi GetCurrentDPI(bool getDpiY)
#else
        // Method to get current DPI setting in windows (96 - 100%, 120 - 125%, 144 - 150%, ...)
        public static ScreenDpi GetCurrentDPI(bool getDpiY = false)
#endif
        {
            Graphics graphics = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktopDC = graphics.GetHdc();

            int dpiX = GetDeviceCaps(desktopDC, (int)DeviceCap.LogPixelsX);
            int dpiY = GetDeviceCaps(desktopDC, (int)DeviceCap.LogPixelsY);
            int dpi = getDpiY ? dpiY : dpiX;

            switch (dpi)
            {
                case 96:
                    return ScreenDpi.Dpi96;

                case 120:
                    return ScreenDpi.Dpi120;

                case 144:
                    return ScreenDpi.Dpi144;
            }

            return ScreenDpi.Unknown;
        }

        public static int DpiScaleX(int value)
        {
            double scaleX = (double)GetCurrentDPI() / 100.0;

            return (int)(value * scaleX);
        }

        public static int DpiScaleY(int value)
        {
            double scaleY = (double)GetCurrentDPI(true) / 100.0;

            return (int)(value * scaleY);
        }

        private static readonly Delegate TraverseSubcontrolDelegeate = new Action<Control, Action<Control>>(TraverseSubcontrols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentControl"></param>
        /// <param name="actionOverControl"></param>
        public static void TraverseSubcontrols(
            [NotNull] Control parentControl,
            [NotNull] Action<Control> actionOverControl
            )
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (parentControl == null 
                // ReSharper disable once MergeSequentialChecks
                || parentControl.Controls == null
                || actionOverControl == null)
                // ReSharper disable once HeuristicUnreachableCode
                return;

            if (parentControl.InvokeRequired)
            {
                parentControl.Invoke(
                    TraverseSubcontrolDelegeate
                    ,parentControl
                    ,actionOverControl);
                return;
            }

            try
            {
                actionOverControl(parentControl);
            }
            catch
            {
                
            }

            foreach (var c in parentControl.Controls)
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                TraverseSubcontrols(c as Control,actionOverControl);
            }
        }
    }
}
