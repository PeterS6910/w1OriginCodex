using System;
using System.Collections.Generic;
using System.Text;

using Contal.IwQuick.Data;
namespace Contal.Cgp.NCAS.Globals
{
    [Serializable()]
    [LwSerialize(120)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class BlockFileInfo
    {
        private string _name = string.Empty;
        [LwSerializeAttribute()]
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private int _blocksRemovedRuntime = 0;
        [LwSerializeAttribute()]
        public int BlocksRemovedRuntime
        {
            get { return _blocksRemovedRuntime; }
            set { _blocksRemovedRuntime = value; }
        }

        private int _blocksAddedRuntime = 0;
        [LwSerializeAttribute()]
        public int BlocksAddedRuntime
        {
            get { return _blocksAddedRuntime; }
            set { _blocksAddedRuntime = value; }
        }

        private long _fileSize = 0;
        [LwSerializeAttribute()]
        public long FileSize
        {
            get { return _fileSize; }
            set { _fileSize = value; }
        }

        public BlockFileInfo()
        {

        }

        public BlockFileInfo(string fileName)
        {
            _name = fileName;
        }
    }
}
