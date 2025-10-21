using System;
using System.Net.Sockets;
using System.Runtime.Remoting;
using System.Threading;

using Contal.IwQuick.Threads;
using JetBrains.Annotations;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// allows keeping the transparrent proxy according to the remoting peer's conditions
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RemotingProxyKeeper<T> : IRemotingProxyKeeper
        where T : class
    {
        private ARemotingPeer _remotingPeer;
        public ARemotingPeer RemotingPeer
        {
            get { return _remotingPeer; }
            [NotNull] set
            {
                Validator.CheckForNull(value,"value");

                if (value != _remotingPeer)
                {
                    _remotingPeer.UnregisterProxyKeeper(this);

                    _remotingPeer = value;

                    _remotingPeer.RegisterProxyKeeper(this);

                    if (null == _keepersThread)
                    {
                        _keepersThread = new SafeThread(KeepersThread);
                        _keepersThread.Start();
                    }
                    else
                    {
                        throw new NotImplementedException();
                        // finish thread restarting 
                        //_keepersThread.Abort();

                    }

                }
            }
        }

        private T _remoteObject;
        /// <summary>
        /// property to current instance
        /// </summary>
        public T RemoteObject
        {
            get { return _remoteObject; }
        }

        private volatile bool _isGained;
        public bool IsGained
        {
            get { return _isGained; }
        }

        /// <summary>
        /// instantiates keeper joined with specific remoting peer
        /// </summary>
        /// <param name="remotingPeer"></param>
        public RemotingProxyKeeper([NotNull] ARemotingPeer remotingPeer)
        {
            Validator.CheckForNull(remotingPeer,"remotingPeer");

            _remotingPeer = remotingPeer;
            _remotingPeer.RegisterProxyKeeper(this);
        }

        private int _retryDelay;

        private volatile SafeThread _keepersThread;

        /// <summary>
        /// starts proxy keeping 
        /// </summary>
        /// <param name="retryDelay">time in miliseconds</param>
        public void Start(int retryDelay)
        {
            if (null == _keepersThread)
                _keepersThread = new SafeThread(KeepersThread);
            else
                // already running
                return;

            if (retryDelay <= 1)
                retryDelay = 3000;

            _retryDelay = retryDelay;

            _keepersThread.Start();
        }

        /// <summary>
        /// starts proxy keeping with default delay time in miliseconds
        /// </summary>
        public void Start()
        {
            Start(0);
        }

        /// <summary>
        /// stops proxy keeping
        /// </summary>
        public void Stop()
        {
            if (null != _keepersThread)
                _keepersThread.Abort();
        }

        private readonly ManualResetEvent _terminateableTimer =
            new ManualResetEvent(false);

        private void KeepersThread()
        {
            try
            {
                while (null == _remoteObject)
                {
                    _terminateableTimer.Reset();

                    bool peerProblem = false;
                    try
                    {
                        _remoteObject = _remotingPeer.GetObject<T>();
                    }
                    catch (NullReferenceException)
                    {
                        peerProblem = true;
                    }
                    catch (ObjectDisposedException)
                    {
                        peerProblem = true;
                    }

                    if (peerProblem)
                    {
                        _terminateableTimer.WaitOne(_retryDelay, false);
                        continue;
                    }


                    if (null == _remoteObject)
                    {
                        // if returned true, it means, it was revoked from the outside
                        if (_terminateableTimer.WaitOne(_retryDelay, false))
                            DebugHelper.Keep();
                            //continue;
                    }
                    else
                    {
                        if (typeof(IRemotingService).IsAssignableFrom(typeof(T)))
                        {
                            ((IRemotingService)_remoteObject).ServiceDisposed +=
                                RemotingEventSurrogate.Create(ServiceDisposed);
                        }

                        _isGained = true;
                        InvokeProxyGained();
                    }
                }
            }
            finally
            {
                _keepersThread = null;
            }
        }

        private void ServiceDisposed()
        {
            RevokeProxy(true);
        }

        /// <summary>
        /// event raised , when the proxy has been gained
        /// </summary>
        public event Action<T> ProxyGained = null;

        /// <summary>
        /// event raised, when the proxy has been lost
        /// </summary>
        public event Action<Type> ProxyLost = null;

        private void InvokeProxyGained()
        {
            if (null != ProxyGained)
                try
                {
                    ProxyGained(_remoteObject);
                }
                catch
                {
                }
        }

        private void InvokeProxyLost()
        {
            if (null != ProxyLost)
                try { ProxyLost(typeof(T)); }
                catch
                {
                }
        }

        private readonly object _lockRevokeProxy = new object();

        private void RevokeProxy(bool raiseEvent)
        {
            if (raiseEvent)
                InvokeProxyLost();

            _remoteObject = default(T);
            _isGained = false;

            lock (_lockRevokeProxy)
            {
                // to continue the cycle in the thread
                if (null != _keepersThread)
                {
                    _terminateableTimer.Set();

                    try
                    {
                        _keepersThread.Stop(500);
                    }
                    catch { }
                }

                _keepersThread = new SafeThread(KeepersThread);
                _keepersThread.Start();
            }
        }

        /// <summary>        
        /// allow re-gaining the proxy
        /// </summary>    
        public void AllowProxyRegaining()
        {
            try
            {
                T remoteObject = _remotingPeer.GetObject<T>();               

                if (remoteObject == null)
                {
                    RevokeProxy(true);
                }
            }
            catch
            { }
        }

        /// <summary>
        /// allows reporting the problem over the proxy,
        /// thus allowing re-gaining the proxy
        /// </summary>
        /// <param name="error"></param>
        public void ReportProblem(Exception error)
        {
            if (null == error)
                return;

            if (error is RemotingException ||
                error is SocketException)
            {
                RevokeProxy(true);
            }
            else
            {
                try
                {
                    T remoteObject = _remotingPeer.GetObject<T>();
                    if (remoteObject == null)
                    {
                        RevokeProxy(true);
                    }
                }
                catch
                { }
            }
        }


        public void Restart()
        {
            SafeThread<bool>.StartThread(RevokeProxy, true);
        }
    }
}
