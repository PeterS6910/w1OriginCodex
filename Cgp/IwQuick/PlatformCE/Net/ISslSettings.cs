namespace Contal.IwQuick.Net
{
    public interface ISslSettings : ITransportSettings
    {
        string CertificatePath { get; }
        bool ClientCertificateRequired { get; }
        bool CheckCRL { get; }
        bool ReverseAuthentication { get; set; }
        bool IsClientSide();
        bool IsServerSide();
        //System.Security.Authentication.SslProtocols SslProtocols { get; }
        bool AllowUntrustedRoot { get; }
        bool AllowCertificateNameMismatch { get; }
        string TargetHost { get; set; }
    }
}
