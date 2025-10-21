using System;
using Contal.Cgp.NCAS.Globals;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    public class APBZCardReader
    {
        public const string COLUMN_ENTRY_EXIT_BY = "EntryExitBy";
        public const string COLUMN_DIRECTION = "Direction";

        public virtual ApbzCardReaderEntryExitBy EntryExitBy { get; set; }
        public virtual bool Direction { get; set; }
    }
}
