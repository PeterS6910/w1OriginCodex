using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// Class for create ssl stream
    /// </summary>
    public class SslLayer : ITransportLayer
    {
        private SslSettings _sslSettings = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sslSettings">Input settings</param>
        public SslLayer(SslSettings sslSettings)
        {
            _sslSettings = sslSettings;
        }

        /// <summary>
        /// Create ssl stream and authenticate as client
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return ssl stream</returns>
        public Stream PrepareClientStream(Socket socket)
        {
            return PrepareClientStream(
                new NetworkStream(
                    socket,
                    false));
        }

        public Stream PrepareClientStream(Stream stream)
        {
            if (_sslSettings.ReverseAuthentication)
            {
                return PrepareServerStream(stream);
            }

            X509Certificate2 aPKI = null;
            if (null != _sslSettings.CertificatePath)
                aPKI = new X509Certificate2(_sslSettings.CertificatePath);

            SslStream aSslStream = new SslStream(
                stream,
                false,
                RemoteCertificateValidation);

            _sslSettings.UsedBySslLayer();

            if (aPKI == null)
            {
                aSslStream.AuthenticateAsClient(
                    _sslSettings.TargetHost,
                    null,
                    _sslSettings.EnabledSslProtocols,
                    _sslSettings.CheckCRL);
            }
            else
            {
                X509CertificateCollection cerCollection = new X509CertificateCollection();
                cerCollection.Add(aPKI);
                aSslStream.AuthenticateAsClient(
                    aPKI.GetNameInfo(
                        X509NameType.SimpleName,
                        false),
                    cerCollection,
                    _sslSettings.EnabledSslProtocols,
                    _sslSettings.CheckCRL);
            }

            _sslSettings.ActualSslProtocols = aSslStream.SslProtocol;

            return aSslStream;
        }

        /// <summary>
        /// If ReverseAutentification is true than authenticate as client, else authenticate as server
        /// </summary>
        /// <param name="socket">Input socket</param>
        /// <returns>Return ssl stream</returns>
        public Stream PrepareServerStream(Socket socket)
        {
            return PrepareServerStream(
                new NetworkStream(
                    socket,
                    false));
        }

        public Stream PrepareServerStream(Stream stream)
        {
            SslStream aSslStream = new SslStream(
                stream,
                false);

            X509Certificate2 aPKI = new X509Certificate2(_sslSettings.CertificatePath);

            _sslSettings.UsedBySslLayer();

            aSslStream.AuthenticateAsServer(
                aPKI,
                _sslSettings.ClientCertificateRequired,
                _sslSettings.EnabledSslProtocols,
                _sslSettings.CheckCRL);

            // if no exception thrown here, register actual SSL protocol

            _sslSettings.ActualSslProtocols = aSslStream.SslProtocol;

            return aSslStream;
        }

        private bool RemoteCertificateValidation(
            object sender,
            X509Certificate certificateToValidate,
            X509Chain certificateChain,
            SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
            {
                if (_sslSettings.AllowCertificateNameMismatch)
                {
                    sslPolicyErrors = SslPolicyErrors.None;
                }
            }

            bool ret = false;

            if (sslPolicyErrors != SslPolicyErrors.None)
            {
                //string strSubReasons = String.Empty;

                bool resolved = false;

                foreach (X509ChainElement aElement in certificateChain.ChainElements)
                {
                    bool chainElementFound = false;
                    foreach (X509ChainStatus aElementStatus in aElement.ChainElementStatus)
                    {
                        //strSubReasons += aElementStatus.Status.ToString() + ",";

                        if (aElementStatus.Status == X509ChainStatusFlags.UntrustedRoot)
                        {
                            ret = _sslSettings.AllowUntrustedRoot;
                            if (ret)
                            {
                                sslPolicyErrors = SslPolicyErrors.None;
                                chainElementFound = true;
                                resolved = true;
                            }
                            break;
                        }
                    }

                    if (!chainElementFound)
                    {
                        ret = _sslSettings.AllowUntrustedRoot;
                        if (ret)
                        {
                            sslPolicyErrors = SslPolicyErrors.None;
                            chainElementFound = true;
                            resolved = true;
                        }
                    }

                    if (resolved)
                        break;

                }
                //_innerSslException = new Exception(strSubReasons);
            }
            else
                ret = true;

            _sslSettings.LastSslPolicyErrors = sslPolicyErrors;
            return ret;
        }

        public void PreConnect(Socket socket)
        {
        }
    }
}
