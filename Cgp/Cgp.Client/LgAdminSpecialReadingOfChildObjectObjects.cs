using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client
{
    class LgAdminSpecialReadingOfChildObjectObjects : ISpecialReadingOfChildObjectsForObject
    {
        public event
            Action<
                IdAndObjectType,
                ICollection<ObjectInSiteInfo>,
                ICollection<ObjectInSiteInfo>>
            ChildObjectsChanged;

        public IdAndObjectType ParentObject { get; private set; }
        public ICollection<ObjectInSiteInfo> ChildObjects { get; private set; }

        public LgAdminSpecialReadingOfChildObjectObjects()
        {
            ParentObject = CgpClient.Singleton.MainServerProvider.LoginGroups.GetLoginGroupByName(
                CgpServerGlobals.DEFAULT_ADMIN_LOGIN_GROUP);

            if (ParentObject != null)
            {
                ChildObjects = new HashSet<ObjectInSiteInfo>(
                    CgpClient.Singleton.MainServerProvider.LoginGroups.GetLoginsOfLoginGroup(
                        (Guid) ParentObject.Id));

                CUDObjectHandler.Singleton.Register(
                    CudObjectEvent,
                    ObjectType.Login,
                    ObjectType.LoginGroup);
            }
        }

        private void CudObjectEvent(
            ObjectType objectType,
            object id,
            bool inserted)
        {
            if (CgpClient.Singleton.IsConnectionLost(false))
                return;

            if (ChildObjectsChanged == null)
                return;

            try
            {
                switch (objectType)
                {
                    case ObjectType.Login:
                        var isAssignedToLoginGroupAdmin = CgpClient.Singleton.MainServerProvider.Logins
                            .AssignedToLoginGroup(
                                (Guid) id,
                                (Guid) ParentObject.Id);

                        var loginObjectInfo = new ObjectInSiteInfo(
                            id,
                            objectType,
                            string.Empty,
                            null);

                        if ((!isAssignedToLoginGroupAdmin
                             && !ChildObjects.Contains(loginObjectInfo))
                            || (isAssignedToLoginGroupAdmin
                                && ChildObjects.Contains(loginObjectInfo)))
                        {
                            return;
                        }

                        break;
                    case ObjectType.LoginGroup:
                        if (!id.Equals(ParentObject.Id))
                            return;

                        break;
                }

                var newChildObjects = new HashSet<ObjectInSiteInfo>(
                    CgpClient.Singleton.MainServerProvider.LoginGroups.GetLoginsOfLoginGroup(
                        (Guid) ParentObject.Id));

                var addedObjects = new LinkedList<ObjectInSiteInfo>(
                    newChildObjects.Where(
                        newChildObject =>
                            !ChildObjects.Contains(newChildObject)));

                var removedObjects = new LinkedList<ObjectInSiteInfo>(
                    ChildObjects.Where(
                        oldChildObject =>
                            !newChildObjects.Contains(oldChildObject)));

                if (addedObjects.Count == 0
                    && removedObjects.Count == 0)
                {
                    return;
                }

                ChildObjects = newChildObjects;
                ChildObjectsChanged(ParentObject, addedObjects, removedObjects);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void Close()
        {
            CUDObjectHandler.Singleton.Unregister(
                CudObjectEvent,
                ObjectType.Login,
                ObjectType.LoginGroup);

            ChildObjectsChanged = null;
        }
    }
}
