using System;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class UserFoldersStructureObject : AOrmObject
    {
        public const string ColumnFolder = "Folder";
        public const string ColumnObjectType = "ObjectType";
        public const string ColumnObjectId = "ObjectId";

        public virtual Guid IdUserFoldersStructureObject { get; set; }
        public virtual UserFoldersStructure Folder { get; set; }
        public virtual ObjectType ObjectType { get; set; }
        public virtual string ObjectId { get; set; }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            var userFoldersStructureObject = obj as UserFoldersStructureObject;

            return userFoldersStructureObject != null &&
                   userFoldersStructureObject.IdUserFoldersStructureObject ==
                   IdUserFoldersStructureObject;
        }

        public override bool Contains(string expression)
        {
            return false;
        }

        public override string GetIdString()
        {
            return IdUserFoldersStructureObject.ToString();
        }

        public override object GetId()
        {
            return IdUserFoldersStructureObject;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.UserFoldersStructureObject;
        }
    }
}
