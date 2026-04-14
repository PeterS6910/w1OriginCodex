using System;
using System.IO;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    public enum CardState : byte
    {
        [Name("active")]
        active = 0,
        [Name("blocked")]
        blocked = 1,
        [Name("unused")]
        unused = 2,
        [Name("lost")]
        lost = 3,
        [Name("destroyed")]
        destroyed = 4,
        [Name("temporarilyBlocked")]
        temporarilyBlocked = 5
    }

    [LwSerialize(202)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class Card : IDbObject, ICard
    {
        [LwSerialize]
        public Guid IdCard
        {
            get; 
            set;
        }

        [LwSerialize]
        public string FullCardNumber
        {
            get;
            set;
        }

        private Guid _guidCardSystem = Guid.Empty;

        [LwSerialize]
        public Guid GuidCardSystem
        {
            get { return _guidCardSystem; }

            set { _guidCardSystem = value; }
        }

        private Guid _guidPerson = Guid.Empty;

        [LwSerialize]
        public Guid GuidPerson
        {
            get { return _guidPerson; } 
            
            set { _guidPerson = value; }
        }

        [LwSerialize]
        public string Pin
        {
            get; 
            set;
        }

        [LwSerialize]
        public byte PinLength
        {
            get;
            set;
        }

        private byte _state;

        [LwSerialize]
        public byte State
        {
            get { return _state; }
            //CCU does not use hybrid symbol, therefore state must be transfered to non hybrid
            set { _state = (byte)(value & 0x0f); }
        }

        [LwSerialize]
        public DateTime UtcDateStateLastChange
        {
            get;
            set;
        }

        [LwSerialize]
        public DateTime? ValidityDateFrom { get; set; }

        [LwSerialize]
        public DateTime? ValidityDateTo { get; set; }

        public Guid GetGuid()
        {
            return IdCard;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.Card;
        }

        public bool IsValid 
        {
            get
            {
                var actualDate = DateTime.Now;

                bool isValid = true;

                if (ValidityDateFrom != null)
                    isValid = ValidityDateFrom.Value <= actualDate;

                if (ValidityDateTo != null)
                    isValid = ValidityDateTo.Value >= actualDate;

                return isValid; 
                
            }
        }
    }
}
