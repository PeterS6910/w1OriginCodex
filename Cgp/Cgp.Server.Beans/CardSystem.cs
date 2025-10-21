using System;
using System.Collections.Generic;
using System.Reflection;

using Contal.IwQuick.Data;
using Contal.Cgp.Globals;
using System.ComponentModel;
using System.Drawing;

namespace Contal.Cgp.Server.Beans
{
    class CardTypeAttrAttribute : NameAttribute
    {
        public int CardDataLengthMin { get; protected set; }
        public int CatdDataLengthMax { get; protected set; }

        public CardTypeAttrAttribute(string name, int cardDataLengthMin, int cardDataLengthMax)
            : base(name)
        {
            CardDataLengthMin = cardDataLengthMin;
            CatdDataLengthMax = cardDataLengthMax;
        }
    }

    public enum CardType : byte
    {
        [CardTypeAttr("Proxy", 1, 14)]
        Proxy = 0,
        [CardTypeAttr("Mifare", 2, 30)]
        Mifare = 1,
        [CardTypeAttr("Mifare Plus", 2, 30)]
        MifarePlus = 2,
        [CardTypeAttr("Direct serial (Proxy/Mifare)", 2, 30)]
        DirectSerial = 3,
        [CardTypeAttr("Magnetic", 0, 38)]
        Magnetic = 4
        //[CardTypeAttr("Wiegand", 0, 18)]
        //Wiegand = 3
    }

    public enum CardSize : byte
    {
        OneKB = 1,
        TwoKB = 2,
        FourKB = 4,
        SevenKB = 7
    }

    public class CardTypes
    {
        private CardType _value;
        public CardType Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public byte ValueByte
        {
            get { return (byte)_value; }
            set { _value = (CardType)value; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly int _cardDataLengthMin;
        public int CardDataLengthMin
        {
            get { return _cardDataLengthMin; }
        }

        private readonly int _cardDataLengthMax;
        public int CardDataLengthMax
        {
            get { return _cardDataLengthMax; }
        }

        public CardTypes(CardType value, string name, int cardDataLengthMin, int cardDataLengthMax)
        {
            _value = value;
            _name = name;
            _cardDataLengthMin = cardDataLengthMin;
            _cardDataLengthMax = cardDataLengthMax;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<CardTypes> GetCardTypesList()
        {
            IList<CardTypes> list = new List<CardTypes>();
            FieldInfo[] fieldsInfo = typeof(CardType).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                CardTypeAttrAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(CardTypeAttrAttribute), false) as CardTypeAttrAttribute[];

                if (attribs.Length > 0)
                {
                    list.Add(new CardTypes((CardType)fieldInfo.GetValue(fieldInfo), attribs[0].Name, attribs[0].CardDataLengthMin, attribs[0].CatdDataLengthMax));
                }
            }

            return list;
        }

        public static CardTypes GetCardType(byte cardType)
        {
            FieldInfo[] fieldsInfo = typeof(CardType).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                CardTypeAttrAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(CardTypeAttrAttribute), false) as CardTypeAttrAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == cardType)
                        return new CardTypes((CardType)fieldInfo.GetValue(fieldInfo), attribs[0].Name, attribs[0].CardDataLengthMin, attribs[0].CatdDataLengthMax);
                }
            }

            return null;
        }

        public static CardTypes GetCardType(IList<CardTypes> listCardTypes, byte cardType)
        {
            if (listCardTypes == null)
            {
                return GetCardType(cardType);
            }
            foreach (CardTypes listCardType in listCardTypes)
            {
                if ((byte)listCardType.Value == cardType)
                    return listCardType;
            }

            return null;
        }
    }

    class CardSubTypeAttrAttribute : NameAttribute
    {
        public string Number { get; protected set; }
        public CardType CardType { get; protected set; }

        public CardSubTypeAttrAttribute(string name, string number, CardType cardType)
            : base(name)
        {
            Number = number;
            CardType = cardType;
        }
    }

    public enum CardSubType : byte
    {
        [CardSubTypeAttr("Read-only", "0", CardType.Proxy)]
        ProxyReadOnly = 0,
        [CardSubTypeAttr("Read/Write", "1", CardType.Proxy)]
        ProxyReadWrite = 1,
        [CardSubTypeAttr("Serial number", "20", CardType.Mifare)]
        MifareSerialNumber = 2,
        [CardSubTypeAttr("Standard sector reading", "40", CardType.Mifare)]
        MifareStandardSectorReadin = 4,
        [CardSubTypeAttr("Sector reading with MAD", "41", CardType.Mifare)]
        MifareSectorReadinWithMAD = 5,
        [CardSubTypeAttr("Security level1 without MAD", "42", CardType.MifarePlus)]
        MifarePlusSecurityLevel1WithoutMAD = 6,
        [CardSubTypeAttr("Security level1 with MAD", "43", CardType.MifarePlus)]
        MifarePlusSecurityLevel1WithMAD = 7,
        [CardSubTypeAttr("Security level1 +AES without MAD", "44", CardType.MifarePlus)]
        MifarePlusSecurityLevel1AESWithoutMAD = 8,
        [CardSubTypeAttr("Security level1 +AES with MAD", "45", CardType.MifarePlus)]
        MifarePlusSecurityLevel1AESWithMAD = 9,
        [CardSubTypeAttr("Security level2 without MAD", "46", CardType.MifarePlus)]
        MifarePlusSecurityLevel2WithoutMAD = 10,
        [CardSubTypeAttr("Security level2 with MAD", "47", CardType.MifarePlus)]
        MifarePlusSecurityLevel2WithMAD = 11,
        [CardSubTypeAttr("Security level3 without MAD", "48", CardType.MifarePlus)]
        MifarePlusSecurityLevel3WithoutMAD = 12,
        [CardSubTypeAttr("Security level3 with MAD", "49", CardType.MifarePlus)]
        MifarePlusSecurityLevel3WithMAD = 13,
        [CardSubTypeAttr("Security level3-virtual card without MAD", "4A", CardType.MifarePlus)]
        MifarePlusSecurityLevel3VirtualCardWithoutMAD = 14,
        [CardSubTypeAttr("Security level3-virtual card with MAD", "4B", CardType.MifarePlus)]
        MifarePlusSecurityLevel3VirtualCardWithMAD = 15,
        [CardSubTypeAttr("Serial number", "", CardType.Proxy)]
        ProxySerialNumber = 16,
        [CardSubTypeAttr("Serial number without prefix", "", CardType.Mifare)]
        MifareSerialNumberWithoutPrefix = 17,
        //this is added because card sub type can not be null (it is part of a database unique key)
        [CardSubTypeAttr("None", "", CardType.Magnetic)]
        None = 18,
    }

    public class CardSubTypes
    {
        private CardSubType _value;
        public CardSubType Value
        {
            get { return _value; }
        }

        public byte ValueByte
        {
            get { return (byte)_value; }
            set { _value = (CardSubType)value; }
        }

        private readonly string _name;
        public string Name
        {
            get { return _name; }
        }

        private readonly string _number;
        public string Number
        {
            get { return _number; }
        }

        private readonly CardType _cardType;
        public CardType CardType
        {
            get { return _cardType; }
        }

        public CardSubTypes(CardSubType value, string name, string number, CardType cardType)
        {
            _value = value;
            _name = name;
            _number = number;
            _cardType = cardType;
        }

        public override string ToString()
        {
            return Name;
        }

        public static IList<CardSubTypes> GetCardSubTypesList(CardType cardType)
        {
            IList<CardSubTypes> list = new List<CardSubTypes>();
            FieldInfo[] fieldsInfo = typeof(CardSubType).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                CardSubTypeAttrAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(CardSubTypeAttrAttribute), false) as CardSubTypeAttrAttribute[];

                if (attribs.Length > 0)
                {
                    if (attribs[0].CardType == cardType)
                        list.Add(new CardSubTypes((CardSubType)fieldInfo.GetValue(fieldInfo), attribs[0].Name, attribs[0].Number, attribs[0].CardType));
                }
            }

            return list;
        }

        public static CardSubTypes GetCardSubType(byte cardSubType)
        {
            FieldInfo[] fieldsInfo = typeof(CardSubType).GetFields();
            foreach (FieldInfo fieldInfo in fieldsInfo)
            {
                CardSubTypeAttrAttribute[] attribs = fieldInfo.GetCustomAttributes(
                typeof(CardSubTypeAttrAttribute), false) as CardSubTypeAttrAttribute[];

                if (attribs.Length > 0)
                {
                    if ((byte)fieldInfo.GetValue(fieldInfo) == cardSubType)
                        return new CardSubTypes((CardSubType)fieldInfo.GetValue(fieldInfo), attribs[0].Name, attribs[0].Number, attribs[0].CardType);
                }
            }

            return null;
        }

        public static CardSubTypes GetCardSubType(IList<CardSubTypes> listCardSubTypes, byte cardSubType)
        {
            if (listCardSubTypes == null)
            {
                return GetCardSubType(cardSubType);
            }
            foreach (CardSubTypes listCardSubType in listCardSubTypes)
            {
                if ((byte)listCardSubType.Value == cardSubType)
                    return listCardSubType;
            }

            return null;
        }
    }

    public enum EncodingType : byte
    {
        [Name("BCD")]
        Bcd = 0,
        [Name("ASCII")]
        Ascii = 1,
        [Name("HEX 8")]
        Hex8 = 2,
        [Name("HEX 16")]
        Hex16 = 3,
        [Name("HEX 32")]
        Hex32 = 4,
        [Name("HEX 64")]
        Hex64 = 5
    }

    [Serializable]
    [LwSerialize(203)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    [LwSerializeNoParent]
    public class CardSystem : AOrmObjectWithVersion, IOrmObjectWithAlarmInstructions, INotifyPropertyChanged
    {
        public const string COLUMNIDCARDSYSTEM = "IdCardSystem";
        public const string COLUMNNAME = "Name";
        public const string COLUMNLENGTHCARDDATA = "LengthCardData";
        public const string COLUMNLENGTHCOMPANYCODE = "LengthCompanyCode";
        public const string COLUMNCOMPANYCODE = "CompanyCode";
        public const string COLUMNFULLCOMPANYCODE = "FullCompanyCode";
        public const string COLUMNCARDTYPE = "CardType";
        public const string COLUMNCARDSUBTYPE = "CardSubType";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMNCARDS = "Cards";
        public const string COLUMNAID = "AID";
        public const string COLUMNAKEY = "Akey";
        public const string COLUMNBKEY = "Bkey";
        public const string COLUMNBANK1ENCODING = "Bank1Encoding";
        public const string COLUMNBAK1SECTOR = "Bank1Sector";
        public const string COLUMNBANK1OFFSET = "Bank1Offset";
        public const string COLUMNBANK1LENGTH = "Bank1Length";
        public const string COLUMNBANK2ENCODING = "Bank2Encoding";
        public const string COLUMNBAK2SECTOR = "Bank2Sector";
        public const string COLUMNBANK2OFFSET = "Bank2Offset";
        public const string COLUMNBANK2LENGTH = "Bank2Length";
        public const string COLUMNBANK3ENCODING = "Bank3Encoding";
        public const string COLUMNBAK3SECTOR = "Bank3Sector";
        public const string COLUMNBANK3OFFSET = "Bank3Offset";
        public const string COLUMNBANK3LENGTH = "Bank3Length";
        public const string COLUMNAKEYBANK1 = "AKeyBank1";
        public const string COLUMNAKEYBANK2 = "AKeyBank2";
        public const string COLUMNAKEYBANK3 = "AKeyBank3";
        public const string COLUMNBKEYBANK1 = "BKeyBank1";
        public const string COLUMNBKEYBANK2 = "BKeyBank2";
        public const string COLUMNBKEYBANK3 = "BKeyBank3";
        public const string COLUMNOBJECTTYPE = "ObjectType";
        public const string COLUMNEXPLICITSMARTCARDDATAPOPULATION = "ExplicitSmartCardDataPopulation";
        public const string COLUMNLOCALALARMINSTRUCTION = "LocalAlarmInstruction";
        public const string ColumnVersion = "Version";

        [LwSerialize]
        public virtual Guid IdCardSystem { get; set; }
        public virtual byte CardSystemNumber { get; set; }
        public virtual string Name { get; set; }
        private byte _lengthCardData;
        public virtual byte LengthCardData
        {
            get
            {
                return _lengthCardData;
            }
            set
            {
                if (_lengthCardData == value)
                    return;

                _lengthCardData = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("LengthCardData"));
            }
        }
        public virtual byte LengthCompanyCode { get; set; }
        public virtual string CompanyCode { get; set; }
        private string _fullCompanyCode = string.Empty;
        [LwSerialize]
        public virtual string FullCompanyCode { get { return _fullCompanyCode; } set { _fullCompanyCode = value; } }
        private byte _cardType;
        [LwSerialize]
        public virtual byte CardType
        {
            get
            {
                return _cardType;
            }
            set
            {
                if (_cardType == value)
                    return;
                _cardType = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CardType"));
            }
        }
        private byte _cardSubType;
        [LwSerialize]
        public virtual byte CardSubType
        {
            get
            {
                return _cardSubType;
            }
            set
            {
                if (_cardSubType == value)
                    return;
                _cardSubType = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("CardSubType"));
            }
        }
        public virtual string Description { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual byte ObjectType { get; set; }
        private bool _explicitSmartCardDataPopulation;
        public virtual bool ExplicitSmartCardDataPopulation
        {
            get
            {
                return _explicitSmartCardDataPopulation;
            }
            set
            {
                _explicitSmartCardDataPopulation = value;
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ExplicitSmartCardDataPopulation"));
            }
        }
        public virtual string LocalAlarmInstruction { get; set; }

        private AOrmObject _cardData;
        public virtual AOrmObject CardData { get { return _cardData; } set { _cardData = value; } }

        //property for Smart card data
        [LwSerialize]
        public virtual byte[] SmartCardDataForCCU { get; set; }

        public CardSystem()
        {
            ObjectType = (byte)Globals.ObjectType.CardSystem;
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Compare(object obj)
        {
            if (obj == null)
                return false;

            if (obj is CardSystem)
            {
                return (obj as CardSystem).IdCardSystem == IdCardSystem;
            }
            return false;
        }

        public virtual string GetFullCompanyCode(CardSubTypes cardSubType, string CompanyCode)
        {
            string fullCompanyCode = CompanyCode;

            if (cardSubType != null)
            {
                fullCompanyCode = cardSubType.Number + fullCompanyCode;
            }

            return fullCompanyCode;
        }

        public virtual string GetFullCompanyCode()
        {
            string fullCompanyCode = CompanyCode;
            
            CardSubTypes cardSubType = CardSubTypes.GetCardSubType(CardSubType);
            if (cardSubType != null)
            {
                fullCompanyCode = cardSubType.Number + fullCompanyCode;
            }

            return fullCompanyCode;
        }

        public virtual int MaxLengthCardNumber(int cardDataLength, CardSubTypes cardSubType)
        {
            if (cardSubType != null)
            {
                return cardDataLength - cardSubType.Number.Length;
            }
            return cardDataLength;
        }

        public virtual int LengthCardNumber()
        {
            int lengthCardNumber = LengthCardData - LengthCompanyCode;

            
            CardSubTypes cardSubType = CardSubTypes.GetCardSubType(CardSubType);
            if (cardSubType != null)
            {
                lengthCardNumber -= cardSubType.Number.Length;
            }
        

            return lengthCardNumber;
        }

        public virtual void PrepareToSend()
        {
            FullCompanyCode = GetFullCompanyCode();
            if (CardData is MifareSectorData)
                SmartCardDataForCCU = (CardData as MifareSectorData).GetSmartCardDataForCCU();
            else
                SmartCardDataForCCU = null;
        }

        public override bool Contains(string expression)
        {
            expression = expression.ToLower();
            if (Name.ToLower().Contains(expression)) return true;
            if (Description != null)
            {
                if (Description.ToLower().Contains(expression)) return true;
            }
            return false;
        }

        public override string GetIdString()
        {
            return IdCardSystem.ToString();
        }

        public override object GetId()
        {
            return IdCardSystem;
        }

        public override IModifyObject CreateModifyObject()
        {
            return new CardSystemModifyObj(this);
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.CardSystem;
        }

        public virtual string GetLocalAlarmInstruction()
        {
            return LocalAlarmInstruction;
        }

        #region INotifyPropertyChanged Members

        [field: NonSerialized]
        public virtual event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }

    [Serializable]
    public class CardSystemShort : IShortObject
    {
        public const string COLUMNIDCARDSYSTEM = "IdCardSystem";
        public const string COLUMNNAME = "Name";
        public const string COLUMNFULLCOMPANYCODE = "FullCompanyCode";
        public const string COLUMNCARDTYPE = "CardType";
        public const string COLUMNSTRINGCARDTYPE = "StringCardType";
        public const string COLUMNDESCRIPTION = "Description";
        public const string COLUMN_SYMBOL = "Symbol";

        public Guid IdCardSystem { get; set; }
        public string Name { get; set; }
        public string FullCompanyCode { get; set; }
        public byte CardType { get; set; }
        public string StringCardType { get; set; }
        public string Description { get; set; }
        public Image Symbol { get; set; }

        public CardSystemShort(CardSystem cardSystem)
        {
            IdCardSystem = cardSystem.IdCardSystem;
            Name = cardSystem.Name;
            FullCompanyCode = cardSystem.GetFullCompanyCode();
            CardType = cardSystem.CardType;
            Description = cardSystem.Description;
        }

        public override string ToString()
        {
            return Name;
        }

        public object Id { get { return IdCardSystem; } }

        #region IShortObject Members

        public ObjectType ObjectType { get { return ObjectType.CardSystem; } }

        public string GetSubTypeImageString(object value)
        {
            return string.Empty;
        }

        #endregion
    }

    [Serializable]
    public class CardSystemModifyObj : AModifyObject
    {
        public override ObjectType GetOrmObjectType { get { return ObjectType.CardSystem; } }

        public CardSystemModifyObj(CardSystem cardSystem)
        {
            Id = cardSystem.IdCardSystem;
            FullName = cardSystem.ToString();
            Description = cardSystem.Description;
        }
    }
}