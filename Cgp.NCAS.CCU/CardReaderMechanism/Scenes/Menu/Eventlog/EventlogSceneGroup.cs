using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Eventlog
{
    internal class EventlogSceneGroup :
        CrSimpleSceneGroup,
        IInstanceProvider<EventlogSceneGroup>,
        ICrEventlogDisplayContext
    {
        private class MenuItemsProvider : ACrMenuSceneItemsProvider<MenuItemsProvider>
        {
            private class EventMenuItem : ACcuMenuItem<MenuItemsProvider>
            {
                private class RouteProvider : IInstanceProvider<ACrSceneRoute, MenuItemsProvider>
                {
                    private readonly IEventForCardReader _eventForCardReader;

                    public RouteProvider(
                        IEventForCardReader eventForCardReader)
                    {
                        _eventForCardReader = eventForCardReader;
                    }

                    public ACrSceneRoute GetInstance(MenuItemsProvider menuItemsProvider)
                    {
                        var eventlogSceneGroup = menuItemsProvider._sceneGroupProvider.Instance;

                        return new EventInfoSceneGroup(
                            eventlogSceneGroup,
                            _eventForCardReader).EnterRoute;
                    }
                }

                private readonly IEventForCardReader _eventForCardReader;

                public EventMenuItem(
                    IEventForCardReader eventForCardReader)
                    : base(new RouteProvider(eventForCardReader))
                {
                    _eventForCardReader = eventForCardReader;
                }

                protected override string GetText(MenuItemsProvider menuItemsProvider)
                {
                    var timeOfDay = _eventForCardReader.DateTime.TimeOfDay;

                    return
                        string.Format(
                            "{0} {1}",
                            _eventForCardReader.GetEventObjectName(
                                menuItemsProvider._sceneGroupProvider.Instance),
                            string.Format(
                                "{0}:{1}",
                                timeOfDay.Hours.ToString("D2"),
                                timeOfDay.Minutes.ToString("D2")));
                }

                protected override IEnumerable<CrIconSymbol> GetInlinedIcons(MenuItemsProvider menuItemsProvider)
                {
                    return _eventForCardReader.InlinedIcons;
                }
            }

            private readonly IInstanceProvider<EventlogSceneGroup> _sceneGroupProvider;

            public MenuItemsProvider(
                [NotNull] IInstanceProvider<EventlogSceneGroup> sceneGroupProvider,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(menuSceneProvider)
            {
                _sceneGroupProvider = sceneGroupProvider;
            }

            protected override MenuItemsProvider This
            {
                get { return this; }
            }

            protected override IEnumerable<ACrMenuSceneItem<MenuItemsProvider>>
                OnEnteredInternal()
            {
                var eventlogSceneGroup = _sceneGroupProvider.Instance;

                var alarmArea = eventlogSceneGroup.AlarmArea;

                var eventsForCardReaders = 
                    eventlogSceneGroup.ACardReaderSettings.CREventlogProcessor
                        .GetEventsForAlarmArea(
                            alarmArea != null
                                ? alarmArea.IdAlarmArea
                                : Guid.Empty);

                return
                    eventsForCardReaders != null
                        ? eventsForCardReaders
                            .Select(
                                eventForCardReader =>
                                    (ACrMenuSceneItem<MenuItemsProvider>)
                                    new EventMenuItem(eventForCardReader))
                        : Enumerable.Empty<ACrMenuSceneItem<MenuItemsProvider>>();
            }
        }

        private class Scene : CrMenuScene
        {
            private readonly CrDisplayProcessor _crDisplayProcessor;

            public Scene(EventlogSceneGroup eventlogSceneGroup)
                : this(
                    eventlogSceneGroup,
                    eventlogSceneGroup.ACardReaderSettings,
                    new DelayedInitReference<CrMenuScene>())
            {

            }

            private Scene(
                EventlogSceneGroup eventlogSceneGroup,
                ACardReaderSettings cardReaderSettings,
                DelayedInitReference<CrMenuScene> delayedInitReference)
                : base(
                    new MenuItemsProvider(
                        eventlogSceneGroup,
                        delayedInitReference),
                    MenuConfigurations.GetEventlogMenuConfiguration(
                        cardReaderSettings.CardReader),
                    eventlogSceneGroup.DefaultGroupExitRoute,
                    eventlogSceneGroup.DefaultGroupExitRoute)
            {
                delayedInitReference.Instance = this;
                _crDisplayProcessor = cardReaderSettings.CrDisplayProcessor;
            }

            protected override void ShowNoMenuItems(CardReader cardReader)
            {
                _crDisplayProcessor.DisplayWriteText(
                    _crDisplayProcessor.GetLocalizationString(
                        "NoEventsToDisplay"),
                    0,
                    0);
            }
        }

        public EventlogSceneGroup(
            RootEventlogSceneGroup rootEventlogSceneGroup,
            DB.AlarmArea alarmArea)
            : this(
                rootEventlogSceneGroup,
                new DelayedInitReference<ICrScene>())
        {
            AlarmArea = alarmArea;
        }

        private EventlogSceneGroup(
            RootEventlogSceneGroup rootEventlogSceneGroup,
            DelayedInitReference<ICrScene> sceneProvider)
            : base(
                sceneProvider,
                CrSceneGroupReturnRoute.Default)
        {
            TimeOutGroupExitRoute = new CrSceneGroupExitRoute(
                this,
                rootEventlogSceneGroup.DefaultGroupExitRoute);

            ACardReaderSettings = rootEventlogSceneGroup.CardReaderSettings;
            sceneProvider.Instance = new Scene(this);
        }

        EventlogSceneGroup IInstanceProvider<EventlogSceneGroup>.Instance
        {
            get { return this; }
        }

        public CrSceneGroupExitRoute TimeOutGroupExitRoute
        {
            get;
            private set;
        }

        public ACardReaderSettings ACardReaderSettings
        {
            get;
            private set;
        }

        public DB.AlarmArea AlarmArea
        {
            get;
            private set;
        }
    }
}
