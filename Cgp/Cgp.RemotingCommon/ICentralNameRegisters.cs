using System;

using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;

namespace Contal.Cgp.RemotingCommon
{
    public interface ICentralNameRegisters : IBaseOrmTable<CentralNameRegister>
    {
        ObjectType GetObjectTypeFromGuid(Guid guid);
        Guid GetGuidFromName(string name);
    }
}
