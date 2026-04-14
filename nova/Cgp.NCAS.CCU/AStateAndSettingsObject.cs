using System;

namespace Contal.Cgp.NCAS.CCU
{
    internal abstract class AStateAndSettingsObject<TDbObject> : AStateAndSettingsObjectBase<TDbObject>
        where TDbObject : DB.IDbObject
    {
        protected AStateAndSettingsObject(Guid id) : base(id)
        {
        }

        public override void ConfigureStart(TDbObject dbObject)
        {
            Configure(dbObject);
        }

        public override void FinishConfiguration()
        {
        }

        public override void ApplyHwSetupOnDcuOnlineStart(TDbObject dbObject)
        {
            ApplyHwSetupOnDcuOnline(dbObject);
        }

        public override void FinishApplyHwSetupOnDcuOnline()
        {
        }

        public override void UnconfigureStart(TDbObject newDbObject)
        {
            Unconfigure(newDbObject);
        }

        public override void FinishUnconfiguration(bool removed)
        {
        }

        public override void StartOnObjectSaved(TDbObject newDbObject)
        {
            OnObjectSaved(newDbObject);
        }

        public override void FinishOnObjectSaved()
        {
        }
    }
}
