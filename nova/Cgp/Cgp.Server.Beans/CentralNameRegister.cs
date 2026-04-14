using System;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class CentralNameRegister : AOrmObject
    {
        public const string COLUMN_ID = "Id";
        public const string COLUMN_NAME = "Name";
        public const string COLUMN_ALTERNATE_NAME = "AlternateName";
        public const string COLUMN_OBJECTTYPE = "ObjectType";
        public const string COLUMN_FULL_TEXT_SEARCH_STRING = "FullTextSearchString";

        public virtual Guid Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string AlternateName { get; set; }
        public virtual string FullTextSearchString { get; set; }
        public virtual byte ObjectType { get; set; }
        public virtual Guid CkUnique { get; set; }

        public const string IMPLICIT_CN_SERVER_NAME= "Server";
        public const string IMPLICIT_CN_CLIENT_NAME = "Client";
        public const string IMPLICIT_CN_DATABASE_NAME = "Database";      

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            var centraRegister = obj as CentralNameRegister;

            if (centraRegister != null)
                return centraRegister.Id == Id;

            return false;
        }

        public override string GetIdString()
        {
            return Id.ToString();
        }

        public override object GetId()
        {
            return Id;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.CentralNameRegister;
        }

    }
}


