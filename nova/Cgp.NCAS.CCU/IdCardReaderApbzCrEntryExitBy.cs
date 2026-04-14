using System;

using Contal.Cgp.NCAS.Globals;

namespace Contal.Cgp.NCAS.CCU
{
    public class IdCardReaderApbzCrEntryExitBy : IEquatable<IdCardReaderApbzCrEntryExitBy>
    {
        public Guid IdCardReader { get; private set; }
        public ApbzCardReaderEntryExitBy EntryExitBy { get; private set; }

        public IdCardReaderApbzCrEntryExitBy(
            Guid idCardReader,
            ApbzCardReaderEntryExitBy entryExitBy)
        {
            IdCardReader = idCardReader;
            EntryExitBy = entryExitBy;
        }

        public bool Equals(IdCardReaderApbzCrEntryExitBy other)
        {
            if (other == null)
                return false;

            return other.IdCardReader == IdCardReader
                   && other.EntryExitBy == EntryExitBy;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as IdCardReaderApbzCrEntryExitBy);
        }

        public override int GetHashCode()
        {
            return IdCardReader.GetHashCode() ^ EntryExitBy.GetHashCode();
        }
    }
}
