using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Threading;
using Contal.IwQuick.Threads;
using JetBrains.Annotations;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// abstract class for remoted objects combined with remoted client callback handling
    /// </summary>
    public abstract class ARemotingService : MarshalByRefObject, IRemotingService
    {
        private readonly RemotingSessionHandler _sessionHandler = 
            RemotingSessionHandler.Singleton;

        /// <summary>
        /// implicitly disallowes lifetime GC mechanism over remoted objects
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }

        private readonly RemoteHandlerStore<string> _remoteCallbackHandlers =
            new RemoteHandlerStore<string>();

        private IRemotingAuthentication<Guid> _remotingAuthentication;
        public IRemotingAuthentication<Guid> RemotingAuthentication
        {
            get { return _remotingAuthentication; }
        }

        public void SetRemotingAuthentication(IRemotingAuthentication<Guid> authentication)
        {
            
                // remoting authentication can be changed to another only during 
                if (null != _remotingAuthentication)
                    Validator.CheckForNull(authentication,"authentication");
                
                _remotingAuthentication = authentication;
            
        }

        /// <summary>
        /// returns type of the authentication
        /// </summary>
        public bool AuthenticationNeeded
        {
            get { return null == _remotingAuthentication; }
        }

        /// <summary>
        /// tries to aunthenticate the remoting session;
        /// if it does not throw an exception, the authentication succeeded
        /// </summary>
        /// <param name="parameters">the parameters passed to the authentication's verification process</param>
        /// <exception cref="InvalidOperationException">if trying to authenticate session from the server side</exception>
        public virtual void Authenticate([NotNull] ARemotingAuthenticationParameters parameters)
        {
            if (_remotingAuthentication == null)
                return;

            Validator.CheckForNull(parameters,"parameters");
            RemotingSession rs = Session;

            Validator.CheckInvalidOperation(rs == null);

            _remotingAuthentication.Authenticate(Session,parameters);
            rs[RemotingSession.SESSIONPARAMETERS] = parameters;
        }

        public void Unauthenticate()
        {
            if (_remotingAuthentication == null)
                return;

            _remotingAuthentication.Logout(Session);
        }

        /// <summary>
        /// Reset session for user which is defined in parameters
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public bool ResetUserSession(ARemotingAuthenticationParameters parameters)
        {
            if (_remotingAuthentication == null)
                return false;

            var sessionId = _remotingAuthentication.GetSessionId(parameters);

            if (sessionId == null)
                return false;

            var session = _sessionHandler[sessionId];

            if (session == null)
                return false;

            _remotingAuthentication.Logout(session);
            return true;
        }

        public void UnauthenticateSessionTimeOut(object sessionIdentification)
        {
            if (_remotingAuthentication == null)
                return;

            _remotingAuthentication.LogoutSessionTimeOut(sessionIdentification);
        }

        protected string SessionId
        {
            get
            {
                try
                {
                    return ClientIdentificationServerSink.CallingSessionId;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// returns current calling session
        /// </summary>
        protected RemotingSession Session
        {
            get
            {
                try
                {
                    string sessionId = ClientIdentificationServerSink.CallingSessionId;
                    if (sessionId == null)
                        return null;

                    RemotingSession rs =
                        _sessionHandler[sessionId];

                    return rs;
                }
                catch
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// can be used to validate the session's authenticate flag in exported service's methods
        /// </summary>
        /// <exception cref="InvalidOperationException">if trying to execute on the server side, not by the remoting call</exception>
        protected void ValidateSession()
        {
            if (_remotingAuthentication == null)
                return;

            RemotingSession rs = Session;

            if (!rs.Authenticated)
                throw new SessionNotAuthenticatedException();
        }

        /// <summary>
        /// returns true, if the calling session is authenticated, otherwise false
        /// </summary>
        /// <exception cref="InvalidOperationException">if trying to execute on the server side, not by the remoting call</exception>
        public bool IsSessionValid
        {
            get
            {
                if (_remotingAuthentication == null)
                    return true;

                RemotingSession rs = Session;

                if (ReferenceEquals(rs,null))
                    throw new InvalidOperationException();
                
                return rs.Authenticated;
            }
        }

        /// <summary>
        /// Get all sessions for current user
        /// </summary>
        /// <returns></returns>
        protected abstract IEnumerable<string> GetSessionsForUser(string userIdentifier);

        //private readonly  object _remoteHandlerRegistrationSync = new object();

        /// <summary>
        /// attaches client callback handler to the remoting service implementation
        /// </summary>
        /// <param name="remoteHandler"></param>
        /// <param name="remoteId"></param>
        /// <returns></returns>
        public bool AttachCallbackHandler(ARemotingCallbackHandler remoteHandler)
        {
            if (null == remoteHandler)
                return false;

            try
            {
                if (!_remoteCallbackHandlers.AddHandler(remoteHandler, SessionId))
                    return true;

                //TODO uncomment if a problem occurs with the registration of call back handlers on the client
                //lock (_remoteHandlerRegistrationSync)
                {
                    remoteHandler.RegisterAttachedTo(this);

#if DEBUG
                    string handlerUri = RemotingServices.GetObjectUri(remoteHandler);
                    Debug.Print("Attaching : " + handlerUri);
#endif
                }

                return true;

            }
            catch
            {
                return false;
            }
        }

        private bool RemoveCallbackHandler(string handlerUri)
        {
            if (Validator.IsNullString(handlerUri))
                return false;

            return _remoteCallbackHandlers.RemoveCallbackHandlerByUri(handlerUri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteHandler"></param>
        /// <returns></returns>
        public bool DetachCallbackHandler(ARemotingCallbackHandler remoteHandler)
        {
            if (null == remoteHandler)
                return false;

            string handlerUri;
            try
            {
                handlerUri = RemotingServices.GetObjectUri(remoteHandler);
            }
            catch
            {
                return false;
            }

            return RemoveCallbackHandler(handlerUri);
        }

        private IEnumerable<ARemotingCallbackHandler> EnsuredActiveHandlers(bool forceVerifyHandlers, params Type[] handlerTypes)
        {
            lock (_remoteCallbackHandlers.SyncObj)
            {

                if (!forceVerifyHandlers)
                {
                    if (handlerTypes == null || handlerTypes.Length == 0)
                        return _remoteCallbackHandlers.GetAllHandlersSnapshot();
                    else
                        return handlerTypes
                            .Where( type => type != null)
                            .SelectMany(type => _remoteCallbackHandlers.GetAllHandlersSnapshot(type));
                }
                else
                {
                    ICollection<string> removeList = new LinkedList<string>();

                    ICollection<ARemotingCallbackHandler> tempList = new LinkedList<ARemotingCallbackHandler>();

                    HashSet<Type> requestedTypes = null;

                    if (handlerTypes != null
                        && handlerTypes.Length > 0)
                        requestedTypes = new HashSet<Type>(handlerTypes);

                    _remoteCallbackHandlers.ForEachHandler(true, (handlerUri, type, handler) =>
                        {
                            if (null == handlerUri
                                || null == handler
                                || (null != requestedTypes 
                                    && !requestedTypes.Contains(type)))
                                return;

                            try
                            {
                                // intentional call
                                string tmp = handler.Name;
                                DebugHelper.Keep(tmp);

                                tempList.Add(handler);
                            }
                            catch (RemotingException)
                            {
                                removeList.Add(handlerUri);
                            }
                            catch (SocketException)
                            {
                                removeList.Add(handlerUri);
                            }
                            catch
                            {
                            }
                        });

                    foreach (string handlerUri in removeList)
                        RemoveCallbackHandler(handlerUri);

                    return tempList;
                }
            }
        }

        public void ForeachCallbackHandler(DRemotingCallback callback)
        {
            ForeachCallbackHandler(callback, DelegateSequenceBlockingMode.Asynchronous, false);
        }

        public void ForeachCallbackHandler(DRemotingCallback callback, bool blocking)
        {
            ForeachCallbackHandler(callback, DelegateSequenceBlockingMode.Asynchronous, false);
        }

        private class RemotingCallbackCarrier
        {
            private readonly DRemotingCallback _localDelegate;
            public DRemotingCallback LocalDelegate
            {
                get { return _localDelegate; }
            }

            private readonly DRemotingCallbackWithObjects _localDelegateWithObjects;
            public DRemotingCallbackWithObjects LocalDelegateWithObjects
            {
                get { return _localDelegateWithObjects; }
            }

            private readonly ARemotingCallbackHandler _remoteCallbackHandler;
            public ARemotingCallbackHandler RemoteCallbackHandler
            {
                get { return _remoteCallbackHandler; }
            }

            public RemotingCallbackCarrier([NotNull] DRemotingCallback localDelegate, [NotNull] ARemotingCallbackHandler handler)
            {
                Validator.CheckForNull(localDelegate, "localDelegate");
                Validator.CheckForNull(handler, "handler");

                _localDelegate = localDelegate;
                _remoteCallbackHandler = handler;
            }

            public RemotingCallbackCarrier([NotNull] DRemotingCallbackWithObjects localDelegate, [NotNull] ARemotingCallbackHandler handler)
            {
                Validator.CheckForNull(localDelegate, "localDelegate");
                Validator.CheckForNull(handler, "handler");

                _localDelegateWithObjects = localDelegate;
                _remoteCallbackHandler = handler;
            }
        }

        /// <summary>
        /// Call single callback for identifier get by virtual method GetCurrentUserId()
        /// </summary>
        /// <param name="userIdentifier">user identifier</param>
        /// <param name="typeOfHandler">type of handler. It has to be derived fom ARemotingCallbackHandler</param>
        /// <param name="localDelegate"></param>
        /// <param name="blockingMode"></param>
        public void SingleCallback(
            [NotNull] string userIdentifier,
            [NotNull] Type typeOfHandler,
            [NotNull] DRemotingCallback localDelegate,
            DelegateSequenceBlockingMode blockingMode)
        {
            Validator.CheckForNull(userIdentifier, "userIdentifier");
            Validator.CheckForNull(typeOfHandler, "typeOfHandler");
            Validator.CheckBaseType(typeOfHandler, typeof(ARemotingCallbackHandler));

            foreach(var session in GetSessionsForUser(userIdentifier))
            {
                ARemotingCallbackHandler handler = _remoteCallbackHandlers.GetHandler(
                    typeOfHandler, 
                    session);

                if (handler != null)
                    SingleCallback(handler, localDelegate, blockingMode);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteHandler"></param>
        /// <param name="localDelegate"></param>
        /// <param name="blockingMode"></param>
        public void SingleCallback(
            [NotNull] ARemotingCallbackHandler remoteHandler,
            [NotNull] DRemotingCallback localDelegate, 
            DelegateSequenceBlockingMode blockingMode)
        {
            Validator.CheckForNull(remoteHandler, "remoteHandler");
            Validator.CheckForNull(localDelegate, "localDelegate");

            try
            {
                // call asynchronously to avoid pending on foreach
                RemotingCallbackCarrier callCarrier = new RemotingCallbackCarrier(localDelegate, remoteHandler);

                IAsyncResult asyncResult = localDelegate.BeginInvoke(remoteHandler, OnCallbackEnd, callCarrier);
                switch (blockingMode)
                {
                    case DelegateSequenceBlockingMode.OverallBlocking:
                    case DelegateSequenceBlockingMode.SequentialBlocking:
                        asyncResult.AsyncWaitHandle.WaitOne();
                        break;
                }
            }
            catch (RemotingException)
            {
                DetachCallbackHandler(remoteHandler);
            }
            catch (SocketException)
            {
                DetachCallbackHandler(remoteHandler);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Call single callback for identifier get by virtual method GetCurrentUserId()
        /// </summary>
        /// <param name="userIdentifier">user identifier</param>
        /// <param name="typeOfHandler">type of handler. It has to be derived fom ARemotingCallbackHandler</param>
        /// <param name="localDelegate"></param>
        /// <param name="blockingMode"></param>
        /// <param name="obj"></param>
        public void SingleCallback(
            [NotNull] string userIdentifier,
            [NotNull] Type typeOfHandler,
            [NotNull] DRemotingCallbackWithObjects localDelegate,
            DelegateSequenceBlockingMode blockingMode,
            object[] obj)
        {
            Validator.CheckForNull(userIdentifier, "userIdentifier");
            Validator.CheckForNull(typeOfHandler, "typeOfHandler");
            Validator.CheckBaseType(typeOfHandler, typeof(ARemotingCallbackHandler));

            foreach (var session in GetSessionsForUser(userIdentifier))
            {
                ARemotingCallbackHandler handler = _remoteCallbackHandlers.GetHandler(
                typeOfHandler,
                session);

                if(handler != null)
                    SingleCallback(handler, localDelegate, blockingMode, obj);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteHandler"></param>
        /// <param name="localDelegate"></param>
        /// <param name="blockingMode"></param>
        /// <param name="obj"></param>
        public void SingleCallback(
            [NotNull] ARemotingCallbackHandler remoteHandler,
            [NotNull] DRemotingCallbackWithObjects localDelegate, 
            DelegateSequenceBlockingMode blockingMode,
            object[] obj)
        {
            Validator.CheckForNull(remoteHandler, "remoteHandler");
            Validator.CheckForNull(localDelegate, "localDelegate");

            try
            {
                // call asynchronously to avoid pending on foreach
                RemotingCallbackCarrier callCarrier = new RemotingCallbackCarrier(localDelegate, remoteHandler);

                IAsyncResult asyncResult = localDelegate.BeginInvoke(remoteHandler, obj, OnCallbackEnd, callCarrier);
                switch (blockingMode)
                {
                    case DelegateSequenceBlockingMode.OverallBlocking:
                    case DelegateSequenceBlockingMode.SequentialBlocking:
                        asyncResult.AsyncWaitHandle.WaitOne();
                        break;
                }
            }
            catch (RemotingException)
            {
                DetachCallbackHandler(remoteHandler);
            }
            catch (SocketException)
            {
                DetachCallbackHandler(remoteHandler);
            }
            catch
            {
            }
        }



        /// <summary>
        /// iterates through all callback handlers and calls the local delegate for each of them;
        /// </summary>
        /// <param name="localDelegate">method to execute for each</param>
        /// <param name="blockingMode">blocking mode during call</param>
        /// <param name="forceVerifyHandlers">if true, the remoting handler is verified </param>
        /// <param name="types">specify types of handlers to call</param>
        public void ForeachCallbackHandler(
            DRemotingCallback localDelegate, 
            DelegateSequenceBlockingMode blockingMode, 
            bool forceVerifyHandlers, 
            params Type[] types)
        {
            Validator.CheckForNull(localDelegate,"localDelegate");

            IEnumerable<ARemotingCallbackHandler> handlers = EnsuredActiveHandlers(forceVerifyHandlers, types);

            LinkedList<WaitHandle> waitHandles = null;
            
            foreach (ARemotingCallbackHandler remoteHandler in handlers)
            {
                try
                {
                    // call asynchronously to avoid pending on foreach
                    RemotingCallbackCarrier callCarrier = new RemotingCallbackCarrier(localDelegate, remoteHandler);

                    IAsyncResult asyncResult = localDelegate.BeginInvoke(remoteHandler, OnCallbackEnd, callCarrier);
                    PrepareOrApplyBlockingMode(blockingMode, ref waitHandles, asyncResult);
                }
                catch (RemotingException)
                {
                    DetachCallbackHandler(remoteHandler);
                }
                catch (SocketException)
                {
                    DetachCallbackHandler(remoteHandler);
                }
                catch
                {
                }

            }

            FinishBlockingMode(blockingMode, waitHandles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localDelegate"></param>
        /// <param name="blockingMode"></param>
        /// <param name="forceVerifyHandlers"></param>
        /// <param name="obj"></param>
        public void ForeachCallbackHandler(
            [NotNull] DRemotingCallbackWithObjects localDelegate,
            DelegateSequenceBlockingMode blockingMode,
            bool forceVerifyHandlers,
            params object[] obj)
        {
            ForeachCallbackHandler(
                localDelegate,
                blockingMode,
                forceVerifyHandlers,
                (Type)null,
                obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localDelegate"></param>
        /// <param name="blockingMode"></param>
        /// <param name="forceVerifyHandlers"></param>
        /// <param name="types"></param>
        /// <param name="obj"></param>
        public void ForeachCallbackHandler(
            [NotNull] DRemotingCallbackWithObjects localDelegate,
            DelegateSequenceBlockingMode blockingMode,
            bool forceVerifyHandlers,
            Type type,
            params object[] obj)
        {
            Validator.CheckForNull(localDelegate, "localDelegate");

            IEnumerable<ARemotingCallbackHandler> handlers;

            if(type == null)
                handlers = EnsuredActiveHandlers(forceVerifyHandlers);
            else
                handlers = EnsuredActiveHandlers(forceVerifyHandlers, type);

            LinkedList<WaitHandle> waitHandles = null;

            foreach (ARemotingCallbackHandler remoteHandler in handlers)
            {
                try
                {
                    // call asynchronously to avoid pending on foreach
                    RemotingCallbackCarrier callCarrier = new RemotingCallbackCarrier(localDelegate, remoteHandler);

                    IAsyncResult asyncResult = localDelegate.BeginInvoke(remoteHandler, obj, OnCallbackEnd, callCarrier);
                    PrepareOrApplyBlockingMode(blockingMode, ref waitHandles, asyncResult);
                }
                catch (RemotingException)
                {
                    DetachCallbackHandler(remoteHandler);
                }
                catch (SocketException)
                {
                    DetachCallbackHandler(remoteHandler);
                }
                catch (ArgumentNullException)
                {
                    DetachCallbackHandler(remoteHandler);
                }
                catch
                {
                }

            }

            FinishBlockingMode(blockingMode, waitHandles);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localDelegate"></param>
        /// <param name="blockingMode"></param>
        /// <param name="forceVerifyHandlers"></param>
        /// <param name="types"></param>
        /// <param name="obj"></param>
        public void ForeachCallbackHandler(
            [NotNull] DRemotingCallbackWithObjects localDelegate, 
            DelegateSequenceBlockingMode blockingMode, 
            bool forceVerifyHandlers, 
            Type[] types,
            params object[] obj)
        {
            Validator.CheckForNull(localDelegate,"localDelegate");

            IEnumerable<ARemotingCallbackHandler> handlers;

            if(types == null || types.Length == 0)
                handlers = EnsuredActiveHandlers(forceVerifyHandlers);
            else
                handlers = EnsuredActiveHandlers(forceVerifyHandlers, types);

            LinkedList<WaitHandle> waitHandles = null;

            foreach (ARemotingCallbackHandler remoteHandler in handlers)
            {
                try
                {
                    // call asynchronously to avoid pending on foreach
                    RemotingCallbackCarrier callCarrier = new RemotingCallbackCarrier(localDelegate, remoteHandler);

                    IAsyncResult asyncResult = localDelegate.BeginInvoke(remoteHandler, obj, OnCallbackEnd, callCarrier);
                    PrepareOrApplyBlockingMode(blockingMode, ref waitHandles, asyncResult);
                }
                catch (RemotingException)
                {
                    DetachCallbackHandler(remoteHandler);
                }
                catch (SocketException)
                {
                    DetachCallbackHandler(remoteHandler);
                }
                catch (ArgumentNullException)
                {
                    DetachCallbackHandler(remoteHandler);
                }
                catch
                {
                }

            }

            FinishBlockingMode(blockingMode, waitHandles);
        }

        private static void PrepareOrApplyBlockingMode(
            DelegateSequenceBlockingMode blockingMode,
            ref LinkedList<WaitHandle> waitHandles,
            IAsyncResult asyncResult)
        {
            switch (blockingMode)
            {
                case DelegateSequenceBlockingMode.OverallBlocking:
                    if (waitHandles == null)
                        waitHandles = new LinkedList<WaitHandle>();

                    waitHandles.AddLast(asyncResult.AsyncWaitHandle);
                    break;
                case DelegateSequenceBlockingMode.SequentialBlocking:
                    asyncResult.AsyncWaitHandle.WaitOne();
                    break;
            }
        }

        private static void FinishBlockingMode(
            DelegateSequenceBlockingMode blockingMode, 
            LinkedList<WaitHandle> waitHandles)
        {
            if (blockingMode == DelegateSequenceBlockingMode.OverallBlocking &&
                waitHandles != null)
            {
                foreach (WaitHandle wh in waitHandles)
                {
                    try
                    {
                        wh.WaitOne();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void OnCallbackEnd(IAsyncResult asyncResult)
        {
            RemotingCallbackCarrier carrier = null;
            try
            {
                carrier = (RemotingCallbackCarrier)asyncResult.AsyncState;
                if (carrier.LocalDelegateWithObjects != null)
                    carrier.LocalDelegateWithObjects.EndInvoke(asyncResult);
                else
                    carrier.LocalDelegate.EndInvoke(asyncResult);
            }
            catch (RemotingException)
            {
                if (carrier != null) DetachCallbackHandler(carrier.RemoteCallbackHandler);
            }
            catch (SocketException)
            {
                if (carrier != null) DetachCallbackHandler(carrier.RemoteCallbackHandler);
            }
            catch (ArgumentNullException)
            {
                if (carrier != null) DetachCallbackHandler(carrier.RemoteCallbackHandler);
            }
            catch
            {
            }
        }

        private class RemoteHandlerStore<T_SESSION_ID>
        {
            private Object _sync = new Object();

            private readonly Dictionary<string, TypeAndSessionAndHandler> _uriToTypeAndSession =
            new Dictionary<string, TypeAndSessionAndHandler>(4);

            private readonly Dictionary<Type, IDictionary<T_SESSION_ID, ARemotingCallbackHandler>> _handlersByType =
                new Dictionary<Type, IDictionary<T_SESSION_ID, ARemotingCallbackHandler>>(4);

            public RemoteHandlerStore()
            {
            }

            public class TypeAndSessionAndHandler
            {
                public Type HandlerType { get; set; }
                public T_SESSION_ID HandlerSession { get; set; }
                public ARemotingCallbackHandler Handler { get; set; }
            }

            internal Object SyncObj { get { return _sync; } }

            public bool AddHandler(ARemotingCallbackHandler remoteHandler, T_SESSION_ID sessionId)
            {
                lock (_sync)
                {
                    string handlerUri = RemotingServices.GetObjectUri(remoteHandler);

                    if (!_uriToTypeAndSession.ContainsKey(handlerUri))
                    {
                        _uriToTypeAndSession[handlerUri] = new TypeAndSessionAndHandler() 
                        { 
                            HandlerType = remoteHandler.GetType(), 
                            HandlerSession = sessionId,
                            Handler = remoteHandler
                        };

                        IDictionary<T_SESSION_ID, ARemotingCallbackHandler> handlerBySession;
                        Type handlerType = remoteHandler.GetType();

                        // Try get or create session to handler dictionary for current handler type
                        if (!_handlersByType.TryGetValue(handlerType, out handlerBySession))
                        {
                            // If dictionary does not exist create new
                            handlerBySession = new Dictionary<T_SESSION_ID,ARemotingCallbackHandler>(4);
                            _handlersByType[handlerType] = handlerBySession;
                        }

                        // Store current handler to dictionary
                        handlerBySession[sessionId] = remoteHandler;

                        return true;
                    }

                    return false;
                }
            }

            public bool RemoveCallbackHandlerByUri(string handlerUri)
            {
                lock (_sync)
                {
                    TypeAndSessionAndHandler typeAndSession;
                    if (_uriToTypeAndSession.TryGetValue(handlerUri, out typeAndSession))
                    {
                        IDictionary<T_SESSION_ID, ARemotingCallbackHandler> handlerBySession;

                        // Try get session to handler dictionary for current handler type
                        if (_handlersByType.TryGetValue(typeAndSession.HandlerType, out handlerBySession))
                        {
                            // Remove current handler by session
                            handlerBySession.Remove(typeAndSession.HandlerSession);

                            // If there is no more sessions for current type remove dictionary, too
                            if (handlerBySession.Count == 0)
                            {
                                _handlersByType.Remove(typeAndSession.HandlerType);
                            }
                        }

                        _uriToTypeAndSession.Remove(handlerUri);
#if DEBUG
                        Debug.Print("Detaching : " + handlerUri);
#endif

                        return true;
                    }

                    return false;
                }
            }

            public IEnumerable<ARemotingCallbackHandler> GetAllHandlersSnapshot()
            {
                lock (_sync)
                {
                    return new LinkedList<ARemotingCallbackHandler>(
                        _handlersByType.Values.SelectMany(handlerBySession => handlerBySession.Values));
                }
            }

            public IEnumerable<ARemotingCallbackHandler> GetAllHandlersSnapshot(Type type)
            {
                lock (_sync)
                {
                    IDictionary<T_SESSION_ID, ARemotingCallbackHandler> handlerBySession;

                    // Try get session to handler dictionary for requested type
                    if (_handlersByType.TryGetValue(type, out handlerBySession))
                    {
                        return new LinkedList<ARemotingCallbackHandler>(handlerBySession.Values);
                    }
                    else
                        return new ARemotingCallbackHandler[0];
                }
            }

            public ARemotingCallbackHandler GetHandler(Type type, T_SESSION_ID sessionId)
            {
                lock (_sync)
                {
                    IDictionary<T_SESSION_ID, ARemotingCallbackHandler> handlerBySession;

                    // Try get session to handler dictionary for requested type
                    if (_handlersByType.TryGetValue(type, out handlerBySession))
                    {
                        ARemotingCallbackHandler result;
                        if (!handlerBySession.TryGetValue(sessionId, out result))
                            return null;

                        return result;
                    }

                    return null;
                }
            }

            public void ForEachHandler(bool useSnapshot, Action<string, Type, ARemotingCallbackHandler> forEachAction)
            {
                Action<ICollection<KeyValuePair<string, TypeAndSessionAndHandler>>> forEachFunc = (values) =>
                    {
                        foreach (KeyValuePair<string, TypeAndSessionAndHandler> item in values)
                        {
                            forEachAction(
                                item.Key,
                                item.Value.HandlerType,
                                item.Value.Handler);
                        }
                    };

                if (!useSnapshot)
                {
                    lock (_sync)
                    {
                        forEachFunc(_uriToTypeAndSession);
                    }
                }
                else
                {
                    ICollection<KeyValuePair<string, TypeAndSessionAndHandler>> snapshot;
                    lock (_sync)
                    {
                        snapshot = new LinkedList<KeyValuePair<string, TypeAndSessionAndHandler>>(_uriToTypeAndSession);
                    }

                    forEachFunc(snapshot);
                }
            }
        }

        #region IRemotingService Members


        public event DVoid2Void ServiceDisposed;

        private bool _disposed;
        private void InvokeServiceDisposed()
        {
            if (_disposed)
                return;

            _disposed = true;

            if (null != ServiceDisposed)
                try
                {
                    Delegate[] delegates = ServiceDisposed.GetInvocationList();

                    foreach (var generalDelegate in delegates)
                    {
                        var d = generalDelegate as DVoid2Void;
                        if (null != d)
                            try
                            {
                                d();
                            }
                            catch
                            {
// ReSharper disable once DelegateSubtraction
                                ServiceDisposed -= d;
                            }
                    }
                }
                catch { }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            InvokeServiceDisposed();   
        }

        ~ARemotingService()
        {
            InvokeServiceDisposed();
        }

        #endregion

    }
}
