using System;
using Contal.IwQuick.Threads;

namespace Contal.IwQuick.Remoting
{
    public interface IRemotingService : IDisposable
    {
        void Authenticate(ARemotingAuthenticationParameters parameters);
        void Unauthenticate();

        event DVoid2Void ServiceDisposed;

        bool IsSessionValid { get; }

        //void AssignEvent(System.Reflection.MethodInfo delegateToAssign);
        bool AttachCallbackHandler(ARemotingCallbackHandler remoteHandler);
        bool DetachCallbackHandler(ARemotingCallbackHandler remoteHandler);
        void ForeachCallbackHandler(DRemotingCallback localDelegate, DelegateSequenceBlockingMode blockingMode, bool forceVerifyHandlers, params Type[] types);
        void ForeachCallbackHandler(DRemotingCallback callback, bool blocking);
        void ForeachCallbackHandler(DRemotingCallback callback);
    }
}
