using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public sealed class CardSystemData : ABaseOrmTable<CardSystemData, AOrmObject>
    {
        private CardSystemData() : base(null)
        {
        }

        public override bool HasAccessView(Login login)
        {
            return CardSystems.Singleton.HasAccessView(login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return CardSystems.Singleton.HasAccessInsert(login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return CardSystems.Singleton.HasAccessUpdate(login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return CardSystems.Singleton.HasAccessDelete(login);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.NotSupport; }
        }
    }
}
