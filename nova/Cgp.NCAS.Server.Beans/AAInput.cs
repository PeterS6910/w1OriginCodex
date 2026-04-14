using System;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(301)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class AAInput : AOrmObjectWithVersion
    {
        public const string COLUMNIDAAINPUT = "IdAAInput";
        public const string COLUMN_ID = "Id";
        public const string COLUMN_SECTION_ID = "SectionId";
        public const string COLUMNALARMAREA = "AlarmArea";
        public const string COLUMNINPUT = "Input";
        public const string COLUMNINPUTNAME = "SensorName";
        public const string COLUMNINPUTNICKNAME = "SensorNickName";
        public const string COLUMNGUIDINPUT = "GuidInput";
        public const string COLUMNNOCRITICALINPUT = "NoCriticalInput";
        public const string COLUMN_BLOCK_TEMPORARILY_UNTIL = "BlockTemporarilyUntil";
        public const string COLUMN_SENSOR_PURPOSE = "Purpose";
        public const string ColumnVersion = "Version";
        public const BlockTemporarilyUntilType DEFAULT_BLOCK_TEMPORARILY_UNTIL_TYPE = BlockTemporarilyUntilType.AreaUnset;

        [LwSerializeAttribute()]
        public virtual Guid IdAAInput { get; set; }
        [LwSerialize]
        public virtual int Id { get; set; }

        public virtual string SectionId
        {
            get
            {
                return
                    string.Format(
                        "{0}{1}",
                        AlarmArea.SectionId,
                        Id.ToString("D2"));
            }
        }

        public virtual AlarmArea AlarmArea { get; set; }
        public virtual string SensorName {
            get { return Input.FullName; }
        }

        public virtual string SensorNickName
        {
            get { return Input.NickName; }
        }
        public virtual Input Input { get; set; }
        private Guid _guidInput = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidInput { get { return _guidInput; } set { _guidInput = value; } }
        [LwSerializeAttribute()]
        public virtual bool NoCriticalInput { get; set; }
        [LwSerializeAttribute()]
        public virtual byte BlockTemporarilyUntil { get; set; }
        [LwSerialize]
        public virtual SensorPurpose? Purpose { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is AAInput)
            {
                return (obj as AAInput).IdAAInput == IdAAInput;
            }
            else
            {
                return false;
            }
        }

        public virtual void PrepareToSend()
        {
            if (Input != null)
            {
                GuidInput = Input.IdInput;
            }
            else
            {
                GuidInput = Guid.Empty;
            }
        }

        public override string GetIdString()
        {
            return IdAAInput.ToString();
        }

        public override object GetId()
        {
            return IdAAInput;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.AAInput;
        }
    }

    public enum BlockTemporarilyUntilType : byte
    {
        AreaUnset = 0,
        SensorStateNormal = 1
    }
}
