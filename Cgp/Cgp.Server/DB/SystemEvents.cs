using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public sealed class SystemEvents : 
        ABaseOrmTable<SystemEvents, SystemEvent>, 
        ISystemEvents
    {
        private const string EVENT_NAME = "Name";

        private SystemEvents() : base(null)
        {
        }

        protected override void LoadObjectsInRelationship(SystemEvent obj)
        {
            if (obj.PresentationGroups != null)
            {
                IList<PresentationGroup> list = new List<PresentationGroup>();

                foreach (PresentationGroup presentationGroup in obj.PresentationGroups)
                {
                    list.Add(PresentationGroups.Singleton.GetById(presentationGroup.IdGroup));
                }

                obj.PresentationGroups.Clear();
                foreach (PresentationGroup presentationGroup in list)
                    obj.PresentationGroups.Add(presentationGroup);
            }
        }

        #region ISystemEvents Members

        public SystemEvent GetByName(string name, out Exception error)
        {
            error = null;
            //CheckAccess accessControl = GetCheckAccessControl();
            //if (accessControl == null || !accessControl.View)
            //{
            //    error = new Contal.IwQuick.AccessDeniedException();
            //    return null;
            //}

            return GetByName(name);
        }

        public SystemEvent GetByName(string name)
        {
            ICollection<SystemEvent> ret = SelectLinq<SystemEvent>(se => se.Name == name);

            if (null == ret || ret.Count != 1)
                return null;
            return ret.ElementAt(0);
        }

        public bool ExistSystemEvent(string eventName)
        {
            ICollection<SystemEvent> ret = base.SelectLinq<SystemEvent>(se => se.Name == eventName);

            if (null == ret || ret.Count == 0)
                return false;
            return true;
        }

        public ICollection<PresentationGroup> GetSystemEventOwnPresentationGroup(SystemEvent sysEvent, out Exception error)
        {
            error = null;
            try
            {
                //CheckAccess accessControl = PresentationGroups.Singleton.GetCheckAccessControl();
                //if (accessControl == null || !accessControl.View)
                //{
                //    error = new Contal.IwQuick.AccessDeniedException();
                //    return null;
                //}

                sysEvent = GetById(sysEvent.IdSystemEvent);
                if (sysEvent.PresentationGroups.Count > 0)
                {
                    ICollection<PresentationGroup> colPg = new List<PresentationGroup>();

                    foreach (PresentationGroup pg in sysEvent.PresentationGroups)
                    {
                        PresentationGroup newPg = PresentationGroups.Singleton.GetById(pg.IdGroup);
                        colPg.Add(newPg);
                    }

                    return colPg;
                }
            }
            catch
            {
                
            }
            return null;
        }

        public bool AddPresentationGroup(SystemEvent systemEvent,PresentationGroup presentationGroup)
        {
            try
            {
                PresentationGroup editPresGroup = PresentationGroups.Singleton.GetById(presentationGroup.IdGroup);
                SystemEvent editSystemEvent = GetById(systemEvent.IdSystemEvent);
                editSystemEvent.PresentationGroups.Add(editPresGroup);
                return base.Merge(editSystemEvent);
            }
            catch
            {
                return false;
            }
        }

        public bool RemovePresentationGroup(SystemEvent systemEvent, PresentationGroup presentationGroup)
        {
            try
            {
                PresentationGroup editPresGroup = PresentationGroups.Singleton.GetById(presentationGroup.IdGroup);
                SystemEvent editSystemEvent = GetById(systemEvent.IdSystemEvent);

                for (int i = 0; i < editSystemEvent.PresentationGroups.Count; i++)
                {
                    if (editPresGroup.IdGroup == editSystemEvent.PresentationGroups.ElementAt<PresentationGroup>(i).IdGroup)
                    {
                        editSystemEvent.PresentationGroups.Remove(editSystemEvent.PresentationGroups.ElementAt<PresentationGroup>(i));
                        return base.Merge(editSystemEvent);
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public void RefreshSystemEvent()
        {
            PerformPresentationProcessor.Singleton.CreateStore();
        }

        #endregion

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.SYSTEM_EVENTS), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.SystemEventsAdmin), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.SystemEventsAdmin), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.SystemEventsAdmin), login);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.SystemEvent; }
        }
    }
}
