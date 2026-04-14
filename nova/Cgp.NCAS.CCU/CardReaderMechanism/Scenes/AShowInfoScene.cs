using Contal.Drivers.CardReader;
using Contal.IwQuick.Threads;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes
{
    internal abstract class AShowInfoScene<TSceneGroup> : CrBaseScene
        where TSceneGroup : ICrSceneGroup
    {
        protected enum ResultType
        {
            Success,
            Failure,
            Info
        }

        protected readonly TSceneGroup SceneGroup;

        protected AShowInfoScene(TSceneGroup sceneGroup)
        {
            SceneGroup = sceneGroup;
        }

        protected abstract CrDisplayProcessor CrDisplayProcessor
        {
            get;
        }

        protected abstract ACrSceneRoute Route
        {
            get;
        }

        protected abstract ResultType Result
        {
            get;
        }

        public override bool OnEntered(ACrSceneContext crSceneContext)
        {
            var cardReader = crSceneContext.CardReader;

            cardReader.DisplayCommands.ClearAllDisplay(cardReader);

            cardReader.MenuCommands.SetBottomMenuButtons(
                cardReader,
                new CRBottomMenu
                {
                    Button1 = CRMenuButtonLook.No,
                    Button1ReturnCode = CRSpecialKey.No,
                    Button4 = CRMenuButtonLook.Clear
                });

            SetIndicatorAnnouncement(cardReader);

            Show(cardReader);

            TimerManager.Static.StartTimeout(
                crSceneContext.ShowInfoDelay,
                crSceneContext,
                OnTimeout);

            return true;
        }

        private void SetIndicatorAnnouncement(CardReader cardReader)
        {
            IndicatorMode buzzerMode;
            IndicatorMode redLedMode;
            IndicatorMode greenLedMode;

            switch (Result)
            {
                case ResultType.Success:

                    buzzerMode = IndicatorMode.ShortPulse;
                    greenLedMode = IndicatorMode.ShortPulse;
                    redLedMode = IndicatorMode.Off;

                    break;

                case ResultType.Failure:

                    buzzerMode = IndicatorMode.LongPulse;
                    greenLedMode = IndicatorMode.Off;
                    redLedMode = IndicatorMode.LongPulse;

                    break;

                default:

                    buzzerMode = IndicatorMode.Click;
                    greenLedMode = IndicatorMode.Click;
                    redLedMode = IndicatorMode.Click;

                    break;
            }

            cardReader.ControlCommands.IndicatorAnnouncement(
                redLedMode,
                greenLedMode,
                buzzerMode,
                IndicatorMode.NoChange);
        }

        public override void OnSpecialKeyPressed(
            ACrSceneContext crSceneContext,
            CRSpecialKey specialKey)
        {
            if (specialKey == CRSpecialKey.No)
                Route.Follow(crSceneContext);
        }

        private bool OnTimeout(TimerCarrier timerCarrier)
        {
            var crSceneContext = ((ACrSceneContext)timerCarrier.Data);

            crSceneContext.PlanDelayedRouteFollowing(
                this,
                SceneGroup,
                Route);

            return true;
        }

        protected abstract void Show(CardReader cardReader);
    }
}
