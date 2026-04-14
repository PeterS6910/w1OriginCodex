using Contal.Drivers.CardReader;
using Contal.IwQuick.Threads;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Eventlog
{
    internal class EventInfoSceneGroup : 
        CrSimpleSceneGroup,
        ICrEventlogDisplayContext
    {
        private class Scene : CrBaseScene
        {
            private readonly IInstanceProvider<EventInfoSceneGroup> _sceneGroupProvider;

            public Scene(IInstanceProvider<EventInfoSceneGroup> sceneGroupProvider)
            {
                _sceneGroupProvider = sceneGroupProvider;
            }

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                var cardReader = crSceneContext.CardReader;

                cardReader.DisplayCommands.ClearAllDisplay(cardReader);

                var eventInfoSceneGroup = _sceneGroupProvider.Instance;

                eventInfoSceneGroup._cardReaderSettings.CREventlogProcessor.DrawEventInformation(
                    eventInfoSceneGroup._eventForCardReader,
                    eventInfoSceneGroup);

                cardReader.MenuCommands.SetBottomMenuButtons(
                    cardReader,
                    new CRBottomMenu
                    {
                        Button1 = CRMenuButtonLook.Clear,
                        Button2 = CRMenuButtonLook.Clear,
                        Button3 = CRMenuButtonLook.Clear,
                        Button4 = CRMenuButtonLook.No,
                        Button4ReturnCode = CRSpecialKey.No
                    });

                TimerManager.Static.StartTimeout(
                    CardReaderConstants.SHOWEVENTDELAY,
                    crSceneContext,
                    OnTimeout);

                return true;
            }

            private bool OnTimeout(TimerCarrier timerCarrier)
            {
                var crSceneContext = 
                    (ACrSceneContext)
                    timerCarrier.Data;

                crSceneContext.PlanDelayedRouteFollowing(
                    this,
                    _sceneGroupProvider.Instance,
                    _sceneGroupProvider.Instance.TimeOutGroupExitRoute);

                return true;
            }

            public override void OnSpecialKeyPressed(
                ACrSceneContext crSceneContext,
                CRSpecialKey specialKey)
            {
                if (specialKey == CRSpecialKey.No)
                    _sceneGroupProvider.Instance
                        .DefaultGroupExitRoute
                        .Follow(crSceneContext);
            }
        }

        public ACrSceneRoute TimeOutGroupExitRoute
        {
            get;
            private set;
        }

        private readonly ACardReaderSettings _cardReaderSettings;
        private readonly IEventForCardReader _eventForCardReader;

        public EventInfoSceneGroup(
            EventlogSceneGroup eventlogSceneGroup,
            IEventForCardReader eventForCardReader)
            : this(
                eventlogSceneGroup,
                eventForCardReader,
                new DelayedInitReference<EventInfoSceneGroup>())
        {

        }

        private EventInfoSceneGroup(
            EventlogSceneGroup eventlogSceneGroup,
            IEventForCardReader eventForCardReader,
            DelayedInitReference<EventInfoSceneGroup> delayedInitReference)
            : base(
                new Scene(delayedInitReference), 
                CrSceneGroupReturnRoute.Default)
        {
            TimeOutGroupExitRoute = new CrSceneGroupExitRoute(
                this,
                eventlogSceneGroup.TimeOutGroupExitRoute);

            delayedInitReference.Instance = this;

            _cardReaderSettings = eventlogSceneGroup.ACardReaderSettings;
            _eventForCardReader = eventForCardReader;
            AlarmArea = eventlogSceneGroup.AlarmArea;
        }

        public DB.AlarmArea AlarmArea
        {
            get;
            private set;
        }
    }
}
