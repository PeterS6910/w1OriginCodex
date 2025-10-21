using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(308)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class AntiPassBackZone : AOrmObjectWithVersion
    {
        public const string COLUMNIDANTIPASSBACKZONE = "IdAntiPassBackZone";
        public const string COLUMNNAME = "Name";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdAntiPassBackZone
        {
            get; 
            set;
        }

        public virtual string Name
        {
            get;
            set;
        }

        public virtual byte ObjectType
        {
            get
            {
                return (byte)Cgp.Globals.ObjectType.AntiPassBackZone;
            }

            set
            {
            }
        }

        public virtual IDictionary<CardReader, APBZCardReader> EntryCardReaders { get; set; }
        public virtual IDictionary<CardReader, APBZCardReader> ExitCardReaders { get; set; }

        [LwSerialize]
        public virtual Dictionary<Guid, ApbzCardReaderEntryExitBy> GuidEntryCardReaders
        {
            get
            {
                return EntryCardReaders.ToDictionary(
                    kvPair => kvPair.Key.IdCardReader,
                    kvPair => kvPair.Value.EntryExitBy);
            }
        }

        [LwSerialize]
        public virtual Dictionary<Guid, ApbzCardReaderEntryExitBy> GuidExitCardReaders
        {
            get
            {
                return ExitCardReaders.ToDictionary(
                    kvPair => kvPair.Key.IdCardReader,
                    kvPair => kvPair.Value.EntryExitBy);
            }
        }

        [LwSerialize]
        public virtual bool ProhibitAccessForCardNotPresent { get; set; }

        [LwSerialize]
        public virtual Guid GuidDestinationAPBZAfterTimeout { get; set; }

        public virtual AntiPassBackZone DestinationAPBZAfterTimeout { get; set; }

        [LwSerialize]
        public virtual int Timeout { get; set; }

        public virtual string Description { get; set; }

        public virtual void PrepareToSend()
        {
            GuidDestinationAPBZAfterTimeout =
                DestinationAPBZAfterTimeout != null
                    ? DestinationAPBZAfterTimeout.IdAntiPassBackZone
                    : Guid.Empty;
        }

        public override bool Compare(object obj)
        {
            var otherAntiPassbackZone = obj as AntiPassBackZone;

            return 
                otherAntiPassbackZone != null &&
                IdAntiPassBackZone.Equals(otherAntiPassbackZone.IdAntiPassBackZone);
        }

        public override string GetIdString()
        {
            return GetId().ToString();
        }

        public override ObjectType GetObjectType()
        {
            return Cgp.Globals.ObjectType.AntiPassBackZone;
        }

        public override object GetId()
        {
            return IdAntiPassBackZone;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new AntiPassBackZoneModifyObj(this);
        }

        public override string ToString()
        {
            return Name;
        }

        private static CCU GetAnyCcuOfCardReaders(IEnumerable<CardReader> cardReaders)
        {
            return
                cardReaders
                    .Select(
                        cardReader =>
                            cardReader.CCU ?? (
                                cardReader.DCU != null
                                    ? cardReader.DCU.CCU
                                    : null))
                    .FirstOrDefault(ccu => ccu != null);
        }

        public virtual CCU GetParentCCU()
        {
            var entryCardReaders = EntryCardReaders;

            CCU ccu =
                entryCardReaders != null
                    ? GetAnyCcuOfCardReaders(entryCardReaders.Keys)
                    : null;

            if (ccu == null)
            {
                var exitCardReaders = ExitCardReaders;

                if (exitCardReaders != null)
                    ccu = GetAnyCcuOfCardReaders(exitCardReaders.Keys);
            }

            return ccu;
        }
    }

    [Serializable]
    public class AntiPassBackZoneModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType
        {
            get { return ObjectType.AntiPassBackZone; }
        }

        public AntiPassBackZoneModifyObj(AntiPassBackZone antiPassBackZone)
        {
            Id = antiPassBackZone.IdAntiPassBackZone;
            FullName = antiPassBackZone.ToString();
            Description = antiPassBackZone.Description;
        }
    }

    [Serializable]
    public class AntiPassBackZoneShort : IShortObject
    {
        public const string COLUMN_SYMBOL = "Symbol";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_DESCRIPTION = "Description";

        public AntiPassBackZoneShort(AntiPassBackZone antiPassBackZone)
        {
            Id = antiPassBackZone.IdAntiPassBackZone;
            Name = antiPassBackZone.Name;
            Description = antiPassBackZone.Description;
        }

        public ObjectType ObjectType
        {
            get
            {
                return ObjectType.AntiPassBackZone;
            }
        }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }


        public object Id { get; private set; }

        public string Name { get; private set; }
        public string Description { get; private set; }

        public Image Symbol { get; set; }
    }

    [LwSerialize(311)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class CCUCardInAntiPassBackZone
    {
        [LwSerialize]
        public Guid GuidCard { get; set; }

        [LwSerialize]
        public Guid GuidEntryCardReader { get; set; }

        [LwSerialize]
        public DateTime EntryDateTime { get; set; }

        [LwSerialize]
        public ApbzCardReaderEntryExitBy EntryBy { get; set; }
    }

    [Serializable]
    public class CardInAntiPassBackZone
    {
        public CardInAntiPassBackZone(
            Guid idCard,
            string name,
            string cardNumber,
            string entryCardReaderName,
            DateTime entryDateTime,
            ApbzCardReaderEntryExitBy entryBy)
        {
            IdCard = idCard;

            Name = name;
            CardNumber = cardNumber;

            EntryCardReaderName = entryCardReaderName;
            EntryDateTime = entryDateTime;
            EntryBy = entryBy;
        }

        public Guid IdCard { get; private set; }

        public string Name { get; private set; }
        public string CardNumber { get; private set; }

        public string EntryCardReaderName { get; private set; }
        public DateTime EntryDateTime { get; private set; }
        public ApbzCardReaderEntryExitBy EntryBy { get; set; }
    }
}
