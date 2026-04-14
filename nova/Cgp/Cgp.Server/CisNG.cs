using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Contal.IwQuick.Net;
using Contal.IwQuick;

namespace Contal.Cgp.Server
{
    class CisNG
    {
        #region Variable
        private bool _isOnline = false;
        private bool _isAuth = false;
        private bool _messageSucces = false;
        private string _lassMessageResult = string.Empty;

        private string _cisngIpAddress = "10.0.8.14";
        private int _cisngPort = 63001;
        private SimpleTcpClient _tcpClient;
        private SslSettings _sslSettings;

        public event DFromStringToVoid MsgRecived;
        #endregion

        #region Property
        public bool IsOnline
        {
            get { return _isOnline; }
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
        }
        #endregion


        #region TCP
        public void CreateConnection(string address, int port)
        {
            Validator.CheckNullString(address);
            Validator.CheckNull(port);
            _cisngIpAddress = address;
            _cisngPort = port;

            if (_tcpClient != null && _tcpClient.IsConnected)
            {
                _tcpClient.Disconnect();
            }

            _sslSettings = new SslSettings(true);
            _tcpClient = new SimpleTcpClient();
            _tcpClient.Connected += new DTcpConnectEvent(_tcpClient_Connected);
            _tcpClient.Disconnected += new DTcpConnectEvent(_tcpClient_Disconnected);
            _tcpClient.DataSent += new DTcpDataEvent(_tcpClient_DataSent);
            _tcpClient.DataReceived += new DTcpDataEvent(_tcpClient_DataReceived);
            _tcpClient.ConnectionFailed += new DTcpConnectFailureEvent(_tcpClient_ConnectionFailed);
            _tcpClient.Connect(address, port, _sslSettings);
        }

        public void CloseConnection()
        {
            if (_tcpClient == null) return;
            _tcpClient.Disconnect();
        }

        void _tcpClient_DataReceived(ISimpleTcpConnection connection, ByteDataCarrier data)
        {
            ReadXML(data.ToString());
            if (MsgRecived != null)
            {
                MsgRecived(_lassMessageResult);
            }
        }

        void _tcpClient_DataSent(ISimpleTcpConnection connection, ByteDataCarrier data)
        {
            _messageSucces = false;
        }

        void _tcpClient_Connected(ISimpleTcpConnection connection)
        {
            _isOnline = true;
        }
        void _tcpClient_Disconnected(ISimpleTcpConnection connection)
        {
            _isOnline = false;
            _isAuth = false;
        }
        void _tcpClient_ConnectionFailed(Exception connection)
        {
            _isOnline = false;
            _isAuth = false;
        }
        #endregion


        private void ReadXML(string xmlText)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.InnerXml = xmlText;
            string result;

            XmlElement element = xmlDoc.DocumentElement;
            XmlNode node;

            node = element.SelectSingleNode("result");
            if (node != null)
            {
                result = node.InnerText;

                if (element.Name == "cisgeneral")
                {
                    _messageSucces = false;
                }
                else if (element.Name == "cisauth")
                {
                    if (result == "0")
                        _isAuth = true;
                    else
                        _isAuth = false;
                }
                else if (element.Name == "cismessage")
                {
                    if (result == "0")
                        _messageSucces = true;
                    else
                        _messageSucces = false;
                }

                _lassMessageResult = element.InnerText;
                XmlAttributeCollection attributes = node.Attributes;
                foreach (XmlAttribute attr in attributes)
                {
                    _lassMessageResult += " " + attr.InnerText;
                }
            }
        }

        public bool SendMessage(XmlDocument xmlMsg)
        {
            try
            {
                if (!_isOnline) return false;
                ByteDataCarrier sendData = new ByteDataCarrier(xmlMsg.InnerXml.ToString());
                _tcpClient.Send(sendData);
                return true;
            }
            catch// (Exception ex)
            {
                return false;
            }
        }
    }

    class CisNGMessage
    {
        private const string XML_VERSION = "1.0";
        private const string XML_ENCODING = "UTF-8";

        private string _authName = "#XML";
        private string _authPassword = "#XML";
        private bool _isEnclosedPassword = false;

        private string _msgClass;
        private byte? _msqPriority;
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
            XmlElement rootNode;
            XmlElement xmlElement;
            XmlText elementText;

            XmlDeclaration xmlDeclaration = _xmlDoc.CreateXmlDeclaration(XML_VERSION, XML_ENCODING, null);
            _xmlDoc.InsertBefore(xmlDeclaration, _xmlDoc.DocumentElement);
            rootNode = _xmlDoc.CreateElement("cisauth");
            rootNode.SetAttribute("version", "1.0");
            _xmlDoc.AppendChild(rootNode);

            xmlElement = _xmlDoc.CreateElement("password");
            xmlElement.SetAttribute("user", _authName);
            xmlElement.SetAttribute("type", "plain");

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
            XmlElement rootNode;
            XmlElement xmlElement;
            XmlElement xmlElementP;
            XmlElement xmlElementP2;
            XmlText elementText;

            XmlDeclaration xmlDeclaration = _xmlDoc.CreateXmlDeclaration(XML_VERSION, XML_ENCODING, null);
            _xmlDoc.InsertBefore(xmlDeclaration, _xmlDoc.DocumentElement);
            rootNode = _xmlDoc.CreateElement("cismessage");
            rootNode.SetAttribute("version", "1.0");
            _xmlDoc.AppendChild(rootNode);


            //class 
            xmlElement = _xmlDoc.CreateElement("class");
            elementText = _xmlDoc.CreateTextNode(_msgClass);
            xmlElement.AppendChild(elementText);
            rootNode.AppendChild(xmlElement);
            //priority
            if (_msqPriority != null)
            {
                xmlElement = _xmlDoc.CreateElement("priority");
                elementText = _xmlDoc.CreateTextNode(_msqPriority.ToString());
                xmlElement.AppendChild(elementText);
                rootNode.AppendChild(xmlElement);
            }
            //expiration
            xmlElement = _xmlDoc.CreateElement("expiration");
            xmlElement.SetAttribute("type", _msgExpirationType);
            if (_msgExpirationType == "lifetime")
            {
                xmlElementP = _xmlDoc.CreateElement("d");
                elementText = _xmlDoc.CreateTextNode(_lifetime.Days.ToString());
                xmlElementP.AppendChild(elementText);
                xmlElement.AppendChild(xmlElementP);

                xmlElementP = _xmlDoc.CreateElement("h");
                elementText = _xmlDoc.CreateTextNode(_lifetime.Hours.ToString());
                xmlElementP.AppendChild(elementText);
                xmlElement.AppendChild(xmlElementP);

                xmlElementP = _xmlDoc.CreateElement("i");
                elementText = _xmlDoc.CreateTextNode(_lifetime.Minutes.ToString());
                xmlElementP.AppendChild(elementText);
                xmlElement.AppendChild(xmlElementP);

                xmlElementP = _xmlDoc.CreateElement("s");
                elementText = _xmlDoc.CreateTextNode(_lifetime.Seconds.ToString());
                xmlElementP.AppendChild(elementText);
                xmlElement.AppendChild(xmlElementP);

                rootNode.AppendChild(xmlElement);
            }
            else if (_msgExpirationType == "datetime")
            {
                //<date><y>YEAR</y><m>MONTH</m><d>DAY</d></date>
                //<time><h>HOUR</h><i>MINUTE</i><s>SECOND</s></time>
                xmlElementP = _xmlDoc.CreateElement("date");

                xmlElementP2 = _xmlDoc.CreateElement("y");
                elementText = _xmlDoc.CreateTextNode(_datetime.Year.ToString());
                xmlElementP2.AppendChild(elementText);
                xmlElementP.AppendChild(xmlElementP2);
                xmlElementP2 = _xmlDoc.CreateElement("m");
                elementText = _xmlDoc.CreateTextNode(_datetime.Month.ToString());
                xmlElementP2.AppendChild(elementText);
                xmlElementP.AppendChild(xmlElementP2);
                xmlElementP2 = _xmlDoc.CreateElement("d");
                elementText = _xmlDoc.CreateTextNode(_datetime.Day.ToString());
                xmlElementP2.AppendChild(elementText);
                xmlElementP.AppendChild(xmlElementP2);
                xmlElement.AppendChild(xmlElementP);

                xmlElementP = _xmlDoc.CreateElement("time");
                xmlElementP2 = _xmlDoc.CreateElement("h");
                elementText = _xmlDoc.CreateTextNode(_datetime.Hour.ToString());
                xmlElementP2.AppendChild(elementText);
                xmlElementP.AppendChild(xmlElementP2);
                xmlElementP2 = _xmlDoc.CreateElement("i");
                elementText = _xmlDoc.CreateTextNode(_datetime.Minute.ToString());
                xmlElementP2.AppendChild(elementText);
                xmlElementP.AppendChild(xmlElementP2);
                xmlElementP2 = _xmlDoc.CreateElement("s");
                elementText = _xmlDoc.CreateTextNode(_datetime.Second.ToString());
                xmlElementP2.AppendChild(elementText);
                xmlElementP.AppendChild(xmlElementP2);
                xmlElement.AppendChild(xmlElementP);

                rootNode.AppendChild(xmlElement);
            }
            else if (_msgExpirationType == "cyclecount")
            {
                elementText = _xmlDoc.CreateTextNode(_cyclecount.ToString());
                xmlElement.AppendChild(elementText);
                rootNode.AppendChild(xmlElement);
            }
            else if (_msgExpirationType == "unicitygroup")
            {
                xmlElementP = _xmlDoc.CreateElement("name");
                elementText = _xmlDoc.CreateTextNode(_unicitygroup.ToString());
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

            //showtime
            xmlElement = _xmlDoc.CreateElement("showtime");
            elementText = _xmlDoc.CreateTextNode(_msgShowtime.ToString());
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
                elementText = _xmlDoc.CreateTextNode(_msgContent.ToString());
                xmlElementP.AppendChild(elementText);
                xmlElement.AppendChild(xmlElementP);
            }
            else
            //<powerpoint file=”POWERPOINT_FILE”/>
            {
                xmlElementP = _xmlDoc.CreateElement("powerpoint");
                xmlElementP.SetAttribute("file", _msgContent.ToString());
                xmlElement.AppendChild(xmlElementP);
            }
            rootNode.AppendChild(xmlElement);
        }

        public void CreateMessage(string name, string password, bool enclosePassword)
        {
            Validator.CheckNullString(name);
            Validator.CheckNullString(password);
            _authName = name;
            _authPassword = password;
            _isEnclosedPassword = enclosePassword;

            CreateCisAuth();
        }

        public void CreateMessage(string className, byte? priority, TimeSpan lifetime, int showtime,
            bool idle, bool fullscreen, bool immediate, bool noscrolling, bool isXml, string content)
        {
            Validator.CheckNullString(className);
            _msgClass = className;
            _msqPriority = priority;

            Validator.CheckNull(lifetime);
            _msgExpirationType = "lifetime";
            _lifetime = lifetime;

            Validator.CheckNull(showtime);
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
            Validator.CheckNull(content);
            _msgContent = content;

            CreateCisMessage();
        }

        public void CreateMessage(string className, byte? priority, DateTime datetime, int showtime,
            bool idle, bool fullscreen, bool immediate, bool noscrolling, bool isXml, string content)
        {
            Validator.CheckNullString(className);
            _msgClass = className;
            _msqPriority = priority;

            Validator.CheckNull(datetime);
            _msgExpirationType = "datetime";
            _datetime = datetime;

            Validator.CheckNull(showtime);
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
            Validator.CheckNull(content);
            _msgContent = content;

            CreateCisMessage();
        }

        public void CreateMessage(string className, byte? priority, int cyclecount, int showtime,
            bool idle, bool fullscreen, bool immediate, bool noscrolling, bool isXml, string content)
        {
            Validator.CheckNullString(className);
            _msgClass = className;
            _msqPriority = priority;

            Validator.CheckNull(cyclecount);
            if (cyclecount < 1 || cyclecount > 65535)
            {
                throw new ArgumentOutOfRangeException("Invalid Cyclecount value");
            }
            _msgExpirationType = "cyclecount";
            _cyclecount = cyclecount;

            Validator.CheckNull(showtime);
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
            Validator.CheckNull(content);
            _msgContent = content;

            CreateCisMessage();
        }

        public void CreateMessage(string className, byte? priority, string unicitygroupName, int? unicityGroupTimeOut,
            int showtime, bool idle, bool fullscreen, bool immediate, bool noscrolling, bool isXml, string content)
        {
            Validator.CheckNullString(className);
            _msgClass = className;
            _msqPriority = priority;

            Validator.CheckNull(unicitygroupName);
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

            Validator.CheckNull(showtime);
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
            Validator.CheckNull(content);
            _msgContent = content;

            CreateCisMessage();
        }
    }
}
