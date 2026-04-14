using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class FilteredAlarmAreasSceneGroup :
        FilteredAlarmAreasSceneGroupBase<
            RootAlarmAreasSceneGroup,
            IAlarmAreaAccessEventHandler>,
        IInstanceProvider<FilteredAlarmAreasSceneGroup>
    {
        protected abstract class AMenuItemsProvider<TMenuItemsProvider> :
            AlarmAreasMenuScene<FilteredAlarmAreasSceneGroup, TMenuItemsProvider>.AMenuItemsProvider
            where TMenuItemsProvider : AMenuItemsProvider<TMenuItemsProvider>
        {
            private readonly AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> _alarmAreaAccessManager;

            protected AMenuItemsProvider(
                IInstanceProvider<FilteredAlarmAreasSceneGroup> sceneGroupProvider,
                IInstanceProvider<CrMenuScene> menuSceneProvider,
                AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> alarmAreaAccessManager)
                : base(
                    sceneGroupProvider,
                    menuSceneProvider)
            {
                _alarmAreaAccessManager = alarmAreaAccessManager;
            }

            protected override AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> AlarmAreaAccessManager
            {
                get { return _alarmAreaAccessManager; }
            }
        }

        protected FilteredAlarmAreasSceneGroup(
            RootAlarmAreasSceneGroup rootAlarmAreasSceneGroup,
            IInstanceProvider<ICrScene> sceneProvider)
            : base(
                rootAlarmAreasSceneGroup,
                sceneProvider)
        {
        }

        FilteredAlarmAreasSceneGroup IInstanceProvider<FilteredAlarmAreasSceneGroup>.Instance
        {
            get { return this; }
        }
    }

    internal class AlarmAreasInAlarmStateSceneGroup : FilteredAlarmAreasSceneGroup
    {
        private class MenuItemsProvider : AMenuItemsProvider<MenuItemsProvider>
        {
            public MenuItemsProvider(
                FilteredAlarmAreasSceneGroup sceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider,
                AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> alarmAreaAccessManager)
                : base(
                    sceneGroup,
                    menuSceneProvider,
                    alarmAreaAccessManager)
            {
            }

            public override bool IsAlarmAreaVisible(AAlarmAreaAccessInfoBase accessInfo)
            {
                return
                    accessInfo.IsVisible
                    && accessInfo.CrAlarmAreaInfo.AlarmState == State.Alarm;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public AlarmAreasInAlarmStateSceneGroup(
            RootAlarmAreasSceneGroup rootAlarmAreasSceneGroup,
            AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> alarmAreaAccessManager)
            : this(
                rootAlarmAreasSceneGroup,
                new DelayedCrMenuSceneProvider(),
                alarmAreaAccessManager)
        {
        }

        private AlarmAreasInAlarmStateSceneGroup(
            RootAlarmAreasSceneGroup rootAlarmAreasSceneGroup,
            DelayedCrMenuSceneProvider sceneProvider,
            AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> alarmAreaAccessManager)
            : base(
                rootAlarmAreasSceneGroup,
                sceneProvider)
        {
            sceneProvider.Instance =
                new AlarmAreasMenuScene<FilteredAlarmAreasSceneGroup, MenuItemsProvider>(
                    this,
                    new MenuItemsProvider(
                        this,
                        sceneProvider,
                        alarmAreaAccessManager));
        }
    }

    internal class AlarmAreasNotAcknowledgedSceneGroup : FilteredAlarmAreasSceneGroup
    {
        private class MenuItemsProvider : AMenuItemsProvider<MenuItemsProvider>
        {
            public MenuItemsProvider(FilteredAlarmAreasSceneGroup sceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider,
                AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> alarmAreaAccessManager)
                : base(
                    sceneGroup,
                    menuSceneProvider,
                    alarmAreaAccessManager)
            {
            }

            public override bool IsAlarmAreaVisible(AAlarmAreaAccessInfoBase accessInfo)
            {
                return
                    accessInfo.IsVisible &&
                    accessInfo.CrAlarmAreaInfo.IsNotAcknowledged;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public AlarmAreasNotAcknowledgedSceneGroup(
            RootAlarmAreasSceneGroup rootAlarmAreasSceneGroup,
            AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> alarmAreaAccessManager)
            : this(
                rootAlarmAreasSceneGroup,
                new DelayedCrMenuSceneProvider(),
                alarmAreaAccessManager)
        {
        }

        private AlarmAreasNotAcknowledgedSceneGroup(
            RootAlarmAreasSceneGroup rootAlarmAreasSceneGroup,
            DelayedCrMenuSceneProvider sceneProvider,
            AAlarmAreaAccessManagerBase<IAlarmAreaAccessEventHandler> alarmAreaAccessManager)
            : base(
                rootAlarmAreasSceneGroup,
                sceneProvider)
        {
            sceneProvider.Instance =
                new AlarmAreasMenuScene<FilteredAlarmAreasSceneGroup, MenuItemsProvider>(
                    this,
                    new MenuItemsProvider(
                        this,
                        sceneProvider,
                        alarmAreaAccessManager));
        }
    }
}