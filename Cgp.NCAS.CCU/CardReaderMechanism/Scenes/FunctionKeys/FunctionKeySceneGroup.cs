using System;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Drivers.CardReader;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.FunctionKeys
{
    internal class FunctionKeySceneGroup : ASceneAuthorizationSceneGroup<FunctionKeySceneGroup>
    {
        private readonly DB.FunctionKey _functionKey;

        private class Scene : CrBaseScene
        {
            private readonly FunctionKeySceneGroup _sceneGroup;

            public Scene(FunctionKeySceneGroup sceneGroup)
            {
                _sceneGroup = sceneGroup;
            }

            public override bool OnEntered(ACrSceneContext crSceneContext)
            {
                Show(crSceneContext);
                return true;
            }

            public override void OnReturned(ACrSceneContext crSceneContext)
            {
                Show(crSceneContext);
            }

            private void Show(ACrSceneContext crSceneContext)
            {
                string header =
                    _sceneGroup.CardReaderSettings
                        .GetLocalizationString("PerformAction");

                var sentences =
                    CrDisplayProcessor
                        .GetStringLinesForDisplay(_sceneGroup._functionKey.Text);

                var cardReader = crSceneContext.CardReader;

                cardReader.DisplayCommands.ClearAllDisplay(cardReader);

                cardReader.DisplayCommands
                    .DisplayText(
                        cardReader,
                        (byte)(12 - header.Length / 2),
                        0,
                        header);

                const int maxLines = 14;

                if (sentences != null)
                {
                    var posY = 2;

                    if (maxLines - sentences.Count > 0)
                        posY += (maxLines - sentences.Count) / 2;

                    foreach (var text in sentences)
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            var posX = CrDisplayProcessor.MaxDisplayCountOfChar / 2 - text.Length / 2;

                            if (posY > maxLines)
                                break;

                            cardReader.DisplayCommands.DisplayText(
                                cardReader,
                                (byte)posX,
                                (byte)posY,
                                text);
                        }

                        posY++;
                    }
                }

                cardReader.MenuCommands.SetBottomMenuButtons(
                    cardReader,
                    new CRBottomMenu
                    {
                        Button1 = CRMenuButtonLook.No,
                        Button1ReturnCode = CRSpecialKey.No,
                        Button2 = CRMenuButtonLook.Yes,
                        Button2ReturnCode = CRSpecialKey.Yes,
                        Button3 = CRMenuButtonLook.Clear,
                        Button4 = CRMenuButtonLook.Clear,
                    });
            }

            private void PerformFunctionKeyAction()
            {
                var functionKey = _sceneGroup._functionKey;

                var resultingState = State.Unknown;

                switch (functionKey.ObjectAction)
                {
                    case DB.ObjectAction.Activate:

                        Outputs.Singleton.On(
                            "FK" + functionKey.IdOutput,
                            functionKey.IdOutput);

                        resultingState = State.On;

                        break;

                    case DB.ObjectAction.Deactivate:

                        Outputs.Singleton.Off(
                            "FK" + functionKey.IdOutput,
                            functionKey.IdOutput);

                        resultingState = State.Off;

                        break;

                    case DB.ObjectAction.ActivateDeactivate:

                        if (!Outputs.Singleton.HasOutputActivator(
                            functionKey.IdOutput,
                            "FK" + functionKey.IdOutput))
                        {
                            Outputs.Singleton.On(
                                "FK" + functionKey.IdOutput,
                                functionKey.IdOutput);

                            resultingState = State.On;
                        }
                        else
                        {
                            Outputs.Singleton.Off(
                                "FK" + functionKey.IdOutput,
                                functionKey.IdOutput);

                            resultingState = State.Off;
                        }

                        break;
                }

                Events.ProcessEvent(
                    new EventFunctionKeyPressed(
                        _sceneGroup.CardReaderSettings.Id,
                        resultingState,
                        functionKey.IdOutput,
                        _sceneGroup.SceneAuthorizationProcess.AccessData.IdCard
                        ));
            }

            public override void OnSpecialKeyPressed(
                ACrSceneContext crSceneContext,
                CRSpecialKey specialKey)
            {
                switch (specialKey)
                {
                    case CRSpecialKey.Yes:

                        PerformFunctionKeyAction();
                        _sceneGroup.DefaultGroupExitRoute.Follow(crSceneContext);

                        break;

                    case CRSpecialKey.No:

                        _sceneGroup.DefaultGroupExitRoute.Follow(crSceneContext);

                        break;
                }
            }
        }

        private class AuthorizationProcessClass : ASceneAuthorizationProcess<FunctionKeySceneGroup>
        {
            public AuthorizationProcessClass(IInstanceProvider<FunctionKeySceneGroup> sceneGroupProvider)
                : base(sceneGroupProvider)
            {
            }

            protected override bool AuthorizeByCardInternal()
            {
                return HasAccess();
            }

            private bool HasAccess()
            {
                if (CardReaderSettings.DoorEnvironmentAdapter != null)
                    return CardReaderSettings.DoorEnvironmentAdapter.HasAccess(
                        AccessData,
                        CardReaderSettings.Id);

                return CardAccessRightsManager.Singleton.HasAccess(
                    AccessData,
                    CardReaderSettings.Id,
                    Guid.Empty,
                    CardReaderSettings.CardReaderDb.GuidDCU);
            }

            protected override bool AuthorizationByCardEnabled
            {
                get
                {
                    var securityLevel = SceneGroupProvider.Instance._functionKey.SecurityLevel;

                    return
                        securityLevel == DB.SecurityLevelForSpecialKey.CARD
                        || securityLevel == DB.SecurityLevelForSpecialKey.CARDPIN
                        || securityLevel == DB.SecurityLevelForSpecialKey.CodeOrCard
                        || securityLevel == DB.SecurityLevelForSpecialKey.CodeOrCardPin;
                }
            }

            protected override bool AuthorizationByCodeEnabled
            {
                get
                {
                    var securityLevel = SceneGroupProvider.Instance._functionKey.SecurityLevel;

                    return
                        securityLevel == DB.SecurityLevelForSpecialKey.Code
                        || securityLevel == DB.SecurityLevelForSpecialKey.CodeOrCard
                        || securityLevel == DB.SecurityLevelForSpecialKey.CodeOrCardPin;
                }
            }

            protected override bool CardRequiresPin
            {
                get
                {
                    var securityLevel = SceneGroupProvider.Instance._functionKey.SecurityLevel;

                    return
                        securityLevel == DB.SecurityLevelForSpecialKey.CARDPIN
                        || securityLevel == DB.SecurityLevelForSpecialKey.CodeOrCardPin;
                }
            }

            protected override string Gin
            {
                get
                {
                    return SceneGroupProvider.Instance._functionKey.GIN;
                }
            }

            protected override void OnAccessDeniedInvalidPin()
            {
                var idCardReader = CardReaderSettings.Id;

                if (BlockedAlarmsManager.Singleton.ProcessEvent(
                    AlarmType.CardReader_InvalidPIN,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader)))
                {
                    Events.ProcessEvent(
                        new EventAccessDeniedInvalidPin(
                            idCardReader,
                            AccessData));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new CrInvalidPinAlarm(
                        idCardReader,
                        AccessData.IdCard));
            }

            protected override bool AuthorizeByPersonInternal()
            {
                return HasAccess();
            }

            protected override void OnAccessDeniedInvalidCode()
            {
                var idCardReader = CardReaderSettings.Id;

                if (BlockedAlarmsManager.Singleton.ProcessEvent(
                    AlarmType.CardReader_InvalidCode,
                    new IdAndObjectType(
                        idCardReader,
                        ObjectType.CardReader)))
                {
                    Events.ProcessEvent(
                        new EventAccessDeniedInvalidCode(idCardReader));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new CrInvalidCodeAlarm(idCardReader));
            }
        }

        private class AuthorizationSceneGroup : AAuthorizationSceneGroup
        {
            private class WaitingForCodeScene : AAuthorizationSceneForcedGinCodeLedPresentation
            {
                public WaitingForCodeScene(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    cardReader.AccessCommands.WaitingForCode(
                        cardReader
                        //,CcuCardReaders.MaximalCodeLength
                        );
                }
            }

            private class WaitingForCardScene : AWaitingForCardScene
            {
                public WaitingForCardScene(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    cardReader.AccessCommands.WaitingForCard(cardReader);
                }
            }

            private class WaitingForPinScene : AAuthorizationScene
            {
                public WaitingForPinScene(AAuthorizationSceneGroup sceneGroup)
                    : base(sceneGroup)
                {
                }

                protected override void ShowInternal(CardReader cardReader)
                {
                    if (CcuCardReaders.IsPinConfirmationObligatory)
                        cardReader.AccessCommands.WaitingForPIN(
                            cardReader,
                            CcuCardReaders.MinimalPinLength,
                            CcuCardReaders.MaximalPinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            true);
                    else
                        cardReader.AccessCommands.WaitingForPIN(
                            cardReader,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            SceneGroup.ParentSceneGroup.SceneAuthorizationProcess.PinLength,
                            false);
                }
            }

            public AuthorizationSceneGroup(FunctionKeySceneGroup parentSceneGroup)
                : base(parentSceneGroup)
            {
            }

            protected override ICrScene GetSceneWaitingForCode()
            {
                return new WaitingForCodeScene(this);
            }

            protected override ICrScene GetSceneWaitingForCard()
            {
                return new WaitingForCardScene(this);
            }

            protected override ICrScene GetSceneWaitingForPin()
            {
                return new WaitingForPinScene(this);
            }
        }

        public FunctionKeySceneGroup(
            [NotNull] IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider,
            ACardReaderSettings cardReaderSettings,
            DB.FunctionKey functionKey)
            : this(
                parentDefaultRouteProvider,
                cardReaderSettings,
                functionKey,
                new DelayedInitReference<FunctionKeySceneGroup>())
        {

        }

        private FunctionKeySceneGroup(
            [NotNull] IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider,
            ACardReaderSettings cardReaderSettings,
            DB.FunctionKey functionKey,
            DelayedInitReference<FunctionKeySceneGroup> delayedInitReference)
            : base(
                new AuthorizationProcessClass(delayedInitReference),
                parentDefaultRouteProvider,
                cardReaderSettings)
        {
            delayedInitReference.Instance = this;

            _functionKey = functionKey;
        }

        protected override ICrScene CreateAuthorizedScene()
        {
            return new Scene(this);
        }

        protected override AAuthorizationSceneGroup CreateInnerAuthorizationSceneGroup()
        {
            return new AuthorizationSceneGroup(this);
        }
    }
}
