using System;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Globals
{
    [Serializable]
    [LwSerialize(123)]
    [LwSerializeMode(LwSerializationMode.Direct)]
    public class MemoryInfo
    {
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_COUNT = "Count";

        public string Name { get; private set; }
        public int Count { get; private set; }

        private MemoryInfo()
        {
            
        }

        public MemoryInfo(
            string name,
            int count)
        {
            Name = name;
            Count = count;
        }
    }
}
