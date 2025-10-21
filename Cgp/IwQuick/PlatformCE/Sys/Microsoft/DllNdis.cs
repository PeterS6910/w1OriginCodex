using System.Runtime.InteropServices;

namespace Contal.IwQuick.Sys.Microsoft
{
    public static class DllNdis
    {
        public const string DLL_NAME = @"ndis.dll";

        [DllImport(DLL_NAME, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern void NdisGetSystemUpTime(
            out ulong pSystemUpTime
            );
    }
}
