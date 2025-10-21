using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class DevicesAlarmSettingAlarmArcs : ANcasBaseOrmTable<DevicesAlarmSettingAlarmArcs, DevicesAlarmSettingAlarmArc>
    {
        private DevicesAlarmSettingAlarmArcs()
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
            get { return ObjectType.DevicesAlarmSettingAlarmArc; }
        }

        protected override void LoadObjectsInRelationshipGetById(DevicesAlarmSettingAlarmArc obj)
        {
            obj.AlarmArc = AlarmArcs.Singleton.GetById(obj.IdAlarmArc);
        }
    }
}
