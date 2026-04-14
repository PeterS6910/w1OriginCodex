using System;
using System.Collections.Generic;
#if TRACE_REMOTING
using System.Diagnostics;
#endif
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using JetBrains.Annotations;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// abstract class for encapsulation of the remoting peer endpoint
    /// </summary>
    public abstract class ARemotingPeer
    {
        private readonly bool _server;
        /// <summary>
        /// returns true, if the peer is primary role is to be host for services
        /// </summary>
        public bool IsServer
        {
            get { return _server; }
        }

        private readonly bool _securedChannel;
        /// <summary>
        /// returns true, if the channel is secured
        /// </summary>
        public bool IsSecuredChannel
        {
            get { return _securedChannel; }
        }


        protected class ChannelInfo<TChannel> 
            where TChannel : class, IChannel
        {
            public TChannel Channel { get; private set; }
            public bool Registered { get; private set; }

            public ChannelInfo([NotNull] TChannel channel)
            {
                Validator.CheckForNull(channel,"channel");

                Channel = channel;
            }

            public void TryUnregister()
            {
                if (!Registered)
                    return;

                try
                {
                    ChannelServices.UnregisterChannel(Channel);
                }
                catch (Exception)
                {
                }
                finally
                {
                    Registered = false;
                }
            }

            public void TryRegister(bool securedChannel)
            {
                if (Registered)
                    return;

                try
                {
                    ChannelServices.RegisterChannel(
                        Channel,
                        securedChannel);

                    Registered = true;
                }
                catch
                {
                }
            }
        }

        private ChannelInfo<IChannelSender> _clientChannelInfo;

        private readonly IDictionary<string, ChannelInfo<IChannelReceiver>> _serverChannelsChache =
            new Dictionary<string, ChannelInfo<IChannelReceiver>>();

        protected ChannelInfo<IChannelReceiver> GetServerChannelInfo(string address)
        {
            ChannelInfo<IChannelReceiver> channelInfo;

            _serverChannelsChache.TryGetValue(
                address,
                out channelInfo);

            return channelInfo;
        }

        /// <summary>
        /// constructor with role separation
        /// </summary>
        /// <param name="isServer">if true, peer's primary role is to provide services</param>
        /// <param name="isSecuredChannel">if true, the underlying channel will use secured transport</param>
        protected ARemotingPeer(bool isServer, bool isSecuredChannel)
        {
            _server = isServer;
            _securedChannel = isSecuredChannel;
        }

        /// <summary>
        /// returns URI of the required type
        /// </summary>
        /// <param name="requiredType"></param>
        /// <returns></returns>
        protected abstract string GetRemotingUrl(Type requiredType);

        /// <summary>
        /// prepares underlying transport channels
        /// </summary>
        /// <returns></returns>
        /// 
        protected abstract IChannelSender CreateClientChannel();
        protected abstract void PrepareServerChannels(IDictionary<string, ChannelInfo<IChannelReceiver>> channelSet);

        protected void EnsureChannelRegistration()
        {
            try
            {
                RemotingConfiguration.CustomErrorsMode = CustomErrorsModes.Off;
            }
            catch
            {
            }

            lock (_serverChannelsChache)
            {
                if (_clientChannelInfo == null)
                    _clientChannelInfo =
                        new ChannelInfo<IChannelSender>(
                            CreateClientChannel());

                _clientChannelInfo.TryRegister(_securedChannel);

                IDictionary<string, ChannelInfo<IChannelReceiver>> newChannels =
                    new Dictionary<string, ChannelInfo<IChannelReceiver>>();

                //bool pcSucceeded =
                // throws exceptions if fails
                PrepareServerChannels(newChannels);

                /*if (!pcSucceeded)
                    throw new InvalidOperationException("Some of the .NET remoting channel preparations failed");*/

                ICollection<string> addressesOfChannelsToRemove =
                    new LinkedList<string>();

                foreach (string address in _serverChannelsChache.Keys)
                    if (!newChannels.Keys.Contains(address))
                        addressesOfChannelsToRemove.Add(address);

                foreach (string address in addressesOfChannelsToRemove)
                {
                    ChannelInfo<IChannelReceiver> channelInfo;

                    if (!_serverChannelsChache.TryGetValue(
                            address,
                            out channelInfo))
                        continue;

                    channelInfo.TryUnregister();
                    _serverChannelsChache.Remove(address);
                }

                foreach (var kvPair in newChannels)
                {
                    if (!_serverChannelsChache.ContainsKey(kvPair.Key))
                        _serverChannelsChache.Add(kvPair);

                    kvPair.Value.TryRegister(_securedChannel);
                }
            }
        }

        /// <summary>
        /// retrieves transparent proxy for the remote object
        /// </summary>
        /// <param name="requiredType">requested type / interface;
        /// IMPORTANT; this type must be shared between remoting client and server</param>
        /// <exception cref="InvalidOperationException">if called over peer set as server</exception>
        /// <exception cref="ArgumentNullException">if the required type is null</exception>
        public Object GetObject([NotNull] Type requiredType)
        {
            if (_server)
                throw new InvalidOperationException("Remoting peer is set as server handler");

            Validator.CheckForNull(requiredType,"requiredType");

            EnsureChannelRegistration();

            string remotingUrl = GetRemotingUrl(requiredType);

#if TRACE_REMOTING
            Trace.WriteLine(
                string.Format(
                    "ARemotingPeer.Getobject url={0}",
                    remotingUrl));
#endif

            try
            {
                Object aRet = Activator.GetObject(requiredType, remotingUrl);
                aRet.ToString();
                return aRet;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// retrieves transparent proxy for the remote object in requested type
        /// </summary>
        /// <typeparam name="TRemote">requested type / interface;
        /// IMPORTANT; this type must be shared between remoting client and server</typeparam>
        public TRemote GetObject<TRemote>()
        {
            return (TRemote)GetObject(typeof(TRemote));
        }


        /// <summary>
        /// initiates sharing of the type for remoting
        /// </summary>
        /// <param name="providedType">type to be shared, instantiated and remoted</param>
        /// <param name="providedInterface">interface of the type to be shared;
        /// IMPORTANT - providedInterface must be shared between remoting client and server;
        /// can be null and then, the providedType is used;
        /// </param>
        /// <param name="isSingleton">if true, only one instance is created for all calls</param>
        /// <exception cref="ArgumentNullException">if providedObject is null</exception>
        /// <exception cref="InvalidOperationException">if the peer is set as client</exception>
        /// <exception cref="InvalidCastException">if the type does not inherit from the interface</exception>
        public void ProvideType([NotNull] Type providedType, Type providedInterface, bool isSingleton)
        {
            if (!_server)
                throw new InvalidOperationException("Remoting peer is set as client handler");

            Validator.CheckForNull(providedType,"providedType");
            if (null == providedInterface)
                providedInterface = providedType;
            else
            {
                if (!providedInterface.IsAssignableFrom(providedType))
                    throw new InvalidCastException("Specified type \"" + providedType + "\" does not inherit from the \"" + providedInterface + "\" interface");
            }

            EnsureChannelRegistration();

            WellKnownServiceTypeEntry aServiceEntry = new WellKnownServiceTypeEntry(
                providedType, providedInterface.Name,
                isSingleton ? WellKnownObjectMode.Singleton : WellKnownObjectMode.SingleCall);

            RemotingConfiguration.RegisterWellKnownServiceType(
                aServiceEntry
                );

        }

        /// <summary>
        /// initiates sharing of the object identified by an intefrace
        /// </summary>
        /// <param name="providedObject">object to be shared</param>
        /// <param name="providedInterface">interface of the object , by which it is identified; 
        /// IMPORTANT - providedInterface must be shared between remoting client and server;</param>
        /// <exception cref="ArgumentNullException">if providedObject or providedInterface is null</exception>
        /// <exception cref="InvalidOperationException">if the peer is set as client</exception>
        /// <exception cref="InvalidCastException">if the object does not inherit from the interface</exception>
        public void ProvideObject([NotNull] MarshalByRefObject providedObject, [NotNull] Type providedInterface)
        {
            if (!_server)
                throw new InvalidOperationException("Remoting peer is set as client handler");

            Validator.CheckForNull(providedObject, "providedObject");
            Validator.CheckForNull(providedInterface, "providedInterface");

            if (!providedInterface.IsInstanceOfType(providedObject))
                throw new InvalidCastException("Specified object type \"" + providedObject.GetType() + "\" does not inherit from the \"" + providedInterface + "\" interface");

            EnsureChannelRegistration();

            RemotingServices.Marshal(providedObject, providedInterface.Name);
        }

        /// <summary>
        /// initiates sharing of the object identified by an intefrace
        /// </summary>
        /// <typeparam name="TInterface">interface of the object , by which it is identified; 
        /// IMPORTANT - providedInterface must be shared between remoting client and server</typeparam>
        /// <param name="provideObject">object to be shared</param>
        /// <exception cref="ArgumentNullException">if providedObject or providedInterface is null</exception>
        /// <exception cref="InvalidOperationException">if the peer is set as client</exception>
        /// <exception cref="InvalidCastException">if the object does not inherit from the interface</exception>
        public void ProvideObject<TInterface>(MarshalByRefObject provideObject)
        {
            ProvideObject(provideObject, typeof(TInterface));
        }

        public void ProvideObject<TInterface>(ARemotingService service)
        {
            ProvideObject(service, typeof(TInterface));
        }

        /// <summary>
        /// initiates sharing of the type for remoting
        /// </summary>
        /// <typeparam name="TType">type to be shared, instantiated and remoted</typeparam>
        /// <typeparam name="TInterface">interface of the type to be shared;
        /// IMPORTANT - providedInterface must be shared between remoting client and server;
        /// can be null and then, the providedType is used</typeparam>
        /// <exception cref="ArgumentNullException">if providedObject is null</exception>
        /// <exception cref="InvalidOperationException">if the peer is set as client</exception>
        /// <exception cref="InvalidCastException">if the type does not inherit from the interface</exception>
        public void ProvideType<TType, TInterface>(bool isSingleton)
        {
            ProvideType(typeof(TType), typeof(TInterface), isSingleton);
        }

        /// <summary>
        /// unregisters the underlying channel
        /// </summary>
        public void UnregisterChannels()
        {
            lock (_serverChannelsChache)
            {
                if (_clientChannelInfo != null)
                    _clientChannelInfo.TryUnregister();

                foreach (ChannelInfo<IChannelReceiver> channelInfo in _serverChannelsChache.Values)
                    channelInfo.TryUnregister();

                _serverChannelsChache.Clear();
            }
        }

        private readonly LinkedList<IRemotingProxyKeeper> _proxyKeepers = new LinkedList<IRemotingProxyKeeper>();
        protected internal void RegisterProxyKeeper([NotNull] IRemotingProxyKeeper proxyKeeper)
        {
            Validator.CheckForNull(proxyKeeper,"proxyKeeper");

            lock (_proxyKeepers)
            {
                if (null == _proxyKeepers.Find(proxyKeeper))
                    _proxyKeepers.AddLast(proxyKeeper);
            }
        }

        protected internal void UnregisterProxyKeeper(IRemotingProxyKeeper proxyKeeper)
        {
            if (null == proxyKeeper)
                return;

            lock (_proxyKeepers)
            {
                try
                {
                    _proxyKeepers.Remove(proxyKeeper);
                }
                catch
                {
                }
            }
        }

        protected internal void RestartProxyKeepers()
        {
            IRemotingProxyKeeper[] pks;
            lock (_proxyKeepers)
            {
                pks = new IRemotingProxyKeeper[_proxyKeepers.Count];
                _proxyKeepers.CopyTo(pks, 0);
            }

            foreach (IRemotingProxyKeeper pk in pks)
            {
                if (null == pk)
                    continue;

                try
                {
                    pk.Restart();
                }
                catch
                {
                    UnregisterProxyKeeper(pk);
                }
            }
        }
    }
}
