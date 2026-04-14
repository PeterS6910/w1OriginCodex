using System;
using System.Collections.Generic;
using Contal.IwQuick;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.Beans.Extern;
using Contal.Cgp.Server.DB;
using System.Net.Mail;

namespace Contal.Cgp.Server
{
    public class PerformPresentationProcessor
    {
        DateTime _lastErrorSmtp = DateTime.MinValue;
        DateTime _lastErrorNullCisNG = DateTime.MinValue;
        DateTime _lastErrorCisNGMEssageFail = DateTime.MinValue;
        DateTime _lastErrorCisNGInnerFail = DateTime.MinValue;
        string _processorName;
        PerformCisNG[] _cisNG;

        private StoreEvent _databaseDisconnected;
        public StoreEvent DatabaseDisconnected
        {
            get { return _databaseDisconnected; }
        }

        string _exceptionError;
        public event Contal.IwQuick.DVoid2Void ErrorOccured;


        private static volatile PerformPresentationProcessor _singleton = null;
        private static object _syncRoot = new object();

        public static PerformPresentationProcessor Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new PerformPresentationProcessor();
                    }

                return _singleton;
            }
        }

        ~PerformPresentationProcessor()
        {
            CloseCisNGPerformer();
        }

        #region Properties
        public string ProcessorName
        {
            get { return _processorName; }
            set 
            {
                Validator.CheckNullString(value);
                _processorName = value; 
            }
        }
        #endregion

        private PerformPresentationProcessor()
        {
        }

        #region Perform
        //vykonanie poslania spravy

        /// <summary>
        /// Send the message string,
        /// </summary>
        /// <param name="presentationGroup"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool ProcessSendMessage(PerformPresentationGroup presentationGroup, string msg)
        {
            try
            {
                bool succes = true;
                if (string.IsNullOrEmpty(msg)) return false;
                if (!string.IsNullOrEmpty(presentationGroup.Email))
                {
                    if (!SendMail(presentationGroup.Email,
                            presentationGroup.InheritedEmailSubject,
                            presentationGroup.EmailSubject,
                            presentationGroup.ReturnFormattedMessage(msg),
                            false))
                    {
                        succes = false;
                    }
                }
                if (!string.IsNullOrEmpty(presentationGroup.Sms))
                {
                    //not implementated
                    if (!SendSms(presentationGroup.Sms, presentationGroup.ReturnFormattedMessage(msg)))
                        succes = false;
                }
                if (presentationGroup.PresentationGroup.CisNG != null)
                {
                    if (!SendCisNGMessage(presentationGroup, msg))
                        succes = false;
                }

                var plugins = CgpServer.Singleton.PluginManager.GetLoadedPlugins();

                if (plugins != null)
                    foreach (var plugin in plugins)
                    {
                        if (!plugin.ProcessPresentationGroup(presentationGroup, msg))
                            succes = false;
                    }

                return succes;
            }
            catch (Exception ex)
            {
                _exceptionError = ex.Message;
                if (ErrorOccured != null) ErrorOccured();
                return false;
            }
        }

        public bool ProcessSendTestMail(PerformPresentationGroup presentationGroup, string msg, out Exception error)
        {
            try
            {
                error = null;
                if (string.IsNullOrEmpty(msg))
                    return false;

                if (string.IsNullOrEmpty(presentationGroup.Email))
                    return false;

                if (SendMail(presentationGroup.Email,
                        presentationGroup.InheritedEmailSubject,
                        presentationGroup.EmailSubject,
                        presentationGroup.ReturnFormattedMessage(msg),
                        true))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                error = ex;
                return false;
            }
        }



        public bool ProcessSendMessageNoDbs(NoDbsPG pg, string msg)
        {
            try
            {
                bool succes = true;
                if (string.IsNullOrEmpty(msg)) return false;
                if (!string.IsNullOrEmpty(pg.Mail))
                {
                    string sendMsg = msg;
                    if (pg.Message != null)
                    {
                        PerformPresentationFormatter formatMsg = new PerformPresentationFormatter("tmp", pg.Message);
                        sendMsg = formatMsg.FormateString(msg);
                    }
                    if (!SendMail(pg.Mail, 
                            pg.InheritedEmailSubject,
                            pg.EmailSubject,
                            sendMsg,
                            false))
                    {
                        succes = false;
                    }
                }
                if (!string.IsNullOrEmpty(pg.Sms))
                {
                    PerformPresentationFormatter formatMsg = new PerformPresentationFormatter("tmp", pg.Message);
                    string sendMsg = formatMsg.FormateString(msg);
                    //not implementated
                    if (!SendSms(pg.Sms, msg))
                        succes = false;
                }
                if (pg.CisNgId != null)
                {
                    if (!SendCisNGMessageNoDbs(pg, msg))
                        succes = false;
                }
                return succes;
            }
            catch (Exception ex)
            {
                _exceptionError = ex.Message;
                if (ErrorOccured != null) ErrorOccured();
                return false;
            }
        }



        //private bool SendMail(PerformPresentationGroup sendPressGroup, string message)
        private bool SendMail(string pgMailAddress, bool pgInheritedSubject, string pgSubject, string formattedMessage, bool throwException)
        {
            try
            {
                if (!GeneralOptions.Singleton.IsSetSMTP())
                    return false;

                System.Net.Mail.MailMessage mailMsg = new System.Net.Mail.MailMessage();
                string[] parts = pgMailAddress.Split(';', ' ', ',');
                if (parts == null || parts.Length == 0) return true;
                foreach (string email in parts)
                {
                    string mailAddress = email.Trim();
                    if (mailAddress == string.Empty) continue;
                    mailMsg.To.Add(mailAddress);
                }
                if (pgInheritedSubject)
                {
                    mailMsg.Subject = GeneralOptions.Singleton.SmtpSubject;
                }
                else
                {
                    mailMsg.Subject = pgSubject;
                }

                mailMsg.From = new System.Net.Mail.MailAddress(GeneralOptions.Singleton.SmtpSourceEmailAddress);
                //mailMsg.Body = sendPressGroup.ReturnFormattedMessage(message);
                mailMsg.Body = formattedMessage;
                using (SmtpClient smtpClient = new SmtpClient(GeneralOptions.Singleton.SmtpServer))
                {
                    smtpClient.Port = GeneralOptions.Singleton.SmtpPort;
                    smtpClient.EnableSsl = GeneralOptions.Singleton.SmtpSsl;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    if (String.IsNullOrEmpty(GeneralOptions.Singleton.SmtpCredentials))
                    {
                        smtpClient.UseDefaultCredentials = true;
                    }
                    else
                    {
                        smtpClient.UseDefaultCredentials = false;
                        string[] credentials = GeneralOptions.Singleton.SmtpCredentials.Split('|');
                        smtpClient.Credentials = new System.Net.NetworkCredential(credentials[0], credentials[1]);
                    }

                    smtpClient.Send(mailMsg);
                }
                
                return true;
            }
            catch
            {
                if (throwException)
                {
                    throw;
                }

                LogError("SMTP send mail failed.", ref _lastErrorSmtp);
                return false;
            }
        }

        private bool SendSms(string pgSmsNumbers, string message)
        {
            //if (string.IsNullOrEmpty(sendPressGroup.PresentationGroup.Sms)) return true;
            if (!GeneralOptions.Singleton.IsSetSerialPort()) return false;
            return true;
        }

        private void LogError(string error, ref DateTime errorDate)
        {
            try
            {
                TimeSpan ts;
                ts = DateTime.Now - errorDate;

#if DEBUG
                if (ts.Seconds > 30)
#else
                if (ts.Minutes > 5)
#endif
                {
                    errorDate = DateTime.Now;
                    DB.Eventlogs.Singleton.InsertEvent(Eventlog.TYPEPRESENTATIONPROCESSOR, this.GetType().Assembly.GetName().Name,null, error);
                    //DB.Eventlogs.Singleton.InsertEvent(Eventlog.TYPEPRESENTATIONPROCESSOR, this.GetType().Assembly.GetName().Name, "Presentation processor", null, error);
                }
            }
            catch
            {
            }
        }

        private void CloseCisNGPerformer()
        {
            if (_cisNG == null) return;
            foreach (PerformCisNG cis in _cisNG)
            {
                cis.CloseConnection();
            }
        }

        public int ReturnCisNGStatus(Server.Beans.CisNG dbsCisNG)
        {
            try
            {
                int number = ReturnCisNGNumber(dbsCisNG);
                if (_cisNG[number].IsAuth)
                    return 2;
                else if (_cisNG[number].IsOnline)
                    return 1;
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        private int ReturnCisNGNumber(Server.Beans.CisNG dbsCisNG)
        {
            if (_cisNG != null && _cisNG.Length != 0)
            {
                for (int i = 0; i < _cisNG.Length; i++)
                {
                    if (dbsCisNG.IdCisNG == _cisNG[i].CisGuid)
                    {
                        _cisNG[i].AuthenticateCis(dbsCisNG);
                        return i;
                    }
                }
            }

            PerformCisNG cisNGadd = new PerformCisNG(dbsCisNG);
            cisNGadd.MsgRecived += CisNGMsgRecived;
            cisNGadd.ErrorOccured += CisNGErrorOccured;
            cisNGadd.AuthenticateCis(dbsCisNG);

            if (_cisNG != null && _cisNG.Length != 0)
            {
                Array.Resize(ref _cisNG, _cisNG.Length + 1);
                _cisNG[_cisNG.Length - 1] = cisNGadd;
                return _cisNG.Length - 1;
            }
            else
            {
                _cisNG = new PerformCisNG[1];
                _cisNG[0] = cisNGadd;
                return 0;
            }
        }

        private int ReturnCisNGNumber(Guid CisNGGuid)
        {
            if (_cisNG != null && _cisNG.Length != 0)
            {
                for (int i = 0; i < _cisNG.Length; i++)
                {
                    if (CisNGGuid == _cisNG[i].CisGuid)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private bool SendCisNGMessage(PerformPresentationGroup sendPressGroup, string message)
        {
            try
            {
                ICollection<CisNG> cisy = Contal.Cgp.Server.DB.PresentationGroups.Singleton.ReturnAllCisNG(sendPressGroup.PresentationGroup);
                if (cisy != null)
                {
                    foreach (CisNG cis in cisy)
                    {
                        PerformCisNGMessage cisMsg = new PerformCisNGMessage();
                        if (cis == null)
                        {
                            LogError("CisNG is null.", ref _lastErrorNullCisNG);
                            return false;
                        }

                        int myCisNG = ReturnCisNGNumber(cis);
                        _cisNG[myCisNG].AuthenticateCis(cis);
                        cisMsg.CreateMessage(sendPressGroup.PresentationGroup, sendPressGroup.ReturnFormattedMessage(message));
                        _cisNG[myCisNG].SendMessageToCisNG(cisMsg.XmlDoc);

                        cisMsg.CreateMessage(sendPressGroup.PresentationGroup, sendPressGroup.ReturnFormattedMessage(message));
                    }
                }

                //if (sendPressGroup.PresentationGroup.CisNG != null)
                //{
                //    foreach (CisNG cis in sendPressGroup.PresentationGroup.CisNG)
                //    {

                //        PerformCisNGMessage cisMsg = new PerformCisNGMessage();
                //        if (cis == null)
                //        {
                //            LogError("CisNG is null.", ref _lastErrorNullCisNG);
                //            return false;
                //        }

                //        int myCisNG = ReturnCisNGNumber(cis);
                //        _cisNG[myCisNG].AuthenticateCis(cis);
                //        cisMsg.CreateMessage(sendPressGroup.PresentationGroup, sendPressGroup.ReturnFormattedMessage(message));
                //        _cisNG[myCisNG].SendMessageToCisNG(cisMsg.XmlDoc);

                //        //cisMsg.CreateMessage(sendPressGroup.PresentationGroup, sendPressGroup.ReturnFormattedMessage(message));

                //    }
                //}
                return true;
            }
            catch
            {
                LogError("Cis.NG send message failed.", ref _lastErrorCisNGMEssageFail);
                return false; 
            }

        }

        private bool SendCisNGMessageNoDbs(NoDbsPG sendPressGroup, string message)
        {
            try
            {
                if (sendPressGroup.CisNgId != null)
                {
                    foreach (Guid cis in sendPressGroup.CisNgId)
                    {
                        int myCisNG = ReturnCisNGNumber(cis);
                        if (myCisNG != -1)
                        {
                            PerformCisNGMessage cisMsg = new PerformCisNGMessage();
                            System.Xml.XmlDocument docXml = sendPressGroup.XmlMessage;
                            cisMsg.ReplaceMessage(ref docXml, message);
                            _cisNG[myCisNG].SendMessageToCisNG(docXml);
                        }
                    }
                }
                return true;
            }
            catch
            {
                LogError("Cis.NG send message failed.", ref _lastErrorCisNGMEssageFail);
                return false;
            }

        }
        
        void CisNGMsgRecived(string inputString)
        {
            if (!inputString.Contains("uccess"))
            {
                LogError("Cis.NG error:" + inputString, ref _lastErrorCisNGMEssageFail);
            }
        }

        void CisNGErrorOccured(string inputString)
        {
            LogError("Cis.NG error:" + inputString, ref _lastErrorCisNGInnerFail);
        }

        public void RunAllCisNG()
        {
            try
            {
                Contal.Cgp.Server.DB.CisNGs cNG = CisNGs.Singleton;
                ICollection<CisNG> allCisNG = cNG.List();
                if (allCisNG != null)
                {
                    foreach (CisNG cis in allCisNG)
                    {
                        ReturnCisNGNumber(cis);
                    }
                }
            }
            catch
            { }
        }
        #endregion

        public void CreateStore()
        {
            _databaseDisconnected = new StoreEvent(SystemEvent.DATABASE_DISCONNECTED);
        }
    }

    public class NoDbsPG
    {
        private string _mail;
        private bool _inheritedEmailSubject;
        private string _emailSubject;
        private string _sms;
        private Guid[] _cisNG;
        private string _message;
        private System.Xml.XmlDocument _xmlMessage;

        public string Mail
        {
            get { return _mail; }
        }

        public bool InheritedEmailSubject
        {
            get { return _inheritedEmailSubject; }
        }

        public string EmailSubject
        {
            get { return _emailSubject; }
        }

        public string Sms
        {
            get { return _sms; }
        }

        public Guid[] CisNgId
        {
            get { return _cisNG; }
        }

        public string Message
        {
            get { return _message; }
        }

        public System.Xml.XmlDocument XmlMessage
        {
            get { return _xmlMessage; }
        }

        public NoDbsPG(PresentationGroup pg)
        {
            _mail = pg.Email;
            _inheritedEmailSubject = pg.InheritedEmailSubject;
            _emailSubject = pg.EmailSubject;

            _sms = pg.Sms;

            FillCisNG(pg);
            if (pg.PresentationFormatter != null)
            {
                _message = pg.PresentationFormatter.MessageFormat;
            }
            PerformCisNGMessage cisMsg = new PerformCisNGMessage();
            cisMsg.CreateMessage(pg,"PLAINTEXT");
            _xmlMessage = cisMsg.XmlDoc;
        }

        private void FillCisNG(PresentationGroup pg)
        {
            IList<CisNG> allCisNGs = new List<CisNG>();
            if (pg.CisNG != null)
            {
                foreach (CisNG cis in pg.CisNG)
                {
                    if (!allCisNGs.Contains(cis))
                    {
                        allCisNGs.Add(cis);
                    }
                }
            }

            if (pg.CisNGGroup != null)
            {
                foreach (CisNGGroup cisGrup in pg.CisNGGroup)
                {
                    foreach (CisNG cis in cisGrup.CisNG)
                    {
                        if (!allCisNGs.Contains(cis))
                        {
                            allCisNGs.Add(cis);
                        }
                    }
                }
            }

            _cisNG = new Guid[allCisNGs.Count];
            int i = 0;
            foreach (CisNG cis in allCisNGs)
            {
                _cisNG[i] = cis.IdCisNG;
                i++;
            }
        }
    }

    public class StoreEvent
    {
        List<NoDbsPG> _storedPG;

        public List<NoDbsPG> StoredPG
        {
            get { return _storedPG; }
        }

        public StoreEvent(string eventName)
        {
            SystemEvent sysEvent;
            sysEvent = SystemEvents.Singleton.GetByName(eventName);

            if (sysEvent == null) return;
            if (sysEvent.PresentationGroups == null) return;
            if (sysEvent.PresentationGroups.Count == 0) return;
                
            _storedPG = new List<NoDbsPG>();
            foreach (PresentationGroup pg in sysEvent.PresentationGroups)
            {
                NoDbsPG newDbsPG = new NoDbsPG(pg);
                _storedPG.Add(newDbsPG);
            }
        }
    }
}