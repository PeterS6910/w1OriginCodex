using System;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable()]
    public class ServerGeneralOptionsDB : AOrmObject
    {
        public virtual Guid IdServerGeneralOptionsDB { get; set; }
        public virtual string NtpIpAddresses { get; set; }
        public virtual int NtpTimeDiffTolerance { get; set; }

        public override string ToString()
        {
            return string.Empty;
        }

        public override bool Compare(object obj)
        {
            throw new NotImplementedException();
        }

        public override bool Contains(string expression)
        {
            return false;
        }

        public override string GetIdString()
        {
            return IdServerGeneralOptionsDB.ToString();
        }

        public override object GetId()
        {
            return IdServerGeneralOptionsDB;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }

        public override Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.ServerGeneralOptionsDB;
        }
    }


    public class NtpSettingsChangedHandler : Contal.IwQuick.Remoting.ARemotingCallbackHandler
    {
        private static volatile NtpSettingsChangedHandler _singleton = null;
        private static object _syncRoot = new object();

        private Contal.IwQuick.DVoid2Void _ntpSettingsChangedHandler;

        public static NtpSettingsChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                            _singleton = new NtpSettingsChangedHandler();
                    }

                return _singleton;
            }
        }

        public NtpSettingsChangedHandler()
            : base("NtpSettingsChangedHandler")
        {
        }

        public void RegisterChanged(Contal.IwQuick.DVoid2Void ntpSettingsChangedHandler)
        {
            _ntpSettingsChangedHandler += ntpSettingsChangedHandler;
        }

        public void UnregisterChanged(Contal.IwQuick.DVoid2Void ntpSettingsChangedHandler)
        {
            _ntpSettingsChangedHandler -= ntpSettingsChangedHandler;
        }

        public void RunEvent()
        {
            if (_ntpSettingsChangedHandler != null)
            {
                try
                {
                    _ntpSettingsChangedHandler();
                }
                catch
                { }
            }
        }
    } 
}
