using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.EventParameters;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Access
{
    internal abstract class AAccessAuthorizationProcess : ACcuAuthorizationProcess
    {
        private DB.SecurityLevel SecurityLevel
        {
            get
            {
                var implicitCrCodeParams =
                    CardReaderSettings.CurrentImplicitCrCodeParams;

                return 
                    implicitCrCodeParams != null
                        ? implicitCrCodeParams.SecurityLevel
                        : DB.SecurityLevel.Locked;
            }
        }

        protected sealed override bool AuthorizationByCodeEnabled
        {
            get
            {
                switch (SecurityLevel)
                {
                    case DB.SecurityLevel.Code:
                    case DB.SecurityLevel.CodeOrCard:
                    case DB.SecurityLevel.CodeOrCardPin:

                        return true;
                }

                return false;
            }
        }

        protected sealed override bool AuthorizationByCardEnabled
        {
            get
            {
                switch (SecurityLevel)
                {
                    case DB.SecurityLevel.Card:
                    case DB.SecurityLevel.CardPIN:
                    case DB.SecurityLevel.CodeOrCard:
                    case DB.SecurityLevel.CodeOrCardPin:

                        return true;
                }

                return false;
            }
        }

        protected sealed override bool CardRequiresPin
        {
            get
            {
                switch (SecurityLevel)
                {
                    case DB.SecurityLevel.CardPIN:
                    case DB.SecurityLevel.CodeOrCardPin:
                        return true;
                }

                return false;
            }
        }

        protected override void OnAccessDeniedNoRightsForCard()
        {
            var idCardReader = CardReaderSettings.Id;

            if (BlockedAlarmsManager.Singleton.ProcessEvent(
                AlarmType.CardReader_AccessDenied,
                new IdAndObjectType(
                    idCardReader,
                    ObjectType.CardReader)))
            {
                Events.ProcessEvent(
                    new EventAccessDenied(
                        idCardReader,
                        AccessData));
            }

            AlarmsManager.Singleton.AddAlarm(
                new CrAccessDeniedAlarm(
                    idCardReader,
                    AccessData));
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
                CardReaderSettings.DoorEnvironmentAdapter.OnAccessDenied(
                    AccessData.IdCard,
                    string.Empty,
                    "wrong pin");

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

        protected override void OnAccessDeniedInvalidCode()
        {
            var idCardReader = CardReaderSettings.Id;

            if (BlockedAlarmsManager.Singleton.ProcessEvent(
                AlarmType.CardReader_InvalidCode,
                new IdAndObjectType(
                    idCardReader,
                    ObjectType.CardReader)))
            {
                CardReaderSettings.DoorEnvironmentAdapter.OnAccessDenied(
                    Guid.Empty,
                    string.Empty,
                    "wrong code");

                Events.ProcessEvent(
                    new EventAccessDeniedInvalidCode(idCardReader));
            }

            AlarmsManager.Singleton.AddAlarm(
                new CrInvalidCodeAlarm(idCardReader));
        }

        protected override void OnAccessDeniedNoRightsForPerson()
        {
            var idCardReader = CardReaderSettings.Id;

            if (BlockedAlarmsManager.Singleton.ProcessEvent(
                AlarmType.CardReader_AccessDenied,
                new IdAndObjectType(
                    idCardReader,
                    ObjectType.CardReader)))
            {
                Events.ProcessEvent(
                    new EventAccessDenied(
                        idCardReader,
                        AccessData));
            }

            AlarmsManager.Singleton.AddAlarm(
                new CrAccessDeniedAlarm(
                    idCardReader,
                    AccessData));
        }

        protected override string Gin
        {
            get { return CardReaderSettings.CardReaderDb.GIN; }
        }
    }
}