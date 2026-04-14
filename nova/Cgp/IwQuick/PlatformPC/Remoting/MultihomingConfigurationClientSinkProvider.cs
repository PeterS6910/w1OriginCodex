using System;
using System.Collections;
using System.Collections.Generic;
#if TRACE_REMOTING
using System.Diagnostics;
#endif
using System.IO;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Contal.IwQuick.Remoting
{
    /// <summary>The purpose of this sink provider is two-fold:
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// to create a sink that can (depending on value of the 
    /// constructor parameter <c>multihoming</c>) 
    /// include a transport header "X-Multihoming". 
    /// The transport header is subsequently interpreted by remoting server 
    /// (equipped with MultihomingConfigurationServerSinkProvider) 
    /// as request by client to be treated as multihomed.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// to select the appropriate remote object's URL from among the ones
    /// sent as a part of remote object's ObjectData. 
    /// This happens when the remoting server
    /// deserializes received ObjRef and tries to create client sink chain
    /// for the particular remote object. See function <c>CreateSink</c>
    /// </description>
    /// </item>
    /// </list>
    /// </summary>
    public class MultihomingConfigurationClientSinkProvider : IClientChannelSinkProvider
    {
        private class Sink : IClientChannelSink
        {
            private readonly bool _multihoming;

            public Sink(
                IClientChannelSink nextChannelSink,
                bool multihoming)
            {
                _multihoming = multihoming;
                NextChannelSink = nextChannelSink;
            }

            public IDictionary Properties
            {
                get { return null; }
            }

            public void ProcessMessage(
              IMessage msg,
              ITransportHeaders requestHeaders,
              Stream requestStream,
              out ITransportHeaders responseHeaders,
              out Stream responseStream)
            {
                CallContext.FreeNamedDataSlot(RemotingConstants.MULTIHOMING_REQUESTED);

                NextChannelSink.ProcessMessage(
                    msg,
                    requestHeaders,
                    requestStream,
                    out responseHeaders,
                    out responseStream);

                if (responseHeaders[RemotingConstants.MULTIHOMING_HEADER] != null)
                    CallContext.SetData(
                        RemotingConstants.MULTIHOMING_REQUESTED, 
                        new object());
            }

            public void AsyncProcessRequest(
              IClientChannelSinkStack sinkStack,
              IMessage msg,
              ITransportHeaders headers,
              Stream stream)
            {
                NextChannelSink.AsyncProcessRequest(
                  sinkStack, msg, headers, stream);
            }

            public void AsyncProcessResponse(
              IClientResponseChannelSinkStack sinkStack,
              object state,
              ITransportHeaders headers,
              Stream stream)
            {
                NextChannelSink.AsyncProcessResponse(
                  sinkStack, state, headers, stream);
            }

            public Stream GetRequestStream(
              IMessage msg,
              ITransportHeaders headers)
            {
                if (_multihoming)
                    headers[RemotingConstants.MULTIHOMING_HEADER] = true;

                return
                    NextChannelSink.GetRequestStream(
                        msg, 
                        headers);
            }

            public IClientChannelSink NextChannelSink
            {
                get;
                private set;
            }
        }

        private readonly IDictionary<string, bool> _encounteredHostCache =
            new Dictionary<string, bool>();

        private readonly bool _multihoming;

        public MultihomingConfigurationClientSinkProvider(
            bool multihoming)
        {
            _multihoming = multihoming;
        }

        private bool IsValidUrlHost(string url)
        {
            var receivedIpAddress =
                ClientIdentificationServerSink.CallingClientIP;

            var serverChannelUri = new Uri(url);
            var serverChannelUriHost = serverChannelUri.Host;

            lock (_encounteredHostCache)
            {
                if (_encounteredHostCache.ContainsKey(serverChannelUriHost))
                    return true;

                CallContext.GetHeaders();

                if (CallContext.GetData(RemotingConstants.MULTIHOMING_REQUESTED) != null)
                {
#if TRACE_REMOTING
                    Trace.WriteLine(
                        string.Format(
                            "MultihomingConfigurationClientSinkProvider: ipAddress = {0}",
                            receivedIpAddress));
#endif
                    IPAddress serverChannelUriIpAddress;

                    if (IPAddress.TryParse(
                            serverChannelUriHost,
                            out serverChannelUriIpAddress))
                        if (receivedIpAddress == null ||
                                !receivedIpAddress.Equals(serverChannelUriIpAddress))
                            return false;
                }
#if TRACE_REMOTING
                else
                    Trace.WriteLine("Multihoming not requested");
#endif

                _encounteredHostCache.Add(serverChannelUriHost, true);

                return true;
            }
        }

        /// <summary>
        /// Create this channels's sink chain for object referred to by provided url.
        /// If remote party's client channel requested multihoming configuration,
        /// then we create sink chain only if the remote object's url corresponds 
        /// to the remote party's ip address (as stored in CallContext).
        /// 
        /// This function is called by remoting infrastructure iteratively, once for 
        /// each url from among the ones received as a part of remote object's 
        /// channel data.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="url"></param>
        /// <param name="remoteChannelData"></param>
        /// <returns></returns>
        public IClientChannelSink CreateSink(
          IChannelSender channel,
          string url,
          object remoteChannelData)
        {
#if TRACE_REMOTING
            Trace.WriteLine(
                string.Format(
                    "Attempting to create to an client channel for url={0}",
                    url));
#endif
            if (!IsValidUrlHost(url))
                return null;

#if TRACE_REMOTING
            Trace.WriteLine(
                string.Format(
                    "Accepted url={0}",
                    url));
#endif
            IClientChannelSink nextSink = null;

            if (Next != null)
                nextSink =
                  Next.CreateSink(
                    channel,
                    url,
                    remoteChannelData);

            return 
                new Sink(
                    nextSink, 
                    _multihoming);
        }

        public IClientChannelSinkProvider Next
        {
            get;
            set;
        }
    }
}
