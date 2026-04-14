using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class AlarmAreaAlarmArcs : ANcasBaseOrmTable<AlarmAreaAlarmArcs, AlarmAreaAlarmArc>
    {
        private AlarmAreaAlarmArcs()
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
            get { return ObjectType.AlarmAreaAlarmArc; }
        }

        protected override void LoadObjectsInRelationshipGetById(AlarmAreaAlarmArc obj)
        {
            obj.AlarmArea = AlarmAreas.Singleton.GetById(obj.AlarmArea.IdAlarmArea);
            obj.AlarmArc = AlarmArcs.Singleton.GetById(obj.IdAlarmArc);
        }
    }
}
