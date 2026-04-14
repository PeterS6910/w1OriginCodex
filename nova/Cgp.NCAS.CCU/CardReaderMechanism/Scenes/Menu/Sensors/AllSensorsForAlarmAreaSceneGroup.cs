using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.AlarmArea;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class AllSensorsForAlarmAreaSceneGroup :
        ASensorsForAlarmAreaSceneGroupBase<AllSensorsForAlarmAreaSceneGroup.MenuItemsProvider>,
        IInstanceProvider<AllSensorsForAlarmAreaSceneGroup>
    {
        public class MenuItemsProvider :
            SensorsForAlarmAreaMenuScene<MenuItemsProvider>.AMenuItemsProvider
        {
            private readonly CrAlarmAreasManager.CrAlarmAreaInfo _crAlarmAreaInfo;

            private class SensorsInAlarmMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var sceneGroup =
                            menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                        return new SensorsInAlarmForAlarmAreaSceneGroup(
                            sceneGroup.AlarmAreaSensorListener,
                            sceneGroup).EnterRoute;
                    }
                }

                public SensorsInAlarmMenuItem()
                    : base(
                        new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSINALARM;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsInAlarm;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return 
                        menuItemsProvider
                            ._crAlarmAreaInfo
                            .IsAnySensorInAlarm;
                }
            }

            private class SensorsNotAcknowledgedMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var sceneGroup =
                            menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                        return new SensorsNotAcknowledgedForAlarmAreasSceneGroup(
                            sceneGroup.AlarmAreaSensorListener,
                            sceneGroup).EnterRoute;
                    }
                }

                public SensorsNotAcknowledgedMenuItem()
                    : base(new RouteProvider())
                {

                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSNOTACKNOWLEDGED;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsNormalNotAcknowledged;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._crAlarmAreaInfo.IsAnySensorNotAcknowledged;
                }
            }

            private class SensorsTemporarilyBlockedMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider
                    : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var sceneGroup =
                            menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                        return new SensorsTemporarilyBlockedForAlarmAreaSceneGroup(
                            sceneGroup.AlarmAreaSensorListener,
                            sceneGroup).EnterRoute;
                    }
                }

                public SensorsTemporarilyBlockedMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSTEMPORARILYBLOCKED;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsTemporarilyBlocked;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._crAlarmAreaInfo.IsAnySensorTemporarilyBlocked;
                }
            }

            private class SensorsPermanentlyBlockedMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider
                    : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var sceneGroup =
                            menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                        return new SensorsPermanentlyBlockedForAlarmAreaSceneGroup(
                            sceneGroup.AlarmAreaSensorListener,
                            sceneGroup).EnterRoute;
                    }
                }

                public SensorsPermanentlyBlockedMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSPERMANENTLYBLOCKED;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsPermanentlyBlocked;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._crAlarmAreaInfo.IsAnySensorPermanentlyBlocked;
                }
            }

            private class SensorsInSabotageMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider
                    : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var sceneGroup =
                            menuItemsProvider.SensorsForAlarmAreaSceneGroup;

                        return new SensorsInSabotageForAlarmAreaSceneGroup(
                            sceneGroup.AlarmAreaSensorListener,
                            sceneGroup).EnterRoute;
                    }
                }

                public SensorsInSabotageMenuItem()
                    : base(
                        new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORSINSABOTAGE;
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    yield return CrIconSymbol.SensorsInSabotage;
                }

                public override bool IsVisible(MenuItemsProvider menuItemsProvider)
                {
                    return menuItemsProvider._crAlarmAreaInfo.IsInSabotage;
                }
            }

            public MenuItemsProvider(
                AllSensorsForAlarmAreaSceneGroup sensorsForAlarmAreaSceneGroup,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    sensorsForAlarmAreaSceneGroup,
                    menuSceneProvider)
            {
                _crAlarmAreaInfo = SensorsForAlarmAreaSceneGroup.CrAlarmAreaInfo;
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> CreateGeneralItems()
            {
                yield return new SensorsInAlarmMenuItem();

                yield return new SensorsNotAcknowledgedMenuItem();

                yield return new SensorsTemporarilyBlockedMenuItem();

                yield return new SensorsPermanentlyBlockedMenuItem();

                yield return new SensorsInSabotageMenuItem();
            }

            protected override bool IsSensorVisible(ISensorStateAndSettings sensorStateAndSettings)
            {
                return true;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }
        }

        public AllSensorsForAlarmAreaSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            RootCategorizedSensorsSceneGroup parentSceneGroup)
            : base(
                new AlarmAreaSensorListener(crAlarmAreaInfo), 
                parentSceneGroup)
        {
        }

        protected override MenuItemsProvider GetMenuItemsProvider(
            IInstanceProvider<CrMenuScene> menuSceneProvider)
        {
            return new MenuItemsProvider(
                this,
                menuSceneProvider);
        }

        AllSensorsForAlarmAreaSceneGroup IInstanceProvider<AllSensorsForAlarmAreaSceneGroup>.Instance
        {
            get { return this; }
        }
    }
}
