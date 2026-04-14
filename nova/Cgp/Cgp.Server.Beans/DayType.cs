using System;
using System.Collections.Generic;

using Contal.IwQuick.Data;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    [LwSerialize(207)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class DayType : AOrmObjectWithVersion
    {
        public const string COLUMNIDDAYTYPE = "IdDayType";
        public const string COLUMNDAYTYPENAME = "DayTypeName";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNTIMEZONEDATESETTING = "TimeZoneDateSetting";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string ColumnVersion = "Version";

        public const string IMPLICIT_DAY_TYPE_HOLIDAY = "implicitDayTypeHoliday";
        public const string IMPLICIT_DAY_TYPE_VACATION = "implicitDayTypeVacation";

        [LwSerialize]
        public virtual Guid IdDayType { get; set; }
        public virtual string DayTypeName { get; set; }
        public virtual ICollection<TimeZoneDateSetting> TimeZoneDateSetting { get; set; }
        public virtual string Description { get; set; }
        public virtual byte ObjectType { get; set; }

        public DayType()
        {
            ObjectType = (byte)Globals.ObjectType.DayType;
        }

        public override string ToString()
        {
            return DayTypeName;
        }

        public override bool Compare(object obj)
        {
            var dayType = obj as DayType;

            return 
                dayType != null && 
                dayType.IdDayType == IdDayType;
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();

            if (DayTypeName == IMPLICIT_DAY_TYPE_HOLIDAY ||
                DayTypeName == IMPLICIT_DAY_TYPE_VACATION)
            {
                return true;
            }

            return 
                DayTypeName.ToLower().Contains(expression) || 
                    Description != null &&
                    Description.ToLower().Contains(expression);
        }

        public override string GetIdString()
        {
            return IdDayType.ToString();
        }

        public override object GetId()
        {
            return IdDayType;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new DayTypeModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.DayType;
        }
    }

    [Serializable]
    public class DayTypeShort : IShortObject
    {
        public const string COLUMNIDDAYTYPE = "IdDayType";
        public const string COLUMNDAYTYPENAME = "DayTypeName";
        public const string COLUMNDESCRIPTION = "Description";

        public const string IMPLICIT_DAY_TYPE_HOLIDAY = "implicitDayTypeHoliday";
        public const string IMPLICIT_DAY_TYPE_VACATION = "implicitDayTypeVacation";

        public Guid IdDayType { get; set; }
        public string DayTypeName { get; set; }
        public string Description { get; set; }

        public DayTypeShort(DayType dayType)
        {
            IdDayType = dayType.IdDayType;
            DayTypeName = dayType.DayTypeName;
            Description = dayType.Description;
        }

        public override string ToString()
        {
            return DayTypeName;
        }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.DayType; } }

        public string Name { get { return DayTypeName; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public object Id { get { return IdDayType; } }

        #endregion
    }

    [Serializable]
    public class DayTypeModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.DayType; } }
        private readonly bool _isDefaultDayType;

        public DayTypeModifyObj(DayType dayType)
        {
            Id = dayType.IdDayType;
            FullName = dayType.ToString();
            Description = dayType.Description;

            if (dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_HOLIDAY ||
                dayType.DayTypeName == DayType.IMPLICIT_DAY_TYPE_VACATION)
            {
                _isDefaultDayType = true;
            }
        }

        public override bool Contains(string expression)
        {
            if (String.IsNullOrEmpty(expression)) return true;
            if (_isDefaultDayType) return true;
            if (AOrmObject.RemoveDiacritism(FullName).ToLower().Contains(AOrmObject.RemoveDiacritism(expression).ToLower()))
            {
                return true;
            }

            if (Description != null)
            {
                return AOrmObject.RemoveDiacritism(Description).ToLower().Contains(AOrmObject.RemoveDiacritism(expression).ToLower());
            }

            return false;
        }
    }
}
