using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using Contal.IwQuick.Sys.Microsoft;

namespace Contal.IwQuick.Sys.Microsoft
{
    public static class FileVersionInfo

    {

        public static Version GetVersionInfo(string path)
        {

            if (!File.Exists(path))
                throw new FileNotFoundException();

            IntPtr handle;
            var size = DllCoredll.GetFileVersionInfoSize(path, out handle);

            var buffer = Marshal.AllocHGlobal(size);

            try
            {

                if (!DllCoredll.GetFileVersionInfo(path, handle, size, buffer))
                {
                    var lastError = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastError);

                }


                IntPtr pVersion;
                int versionLength;

                DllCoredll.VerQueryValue(buffer, "\\", out pVersion, out versionLength);


                var versionInfo = (DllCoredll.VS_FIXEDFILEINFO)Marshal.PtrToStructure(pVersion, typeof(DllCoredll.VS_FIXEDFILEINFO));

                var version = new Version((int) versionInfo.dwFileVersionMS >> 16,
                    (int) versionInfo.dwFileVersionMS & 0xFFFF,
                    (int) versionInfo.dwFileVersionLS >> 16,
                    (int) versionInfo.dwFileVersionLS & 0xFFFF);

                return version;

            }
            finally
            {
                Marshal.FreeHGlobal(buffer);
            }

        }


        

    }

}
