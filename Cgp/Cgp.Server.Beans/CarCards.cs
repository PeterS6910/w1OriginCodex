using System;
using Contal.Cgp.Globals;

namespace Contal.Cgp.Server.Beans
{
    [Serializable]
    public class CarCard : AOrmObject
    {
        public const string COLUMN_ID_CAR_CARD = "IdCarCard";
        public const string COLUMN_CAR = "IdCar";
        public const string COLUMN_CARD = "IdCard";

        public virtual Guid IdCarCard { get; set; }
        public virtual Car Car { get; set; }
        public virtual Card Card { get; set; }

        public CarCard()
        {
        }

        public CarCard(Car car, Card card)
        {
            Car = car;
            Card = card;
        }

        public override bool Compare(object obj)
        {
            var carCard = obj as CarCard;
            return carCard != null && carCard.IdCarCard.Equals(IdCarCard);
        }

        public override string GetIdString()
        {
            return IdCarCard.ToString();
        }

        public override ObjectType GetObjectType()
        {
            return ObjectType.NotSupport;
        }

        public override object GetId()
        {
            return IdCarCard;
        }

        public override IModifyObject CreateModifyObject()
        {
            return null;
        }
    }
}
