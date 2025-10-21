using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class AlarmAreaTimeBuyingSceneGroup :
        ACrSequentialSceneGroup,
        TimeBuyingMenuScene.ITimeBuyingSceneGroup,
        ICcuSceneGroup
    {
        public static string FormatTimeStringForCR(int timeInSeconds)
        {
            return
                string.Format(
                    "{0:00}:{1:00}:{2:00}",
                    timeInSeconds / 3600,
                    (timeInSeconds % 3600) / 60,
                    timeInSeconds % 60);
        }

        private class UnsetAlarmAreaResult : AlarmAreaStateAndSettings.IAlarmAreaUnsetResult
        {
            private int _timeToBuy;
            private int _remainingTime;

            public bool NonAcknowledgedAlarmDuringSetPeriod
            {
                get;
                private set;
            }

            public void OnFailed(AlarmAreaActionResult alarmAreaActionResult, int timeToBuy, int remainingTime)
            {
                Reason = alarmAreaActionResult;

                _timeToBuy = timeToBuy;
                _remainingTime = remainingTime;
            }

            public void OnSucceded(
                int timeToBuy,
                int remainingTime,
                bool nonAcknowledgedAlarmDuringSetPeriod)
            {
                Success = true;

                _remainingTime = remainingTime;
                _timeToBuy = timeToBuy;

                NonAcknowledgedAlarmDuringSetPeriod = nonAcknowledgedAlarmDuringSetPeriod;
            }

            public int TimeToBuy
            {
                get { return _timeToBuy; }
            }

            public int RemainingTime
            {
                get { return _remainingTime; }
            }

            public AlarmAreaActionResult Reason { get; private set; }

            public bool Success { get; private set; }

        }

        private class TimeBuyingSucceededScene : 
            AShowInfoAlarmAreaScene<AlarmAreaTimeBuyingSceneGroup>
        {
            [NotNull]
            private readonly UnsetAlarmAreaResult _unsetResult;

            public TimeBuyingSucceededScene(
                AlarmAreaTimeBuyingSceneGroup sceneGroup,
                UnsetAlarmAreaResult unsetResult)
                : base(sceneGroup)
            {
                _unsetResult = unsetResult;
            }

            protected override bool ShowAlarmAreaName
            {
                get { return true; }
            }

            protected override CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
            {
                get { return SceneGroup._crAlarmAreaInfo; }
            }

            protected override void ShowInternal(CardReader cardReader)
            {
                var timeToBuy = _unsetResult.TimeToBuy;

                string usedTimeString = timeToBuy >= 0
                                  && timeToBuy != int.MaxValue
                    ? FormatTimeStringForCR(timeToBuy)
                    : CrDisplayProcessor.GetLocalizationString("UNKNOWN");

                var remainingTime = _unsetResult.RemainingTime;

                string remainingTimeString = remainingTime >= 0
                    ? (remainingTime != int.MaxValue
                        ? FormatTimeStringForCR(remainingTime)
                        : CrDisplayProcessor.GetLocalizationString("Unlimited"))
                    : CrDisplayProcessor.GetLocalizationString("UNKNOWN");

                byte top =
                    CrDisplayProcessor.DisplayWriteText(
                        CrDisplayProcessor.GetLocalizationString("TimeBuyingSucceeded"),
                        0,
                        1);

                // Print Bought time
                top = CrDisplayProcessor.DisplayWriteText(
                    string.Format(
                        "{0}: {1}",
                        CrDisplayProcessor.GetLocalizationString("BoughtTime"),
                        usedTimeString),
                    0,
                    (byte)(top + 1));

                // Print remaining or missing time
                top =
                    CrDisplayProcessor.DisplayWriteText(
                        string.Format(
                            "{0}: {1}",
                            CrDisplayProcessor.GetLocalizationString("Remining"),
                            remainingTimeString),
                        0,
                        (byte)(top + 1));

                if (_unsetResult.NonAcknowledgedAlarmDuringSetPeriod)
                    CrDisplayProcessor.DisplayWriteText(
                        CrDisplayProcessor.GetLocalizationString(
                            "AlarmAreaHasBeenInAlarmStateDuringSetPeriod"),
                        0,
                        (byte)(top + 2));
            }

            protected override CrDisplayProcessor CrDisplayProcessor
            {
                get { return SceneGroup.CardReaderSettings.CrDisplayProcessor; }
            }

            protected override ACrSceneRoute Route
            {
                get { return SceneGroup.DefaultGroupExitRoute; }
            }

            protected override ResultType Result
            {
                get { return ResultType.Success; }
            }
        }

        private class TimeBuyingFailedScene : 
            AShowInfoAlarmAreaScene<AlarmAreaTimeBuyingSceneGroup>
        {
            private readonly UnsetAlarmAreaResult _unsetResult;

            public TimeBuyingFailedScene(
                AlarmAreaTimeBuyingSceneGroup sceneGroup,
                UnsetAlarmAreaResult unsetResult)
                : base(sceneGroup)
            {
                _unsetResult = unsetResult;
            }

            protected override CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
            {
                get { return SceneGroup._crAlarmAreaInfo; }
            }

            protected override void ShowInternal(CardReader cardReader)
            {
                var usedTime = CrDisplayProcessor.GetLocalizationString("UNKNOWN");

                string remainingTime = string.Empty;

                var tempTime = _unsetResult.TimeToBuy;

                if (tempTime >= 0
                    && tempTime != int.MaxValue)
                    usedTime = FormatTimeStringForCR(tempTime);

                tempTime = _unsetResult.RemainingTime;

                if (tempTime >= 0 && tempTime != int.MaxValue)
                {
                    if (tempTime >= 0)
                    {
                        remainingTime = tempTime != int.MaxValue
                            ? CrDisplayProcessor.GetLocalizationString("Remining")
                                + ": " + FormatTimeStringForCR(tempTime)
                            : CrDisplayProcessor.GetLocalizationString("Remining");
                    }
                    else
                        if (tempTime < 0 && tempTime != int.MinValue)
                            remainingTime =
                                CrDisplayProcessor.GetLocalizationString("Missing")
                                + ": " + FormatTimeStringForCR(tempTime * -1);
                }

                byte top =
                    CrDisplayProcessor.DisplayWriteText(
                        CrDisplayProcessor.GetLocalizationString("TimeBuyingFailed"),
                        0,
                        1);

                top =
                    CrDisplayProcessor.DisplayWriteText(
                        CrDisplayProcessor.GetLocalizationString(_unsetResult.Reason.ToString()),
                        0,
                        ++top);

                // Print Bought time
                top =
                    CrDisplayProcessor.DisplayWriteText(
                        CrDisplayProcessor.GetLocalizationString("BoughtTime") + ": " + usedTime,
                        0,
                        ++top);

                // Print remaining or missing time
                if (remainingTime != string.Empty)
                    CrDisplayProcessor.DisplayWriteText(
                        remainingTime,
                        0,
                        ++top);
            }

            protected override bool ShowAlarmAreaName
            {
                get { return true; }
            }

            protected override CrDisplayProcessor CrDisplayProcessor
            {
                get { return SceneGroup.CardReaderSettings.CrDisplayProcessor; }
            }

            protected override ACrSceneRoute Route
            {
                get { return SceneGroup.DefaultGroupExitRoute; }
            }

            protected override ResultType Result
            {
                get { return ResultType.Failure; }
            }
        }

        private readonly CrAlarmAreasManager.CrAlarmAreaInfo _crAlarmAreaInfo;
        private readonly IAuthorizedSceneGroup _parentSceneGroup;

        public AlarmAreaTimeBuyingSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            IAuthorizedSceneGroup parentSceneGroup,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(parentDefaultRouteProvider)
        {
            _crAlarmAreaInfo = crAlarmAreaInfo;
            _parentSceneGroup = parentSceneGroup;
        }

        public ACardReaderSettings CardReaderSettings
        {
            get
            {
                return _parentSceneGroup.CardReaderSettings;
            }
        }

        public int TimeToBuy
        {
            set;
            private get;
        }

        bool TimeBuyingMenuScene.ITimeBuyingSceneGroup.MaxTimeBuyingEnabled
        {
            get
            {
                var alarmArea = _crAlarmAreaInfo.AlarmArea;
                return alarmArea.TimeBuyingMaxDuration.HasValue
                        || alarmArea.TimeBuyingTotalMax.HasValue;
            }
        }

        bool TimeBuyingMenuScene.ITimeBuyingSceneGroup.IsUnsetEnabledInTimeBuyingScene
        {
            get
            {
                var idAlarmArea = _crAlarmAreaInfo.IdAlarmArea;

                var accessData = _parentSceneGroup.AccessData;

                return
                    _crAlarmAreaInfo.IsUnsettable
                    && ((!accessData.EntryViaCard
                         && !accessData.EntryViaPersonalCode)
                        || AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnset(accessData, idAlarmArea)
                        || AlarmArea.AlarmAreas.Singleton.HasInactiveObjectForForcedTimeBuying(idAlarmArea));
            }
        }

        protected override IEnumerable<ICrScene> Scenes
        {
            get
            {
                var accessData = _parentSceneGroup.AccessData;

                // Check if person can buy time for this alarm area
                var idAlarmArea = _crAlarmAreaInfo.IdAlarmArea;

                var cardReaderMechanism =
                    _parentSceneGroup.CardReaderSettings;

                Guid idCardReader =
                    cardReaderMechanism.Id;

                if (accessData.EntryViaCard
                    || accessData.EntryViaPersonalCode)
                {
                    if (!CheckAccessToTimeBuying(accessData))
                    {
                        Events.ProcessEvent(new EventCrAccessDeniedUnsetAlarmAreaNoRights(
                            idCardReader,
                            accessData,
                            idAlarmArea));

                        // AclSettingAA is not in local database or person does not have enough rights
                        yield return new RejectedScene(DefaultGroupExitRoute);
                        yield break;
                    }
                }

                yield return new TimeBuyingMenuScene(this);

                var alarmAreaUnsetResult = PerformTimeBuying(
                    accessData,
                    idCardReader,
                    idAlarmArea);

                if (alarmAreaUnsetResult.Success)
                {
                    Events.ProcessEvent(
                        new EventUnsetAlarmAreaFromCardReader(
                            idCardReader,
                            accessData,
                            idAlarmArea));

                    if (TimeToBuy == 0)
                        yield return
                            new CrSceneGroupWrapperScene<UnsetAlarmAreaSucceededSceneGroup>(
                                new UnsetAlarmAreaSucceededSceneGroup(
                                    _crAlarmAreaInfo,
                                    CardReaderSettings,
                                    alarmAreaUnsetResult.NonAcknowledgedAlarmDuringSetPeriod,
                                    CrSceneAdvanceRoute.Default));
                    else
                        yield return new TimeBuyingSucceededScene(
                            this,
                            alarmAreaUnsetResult);

                    yield break;
                }

                yield return new TimeBuyingFailedScene(
                    this,
                    alarmAreaUnsetResult);
            }
        }

        private bool CheckAccessToTimeBuying(AccessDataBase accessData)
        {
            if (!_crAlarmAreaInfo.IsTimeBuyingPossible)
            {
                return false;
            }

            var idAlarmArea = _crAlarmAreaInfo.IdAlarmArea;

            return
                _crAlarmAreaInfo.IsUnsettable
                && (AlarmAreaAccessRightsManager.Singleton.CheckRigthsToTimeBuying(
                    accessData,
                    idAlarmArea)
                    || AlarmAreaAccessRightsManager.Singleton.CheckRigthsToUnset(
                        accessData,
                        idAlarmArea));
        }

        private UnsetAlarmAreaResult PerformTimeBuying(
            AccessDataBase accessData,
            Guid idCardReader,
            Guid idAlarmArea)
        {

            var alarmAreaUnsetResult = new UnsetAlarmAreaResult();

            AlarmArea.AlarmAreas.Singleton.UnsetAlarmArea(
                alarmAreaUnsetResult,
                true,
                idAlarmArea,
                Guid.Empty,
                accessData.IdPerson,
                TimeToBuy,
                new AlarmArea.AlarmAreas.SetUnsetParams(
                    idCardReader,
                    accessData,
                    Guid.Empty,
                    false,
                    false));

            return alarmAreaUnsetResult;
        }
    }
}
