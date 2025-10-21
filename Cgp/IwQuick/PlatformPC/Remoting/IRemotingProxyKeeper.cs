using System;
namespace Contal.IwQuick.Remoting
{
    public interface IRemotingProxyKeeper
    {
        ARemotingPeer RemotingPeer { get; set; }
        void ReportProblem(Exception error);
        void AllowProxyRegaining();
        void Start(int retryDelay);
        void Start();
        void Stop();
        void Restart();
    }
}
