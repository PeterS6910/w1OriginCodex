using System;
using Contal.IwQuick.Sys.Microsoft;

namespace Contal.Cgp.NCAS.CCU
{
    class NandFlashInfo
    {
        private readonly UInt64 _freeBytesAvailable;
        private readonly UInt64 _totalNumberOfBytes;

        public UInt64 FreeBytesAvailable { get { return _freeBytesAvailable; } }
        public UInt64 TotalNumberOfBytes { get { return _totalNumberOfBytes; } }

        public NandFlashInfo()
        {
            ulong outFlashTotalNumberOfFreeBytes;

            DllCoredll.GetDiskFreeSpaceEx(
                @"\NandFlash",
                out _freeBytesAvailable,
                out _totalNumberOfBytes,
                out outFlashTotalNumberOfFreeBytes);
        }
    }
}
