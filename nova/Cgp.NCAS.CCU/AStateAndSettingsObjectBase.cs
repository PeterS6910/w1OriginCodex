using System;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU
{
    internal enum ConfigurationState
    {
        Precreated = 0,
        Unconfiguring,
        Unconfigured,
        ConfiguringNew,
        ConfiguringExisting,
        ApplyingHwSetupNew,
        ApplyingHwSetupExisting,
        ConfigurationDone
    }

    internal abstract class AStateAndSettingsObjectBase<TDbObject>
        where TDbObject : DB.IDbObject
    {
        public Guid Id
        {
            get;
            private set;
        }

        public ConfigurationState ConfigurationState
        {
            get;
            protected set;
        }

        protected AStateAndSettingsObjectBase(Guid id)
        {
            Id = id;
            ConfigurationState = ConfigurationState.Precreated;
        }

        public abstract void ConfigureStart(TDbObject dbObject);

        protected abstract void ConfigureInternal(TDbObject dbObject);

        protected void Configure(TDbObject dbObject)
        {
            ConfigurationState =
                ConfigurationState == ConfigurationState.Precreated
                    ? ConfigurationState.ConfiguringNew
                    : ConfigurationState.ConfiguringExisting;

            ConfigureInternal(dbObject);

            ConfigurationState =
                ConfigurationState == ConfigurationState.ConfiguringNew
                    ? ConfigurationState.ApplyingHwSetupNew
                    : ConfigurationState.ApplyingHwSetupExisting;

            ApplyHwSetup(dbObject);

            ConfigurationState = ConfigurationState.ConfigurationDone;
        }

        public abstract void FinishConfiguration();

        protected void ApplyHwSetupOnDcuOnline([NotNull] TDbObject dbObject)
        {
            if (ConfigurationState != ConfigurationState.ConfigurationDone)
                return;

            ConfigurationState = ConfigurationState.ApplyingHwSetupExisting;

            ApplyHwSetup(dbObject);

            ConfigurationState = ConfigurationState.ConfigurationDone;
        }

        public abstract void ApplyHwSetupOnDcuOnlineStart([NotNull] TDbObject dbObject);

        protected abstract void ApplyHwSetup([NotNull] TDbObject dbObject);

        public abstract void FinishApplyHwSetupOnDcuOnline();

        public abstract void UnconfigureStart(TDbObject newDbObject);

        protected void Unconfigure(TDbObject newDbObject)
        {
            ConfigurationState = ConfigurationState.Unconfiguring;

            UnconfigureInternal(newDbObject);

            ConfigurationState = ConfigurationState.Unconfigured;
        }

        protected abstract void UnconfigureInternal(TDbObject newDbObject);

        public abstract void FinishUnconfiguration(bool removed);

        public abstract void StartOnObjectSaved(TDbObject newDbObject);

        protected virtual void OnObjectSaved(TDbObject newDbObject)
        {
        }

        public abstract void FinishOnObjectSaved();
    }
}