using System;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;
using System.Net;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Xml;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

namespace Contal.IwQuick.Net
{
    // CROSS
    public partial class CNMPAgent
    {
        private const string DefaultMulticastAddress = "239.192.0.63";
        private const int DefaultCnmpPort = 34000;
        private const CNMPLookupMode DefaultLookupMode = CNMPLookupMode.Broadcast;
        private static readonly IPAddress LoopbackBroadcast = IPAddress.Parse("127.255.255.255");

        private static readonly IPAddress MulticastIp = IPAddress.Parse(DefaultMulticastAddress);

        private readonly SyncDictionary<int, LookupInfo> _requests = new SyncDictionary<int, LookupInfo>(2);

        /// <summary>
        /// 
        /// </summary>
        private class LookupInfo
        {
            internal LookupInfo()
            {
                
            }

            internal LookupInfo([NotNull] ParsedXmlValues parsedXmlValues)
            {
                _parsedXmlValues = parsedXmlValues;
                _id = parsedXmlValues._id;
                _timeStamp = DateTime.Now;
            }

            internal int _id = -1;
            internal DateTime _timeStamp;
            internal string TimeStampString
            {
                get { return UnifiedFormat.GetDateTime(_timeStamp); }
            }

            private CNMPLookupType _lookupType = CNMPLookupType.Unknown;

            /// <summary>
            /// 
            /// </summary>
            internal CNMPLookupType LookupType
            {
                get
                {
                    if (_parsedXmlValues != null)
                        return _parsedXmlValues._lookupType;


                    if (_lookupType != CNMPLookupType.Unknown)
                        return _lookupType;

                    
                    return CNMPLookupType.Unknown;
                }
                set { _lookupType = value; }
            }

            private string _value = null;

            internal string Value
            {
                get
                {
                    if (_parsedXmlValues != null)
                        return _parsedXmlValues._lookupValue;

                    return _value;
                }
                set { _value = value; }
            }


            internal int _retryCount = -1;
            internal EventWaitHandle _waitMutex;
            internal CNMPLookupMode _lookupMode = DefaultLookupMode;
            internal IPAddress _unicastIPDown = null;
            internal IPAddress _unicastIPUp = null;
            internal DCNMPLookupFinished _explicitDelegate = null;

            internal CNMPLookupResultList _lookupResult = null;
            internal Dictionary<string, string> _requestExtras = null;
            internal bool _ignoreLocalResponse = true;

            private readonly ParsedXmlValues _parsedXmlValues = null;

            private CnmpHashType _hashType;
            private bool _hashTypeDefined = false;

            /// <summary>
            /// 
            /// </summary>
            internal CnmpHashType HashType
            {
                get
                {
                    if (_parsedXmlValues != null)
                        return _parsedXmlValues._hashType;

                    if (_hashTypeDefined)
                        return _hashType;

                    // backward compatibility
                    return CnmpHashType.Sha1;
                    
                }
                set
                {
                    _hashType = value;
                    _hashTypeDefined = true;
                }
            }
        }

        private string _identificationHash = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public string IdentificationHash
        {
            get { return _identificationHash; }
        }

        private bool _isMasterInstance = true;
        public bool IsMasterInstance
        {
            get { return _isMasterInstance; }
        }

        private int _port = DefaultCnmpPort;
        private int _timeout = 2500;

        private string _instanceName = null;
        /// <summary>
        /// 
        /// </summary>
        public string InstanceName
        {
            get { return _instanceName; }
        }

        private string _typeName = null;
        /// <summary>
        /// 
        /// </summary>
        public string TypeName
        {
            get { return _typeName; }
        }

        private bool _allowInternalForward = true;
        public bool AllowInternalForward
        {
            get
            {
                return _allowInternalForward;
            }
            set
            {
                _allowInternalForward = value;
            }
        }

       
        private int _keyCounter = (int)DateTime.Now.Ticks;
        private readonly SyncDictionary<string, int> _keys = new SyncDictionary<string, int>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns>0 if problem occured</returns>
        public int NewKey([NotNull] string key)
        {
            if (Validator.IsNullString(key))
                return 0;

            int newId = 0;

            _keys.GetOrAddValue(key,
                s =>
                {
                    newId = ++_keyCounter;
                    return newId;
                }, null
                );

            return newId;
        }

        private string _contalVersion = null;
        /// <summary>
        /// 
        /// </summary>
        public string ContalVersion
        {
            get
            {
                return _contalVersion;
            }
            set
            {
                Validator.CheckNullString(value);

                _contalVersion = value;
            }
        }

        private class CNMPExtra
        {
            internal string _name;
            internal string _value;
            internal readonly bool _onDemandOnly = true;

            internal CNMPExtra(string name, string value, bool onDemandOnly)
            {
                _name = name;
                _value = value ?? String.Empty;
                _onDemandOnly = onDemandOnly;
            }

            // ReSharper disable once UnusedMember.Local
            internal CNMPExtra(string name, string value)
                : this(name, value, true)
            {

            }
        }

        private volatile SyncDictionary<string, CNMPExtra> _extras = null;
        private readonly object _syncExtrasInstance = new object();


        /// <summary>
        /// sets extra parameters to be transfered in the response,
        /// they're defined as string-to-string key-value pair
        /// </summary>
        /// <param name="paramName">name of the extra parameter</param>
        /// <param name="paramValue">value of the extra parameter</param>
        /// <param name="onDemandOnly"></param>
        /// <exception cref="ArgumentNullException">if the name of the extra parameter is null or empty</exception>
        public void SetExtra(
            [NotNull]
            string paramName,
            string paramValue,
            bool onDemandOnly)
        {
            Validator.CheckNullString(paramName, "paramName");

            if (null == _extras)
                lock (_syncExtrasInstance)
                {
                    if (null == _extras)
                        _extras = new SyncDictionary<string, CNMPExtra>();
                }

            CNMPExtra ev;
            if (_extras.TryGetValue(paramName, out ev))
            {
                ev._name = paramName;
                ev._value = paramValue;
            }
            else
            {
                _extras[paramName] = new CNMPExtra(paramName, paramValue, onDemandOnly);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        public void SetExtra(
            [NotNull]
            string paramName,
            string paramValue)
        {
            SetExtra(paramName, paramValue, true);
        }

#pragma warning disable 169
        private volatile bool _multicastResolved = false;
#pragma warning restore 169



        /// <summary>
        /// timeout for the lookup per retry in miliseconds, default is 1000,
        /// can be set within range of 1-60000 ms
        /// </summary>
        public int Timeout
        {
            get { return _timeout; }
            set
            {
                if (value > 0 && value <= 60000)
                    _timeout = value;
            }
        }

        private int _retryCount = 2;
        private bool _backwardCompatibilityWith2 = true;

        /// <summary>
        /// number of lookup retries trough timeout, default is 2
        /// </summary>
        public int RetryCount
        {
            get { return _retryCount; }
            set
            {
                if (value >= 0 && value < 100)
                    _retryCount = value;
            }
        }

        private void ConstructorCore( 
            int udpPort, 
            string instanceName, 
            [NotNull] string typeName, 
            params string[] keys)
        {
            Validator.CheckNullString(typeName, "typeName");

            if (udpPort > 0)
            {
                TcpUdpPort.CheckValidity(udpPort);
                _port = udpPort;
            }

            if (Validator.IsNotNullString(instanceName))
                _instanceName = instanceName;

            _typeName = typeName;

            var r = new Random((int)DateTime.Now.Ticks);

            _identificationHash = QuickHashes.GetSHA1String(
                string.Concat(
                    DateTime.Now.Ticks.ToString(CultureInfo.InvariantCulture),
                    r.Next(int.MaxValue),
                    Thread.CurrentThread.ManagedThreadId,
                    _port,
                    Process.GetCurrentProcess().Id));

            if (null != keys &&
                keys.Length > 0)
                foreach (var k in keys)
                    NewKey(k);

            AllowBroadcast = true;
            AllowReuseAddress = true;
        }

        private class ParsedXmlValues
        {
            public string _rootNodeName;
            public string _identHash;
            public string _callerType;
            public string _callerInstance;
            public int _id;
            public CNMPLookupType _lookupType;
            public string _lookupValue;
            public string _ts;
            public Dictionary<string, string> _extrasToGet = null;
            public Dictionary<string, string> _extrasReceived = null;

            public string _signature;
            // ReSharper disable once NotAccessedField.Local
#pragma warning disable 169
            public bool _notForThisRequestee = false;
#pragma warning restore 169
            public bool _isForwarded = false;
            public string _forwardersIdentHash;
            public IPAddress _forwardedIp;
            public int _forwardedPort;
            public CnmpHashType _hashType;
        }

        private string GetCurrentParentInPath(LinkedList<string> rootPath)
        {
            if (rootPath == null ||
                rootPath.Last == null)
                return null;

            return rootPath.Last.Value;
        }



        private void RegisterParentInPath(LinkedList<string> rootPath, XmlReader currentXmlNode)
        {
            if (null == rootPath || currentXmlNode == null)
                return;

            if (currentXmlNode.NodeType != XmlNodeType.Element ||
                currentXmlNode.IsEmptyElement)
                return;

            lock (rootPath)
            {
                rootPath.AddLast(currentXmlNode.Name);
            }
        }

        private void UnregisterParentInPath(LinkedList<string> rootPath, XmlReader currentXmlNode)
        {
            if (rootPath == null ||
                null == currentXmlNode ||
                (currentXmlNode.NodeType != XmlNodeType.Element && currentXmlNode.NodeType != XmlNodeType.EndElement))
                return;

            lock (rootPath)
            {
                if (rootPath.Last != null && currentXmlNode.Name == rootPath.Last.Value)
                {
                    rootPath.RemoveLast();
                    //Console.WriteLine("XML PATH remove : " + ReportPath(currentRootPath));
                }
            }
        }

        private ParsedXmlValues ParseXmlBySax(ByteDataCarrier data)
        {
            if (null == data || data.ActualSize == 0)
                return null;

#if DEBUG
            var swx = Stopwatch.StartNew();
#endif
            var xmlReader = XmlReader.Create(data.ToStream());

            var parsedXmlValues = new ParsedXmlValues();

            var xmlRootPath = new LinkedList<string>();

            if (xmlReader.IsEmptyElement)
                return null;
            try
            {
                var foundFirstElement = false;
                while (!foundFirstElement)
                {
                    if (!xmlReader.Read())
                        return null;

                    foundFirstElement = (xmlReader.NodeType == XmlNodeType.Element);

                }

                parsedXmlValues._rootNodeName = xmlReader.Name;
                xmlRootPath.AddLast(parsedXmlValues._rootNodeName);

                while (xmlReader.Read())
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            switch (xmlReader.Name)
                            {
                                case CnmpProtocol.IDENT_HASH_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.RequestTagname:
                                        case CnmpProtocol.ResponseTagname:
                                            parsedXmlValues._identHash = xmlReader.ReadString();

                                            if (parsedXmlValues._identHash == null ||
                                                parsedXmlValues._identHash == _identificationHash)
                                                // SPLIT HORIZON CONDITION
                                                return null; // DO NOT PROCEED WITH PARSING OWN REQUESTS
                                            break;
                                        case CnmpProtocol.FORWARDED_FROM_TAGNAME:
                                            parsedXmlValues._forwardersIdentHash = xmlReader.ReadString();
                                            break;
                                    }
                                    break;
                                case CnmpProtocol.CALLER_TYPE_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.RequestTagname:
                                        case CnmpProtocol.ResponseTagname:
                                            parsedXmlValues._callerType = xmlReader.ReadString();
                                            break;
                                    }
                                    break;
                                case CnmpProtocol.CALLER_INSTANCE_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.RequestTagname:
                                        case CnmpProtocol.ResponseTagname:
                                            parsedXmlValues._callerInstance = xmlReader.ReadString();
                                            break;
                                    }
                                    break;
                                case CnmpProtocol.ID_TAGNAME:
                                    string idText;
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.RequestTagname:
                                            idText = xmlReader.ReadString();
                                            parsedXmlValues._id = int.Parse(idText);
                                            break;
                                        case CnmpProtocol.ResponseTagname:
                                            idText = xmlReader.ReadString();
                                            parsedXmlValues._id = int.Parse(idText);

                                            if (!_requests.ContainsKey(parsedXmlValues._id))
#if COMPACT_FRAMEWORK
                                                // DO NOT PARSE MORE IF IT'S NOT THIS AGENT'S REQUEST
                                                return null;
#else
                                                // PARSE MORE TO ALLOW FORWARDING
                                                parsedXmlValues._notForThisRequestee = true;
#endif
                                            break;
                                    }
                                    break;
                                case CnmpProtocol.LOOKUP_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.RequestTagname:
                                        case CnmpProtocol.ResponseTagname:
                                            var lookupTypeText = xmlReader.GetAttribute("type");

                                            parsedXmlValues._lookupType = (CNMPLookupType)int.Parse(lookupTypeText);
                                            parsedXmlValues._lookupValue = xmlReader.ReadString();
                                            break;
                                    }
                                    break;
                                case CnmpProtocol.TS_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.RequestTagname:
                                        case CnmpProtocol.ResponseTagname:
                                            parsedXmlValues._ts = xmlReader.ReadString();
                                            if (Validator.IsNullString(parsedXmlValues._ts))
                                                return null;
                                            break;
                                    }
                                    break;
                                case CnmpProtocol.GET_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.RequestTagname:
                                        case CnmpProtocol.ResponseTagname:
                                            if (null == parsedXmlValues._extrasToGet)
                                                parsedXmlValues._extrasToGet = new Dictionary<string, string>(4);

                                            parsedXmlValues._extrasToGet[xmlReader.ReadString()] = null;
                                            break;
                                    }

                                    break;
                                case CnmpProtocol.FORWARDED_FROM_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.RequestTagname:
                                        case CnmpProtocol.ResponseTagname:
                                            RegisterParentInPath(xmlRootPath, xmlReader);
                                            break;
                                    }

                                    parsedXmlValues._isForwarded = true;
                                    break;
                                case CnmpProtocol.SIGNATURE_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.ResponseTagname:
                                            parsedXmlValues._signature = xmlReader.ReadString();
                                            if (Validator.IsNullString(parsedXmlValues._signature))
                                                return null;
                                            break;
                                    }
                                    break;
                                case CnmpProtocol.RETURN_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.ResponseTagname:
                                            var returnName = xmlReader.GetAttribute("name");

                                            if (parsedXmlValues._extrasReceived == null)
                                                parsedXmlValues._extrasReceived = new Dictionary<string, string>();

                                            parsedXmlValues._extrasReceived[returnName] = xmlReader.ReadString();

                                            break;
                                    }
                                    break;
                                case CnmpProtocol.IP_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.FORWARDED_FROM_TAGNAME:

                                            parsedXmlValues._forwardedIp = IPAddress.Parse(xmlReader.ReadString());
                                            break;
                                    }
                                    break;
                                case CnmpProtocol.PORT_TAGNAME:
                                    switch (GetCurrentParentInPath(xmlRootPath))
                                    {
                                        case CnmpProtocol.FORWARDED_FROM_TAGNAME:

                                            parsedXmlValues._forwardedPort = int.Parse(xmlReader.ReadString());
                                            break;
                                    }
                                    break;
                                default:
                                    RegisterParentInPath(xmlRootPath, xmlReader);
                                    break;

                            }

                            break;
                        case XmlNodeType.EndElement:
                            UnregisterParentInPath(xmlRootPath, xmlReader);

                            break;
                    }
                }
            }
            catch
            {
                return null;
            }

#if DEBUG
            var took = swx.ElapsedMilliseconds;
            DebugHelper.NOP(took);
#endif

            return parsedXmlValues;
        }

        private void ParseExtrasFromRequestXml(ParsedXmlValues parsedXmlValues, LookupInfo lookupInfo)
        {
            if (null == parsedXmlValues)
                return;

            if (null == _extras)
                return;

            if (null == lookupInfo._requestExtras)
                lookupInfo._requestExtras = new Dictionary<string, string>();

            foreach (var ev in _extras.Values)
            {
                if (!ev._onDemandOnly)
                    lookupInfo._requestExtras[ev._name] = ev._value;
            }

            // optionals
            if (null != parsedXmlValues._extrasToGet &&
                parsedXmlValues._extrasToGet.Count > 0)
            {


                foreach (var extraName in parsedXmlValues._extrasToGet.Keys)
                {
                    if (Validator.IsNullString(extraName))
                        continue;


                    CNMPExtra ev;
                    if (_extras != null
                        && _extras.TryGetValue(extraName, out ev)
                        && ev._onDemandOnly)
                        lookupInfo._requestExtras[extraName] = ev._value;

                }
            }
        }

        private void AnalyzeRequest([NotNull] ParsedXmlValues parsedXmlValues, IPEndPoint ipEndpoint)
        {
            /*
            // anti-pass back condition on the box, that requested lookup
            LookupInfo aLocalRequest;
            if (_requests.TryGetValue(iId, out aLocalRequest) &&
                aLocalRequest._timeStampString == strTs &&
                aLocalRequest._ignoreLocalResponse)
                return;*/

#if DEBUG_OUTPUTS
            Console.WriteLine();
            Console.WriteLine(lookupType.ToString() + " " + (valueToCompare ?? "<none>") + " " + (callerInstance ?? String.Empty) + " " + callerType);
#endif

            var keyId = -1;

            switch (parsedXmlValues._lookupType)
            {
                case CNMPLookupType.Instance:
                    if (null != _instanceName)
                    {
                        if (CompareRequestHash(
                            parsedXmlValues,
                            _instanceName)
                            )
                            parsedXmlValues._lookupValue = _instanceName;
                        else
                            return;
                        break;
                    }
                    return;
                case CNMPLookupType.Type:
                    if (CompareRequestHash(
                        parsedXmlValues,
                        _typeName))
                        parsedXmlValues._lookupValue = _typeName;
                    else
                    {
                        // means it's not in the XPath format
                        if (_typeName.IndexOf('/') != 0)
                            return;

                        // verify also domain search

                        var splitted = _typeName.Split('/');

                        if (null == splitted ||
                            splitted.Length < 2)
                            return;

                        var aggregated = String.Empty;
                        var found = false;

                        for (var i = 1; i < splitted.Length - 1; i++)
                        {
                            aggregated += "/" + splitted[i];

                            if (CompareRequestHash(
                                parsedXmlValues,
                                aggregated))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (!found)
                            return;
                        parsedXmlValues._lookupValue = aggregated;
                    }

                    break;
                case CNMPLookupType.Key:
                    foreach (var pair in _keys)
                    {
                        if (CompareRequestHash(
                            parsedXmlValues,
                            pair.Key))
                        {
                            parsedXmlValues._lookupValue = pair.Key;
                            keyId = pair.Value;
                            break;
                        }
                    }

                    if (keyId == -1)
                        return;


                    break;
                case CNMPLookupType.ComputerName:
                    if (CompareRequestHash(
                            parsedXmlValues,
                            MachineName))
                        parsedXmlValues._lookupValue = MachineName;
                    else
                        return;
                    break;
                default:
                    return;
            }



            var aInfo = new LookupInfo(parsedXmlValues);


            ParseExtrasFromRequestXml(parsedXmlValues, aInfo);

            // at this moment the request is valid AND also request handled
            InvokeValidRequestReceived(
                ipEndpoint,
                parsedXmlValues._lookupType,
                keyId,
                parsedXmlValues._callerInstance,
                parsedXmlValues._callerType,
                aInfo._requestExtras);

            Identify(ipEndpoint, aInfo);


        }

        /// <summary>
        /// asynchronously begins lookup, 
        /// the end of asynchronous operation is notified by calling the LookupSucceeded or LookupTimeout event
        /// </summary>
        /// <param name="lookupType">specifies type of the lookup</param>
        /// <param name="value">value to lookup</param>
        /// <param name="getExtras">variable list of extra parameters to request in response</param>
        /// <exception cref="ArgumentNullException">if the value is null or empty string</exception>
        /// <exception cref="InvalidOperationException">if the lookup type is unknown</exception>
        public void BeginLookup(
            CNMPLookupType lookupType,
            [NotNull] string value,
            params string[] getExtras)
        {
            BeginLookup(lookupType, value, false, getExtras, DefaultLookupMode, true, null, null, null);
        }

        /// <summary>
        /// asynchronously begins lookup, 
        /// the end of asynchronous operation is notified by calling the LookupSucceeded or LookupTimeout event
        /// </summary>
        /// <param name="onLookupFinished"></param>
        /// <param name="lookupType">specifies type of the lookup</param>
        /// <param name="value">value to lookup</param>
        /// <param name="getExtras">variable list of extra parameters to request in response</param>
        /// <exception cref="ArgumentNullException">if the value is null or empty string</exception>
        /// <exception cref="InvalidOperationException">if the lookup type is unknown</exception>
        public void BeginLookup(
            [NotNull] DCNMPLookupFinished onLookupFinished,
            CNMPLookupType lookupType,
            [NotNull] string value,
            params string[] getExtras)
        {
            Validator.CheckForNull(onLookupFinished, "onLookupFinished");

            BeginLookup(lookupType, value, false, getExtras, DefaultLookupMode, true, null, null, onLookupFinished);
        }

        /// <summary>
        /// asynchronously begins lookup, 
        /// the end of asynchronous operation is notified by calling the LookupSucceeded or LookupTimeout event
        /// </summary>
        /// <param name="onLookupFinished"></param>
        /// <param name="lookupType">specifies type of the lookup</param>
        /// <param name="value">value to lookup</param>
        /// <param name="getExtras">variable list of extra parameters to request in response</param>
        /// <exception cref="ArgumentNullException">if the value is null or empty string</exception>
        /// <exception cref="InvalidOperationException">if the lookup type is unknown</exception>
        public void BeginLookupWithLocals(
            [NotNull] DCNMPLookupFinished onLookupFinished,
            CNMPLookupType lookupType,
            [NotNull] string value,
            params string[] getExtras)
        {
            Validator.CheckForNull(onLookupFinished, "onLookupFinished");

            BeginLookup(lookupType, value, false, getExtras, DefaultLookupMode, false, null, null, onLookupFinished);
        }

        /// <summary>
        /// asynchronously begins lookup, 
        /// the end of asynchronous operation is notified by calling the LookupSucceeded or LookupTimeout event
        /// </summary>
        /// <param name="lookupType">specifies type of the lookup</param>
        /// <param name="value">value to lookup</param>
        /// <param name="getExtras">variable list of extra parameters to request in response</param>
        /// <exception cref="ArgumentNullException">if the value is null or empty string</exception>
        /// <exception cref="InvalidOperationException">if the lookup type is unknown</exception>
        public void BeginBroadcastLookup(CNMPLookupType lookupType, string value, params string[] getExtras)
        {
            BeginLookup(lookupType, value, false, getExtras, DefaultLookupMode, true, null, null, null);
        }

        /// <summary>
        /// asynchronously begins lookup,
        /// the end of asynchronous operation is notified by calling the LookupSucceeded or LookupTimeout event
        /// </summary>
        /// <param name="lookupType">specifies type of the lookup</param>
        /// <param name="value">value to lookup</param>
        /// <exception cref="ArgumentNullException">if the value is null or empty string</exception>
        /// <exception cref="InvalidOperationException">if the lookup type is unknown</exception>
        public void BeginLookup(
            CNMPLookupType lookupType,
            [NotNull] string value)
        {
            BeginLookup(lookupType, value, null);
        }

        /// <summary>
        /// begins synchronous lookup
        /// </summary>
        /// <param name="lookupType">specifies type of the lookup</param>
        /// <param name="value">value to lookup</param>
        /// <param name="getExtras">variable list of extra parameters to request in response</param>
        /// <exception cref="ArgumentNullException">if the value is null or empty string</exception>
        /// <exception cref="InvalidOperationException">if the lookup type is unknown</exception>
        /// <returns>list of results found</returns>
        public CNMPLookupResultList Lookup(
            CNMPLookupType lookupType,
            [NotNull] string value,
            params string[] getExtras)
        {
            // false means, that mutex is blocking
            var aInfo = BeginLookup(lookupType, value, true, getExtras, DefaultLookupMode, true, null, null, null);
            aInfo._waitMutex.WaitOne();
            return aInfo._lookupResult;
        }

        /// <summary>
        /// begins synchronous lookup
        /// </summary>
        /// <param name="lookupType">specifies type of the lookup</param>
        /// <param name="value">value to lookup</param>
        /// <exception cref="ArgumentNullException">if the value is null or empty string</exception>
        /// <exception cref="InvalidOperationException">if the lookup type is unknown</exception>
        /// <returns>list of results found</returns>
        public CNMPLookupResultList Lookup(
            CNMPLookupType lookupType,
            [NotNull] string value)
        {
            return Lookup(lookupType, value, null);
        }

        /// <summary>
        /// begins synchronous lookup
        /// </summary>
        /// <param name="lookupType">specifies type of the lookup</param>
        /// <param name="value">value to lookup</param>
        /// <param name="getExtras">variable list of extra parameters to request in response</param>
        /// <exception cref="ArgumentNullException">if the value is null or empty string</exception>
        /// <exception cref="InvalidOperationException">if the lookup type is unknown</exception>
        /// <returns>list of results found</returns>
        public CNMPLookupResultList MulticastLookup(
            CNMPLookupType lookupType,
            [NotNull] string value,
            params string[] getExtras)
        {
            // false means, that mutex is blocking
            var aInfo = BeginLookup(lookupType, value, true, getExtras, CNMPLookupMode.Multicast, true, null, null, null);
            aInfo._waitMutex.WaitOne();
            return aInfo._lookupResult;
        }

        /// <summary>
        /// notifies successful end of asynchronous lookup,
        /// means, that at least one entity found during the retries*timeout period
        /// </summary>
        public event DCNMPLookupFinished LookupFinished = null;
        private void FireIdentified(LookupInfo lookupInfo)
        {
            if (null != LookupFinished)
            {
                try
                {
                    LookupFinished(
                        lookupInfo.LookupType,
                        lookupInfo.Value,
                        lookupInfo._lookupResult);

                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);

                }
            }

            if (null != lookupInfo._explicitDelegate)
                try
                {
                    lookupInfo._explicitDelegate(
                        lookupInfo.LookupType,
                        lookupInfo.Value,
                        lookupInfo._lookupResult);
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
        }

        /// <summary>
        /// notifies end of asynchronous lookup on timeout,
        /// means, that no entities were found during the retries*timeout period
        /// </summary>
        private void FireTimedOut(LookupInfo lookupInfo)
        {
            if (lookupInfo._retryCount < _retryCount)
                return;

            if (null != LookupFinished)
            {
                try
                {
                    LookupFinished(
                        lookupInfo.LookupType,
                        lookupInfo.Value,
                        null
                    );

                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }
            }

            if (null != lookupInfo._explicitDelegate)
                try
                {
                    lookupInfo._explicitDelegate(
                        lookupInfo.LookupType,
                        lookupInfo.Value,
                        null);
                }
                catch (Exception e)
                {
                    HandledExceptionAdapter.Examine(e);
                }

        }

        /// <summary>
        /// concepts XML request for CNMP
        /// </summary>
        /// <param name="lookupInfo"></param>
        /// <returns></returns>
        private string ConceptRequest(LookupInfo lookupInfo)
        {
            StringBuilder stringBuilder = StringBuilderPool.Implicit2k.Get();

            

            stringBuilder.Length = 0;

            stringBuilder.Append(CnmpProtocol.XML_HEADER);
            NiceLineBreaking(stringBuilder);
            stringBuilder.Append(CnmpProtocol.RequestStarttag);
            NiceLineBreaking(stringBuilder);

            stringBuilder.Append(CnmpProtocol.CALLER_TYPE_STARTTAG);
            stringBuilder.Append(_typeName);
            stringBuilder.Append(CnmpProtocol.CALLER_TYPE_ENDTAG);
            NiceLineBreaking(stringBuilder);

            if (null != _instanceName)
            {
                stringBuilder.Append(CnmpProtocol.CALLER_INSTANCE_STARTTAG);
                stringBuilder.Append(_instanceName);
                stringBuilder.Append(CnmpProtocol.CALLER_INSTANCE_ENDTAG);
                NiceLineBreaking(stringBuilder);
            }

            stringBuilder.Append(CnmpProtocol.ID_STARTTAG);
            stringBuilder.Append(lookupInfo._id);
            stringBuilder.Append(CnmpProtocol.ID_ENDTAG);
            NiceLineBreaking(stringBuilder);

            stringBuilder.Append(CnmpProtocol.LOOKUP_STARTTAG(lookupInfo.LookupType));
            stringBuilder.Append(GetRequestHash(CnmpHashType.Crc32, lookupInfo));
            stringBuilder.Append(CnmpProtocol.LOOKUP_ENDTAG);
            NiceLineBreaking(stringBuilder);

            if (_backwardCompatibilityWith2)
            {
                stringBuilder.Append(CnmpProtocol.LOOKUP_STARTTAG(lookupInfo.LookupType));
                stringBuilder.Append(GetRequestHash(CnmpHashType.Sha1, lookupInfo));
                stringBuilder.Append(CnmpProtocol.LOOKUP_ENDTAG);
                NiceLineBreaking(stringBuilder);

            }


            stringBuilder.Append(CnmpProtocol.TS_STARTTAG);
            stringBuilder.Append(lookupInfo.TimeStampString);
            stringBuilder.Append(CnmpProtocol.TS_ENDTAG);
            NiceLineBreaking(stringBuilder);

            stringBuilder.Append(CnmpProtocol.IDENT_HASH_STARTTAG);
            stringBuilder.Append(_identificationHash);
            stringBuilder.Append(CnmpProtocol.IDENT_HASH_ENDTAG);
            NiceLineBreaking(stringBuilder);

            if (null != lookupInfo._requestExtras &&
                lookupInfo._requestExtras.Count > 0)
            {
                foreach (string extraKey in lookupInfo._requestExtras.Keys)
                {
                    stringBuilder.Append(CnmpProtocol.GET_STARTTAG);
                    stringBuilder.Append(extraKey);
                    stringBuilder.Append(CnmpProtocol.GET_ENDTAG);
                    NiceLineBreaking(stringBuilder);
                }
            }

            stringBuilder.Append(CnmpProtocol.RequestEndtag);

            string stringRequest = stringBuilder.ToString();

            StringBuilderPool.Implicit2k.Return(stringBuilder);

            return stringRequest;
        }

        [Conditional("DEBUG")]
        private static void NiceLineBreaking(StringBuilder stringBuilder)
        {
            stringBuilder.Append(StringConstants.CR_LF);
        }


        private void AppendReturn(StringBuilder sb, string returnName, string value, string encryptionKey, string timeStampString)
        {
            if (null == sb ||
                Validator.IsNullString(returnName) ||
                value == null ||
                Validator.IsNullString(timeStampString))
                return;

            sb.Append(CnmpProtocol.RETURN_STARTTAG(returnName));
            sb.Append(QuickCrypto.Encrypt(value, encryptionKey, timeStampString));
            sb.Append(CnmpProtocol.RETURN_ENDTAG);
            sb.Append(CnmpProtocol.CR_LF);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lookupInfo"></param>
        /// <param name="ipEndpoint"></param>
        /// <returns></returns>
        private string ConceptResponse(
            [NotNull] LookupInfo lookupInfo,
            // ReSharper disable once UnusedParameter.Local
            IPEndPoint ipEndpoint)
        {
            

            var responseSb = StringBuilderPool.Implicit2k.Get();

            responseSb.Append(CnmpProtocol.XML_HEADER);
            NiceLineBreaking(responseSb);

            responseSb.Append(string.Format(CnmpProtocol.ResponseStarttagFormatString, "2.0"));
            NiceLineBreaking(responseSb);

            responseSb.Append(CnmpProtocol.ID_STARTTAG);
            responseSb.Append(lookupInfo._id);
            responseSb.Append(CnmpProtocol.ID_ENDTAG);
            NiceLineBreaking(responseSb);

            responseSb.Append(CnmpProtocol.TS_STARTTAG);
            responseSb.Append(lookupInfo.TimeStampString);
            responseSb.Append(CnmpProtocol.TS_ENDTAG);

            //response.Append(FORWARDED_FROM_STARTTAG);
            ////empty - message is no forwarded            
            //response.Append(FORWARDED_FROM_ENDTAG);
            //response.Append(CR_LF);

            responseSb.Append(CnmpProtocol.SIGNATURE_STARTTAG);
            responseSb.Append(GetRepsonseHash(lookupInfo));
            responseSb.Append(CnmpProtocol.SIGNATURE_ENDTAG);
            NiceLineBreaking(responseSb);

            

            AppendReturn(responseSb, "instance", _instanceName, lookupInfo.Value, lookupInfo.TimeStampString);
            AppendReturn(responseSb, "type", _typeName, lookupInfo.Value, lookupInfo.TimeStampString);
            AppendReturn(responseSb, "computername", MachineName, lookupInfo.Value, lookupInfo.TimeStampString);
            AppendReturn(responseSb, "contal/version", _contalVersion, lookupInfo.Value, lookupInfo.TimeStampString);


            //s = extras.ToString();

            if (null != lookupInfo._requestExtras &&
                lookupInfo._requestExtras.Count > 0)
            {
                foreach (var pair in lookupInfo._requestExtras)
                {
                    try
                    {
                        AppendReturn(responseSb, pair.Key, pair.Value, lookupInfo.Value, lookupInfo.TimeStampString);

                    }
                    catch
                    {
                        // if encryption failed, forget about it
                    }
                }
            }

            
            //--- apend hash to identify agent reslults
            responseSb.Append(CnmpProtocol.IDENT_HASH_STARTTAG);
            responseSb.Append(_identificationHash);
            responseSb.Append(CnmpProtocol.IDENT_HASH_ENDTAG);
            //---

            responseSb.Append(CnmpProtocol.ResponseEndtag);

            try
            {
                return responseSb.ToString();

            }
            finally
            {
                StringBuilderPool.Implicit2k.Return(responseSb);
            }
        }

        private string GetRequestHash(CnmpHashType hashType,int requestId, CNMPLookupType lookupType, string timeStamp, string value)
        {
            var srcToHash = requestId +
                            (int) lookupType +
                            timeStamp +
                            value;
            
            switch (hashType)
            {
                case CnmpHashType.Crc32:
                    return QuickHashes.GetCRC32String(srcToHash);
                default:
                    return QuickHashes.GetSHA1String(srcToHash);
            }
            
                    
        }

        enum CnmpHashType
        {
            Sha1,
            Crc32
        }

        private bool CompareRequestHash(
            ParsedXmlValues parsedXmlValues,
            string value)
        {
            var hashType= GetHashType(parsedXmlValues._lookupValue);

           
            parsedXmlValues._hashType = hashType;

            return
                GetRequestHash(
                    hashType, 
                    parsedXmlValues._id, 
                    parsedXmlValues._lookupType, 
                    parsedXmlValues._ts,
                    value) == parsedXmlValues._lookupValue;
        }

        private static CnmpHashType GetHashType(string hashedLookupValue)
        {
            CnmpHashType hashType;
            switch (hashedLookupValue.Length)
            {
                case 8:
                    hashType = CnmpHashType.Crc32;
                    break;
                default:
                    hashType = CnmpHashType.Sha1;
                    break;
            }

            return hashType;
        }

        private string GetRequestHash(CnmpHashType hashType,LookupInfo lookupInfo)
        {
            return GetRequestHash(
                hashType,
                lookupInfo._id,
                lookupInfo.LookupType,
                lookupInfo.TimeStampString,
                lookupInfo.Value);
        }

        private string GetRepsonseHash(LookupInfo lookupInfo)
        {
            return
                GetRepsonseHash(lookupInfo.HashType,lookupInfo, lookupInfo.TimeStampString);


        }

        private bool CompareResponseHash(string hashedValue, LookupInfo lookupInfo, string timeStampString)
        {
            var hashType = GetHashType(hashedValue);

            return
                GetRepsonseHash(hashType, lookupInfo, timeStampString) == hashedValue;
        }

        private string GetRepsonseHash(CnmpHashType hashType,LookupInfo lookupInfo, string timeStampString)
        {
            var srcToHash = string.Concat(

                    lookupInfo.Value,
                    timeStampString,
                    (int) lookupInfo.LookupType,
                    lookupInfo._id

                );

            switch (hashType)
            {
                case CnmpHashType.Crc32:
                    return QuickHashes.GetCRC32String(srcToHash);
                default:
                    return QuickHashes.GetSHA1String(srcToHash);
            }

            



        }


        //// ReSharper disable once UnusedMember.Local
        //private LookupInfo CopyLookupInfo(LookupInfo lookupInfo)
        //{
        //    var lookupInfoNew = new LookupInfo
        //    {
        //        _explicitDelegate = lookupInfo._explicitDelegate,
        //        _id = lookupInfo._id,
        //        _ignoreLocalResponse = lookupInfo._ignoreLocalResponse,
        //        _lookupMode = lookupInfo._lookupMode,
        //        _lookupResult = lookupInfo._lookupResult,
        //        LookupType = lookupInfo.LookupType,
        //        _requestExtras = lookupInfo._requestExtras,
        //        _retryCount = lookupInfo._retryCount,
        //        _timeStamp = lookupInfo._timeStamp,
        //        _unicastIPDown = lookupInfo._unicastIPDown,
        //        _unicastIPUp = lookupInfo._unicastIPUp,
        //        Value = lookupInfo.Value,
        //        _waitMutex = lookupInfo._waitMutex
        //    };

        //    return lookupInfoNew;
        //}

        private void AnalyzeResponse(ParsedXmlValues parsedXmlValues, IPEndPoint ipEndpoint)
        {
            if (null == parsedXmlValues)
                return;

            LookupInfo lookupInfo;
            if (!_requests.TryGetValue(parsedXmlValues._id, out lookupInfo))
                return;

            if (!CompareResponseHash(parsedXmlValues._signature,lookupInfo, parsedXmlValues._ts) )
                // means invalid response - or the response does not belong to any request
                return;

#if DEBUG_OUTPUTS
            Console.WriteLine("CNMP: Valid response received from " + ipEndpoint);
#endif


            Dictionary<string, string> extras = null;
            if (!IsTimedOut(lookupInfo))
            {
                string instanceName = null;
                string typeName = null;

                string computerName = null;
                string contalVersion = null;


                if (null != parsedXmlValues._extrasReceived && parsedXmlValues._extrasReceived.Count > 0)
                {
                    foreach (var pair in parsedXmlValues._extrasReceived)
                    {
                        if (Validator.IsNullString(pair.Key))
                            continue;

                        if (null == extras)
                            extras = new Dictionary<string, string>();

                        string extraValue;
                        try
                        {
                            extraValue = QuickCrypto.Decrypt(
                                    pair.Value,
                                    lookupInfo.Value,
                                    parsedXmlValues._ts
                                );
                            
                        }
                        catch
                        {
                            continue;
                        }

                        switch (pair.Key)
                        {
                            case CnmpProtocol.EXTRA_INSTANCE:
                                instanceName = extraValue;
                                break;
                            case CnmpProtocol.EXTRA_TYPE:
                                typeName = extraValue;
                                break;
                            case CnmpProtocol.EXTRA_COMPUTERNAME:
                                computerName = extraValue;
                                break;
                            case CnmpProtocol.EXTRA_CONTALVERSION:
                                contalVersion = extraValue;
                                break;
                            default:

                                extras[pair.Key] = extraValue;
                                break;
                        }

                    }
                }

                CNMPLookupResultItem item;
                if ((item = lookupInfo._lookupResult.SetResult(
                    parsedXmlValues._identHash,
                    ipEndpoint,
                    instanceName,
                    typeName,
                    computerName,
                    parsedXmlValues._ts,
                    contalVersion)) != null)
                {
                    if (null != extras)
                        item.SetExtras(extras);
                    //FireIdentified(iPEndpoint, aInfo);
                }

            }
            else
            {
                FireTimedOut(lookupInfo);
            }
        }

        private bool IsTimedOut([NotNull] LookupInfo info)
        {
            var now = DateTime.Now;

            var result = now > (info._timeStamp + new TimeSpan(0, 0, 0, 0, _timeout));

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="lookupType"></param>
        /// <param name="keyId"></param>
        /// <param name="callerInstanceName"></param>
        /// <param name="callerType"></param>
        /// <param name="extras"></param>
        public delegate void DValidRequestReceived(IPEndPoint ipEndpoint, CNMPLookupType lookupType, int keyId, string callerInstanceName, string callerType, IDictionary<string, string> extras);
        /// <summary>
        /// 
        /// </summary>
        public event DValidRequestReceived ValidRequestReceived;

        private void InvokeValidRequestReceived(IPEndPoint ipEndpoint, CNMPLookupType lookupType, int keyId, string callerInstanceName, string callerType, IDictionary<string, string> extras)
        {
            if (null != ValidRequestReceived)
                try
                {
                    ValidRequestReceived(ipEndpoint, lookupType, keyId, callerInstanceName, callerType, extras);
                }
                catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyId"></param>
        public void RemoveKey(
            [NotNull]
            string key,
            ref int keyId)
        {
            if (Validator.IsNullString(key))
                return;

            if (_keys.ContainsKey(key))
                try
                {
                    _keys.Remove(key);
                    keyId = 0;
                }
                catch { }
        }

        [UsedImplicitly]
        private class CnmpProtocol
        {
            #region protocol constants

            private const string BeginMark = "<";
            private const string BeginMark2nd = "</";
            private const string EndMark = ">";

            internal const string RequestTagname = "CnmpRequest";
            internal const string RequestStarttag = BeginMark + RequestTagname + " version=\"2.0\">";
            internal const string RequestEndtag = BeginMark2nd + RequestTagname + EndMark;
            internal static readonly byte[] RequestEndtagBytes = RequestEndtag.ToBytes();

            internal const string ResponseTagname = "CnmpResponse";
            internal const string ResponseStarttagFormatString = BeginMark + ResponseTagname + " version=\"{0}\">";
            internal const string ResponseEndtag = BeginMark2nd + ResponseTagname + EndMark;
            internal static readonly byte[] ResponseEndtagBytes = ResponseEndtag.ToBytes();

            internal const string ID_TAGNAME = "Id";
            internal const string ID_STARTTAG = BeginMark + ID_TAGNAME + EndMark;
            internal const string ID_ENDTAG = BeginMark2nd + ID_TAGNAME + EndMark;

            internal const string CALLER_INSTANCE_TAGNAME = "CallerInstance";
            internal const string CALLER_INSTANCE_STARTTAG = BeginMark + CALLER_INSTANCE_TAGNAME + EndMark;
            internal const string CALLER_INSTANCE_ENDTAG = BeginMark2nd + CALLER_INSTANCE_TAGNAME + EndMark;

            internal const string CALLER_TYPE_TAGNAME = "CallerType";
            internal const string CALLER_TYPE_STARTTAG = BeginMark + CALLER_TYPE_TAGNAME + EndMark;
            internal const string CALLER_TYPE_ENDTAG = BeginMark2nd + CALLER_TYPE_TAGNAME + EndMark;

            internal const string SIGNATURE_TAGNAME = "Signature";
            internal const string SIGNATURE_STARTTAG = BeginMark + SIGNATURE_TAGNAME + EndMark;
            internal const string SIGNATURE_ENDTAG = BeginMark2nd + SIGNATURE_TAGNAME + EndMark;

            internal const string IP_TAGNAME = "Ip";
            internal const string IP_STARTTAG = BeginMark + IP_TAGNAME + EndMark;
            internal const string IP_ENDTAG = BeginMark2nd + IP_TAGNAME + EndMark;

            internal const string PORT_TAGNAME = "Port";
            internal const string PORT_STARTTAG = BeginMark + PORT_TAGNAME + EndMark;
            internal const string PORT_ENDTAG = BeginMark2nd + PORT_TAGNAME + EndMark;


            internal const string LOOKUP_TAGNAME = "Lookup";

            internal static string LOOKUP_STARTTAG(CNMPLookupType lookupType)
            {
                return
                    BeginMark + LOOKUP_TAGNAME + " type=\"" + (int)lookupType + "\">";
            }

            internal const string LOOKUP_ENDTAG = BeginMark2nd + LOOKUP_TAGNAME + EndMark;

            internal const string CR_LF = "\r\n";
            internal const string XML_HEADER = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            internal const string TS_TAGNAME = "Ts";
            internal const string TS_STARTTAG = BeginMark + TS_TAGNAME + EndMark;
            internal const string TS_ENDTAG = BeginMark2nd + TS_TAGNAME + EndMark;

            internal const string FORWARDED_FROM_TAGNAME = "ForwardedFrom";
            internal const string FORWARDED_FROM_STARTTAG = BeginMark + FORWARDED_FROM_TAGNAME + EndMark;
            internal const string FORWARDED_FROM_ENDTAG = BeginMark2nd + FORWARDED_FROM_TAGNAME + EndMark;

            internal const string GET_TAGNAME = "Get";
            internal const string GET_STARTTAG = BeginMark + GET_TAGNAME + EndMark;
            internal const string GET_ENDTAG = BeginMark2nd + GET_TAGNAME + EndMark;

            internal const string RETURN_TAGNAME = "Return";

            internal static string RETURN_STARTTAG(string name)
            {
                return
                    BeginMark + RETURN_TAGNAME + " name=\"" + (name ?? String.Empty) + "\">";
            }

            internal const string RETURN_ENDTAG = BeginMark2nd + RETURN_TAGNAME + EndMark;

            internal const string IDENT_HASH_TAGNAME = "IdentHash";
            internal const string IDENT_HASH_STARTTAG = BeginMark + IDENT_HASH_TAGNAME + EndMark;
            internal const string IDENT_HASH_ENDTAG = BeginMark2nd + IDENT_HASH_TAGNAME + EndMark;

            //internal const string XPATH_REQUEST = SLASH + CNMP_REQUEST_TAGNAME;
            //internal const string XPATH_REQUEST_GET = XPATH_REQUEST + SLASH + GET_TAGNAME;

            internal const string EXTRA_INSTANCE = "instance";
            internal const string EXTRA_TYPE = "type";
            internal const string EXTRA_COMPUTERNAME = "computername";
            internal const string EXTRA_CONTALVERSION = "contal/version";

            #endregion
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipEndpoint"></param>
        /// <param name="data"></param>
        protected override void OnDataReceived(IPEndPoint ipEndpoint, ByteDataCarrier data)
        {

#if DEBUG_OUTPUTS
            System.Diagnostics.Debug.WriteLine(cnmpMessage);
#endif

            if (data.IndexOf(CnmpProtocol.RequestEndtagBytes) < 0 &&
                data.IndexOf(CnmpProtocol.ResponseEndtagBytes) < 0)
                return;

            var parsedXmlValues = ParseXmlBySax(data);
            if (parsedXmlValues == null)
                return;



            // FORWARDING IS IMPLEMENTED INSIDE
            switch (parsedXmlValues._rootNodeName)
            {
                case CnmpProtocol.RequestTagname:
                    // DONE EARLIER
                    /*if (parsedXmlValues._identHash != null &&
                        parsedXmlValues._identHash == _identificationHash)
                        // SPLIT HORIZON CONDITION
                        return;*/

#if DEBUG_OUTPUTS
                    Console.WriteLine("Request received from "+ipEndpoint);
#endif

                    AnalyzeRequest(parsedXmlValues, ipEndpoint);
                    break;

                case CnmpProtocol.ResponseTagname:
                    //if "ForwardedFrom" node for response socket does not exist (means that this app received socket as a first app) 
                    //socket must be modified and sent to all apps listening to same port (on same computer!)


                    if (!parsedXmlValues._isForwarded)
                    {
                        // if the forward tag is NOT present

                        if (ForwardIfEnabled(ipEndpoint, data)) return;
                    }
                    else
                    {
                        if (AnalyzeForwarded(ref ipEndpoint, parsedXmlValues)) return;
                    }

#if DEBUG_OUTPUTS
                Console.WriteLine("Response received from " + ipEndpoint);
#endif
                    AnalyzeResponse(parsedXmlValues, ipEndpoint);
                    break;
            }
        }

        private bool AnalyzeForwarded(ref IPEndPoint ipEndpoint, ParsedXmlValues parsedXmlValues)
        {
// if the forward tag IS present

            if (!ipEndpoint.Address.Equals(IPAddress.Loopback))
                // PROBABLY AN INTRUDER 
                // FORWARDED PACKETS FROM OTHER THAN LOOPBACK ARE NOT ALLOWED
                return true;

            var forwardersIdentHash = parsedXmlValues._forwardersIdentHash;
            if (Validator.IsNullString(forwardersIdentHash))
                return true;

            if (forwardersIdentHash == _identificationHash)
            {
                // SPLIT HORIZON CODITION
                return true;
            }

            try
            {
                _isMasterInstance = false;
#if DEBUG_OUTPUTS
                            IPEndPoint formerIPe = new IPEndPoint(ipEndpoint.Address, ipEndpoint.Port);
#endif

                var formerIPe = new IPEndPoint(ipEndpoint.Address, ipEndpoint.Port);

                ipEndpoint = new IPEndPoint(parsedXmlValues._forwardedIp, parsedXmlValues._forwardedPort);

                DebugHelper.Keep(formerIPe);

#if DEBUG_OUTPUTS
                            Console.WriteLine("CNMP: Some forwarded response received from " + ipEndpoint + " via "+ipEndpoint);
#endif
            }
            catch
            {
                return true;
            }
            return false;
        }

        private bool ForwardIfEnabled(IPEndPoint ipEndpoint, ByteDataCarrier data)
        {
            if (!_allowInternalForward) return false;

            // NOT FORWARDED PACKETS SHOULD NOT COME FROM LOOPBACK
            if (ipEndpoint.Address.Equals(IPAddress.Loopback))
                return true;

            _isMasterInstance = true;

            try
            {
                var originalMessage = data.ToString();
                var pos = originalMessage.IndexOf(CnmpProtocol.ResponseEndtag, StringComparison.Ordinal);
                originalMessage = originalMessage.Substring(0, pos);


                var messageWithForwarding = StringBuilderPool.Implicit2k.Get();

                messageWithForwarding.Append(originalMessage);

                messageWithForwarding.Append(CnmpProtocol.FORWARDED_FROM_STARTTAG);

                messageWithForwarding.Append(CnmpProtocol.IP_STARTTAG);
                messageWithForwarding.Append(ipEndpoint.Address);
                messageWithForwarding.Append(CnmpProtocol.IP_ENDTAG);
                messageWithForwarding.Append(CnmpProtocol.CR_LF);

                messageWithForwarding.Append(CnmpProtocol.PORT_STARTTAG);
                messageWithForwarding.Append(ipEndpoint.Port.ToString(CultureInfo.InvariantCulture));
                messageWithForwarding.Append(CnmpProtocol.PORT_ENDTAG);
                messageWithForwarding.Append(CnmpProtocol.CR_LF);

                messageWithForwarding.Append(CnmpProtocol.IDENT_HASH_STARTTAG);
                messageWithForwarding.Append(_identificationHash);
                messageWithForwarding.Append(CnmpProtocol.IDENT_HASH_ENDTAG);
                messageWithForwarding.Append(CnmpProtocol.CR_LF);

                messageWithForwarding.Append(CnmpProtocol.FORWARDED_FROM_ENDTAG);

                messageWithForwarding.Append(CnmpProtocol.ResponseEndtag);

                // TODO : replace by more optimal UTF8 conversion
                var rawData = new ByteDataCarrier(messageWithForwarding.ToString());

                StringBuilderPool.Implicit2k.Return(messageWithForwarding);

                Send(new IPEndPoint(LoopbackBroadcast, _port), rawData);

#if DEBUG
                Debug.WriteLine("\n\n\tForwarding to " + LoopbackBroadcast + " : \n" + messageWithForwarding);
#endif
            }
            catch
            {
            }

            return false;
        }
    }
}
