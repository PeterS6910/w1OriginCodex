using System;
using Contal.Drivers.LPC3250;

namespace Contal.Cgp.NCAS.CCU
{
    public class SdCardInfo
    {
        public bool Present { get; private set; }
        public UInt64 FreeSpace { get; private set; }
        public UInt64 Size { get; private set; }

        public SdCardInfo()
        {
            Present = Program.SDCardPresent;
            FreeSpace = SystemUtility.SDcard.FreeSpace;
            Size = SystemUtility.SDcard.Size;
        }
    }
}
