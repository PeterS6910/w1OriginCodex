using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.IO;
using Contal.IwQuick.Crypto;
using JetBrains.Annotations;

namespace Contal.IwQuick.Remoting
{
    /// <summary>
    /// Class for client sink with encryption stream
    /// </summary>
    public class AesCryptoClientSink : BaseChannelSinkWithProperties, IClientChannelSink
    {
        private readonly IClientChannelSink _nextSink;

        private readonly CryptoProcessor _cryptoProcessor;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="nextSink"></param>
        /// <param name="cryptoProcessor"></param>
        public AesCryptoClientSink(
            [NotNull] IClientChannelSink nextSink, 
            CryptoProcessor cryptoProcessor)
        {
            Validator.CheckForNull(nextSink,"nextSink");

            _nextSink = nextSink;
            _cryptoProcessor = cryptoProcessor;
        }

        /// <summary>
        /// Function for send message
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="requestHeaders">Headers</param>
        /// <param name="requestStream">Request stream</param>
        /// <param name="responseHeaders">Headers</param>
        /// <param name="responseStream">Response stream</param>
        public void ProcessMessage(IMessage msg, ITransportHeaders requestHeaders, Stream requestStream, out ITransportHeaders responseHeaders, out Stream responseStream)
        {
            requestStream = 
                _cryptoProcessor
                    .EncryptStream(requestStream);

            _nextSink.ProcessMessage(
                msg,
                requestHeaders,
                requestStream,
                out responseHeaders,
                out responseStream);

            responseStream = 
                _cryptoProcessor
                    .DecryptStream(responseStream);
        }

        /// <summary>
        /// Function for send request
        /// </summary>
        /// <param name="sinkStack">Client channel sink state</param>
        /// <param name="msg">Message</param>
        /// <param name="headers">Headers</param>
        /// <param name="stream">Stream with bytes to send</param>
        public void AsyncProcessRequest(
            IClientChannelSinkStack sinkStack,
            IMessage msg,
            ITransportHeaders headers,
            Stream stream)
        {
            // push onto stack and forward the request
            sinkStack.Push(this, null);

            stream = 
                _cryptoProcessor
                    .EncryptStream(stream);

            _nextSink.AsyncProcessRequest(
                sinkStack,
                msg,
                headers,
                stream);
        }

        /// <summary>
        /// Function for get response
        /// </summary>
        /// <param name="sinkStack">Client response channel sink</param>
        /// <param name="state">If stream is encrypted then state is true, else is false</param>
        /// <param name="headers">Headers</param>
        /// <param name="stream">Stream with reading bytes</param>
        public void AsyncProcessResponse(IClientResponseChannelSinkStack sinkStack, object state, ITransportHeaders headers, Stream stream)
        {
            stream = 
                _cryptoProcessor
                    .DecryptStream(stream);

            // forward the request
            sinkStack.AsyncProcessResponse(
                headers,
                stream);
        }

        /// <summary>
        /// Function always return null
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="headers">Headers</param>
        /// <returns>Return stream</returns>
        public Stream GetRequestStream(IMessage msg, ITransportHeaders headers)
        {
            return null; // request stream will be manipulated later
        }

        /// <summary>
        /// Return _nextSink
        /// </summary>
        public IClientChannelSink NextChannelSink
        {
            get
            {
                return _nextSink;
            }
        }
    }
}
