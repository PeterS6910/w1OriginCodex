using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace WcfServiceNovaConnection
{
    //enum for transitions add result

    [ServiceContract(CallbackContract = typeof(IServiceNovaTimetecCallback))]
    [ServiceKnownType(typeof(SavePersonResult))]
    [ServiceKnownType(typeof(DeletePersonResult))]
    [ServiceKnownType(typeof(SaveCardResult))]
    [ServiceKnownType(typeof(DeleteCardResult))]
    public interface IServiceNovaTimetec
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        TransitionAddResult TimetecInsertTransition(TransitionObject transition);

        [OperationContract]
        DateTime RpcPing();

        [OperationContract]
        void ProcessChangesResult(IEnumerable<ObjectChangeResult> changeActionResults);
    }

    [ServiceContract]
    [ServiceKnownType(typeof(SavePersonEvent))]
    [ServiceKnownType(typeof(DeletePersonEvent))]
    [ServiceKnownType(typeof(SaveCardEvent))]
    [ServiceKnownType(typeof(DeleteCardEvent))]
    public interface IServiceNovaTimetecCallback
    {
        [OperationContract(IsOneWay = true)]
        void ProcessChanges(IEnumerable<ObjectChangeEvent> changeActions);
    }
}
