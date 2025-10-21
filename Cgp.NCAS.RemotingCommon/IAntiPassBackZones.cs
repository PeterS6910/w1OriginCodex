using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{

    #region APBZ-related exceptions 

    [Serializable]
    public class CycleInExirationTargetsException : Exception
    {
        public CycleInExirationTargetsException()
        {
        }

        public CycleInExirationTargetsException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class ConflictingEntryCardReadersException : Exception
    {
        public ConflictingEntryCardReadersException()
        {
            
        }

        public ConflictingEntryCardReadersException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class TimeoutNotSetAndNoExitCardReaderException : Exception
    {
        public TimeoutNotSetAndNoExitCardReaderException()
        {
        }

        protected TimeoutNotSetAndNoExitCardReaderException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }
    }

    #endregion

    public interface IAntiPassBackZones : IBaseOrmTable<AntiPassBackZone>
    {
        ICollection<AntiPassBackZoneShort> ShortSelectByCriteria(
            out Exception error, 
            LogicalOperators filterJoinOperator, 
            params ICollection<FilterSettings>[] filterSettings);

        ICollection<IModifyObject> ModifyObjectsSelectByCriteria(
            ICollection<FilterSettings> filterSettings,
            out Exception err);

        IEnumerable<IModifyObject> GetAvailableExpirationTargets(
            Guid guidAntiPassBackZone,
            Guid guidCCU,
            out Exception err);

        ICollection<Guid> GetExistingEntryCardReaderGuids(
            Guid skippedAntiPassBackZone,
            ApbzCardReaderEntryExitBy entryExitBy);

        ICollection<IModifyObject> GetAPBZModifyObjectsForEntryCardReaders(
            ICollection<Guid> guidCardReaders,
            ApbzCardReaderEntryExitBy entryExitBy);

        void RemoveEntryCardReadersFromAntiPassBackZones(
            Guid guidSkippedAntiPassBackZone,
            ICollection<Guid> guidCardReaders,
            ApbzCardReaderEntryExitBy entryExitBy);

        ICollection<CardInAntiPassBackZone> GetCardsInZone(Guid idAntiPassbackZone);

        void RemoveCardsFromZone(
            Guid idAntiPassbackZone,
            ICollection<Guid> guidCardsToRemove);

        void AddCardsToZone(
            Guid idAntiPassbackZone,
            ICollection<Guid> guidCardsToAdd);

        ICollection<Card> GetCardsWhichCanBeAdded(Guid idApbz);
        bool CardCanBeAdded(Guid idApbz, Guid idCard);
        ICollection<Card> GetCardsWhichCanBeAdded(Guid idApbz, Guid idPerson);
    }
}
