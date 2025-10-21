using System;
using System.Drawing;

using Contal.Cgp.Globals;

namespace Contal.Cgp.NCAS.Server.Beans.Shorts
{
    [Serializable]
    public class FloorShort : IShortObject
    {
        public const string ColumnIdFloor = "IdFloor";
        public const string ColumnName = "Name";
        public const string ColumnFloorNumber = "DoorIndex";
        public const string ColumnDescription = "Description";
        public const string ColumnSymbol = "Symbol";

        public Guid IdFloor { get; private set; }
        public string Name { get; private set; }
        public int FloorNumber { get; private set; }
        public string Description { get; private set; }
        public Image Symbol { get; set; }

        public FloorShort(Floor floor)
        {
            IdFloor = floor.IdFloor;
            Name = floor.ToString();
            FloorNumber = floor.FloorNumber;
            Description = floor.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.Floor; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }
        
        public object Id { get { return IdFloor; } }

        #endregion
    }
}
