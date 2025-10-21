using System;
//using System.Security.Authentication;
using System.IO;
using Contal.IwQuick;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// default implementation of the SSL settings encapsulation class
    /// </summary>
    public class SslSettings : ISslSettings
    {
        private bool _serverSide = true;
        public bool IsServerSide()
        {
            return _serverSide;
        }

        public bool IsClientSide()
        {
            return !_serverSide;
        }

        private bool _reverseAuthentication = true;
        public bool ReverseAuthentication
        {
            get
            {
                return _reverseAuthentication;
            }
            set
            {
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
        }

        private bool _checkCRL = false;
        public bool CheckCRL
        {
            get { return _checkCRL; }
        }

        /*
        private SslProtocols _protocols = SslProtocols.Default;
        public SslProtocols SslProtocols
        {
            get { return _protocols; }
        }*/

        private bool _allowUntrustedRoot = false;
        public bool AllowUntrustedRoot
        {
            get { return _allowUntrustedRoot; }
        }

        private bool _allowCertificateNameMismatch = true;
        public bool AllowCertificateNameMismatch
        {
            get { return _allowCertificateNameMismatch; }
        }

        private bool _allowExpiredOrIneffectiveCertificate = false;
        public bool AllowExpiredOrIneffectiveCertificate
        {
            get { return _allowExpiredOrIneffectiveCertificate; }
        }

        private string _targetHost = String.Empty;
        public string TargetHost
        {
            get { return _targetHost; }
            set
            {
                if (null == value)
                    _targetHost = String.Empty;
                else
                    _targetHost = value;

                _allowCertificateNameMismatch = (_targetHost.Length == 0);
            }
        }

        public SslSettings(
            string serverCertificatePath,
            //SslProtocols protocols, 
            bool clientCertificateRequired,
            bool checkCRL,
            bool allowUntrustedRoot)
        {
            Validator.CheckFileExists(serverCertificatePath);

            if (!File.Exists(serverCertificatePath))
                throw new DoesNotExistException(serverCertificatePath, "Certificate \"" + serverCertificatePath + "\" does not exist");

            //Validator.CheckInvalidOperation(SslProtocols.None == protocols);

            _serverSide = true;
            _certificatePath = serverCertificatePath;
            _clientCertificateRequired = clientCertificateRequired;
            //_protocols = protocols;
            _checkCRL = checkCRL;
            _allowUntrustedRoot = allowUntrustedRoot;
        }

        /*
        public SslSettings(
            string i_strServerCertificatePath,
            bool i_bClientCertificateRequired,
            bool i_bCheckCRL,
            bool i_bAllowUntrustedRoot)
            :this(i_strServerCertificatePath,SslProtocols.Default,i_bClientCertificateRequired,i_bCheckCRL,i_bAllowUntrustedRoot)
        {
        }*/

        public SslSettings(
            string clientCertificatePath,
            //SslProtocols protocols,
            bool i_bTrustSelfSignedCertificates)
        {
            if (null != clientCertificatePath)
            {
                Validator.CheckFileExists(clientCertificatePath);
                _certificatePath = clientCertificatePath;
                _allowCertificateNameMismatch = false;

            }

            //Validator.CheckInvalidOperation(SslProtocols.None == protocols);

            _serverSide = false;
            //_protocols = protocols;
            _allowUntrustedRoot = i_bTrustSelfSignedCertificates;
        }

        public SslSettings(string clientCertificatePath,
            //SslProtocols protocols,
            bool i_bTrustSelfSignedCertificates,
            bool allowCertificateNameMismatch,
            bool allowExpiredOrIneffectiveCertificate,
            bool allowUntrustedRoot)
        {
            if (null != clientCertificatePath)
            {
                Validator.CheckFileExists(clientCertificatePath);
                _certificatePath = clientCertificatePath;
                _allowCertificateNameMismatch = false;

            }

            //Validator.CheckInvalidOperation(SslProtocols.None == protocols);

            _serverSide = false;
            //_protocols = protocols;
            _allowUntrustedRoot = i_bTrustSelfSignedCertificates;

            _allowCertificateNameMismatch = allowCertificateNameMismatch;
            _allowExpiredOrIneffectiveCertificate = allowExpiredOrIneffectiveCertificate;
            _allowUntrustedRoot = allowUntrustedRoot;
        }

        /*
        public SslSettings(
            string clientCertificatePath,
            bool i_bAllowUntrustedRoot)
            : this(clientCertificatePath, SslProtocols.Default, i_bAllowUntrustedRoot)
        {
        }

        public SslSettings(
            SslProtocols protocols,
            bool i_bAllowUntrustedRoot)
            : this(null, protocols, i_bAllowUntrustedRoot)
        {
        }

        public SslSettings(
            bool i_bAllowUntrustedRoot)
            : this(null, SslProtocols.Default, i_bAllowUntrustedRoot)
        {
        }*/


        private SslLayer _sslLayer = null;
        /// <summary>
        /// Get SslLayer
        /// </summary>
        /// <returns>Return new SslLayer</returns>
        public ITransportLayer GetLayer()
        {
            if (null == _sslLayer)
                _sslLayer = new SslLayer(this);

            return _sslLayer;
        }
    }
}
