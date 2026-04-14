using System;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IACLSettings : IBaseOrmTable<ACLSetting>
    {
        bool ExistDirectlyCardReaderAclAssigment(Guid cardReaderId);
    }
}
