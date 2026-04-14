using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class AlarmPriorityDatabase : AOrmObject
    {
        public virtual byte AlarmTypeDb { get; set; }
        public virtual byte AlarmPriorityDb { get; set; }
        public virtual int? ClosestParentObjectDb { get; set; }
        public virtual int? SecondClosestParentObjectDb { get; set; }
        public virtual AlarmType AlarmType { get { return (AlarmType)AlarmTypeDb; } }
        public virtual AlarmPriority AlarmPriority { get { return (AlarmPriority)AlarmPriorityDb; } }

        public virtual ObjectType? ClosestParentObject
        {
            get
            {
                if (ClosestParentObjectDb != null)
                    return (ObjectType)ClosestParentObjectDb.Value;
                else
                    return null;
            }
        }

        public virtual ObjectType? SecondClosestParentObject
        {
            get
            {
                if (SecondClosestParentObjectDb != null)
                    return (ObjectType)SecondClosestParentObjectDb.Value;
                else
                    return null;
            }
        }

        public AlarmPriorityDatabase()
        {
        }

        public AlarmPriorityDatabase(AlarmType aType, AlarmPriority aPriority, ObjectType? closestParentObject, ObjectType? secondClosestParentObject)
        {
            AlarmTypeDb = (byte)aType;
            AlarmPriorityDb = (byte)aPriority;

            if (closestParentObject != null)
                ClosestParentObjectDb = (int)closestParentObject;
            else
                ClosestParentObjectDb = null;

            if (secondClosestParentObject != null)
                SecondClosestParentObjectDb = (int)secondClosestParentObject;
            else
                SecondClosestParentObjectDb = null;
        }

        public override string ToString()
        {
            return AlarmTypeDb.ToString();
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is AlarmPriorityDatabase)
            {
                return (obj as AlarmPriorityDatabase).AlarmTypeDb == AlarmTypeDb;
            }
            else
            {
                return false;
            }
        }

        //public override bool Contains(string expression)
        //{
        //    expression = expression.ToLower();
        //    if (this.Birthday.ToString().ToLower().Contains(expression)) return true;
        //    if (this.FirstName.ToLower().Contains(expression)) return true;
        //    if (this.Surname.ToLower().Contains(expression)) return true;
        //    if (this.Identification != null)
        //    {
        //        if (this.Identification.ToLower().Contains(expression)) return true;
        //    }
        //    if (this.Description != null)
        //    {
        //        if (this.Description.ToLower().Contains(expression)) return true;
        //    }
        //    return false;
        //}

        public override string GetIdString()
        {
            return AlarmTypeDb.ToString();
        }

        public override object GetId()
        {
            return AlarmTypeDb;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.Person;
        }
    }
}
