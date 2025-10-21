using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.EventParameters;
using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal abstract class ASceneAuthorizationProcessBase : ACcuAuthorizationProcess
    {
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
    }

    internal abstract class ASceneAuthorizationProcess<TSceneGroup> :
        ASceneAuthorizationProcessBase
        where TSceneGroup : class, ICcuSceneGroup
    {
        protected readonly IInstanceProvider<TSceneGroup> SceneGroupProvider;

        protected ASceneAuthorizationProcess(
            IInstanceProvider<TSceneGroup> sceneGroupProvider)
        {
            SceneGroupProvider = sceneGroupProvider;
        }

        public override ACardReaderSettings CardReaderSettings
        {
            get
            {
                return SceneGroupProvider.Instance.CardReaderSettings;
            }
        }
    }

    internal interface ISceneGroupWithEntrySL : ICcuSceneGroup
    {
        DB.SecurityLevel EntrySecurityLevel
        {
            get;
        }

        string Gin
        {
            get;
        }
    }

    internal abstract class ASceneAuthorizationProcessByEntrySL<TEntrySceneGroup> :
        ASceneAuthorizationProcess<TEntrySceneGroup>
        where TEntrySceneGroup : class, ISceneGroupWithEntrySL
    {
        protected ASceneAuthorizationProcessByEntrySL(
            IInstanceProvider<TEntrySceneGroup> sceneGroupProvider)
            : base(sceneGroupProvider)
        {
        }

        protected override bool AuthorizationByCardEnabled
        {
            get
            {
                var entrySecurityLevel = SceneGroupProvider.Instance.EntrySecurityLevel;

                return entrySecurityLevel == DB.SecurityLevel.Card
                       || entrySecurityLevel == DB.SecurityLevel.CardPIN
                       || entrySecurityLevel == DB.SecurityLevel.CodeOrCard
                       || entrySecurityLevel == DB.SecurityLevel.CodeOrCardPin;
            }
        }

        protected override bool AuthorizationByCodeEnabled
        {
            get
            {
                var entrySecurityLevel = SceneGroupProvider.Instance.EntrySecurityLevel;

                return
                    entrySecurityLevel == DB.SecurityLevel.Code
                    || entrySecurityLevel == DB.SecurityLevel.CodeOrCard
                    || entrySecurityLevel == DB.SecurityLevel.CodeOrCardPin;
            }
        }

        protected override bool CardRequiresPin
        {
            get
            {
                var entrySecurityLevel = SceneGroupProvider.Instance.EntrySecurityLevel;

                return
                    entrySecurityLevel == DB.SecurityLevel.CardPIN
                    || entrySecurityLevel == DB.SecurityLevel.CodeOrCardPin;
            }
        }

        protected override string Gin
        {
            get { return SceneGroupProvider.Instance.Gin; }
        }
    }

}
