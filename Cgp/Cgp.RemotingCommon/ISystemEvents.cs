using System;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.RemotingCommon
{
    public interface ISystemEvents : IBaseOrmTable<SystemEvent>
    {
        SystemEvent GetByName(string name, out Exception error);
        bool ExistSystemEvent(string eventName);
        System.Collections.Generic.ICollection<PresentationGroup> GetSystemEventOwnPresentationGroup(SystemEvent sysEvent, out Exception error);
        bool AddPresentationGroup(SystemEvent systemEvent, PresentationGroup presentationGroup);
        bool RemovePresentationGroup(SystemEvent systemEvent, PresentationGroup presentationGroup);
        void RefreshSystemEvent();
    }
}
