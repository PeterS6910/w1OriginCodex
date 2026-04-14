//#define DEBUG_OUTPUTS

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Threading;
using Contal.IwQuick.Net.Microsoft;
using Contal.IwQuick.Data;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    /// <summary>
    /// class for secure lookup of entities over network via UDP protocol
    /// </summary>
    /// CE
    public partial class CNMPAgent: SimpleUdpPeer
    {
        

        private TimerCallback _tcb;
        private Timer _timerNew;

        /// <summary>
        /// creates instance of the UDP lookup entity allowing lookups and also identifying itself
        /// </summary>
        /// <param name="ceCeNetworkManagement"></param>
        /// <param name="udpPort">UDP port to run on</param>
        /// <param name="instanceName">string specificator of this entity instance</param>
        /// <param name="typeName">string specificator of this entity type</param>
        /// <param name="keys"></param>
        /// <exception cref="ArgumentNullException">if instance name or type name is null or empty string</exception>
        public CNMPAgent(
            [NotNull] CeNetworkManagement ceCeNetworkManagement, 
            int udpPort, 
            string instanceName, 
            [NotNull] string typeName, 
            params string[] keys )
            :base(false,(udpPort > 0 ? udpPort : DefaultCnmpPort))
        {
            Validator.CheckForNull(ceCeNetworkManagement,"ceCeNetworkManagement");
            _ceCeNetworkManagement = ceCeNetworkManagement;

            ConstructorCore(udpPort,instanceName,typeName,keys);
           
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="ceCeNetworkManagement"></param>
        /// <param name="instanceName"></param>
        /// <param name="typeName"></param>
        public CNMPAgent(
            [NotNull] CeNetworkManagement ceCeNetworkManagement,
            string instanceName, 
            string typeName)
            :this(ceCeNetworkManagement,0,instanceName,typeName,null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ceCeNetworkManagement"></param>
        /// <param name="typeName"></param>
        public CNMPAgent(
            [NotNull] CeNetworkManagement ceCeNetworkManagement,
            string typeName)
            : this(ceCeNetworkManagement,0, null, typeName, null)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ceCeNetworkManagement"></param>
        /// <param name="instanceName"></param>
        /// <param name="typeName"></param>
        /// <param name="keys"></param>
        public CNMPAgent(
            [NotNull] CeNetworkManagement ceCeNetworkManagement,
            string instanceName, 
            string typeName, 
            params string[] keys)
            : this(ceCeNetworkManagement,0, instanceName, typeName, keys)
        {
        }

       

        /// <summary>
        /// starts CNMP responding on default or explicit port
        /// </summary>
        public override void Start()
        {
            base.Start();
            _tcb = OnRequestTimeout;
            if (!AddMulticastMembership(MulticastIp))
            {         
                // post activation, if it was started before cable/line connected       
                //System.Net.NetworkInformation.NetworkChange.NetworkAddressChanged += 
                //    new System.Net.NetworkInformation.NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
            }
            /*else
            {
                _multicastResolved = true;
            }*/
        }

        /*
        private void NetworkChange_NetworkAddressChanged(
            object sender, 
            EventArgs e)
        {
            if (IsListening)
            {
                if (!_multicastResolved)
                    _multicastResolved = AddMulticastMembership(_mcastIP);
            }
            else
            {
                _multicastResolved = false;
            }
        }*/


        

        


        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="lookupInfo"></param>
        private void Identify(IPEndPoint ipEndpoint, LookupInfo lookupInfo)
        {
            var xmlResponse = ConceptResponse(lookupInfo,ipEndpoint);

            var replyAsBroadcast = false;

            try
            {
                // FIND, IF THE REQUESTEE IS FROM OUR NETWORK
                var deviceNames = CeNetworkManagement.GetDeviceNames();

                if (deviceNames != null &&
                    deviceNames.Length > 0)
                {
                    var found = false;
                    foreach (var dn in deviceNames)
                    {
                        bool isDhcp;
                        string ip, mask, gw;

                        if (!_ceCeNetworkManagement.GetIPSettings(dn, out isDhcp, out ip, out mask, out gw)) 
                            continue;

                        try
                        {
                            if (!IPHelper.IsSameNetwork4(ip, mask, ipEndpoint.Address.ToString())) 
                                continue;

                            found = true;
                            break;
                        }
                        catch
                        {
                        }
                    }

                    if (!found)
                        replyAsBroadcast = true;
                }
            }
            catch
            {
            }

            // IF THE REQUESTEE IS FROM NONE OF OUR NETWORKS , RESPOND VIA BROADCAST
            var ipe = replyAsBroadcast ? new IPEndPoint(IPAddress.Broadcast, ipEndpoint.Port) : ipEndpoint;

            Send(ipe,new ByteDataCarrier(xmlResponse));
        }

        

        private static int _requestCounter = 0;

        private readonly CeNetworkManagement _ceCeNetworkManagement;

        private LookupInfo BeginLookup(
            CNMPLookupType lookupType, 
            [NotNull] string value,
            bool isBlocking,
            string[] getExtras,
            CNMPLookupMode lookupMode,
            bool ignoreLocalResponse,
            IPAddress ipDown,
            IPAddress ipUp,
            DCNMPLookupFinished explicitDelegate)
        {
            if (!IsListening)
                throw new InvalidOperationException(
                    "Lookup cannot be started, if the UDP socket was not bound.\n"+
                    "Call the \"Start\" method first");

            Validator.CheckNullString(value);
            Validator.CheckInvalidOperation(lookupType == CNMPLookupType.Unknown);

            var aInfo = new LookupInfo
            {
                LookupType = lookupType,
                Value = value,
                _lookupResult = new CNMPLookupResultList(),
                _lookupMode = lookupMode,
                _unicastIPDown = ipDown,
                _unicastIPUp = ipUp,
                _explicitDelegate = explicitDelegate,
                _ignoreLocalResponse = ignoreLocalResponse
            };

            if (null != getExtras &&
                0 < getExtras.Length)
            {
                aInfo._requestExtras = new Dictionary<string, string>();
                foreach (var strKey in getExtras)
                {
                    if (Validator.IsNullString(strKey))
                        continue;

                    aInfo._requestExtras[strKey] = String.Empty;
                }
            }

            if (isBlocking)
            {
                aInfo._waitMutex = new EventWaitHandle(false, EventResetMode.ManualReset);
            }

            SendRequest(aInfo);

            return aInfo;
        }

        

       

       

        private void SendRequest(LookupInfo lookupInfo)
        {
            if (!IsListening)
                throw new InvalidOperationException("Lookup cannot be started, if the UDP socket was not bound");

            lookupInfo._id = _requestCounter++;
            lookupInfo._timeStamp = DateTime.Now;
            ++lookupInfo._retryCount;            
            

            var xmlRequestData = ConceptRequest(lookupInfo);

            _requests[lookupInfo._id] = lookupInfo;

            IPAddress destination;

            switch (lookupInfo._lookupMode)
            {
                case CNMPLookupMode.Multicast:
                    destination = MulticastIp;
                    break;
                //case CNMPLookupMode.Broadcast:
                default:
                    destination = IPAddress.Broadcast;
                    break;
            }

            try
            {                
// ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (Equals(destination, IPAddress.Broadcast))
                {

                    // SEND BROADCAST FOR LOGICALY SEPARATED AGENTS THAT ARE ON THE SAME PHYSICAL NETWORK AS REQUESTEE
#if DEBUG_OUTPUTS
                    Console.WriteLine("CNMP: Sending to " + IPAddress.Broadcast + ":" + _port);
#endif
                    Send(new IPEndPoint(IPAddress.Broadcast, _port), new ByteDataCarrier(xmlRequestData));
                }
                else
                {
#if DEBUG_OUTPUTS
                    Console.WriteLine("CNMP: Sending to " + destination + ":" + _port);
#endif
                    
                    Send(new IPEndPoint(destination, _port), new ByteDataCarrier(xmlRequestData));
                }
            }
            catch(Exception error)
            {
                Sys.HandledExceptionAdapter.Examine(error);
                throw;
            }

            
            //if _retryCount eqals 0 only one send at 0s is made, else one send at 0s an other retries at _timeout / (_retryCount + 1))s
            //example _timeout = 1000 _retryCount = 2  0s, 0.333s, 0.666s time intervas sends are made
            //_timers.StartTimeout(((_retryCount < 1) ? _timeout : _timeout / (_retryCount + 1)), lookupInfo, new DOnTimerEvent(OnRequestTimeout));
            if (_timerNew != null)
            {
                try
                {
                    _timerNew.Dispose();
                    _timerNew = null;
                }
                catch { }
            }
            _timerNew = new Timer(_tcb, lookupInfo, ((_retryCount < 1) ? _timeout : _timeout / (_retryCount + 1)), System.Threading.Timeout.Infinite);
        }      

        private void OnRequestTimeout(object info)
        {
            try
            {
                var lookupInfo = (LookupInfo)info;

                // do not stop searching after first successfull find
                //if (_requests.ContainsKey(aInfo._id))
                //{
                // do not condition this ... actual timer can vary
                // if the timer has reached this state, it means it's timeout
                //IsTimedOut(aInfo);

                //if (_requests.ContainsKey(lookupInfo._id))//*
                //    _requests.Remove(lookupInfo._id);//*

                if (lookupInfo._retryCount < _retryCount)
                {
#if DEBUG_OUTPUTS
                    Console.WriteLine("CNMP: retrying ... " + (lookupInfo._retryCount + 1));
#endif
                    SendRequest(lookupInfo);
                }
                else
                {
                    //if (_requests.ContainsKey(lookupInfo._id))//*
                    //    _requests.Remove(lookupInfo._id);//*

                    if (lookupInfo._lookupResult.Count == 0)
                        FireTimedOut(lookupInfo);
                    else
                        FireIdentified(lookupInfo);

                    if (null != lookupInfo._waitMutex)
                        lookupInfo._waitMutex.Set();
                    _requests.Clear();
                }
                //}

            }
            catch
            {
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public string MachineName {get { return ExtendedEnvironment.MachineName; }}

        


        
        ~CNMPAgent()
        {
            if (IsListening)
                try { RemoveMulticastMembership(MulticastIp); }
                catch { }
        }

        
    }
}

