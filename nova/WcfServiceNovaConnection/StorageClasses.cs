using System;
using System.Collections.Generic;

namespace WcfServiceNovaConnection
{
    //Object for transferring of transitions from NOVA to Timetec (direct WCF call)
    public class TransitionObject
    {
        //Card number
        public string CardNumber { get; set; }

        //Readers unique name, from which was transition detected
        public string ReaderName { get; set; }

        public TransitionType TransitionType { get; set; }

        //Date of transition detection
        public DateTime AddDateTime { get; set; }
    }

    //Basic object for sending of Person from Timetec to NOVA (callback WCF call)
    public class PersonObject
    {
        public string PersonalNumber { get; set; } //Main person ID
        public string Department { get; set; } //Department ID
        public string FirstTitulus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string LastTitulus { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string StreetHouseNumber { get; set; }
        public string City { get; set; }
        public string CityCode { get; set; }
        public string HomePhoneNumber { get; set; }
        public string Email { get; set; }
        public DateTime? DateArrival { get; set; } //When person started working at the company
        public DateTime? DateDeparture { get; set; } //When person stopped working at the company
    }

    [Flags]
    public enum CardSettings : uint
    {
        NONE = 0x00000000,
        LOST_CARD = 0x00000001,
        ACCESS_CARD = 0x00000002,
        SPECIAL_CARD = 0x00000004,
        SPECIAL_ISSUE = 0x00000008,
        SPECIAL_GROUP = 0x00000010,
        ESD_BOTH_TESTS = 0x00000020,    // card holder has to complete both ESD tests before opening door for him (action OPENDOOR_WITH_ESDCHECK)
        USE_PIN = 0x80000000,
    }

    //Basic object for sending of Card from Timetec to NOVA (callback WCF call)
    public class CardObject
    {
        public string CardNumber { get; set; }
        public string PersonalNumber { get; set; } //Owner Id
        public DateTime? CardSince { get; set; } //Start of card validity
        public DateTime? CardUntill { get; set; } //End of card validity
        public string PIN { get; set; }
        public string Description { get; set; }
        public CardSettings Settings { get; set; } //Card settings:

        public long ID { get; set; } //Card Id: 
    }

    //Basic object for sending of Person with Cards from Timetec to NOVA (callback WCF call)
    public class PersonWithCardsObject
    {
        public PersonObject Person { get; set; }
        public List<CardObject> Cards { get; set; }

        public PersonWithCardsObject(PersonObject pPerson, List<CardObject> pCards)
        {
            Person = pPerson;
            Cards = pCards;
        }
    }
}
