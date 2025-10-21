using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Eventlog;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu
{
    internal class HomeMenuSceneGroup : CrSimpleSceneGroup
    {
        public HomeMenuSceneGroup(
            ACardReaderSettings cardReaderSettings,
            IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
            : this(
                cardReaderSettings,
                new DelayedInitReference<ICrScene>(),
                defaultRouteProvider)
        {
        }

        private HomeMenuSceneGroup(
            ACardReaderSettings cardReaderSettings,
            DelayedInitReference<ICrScene> sceneProvider,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(
                sceneProvider,
                parentDefaultRouteProvider)
        {
            sceneProvider.Instance = new Scene(
                cardReaderSettings,
                DefaultGroupExitRoute);
        }

        private class Scene : CrMenuScene
        {
            private readonly CrDisplayProcessor _crDisplayProcessor;

            private class AlarmAreasMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return new RootAlarmAreasSceneGroup(
                            menuItemsProvider.CardReaderSettings).EnterRoute;
                    }
                }

                public AlarmAreasMenuItem()
                    : this(
                        new DelayedInitReference<AlarmAreasMenuItem>())
                {
                }

                private AlarmAreasMenuItem(
                    DelayedInitReference<AlarmAreasMenuItem> delayedInitReference)
                    : base(new RouteProvider())
                {
                    delayedInitReference.Instance = this;
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUALARMAREAS;
                }

                protected override CrIconSymbol GetGraphicIndex(MenuItemsProvider menuItemsProvider)
                {
                    return CrIconSymbol.AlarmAreas;
                }
            }

            private class SensorsMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return
                            new RootCategorizedSensorsSceneGroup(
                                menuItemsProvider.CardReaderSettings).EnterRoute;
                    }
                }

                public SensorsMenuItem()
                    : this(
                        new DelayedInitReference<SensorsMenuItem>())
                {
                }

                private SensorsMenuItem(
                    DelayedInitReference<SensorsMenuItem> delayedInitReference)
                    : base(new RouteProvider())
                {
                    delayedInitReference.Instance = this;
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUSENSORS;
                }

                protected override CrIconSymbol GetGraphicIndex(MenuItemsProvider menuItemsProvider)
                {
                    return CrIconSymbol.Sensors;
                }
            }

            private class EventlogMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return new RootEventlogSceneGroup(menuItemsProvider.CardReaderSettings).EnterRoute;
                    }
                }

                public EventlogMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUEVENTLOGS;
                }

                protected override CrIconSymbol GetGraphicIndex(MenuItemsProvider menuItemsProvider)
                {
                    return CrIconSymbol.EventLog;
                }
            }

            private class EmergencyCodeMenuItem : ALocalizedMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        return new EmergencyCodeSceneGroup(
                            menuItemsProvider.CardReaderSettings,
                            CrSceneGroupReturnRoute.Default).EnterRoute;
                    }
                }

                public EmergencyCodeMenuItem()
                    : base(new RouteProvider())
                {
                }

                protected override string GetLocalizationKey(MenuItemsProvider menuItemsProvider)
                {
                    return CardReaderConstants.MENUEMERGENCYCODE;
                }

                protected override CrIconSymbol GetGraphicIndex(MenuItemsProvider menuItemsProvider)
                {
                    return CrIconSymbol.EmergencyCode;
                }
            }

            private class MenuItemsProvider :
                ACrMenuSceneItemsProvider<MenuItemsProvider>,
                ICcuMenuItemsProvider
            {
                private readonly ACardReaderSettings _cardReaderSettings;

                public MenuItemsProvider(
                    ACardReaderSettings cardReaderSettings,
                    IInstanceProvider<CrMenuScene> menuSceneProvider)
                    : base(menuSceneProvider)
                {
                    _cardReaderSettings = cardReaderSettings;
                }

                protected override MenuItemsProvider This
                {
                    get { return this; }
                }

                protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>> OnEnteredInternal()
                {
                    var result = new List<ACrMenuSceneItem<MenuItemsProvider>>();

                    if (_cardReaderSettings.CrAlarmAreasManager.AlarmAreaCount > 0)
                    {
                        result.Add(new AlarmAreasMenuItem());
                        result.Add(new SensorsMenuItem());
                    }

                    if (_cardReaderSettings.CREventlogProcessor
                        .SetAlarmAreasWithEnabledEventlog(
                            _cardReaderSettings.CardReaderDb))
                    {
                        result.Add(new EventlogMenuItem());
                    }

                    if (_cardReaderSettings.DoorEnvironmentAdapter != null
                        && _cardReaderSettings.CardReaderDb.IsEmergencyCode)
                    {
                        var implicitAlarmAreaInfo = _cardReaderSettings.ImplicitCrAlarmAreaInfo;

                        if (implicitAlarmAreaInfo != null && implicitAlarmAreaInfo.IsUnset)
                            result.Add(new EmergencyCodeMenuItem());
                    }

                    return result;
                }

                public ACardReaderSettings CardReaderSettings
                {
                    get { return _cardReaderSettings; }
                }
            }

            public Scene(
                ACardReaderSettings cardReaderSettings,
                CrSceneGroupExitRoute defaultGroupExitRoute)
                : this(
                    cardReaderSettings,
                    defaultGroupExitRoute,
                    new DelayedInitReference<CrMenuScene>())
            {

            }

            private Scene(
                ACardReaderSettings cardReaderSettings,
                CrSceneGroupExitRoute defaultGroupExitRoute,
                DelayedInitReference<CrMenuScene> menuSceneProvider)
                : base(
                    new MenuItemsProvider(
                        cardReaderSettings,
                        menuSceneProvider),
                    MenuConfigurations.GetMainMenuConfiguration(cardReaderSettings.CardReader),
                    defaultGroupExitRoute,
                    defaultGroupExitRoute)
            {
                menuSceneProvider.Instance = this;
                _crDisplayProcessor = cardReaderSettings.CrDisplayProcessor;
            }

            protected override void ShowNoMenuItems(CardReader cardReader)
            {
                _crDisplayProcessor.DisplayWriteText(
                    _crDisplayProcessor.GetLocalizationString(
                        "NoItemsToDisplay"),
                    0,
                    0);
            }
        }
    }
}
