using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class FilteredCategorizedSensorsSceneGroup :
        FilteredAlarmAreasSceneGroupBase<
            RootCategorizedSensorsSceneGroup,
            CategorizedSensorsAccessManager.IEventHandler>,
        IInstanceProvider<FilteredCategorizedSensorsSceneGroup>
    {
        protected abstract class AMenuItemsProvider<TMenuItemsProvider>
            : CategorizedSensorsMenuScene<FilteredCategorizedSensorsSceneGroup, TMenuItemsProvider>.AMenuItemsProvider
            where TMenuItemsProvider : AMenuItemsProvider<TMenuItemsProvider>
        {
            private readonly AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler>
                _alarmAreaAccessManager;

            protected AMenuItemsProvider(
                IInstanceProvider<FilteredCategorizedSensorsSceneGroup> sceneGroupProvider,
                IInstanceProvider<CrMenuScene> menuSceneProvider,
                AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
                : base(
                    sceneGroupProvider,
                    menuSceneProvider)
            {
                _alarmAreaAccessManager = alarmAreaAccessManager;
            }

            protected override AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler>
                AlarmAreaAccessManager
            {
                get { return _alarmAreaAccessManager; }
            }

            protected override CategorizedSensorsAccessManager.IEventHandler ThisEventHandler
            {
                get { return this; }
            }
        }

        protected FilteredCategorizedSensorsSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            [NotNull] IInstanceProvider<ICrScene> sceneProvider)
            : base(
                rootCategorizedSensorsSceneGroup,
                sceneProvider)
        {
        }

        FilteredCategorizedSensorsSceneGroup IInstanceProvider<FilteredCategorizedSensorsSceneGroup>.Instance
        {
            get { return this; }
        }
    }

    internal class CategorizedSensorsInAlarmSceneGroup :
        FilteredCategorizedSensorsSceneGroup
    {
        private class MenuItemsProvider
            : AMenuItemsProvider<MenuItemsProvider>
        {
            private new class AlarmAreaMenuItem :
                AMenuItemsProvider<MenuItemsProvider>.AlarmAreaMenuItem
            {
                private class RouteProvider : ARouteProvider
                {
                    public RouteProvider(AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
                        : base(alarmAreaAccessInfo)
                    {
                    }

                    public override ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var filteredCategorizedSensorsMenuSceneGroup =
                            menuItemsProvider.SceneGroupProvider.Instance;

                        var crAlarmAreaInfo = AlarmAreaAccessInfo.CrAlarmAreaInfo;

                        return new SensorsInAlarmForAlarmAreaSceneGroup(
                            new AlarmAreaSensorListener(crAlarmAreaInfo),
                            filteredCategorizedSensorsMenuSceneGroup).EnterRoute;
                    }
                }

                public AlarmAreaMenuItem(
                    AAlarmAreaAccessInfoBase accessInfo)
                    : base(
                        accessInfo,
                        new RouteProvider(accessInfo))
                {
                }
            }

            public MenuItemsProvider(FilteredCategorizedSensorsSceneGroup sceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider,
                AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
                : base(
                    sceneGroup,
                    menuSceneProvider,
                    alarmAreaAccessManager)
            {
            }

            protected override AlarmAreaMenuItemBase CreateAlarmAreaMenuItem(AAlarmAreaAccessInfoBase alarmAreaInfo)
            {
                return new AlarmAreaMenuItem(alarmAreaInfo);
            }

            public override bool IsAlarmAreaVisible(AAlarmAreaAccessInfoBase accessInfo)
            {
                return
                    accessInfo.IsVisible
                    && accessInfo.CrAlarmAreaInfo.IsAnySensorInAlarm;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public CategorizedSensorsInAlarmSceneGroup(RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            AAlarmAreaAccessManagerBase<
                CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : this(
                rootCategorizedSensorsSceneGroup,
                new DelayedCrMenuSceneProvider(),
                alarmAreaAccessManager)
        {
        }

        private CategorizedSensorsInAlarmSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            DelayedCrMenuSceneProvider sceneProvider,
            AAlarmAreaAccessManagerBase<
                CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : base(
                rootCategorizedSensorsSceneGroup,
                sceneProvider)
        {
            sceneProvider.Instance =
                new CategorizedSensorsMenuScene<FilteredCategorizedSensorsSceneGroup, MenuItemsProvider>(
                    this,
                    new MenuItemsProvider(
                        this,
                        sceneProvider,
                        alarmAreaAccessManager));
        }
    }

    internal class CategorizedSensorsNotAcknowledgedSceneGroup :
        FilteredCategorizedSensorsSceneGroup
    {
        private class MenuItemsProvider
            : AMenuItemsProvider<MenuItemsProvider>
        {
            private new class AlarmAreaMenuItem :
                AMenuItemsProvider<MenuItemsProvider>.AlarmAreaMenuItem
            {
                private class RouteProvider : ARouteProvider
                {
                    public RouteProvider(
                        AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
                        : base(alarmAreaAccessInfo)
                    {
                    }

                    public override ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var filteredCategorizedSensorsMenuSceneGroup =
                            menuItemsProvider.SceneGroupProvider.Instance;

                        var crAlarmAreaInfo = AlarmAreaAccessInfo.CrAlarmAreaInfo;

                        return new SensorsNotAcknowledgedForAlarmAreasSceneGroup(
                            new AlarmAreaSensorListener(crAlarmAreaInfo),
                            filteredCategorizedSensorsMenuSceneGroup).EnterRoute;
                    }
                }

                public AlarmAreaMenuItem(
                    AAlarmAreaAccessInfoBase accessInfo)
                    : base(
                        accessInfo,
                        new RouteProvider(accessInfo))
                {
                }
            }

            private class GlobalAcknowledgeAllMenuItem
                : GlobalAcknowledgeAllMenuItemBase<MenuItemsProvider>
            {
                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return true;
                }
            }

            public MenuItemsProvider(FilteredCategorizedSensorsSceneGroup sceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider,
                AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
                : base(
                    sceneGroup,
                    menuSceneProvider,
                    alarmAreaAccessManager)
            {
            }

            protected override AlarmAreaMenuItemBase CreateAlarmAreaMenuItem(AAlarmAreaAccessInfoBase alarmAreaInfo)
            {
                return new AlarmAreaMenuItem(alarmAreaInfo);
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateGeneralItems()
            {
                yield return new GlobalAcknowledgeAllMenuItem();
            }

            public override bool IsAlarmAreaVisible(AAlarmAreaAccessInfoBase accessInfo)
            {
                return
                    accessInfo.IsVisible
                    && accessInfo.CrAlarmAreaInfo.IsAnySensorNotAcknowledged;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public CategorizedSensorsNotAcknowledgedSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : this(
                rootCategorizedSensorsSceneGroup,
                new DelayedCrMenuSceneProvider(),
                alarmAreaAccessManager)
        {
        }

        private CategorizedSensorsNotAcknowledgedSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            DelayedCrMenuSceneProvider sceneProvider,
            AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : base(
                rootCategorizedSensorsSceneGroup,
                sceneProvider)
        {
            sceneProvider.Instance =
                new CategorizedSensorsMenuScene<FilteredCategorizedSensorsSceneGroup, MenuItemsProvider>(
                    this,
                    new MenuItemsProvider(
                        this,
                        sceneProvider,
                        alarmAreaAccessManager));
        }
    }

    internal class CategorizedSensorsTemporarilyBlockedSceneGroup :
        FilteredCategorizedSensorsSceneGroup
    {
        private class MenuItemsProvider
            : AMenuItemsProvider<MenuItemsProvider>
        {
            private new class AlarmAreaMenuItem :
                AMenuItemsProvider<MenuItemsProvider>.AlarmAreaMenuItem
            {
                private class RouteProvider : ARouteProvider
                {
                    public RouteProvider(
                        AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
                        : base(alarmAreaAccessInfo)
                    {
                    }

                    public override ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var filteredCategorizedSensorsMenuSceneGroup = menuItemsProvider.SceneGroupProvider.Instance;

                        return new SensorsTemporarilyBlockedForAlarmAreaSceneGroup(
                            new AlarmAreaSensorListener(AlarmAreaAccessInfo.CrAlarmAreaInfo),
                            filteredCategorizedSensorsMenuSceneGroup).EnterRoute;
                    }
                }

                public AlarmAreaMenuItem(
                    AAlarmAreaAccessInfoBase accessInfo)
                    : base(
                        accessInfo,
                        new RouteProvider(accessInfo))
                {
                }
            }

            public MenuItemsProvider(FilteredCategorizedSensorsSceneGroup sceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider,
                AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
                : base(
                    sceneGroup,
                    menuSceneProvider,
                    alarmAreaAccessManager)
            {
            }

            protected override AlarmAreaMenuItemBase CreateAlarmAreaMenuItem(AAlarmAreaAccessInfoBase alarmAreaInfo)
            {
                return new AlarmAreaMenuItem(alarmAreaInfo);
            }

            public override bool IsAlarmAreaVisible(AAlarmAreaAccessInfoBase accessInfo)
            {
                if (!accessInfo.IsVisible)
                    return false;

                return accessInfo.CrAlarmAreaInfo.IsAnySensorTemporarilyBlocked;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public CategorizedSensorsTemporarilyBlockedSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : this(
                rootCategorizedSensorsSceneGroup,
                new DelayedCrMenuSceneProvider(),
                alarmAreaAccessManager)
        {
        }

        private CategorizedSensorsTemporarilyBlockedSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            DelayedCrMenuSceneProvider sceneProvider,
            AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : base(
                rootCategorizedSensorsSceneGroup,
                sceneProvider)
        {
            sceneProvider.Instance =
                new CategorizedSensorsMenuScene<FilteredCategorizedSensorsSceneGroup, MenuItemsProvider>(
                    this,
                    new MenuItemsProvider(
                        this,
                        sceneProvider,
                        alarmAreaAccessManager));
        }
    }

    internal class CategorizedSensorsPermanentlyBlockedSceneGroup :
        FilteredCategorizedSensorsSceneGroup
    {
        private class MenuItemsProvider
            : AMenuItemsProvider<MenuItemsProvider>
        {
            private new class AlarmAreaMenuItem :
                AMenuItemsProvider<MenuItemsProvider>.AlarmAreaMenuItem
            {
                private class RouteProvider : ARouteProvider
                {
                    public RouteProvider(
                        AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
                        : base(alarmAreaAccessInfo)
                    {
                    }

                    public override ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var filteredCategorizedSensorsMenuSceneGroup =
                            menuItemsProvider.SceneGroupProvider.Instance;

                        return new SensorsPermanentlyBlockedForAlarmAreaSceneGroup(
                            new AlarmAreaSensorListener(AlarmAreaAccessInfo.CrAlarmAreaInfo),
                            filteredCategorizedSensorsMenuSceneGroup).EnterRoute;
                    }
                }

                public AlarmAreaMenuItem(
                    AAlarmAreaAccessInfoBase accessInfo)
                    : base(
                        accessInfo,
                        new RouteProvider(accessInfo))
                {
                }
            }

            public MenuItemsProvider(FilteredCategorizedSensorsSceneGroup sceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider,
                AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
                : base(
                    sceneGroup,
                    menuSceneProvider,
                    alarmAreaAccessManager)
            {
            }

            protected override AlarmAreaMenuItemBase CreateAlarmAreaMenuItem(AAlarmAreaAccessInfoBase alarmAreaInfo)
            {
                return new AlarmAreaMenuItem(alarmAreaInfo);
            }

            public override bool IsAlarmAreaVisible(AAlarmAreaAccessInfoBase accessInfo)
            {
                return
                    accessInfo.IsVisible
                    && accessInfo.CrAlarmAreaInfo.IsAnySensorPermanentlyBlocked;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public CategorizedSensorsPermanentlyBlockedSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : this(
                rootCategorizedSensorsSceneGroup,
                new DelayedCrMenuSceneProvider(),
                alarmAreaAccessManager)
        {
        }

        private CategorizedSensorsPermanentlyBlockedSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            DelayedCrMenuSceneProvider sceneProvider,
            AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : base(
                rootCategorizedSensorsSceneGroup,
                sceneProvider)
        {
            sceneProvider.Instance =
                new CategorizedSensorsMenuScene<FilteredCategorizedSensorsSceneGroup, MenuItemsProvider>(
                    this,
                    new MenuItemsProvider(
                        this,
                        sceneProvider,
                        alarmAreaAccessManager));
        }
    }

    internal class CategorizedSensorsInSabotageSceneGroup :
        FilteredCategorizedSensorsSceneGroup
    {
        private class MenuItemsProvider
            : AMenuItemsProvider<MenuItemsProvider>
        {
            private new class AlarmAreaMenuItem :
                AMenuItemsProvider<MenuItemsProvider>.AlarmAreaMenuItem
            {
                private class RouteProvider : ARouteProvider
                {
                    public RouteProvider(
                        AAlarmAreaAccessInfoBase alarmAreaAccessInfo)
                        : base(alarmAreaAccessInfo)
                    {
                    }

                    public override ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var filteredCategorizedSensorsMenuSceneGroup = menuItemsProvider.SceneGroupProvider.Instance;

                        var crAlarmAreaInfo = AlarmAreaAccessInfo.CrAlarmAreaInfo;

                        return new SensorsInSabotageForAlarmAreaSceneGroup(
                            new AlarmAreaSensorListener(crAlarmAreaInfo),
                            filteredCategorizedSensorsMenuSceneGroup).EnterRoute;
                    }
                }

                public AlarmAreaMenuItem(
                    AAlarmAreaAccessInfoBase accessInfo)
                    : base(
                        accessInfo,
                        new RouteProvider(accessInfo))
                {
                }
            }

            public MenuItemsProvider(FilteredCategorizedSensorsSceneGroup sceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider,
                AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
                : base(
                    sceneGroup,
                    menuSceneProvider,
                    alarmAreaAccessManager)
            {
            }

            protected override AlarmAreaMenuItemBase CreateAlarmAreaMenuItem(AAlarmAreaAccessInfoBase alarmAreaInfo)
            {
                return new AlarmAreaMenuItem(alarmAreaInfo);
            }

            public override bool IsAlarmAreaVisible(AAlarmAreaAccessInfoBase accessInfo)
            {
                return
                    accessInfo.IsVisible
                    && accessInfo.CrAlarmAreaInfo.IsInSabotage;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public CategorizedSensorsInSabotageSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : this(
                rootCategorizedSensorsSceneGroup,
                new DelayedCrMenuSceneProvider(),
                alarmAreaAccessManager)
        {
        }

        private CategorizedSensorsInSabotageSceneGroup(
            RootCategorizedSensorsSceneGroup rootCategorizedSensorsSceneGroup,
            DelayedCrMenuSceneProvider sceneProvider,
            AAlarmAreaAccessManagerBase<CategorizedSensorsAccessManager.IEventHandler> alarmAreaAccessManager)
            : base(
                rootCategorizedSensorsSceneGroup,
                sceneProvider)
        {
            sceneProvider.Instance =
                new CategorizedSensorsMenuScene<FilteredCategorizedSensorsSceneGroup, MenuItemsProvider>(
                    this,
                    new MenuItemsProvider(
                        this,
                        sceneProvider,
                        alarmAreaAccessManager));
        }
    }
}
