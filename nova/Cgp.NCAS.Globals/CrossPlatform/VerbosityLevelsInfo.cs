using System;
using System.Collections.Generic;
using System.Text;

using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Globals
{
    [Serializable()]
    [LwSerialize(122)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class VerbosityLevelInfo
    {
        private string _name = string.Empty;
        [LwSerializeAttribute()]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private byte _level = 0;
        [LwSerializeAttribute()]
        public byte Level
        {
            get { return _level; }
            set { _level = value; }
        }

        public VerbosityLevelInfo()
        {

        }

        public VerbosityLevelInfo(string name, byte level)
        {
            _name = name;
            _level = level;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
