using System;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Channels;

using System.IO;

using System.Net;
using Contal.IwQuick.Crypto;

namespace Contal.IwQuick.Remoting
{
    public class ClientIdentificationServerSink :
        BaseChannelObjectWithProperties,
        IServerChannelSink
    {
        public ClientIdentificationServerSink(IServerChannelSink next)
        {
            NextChannelSink = next;
        }

        public IServerChannelSink NextChannelSink { get; set; }

        // Count of client connected to server
        private long _sesionCount;

        /// <summary>
        /// Return next session id for new client
        /// </summary>
        private long NextSessionId
        {
            get 
            {
                _sesionCount++;
                return _sesionCount;
            }
        }

        /// <summary>
        /// Generate hash code for session
        /// </summary>
        /// <returns>Return hash code</returns>
        private string GenerateHashSessionCode()
        {
            IPAddress clientIp = CallingClientIP;
            String hashString = clientIp.ToString();
            hashString += " " + NextSessionId;
            hashString += " " + GetHashCode();

            QuickHashes.GetSHA256String(hashString);
            return QuickHashes.GetSHA256String(hashString); 
        }

        private int _sessionTimeout = 300000;
        /// <summary>
        /// Timeout for session in miliseconds
        /// </summary>
        public int SessionTimeout
        {
            get
            {
                return _sessionTimeout;
            }
            set
            {
                if (value > 0)
                    _sessionTimeout = value;
            }
        }

        private const string SESION_ID = "SesionId";
     
        /// <summary>
        /// Get session id from client and set it to CallContext 
        /// if session id is null or session is time out then generate new sessieon id.
        /// </summary>
        /// <param name="requestMsg">Input request message</param>
        /// <returns>Return session id</returns>
        private string GetSessionIdFromClient(ITransportHeaders requestMsg)
        {
            string sessionId = null;

            if (requestMsg != null)
            {
                //LogicalCallContext lcc = (LogicalCallContext)requestMsg.Properties["__CallContext"];
                sessionId = requestMsg[SESION_ID] as string;

                if (!Validator.IsNullString(sessionId))
                    if (!RemotingSessionHandler.Singleton.RefreshSessionTimeout(sessionId))
                        // this means transeferred sessionId is either expired or invalid
                        sessionId = null;

                if (Validator.IsNullString(sessionId)) // revalidate intentionally as previous code can null sessionId
                {
                    // executed if transfered sessionId is not known or invalid (e.g. expired)
                    sessionId = GenerateHashSessionCode();
                    RemotingSessionHandler.Singleton.RegisterSession(sessionId);
                }

                CallContext.SetData(SESION_ID, sessionId);                
            }

            return sessionId;
        }

        /// <summary>
        /// retrieves SessionId form the CallContext.GetData if any
        /// if not available, returns null
        /// </summary>
        public static string CallingSessionId
        {
            get
            {
                try
                {
                    return CallContext.GetData(SESION_ID) as string;
                }
                catch
                {
                    return null;
                }
            }
        }

        private const string KEY_CLIENT_IP = "ClientIP";
        private const string KEY_CONNECTION_ID = "ConnectionID";

        private void ExtractTransportHeaders(ITransportHeaders transportHeaders)
        {
            IPAddress ip = transportHeaders[CommonTransportKeys.IPAddress] as IPAddress;
            CallContext.SetData(KEY_CLIENT_IP, ip);

           

#if DEBUG
            Int64 cid = (Int64)transportHeaders[CommonTransportKeys.ConnectionId];
            object o = transportHeaders[CommonTransportKeys.RequestUri];
#endif
            CallContext.SetData(KEY_CONNECTION_ID, transportHeaders[CommonTransportKeys.ConnectionId]);
        }

        /// <summary>
        /// retrieves Client IP from the CallContext.GetData if any;
        /// if not available, returns null
        /// </summary>
        public static IPAddress CallingClientIP
        {
            get
            {
                CallContext.GetHeaders();

                return (IPAddress)CallContext.GetData(KEY_CLIENT_IP);
            }
        }

        /// <summary>
        /// retrieves Connection ID from the CallContext.GetData if any;
        /// if not available, returns -1
        /// </summary>
        public static Int64 CallingConnectionID
        {
            get
            {
                try
                {
                    object o = CallContext.GetData(KEY_CONNECTION_ID);
                    return null != o ? (Int64) o : -1;
                }
                catch
                {
                    return -1;
                }
            }
        }

        public void AsyncProcessResponse(
                IServerResponseChannelSinkStack sinkStack,
                Object state,
                IMessage message,
                ITransportHeaders headers,
                Stream stream)
        {
            ExtractTransportHeaders(headers);
            // Get sessionId from client or generate new sassionId
            GetSessionIdFromClient(headers);
            
            sinkStack.AsyncProcessResponse(message, headers, stream);
        }

        public Stream GetResponseStream(
                IServerResponseChannelSinkStack sinkStack,
                Object state,
                IMessage message,
                ITransportHeaders headers)
        {
            return null;
        }

        public ServerProcessing ProcessMessage(
                IServerChannelSinkStack sinkStack,
                IMessage requestMsg,
                ITransportHeaders requestHeaders,
                Stream requestStream,
                out IMessage responseMsg,
                out ITransportHeaders responseHeaders,
                out Stream responseStream)
        {
            if (NextChannelSink != null)
            {
                ExtractTransportHeaders(requestHeaders);
                // Get sessionId from client or generate new sassionId
                string sessionId = GetSessionIdFromClient(requestHeaders);

                ServerProcessing spres = NextChannelSink.ProcessMessage(
                        sinkStack,
                        requestMsg,
                        requestHeaders,
                        requestStream,
                        out responseMsg,
                        out responseHeaders,
                        out responseStream);

                // Set session id in response headers
                responseHeaders[SESION_ID] = sessionId;

                return spres;
            }

            responseMsg = null;
            responseHeaders = null;
            responseStream = null;

            return new ServerProcessing();
        }
    }
}