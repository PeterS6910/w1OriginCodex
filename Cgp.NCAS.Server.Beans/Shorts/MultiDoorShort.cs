using System;
using System.Drawing;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;

namespace Contal.Cgp.NCAS.Server.Beans.Shorts
{
    [Serializable]
    public class MultiDoorShort : IShortObject
    {
        public const string ColumnIdMultiDoorEnvironment = "IdMultiDoor";
        public const string ColumnName = "Name";
        public const string ColumnType = "Type";
        public const string ColumnStringType = "StringType";
        public const string ColumnDescription = "Description";
        public const string ColumnSymbol = "Symbol";

        public Guid IdMultiDoor { get; private set; }
        public string Name { get; private set; }
        public MultiDoorType Type { get; private set; }
        public string StringType { get; set; }
        public string Description { get; private set; }
        public Image Symbol { get; set; }

        public MultiDoorShort(MultiDoor multiDoor)
        {
            IdMultiDoor = multiDoor.IdMultiDoor;
            Name = multiDoor.ToString();
            Type = multiDoor.Type;
            Description = multiDoor.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.MultiDoor; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }
        
        public object Id { get { return IdMultiDoor; } }

        #endregion
    }
}
