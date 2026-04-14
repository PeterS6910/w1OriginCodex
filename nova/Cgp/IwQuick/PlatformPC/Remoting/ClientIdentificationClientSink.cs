using System.Collections;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;

namespace Contal.IwQuick.Remoting
{
    public class ClientIdentificationClientSink : 
        BaseChannelObjectWithProperties, 
        IClientChannelSink
    {
        private static string _sessionId;

        #region IChannelSinkBase Members

        IDictionary IChannelSinkBase.Properties
        {
            get { return null; }
        }

        #endregion

        #region IClientChannelSink Members

        public void AsyncProcessRequest(
            IClientChannelSinkStack sinkStack, 
            IMessage msg, 
            ITransportHeaders headers, 
            Stream stream)
        {
            NextChannelSink.AsyncProcessRequest(sinkStack, msg, headers, stream);
        }

        public void AsyncProcessResponse(
            IClientResponseChannelSinkStack sinkStack, 
            object state, 
            ITransportHeaders headers, 
            Stream stream)
        {
            NextChannelSink.AsyncProcessResponse(
                sinkStack, 
                state, 
                headers, 
                stream);
        }

        public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
        {
            return NextChannelSink.GetRequestStream(msg, headers);
        }

        public IClientChannelSink NextChannelSink
        {
            get; 
            private set;
        }

        private const string SESION_ID = "SesionId";

        public void ProcessMessage(
            IMessage msg, 
            ITransportHeaders requestHeaders, 
            Stream requestStream, 
            out ITransportHeaders responseHeaders, 
            out Stream responseStream)
        {
            requestHeaders[SESION_ID] = _sessionId;

            NextChannelSink.ProcessMessage(
                msg,
                requestHeaders,
                requestStream,
                out responseHeaders,
                out responseStream);

            if (responseHeaders[SESION_ID] != null)
                _sessionId = responseHeaders[SESION_ID] as string;
        }

        #endregion

        public ClientIdentificationClientSink(IClientChannelSink nextClientChannelSink)
        {
            NextChannelSink = nextClientChannelSink;
        }
    }
}