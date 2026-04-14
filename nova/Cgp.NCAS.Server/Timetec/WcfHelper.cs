using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.IdentityModel.Tokens;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Security;

using Contal.IwQuick.Sys;
using Contal.IwQuick.Threads;
using WcfServiceNovaConnection;

namespace Contal.Cgp.NCAS.Server.Timetec
{
    public class WcfHelper
    {
        public class CustomCertificateValidator : X509CertificateValidator
        {
            private readonly string _allowedIssuerName;
            private readonly string _allowedCertThumbrint;

            public CustomCertificateValidator(string allowedIssuer, string allowedThumbprint)
            {
                _allowedIssuerName = allowedIssuer;
                _allowedCertThumbrint = allowedThumbprint;
            }

            public override void Validate(X509Certificate2 certificate)
            {
                // Check that there is a certificate.
                if (certificate == null)
                {
                    throw new ArgumentNullException("certificate");
                }

                if (certificate.IssuerName.Name != _allowedIssuerName)
                {
                    throw new SecurityTokenValidationException("certificate");
                }

                if (certificate.Thumbprint != _allowedCertThumbrint)
                {
                    throw new SecurityTokenValidationException("certificate");
                }
                var nowDate = DateTime.Now;
                var startDateCert = certificate.NotBefore;
                var stopDateCert = certificate.NotAfter;

                if (DateTime.Compare(nowDate, startDateCert) < 0) //Current Date is before starting
                    throw new SecurityTokenValidationException("certificate");

                if (DateTime.Compare(nowDate, stopDateCert) > 0) //Current Date is after stopping
                    throw new SecurityTokenValidationException("certificate");
            }
        }

        private abstract class Executor
        {
            public abstract void Execute(IServiceNovaTimetec channel);
        }

        private abstract class Executor<T>
        {
            public abstract T Execute(IServiceNovaTimetec channel);
        }

        private class TimetecInsertTransitionExecutor : Executor<TransitionAddResult?>
        {
            private readonly TransitionObject _transitionObject;

            public TimetecInsertTransitionExecutor(TransitionObject transitionObject)
            {
                _transitionObject = transitionObject;
            }

            public override TransitionAddResult? Execute(IServiceNovaTimetec channel)
            {
                return channel.TimetecInsertTransition(_transitionObject);
            }
        }

        private class SendObjectChangeExecutor : Executor
        {
            private readonly IEnumerable<ObjectChangeResult> _objectChangeResults;

            public SendObjectChangeExecutor(IEnumerable<ObjectChangeResult> objectChangeResults)
            {
                _objectChangeResults = objectChangeResults;
            }

            public override void Execute(IServiceNovaTimetec channel)
            {
                channel.ProcessChangesResult(_objectChangeResults);
            }
        }

        private DuplexChannelFactory<IServiceNovaTimetec> _channelFactory;
        private readonly NetTcpContextBinding _netTcpContextBinding;
        private IServiceNovaTimetec _channel;
        private bool _isRunning;
        private ITimer _timerCarrier;

        public bool? IsConnected { get; private set; }

        public event Action<bool?> OnlineStateChanged;
        
        public WcfHelper()
        {
            _netTcpContextBinding = new NetTcpContextBinding(SecurityMode.Message)
            {
                MaxBufferSize = Int32.MaxValue,
                MaxReceivedMessageSize = Int32.MaxValue,
                SendTimeout = new TimeSpan(0, 0, 0, 7),
                ReceiveTimeout = new TimeSpan(0, 0, 5, 0),
                OpenTimeout = new TimeSpan(0, 0, 5, 0),
                CloseTimeout = new TimeSpan(0, 0, 5, 0),
                MaxConnections = 10000,
                MaxBufferPoolSize = Int32.MaxValue,
            };

            _netTcpContextBinding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;
            _netTcpContextBinding.ReliableSession.Enabled = true;
        }

        public void StartCommunication(
            string ipAddress,
            int port,
            string userName,
            string password,
            byte[] certificateRawData)
        {
            lock (_netTcpContextBinding)
            {
                if (_isRunning)
                    throw new Exception("The communication is already running");

                var certificate = new X509Certificate2(certificateRawData);

                var identity = EndpointIdentity.CreateX509CertificateIdentity(certificate);
                var endPointAdr = new EndpointAddress(
                    new Uri(string.Format(
                        "net.tcp://{0}:{1}/service",
                        ipAddress,
                        port)),
                    identity);


                var providerCallback = new TimetecProviderCallback();

                var context = new InstanceContext(
                    new ServiceHost(typeof (TimetecProviderCallback)),
                    providerCallback);

                _channelFactory = new DuplexChannelFactory<IServiceNovaTimetec>(
                    context,
                    _netTcpContextBinding,
                    endPointAdr);

                _channelFactory.Credentials.UserName.UserName = userName;
                _channelFactory.Credentials.UserName.Password = password;
                 _channelFactory.Credentials.ClientCertificate.Certificate = certificate;
                _channelFactory.Credentials.ServiceCertificate.DefaultCertificate = certificate;

                _channelFactory.Credentials.ServiceCertificate.Authentication.CertificateValidationMode =
                    X509CertificateValidationMode.Custom;

                _channelFactory.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator =
                   new CustomCertificateValidator(
                       "CN=Contal TtN cert",
                        "5435577EB2308AED57D24BB18441674D8806AECB");

                _isRunning = true;

                CreateChannel();
                _timerCarrier = TimerManager.Static.StartTimer(10000, false, RpcPing);
            }
        }

        private void CreateChannel()
        {
            _channel = _channelFactory.CreateChannel();
            CheckConnection();
        }

        public void StopCommunication()
        {
            lock (_netTcpContextBinding)
            {
                if (!_isRunning)
                    return;

                try
                {
                    _channelFactory.Close();
                }
                catch (Exception error)
                {
                    HandledExceptionAdapter.Examine(error);
                }

                _channelFactory = null;
                _channel = null;

                _timerCarrier.StopTimer();
                _timerCarrier = null;

                _isRunning = false;

                if (IsConnected.HasValue)
                    ExecuteOnlineStateChanged(null);
            }
        }

        private bool CheckConnection()
        {
            if (_channel == null)
                return false;

            try
            {
                _channel.RpcPing();
                if (!IsConnected.HasValue || !IsConnected.Value)
                    ExecuteOnlineStateChanged(true);

                return true;
            }
            catch (Exception error)
            {
                OnException(error);
                return false;
            }
        }

        private void ExecuteOnlineStateChanged(bool? isConnected)
        {
            IsConnected = isConnected;

            if (OnlineStateChanged == null)
                return;

            try
            {
                OnlineStateChanged(isConnected);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void OnException(Exception error)
        {
            if (error is CommunicationException)
            {
                _channel = null;

                if (!IsConnected.HasValue || IsConnected.Value)
                    ExecuteOnlineStateChanged(false);

                return;
            }

            HandledExceptionAdapter.Examine(error);
        }

        private bool RpcPing(ITimer timer)
        {
            lock (_netTcpContextBinding)
            {
                if (!_isRunning)
                    return false;

                if (!CheckConnection())
                    CreateChannel();

                return true;
            }
        }

        public TransitionAddResult? TimetecInsertTransition(TransitionObject transitionObject)
        {
            return RunMethod(new TimetecInsertTransitionExecutor(transitionObject));
        }

        public void SendObjectChangeResults(IEnumerable<ObjectChangeResult> objectChangeResults)
        {
            RunMethod(new SendObjectChangeExecutor(objectChangeResults));
        }

        private T RunMethod<T>(Executor<T> executor)
        {
            lock (_netTcpContextBinding)
            {
                try
                {
                    if (!_isRunning)
                        throw new Exception("The communication is not running");

                    if (_channel == null)
                        return default(T);

                    return executor.Execute(_channel);
                }
                catch (Exception error)
                {
                    OnException(error);
                    return default(T);
                }
            }
        }

        private void RunMethod(Executor executor)
        {
            lock (_netTcpContextBinding)
            {
                try
                {
                    if (!_isRunning)
                        throw new Exception("The communication is not running");

                    if (_channel == null)
                        return;

                    executor.Execute(_channel);
                }
                catch (Exception error)
                {
                    OnException(error);
                }
            }
        }
    }
}
