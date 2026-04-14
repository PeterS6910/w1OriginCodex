using System;
using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    public class DllUxTheme
    {
        public const string DLL_PATH = "UxTheme.dll";

        [DllImport(DLL_PATH, SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);
    }
}
