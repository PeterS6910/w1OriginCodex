using System;


using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICustomAccessControls
    {
        System.Collections.Generic.ICollection<CustomAccessControl> List(out Exception error);
    }
}
