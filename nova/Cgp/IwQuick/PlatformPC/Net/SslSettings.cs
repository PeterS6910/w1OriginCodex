using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Authentication;
using System.IO;
using System.Net.Security;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// default implementation of the SSL settings encapsulation class
    /// </summary>
    public class SslSettings : ISslSettings
    {
        private bool _used = false;
        protected internal void UsedBySslLayer()
        {
            _used = true;
        }

            

        private bool _serverSide = true;
        public bool IsServerSide()
        {
            return _serverSide;
        }

        public bool IsClientSide()
        {
            return !_serverSide;
        }

        private bool _reverseAuthentication = false;
        public bool ReverseAuthentication
        {
            get
            {
                return _reverseAuthentication;
            }
            set
            {
                if (_used)
                    throw new InvalidOperationException("SSL settings are already used by SSL layer, therefore they cannot be modified this way");

                _reverseAuthentication = value;
            }
        }

        private string _certificatePath = null;
        public string CertificatePath
        {
            get { return _certificatePath; }
        }

        private bool _clientCertificateRequired = false;
        public bool ClientCertificateRequired
        {
            get { return _clientCertificateRequired; }
            set
            {
                if (_used)
                    throw new InvalidOperationException("SSL settings are already used by SSL layer, therefore they cannot be modified this way");

                _clientCertificateRequired = value;
            }
        }

        private bool _checkCRL = false;
        public bool CheckCRL
        {
            get { return _checkCRL; }
            set
            {
                if (_used)
                    throw new InvalidOperationException("SSL settings are already used by SSL layer, therefore they cannot be modified this way");

                _checkCRL = value;
            }
        }

        private SslProtocols _enabledSslProtocols = SslProtocols.Default;
        public SslProtocols EnabledSslProtocols
        {
            get { return _enabledSslProtocols; }
            set
            {
                if (_used)
                    throw new InvalidOperationException("SSL settings are already used by SSL layer, therefore they cannot be modified this way");
                _enabledSslProtocols = value;
            }
        }

        private SslProtocols _actualSslProtocols = SslProtocols.None;
        public SslProtocols ActualSslProtocols
        {
            get
            {
                return _actualSslProtocols;
            }
            protected internal set
            {
                _actualSslProtocols = value;
            }
        }

        private bool _allowUntrustedRoot = false;
        public bool AllowUntrustedRoot
        {
            get { return _allowUntrustedRoot; }
            set
            {
                if (_used)
                    throw new InvalidOperationException("SSL settings are already used by SSL layer, therefore they cannot be modified this way");
                _allowUntrustedRoot = value;
            }
        }

        private bool _allowCertificateNameMismatch = true;
        public bool AllowCertificateNameMismatch
        {
            get { return _allowCertificateNameMismatch; }
            set
            {
                if (_used)
                    throw new InvalidOperationException("SSL settings are already used by SSL layer, therefore they cannot be modified this way");
                _allowCertificateNameMismatch = value;
            }
        }

        private string _targetHost = String.Empty;
        public string TargetHost
        {
            get { return _targetHost; }
            set
            {
                _targetHost = value;
                if (_targetHost != String.Empty)
                    _allowCertificateNameMismatch = false;
            }
        }

        private SslPolicyErrors _lastSslPolicyErrors = SslPolicyErrors.None;
        public SslPolicyErrors LastSslPolicyErrors
        {
            get
            {
                return _lastSslPolicyErrors;
            }
            protected internal set
            {
                _lastSslPolicyErrors = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverCertificatePath"></param>
        /// <param name="enabledSslProtocols"></param>
        /// <param name="clientCertificateRequired"></param>
        /// <param name="checkCRL"></param>
        /// <param name="allowUntrustedRoot"></param>
        public SslSettings(
            string serverCertificatePath, 
            SslProtocols enabledSslProtocols, 
            bool clientCertificateRequired, 
            bool checkCRL,
            bool allowUntrustedRoot)
        {
            Validator.CheckFileExists(serverCertificatePath);

            if (!File.Exists(serverCertificatePath))
                throw new DoesNotExistException(serverCertificatePath,"Certificate \"" + serverCertificatePath + "\" does not exist");

            Validator.CheckInvalidOperation(SslProtocols.None == enabledSslProtocols);

            _serverSide = true;
            _certificatePath = serverCertificatePath;
            _clientCertificateRequired = clientCertificateRequired;
            _enabledSslProtocols = enabledSslProtocols;
            _checkCRL = checkCRL;
            _allowUntrustedRoot = allowUntrustedRoot;
        }

        public SslSettings(
            string serverCertificatePath,
            bool clientCertificateRequired,
            bool checkCRL,
            bool allowUntrustedRoot)
            :this(serverCertificatePath,SslProtocols.Default,clientCertificateRequired,checkCRL,allowUntrustedRoot)
        {
        }

        public SslSettings(
            string clientCertificatePath,
            SslProtocols enabledSslProtocols,
            bool allowUntrustedRoot)
        {
            if (null != clientCertificatePath)
            {
                Validator.CheckFileExists(clientCertificatePath);
                _certificatePath = clientCertificatePath;
                _allowCertificateNameMismatch = false;

            }
            
            Validator.CheckInvalidOperation(SslProtocols.None == enabledSslProtocols);

            _serverSide = false;
            _enabledSslProtocols = enabledSslProtocols;
            _allowUntrustedRoot = allowUntrustedRoot;
        }

        public SslSettings(
            string clientCertificatePath,
            bool allowUntrustedRoot)
            : this(clientCertificatePath, SslProtocols.Default, allowUntrustedRoot)
        {
        }

        public SslSettings(
            SslProtocols enabledSslProtocols,
            bool allowUntrustedRoot)
            : this(null, enabledSslProtocols, allowUntrustedRoot)
        {
        }

        public SslSettings(
            bool allowUntrustedRoot)
            : this(null, SslProtocols.Default, allowUntrustedRoot)
        {
        }

        /// <summary>
        /// Get SslLayer
        /// </summary>
        /// <returns>Return new SslLayer</returns>
        public ITransportLayer GetLayer()
        {
            return new SslLayer(this);
        }
    }
}
