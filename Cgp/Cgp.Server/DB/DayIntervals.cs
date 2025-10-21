using Contal.Cgp.RemotingCommon;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public sealed class DayIntervals : 
        ABaseOrmTable<DayIntervals, DayInterval>, 
        IDayIntervals
    {
        private DayIntervals() : base(null)
        {
        }

        protected override void LoadObjectsInRelationship(DayInterval obj)
        {
            if (obj.DailyPlan != null)
            {
                obj.DailyPlan = DailyPlans.Singleton.GetById(obj.DailyPlan.IdDailyPlan);
            }
        }

        public override bool HasAccessView(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.DAILY_PLANS_TIME_ZONES), login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesInsertDeletePerform), login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccessesForGroup(LoginAccessGroups.DAILY_PLANS_TIME_ZONES), login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return
                AccessChecker.HasAccessControl(BaseAccess.GetAccess(LoginAccess.DailyPlansTimeZonesInsertDeletePerform), login);
        }

        public override Contal.Cgp.Globals.ObjectType ObjectType
        {
            get { return Contal.Cgp.Globals.ObjectType.DayInterval; }
        }
    }
}
