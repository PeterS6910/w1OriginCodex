using System;
using System.Drawing;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Definitions;

namespace Contal.Cgp.NCAS.Server.Beans.Shorts
{
    [Serializable]
    public class MultiDoorElementShort : IShortObject
    {
        public const string ColumnIdMultiDoorElement = "IdMultiDoorElement";
        public const string ColumnName = "Name";
        public const string ColumnDoorIndex = "DoorIndex";
        public const string ColumnState = "State";
        public const string ColumnStringState = "StringState";
        public const string ColumnDescription = "Description";
        public const string ColumnSymbol = "Symbol";

        public Guid IdMultiDoorElement { get; private set; }
        public string Name { get; private set; }
        public int DoorIndex { get; private set; }
        public DoorEnvironmentState State { get; set; }
        public string StringState { get; set; }
        public string Description { get; private set; }
        public Image Symbol { get; set; }

        public MultiDoorElementShort(MultiDoorElement multiDoorElement)
            : this(multiDoorElement, true)
        {

        }

        public MultiDoorElementShort(MultiDoorElement multiDoorElement, bool setFullName)
        {
            IdMultiDoorElement = multiDoorElement.IdMultiDoorElement;
            Name = setFullName ? multiDoorElement.ToString() : multiDoorElement.Name;
            DoorIndex = multiDoorElement.DoorIndex;
            Description = multiDoorElement.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.MultiDoorElement; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdMultiDoorElement; } }

        #endregion
    }
}
