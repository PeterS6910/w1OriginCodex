using System;
using System.Collections.Generic;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;

namespace Contal.IwQuick.Remoting
{
    enum TRemotingPeerType
    {
        Tcp,
        Ipc
    }

    /// <summary>
    /// class for Remoting channel instance
    /// </summary>
    public class IpcRemotingPeer : ARemotingPeer
    {
        private static int _channelCounter;
        
        
        /// <summary>
        /// used for addressing the host in TCP channel or named pipe in IPC channel
        /// </summary>
        private string _ipcPortDestinationName;
        
        private string _ipcPortSourceName;

        private string _authorizedGroup;

        
        /// <summary>
        /// Client-side IPC-based remoting channel
        /// </summary>
        /// <param name="ipcDestination"></param>
        /// <param name="isSecuredChannel"></param>
        /// <param name="ipcPortName"></param>
        public IpcRemotingPeer(string ipcDestination, bool isSecuredChannel,string ipcPortName)
            :base(false,isSecuredChannel)
        {
            Validator.CheckNullString(ipcDestination);
            _ipcPortDestinationName = ipcDestination;
            _ipcPortSourceName = ipcPortName;
        }

        /// <summary>
        /// Client-side IPC-based remoting channel
        /// </summary>
        /// <param name="ipcDestination"></param>
        /// <param name="isSecuredChannel"></param>
        public IpcRemotingPeer(string ipcDestination, bool isSecuredChannel)
            :this(ipcDestination,isSecuredChannel,null)
        {
        }

        /// <summary>
        /// Server-side IPC-based remoting channel
        /// </summary>
        /// <param name="ipcPortName"></param>
        /// <param name="?"></param>
        /// <param name="authorizedGroup"></param>
        /// <param name="isSecuredChannel"></param>
        public IpcRemotingPeer(string ipcPortName, string authorizedGroup, bool isSecuredChannel)
            : base(true, isSecuredChannel)
        {
            Validator.CheckNullString(ipcPortName);
            _ipcPortSourceName = ipcPortName;
            _authorizedGroup = authorizedGroup;
        }

        protected override string GetRemotingUrl(Type requiredType)
        {
            if (null == requiredType)
                return null;

            return 
                string.Format(
                    "ipc://{0}/{1}", 
                    _ipcPortDestinationName, 
                    requiredType.Name);            
        }

        protected override IChannelSender CreateClientChannel()
        {
            return
                new IpcClientChannel(
                    string.Format(
                        "IpcRemoting{0}_{1}",
                        IsServer ? "Server" : "Client", 
                        _channelCounter++),
                    new BinaryClientFormatterSinkProvider());
        }

        protected override void PrepareServerChannels(
            IDictionary<string, ChannelInfo<IChannelReceiver>> channelSet)
        {
            ChannelInfo<IChannelReceiver> channelInfo =
                GetServerChannelInfo(_ipcPortSourceName);

            if (channelInfo == null)
            {
                IDictionary aRemotingProperties = new Hashtable(4);

                if (Validator.IsNotNullString(_authorizedGroup))
                    aRemotingProperties["authorizedGroup"] = _authorizedGroup;

                aRemotingProperties["exclusiveAddressUse"] = false;
                aRemotingProperties["name"] = "IpcRemoting" + (IsServer ? "Server" : "Client") + "_" + _channelCounter++;

                aRemotingProperties["portName"] = _ipcPortSourceName;

                BinaryServerFormatterSinkProvider aServerSinkProvider = new BinaryServerFormatterSinkProvider();
                aServerSinkProvider.TypeFilterLevel = TypeFilterLevel.Full;

                channelInfo =
                    new ChannelInfo<IChannelReceiver>(
                        new IpcServerChannel(
                            aRemotingProperties, 
                            aServerSinkProvider));
            }

            channelSet.Add(
                _ipcPortSourceName, 
                channelInfo);

        }
    }
}
