using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class DcuAlarmArcs : ANcasBaseOrmTable<DcuAlarmArcs, DcuAlarmArc>
    {
        private DcuAlarmArcs()
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
            get { return ObjectType.DcuAlarmArc; }
        }

        protected override void LoadObjectsInRelationshipGetById(DcuAlarmArc obj)
        {
            obj.Dcu = DCUs.Singleton.GetById(obj.Dcu.IdDCU);
            obj.AlarmArc = AlarmArcs.Singleton.GetById(obj.IdAlarmArc);
        }
    }
}
