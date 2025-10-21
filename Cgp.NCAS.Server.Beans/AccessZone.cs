using System;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable()]
    [LwSerialize(303)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class AccessZone : AOrmObjectWithVersion
    {
        public const string COLUMNIDACCESSZONE = "IdAccessZone";
        public const string COLUMNPERSON = "Person";
        public const string COLUMNGUIDPERSON = "GuidPerson";
        public const string COLUMNCARDREADEROBJECT = "CardReaderObject";
        public const string COLUMNGUIDCARDREADEROBJECT = "GuidCardReaderObject";
        public const string COLUMNCARDREADEROBJECTTYPE = "CardReaderObjectType";
        public const string COLUMNTIMEZONE = "TimeZone";
        public const string COLUMNGUIDTIMEZONE = "GuidTimeZone";
        public const string COLUMNDESCRIPTION = "Description";
        public const string ColumnVersion = "Version";

        [LwSerializeAttribute()]
        public virtual Guid IdAccessZone { get; set; }
        public virtual Person Person { get; set; }
        private Guid _guidPerson = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidPerson { get { return _guidPerson; } set { _guidPerson = value; } }
        private AOrmObject _cardReaderObject = null;
        public virtual AOrmObject CardReaderObject { get { return _cardReaderObject; } set { _cardReaderObject = value; } }
        [LwSerializeAttribute()]
        public virtual Guid GuidCardReaderObject { get; set; }
        [LwSerializeAttribute()]
        public virtual byte CardReaderObjectType { get; set; }
        public virtual Contal.Cgp.Server.Beans.TimeZone TimeZone { get; set; }
        private Guid _guidTimeZone = Guid.Empty;
        [LwSerializeAttribute()]
        public virtual Guid GuidTimeZone { get { return _guidTimeZone; } set { _guidTimeZone = value; } }
        public virtual string Description { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is AccessZone)
            {
                return (obj as AccessZone).IdAccessZone == IdAccessZone;
            }
            else
            {
                return false;
            }
        }

        public virtual void PrepareToSend()
        {
            if (Person != null)
            {
                GuidPerson = Person.IdPerson;
            }
            else
            {
                GuidPerson = Guid.Empty;
            }

            if (TimeZone != null)
            {
                GuidTimeZone = TimeZone.IdTimeZone;
            }
            else
            {
                GuidTimeZone = Guid.Empty;
            }
        }

        public override string GetIdString()
        {
            return IdAccessZone.ToString();
        }

        public override object GetId()
        {
            return IdAccessZone;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.AccessZone;
        }
    }

    public class AccessZoneShort : IShortObject
    {
        public AccessZone AccessZone { get; private set; }

        public AccessZoneShort(AccessZone accessZone)
        {
            AccessZone = accessZone;
        }

        #region IShortObject Members

        public ObjectType ObjectType
        {
            get { return ObjectType.AccessZone; }
        }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        public string Name
        {
            get { return AccessZone.CardReaderObject.ToString(); }
        }

        public object Id
        {
            get { return AccessZone.IdAccessZone; }
        }

        #endregion
    }
}
