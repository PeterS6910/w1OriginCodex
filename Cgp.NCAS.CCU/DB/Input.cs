using System;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;
using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.CCU.DB
{
    [LwSerialize(319)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class Input : IDbObject, IComparable
    {
        [LwSerialize]
        public virtual Guid IdInput { get; set; }
        [LwSerialize]
        public virtual byte InputNumber { get; set; }
        [LwSerialize]
        public virtual string NickName { get; set; }
        [LwSerialize]
        public virtual byte InputType { get; set; }
        [LwSerialize]
        public virtual bool Inverted { get; set; }
        public virtual bool HighPriority { get; set; }
        [LwSerialize]
        public virtual bool OffACK { get; set; }
        [LwSerialize]
        public virtual int DelayToOn { get; set; }
        [LwSerialize]
        public virtual int DelayToOff { get; set; }
        [LwSerialize]
        public virtual int TamperDelayToOn { get; set; }
        private Guid _guidDCU = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidDCU { get { return _guidDCU; } set { _guidDCU = value; } }
        private Guid _guidCCU = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidCCU { get { return _guidCCU; } set { _guidCCU = value; } }
        [LwSerialize]
        public virtual bool AlarmOn { get; set; }
        [LwSerialize]
        public virtual bool AlarmTamper { get; set; }
        [LwSerialize]
        public virtual byte BlockingType { get; set; }

        [LwSerialize]
        public virtual ObjectType OnOffObjectObjectType { get; set; }

        [LwSerialize]
        public virtual Guid? OnOffObjectId { get; set; }

        public Guid GetGuid()
        {
            return IdInput;
        }

        public ObjectType GetObjectType()
        {
            return ObjectType.Input;
        }

        public override string ToString()
        {
            return "Input" + InputNumber;
        }

        public int CompareTo(object obj)
        {
            var input = obj as Input;

            return input != null ? NickName.CompareTo(input.NickName) : -1;
        }
    }
}