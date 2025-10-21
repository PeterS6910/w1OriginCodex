using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class CcuAlarmArcs : ANcasBaseOrmTable<CcuAlarmArcs, CcuAlarmArc>
    {
        private CcuAlarmArcs()
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
            get { return ObjectType.CcuAlarmArc; }
        }

        protected override void LoadObjectsInRelationshipGetById(CcuAlarmArc obj)
        {
            obj.Ccu = CCUs.Singleton.GetById(obj.Ccu.IdCCU);
            obj.AlarmArc = AlarmArcs.Singleton.GetById(obj.IdAlarmArc);
        }
    }
}
