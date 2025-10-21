using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;

using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class SetAllAlarmAreasQuestionSceneGroup : ASetUnsetAllAlarmAreasQuestionSceneGroup
    {
        private class ResultScene : AResultScene
        {
            private readonly SetAllAlarmAreasQuestionSceneGroup _sceneGroup;

            public ResultScene(SetAllAlarmAreasQuestionSceneGroup sceneGroup)
            {
                _sceneGroup = sceneGroup;
            }

            protected override long TimeoutDuration
            {
                get { return CardReaderConstants.SHOWINFODELAYPREMIUM; }
            }

            protected override bool Success
            {
                get { return _sceneGroup.CountAllAlarmAreas == _sceneGroup.CountSetAlarmAreas; }
            }

            protected override ACardReaderSettings CardReaderSettings
            {
                get { return _sceneGroup.ParentSceneGroup.CardReaderSettings; }
            }

            protected override void DisplaySuccessMessage(CrDisplayProcessor crDisplayProcessor)
            {
                crDisplayProcessor.DisplayWriteText(
                    crDisplayProcessor.GetLocalizationString("SetSuccess"),
                    0,
                    1);
            }

            protected override void DisplayFailureMessage(CrDisplayProcessor crDisplayProcessor)
            {
                crDisplayProcessor.DisplayWriteText(
                    string.Format(
                        crDisplayProcessor.GetLocalizationString("SetFailure"),
                        _sceneGroup.CountSetAlarmAreas),
                    0,
                    2);
            }

            public override ICrScene GetDetailedResultScene()
            {
                return null;
            }
        }
        
        private int _countAllAlarmAreas;
        private int _countSetAlarmAreas;

        private readonly IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> _alarmAreaInfos;

        public SetAllAlarmAreasQuestionSceneGroup(
            IAuthorizedSceneGroup parentSceneGroup,
            IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> alarmAreaInfos)
            : base(parentSceneGroup)
        {
            _alarmAreaInfos = alarmAreaInfos;
        }

        public override IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> AlarmAreaInfos
        {
            get { return _alarmAreaInfos; }
        }

        public int CountAllAlarmAreas
        {
            get { return _countAllAlarmAreas; }
        }

        public int CountSetAlarmAreas
        {
            get { return _countSetAlarmAreas; }
        }


        protected override bool SetAlarmAreas
        {
            get { return true; }
        }

        protected override AResultScene CreateResultScene()
        {
            return new ResultScene(this);
        }

        protected override ICrScene CreateExtraParametersScene()
        {
            return null;
        }

        protected override void SetUnsetAllAlarmAreas()
        {
            var accessData = ParentSceneGroup.AccessData;

            _countAllAlarmAreas = 0;
            _countSetAlarmAreas = 0;

            var cardReaderMechanism = ParentSceneGroup.CardReaderSettings;

            var idCardReader =
                cardReaderMechanism.Id;

            foreach (var alarmArea in _alarmAreaInfos)
            {
                var idAlarmArea = alarmArea.IdAlarmArea;

                ++_countAllAlarmAreas;

                EventParameters.EventParameters eventParameters;

                if (AlarmArea.AlarmAreas.Singleton.SetAlarmArea(
                    idAlarmArea,
                    false,
                    new AlarmArea.AlarmAreas.SetUnsetParams(
                        idCardReader,
                        accessData,
                        Guid.Empty,
                        false,
                        false)))
                {
                    eventParameters = new EventSetAlarmAreaFromCardReader(
                        idCardReader,
                        accessData,
                        idAlarmArea);

                    ++_countSetAlarmAreas;
                }
                else
                {
                    eventParameters = new EventAlarmAreaSetFromCrFailed(
                        idCardReader,
                        accessData,
                        idAlarmArea);
                }

                Events.ProcessEvent(eventParameters);
            }
        }

        public override void ProcessEventAccessDeniedInvalidPin()
        {
            Events.ProcessEvent(
                new EventCrAccessDeniedSetAlarmAreaInvalidPin(
                    ParentSceneGroup.CardReaderSettings.Id,
                    ParentSceneGroup.AccessData,
                    Guid.Empty));
        }
    }
}