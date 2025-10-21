using System;
using System.IO;

using Contal.Cgp.Globals;
using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(300)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class AACardReader : IDbObject
    {
        [LwSerialize]
        public Guid IdAACardReader
        {
            get; 
            set;
        }

        private Guid _guidAlarmArea = Guid.Empty;

        [LwSerialize]
        public Guid GuidAlarmArea 
        {
            get
            {
                return _guidAlarmArea;
            }
            set
            {
                _guidAlarmArea = value;
            } 
        }

        private Guid _guidCardReader = Guid.Empty;

        [LwSerialize]
        public Guid GuidCardReader
        {
            get
            {
                return _guidCardReader;
            }

            set
            {
                _guidCardReader = value;
            }
        }

        [LwSerialize]
        public bool AASet
        {
            get; 
            set;
        }

        [LwSerialize]
        public bool AAUnset
        {
            get; 
            set;
        }

        [LwSerialize]
        public bool AAUnconditionalSet
        {
            get; 
            set;
        }

        [LwSerialize]
        public bool PermanentlyUnlock
        {
            get; 
            set;
        }

        [LwSerialize]
        public bool EnableEventlog
        {
            get; 
            set;
        }

        public Guid GetGuid()
        {
            return IdAACardReader;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.AACardReader;
        }
    }
}
