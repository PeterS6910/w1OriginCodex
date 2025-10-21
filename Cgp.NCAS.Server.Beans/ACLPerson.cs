using System;
using System.Collections.Specialized;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.Server.Beans
{
    [Serializable]
    [LwSerialize(304)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class ACLPerson : AOrmObjectWithVersion
    {
        public const string COLUMNIDACLPERSON = "IdACLPerson";
        public const string COLUMNACCESSCONTROLLIST = "AccessControlList";
        public const string COLUMNGUIDACCESSCONTROLLIST = "GuidAccessControlList";
        public const string COLUMNPERSON = "Person";
        public const string COLUMNGUIDPERSON = "GuidPerson";
        public const string COLUMNDATEFROM = "DateFrom";
        public const string COLUMNDATETO = "DateTo";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdACLPerson { get; set; }

        public virtual AccessControlList AccessControlList { get; set; }

        [LwSerialize]
        public virtual Guid GuidAccessControlList { get; set; }

        public virtual Person Person { get; set; }

        public ACLPerson()
        {
            GuidPerson = Guid.Empty;
            GuidAccessControlList = Guid.Empty;
        }

        [LwSerialize]
        public virtual Guid GuidPerson { get; set; }

        [LwSerialize]
        public virtual DateTime? DateFrom { get; set; }

        [LwSerialize]
        public virtual DateTime? DateTo { get; set; }

        public override string ToString()
        {
            return AccessControlList + " - " + Person;
        }

        public override bool Compare(object obj)
        {
            var aclPerson = obj as ACLPerson;

            return 
                aclPerson != null && 
                aclPerson.IdACLPerson == IdACLPerson;
        }

        public virtual void PrepareToSend()
        {
            GuidAccessControlList = 
                AccessControlList != null 
                    ? AccessControlList.IdAccessControlList 
                    : Guid.Empty;

            GuidPerson = 
                Person != null
                    ? Person.IdPerson
                    : Guid.Empty;
        }

        public override string GetIdString()
        {
            return IdACLPerson.ToString();
        }

        public override object GetId()
        {
            return IdACLPerson;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.ACLPerson;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ACLPerson;

            if (other == null)
                return false;

            return 
                IdACLPerson == other.IdACLPerson
                   && DateFrom == other.DateFrom
                   && DateTo == other.DateTo;
        }
    }
}
