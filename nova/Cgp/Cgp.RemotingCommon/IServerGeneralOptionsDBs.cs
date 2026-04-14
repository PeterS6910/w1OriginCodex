using System;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;

namespace Contal.Cgp.RemotingCommon
{
    public interface IServerGeneralOptionsDBs
    {
        string GetGeneralNtpIpAddress();
        bool SaveNtpSettings(string ipAddresses, int interval);
        int GetGeneralNtpTimeDiffTolerance();
        bool IsSqlServerRunningOnServerMachine();
    }
}
