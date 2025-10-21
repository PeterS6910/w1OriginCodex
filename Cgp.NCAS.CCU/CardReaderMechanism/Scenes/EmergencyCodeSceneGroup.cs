using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Drivers.CardReader;
using Contal.IwQuick.Crypto;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes
{
    internal class EmergencyCodeSceneGroup : CrSimpleSceneGroup
    {
        private class Scene : CrSceneDecorator
        {
            private class AccessData : AccessDataBase
            {
                public override AccessGrantedSource AccessGrantedSource
                {
                    get { return AccessGrantedSource.EmergencyCode; }
                }
            }

            private readonly ACardReaderSettings _cardReaderSettings;
            private readonly CrSceneGroupExitRoute _returnExitRoute;

            private readonly string _expectedEmergencyCodeHash;

            public Scene(
                ACardReaderSettings cardReaderSettings,
                CrSceneGroupExitRoute returnExitRoute,
                string expectedEmergencyCodeHash,
                int expectedEmergencyCodeLength)
                : base(new CrWaitForEmergencyCodeScene(
                    CcuCardReaders.MinimalPinLength,
                    CcuCardReaders.MaximalPinLength,
                    expectedEmergencyCodeLength,
                    CrSceneGroupReturnRoute.Default))
            {
                _cardReaderSettings = cardReaderSettings;
                _returnExitRoute = returnExitRoute;
                _expectedEmergencyCodeHash = expectedEmergencyCodeHash;
            }

            public override void OnCodeTimedOut(ACrSceneContext crSceneContext)
            {
                _returnExitRoute.Follow(crSceneContext);
            }

            public override void OnCodeSpecified(
                ACrSceneContext crSceneContext,
                string codeData)
            {
                if (QuickHashes.GetCRC32String(codeData) == _expectedEmergencyCodeHash)
                {
                    CrSceneAdvanceRoute.Default.Follow(crSceneContext);

                    _cardReaderSettings.DoorEnvironmentAdapter
                        .OnAccessGranted(new AccessData());
                }
                else
                {
                    SaveEventAccessDenied();

                    new RejectedSceneGroup(CrSceneGroupReturnRoute.Default)
                        .EnterRoute
                        .Follow(crSceneContext);
                }

            }

            private void SaveEventAccessDenied()
            {
                var cardReaderDb = _cardReaderSettings.CardReaderDb;

                if (BlockedAlarmsManager.Singleton.ProcessEvent(
                    AlarmType.CardReader_InvalidEmergencyCode,
                    new IdAndObjectType(
                        cardReaderDb.IdCardReader,
                        ObjectType.CardReader)))
                {
                    Events.ProcessEvent(
                        new EventAccessDeniedInvalidEmergencyCode(
                            cardReaderDb.IdCardReader));
                }

                AlarmsManager.Singleton.AddAlarm(
                    new CrInvalidEmergencyCodeAlarm(
                        cardReaderDb.IdCardReader));
            }

            public override void OnSpecialKeyPressed(
                ACrSceneContext crSceneContext,
                CRSpecialKey specialKey)
            {
                if (specialKey == CRSpecialKey.No)
                {
                    _returnExitRoute.Follow(crSceneContext);
                    return;
                }

                base.OnSpecialKeyPressed(crSceneContext,
                    specialKey);
            }
        }

        public EmergencyCodeSceneGroup(
            ACardReaderSettings cardReaderSettings,
            [NotNull] IInstanceProvider<ACrSceneRoute> defaultRouteProvider)
            : this(
                cardReaderSettings,
                new DelayedInitReference<ICrScene>(),
                defaultRouteProvider)
        {
        }

        private EmergencyCodeSceneGroup(
            ACardReaderSettings cardReaderSettings,
            DelayedInitReference<ICrScene> sceneProvider,
            [NotNull] IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider)
            : base(
                sceneProvider,
                parentDefaultRouteProvider)
        {
            sceneProvider.Instance =
                new Scene(
                    cardReaderSettings,
                    new CrSceneGroupExitRoute(
                        this,
                        CrSceneGroupReturnRoute.Default),
                cardReaderSettings.CardReaderDb.EmergencyCode,
                cardReaderSettings.CardReaderDb.EmergencyCodeLength);
        }
    }
}
