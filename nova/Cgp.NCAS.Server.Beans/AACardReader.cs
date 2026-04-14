using System;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(300)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class AACardReader : AOrmObjectWithVersion
    {
        public const string COLUMNIDAACARDREADER = "IdAACardReader";
        public const string COLUMNALARMAREA = "AlarmArea";
        public const string COLUMNGUIDALARMAREA = "GuidAlarmArea";
        public const string COLUMNCARDREADER = "CardReader";
        public const string COLUMNGUIDCARDREADER = "GuidCardReader";
        public const string COLUMNAASET = "AASet";
        public const string COLUMNAAUNSET = "AAUnset";
        public const string COLUMNAAUNCONDITIONALSET = "AAUnconditionalSet";
        public const string COLUMNPERMANENTLYUNLOCK = "PermanentlyUnlock";
        public const string COLUMNENABLEEVENTLOG = "EnableEventlog";
        public const string ColumnVersion = "Version";

        [LwSerializeAttribute]
        public virtual Guid IdAACardReader { get; set; }
        public virtual AlarmArea AlarmArea { get; set; }
        private Guid _guidAlarmArea = Guid.Empty;
        [LwSerializeAttribute]
        public virtual Guid GuidAlarmArea { get { return _guidAlarmArea; } set { _guidAlarmArea = value; } }
        public virtual CardReader CardReader { get; set; }
        private Guid _guidCardReader = Guid.Empty;
        [LwSerializeAttribute]
        public virtual Guid GuidCardReader { get { return _guidCardReader; } set { _guidCardReader = value; } }
        [LwSerializeAttribute]
        public virtual bool AASet { get; set; }
        [LwSerializeAttribute]
        public virtual bool AAUnset { get; set; }
        [LwSerializeAttribute]
        public virtual bool AAUnconditionalSet { get; set; }
        [LwSerializeAttribute]
        public virtual bool PermanentlyUnlock { get; set; }
        [LwSerializeAttribute]
        public virtual bool EnableEventlog { get; set; }

        public override bool Compare(object obj)
        {
            var aaCardReader = obj as AACardReader;

            return 
                aaCardReader != null && 
                aaCardReader.IdAACardReader == IdAACardReader;
        }

        public virtual void PrepareToSend()
        {
            GuidAlarmArea = 
                AlarmArea != null 
                    ? AlarmArea.IdAlarmArea
                    : Guid.Empty;

            GuidCardReader = 
                CardReader != null
                    ? CardReader.IdCardReader
                    : Guid.Empty;
        }

        public override string GetIdString()
        {
            return IdAACardReader.ToString();
        }

        public override object GetId()
        {
            return IdAACardReader;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.AACardReader;
        }
    }
}
