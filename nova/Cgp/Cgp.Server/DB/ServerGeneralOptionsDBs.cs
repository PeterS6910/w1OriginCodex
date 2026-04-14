using System;
using System.Linq;

using Contal.Cgp.DBSCreator;
using Contal.Cgp.Globals;
using Contal.Cgp.ORM;
using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Remoting;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Server.DB
{
    public sealed class ServerGeneralOptionsDBs : 
        ABaseOrmTable<ServerGeneralOptionsDBs, ServerGeneralOptionsDB>, 
        IServerGeneralOptionsDBs
    {
        private ServerGeneralOptionsDBs() : base(null)
        {
        }

        public string GetGeneralNtpIpAddress()
        {
            var listGo = List();
            if (listGo != null && listGo.Count > 0)
            {
                var sgodb = listGo.ElementAt(0);
                return sgodb.NtpIpAddresses;
            }
            return String.Empty;
        }

        public bool AddNtpIpAddress(string ipAddresses)
        {
            bool success;
            var listGo = List();
            if (listGo != null && listGo.Count > 0)
            {
                var sgodb = GetObjectForEdit(listGo.ElementAt(0).IdServerGeneralOptionsDB);
                sgodb.NtpIpAddresses = AddIpAddress(ipAddresses, sgodb.NtpIpAddresses);
                success = Update(sgodb);
            }
            else
            {
                var sgodb = new ServerGeneralOptionsDB
                {
                    NtpIpAddresses = ipAddresses
                };
                success = Insert(ref sgodb);
            }
            if (success)
            {
                DbWatcher.Singleton.DbGeneralNtpSettignsChanged();
            }
            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(RunNtpSettingsChanged, DelegateSequenceBlockingMode.Asynchronous, false);
            return success;
        }

        private static string AddIpAddress(string newIpAddresses, string oldIpAddresses)
        {
            var newIpAddressesSplit = newIpAddresses.Split(';');

            foreach (var ipAddress in newIpAddressesSplit)
            {
                if (!oldIpAddresses.Contains(ipAddress))
                {
                    oldIpAddresses += ipAddress + ";";
                }
            }

            return oldIpAddresses;
        }

        public bool SaveNtpSettings(string ipAddresses, int interval)
        {
            bool success;
            var listGo = List();
            if (listGo != null && listGo.Count > 0)
            {
                var sgodb = GetObjectForEdit(listGo.ElementAt(0).IdServerGeneralOptionsDB);
                sgodb.NtpIpAddresses = ipAddresses;
                sgodb.NtpTimeDiffTolerance = interval;
                success = Update(sgodb);
            }
            else
            {
                var sgodb = new ServerGeneralOptionsDB
                {
                    NtpIpAddresses = ipAddresses,
                    NtpTimeDiffTolerance = interval
                };
                success = Insert(ref sgodb);
            }
            if (success)
            {
                DbWatcher.Singleton.DbGeneralNtpSettignsChanged();
            }
            CgpServerRemotingProvider.Singleton.ForeachCallbackHandler(RunNtpSettingsChanged, DelegateSequenceBlockingMode.Asynchronous, false);
            return success;
        }

        public int GetGeneralNtpTimeDiffTolerance()
        {
            var listGo = List();
            if (listGo != null && listGo.Count > 0)
            {
                var sgodb = listGo.ElementAt(0);
                return sgodb.NtpTimeDiffTolerance;
            }
            return -1;
        }

        public override bool HasAccessView(Login login)
        {
            return true;
        }

        public override bool HasAccessInsert(Login login)
        {
            return true;
        }

        public override bool HasAccessUpdate(Login login)
        {
            return true;
        }

        public override bool HasAccessDelete(Login login)
        {
            return true;
        }

        public void RunNtpSettingsChanged(ARemotingCallbackHandler remoteHandler)
        {
            if (remoteHandler is NtpSettingsChangedHandler)
                (remoteHandler as NtpSettingsChangedHandler).RunEvent();
        }

        public bool IsSqlServerRunningOnServerMachine()
        {
            var connectString = ConnectionString.LoadFromRegistry
                (CgpServerGlobals.REGISTRY_CONNECTION_STRING);
            
            if (connectString != null && connectString.IsValid())
                return DatabaseBackupAndRestore.SqlServerRunOnSameComputer(connectString.ToString());

            return false;
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.ServerGeneralOptionsDB; }
        }
    }
}
