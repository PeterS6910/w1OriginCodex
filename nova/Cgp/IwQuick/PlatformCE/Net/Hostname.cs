using System;
using System.Text.RegularExpressions;

using System.Net;
using System.Net.Sockets;

using System.Diagnostics;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    public class Hostname
    {
        public static bool IsValid(string hostname)
        {
            if (string.IsNullOrEmpty(hostname))
                return false;

            Regex aRegex;
            try
            {
                aRegex = new Regex(@"^[0-9a-zA-Z\.]{1,126}$");
            }
            catch (Exception aError)
            {
                Debug.Assert(false,aError.Message);
                return false;
            }

            return (aRegex.Match(hostname).Success);
        }

        public static void CheckValidity(string hostname)
        {
            if (!IsValid(hostname))
                throw new InvalidHostnameException(hostname);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostname"></param>
        /// <returns></returns>
        public static IPAddress[] LookupIPs(string hostname)
        {
            if (!IsValid(hostname))
                throw new InvalidHostnameException(hostname);

            try
            {
                IPAddress[] arIPs = null;/* Dns.GetHostAddresses(hostname);
                if (0 == arIPs.Length)
                    throw new DnsLookupException(hostname);*/
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
                
                DebugHelper.TryBreak(aSockError);
                return null;
            }
            catch (DnsLookupException)
            {
                throw;
            }
            catch (Exception aError) 
            { 
                Debug.Assert(false,aError.Message);
                return null;
            }

        }

        private struct TLookupState
        {
            public DIpEventResolve _callback;
// ReSharper disable once NotAccessedField.Local
            public string _hostname;
        }

        public static void LookupIPsAsync(
            [NotNull] string hostname,
            [NotNull] DIpEventResolve callBack)
        {
            if (!IsValid(hostname))
                throw new InvalidHostnameException(hostname);

            Validator.CheckForNull(callBack,"callBack");

#pragma warning disable 219
            TLookupState aState = new TLookupState();
#pragma warning restore 219
            aState._callback = callBack;
            aState._hostname = hostname;

            // TODO : async not finished
            /*
            try
            {
                IAsyncResult aRet = Dns.BeginGetHostAddresses(hostname, new AsyncCallback(LookupCallback), aState);
            }
            catch // (Exception aError)
            {
                throw new DnsLookupException(hostname);
            }*/
        }

        private static void LookupCallback(IAsyncResult result)
        {
            TLookupState aState = (TLookupState)result.AsyncState;
            if (null == aState._callback)
// ReSharper disable once RedundantJumpStatement
                return;

            // TODO : not finished
            /*
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
            }*/
        }
    }
}
