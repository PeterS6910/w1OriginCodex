using System;

using Contal.Cgp.Server.Beans;
using System.Collections.Generic;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface IUserFoldersSutructures : IBaseOrmTable<UserFoldersStructure>
    {
        bool HasSubFolders(UserFoldersStructure userFoldersStructure);
        IList<UserFoldersStructure> GetUserFoldersForObject(string objectId, ObjectType objectType);
        string GetFullPath(UserFoldersStructure userFoldersStructure);
        //bool RemoveDepartment(UserFoldersStructure userFoldersStructure, out Exception error);
        //void OnDeleteUfsObj(UserFoldersStructureObject deletedObj);
        ICollection<AOrmObject> ListDepartments(string rootName, string separator, object personId, out Exception error);
        ICollection<AOrmObject> ListDepartmentsBySubSite(int subSiteId, out Exception error);
        string GetFullDepartmentName(string strDepartmentId, string folderName, string separator, string rootName);
        ICollection<ObjectInSiteInfo> LoadFolderStructureObjects(object folderStructureId);
    }
}
