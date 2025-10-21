using System;
using System.Text.RegularExpressions;

using System.Net;
using System.Net.Sockets;

using System.Diagnostics;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    public class Hostname
    {
        private static readonly Regex _hostnameRegex = new Regex(@"^[0-9a-zA-Z\.]{1,126}$");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public static bool IsValid(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
                return false;

            return (_hostnameRegex.Match(hostname).Success);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostname"></param>
        public static void CheckValidity(string hostname)
        {
            if (!IsValid(hostname))
                throw new InvalidHostnameException(hostname);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostname"></param>
        /// <exception cref="InvalidHostnameException"></exception>
        /// <exception cref="DnsLookupException"></exception>
        /// <returns></returns>
        public static IPAddress[] LookupIPs(string hostname)
        {
            if (!IsValid(hostname))
                throw new InvalidHostnameException(hostname);

            try
            {
                var arIPs = Dns.GetHostAddresses(hostname);
                if (0 == arIPs.Length)
                    // not caught in this method
                    throw new DnsLookupException(hostname);

                return arIPs;
            }
            catch (ArgumentException)
            {
                throw new InvalidHostnameException(hostname);
            }
            catch (SocketException aSockError)
            {
                if (aSockError.ErrorCode == 11001)
                    throw new DnsLookupException(hostname);
                
                Debug.Assert(false,aSockError.Message);
                return null;
            }
            catch (Exception aError) 
            { 
                Debug.Assert(false,aError.Message);
                return null;
            }

        }

        private struct TLookupState
        {
            public DIpResolved _callback;
            public string _hostname;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostname"></param>
        /// <param name="callBack"></param>
        /// <exception cref="InvalidHostnameException"></exception>
        public static void LookupIPsAsync([NotNull] string hostname,[NotNull] DIpResolved callBack)
        {
            if (!IsValid(hostname))
                throw new InvalidHostnameException(hostname);

            Validator.CheckForNull(callBack,"callBack");

            TLookupState aState = new TLookupState {_callback = callBack, _hostname = hostname};

            try
            {
                IAsyncResult aRet = Dns.BeginGetHostAddresses(hostname, LookupCallback, aState);
                DebugHelper.Keep(aRet);
            }
            catch // (Exception aError)
            {
                throw new DnsLookupException(hostname);
            }
        }

        private static void LookupCallback(IAsyncResult result)
        {
            TLookupState aState = (TLookupState)result.AsyncState;
            if (null == aState._callback)
                return;

            try
            {
                IPAddress[] aIPs = Dns.EndGetHostAddresses(result);
                if (null == aIPs ||
                    0 == aIPs.Length)
                    aState._callback(aState._hostname, null);
                else
                    aState._callback(aState._hostname, aIPs);
            }
            catch (SocketException aSockError)
            {
                aState._callback(aState._hostname, null);

                if (aSockError.ErrorCode != 11001)
                    Debug.Assert(false,aSockError.Message);
                    
            }
            catch (Exception aError)
            {
                aState._callback(aState._hostname, null);

                Debug.Assert(false, aError.Message);
            }
        }
    }
}
