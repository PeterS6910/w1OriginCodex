using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class CardReaderAlarmArcs : ANcasBaseOrmTable<CardReaderAlarmArcs, CardReaderAlarmArc>
    {
        private CardReaderAlarmArcs()
            : base(null)
        {
        }

        public override bool HasAccessView(Login login)
        {
            return false;
        }

        public override bool HasAccessInsert(Login login)
        {
            return false;
        }

        public override bool HasAccessUpdate(Login login)
        {
            return false;
        }

        public override bool HasAccessDelete(Login login)
        {
            return false;
        }

        public override ObjectType ObjectType
        {
            get { return ObjectType.CardReaderAlarmArc; }
        }

        protected override void LoadObjectsInRelationshipGetById(CardReaderAlarmArc obj)
        {
            obj.CardReader = CardReaders.Singleton.GetById(obj.CardReader.IdCardReader);
            obj.AlarmArc = AlarmArcs.Singleton.GetById(obj.IdAlarmArc);
        }
    }
}
