using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Drivers.CardReader;
using Contal.IwQuick.Threads;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal abstract class ASetUnsetAllAlarmAreasQuestionSceneGroup :
        ACrSequentialSceneGroup
    {
        protected abstract class AResultScene : CrBaseScene
        {
            private ITimer _exitTimeout;
            private readonly object _exitTimeoutLock = new object();

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                var cardReader = crSceneContext.CardReader;

                cardReader.DisplayCommands.ClearAllDisplay(cardReader);

                var crDisplayProcessor =
                    CardReaderSettings.CrDisplayProcessor;

                if (Success)
                    DisplaySuccessMessage(crDisplayProcessor);
                else
                    DisplayFailureMessage(crDisplayProcessor);

                _exitTimeout = 
                    TimerManager.Static.StartTimeout(
                        TimeoutDuration,
                        crSceneContext,
                        OnTimeout);

                return true;
            }

            public override void OnAdvancing(ACrSceneContext crSceneContext)
            {
                if (_exitTimeout != null)
                    lock (_exitTimeoutLock)
                        if (_exitTimeout != null)
                            _exitTimeout.StopTimer();
            }

            public override void OnExiting(ACrSceneContext crSceneContext)
            {
                if (_exitTimeout != null)
                    lock (_exitTimeoutLock)
                        if (_exitTimeout != null)
                            _exitTimeout.StopTimer();
            }

            protected abstract long TimeoutDuration
            {
                get;
            }

            private bool OnTimeout(TimerCarrier timerCarrier)
            {
                lock (_exitTimeoutLock)
                {
                    _exitTimeout = null;
                    CrSceneAdvanceRoute.Default.Follow((ACrSceneContext)timerCarrier.Data);
                }

                return true;
            }

            protected abstract bool Success
            {
                get;
            }

            protected abstract ACardReaderSettings CardReaderSettings
            {
                get;
            }

            protected abstract void DisplaySuccessMessage(CrDisplayProcessor crDisplayProcessor);

            protected abstract void DisplayFailureMessage(CrDisplayProcessor crDisplayProcessor);

            public override void OnSpecialKeyPressed(
                ACrSceneContext crSceneContext,
                CRSpecialKey specialKey)
            {
                if (specialKey == CRSpecialKey.No)
                    CrSceneAdvanceRoute.Default.Follow(crSceneContext);
            }

            public abstract ICrScene GetDetailedResultScene();
        }

        private class QuestionScene : CrBaseScene
        {
            private readonly ASetUnsetAllAlarmAreasQuestionSceneGroup _sceneGroup;

            public QuestionScene(ASetUnsetAllAlarmAreasQuestionSceneGroup sceneGroup)
            {
                _sceneGroup = sceneGroup;
            }

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                var cardReader = crSceneContext.CardReader;

                if (_sceneGroup.SetAlarmAreas)
                    cardReader.AlarmAreaCommands.SetAlarmAreaQuestion(cardReader);
                else
                    cardReader.AlarmAreaCommands.UnsetAlarmAreaQuestion(cardReader);

                return true;
            }

            public override void OnSpecialKeyPressed(
                ACrSceneContext crSceneContext,
                CRSpecialKey specialKey)
            {
                switch (specialKey)
                {
                    case CRSpecialKey.Yes:
                        CrSceneAdvanceRoute.Default.Follow(crSceneContext);
                        break;

                    case CRSpecialKey.No:
                        _sceneGroup.DefaultGroupExitRoute.Follow(crSceneContext);
                        break;
                }
            }
        }

        public IAuthorizedSceneGroup ParentSceneGroup
        {
            get;
            private set;
        }

        public abstract IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> AlarmAreaInfos
        {
            get;
        }

        protected abstract bool SetAlarmAreas
        {
            get;
        }

        protected ASetUnsetAllAlarmAreasQuestionSceneGroup(
            IAuthorizedSceneGroup parentSceneGroup)
            : base(CrSceneGroupReturnRoute.Default)
        {
            ParentSceneGroup = parentSceneGroup;
        }

        protected override IEnumerable<ICrScene> Scenes
        {
            get
            {
                yield return new QuestionScene(this);

                ICrScene extraParametersScene = CreateExtraParametersScene();

                if (extraParametersScene != null)
                    yield return extraParametersScene;

                SetUnsetAllAlarmAreas();

                var resultScene = CreateResultScene();

                yield return resultScene;

                var detailedResultScene = resultScene.GetDetailedResultScene();

                if (detailedResultScene != null)
                    yield return detailedResultScene;
            }
        }

        protected abstract AResultScene CreateResultScene();

        protected abstract ICrScene CreateExtraParametersScene();

        protected abstract void SetUnsetAllAlarmAreas();

        public abstract void ProcessEventAccessDeniedInvalidPin();
    }
}
