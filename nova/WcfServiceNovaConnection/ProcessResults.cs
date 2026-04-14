using System.Runtime.Serialization;

namespace WcfServiceNovaConnection
{
    //Basic interface for object, which handles results of all event types (NOVA side)
    //(On Timetec side, implement this interface in one result handling object)
    public interface IChangeProcessResultManager
    {
        void SavePersonResult(ObjectChangeProccessResult result, string idPerson, int version);
        void DeletePersonResult(ObjectChangeProccessResult result, string idPerson, int version);
        void SaveCardResult(ObjectChangeProccessResult result, string idCard, int version);
        void DeleteCardResult(ObjectChangeProccessResult result, string idCard, int version);
    }

    //Common result object for receiving of result from NOVA (all result types)
    [DataContract]
    public abstract class ObjectChangeResult
    {
        //Id of object, that was processed in NOVA
        [DataMember]
        public string IdObject { get; set; }
        //Version of object, that was processed in NOVA
        [DataMember]
        public int Version { get; set; }
        [DataMember]
        public ObjectChangeProccessResult Result { get; set; }

        //Function for processing of object on Timetec side in one loop without manually casting objects
        public abstract void Process(IChangeProcessResultManager manager);
    }

    /*
     * Object for storing results from processing in NOVA 
     */

    //Result of updating Person
    [DataContract]
    public class SavePersonResult : ObjectChangeResult
    {
        public override void Process(IChangeProcessResultManager manager)
        {
            manager.SavePersonResult(Result, IdObject, Version);
        }
    }

    //Result of deleting Person
    [DataContract]
    public class DeletePersonResult : ObjectChangeResult
    {
        public override void Process(IChangeProcessResultManager manager)
        {
            manager.DeletePersonResult(Result, IdObject, Version);
        }
    }

    //Result of updating Card
    [DataContract]
    public class SaveCardResult : ObjectChangeResult
    {
        public override void Process(IChangeProcessResultManager manager)
        {
            manager.SaveCardResult(Result, IdObject, Version);
        }
    }

    //Result of deleting Card
    [DataContract]
    public class DeleteCardResult : ObjectChangeResult
    {
        public override void Process(IChangeProcessResultManager manager)
        {
            manager.DeleteCardResult(Result, IdObject, Version);
        }
    }
}
