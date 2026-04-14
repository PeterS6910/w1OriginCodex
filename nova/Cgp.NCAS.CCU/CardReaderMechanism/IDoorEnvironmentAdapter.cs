using System;
using System.Collections.Generic;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Access;
using Contal.Drivers.CardReader;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism
{
    internal interface IDoorEnvironmentAdapter
    {
        bool IsAssociatedWithDoorEnvironment(Guid idDoorEnvironment);
        bool IsAssociatedWithMultiDoor(Guid idMultiDoor);

        bool IsCrStartScheduled
        {
            get;
        }

        bool IsCardReaderSuppressed
        {
            get;
        }

        void OnAccessDenied(
            Guid guidCard,
            string cardNumber,
            string message);

        void OnAccessGranted(
            [NotNull]
            AccessDataBase accessData);

        void ClearAccessGrantedVariables();

        void AttachEvents();
        void DetachEvents();

        void LooseCardReaderIfSuppressed();
        void ForceLooseCardReader();

        void SetImplicitCrCode(
            CRMessage implicitCrMessage,
            IList<CRMessage> followingMessages,
            bool intrusionOnlyViaLed);

        void SuppressCardReader();

        AAccessAuthorizationProcess CreateAccessAuthorizationProcess();

        void OnCrOnlineStateChanged(bool isOnline);

        bool HasAccess(AccessDataBase accessData, Guid idCardReader);
        ICollection<Guid> HasAccessMultiDoor(AccessDataBase accessData);
    }
}