using System;

using Contal.IwQuick.Data;
namespace Contal.Cgp.NCAS.Globals
{
    [Serializable()]
    [LwSerialize(121)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class ThreadInfo
    {
        public const string COLUMN_ID = "Id";
        public const string COLUMN_PRIORITY = "Priority";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_IS_BACKGROUND = "IsBackground";        

        private uint _id = 0;
        [LwSerializeAttribute()]
        public uint Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _name = null;
        [LwSerializeAttribute()]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _priority = 0;
        [LwSerializeAttribute()]
        public int Priority
        {
            get { return _priority; }
            set { _priority = value; }
        }

        private bool? _isBackground = null;
        [LwSerializeAttribute()]
        public bool? IsBackground
        {
            get { return _isBackground; }
            set { _isBackground = value; }
        }

        public ThreadInfo()
        {

        }

        public override string ToString()
        {
            return _id + ":" + _name ?? "";
        }
    }
}
