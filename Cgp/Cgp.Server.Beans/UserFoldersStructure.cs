using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class UserFoldersStructure : AOrmObject
    {
        public const string COLUMN_ID_USER_FOLDERS_STRUCTURE = "IdUserFoldersStructure";
        public const string COLUMNPARENTFOLDER = "ParentFolder";
        public const string ColumnFolderName = "FolderName";

        public virtual Guid IdUserFoldersStructure { get; set; }
        public virtual UserFoldersStructure ParentFolder { get; set; }
        public virtual string FolderName { get; set; }
        public virtual ICollection<UserFoldersStructureObject> UserFoldersStructureObjects { get; set; }

        private string _fullFolderName;

        public override string ToString()
        {
            if (string.IsNullOrEmpty(_fullFolderName))
                return FolderName;

            return _fullFolderName;
        }

        public override bool Compare(object obj)
        {
            var userFoldersStructure = obj as UserFoldersStructure;

            return userFoldersStructure != null &&
                   userFoldersStructure.IdUserFoldersStructure == IdUserFoldersStructure;
        }

        public override bool Contains(string expression)
        {
            return FolderName.ToLower().Contains(expression.ToLower());
        }

        public override string GetIdString()
        {
            return IdUserFoldersStructure.ToString();
        }

        public override object GetId()
        {
            return IdUserFoldersStructure;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.UserFoldersStructure;
        }

        public virtual void SetFullFolderName(string fullFolderName)
        {
            _fullFolderName = fullFolderName;
        }
    }
}
