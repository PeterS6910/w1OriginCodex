using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;

namespace Contal.Cgp.Client.Common
{
    public abstract class ACgpClientPlugin<TCgpClientPlugin> : 
        ACgpPlugin<TCgpClientPlugin>,
        ICgpClientPlugin
        where TCgpClientPlugin : ACgpClientPlugin<TCgpClientPlugin>
    {
        public virtual void OpenDBSEdit(AOrmObject dbObj) { }
        public virtual void OpenDBSInsert(string strObjectTableType, Action<object> doAfterInsert) { }
        public virtual ICollection<IModifyObject> GetIModifyObjects(ObjectType objectType) { return null; }
        public virtual void RestartCardReaderCommunication() { }
        public virtual void SendCardReaderCommand(CardReaderSceneType crCommanad) { }
        public virtual void SetRemotingProviderInterface(object remotingProviderInterface) { }

        public virtual void PreRegisterAttachCallbackHandlers() { }

        public abstract IList<AlarmType> GetPluginAlarmTypes();

        public abstract void SaveAfterInsertWithData(
            object o, 
            object clonedObj);
    }
}
