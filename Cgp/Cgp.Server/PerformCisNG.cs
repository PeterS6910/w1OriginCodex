using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Net;
using Contal.IwQuick;
using JetBrains.Annotations;
using System.Net.Sockets;
using System.IO;
using Contal.IwQuick.Threads;
using System.Threading;

namespace Contal.Cgp.Server
{
    public class PerformCisNG
    {
        #region Variable
        private bool _isAuth = false;
        private bool _messageSucces = false;
        private string _lassMessageResult = string.Empty;
        // ReSharper disable once ConvertToConstant.Local
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private int _errorCount = 0;

        private string _cisngIpAddress;
        private int _cisngPort;
        private Guid _cisGuid;
        private string _userName = string.Empty;
        private string _password = string.Empty;
        private string _cisName = string.Empty;
        private TcpClient _tcpClient;
        private Stream _sslStream;
        private SslSettings _sslSettings;
        ManualResetEvent _isReadingStopped = new ManualResetEvent(true);
        private readonly object _outernLock = new object();
        private readonly object _connectionLock = new object();

        private readonly Queue<XmlDocument> _queueMessages = new Queue<XmlDocument>();

        //private 

        public event DString2Void MsgRecived;
        public event DString2Void ErrorOccured;
        #endregion

        #region Property
        public bool NeedRestart
        {
            get
            {
                if (_errorCount > 10)
                    return true;
                return false;
            }
        }
        public bool IsOnline
        {
            get
            {
                lock (_connectionLock)
                    return _tcpClient != null;
            }
        }

        public bool IsAuth
        {
            get { return _isAuth; }
        }
        public bool MessageSucces
        {
            get { return _messageSucces; }
        }
        public string LassMessageResult
        {
            get { return _lassMessageResult; }
            set { _lassMessageResult = value; }
        }
        public Guid CisGuid
        {
            get { return _cisGuid; }
            set { _cisGuid = value; }
        }
        #endregion

        public PerformCisNG(CisNG dbsCis)
        {
            _cisngIpAddress = dbsCis.IpAddress;
            _cisngPort = dbsCis.Port;
            _cisGuid = dbsCis.IdCisNG;
            _userName = dbsCis.UserName;
            _password = dbsCis.Password;
            _cisName = dbsCis.CisNGName;
        }

        #region TCP
        public void CreateConnection(string address, int port)
        {
            lock (_outernLock)
            {
                Validator.CheckNullString(address);

                _cisngIpAddress = address;
                _cisngPort = port;

                lock (_connectionLock)
                    CloseConnectionInternal();

                _isReadingStopped.WaitOne();

                lock (_connectionLock)
                    Connect();

                PerformCisNGAuthentication();
            }
        }

        private void Connect()
        {
            _sslSettings = new SslSettings(true);
            _tcpClient = new TcpClient();

            try
            {
                _tcpClient.Connect(
                    _cisngIpAddress,
                    _cisngPort);

                var sslLayer = new SslLayer(_sslSettings);
                _sslStream = sslLayer.PrepareClientStream(_tcpClient.GetStream());
            }
            catch
            {
                CloseConnectionInternal();
                return;
            }

            _isReadingStopped.Reset();
            SafeThread.StartThread(ReadXmlThread);
        }

        public void CloseConnection()
        {
            lock (_outernLock)
                lock (_connectionLock)
                    CloseConnectionInternal();
        }

        private void CloseConnectionInternal()
        {
            if (_sslStream != null)
            {
                _sslStream.Dispose();
                _sslStream = null;
            }

            if (_tcpClient == null)
                return;

            _tcpClient.Close();
            _tcpClient.Dispose();

            _tcpClient = null;

            _isAuth = false;
        }

        #endregion

        private void ReadXmlThread()
        {
            try
            {
                var xr = CreateXmlReader(_sslStream);

                while (!xr.EOF)
                {
                    if (xr.NodeType == XmlNodeType.Element)
                    {
                        var currentXmlDocument = new XmlDocument();

                        currentXmlDocument.Load(xr.ReadSubtree());

                        ProcessXml(currentXmlDocument);

                        xr.Close();
                        xr = CreateXmlReader(_sslStream);
                    }

                    xr.Read();
                }
            }
            catch
            {
            }
            finally
            {
                lock (_connectionLock)
                    CloseConnectionInternal();

                _isReadingStopped.Set();
            }
        }

        private static XmlReader CreateXmlReader(Stream stream)
        {
            return XmlReader.Create(
                stream,
                new XmlReaderSettings
                {
                    CloseInput = false
                });
        }

        private void ProcessXml(XmlDocument xmlDoc)
        {
            XmlElement element = xmlDoc.DocumentElement;

            XmlNode node = element.SelectSingleNode("result");
            if (node != null)
            {
                string result = node.InnerText;

                if (element.Name == "cisgeneral")
                {
                    _messageSucces = false;
                }
                else if (element.Name == "cisauth")
                {
                    _isAuth = result == "0";
                }
                else if (element.Name == "cismessage")
                {
                    _messageSucces = result == "0";
                }

                _lassMessageResult = element.InnerText;
                XmlAttributeCollection attributes = node.Attributes;
                foreach (XmlAttribute attr in attributes)
                {
                    _lassMessageResult += " " + attr.InnerText;
                }
            }

            MsgRecived?.Invoke(_lassMessageResult);

            if (_isAuth)
                lock (_queueMessages)
                    if (_queueMessages.Count != 0)
                        PerformSendMessage(_queueMessages.Dequeue());
        }

        public void SendMessageToCisNG(XmlDocument xmlMsg)
        {
            if (_isAuth)
            {
                PerformSendMessage(xmlMsg);
            }
            else
            {
                lock (_queueMessages)
                    _queueMessages.Enqueue(xmlMsg);
            }
        }

        private void PerformSendMessage(XmlDocument xmlMsg)
        {
            try
            {
                PerformSendMessageCore(xmlMsg);
            }
            catch
            {
                ErrorOccured?.Invoke("failed send message");
            }
        }

        private void PerformSendMessageCore(XmlDocument xmlMsg)
        {
            _messageSucces = false;
            var data = xmlMsg.InnerXml.ToBytes();
            _sslStream.Write(data, 0, data.Length);
        }

        public void AuthenticateCis(CisNG dbCisNG)
        {
            lock (_outernLock)
            {
                if (dbCisNG.IpAddress != _cisngIpAddress || dbCisNG.Port != _cisngPort
                    || dbCisNG.Password != _password || dbCisNG.UserName != _userName)
                {
                    //treba ho restartovat
                    lock (_connectionLock)
                        CloseConnectionInternal();
                }

                bool closing;

                lock (_connectionLock)
                    closing = _tcpClient == null;

                if (closing)
                {
                    _isReadingStopped.WaitOne();

                    SetCisNGParameters(dbCisNG);

                    lock (_connectionLock)
                        Connect();
                }

                if (!_isAuth)
                    PerformCisNGAuthentication();
            }
        }
        
        private void SetCisNGParameters(CisNG dbCisNG)
        {
            _cisngIpAddress = dbCisNG.IpAddress;
            _cisngPort = dbCisNG.Port;
            _userName = dbCisNG.UserName;
            _password = dbCisNG.Password;
            _cisGuid = dbCisNG.IdCisNG;
            _cisName = dbCisNG.CisNGName;
        }

        private void PerformCisNGAuthentication()
        {
            try
            {
                PerformCisNGMessage msg = new PerformCisNGMessage();
                msg.CreateMessage(_userName, _password, false);

                PerformSendMessageCore(msg.XmlDoc);
            }
            catch
            {
                ErrorOccured?.Invoke("Cis NG " + _cisName + " authentification fail");
            }
        }
    }

    class PerformCisNGMessage
    {
        private const string XML_VERSION = "1.0";
        private const string XML_ENCODING = "UTF-8";

        private string _authName = "#XML";
        private string _authPassword = "#XML";
        private bool _isEnclosedPassword = false;

        private string _msgClass;
        private byte? _msgPriority;
        private string _msgExpirationType;
        private TimeSpan _lifetime;
        private DateTime _datetime;
        private int _cyclecount;
        private string _unicitygroup;
        private int? _unicitygroupTimeout;
        private int _msgShowtime;
        private bool _switchIdle;
        private bool _switchFullscreen;
        private bool _switchImmediate;
        private bool _switchNoscrolling;
        private bool _msgContentIsXml;
        private string _msgContent;

        XmlDocument _xmlDoc;

        #region Property
        public XmlDocument XmlDoc
        {
            get { return _xmlDoc; }
        }
        #endregion

        private void CreateCisAuth()
        {
            _xmlDoc = new XmlDocument();
            XmlText elementText;

            XmlDeclaration xmlDeclaration = _xmlDoc.CreateXmlDeclaration(XML_VERSION, XML_ENCODING, null);
            _xmlDoc.InsertBefore(xmlDeclaration, _xmlDoc.DocumentElement);
            XmlElement rootNode = _xmlDoc.CreateElement("cisauth");
            rootNode.SetAttribute("version", "1.0");
            _xmlDoc.AppendChild(rootNode);

            XmlElement xmlElement = _xmlDoc.CreateElement("password");
            xmlElement.SetAttribute("user", _authName);
            xmlElement.SetAttribute("type", "plain");

            // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
            if (_isEnclosedPassword)
            {
                elementText = _xmlDoc.CreateTextNode("<![CDATA[" + _authPassword + "]]>");
            }
            else
            {
                elementText = _xmlDoc.CreateTextNode(_authPassword);
            }
            xmlElement.AppendChild(elementText);
            rootNode.AppendChild(xmlElement);
        }

        private void CreateCisMessage()
        {
            _xmlDoc = new XmlDocument();
            XmlElement xmlElementP;

            XmlDeclaration xmlDeclaration = _xmlDoc.CreateXmlDeclaration(XML_VERSION, XML_ENCODING, null);
            _xmlDoc.InsertBefore(xmlDeclaration, _xmlDoc.DocumentElement);
            XmlElement rootNode = _xmlDoc.CreateElement("cismessage");
            rootNode.SetAttribute("version", "1.0");
            _xmlDoc.AppendChild(rootNode);


            //class 
            XmlElement xmlElement = _xmlDoc.CreateElement("class");
            XmlText elementText = _xmlDoc.CreateTextNode(_msgClass);
            xmlElement.AppendChild(elementText);
            rootNode.AppendChild(xmlElement);
            //priority
            if (_msgPriority != null)
            {
                xmlElement = _xmlDoc.CreateElement("priority");
                elementText = _xmlDoc.CreateTextNode(_msgPriority.ToString());
                xmlElement.AppendChild(elementText);
                rootNode.AppendChild(xmlElement);
            }
            //expiration
            if (_switchIdle == false)
            {
                xmlElement = _xmlDoc.CreateElement("expiration");
                xmlElement.SetAttribute("type", _msgExpirationType);
                if (_msgExpirationType == "lifetime")
                {
                    xmlElementP = _xmlDoc.CreateElement("d");
                    elementText = _xmlDoc.CreateTextNode(_lifetime.Days.ToString(CultureInfo.InvariantCulture));
                    xmlElementP.AppendChild(elementText);
                    xmlElement.AppendChild(xmlElementP);

                    xmlElementP = _xmlDoc.CreateElement("h");
                    elementText = _xmlDoc.CreateTextNode(_lifetime.Hours.ToString(CultureInfo.InvariantCulture));
                    xmlElementP.AppendChild(elementText);
                    xmlElement.AppendChild(xmlElementP);

                    xmlElementP = _xmlDoc.CreateElement("i");
                    elementText = _xmlDoc.CreateTextNode(_lifetime.Minutes.ToString(CultureInfo.InvariantCulture));
                    xmlElementP.AppendChild(elementText);
                    xmlElement.AppendChild(xmlElementP);

                    xmlElementP = _xmlDoc.CreateElement("s");
                    elementText = _xmlDoc.CreateTextNode(_lifetime.Seconds.ToString(CultureInfo.InvariantCulture));
                    xmlElementP.AppendChild(elementText);
                    xmlElement.AppendChild(xmlElementP);

                    rootNode.AppendChild(xmlElement);
                }
                else if (_msgExpirationType == "datetime")
                {
                    //<date><y>YEAR</y><m>MONTH</m><d>DAY</d></date>
                    //<time><h>HOUR</h><i>MINUTE</i><s>SECOND</s></time>
                    xmlElementP = _xmlDoc.CreateElement("date");

                    XmlElement xmlElementP2 = _xmlDoc.CreateElement("y");
                    elementText = _xmlDoc.CreateTextNode(_datetime.Year.ToString(CultureInfo.InvariantCulture));
                    xmlElementP2.AppendChild(elementText);
                    xmlElementP.AppendChild(xmlElementP2);
                    xmlElementP2 = _xmlDoc.CreateElement("m");
                    elementText = _xmlDoc.CreateTextNode(_datetime.Month.ToString(CultureInfo.InvariantCulture));
                    xmlElementP2.AppendChild(elementText);
                    xmlElementP.AppendChild(xmlElementP2);
                    xmlElementP2 = _xmlDoc.CreateElement("d");
                    elementText = _xmlDoc.CreateTextNode(_datetime.Day.ToString(CultureInfo.InvariantCulture));
                    xmlElementP2.AppendChild(elementText);
                    xmlElementP.AppendChild(xmlElementP2);
                    xmlElement.AppendChild(xmlElementP);

                    xmlElementP = _xmlDoc.CreateElement("time");
                    xmlElementP2 = _xmlDoc.CreateElement("h");
                    elementText = _xmlDoc.CreateTextNode(_datetime.Hour.ToString(CultureInfo.InvariantCulture));
                    xmlElementP2.AppendChild(elementText);
                    xmlElementP.AppendChild(xmlElementP2);
                    xmlElementP2 = _xmlDoc.CreateElement("i");
                    elementText = _xmlDoc.CreateTextNode(_datetime.Minute.ToString(CultureInfo.InvariantCulture));
                    xmlElementP2.AppendChild(elementText);
                    xmlElementP.AppendChild(xmlElementP2);
                    xmlElementP2 = _xmlDoc.CreateElement("s");
                    elementText = _xmlDoc.CreateTextNode(_datetime.Second.ToString(CultureInfo.InvariantCulture));
                    xmlElementP2.AppendChild(elementText);
                    xmlElementP.AppendChild(xmlElementP2);
                    xmlElement.AppendChild(xmlElementP);

                    rootNode.AppendChild(xmlElement);
                }
                else if (_msgExpirationType == "cyclecount")
                {
                    elementText = _xmlDoc.CreateTextNode(_cyclecount.ToString(CultureInfo.InvariantCulture));
                    xmlElement.AppendChild(elementText);
                    rootNode.AppendChild(xmlElement);
                }
                else if (_msgExpirationType == "unicitygroup")
                {
                    xmlElementP = _xmlDoc.CreateElement("name");
                    elementText = _xmlDoc.CreateTextNode(_unicitygroup.ToString(CultureInfo.InvariantCulture));
                    xmlElementP.AppendChild(elementText);
                    xmlElement.AppendChild(xmlElementP);

                    if (_unicitygroupTimeout != null)
                    {
                        xmlElementP = _xmlDoc.CreateElement("timeout");
                        elementText = _xmlDoc.CreateTextNode(_unicitygroupTimeout.ToString());
                        xmlElementP.AppendChild(elementText);
                        xmlElement.AppendChild(xmlElementP);
                    }

                    rootNode.AppendChild(xmlElement);
                }
            }

            //showtime
            xmlElement = _xmlDoc.CreateElement("showtime");
            elementText = _xmlDoc.CreateTextNode(_msgShowtime.ToString(CultureInfo.InvariantCulture));
            xmlElement.AppendChild(elementText);
            rootNode.AppendChild(xmlElement);

            //switches
            if (_switchIdle || _switchFullscreen || _switchImmediate || _switchNoscrolling)
            {
                xmlElement = _xmlDoc.CreateElement("switches");

                if (_switchIdle)
                {
                    xmlElementP = _xmlDoc.CreateElement("idle");
                    xmlElement.AppendChild(xmlElementP);
                }
                if (_switchFullscreen)
                {
                    xmlElementP = _xmlDoc.CreateElement("fullscreen");
                    xmlElement.AppendChild(xmlElementP);
                }
                if (_switchImmediate)
                {
                    xmlElementP = _xmlDoc.CreateElement("immediate");
                    xmlElement.AppendChild(xmlElementP);
                }
                if (_switchNoscrolling)
                {
                    xmlElementP = _xmlDoc.CreateElement("noscrolling");
                    xmlElement.AppendChild(xmlElementP);
                }
                rootNode.AppendChild(xmlElement);
            }

            //content
            xmlElement = _xmlDoc.CreateElement("content");
            if (_msgContentIsXml)
            //xhtml
            {
                xmlElementP = _xmlDoc.CreateElement("xhtml");
                elementText = _xmlDoc.CreateTextNode(_msgContent);
                xmlElementP.AppendChild(elementText);
                xmlElement.AppendChild(xmlElementP);
            }
            else
            //<powerpoint file=”POWERPOINT_FILE”/>
            {
                xmlElementP = _xmlDoc.CreateElement("powerpoint");
                xmlElementP.SetAttribute("file", _msgContent);
                xmlElement.AppendChild(xmlElementP);
            }
            rootNode.AppendChild(xmlElement);
        }

        public void CreateMessage(
            [NotNull] string name,
            [NotNull] string password,
            bool enclosePassword)
        {
            Validator.CheckNullString(name);
            Validator.CheckNullString(password);
            _authName = name;
            _authPassword = password;
            _isEnclosedPassword = enclosePassword;

            CreateCisAuth();
        }

        public void CreateMessage(
            [NotNull] string className,
            byte? priority,
            TimeSpan lifetime,
            int showtime,
            bool idle,
            bool fullscreen,
            bool immediate,
            bool noscrolling,
            bool isXml,
            string content)
        {
            Validator.CheckNullString(className);

            _msgClass = className;
            _msgPriority = priority;

            _msgExpirationType = "lifetime";
            _lifetime = lifetime;

            if (showtime < 1 || showtime > 3600)
            {
                throw new ArgumentOutOfRangeException("Invalid ShowTime value");
            }
            _msgShowtime = showtime;

            _switchIdle = idle;
            _switchFullscreen = fullscreen;
            _switchImmediate = immediate;
            _switchNoscrolling = noscrolling;

            _msgContentIsXml = isXml;

            _msgContent = content ?? String.Empty;

            CreateCisMessage();
        }

        public void CreateMessage(
            [NotNull] string className,
            byte? priority,
            DateTime datetime,
            int showtime,
            bool idle,
            bool fullscreen,
            bool immediate,
            bool noscrolling,
            bool isXml,
            string content)
        {
            Validator.CheckNullString(className);
            _msgClass = className;
            _msgPriority = priority;

            _msgExpirationType = "datetime";
            _datetime = datetime;

            if (showtime < 1 || showtime > 3600)
            {
                throw new ArgumentOutOfRangeException("Invalid ShowTime value");
            }
            _msgShowtime = showtime;

            _switchIdle = idle;
            _switchFullscreen = fullscreen;
            _switchImmediate = immediate;
            _switchNoscrolling = noscrolling;

            _msgContentIsXml = isXml;

            _msgContent = content ?? String.Empty;

            CreateCisMessage();
        }

        public void CreateMessage(
            [NotNull] string className,
            byte? priority,
            int cyclecount,
            int showtime,
            bool idle,
            bool fullscreen,
            bool immediate,
            bool noscrolling,
            bool isXml,
            string content)
        {
            Validator.CheckNullString(className);
            _msgClass = className;
            _msgPriority = priority;

            if (cyclecount < 1 || cyclecount > 65535)
            {
                throw new ArgumentOutOfRangeException("Invalid Cyclecount value");
            }
            _msgExpirationType = "cyclecount";
            _cyclecount = cyclecount;

            if (showtime < 1 || showtime > 3600)
            {
                throw new ArgumentOutOfRangeException("Invalid ShowTime value");
            }
            _msgShowtime = showtime;

            _switchIdle = idle;
            _switchFullscreen = fullscreen;
            _switchImmediate = immediate;
            _switchNoscrolling = noscrolling;

            _msgContentIsXml = isXml;

            _msgContent = content ?? String.Empty;

            CreateCisMessage();
        }

        public void CreateMessage(
            [NotNull] string className,
            byte? priority,
            [NotNull] string unicitygroupName,
            int? unicityGroupTimeOut,
            int showtime,
            bool idle,
            bool fullscreen,
            bool immediate,
            bool noscrolling,
            bool isXml,
            string content)
        {
            Validator.CheckNullString(className);
            Validator.CheckForNull(unicitygroupName, "unicitygroupName");

            _msgClass = className;
            _msgPriority = priority;


            if (unicityGroupTimeOut != null)
            {
                if (unicityGroupTimeOut < 1 || unicityGroupTimeOut > 2592000)
                {
                    throw new ArgumentOutOfRangeException("Invalid UnicityGroupTimeOut value");
                }
            }
            _msgExpirationType = "unicitygroup";
            _unicitygroup = unicitygroupName;
            _unicitygroupTimeout = unicityGroupTimeOut;

            if (showtime < 1 || showtime > 3600)
            {
                throw new ArgumentOutOfRangeException("Invalid ShowTime value");
            }
            _msgShowtime = showtime;

            _switchIdle = idle;
            _switchFullscreen = fullscreen;
            _switchImmediate = immediate;
            _switchNoscrolling = noscrolling;

            _msgContentIsXml = isXml;

            _msgContent = content;

            CreateCisMessage();
        }

        public void CreateMessage(
            [NotNull] PresentationGroup presentationGroup,
            string message)
        {
            Validator.CheckForNull(presentationGroup, "presentationGroup");

            _msgClass = presentationGroup.cisClass;
            if (presentationGroup.cisPriority != null)
                _msgPriority = (byte)presentationGroup.cisPriority;
            if (!presentationGroup.cisIdle)
            {
                switch (presentationGroup.cisExpirationType)
                {
                    case (int)PresentationGroup.ExpirationType.datetime:
                        if (presentationGroup.cisDatetime != null)
                        {
                            _datetime = (DateTime)presentationGroup.cisDatetime;
                            _msgExpirationType = PresentationGroup.ExpirationType.datetime.ToString();
                        }
                        break;
                    case (int)PresentationGroup.ExpirationType.lifetime:
                        if (presentationGroup.cisLifetime != null)
                        {
                            _lifetime = (TimeSpan)presentationGroup.cisLifetime;
                            _msgExpirationType = PresentationGroup.ExpirationType.lifetime.ToString();
                        }
                        break;
                    case (int)PresentationGroup.ExpirationType.cyclecount:
                        _cyclecount = presentationGroup.cisCyclecount;
                        _msgExpirationType = PresentationGroup.ExpirationType.cyclecount.ToString();
                        break;
                    case (int)PresentationGroup.ExpirationType.unicitygroup:
                        _unicitygroup = presentationGroup.cisUnicitygroup;
                        if (presentationGroup.cisUnicitygroupTimeout != null)
                            _unicitygroupTimeout = presentationGroup.cisUnicitygroupTimeout;
                        _msgExpirationType = PresentationGroup.ExpirationType.unicitygroup.ToString();
                        break;
                }
            }
            else
            {
                _msgPriority = 255;
            }
            _msgShowtime = presentationGroup.cisShowtime;
            _switchIdle = presentationGroup.cisIdle;
            _switchFullscreen = presentationGroup.cisFullscreen;
            _switchImmediate = presentationGroup.cisImmediate;
            _switchNoscrolling = presentationGroup.cisNoscrolling;
            _msgContentIsXml = true;
            _msgContent = message;

            CreateCisMessage();
        }

        public void ReplaceMessage(ref XmlDocument xmlDoc, string message)
        {
            XmlElement element = xmlDoc.DocumentElement;
            XmlNode node = element.SelectSingleNode("content");
            if (node != null)
            {
                XmlNode node2 = node.SelectSingleNode("xhtml");
                if (node2 != null)
                    node2.InnerXml = message;
            }
        }

    }
}
