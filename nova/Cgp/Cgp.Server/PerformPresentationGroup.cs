using System;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using JetBrains.Annotations;

namespace Contal.Cgp.Server
{
    public class PerformPresentationGroup
    {
        #region Variables

        private readonly PerformPresentationFormatter _messageFormatter;      // formater for adding information to message
        private readonly PresentationGroup _presentationGroup;

        //string _error;
        #endregion

        #region Property
        public string Email
        {
            get
            {
                if (_presentationGroup == null)
                    return string.Empty;
                return _presentationGroup.Email;
            }
            //set { _email = value; }
        }

        public bool InheritedEmailSubject
        {
            get
            {
                if (_presentationGroup == null)
                {
                    return true;
                }

                return _presentationGroup.InheritedEmailSubject;
            }
        }

        public string EmailSubject
        {
            get
            {
                if (_presentationGroup == null)
                {
                    return string.Empty;
                }

                return _presentationGroup.EmailSubject;
            }
        }

        public string Sms
        {
            get
            {
                if (_presentationGroup == null)
                    return string.Empty;
                return _presentationGroup.Sms;
            }
            //set { _sms = value; }
        }

        public Guid? AlarmTransmitterId
        {
            get
            {
                if (_presentationGroup == null)
                    return null;

                return _presentationGroup.AlarmTransmitterId;
            }
        }

        public string SmsPrefix
        {
            get
            {
                if (_presentationGroup == null)
                    return null;

                return _presentationGroup.MessagesPrefix;
            }
        }

        public string[] PhoneNumbers
        {
            get
            {
                if (_presentationGroup == null
                    || string.IsNullOrEmpty(_presentationGroup.PhoneNumbers))
                    return null;

                return _presentationGroup.PhoneNumbers.Split(';');
            }
        }

        public PresentationGroup PresentationGroup
        {
            get { return _presentationGroup; }
        }
        #endregion

        //public event Contal.IwQuick.DVoid2Void SendedMessage;
        //public event Contal.IwQuick.Action<PerformPresentationGroup,string> SendedMessageWithMsg;
        //public event Contal.IwQuick.DVoid2Void ErrorOccured;

        //public PerformPresentationGroup()
        //{
        //}

        /// <summary>
        /// Constructor to create PPG and set the paramaters from DBS
        /// </summary>
        /// <param name="groupId">the Guid of presentation group</param>
        public PerformPresentationGroup(Guid groupId)
        {
           
            _presentationGroup = PresentationGroups.Singleton.GetById(groupId);

            _messageFormatter = null;
            if (_presentationGroup.PresentationFormatter != null)
                _messageFormatter = 
                    new PerformPresentationFormatter(
                        _presentationGroup.PresentationFormatter.FormatterName,
                        _presentationGroup.PresentationFormatter.MessageFormat);

        }


        public PerformPresentationGroup([NotNull] PresentationGroup groupPG)
        {
            Validator.CheckForNull(groupPG,"groupPG");

            _presentationGroup = groupPG;

            _messageFormatter = 
                _presentationGroup.PresentationFormatter == null 
                    ? null 
                    : new PerformPresentationFormatter(
                        _presentationGroup.PresentationFormatter.FormatterName, 
                        _presentationGroup.PresentationFormatter.MessageFormat);
        }

        /// <summary>
        /// If exists Presentation Formater than format message, else return original
        /// </summary>
        /// <param name="message">original message</param>
        /// <returns>formated message</returns>
        public string ReturnFormattedMessage(string message)
        {
            if (_messageFormatter == null)
                return message;
            return _messageFormatter.FormateString(message);
        }
    }
}
