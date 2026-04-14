using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client.Common;
using Contal.IwQuick;
using Contal.IwQuick.Threads;

namespace Contal.Cgp.Client.NonVisual
{
    public sealed class CgpClientNonVisual : ACgpClientBase<CgpClientNonVisual, CgpClientPluginManager>
    {
        private readonly ManualResetEvent _loggedAsChanged = new ManualResetEvent(false);
        private readonly ManualResetEvent _shutdownRequested = new ManualResetEvent(false);

        private CgpClientNonVisual() : base(null)
        {
            LoggedAsChanged += OnLoggedAsChanged;
        }

        private void OnLoggedAsChanged(string parameter)
        {
            _loggedAsChanged.Set();
        }

        protected override string Language
        {
            get { return "English"; }
        }

        public override bool GetRequirePINCardLogin
        {
            get { return false; }
        }

        public override string ComPortName
        {
            get
            {
                return string.Empty;
            }
        }

        public override void ClientLoginWithCard(string fullCardNumber)
        {
        }

        public override void ClientInfoWrongPIN()
        {
        }

        public override void LoginCardSwiped(string fullCardNumber)
        {
        }

        public override ExtendedVersion Version
        {
            get
            {
                return
                    new ExtendedVersion(
                        typeof(CgpClientNonVisual),
                        true,
                        null,
                        DevelopmentStage.Testing);

            }
        }

        public bool Launch()
        {
            SafeThread.StartThread(Run);

            if (_loggedAsChanged.WaitOne(TimeSpan.FromSeconds(100.0)))
                if (IsLoggedIn)
                    return true;

            _shutdownRequested.Set();
            return false;
        }

        public void RequestShutdown()
        {
            _shutdownRequested.Set();
        }

        protected override void RunInternal()
        {
            Init();

            _shutdownRequested.WaitOne();
        }

        protected override string PluginPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
        }

        protected override void OnPluginSetChanged(ICgpClientPlugin[] parameter)
        {
        }

        protected override void OnPluginLoaded(ICgpClientPlugin parameter)
        {
        }

        protected override string FriendlyName
        {
            get { return string.Empty; }
        }

        protected override void NotifyProcessAlreadyRunning(Process otherProcess)
        {
        }

        protected override void OnInitRemotingFailed()
        {
        }

        private int _port;
        private string _serverAddress;

        public void SetServerConnectionParams(
            string serverAddress,
            int port)
        {
            _serverAddress = serverAddress;
            _port = port;
        }

        protected override int Port
        {
            get { return _port; }
        }

        protected override string ServerAddress
        {
            get { return _serverAddress; }
        }

        protected override void OnDisconnected()
        {
        }

        protected override void TryPerformClientUpgrade()
        {
        }

        protected override void OnConnected()
        {
        }

        protected override void RequestAuthenticateLoginAsync()
        {
            Exception exception =
                MainServerProvider.ClientAuthenticate(
                    _loginAuthenticationParameters);

            LoggedAs =
                exception == null
                    ? _loginAuthenticationParameters.LoginUsername
                    : null;
        }

        protected override bool PrepareLogout()
        {
            return true;
        }

        protected override void OnLoggedOut()
        {
        }

        private LoginAuthenticationParameters _loginAuthenticationParameters =
            new LoginAuthenticationParameters(
                "admin",
                "admin",
                false);
            

        public void SetLoginAuthentication(string userName, string password)
        {
            _loginAuthenticationParameters = new LoginAuthenticationParameters(
                userName,
                password,
                false);
        }

        protected override void RequestUnlockLogin(ref string loggedAs)
        {
            Exception error;

            if (!MainServerProvider.Logins.CheckLoginPassword(
                _loginAuthenticationParameters.LoginUsername,
                _loginAuthenticationParameters.LoginPasswordHash,
                out error))
            {
                loggedAs = null;
            }
        }
    }
}
