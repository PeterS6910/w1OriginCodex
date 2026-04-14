using System;
using System.Collections.Generic;

using Contal.IwQuick;
using Contal.IwQuick.Crypto;
using Contal.IwQuick.Data;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Remoting;
using Contal.Cgp.Globals;
using System.Drawing;
using System.Collections;

namespace Contal.Cgp.Server.Beans
{
    public class NameAttribute : Attribute
    {
        public string Name { get; protected set; }

        public NameAttribute(string value)
        {
            Name = value;
        }
    }

    public enum CardState : byte
    {
        Active = 0x00,
        Blocked = 0x01,
        Unused = 0x02,
        Lost = 0x03,
        Destroyed = 0x04,
        TemporarilyBlocked = 0x05,
        HybridActive = 0x10,
        HybridBlocked = 0x11,
        HybridUnused = 0x12,
        HybridLost = 0x13,
        HybridDestroyed = 0x14,
        HybridTemporarilyBlocked = 0x15
    }

    public class CardStates
    {
        private readonly CardState _value;
        public CardState Value
        {
            get { return _value; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        public CardStates(CardState value, string name)
        {
            _value = value;
            _name = name;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<CardStates> GetCardStatesList(LocalizationHelper localizationHelper)
        {
            IList<CardStates> list = new List<CardStates>();

            var names = Enum.GetNames(typeof(CardState));
            if (names == null)
                return list;

            foreach (var name in names)
                list.Add(
                    new CardStates(
                        (CardState)Enum.Parse(
                            typeof(CardState),
                            name),
                        localizationHelper.GetString("CardStates_" + name)));

            return list;
        }

        public static CardStates GetCardState(LocalizationHelper localizationHelper, IList<CardStates> listCardStates, byte cardState)
        {
            if (listCardStates == null)
                return GetCardState(
                    localizationHelper,
                    cardState);

            foreach (var listCardState in listCardStates)
                if ((byte)listCardState.Value == cardState)
                    return listCardState;

            return null;
        }

        public static CardStates GetCardState(LocalizationHelper localizationHelper, byte cardState)
        {
            var name = Enum.GetName(
                typeof(CardState),
                cardState);

            return name != null
                ? new CardStates(
                    (CardState)cardState, 
                    localizationHelper.GetString("CardStates_" + name))
                : null;
        }
    }

    public class CardGenerationSettings
    {
        public string FullPrefix { get; private set; }
        public byte CardSystem { get; private set; }
        public int NumberDigits { get; private set; }
        public Decimal FromNumber { get; private set; }
        public Decimal ToNumber { get; private set; }
        public string Pin { get; private set; }
        public CardState CardState { get; private set; }
        public int? IdSubSite { get; private set; }


        public CardGenerationSettings(
            string fullPrefix,
            byte cardSystem,
            int numberDigits,
            decimal fromNumber,
            decimal toNumber,
            string pin,
            CardState cardState,
            int? idSubSite)
        {
            FullPrefix = fullPrefix;
            CardSystem = cardSystem;
            NumberDigits = numberDigits;
            FromNumber = fromNumber;
            ToNumber = toNumber;
            Pin = pin;
            CardState = cardState;
            IdSubSite = idSubSite;
        }
    }

    [Serializable]
    [LwSerialize(202)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class Card : AOrmObjectWithVersion, IOrmObjectWithAlarmInstructions, IComparer
    {
        public const int MinimalPinLength = 4;
        public const int MaximalPinLength = 12;

        public const string COLUMNIDCARD = "IdCard";
        public const string COLUMNCARDSYSTEM = "CardSystem";
        public const string COLUMNGUIDCARDSYSTEM = "GuidCardSystem";
        public const string COLUMNPERSON = "Person";
        public const string COLUMNGUIDPERSON = "GuidPerson";
        public const string COLUMNNUMBER = "Number";
        public const string COLUMNFULLCARDNUMBER = "FullCardNumber";
        public const string COLUMNPIN = "Pin";
        public const string COLUMNPINLENGTH = "PinLength";
        public const string COLUMNSTATE = "State";
        public const string COLUMNDATESTATELASTCHANGE = "DateStateLastChange";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNNAME = "Name";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNCKUNIQUE = "CkUnique";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string COLUMNALTERNATECARDNUMBER = "AlternateCardNumber";
        public const string COLUMNLASTPRINTENCODETEMPLATE = "LastPrintEncodeTemplate";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdCard { get; set; }
        public virtual string Number { get; set; }
        [LwSerialize]
        public virtual string FullCardNumber { get; set; }
        public virtual CardSystem CardSystem { get; set; }
        private Guid _guidCardSystem = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidCardSystem { get { return _guidCardSystem; } set { _guidCardSystem = value; } }
        private Person _person;
        public virtual Person Person
        {
            get { return _person; }
            set
            {
                _person = value;
                _guidPerson = _person != null ? _person.IdPerson : Guid.Empty;
            }
        }
        private Guid _guidPerson = Guid.Empty;
        [LwSerialize]
        public virtual Guid GuidPerson
        {
            get { return _guidPerson; }
            set
            {
                _guidPerson = value;
                if (_person != null && _person.IdPerson != _guidPerson)
                {
                    _person = null;
                }
            }
        }
        [LwSerialize]
        public virtual string Pin { get; set; }
        [LwSerialize]
        public virtual byte PinLength { get; set; }
        [LwSerialize]
        public virtual byte State { get; set; }
        public virtual DateTime DateStateLastChange { get; set; }
        [LwSerialize]
        public virtual DateTime UtcDateStateLastChange { get; set; }

        public virtual string Description { get; set; }
        public virtual string Name { get; set; }
        public virtual byte ObjectType { get; set; }
        public virtual Guid CkUnique { get; set; }
        public virtual string LocalAlarmInstruction { get; set; }
        public virtual string AlternateCardNumber { get; set; }
        public virtual Guid LastPrintEncodeTemplate { get; set; }

        [LwSerialize]
        public virtual DateTime? ValidityDateFrom { get; set; }
        [LwSerialize]
        public virtual DateTime? ValidityDateTo { get; set; }
        public virtual bool SynchronizedWithTimetec { get; set; }

        public Card()
        {
            Name = string.Empty;
            ObjectType = (byte)Globals.ObjectType.Card;
            CkUnique = Guid.NewGuid();
            State = (byte)CardState.Unused;
        }

        public Card(ImportCardData importCardData)
            : this()
        {
            if (importCardData == null)
                return;

            Number = importCardData.CardNumber;
            Pin = importCardData.Pin;
            PinLength = importCardData.PinLength;
        }

        public override string ToString()
        {
            return GetFullCardNumber();
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Card)
            {
                return (obj as Card).IdCard == IdCard;
            }
            return false;
        }

        public virtual string GetFullCardNumber(CardSystem cardSystem, string cardNumber)
        {
            if (cardSystem != null)
                cardNumber = cardSystem.GetFullCompanyCode() + cardNumber;

            return cardNumber;
        }

        public virtual string GetFullCardNumber()
        {
            if (CardSystem != null)
                return CardSystem.GetFullCompanyCode() + Number;
            return Number;
        }

        public virtual void PrepareToSend()
        {
            GuidCardSystem = CardSystem != null
                ? CardSystem.IdCardSystem
                : Guid.Empty;

            GuidPerson = Person != null
                ? Person.IdPerson
                : Guid.Empty;
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();

            if (Number.Contains(expression)) 
                return true;

            return Description != null
                   && Description.ToLower().Contains(expression);
        }

        public override string GetIdString()
        {
            return IdCard.ToString();
        }

        public override object GetId()
        {
            return IdCard;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new CardModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.Card;
        }

        public override string GetSubTypeImageString(string subType)
        {
            return subType == "State"
                ? Enum.GetName(
                    typeof(CardState),
                    State)
                : string.Empty;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        public virtual bool IsValid
        {
            get
            {
                var actualDate = DateTime.Now;

                bool isValid = true;

                if (ValidityDateFrom != null)
                    isValid = ValidityDateFrom.Value <= actualDate;

                if (ValidityDateTo != null)
                    isValid = ValidityDateTo.Value >= actualDate;

                return isValid;

            }
        }

        #region IComparer Members

        public virtual int Compare(object x, object y)
        {
            var cX = x as Card;
            var cY = y as Card;

            if (cX == null && cY == null)
                return 0;

            if (cX != null && cY == null)
                return 1;

            if (cX == null && cY != null)
                return -1;

            if (cX.FullCardNumber == null && cY.FullCardNumber == null)
                return 0;

            if (cX.FullCardNumber != null && cY.FullCardNumber == null)
                return 1;

            if (cX.FullCardNumber == null && cY.FullCardNumber != null)
                return -1;

            return (cX.FullCardNumber.CompareTo(cY.FullCardNumber));
        }

        #endregion
    }

    [Serializable]
    public class CardShort : IShortObject
    {
        public const string COLUMN_ID_CARD = "IdCard";
        public const string COLUMN_NUMBER = "Number";
        public const string COLUMN_FULL_CARD_NUMBER = "FullCardNumber";
        public const string COLUMN_CARD_SYSTEM = "CardSystem";
        public const string COLUMN_PERSONAL_ID = "PersonalID";
        public const string COLUMN_PERSON = "Person";
        public const string COLUMN_CARD_STATE = "CardState";
        public const string COLUMN_STATE = "StringCardState";
        public const string COLUMN_DESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public const string COLUMN_TIMETEC_SYNC = "TimetecSync";
        public const string COLUMN_TIMETEC_VALIDFROM = "TimetecValidFrom";
        public const string COLUMN_TIMETEC_VALIDTO = "TimetecValidTo";

        public Guid IdCard { get; set; }
        public string Number { get; set; }
        public string FullCardNumber { get; set; }
        public string CardSystem { get; set; }
        public string PersonalID { get; set; }
        public string Person { get; set; }
        public byte CardState { get; set; }
        public string StringCardState { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        // Timetec fields
        public string TimetecSync { get; set; }
        public string TimetecValidFrom { get; set; }
        public string TimetecValidTo { get; set; }

        public CardShort(Card card)
        {
            IdCard = card.IdCard;
            Number = card.Number;

            CardSystem = 
                card.CardSystem != null 
                    ? card.CardSystem.ToString() : 
                    string.Empty;

            FullCardNumber = card.FullCardNumber;

            PersonalID = card.Person != null ? card.Person.Identification : string.Empty;

            Person = card.Person != null ? card.Person.ToString() : string.Empty;

            CardState = card.State;
            Description = card.Description;

            TimetecSync = card.SynchronizedWithTimetec.ToString();
            TimetecValidFrom = card.ValidityDateFrom.HasValue ? String.Format("{0:dd. MM. yyyy}", card.ValidityDateFrom) : "-";
            TimetecValidTo = card.ValidityDateTo.HasValue ? String.Format("{0:dd. MM. yyyy}", card.ValidityDateTo) : "-";
        }

        public override string ToString()
        {
            return FullCardNumber;
        }

        public object Id { get { return IdCard; } }

        public string Name { get { return Number; } }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.Card; } }

        public string GetSubTypeImageString(object value)
        {
            if (value is byte)
            {
                //State to string is same as image key prefix
                return ((CardState)CardState).ToString();
            }

            return string.Empty;
        }

        #endregion
    }

    [Serializable]
    public class CardModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.Card; } }


        public override string GetObjectSubType(byte option)
        {
            switch (option)
            {
                case 0: return ObjectSubTypes[0];
            }
            return ObjectSubTypes[0];
        }

        public CardModifyObj(Card card)
        {
            Id = card.IdCard;
            FullName = card.ToString();
            Description = card.Description;
            ObjectSubTypes = new string[1];
            ObjectSubTypes[0] = card.GetSubTypeImageString("State");
        }
    }

    public class CardGenerationFinisheEventHandler : ARemotingCallbackHandler
    {
        private static volatile CardGenerationFinisheEventHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, int> _cardGenerationFinished;

        public static CardGenerationFinisheEventHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new CardGenerationFinisheEventHandler();
                    }

                return _singleton;
            }
        }

        public CardGenerationFinisheEventHandler()
            : base("CardGenerationFinisheEventHandler")
        {
        }

        public void RegisterCardGenerationFinished(Action<Guid, int> cardGenerationFinished)
        {
            _cardGenerationFinished += cardGenerationFinished;
        }

        public void UnregisterCardGenerationFinished(Action<Guid, int> cardGenerationFinished)
        {
            _cardGenerationFinished -= cardGenerationFinished;
        }

        public void RunEvent(Guid id, int successCount)
        {
            if (_cardGenerationFinished != null)
                _cardGenerationFinished(id, successCount);
        }
    }

    [Serializable]
    public class CSVImportCard
    {
        readonly Guid _idCard;
        readonly string _number;
        readonly CSVImportResult _importResult;

        public Guid IdCard { get { return _idCard; } }
        public string Number { get { return _number; } }
        public CSVImportResult ImportResult { get { return _importResult; } }

        public CSVImportCard(Guid idCard, string number, CSVImportResult importResult)
        {
            _idCard = idCard;
            _number = number;
            _importResult = importResult;
        }
    }

    [Serializable]
    public class ImportCardData
    {
        private readonly string _identification;
        private readonly string _cardNumber;
        private readonly string _pin;
        private readonly byte _pinLength;

        public string Identification { get { return _identification; } }
        public string CardNumber { get { return _cardNumber; } }
        public string Pin { get { return _pin; } }
        public byte PinLength { get { return _pinLength; } }

        public ImportCardData(string identification, string cardNumber, string pin, byte pinLength)
        {
            _identification = identification;
            _cardNumber = cardNumber;
            _pin = QuickHashes.GetCRC32String(pin);
            _pinLength = pinLength;
        }
    }

    public class ImportedCardCountChangedHandler : ARemotingCallbackHandler
    {
        private static volatile ImportedCardCountChangedHandler _singleton;
        private static readonly object _syncRoot = new object();

        private Action<Guid, int> _importedCardCountChanged;

        public static ImportedCardCountChangedHandler Singleton
        {
            get
            {
                if (_singleton == null)
                    lock (_syncRoot)
                    {
                        if (_singleton == null)
                            _singleton = new ImportedCardCountChangedHandler();
                    }

                return _singleton;
            }
        }

        public ImportedCardCountChangedHandler()
            : base("ImportedCardCountChangedHandler")
        {
        }

        public void RegisterImportedCardCountChanged(Action<Guid, int> importedCardCountChanged)
        {
            _importedCardCountChanged += importedCardCountChanged;
        }

        public void UnregisterImportedCardCountChanged(Action<Guid, int> importedCardCountChanged)
        {
            _importedCardCountChanged -= importedCardCountChanged;
        }

        public void RunEvent(Guid formIdentification, int percent)
        {
            if (_importedCardCountChanged != null)
                _importedCardCountChanged(formIdentification, percent);
        }
    }
}
