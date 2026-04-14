using Contal.Drivers.CardReader;
using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal abstract class AShowInfoAlarmAreaScene<TSceneGroup> : 
        AShowInfoScene<TSceneGroup>
        where TSceneGroup : ICrSceneGroup
    {
        protected abstract CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
        {
            get;
        }

        protected AShowInfoAlarmAreaScene(TSceneGroup sceneGroup)
            : base(sceneGroup)
        {
        }

        protected override void Show(CardReader cardReader)
        {
            if (ShowAlarmAreaName && CrAlarmAreaInfo != null)
            {
                var alarmAreaName = CrAlarmAreaInfo.ToString();

                if (alarmAreaName != string.Empty)
                {
                    cardReader.DisplayCommands.DisplayText(
                        cardReader,
                        0,
                        0,
                        alarmAreaName);
                }
            }

            ShowInternal(cardReader);
        }

        protected abstract void ShowInternal(CardReader cardReader);

        protected abstract bool ShowAlarmAreaName
        {
            get;
        }
    }

    internal abstract class AShowInfoAlarmAreaScene : AShowInfoAlarmAreaScene<ICrSceneGroup>
    {
        private readonly CrDisplayProcessor _crDisplayProcessor;

        private readonly ACrSceneRoute _route;

        private readonly CrAlarmAreasManager.CrAlarmAreaInfo _crAlarmAreaInfo;

        protected AShowInfoAlarmAreaScene(
            ICrSceneGroup sceneGroup,
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            CrDisplayProcessor crDisplayProcessor,
            ACrSceneRoute route)
            : base(sceneGroup)
        {
            _crDisplayProcessor = crDisplayProcessor;
            _route = route;
            _crAlarmAreaInfo = crAlarmAreaInfo;
        }

        protected override CrDisplayProcessor CrDisplayProcessor
        {
            get { return _crDisplayProcessor; }
        }

        protected override ACrSceneRoute Route
        {
            get { return _route; }
        }

        protected override CrAlarmAreasManager.CrAlarmAreaInfo CrAlarmAreaInfo
        {
            get { return _crAlarmAreaInfo; }
        }
    }

    internal class SetAlarmAreaSucceededSceneGroup :
        CrSimpleSceneGroup
    {
        private class Scene : AShowInfoAlarmAreaScene
        {
            public Scene(
                ICrSceneGroup crSceneGroup,
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                CrDisplayProcessor crDisplayProcessor,
                CrSceneGroupExitRoute exitRoute)
                : base(
                    crSceneGroup,
                    crAlarmAreaInfo,
                    crDisplayProcessor,
                    exitRoute)
            {
            }

            protected override bool ShowAlarmAreaName
            {
                get { return true; }
            }

            protected override void ShowInternal(CardReader cardReader)
            {
                bool isExternalAA;
                bool isWaiting;
                bool wasConfirmed;

                AlarmArea.AlarmAreas.Singleton.GetStateOfExternalAA(
                    CrAlarmAreaInfo.IdAlarmArea,
                    out isExternalAA,
                    out isWaiting,
                    out wasConfirmed);

                if (!isExternalAA
                    || (!isWaiting && wasConfirmed))
                {
                    CrDisplayProcessor.DisplayWriteText(
                        CrDisplayProcessor.GetLocalizationString("SetAAsucceeded"),
                        0,
                        2);
                }
                else
                    // in case isExternalAASet && !isWaitingSet && !wasConfirmSet
                    // the process of setting the zone failed
                    CrDisplayProcessor.DisplayWriteText(
                        isWaiting
                            ? CrDisplayProcessor.GetLocalizationString("WaitingForEIS")
                            : CrDisplayProcessor.GetLocalizationString("SetAAfailed"),
                        0,
                        2);
            }

            protected override ResultType Result
            {
                get { return ResultType.Success; }
            }
        }

        public SetAlarmAreaSucceededSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            CrDisplayProcessor crDisplayProcessor,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : this(
                crAlarmAreaInfo,
                crDisplayProcessor,
                parentDefaultRouteProvider,
                new DelayedInitReference<ICrScene>())
        {
        }

        private SetAlarmAreaSucceededSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            CrDisplayProcessor crDisplayProcessor,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider,
            DelayedInitReference<ICrScene> delayedInitReference)
            : base(
                delayedInitReference,
                parentDefaultRouteProvider)
        {
            delayedInitReference.Instance = new Scene(
                this,
                crAlarmAreaInfo,
                crDisplayProcessor,
                DefaultGroupExitRoute);
        }
    }

    internal class SetAlarmAreaFailedSceneGroup :
        CrSimpleSceneGroup
    {
        private class Scene : AShowInfoAlarmAreaScene
        {
            public Scene(
                ICrSceneGroup crSceneGroup,
                CrDisplayProcessor crDisplayProcessor,
                CrSceneGroupExitRoute exitRoute)
                : base(
                    crSceneGroup,
                    null,
                    crDisplayProcessor,
                    exitRoute)
            {
            }

            protected override void ShowInternal(CardReader cardReader)
            {
                CrDisplayProcessor.DisplayWriteText(
                    CrDisplayProcessor.GetLocalizationString("AAcannotBeSet"),
                    0,
                    0);
            }

            protected override bool ShowAlarmAreaName
            {
                get { return false; }
            }

            protected override ResultType Result
            {
                get { return ResultType.Failure; }
            }
        }

        public SetAlarmAreaFailedSceneGroup(CrDisplayProcessor crDisplayProcessor,
            IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
            : this(crDisplayProcessor,
                new DelayedInitReference<ICrScene>(),
                defaultRouteProvider)
        {
        }

        private SetAlarmAreaFailedSceneGroup(CrDisplayProcessor crDisplayProcessor,
            DelayedInitReference<ICrScene> delayedInitReference,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(
                delayedInitReference,
                parentDefaultRouteProvider)
        {
            delayedInitReference.Instance =
                new Scene(
                    this,
                    crDisplayProcessor,
                    DefaultGroupExitRoute);
        }
    }

    internal class UnconditionalSetAlarmAreaSucceededSceneGroup :
        CrSimpleSceneGroup
    {
        private class Scene : AShowInfoAlarmAreaScene
        {
            public Scene(
                ICrSceneGroup crSceneGroup,
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                CrDisplayProcessor crDisplayProcessor,
                CrSceneGroupExitRoute exitRoute)
                : base(
                    crSceneGroup,
                    crAlarmAreaInfo,
                    crDisplayProcessor,
                    exitRoute)
            {
            }

            protected override void ShowInternal(CardReader cardReader)
            {
                CrDisplayProcessor.DisplayWriteText(
                    CrDisplayProcessor.GetLocalizationString("UnconditionalSetAAsucceeded"),
                    0,
                    2);
            }

            protected override bool ShowAlarmAreaName
            {
                get { return true; }
            }

            protected override ResultType Result
            {
                get { return ResultType.Success; }
            }
        }

        public UnconditionalSetAlarmAreaSucceededSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            CrDisplayProcessor crDisplayProcessor,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : this(
                crAlarmAreaInfo,
                crDisplayProcessor,
                parentDefaultRouteProvider,
                new DelayedInitReference<ICrScene>())
        {
        }

        private UnconditionalSetAlarmAreaSucceededSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            CrDisplayProcessor crDisplayProcessor,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider,
            DelayedInitReference<ICrScene> sceneProvider)
            : base(
                sceneProvider,
                parentDefaultRouteProvider)
        {
            sceneProvider.Instance = new Scene(
                this,
                crAlarmAreaInfo,
                crDisplayProcessor,
                DefaultGroupExitRoute);
        }
    }

    internal class UnconditionalSetAlarmAreaFailedSceneGroup :
        CrSimpleSceneGroup
    {
        private class Scene : AShowInfoAlarmAreaScene
        {
            public Scene(
                ICrSceneGroup crSceneGroup,
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                CrDisplayProcessor crDisplayProcessor,
                CrSceneGroupExitRoute exitRoute)
                : base(
                    crSceneGroup,
                    crAlarmAreaInfo,
                    crDisplayProcessor,
                    exitRoute)
            {
            }

            protected override void ShowInternal(CardReader cardReader)
            {
                CrDisplayProcessor.DisplayWriteText(
                    CrDisplayProcessor.GetLocalizationString("UnconditionalSetAAfailed"),
                    0,
                    2);
            }

            protected override bool ShowAlarmAreaName
            {
                get { return true; }
            }

            protected override ResultType Result
            {
                get { return ResultType.Failure; }
            }
        }

        public UnconditionalSetAlarmAreaFailedSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            CrDisplayProcessor crDisplayProcessor,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : this(
                crAlarmAreaInfo,
                crDisplayProcessor,
                parentDefaultRouteProvider,
                new DelayedInitReference<ICrScene>())
        {
        }

        private UnconditionalSetAlarmAreaFailedSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            CrDisplayProcessor crDisplayProcessor,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider,
            DelayedInitReference<ICrScene> sceneProvider)
            : base(
                sceneProvider,
                parentDefaultRouteProvider)
        {
            sceneProvider.Instance =
                new Scene(
                    this,
                    crAlarmAreaInfo,
                    crDisplayProcessor,
                    DefaultGroupExitRoute);
        }
    }

    internal class NotAcknowledgedScene : AShowInfoScene<ICcuSceneGroup>
    {
        public NotAcknowledgedScene(ICcuSceneGroup sceneGroup)
            : base(sceneGroup)
        {
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
            get { return ResultType.Info; }
        }

        protected override void Show(CardReader cardReader)
        {
            CrDisplayProcessor.DisplayWriteText(
                CrDisplayProcessor.GetLocalizationString("AlarmAreaHasBeenInAlarmStateDuringSetPeriod"),
                0,
                1);
        }
    }

    internal class UnsetAlarmAreaSucceededSceneGroup :
        CrSimpleSceneGroup,
        ICcuSceneGroup
    {
        private readonly CrAlarmAreasManager.CrAlarmAreaInfo _crAlarmAreaInfo;

        private readonly bool _alarmNotAcknowledgedDuringSetPeriod;

        private class Scene : AShowInfoAlarmAreaScene<UnsetAlarmAreaSucceededSceneGroup>
        {
            public Scene(
                UnsetAlarmAreaSucceededSceneGroup sceneGroup)
                : base(sceneGroup)
            {
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
                bool isExteranlAAUnset;
                bool isWaiting;
                bool wasConfirmUnset;

                AlarmArea.AlarmAreas.Singleton.GetStateOfExternalAA(
                    CrAlarmAreaInfo.IdAlarmArea,
                    out isExteranlAAUnset,
                    out isWaiting,
                    out wasConfirmUnset);

                byte top =
                    CrDisplayProcessor.DisplayWriteText(
                        CrDisplayProcessor.GetLocalizationString(
                            !isExteranlAAUnset || (!isWaiting && wasConfirmUnset)
                                ? "UnsetAAsucceeded"
                                : (isWaiting
                                    ? "WaitingForEIS"
                                    : "UnsetAAfailed")),
                        0,
                        2);

                if (SceneGroup._alarmNotAcknowledgedDuringSetPeriod)
                    CrDisplayProcessor.DisplayWriteText(
                        CrDisplayProcessor.GetLocalizationString("AlarmAreaHasBeenInAlarmStateDuringSetPeriod"),
                        0,
                        (byte)(top + 2));
            }

            protected override CrDisplayProcessor CrDisplayProcessor
            {
                get { return SceneGroup.CardReaderSettings.CrDisplayProcessor; }
            }

            protected override ACrSceneRoute Route
            {
                get { return CrSceneAdvanceRoute.Default; }
            }

            protected override ResultType Result
            {
                get { return ResultType.Success; }
            }
        }

        public UnsetAlarmAreaSucceededSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            ACardReaderSettings cardReaderSettings,
            bool alarmNotAcknowledgedDuringSetPeriod,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : this(
                new DelayedInitReference<ICrScene>(),  
                parentDefaultRouteProvider)
        {
            _crAlarmAreaInfo = crAlarmAreaInfo;

            CardReaderSettings = cardReaderSettings;

            _alarmNotAcknowledgedDuringSetPeriod = alarmNotAcknowledgedDuringSetPeriod;
        }

        private UnsetAlarmAreaSucceededSceneGroup(
            DelayedInitReference<ICrScene> sceneProvider,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(sceneProvider, parentDefaultRouteProvider)
        {
            sceneProvider.Instance = new Scene(this);
        }

        public ACardReaderSettings CardReaderSettings
        {
            get;
            private set;
        }
    }

    internal class UnsetAlarmAreaFailedSceneGroup : CrSimpleSceneGroup
    {
        private class Scene : AShowInfoAlarmAreaScene
        {
            public Scene(
                ACrSceneGroup crSceneGroup,
                CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
                CrDisplayProcessor crDisplayProcessor,
                CrSceneGroupExitRoute exitRoute)
                : base(
                    crSceneGroup,
                    crAlarmAreaInfo,
                    crDisplayProcessor,
                    exitRoute)
            {
            }

            protected override void ShowInternal(CardReader cardReader)
            {
                CrDisplayProcessor.DisplayWriteText(
                    CrDisplayProcessor.GetLocalizationString("UnsetAAfailed"),
                    0,
                    2);
            }

            protected override bool ShowAlarmAreaName
            {
                get { return true; }
            }

            protected override ResultType Result
            {
                get { return ResultType.Failure; }
            }
        }

        public UnsetAlarmAreaFailedSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            CrDisplayProcessor crDisplayProcessor,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : this(
                crAlarmAreaInfo,
                crDisplayProcessor,
                parentDefaultRouteProvider,
                new DelayedInitReference<ICrScene>())
        {
        }

        private UnsetAlarmAreaFailedSceneGroup(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            CrDisplayProcessor crDisplayProcessor,
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider,
            DelayedInitReference<ICrScene> delayedInitReference)
            : base(
                delayedInitReference,
                parentDefaultRouteProvider)
        {
            delayedInitReference.Instance =
                new Scene(
                    this,
                    crAlarmAreaInfo,
                    crDisplayProcessor,
                    DefaultGroupExitRoute);
        }
    }
}
