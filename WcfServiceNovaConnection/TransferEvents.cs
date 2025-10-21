using System.Runtime.Serialization;

namespace WcfServiceNovaConnection
{
    //Basic interface for object, which handles all event types (NOVA side)
    public interface IProcessChangeEventManager
    {
        void PersonSave(PersonObject personObject, int version);
        void PersonDelete(string personalNumber, int version);
        void CardSave(CardObject cardObject, int version);
        void CardDelete(string cardNumber, int version);
    }

    //Common transfer object for transferring to NOVA (all event types)
    //Do not use this directly on Timetec side
    [DataContract]
    public abstract class ObjectChangeEvent
    {
        //Version of object, that was sent to NOVA
        [DataMember]
        public int Version { get; protected set; }

        //Function for processing of object on NOVA side in one loop without manually casting objects
        public abstract void Process(IProcessChangeEventManager eventManager);
    }


    /*
     * Object for transferring to NOVA 
     * (On Timetec side, use those directly by creating of object and sending it to ProcessChanges method)
     */

    //Object for updating Person
    [DataContract]
    public class SavePersonEvent : ObjectChangeEvent
    {
        [DataMember]
        private readonly PersonObject _personObject;

        public SavePersonEvent(PersonObject person, int version)
        {
            Version = version;
            _personObject = person;
        }

        public override void Process(IProcessChangeEventManager eventManager)
        {
            eventManager.PersonSave(_personObject, Version);
        }
    }

    //Object for deleting Person
    [DataContract]
    public class DeletePersonEvent : ObjectChangeEvent
    {
        [DataMember]
        private readonly string _personalNumber;

        public DeletePersonEvent(string personalNumber, int version)
        {
            Version = version;
            _personalNumber = personalNumber;
        }

        public override void Process(IProcessChangeEventManager eventManager)
        {
            eventManager.PersonDelete(_personalNumber, Version);
        }
    }

    //Object for updating Card
    [DataContract]
    public class SaveCardEvent : ObjectChangeEvent
    {
        [DataMember]
        private readonly CardObject _cardObject;

        public SaveCardEvent(CardObject card, int version)
        {
            Version = version;
            _cardObject = card;
        }

        public override void Process(IProcessChangeEventManager eventManager)
        {
            eventManager.CardSave(_cardObject, Version);
        }
    }

    //Object for deleting Card
    [DataContract]
    public class DeleteCardEvent : ObjectChangeEvent
    {
        [DataMember]
        private readonly string _cardNumber;

        public DeleteCardEvent(string cardNumber, int version)
        {
            Version = version;
            _cardNumber = cardNumber;
        }

        public override void Process(IProcessChangeEventManager eventManager)
        {
            eventManager.CardDelete(_cardNumber, Version);
        }
    }
}
