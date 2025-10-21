using System;

using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class CardPair : AOrmObject
    {
        public const string COLUMNFIRSTCARD = "FirstCard";
        public const string COLUMNSECONDCARD = "SecondCard";

        public virtual Guid FirstCard { get; set; }
        public virtual Guid SecondCard { get; set; }
        public virtual byte ObjectType { get; set; }

        public override bool Equals(object obj)
        {
            var cardPair = obj as CardPair;

            if (cardPair == null)
                return false;

            return 
                FirstCard.Equals(cardPair.FirstCard) && 
                SecondCard.Equals(cardPair.SecondCard);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public CardPair()
        {
            ObjectType = (byte)Globals.ObjectType.CardPair;
        }

        public override string ToString()
        {
            return FirstCard + "|" + SecondCard;
        }

        public override bool Compare(object obj)
        {
            var pair = obj as CardPair;
            if (pair == null)
                return false;

            return pair.FirstCard.Equals(FirstCard) && pair.SecondCard.Equals(SecondCard);
        }

        public override string GetIdString()
        {
            return ToString();
        }

        public override ObjectType GetObjectType()
        {
            return Globals.ObjectType.CardPair;
        }

        public override object GetId()
        {
            return this;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }
}
