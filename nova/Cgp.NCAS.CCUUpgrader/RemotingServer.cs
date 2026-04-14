using System;

using Contal.Drivers.LPC3250;
using Contal.LwRemoting2;
using Contal.IwQuick.Net;
using Contal.IwQuick.Sys;

namespace Cgp.NCAS.CCUUpgrader
{
    class RemotingServer
    {
        private LwRemotingServer _remotingServer = null;
        public void InitRemoting()
        {
            try
            {
                var aesSettings = new AESSettings(
                    WindowsCE.Build >= 3040,
                    LwRemotingGlobals.GetSHA1(LwRemotingGlobals.LWREMOTING_KEY),
                    LwRemotingGlobals.LWREMOTING_SALT, 
                    AESKeySize.Size128);

                //_remotingServer = new LwRemotingServer(aes, 63002, typeof(DB.TimeZone).Assembly);
                _remotingServer = new LwRemotingServer(aesSettings, 63002);
                _remotingServer.RegisterService(CcuCoreRemotingProvider.Singleton);
                _remotingServer.AutheticateFunction += CcuCoreRemotingProvider.AuthenticateClient;
                _remotingServer.ClientDisconnected += ClientDisconnected;
                _remotingServer.ClientConnected += ClientConnected;
                CcuUpgradeHandler.Log.Info("Remoting initialization succeeded");
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                CcuUpgradeHandler.Log.Error("Remoting initialization failed");
            }
        }

        private void ClientDisconnected(ISimpleTcpConnection simpleTcpConnection)
        {
        }

        private void ClientConnected(ISimpleTcpConnection simpleTcpConnection)
        {
        }

        public void Stop()
        {
            _remotingServer.Stop();
        }

    }
}
