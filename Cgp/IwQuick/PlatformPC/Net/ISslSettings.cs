using System;
namespace Contal.IwQuick.Net
{
    /// <summary>
    /// implicit settings for SSL TCP client and server
    /// </summary>
    public interface ISslSettings : ITransportSettings
    {
        /// <summary>
        /// physical location of the certificate
        /// </summary>
        string CertificatePath { get; }

        /// <summary>
        /// if true, the client certificate verification will be applied on server side
        /// </summary>
        bool ClientCertificateRequired { get; }

        /// <summary>
        /// if true, certificates revocation list will be verified on the server
        /// </summary>
        bool CheckCRL { get; }

        bool ReverseAuthentication { get; set; }

        /// <summary>
        /// returns if the SSL settings are related to TCP client side
        /// </summary>
        bool IsClientSide();

        /// <summary>
        /// returns if the SSL settings are related to TCP server side
        /// </summary>
        bool IsServerSide();

        /// <summary>
        /// SSL protocol filter
        /// </summary>
        System.Security.Authentication.SslProtocols EnabledSslProtocols { get; }

        /// <summary>
        /// if true, even self-signed certificates are accepted on either client or server side
        /// </summary>
        bool AllowUntrustedRoot { get; }

        bool AllowCertificateNameMismatch { get; }

        string TargetHost { get; set; }
    }
}
