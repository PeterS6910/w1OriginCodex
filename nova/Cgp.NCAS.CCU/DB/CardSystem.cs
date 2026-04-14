using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;

using Contal.IwQuick.Data;
using Contal.LwSerialization;

namespace Contal.Cgp.NCAS.CCU.DB
{
    class CardTypeAttrAttribute : NameAttribute
    {
        public int CardDataLengthMin { get; protected set; }
        public int CatdDataLengthMax { get; protected set; }

        public CardTypeAttrAttribute(string name, int cardDataLengthMin, int cardDataLengthMax)
            : base(name)
        {
            this.CardDataLengthMin = cardDataLengthMin;
            this.CatdDataLengthMax = cardDataLengthMax;
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
        DirectSerial = 3
        //[CardTypeAttr("Wiegand", 0, 18)]
        //Wiegand = 3
    }

    public class CardTypes
    {
        private CardType _value;
        public CardType Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private int _cardDataLengthMin;
        public int CardDataLengthMin
        {
            get { return _cardDataLengthMin; }
        }

        private int _cardDataLengthMax;
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
            else
            {
                foreach (CardTypes listCardType in listCardTypes)
                {
                    if ((byte)listCardType.Value == cardType)
                        return listCardType;
                }
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
            this.Number = number;
            this.CardType = cardType;
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
        [CardSubTypeAttr("Random serial number", "2E", CardType.Mifare)]
        MifareRandomSerialNumber = 3,
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
        MifareSerialNumberWithoutPrefix = 17
    }

    public class CardSubTypes
    {
        private CardSubType _value;
        public CardSubType Value
        {
            get { return _value; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
        }

        private string _number;
        public string Number
        {
            get { return _number; }
        }

        private CardType _cardType;
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
            Dictionary<byte, CardSubTypes> list = new Dictionary<byte, CardSubTypes>();
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
            else
            {
                foreach (CardSubTypes listCardSubType in listCardSubTypes)
                {
                    if ((byte)listCardSubType.Value == cardSubType)
                        return listCardSubType;
                }
            }

            return null;
        }
    }

    [LwSerialize(203)]
    [LwSerializeMode(LwSerializationMode.Selective, DirectMemberType.All)]
    public class CardSystem : IDbObject
    {
        [LwSerialize()]
        public virtual Guid IdCardSystem { get; set; }
        private string _fullCompanyCode = string.Empty;
        [LwSerialize()]
        public virtual string FullCompanyCode { get { return _fullCompanyCode; } set { _fullCompanyCode = value; } }
        [LwSerialize()]
        public virtual byte CardType { get; set; }
        [LwSerialize()]
        public virtual byte CardSubType { get; set; }

        [LwSerialize()]
        public virtual byte[] SmartCardDataForCCU { get; set; }

        public Guid GetGuid()
        {
            return IdCardSystem;
        }

        public Contal.Cgp.Globals.ObjectType GetObjectType()
        {
            return Contal.Cgp.Globals.ObjectType.CardSystem;
        }
    }
}