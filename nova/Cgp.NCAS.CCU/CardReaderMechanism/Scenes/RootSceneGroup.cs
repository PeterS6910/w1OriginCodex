using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Access;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes
{
    internal interface ICcuSceneGroup : ICrSceneGroup
    {
        ACardReaderSettings CardReaderSettings
        {
            get;
        }
    }

    internal class RootSceneGroup : 
        ACrSequentialSceneGroup,
        ICcuSceneGroup
    {
        private class OutOfOrderScene : CrBaseScene
        {
            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                var cardReader = crSceneContext.CardReader;

                cardReader.AccessCommands.OutOfOrder(cardReader);
                return true;
            }
        }

        private class AlarmPanelScene : BaseRootScene
        {
            public AlarmPanelScene(RootSceneGroup rootSceneGroup)
                : base(rootSceneGroup)
            {
            }

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                Show(crSceneContext);
                return true;
            }

            private void Show(ACrSceneContext crSceneContext)
            {
                var cardReader = crSceneContext.CardReader;

                cardReader.DisplayCommands.ClearAllDisplay(cardReader);

                var cardReaderSettings = CardReaderSettings;

                cardReader.DisplayCommands.DisplayText(
                    cardReader, 
                    0, 
                    0, 
                    cardReaderSettings.GetLocalizationString(CardReaderConstants.MenuAlarmAreaPanel));

                cardReader.ControlCommands.IndicatorAnnouncement(
                    IndicatorMode.Off,
                    IndicatorMode.Off,
                    IndicatorMode.Off,
                    IndicatorMode.Off);

                cardReader.ParentCommunicator.SendMessage(
                    cardReaderSettings.GetImplicitLowMenuButtonsMessage(
                        cardReaderSettings.ImplicitCrAlarmAreaInfo));
            }

            public override void OnReturned(ACrSceneContext crSceneContext)
            {
                Show(crSceneContext);
            }
        }

        private class ImplicitExternalAlarmAreaHandshakeFailureScene : AShowInfoScene<RootSceneGroup>
        {
            private readonly bool _failedOperationIsSet;

            public ImplicitExternalAlarmAreaHandshakeFailureScene(
                RootSceneGroup sceneGroup,
                bool failedOperationIsSet)
                : base(sceneGroup)
            {
                _failedOperationIsSet = failedOperationIsSet;
            }

            protected override CrDisplayProcessor CrDisplayProcessor
            {
                get { return SceneGroup.CardReaderSettings.CrDisplayProcessor; }
            }

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                var doorEnvironmentAdapter =
                    SceneGroup.CardReaderSettings.DoorEnvironmentAdapter;

                if (doorEnvironmentAdapter != null)
                    doorEnvironmentAdapter.SuppressCardReader();

                return base.OnEntered(crSceneContext);
            }

            protected override ACrSceneRoute Route
            {
                get
                {
                    SceneGroup.CardReaderSettings.SetImplicitExternalAlarmAreaHandshakeState(ExternalAlarmAreaHandshakeState.Ready);
                    return CrSceneAdvanceRoute.Default;
                }
            }

            protected override ResultType Result
            {
                get { return ResultType.Failure; }
            }

            protected override void Show(CardReader cardReader)
            {
                CrDisplayProcessor.DisplayWriteText(
                    CrDisplayProcessor.GetLocalizationString(
                        _failedOperationIsSet
                            ? "SetAAfailed"
                            : "UnsetAAfailed"),
                        0,
                        2);
            }
        }

        public ACardReaderSettings CardReaderSettings
        {
            get;
            private set;
        }

        public RootSceneGroup(ACardReaderSettings cardReaderSettings)
            : this(
                cardReaderSettings,
                new DelayedInitReference<ACrSceneRoute>(),
                new DelayedInitReference<RootSceneGroup>())
        {

        }

        private RootSceneGroup(
            ACardReaderSettings cardReaderSettings,
            [NotNull] DelayedInitReference<ACrSceneRoute> parentDefaultRouteProvider,
            DelayedInitReference<RootSceneGroup> delayedInitReference)
            : base(
                parentDefaultRouteProvider)
        {
            CardReaderSettings = cardReaderSettings;
            delayedInitReference.Instance = this;

            parentDefaultRouteProvider.Instance = EnterRoute;
        }

        protected override IEnumerable<ICrScene> Scenes
        {
            get
            {
                if (CardReaderSettings.WasImplicitExternalAlarmAreaHandshakeFailureToSet)
                    yield return new ImplicitExternalAlarmAreaHandshakeFailureScene(
                        this,
                        true);
                else
                    if (CardReaderSettings.WasImplicitExternalAlarmAreaHandshakeFailureToUnset)
                        yield return new ImplicitExternalAlarmAreaHandshakeFailureScene(
                            this,
                            false);

                if (CardReaderSettings.IsAccessAuthorizationEnabled)
                {
                    var accessSceneGroup = new AccessSceneGroup(this);

                    yield return new CrSceneGroupWrapperScene<AccessSceneGroup>(accessSceneGroup);

                    switch (accessSceneGroup.AuthorizationProcessState)
                    {
                        case AuthorizationProcessState.Rejected:

                            yield return new AccessDeniedScene(this);
                            break;

                        case AuthorizationProcessState.Redundant:

                            yield return new AccessRedundantScene(this);
                            break;
                    }

                    yield break;
                }

                if (CardReaderSettings.DoorEnvironmentAdapter != null)
                {
                    yield return new ImplicitCodeScene(
                        CardReaderSettings,
                        new BaseRootScene(
                            this));
                    
                    yield break;
                }

                var crAlarmAreasManager = CardReaderSettings.CrAlarmAreasManager;

                var cardReader = CardReaderSettings.CardReader;

                yield return
                   cardReader != null
                    && cardReader.HasDisplay
                    && crAlarmAreasManager != null
                    && crAlarmAreasManager.AlarmAreaCount > 0
                        ? (ICrScene)new AlarmPanelScene(this)
                        : new OutOfOrderScene();
            }
        }

        public override bool IsTopMost
        {
            get { return true; }
        }
    }
}