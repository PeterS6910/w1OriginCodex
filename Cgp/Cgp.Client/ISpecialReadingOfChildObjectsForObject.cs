using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.RemotingCommon;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public interface ISpecialReadingOfChildObjectsForObject
    {
        IdAndObjectType ParentObject { get; }
        ICollection<ObjectInSiteInfo> ChildObjects { get; }

        event
            Action<
                IdAndObjectType,
                ICollection<ObjectInSiteInfo>,
                ICollection<ObjectInSiteInfo>>
            ChildObjectsChanged;

        void Close();
    }
}
