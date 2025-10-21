using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.IO;
using Contal.IwQuick.Crypto;
using JetBrains.Annotations;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// Class for server sink with encryption stream
    /// </summary>
    public class AesCryptoServerSink : BaseChannelSinkWithProperties, IServerChannelSink
    {
        private readonly IServerChannelSink _nextSink;

        private readonly CryptoProcessor _cryptoProcessor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextSink">Server channel sink</param>
        /// <param name="cryptoProcessor"></param>
        public AesCryptoServerSink(
            [NotNull] IServerChannelSink nextSink, 
            CryptoProcessor cryptoProcessor)
        {
            Validator.CheckForNull(nextSink,"nextSink");

            _nextSink = nextSink;
            _cryptoProcessor = cryptoProcessor;
        }

        /// <summary>
        /// Function for get message
        /// </summary>
        /// <param name="sinkStack">Server channel sink stack</param>
        /// <param name="requestMsg">Message for request</param>
        /// <param name="requestHeaders">Headers for request</param>
        /// <param name="requestStream">Stream for request</param>
        /// <param name="responseMsg">Message for response</param>
        /// <param name="responseHeaders">Headers for response</param>
        /// <param name="responseStream">Stream for response</param>
        /// <returns>Return enum ServerProcessing, which is returned from sink's method PrecessMessage</returns>
        public ServerProcessing ProcessMessage(IServerChannelSinkStack sinkStack, IMessage requestMsg, ITransportHeaders requestHeaders, Stream requestStream, out IMessage responseMsg, out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            requestStream = 
                _cryptoProcessor    
                    .DecryptStream(requestStream);

            ServerProcessing srvProc =
                _nextSink.ProcessMessage(
                    sinkStack,
                    requestMsg,
                    requestHeaders,
                    requestStream,
                    out responseMsg,
                    out responseHeaders,
                    out responseStream);

            responseStream =
                _cryptoProcessor
                    .EncryptStream(responseStream);

            return srvProc;
        }

        /// <summary>
        /// Function for send response
        /// </summary>
        /// <param name="sinkStack">Server response channel sink stack</param>
        /// <param name="state">If stream is encrypted tehn state is true, else is false</param>
        /// <param name="msg">Message</param>
        /// <param name="headers">Headers</param>
        /// <param name="stream">Stream with bytes to send</param>
        public void AsyncProcessResponse(
            IServerResponseChannelSinkStack sinkStack,
            object state,
            IMessage msg,
            ITransportHeaders headers,
            Stream stream)
        {
            // encrypting the response
            stream =
                _cryptoProcessor
                    .EncryptStream(stream);

            // forwarding to the stack for further processing
            sinkStack.AsyncProcessResponse(
                msg,
                headers,
                stream);
        }

        /// <summary>
        /// Function allways return null
        /// </summary>
        /// <param name="sinkStack">Server response channel sink stack</param>
        /// <param name="state">If stream is encrypted tehn state is true, else is false</param>
        /// <param name="msg">Message</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return stream</returns>
        public Stream GetResponseStream(IServerResponseChannelSinkStack sinkStack, object state, IMessage msg, ITransportHeaders headers)
        {
            return null;
        }

        /// <summary>
        /// Return _nextSink
        /// </summary>
        public IServerChannelSink NextChannelSink
        {
            get
            {
                return _nextSink;
            }
        }
    }
}
