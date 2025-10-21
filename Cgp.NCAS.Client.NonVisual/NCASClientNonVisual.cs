using System;
using System.Collections.Generic;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Client.NonVisual;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Client.Common;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.IwQuick;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.NCAS.Client.NonVisual
{
    public class NCASClientNonVisual : ACgpClientPlugin<NCASClientNonVisual>
    {
        private readonly NCASClientCore _ncasClientCore;

        public NCASClientNonVisual()
        {
            _ncasClientCore =
                new NCASClientCore(
                    CgpClientNonVisual.Singleton,
                    new LocalizationHelper(typeof(NCASClientNonVisual).Assembly));
        }

        public override void Initialize()
        {
        }

        public override Type GetRemotingProviderInterfaceType()
        {
            return _ncasClientCore.GetRemotingProviderInterfaceType();
        }

        public override void SetRemotingProviderInterface(object remotingProviderInterface)
        {
            _ncasClientCore.SetRemotingProviderInterface(remotingProviderInterface);
        }

        public override string FriendlyName
        {
            get
            {
                return "NCAS plugin";
            }
        }

        private readonly ExtendedVersion _version =
            new ExtendedVersion(
                typeof(NCASClientNonVisual),
                true,
                DevelopmentStage.Testing);

        public override ExtendedVersion Version
        {
            get { return _version; }
        }

        public override void OnDispose()
        {
        }

        public override string Description
        {
            get
            {
                return "NCAS";
            }
        }

        public override string[] FriendPlugins
        {
            get { return null; }
        }

        public override void SaveAfterInsertWithData(object o, object clonedObj)
        {
        }

        public override void ObjectUpdated(object obj)
        {
            _ncasClientCore.ObjectUpdated(obj);
        }

        public override IList<AlarmType> GetPluginAlarmTypes()
        {
            return _ncasClientCore.GetPluginAlarmTypes();
        }

        public ICgpNCASRemotingProvider MainServerProvider
        {
            get { return _ncasClientCore.MainServerProvider; }
        }
    }
}
