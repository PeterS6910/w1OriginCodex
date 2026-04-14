using System.Collections;
#if TRACE_REMOTING
using System.Diagnostics;
#endif
using System.IO;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// Provides server channel sink that translates multihoming request 
    /// (received in the form of transport header) into CallContext 
    /// boolean variable (hence accessible thread-wide).
    /// </summary>
    public class MultihomingConfigurationServerSinkProvider : IServerChannelSinkProvider
    {
        private readonly bool _multihoming;

        public MultihomingConfigurationServerSinkProvider(bool multihoming)
        {
            _multihoming = multihoming;
        }

        private class Sink : IServerChannelSink
        {
            private readonly bool _multihoming;

            public Sink(IServerChannelSink nextSink, bool multihoming)
            {
                NextChannelSink = nextSink;
                _multihoming = multihoming;
            }

            public void AsyncProcessResponse(
                IServerResponseChannelSinkStack sinkStack,
                object state,
                IMessage msg,
                ITransportHeaders headers,
                Stream stream)
            {
                NextChannelSink.AsyncProcessResponse(
                  sinkStack,
                  state,
                  msg,
                  headers,
                  stream);
            }

            public Stream GetResponseStream(
                IServerResponseChannelSinkStack sinkStack,
                object state,
                IMessage msg,
                ITransportHeaders headers)
            {
                return
                  NextChannelSink.GetResponseStream(
                    sinkStack,
                    state,
                    msg,
                    headers);
            }

            public IServerChannelSink NextChannelSink { get; private set; }

            public ServerProcessing ProcessMessage(
                IServerChannelSinkStack sinkStack,
                IMessage requestMsg,
                ITransportHeaders requestHeaders,
                Stream requestStream,
                out IMessage responseMsg,
                out ITransportHeaders responseHeaders,
                out Stream responseStream)
            {
#if TRACE_REMOTING
                Trace.WriteLine("MultihomingConfigurationServerSinkProvider.ProcessMessage: received");
#endif

                if (requestHeaders[RemotingConstants.MULTIHOMING_HEADER] != null)
                {
#if TRACE_REMOTING
                    Trace.WriteLine(
                        "MultihomingConfigurationServerSinkProvider.ProcessMessage: X-Multihoming = true");
#endif
                    CallContext.SetData(
                        RemotingConstants.MULTIHOMING_REQUESTED,
                        new object());
                }
                else
                    CallContext.FreeNamedDataSlot(RemotingConstants.MULTIHOMING_REQUESTED);

                ServerProcessing result =
                    NextChannelSink.ProcessMessage(
                        sinkStack,
                        requestMsg,
                        requestHeaders,
                        requestStream,
                        out responseMsg,
                        out responseHeaders,
                        out responseStream);
                
                CallContext.FreeNamedDataSlot(RemotingConstants.MULTIHOMING_REQUESTED);

                if (_multihoming)
                    responseHeaders[RemotingConstants.MULTIHOMING_HEADER] = true;

                return result;
            }

            public IDictionary Properties
            {
                get { return null; }
            }
        }

        public IServerChannelSink CreateSink(IChannelReceiver channel)
        {
            IServerChannelSink nextSink = null;

            if (Next != null)
                nextSink = Next.CreateSink(channel);

#if TRACE_REMOTING
            Trace.WriteLine(
                string.Concat(
                    "MultihomingConfigurationServerSinkProvider.CreateSink: called for ",
                    channel.GetUrlsForUri("")));
#endif

            return new Sink(nextSink, _multihoming);
        }

        public void GetChannelData(IChannelDataStore channelData)
        {
        }

        public IServerChannelSinkProvider Next { get; set; }
    }
}
