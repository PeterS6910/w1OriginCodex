using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;

namespace Contal.Cgp.Client.Common
{
    public interface ICgpClientPlugin : ICgpPlugin
    {
        void SetRemotingProviderInterface(object parameter);
        void PreRegisterAttachCallbackHandlers();
        IList<AlarmType> GetPluginAlarmTypes();

        void SaveAfterInsertWithData(
            object o, 
            object clonedObj);

        void OpenDBSEdit(AOrmObject dbObj);
        void RestartCardReaderCommunication();
        void SendCardReaderCommand(CardReaderSceneType crCommanad);
        ICollection<IModifyObject> GetIModifyObjects(ObjectType objectType);
        void OpenDBSInsert(string strObjectTableType, Action<object> doAfterInsert);
    }
}