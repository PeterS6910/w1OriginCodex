using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Server.DB
{
    public sealed class UserFoldersStructureObjects : 
        ABaseOrmTable<UserFoldersStructureObjects, UserFoldersStructureObject>, 
        IUserFoldersSutructureObjects
    {
        private UserFoldersStructureObjects() : base(null)
        {
        }

        public event Action<Guid, PersonDepartmentChange> EventPersonDepartmentChanged;

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.FOLDERS_SRUCTURE), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.FoldersStructureAdmin), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.FoldersStructureAdmin), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.FoldersStructureAdmin), login);
        }

        public IList<UserFoldersStructure> GetUserFoldersForObject(string objectId, ObjectType objectType)
        {
            IList<UserFoldersStructure> userFolders = new List<UserFoldersStructure>();
            ICollection<UserFoldersStructureObject> list = SelectLinq<UserFoldersStructureObject>(ufso => ufso.ObjectId == objectId && ufso.ObjectType == objectType);
            if (list != null && list.Count > 0)
            {
                foreach (UserFoldersStructureObject userFoldersStructureObject in list)
                {
                    if (userFoldersStructureObject.Folder != null)
                    {
                        UserFoldersStructure userFoldersStructure = UserFoldersStructures.Singleton.GetObjectById(userFoldersStructureObject.Folder.IdUserFoldersStructure);
                        userFolders.Add(userFoldersStructure);
                    }
                }
            }

            return userFolders;
        }

        public void SetPersonDepartment(UserFoldersStructure newDepartment, Guid personId)
        {
            SetPersonDepartment(newDepartment, null, personId, true);
        }

        public void SetPersonDepartment(UserFoldersStructure newDepartment, UserFoldersStructure oldDepartment, Guid personId)
        {
            SetPersonDepartment(newDepartment, oldDepartment, personId, false);
        }
        
        private void SetPersonDepartment(UserFoldersStructure newDepartment, UserFoldersStructure oldDepartment, Guid personId, bool insert)
        {
            try
            {
                if (personId == Guid.Empty)
                    return;

                if (!insert)
                {
                    if (oldDepartment == null &&
                        newDepartment == null)
                    {
                        return;
                    }

                    if (oldDepartment != null &&
                        oldDepartment.Compare(newDepartment))
                    {
                        return;
                    }
                }

                if (oldDepartment != null)
                {
                    ICollection<UserFoldersStructureObject> userFoldersStructureObjectsToRemove =
                        SelectLinq<UserFoldersStructureObject>(
                            ufso =>
                                ufso.ObjectId == personId.ToString() &&
                                ufso.Folder == oldDepartment);

                    if (userFoldersStructureObjectsToRemove != null &&
                        userFoldersStructureObjectsToRemove.Count > 0)
                    {
                        UserFoldersStructureObject ufso = userFoldersStructureObjectsToRemove.ElementAt(0);
                        if (ufso != null)
                        {
                            UserFoldersStructureObject deleteUfSo = GetById(ufso.IdUserFoldersStructureObject);
                            deleteUfSo.Folder = null;
                            Delete(deleteUfSo);
                        }
                    }
                }

                if (newDepartment != null)
                {
                    ICollection<UserFoldersStructureObject> linqResult =
                        SelectLinq<UserFoldersStructureObject>(
                            ufso =>
                                ufso.ObjectId == personId.ToString() &&
                                ufso.Folder == newDepartment);

                    if (linqResult == null || linqResult.Count == 0)
                    {
                        var ufso = new UserFoldersStructureObject
                        {
                            Folder = newDepartment,
                            ObjectId = personId.ToString(),
                            ObjectType = ObjectType.Person
                        };

                        Insert(ref ufso);
                    }
                }

                RunEventPersonDepartmentChanged(personId, new PersonDepartmentChange(oldDepartment, newDepartment));
                StructuredSubSites.Singleton.GlobalEvaluator.UserFoldersStructureWasChanged();
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void RunEventPersonDepartmentChanged(Guid personId, PersonDepartmentChange personDepartmentChange)
        {
            if (EventPersonDepartmentChanged != null)
            {
                EventPersonDepartmentChanged(personId, personDepartmentChange);
            }
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.UserFoldersStructureObject; }
        }

        public void DeleteObject(IdAndObjectType idAndObjectType)
        {
            try
            {
                if (idAndObjectType == null)
                    return;

                if (idAndObjectType.ObjectType == ObjectType.UserFoldersStructureObject
                    || idAndObjectType.ObjectType == ObjectType.UserFoldersStructure)
                {
                    return;
                }

                ICollection<UserFoldersStructureObject> userFoldersStructureObjectsToRemove =
                    SelectLinq<UserFoldersStructureObject>(
                        ufso =>
                            ufso.ObjectId == idAndObjectType.Id.ToString());

                if (userFoldersStructureObjectsToRemove == null)
                    return;

                var userFoldersStructureObjectIdsToRemove = new LinkedList<Guid>
                    (userFoldersStructureObjectsToRemove.Select(
                        ufso =>
                            ufso.IdUserFoldersStructureObject));

                foreach (var userFoldersStructureObjectIdToRemove in userFoldersStructureObjectIdsToRemove)
                {
                    DeleteById(userFoldersStructureObjectIdToRemove);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }
    }
}
