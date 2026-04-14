using System;

using CrSceneFrameworkCF;

using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common
{
    internal abstract class ASceneAuthorizationSceneGroupByEntrySL<TEntrySceneGroup> : 
        ASceneAuthorizationSceneGroup<TEntrySceneGroup>,
        ISceneGroupWithEntrySL
        where TEntrySceneGroup : ASceneAuthorizationSceneGroupByEntrySL<TEntrySceneGroup>
    {
        protected ASceneAuthorizationSceneGroupByEntrySL(
            [NotNull]
            ASceneAuthorizationProcessByEntrySL<TEntrySceneGroup> sceneAuthorizationProcess,
            [NotNull] 
            IInstanceProvider<ACrSceneRoute> parentDefaultRouteProvider,
            [NotNull]
            ACardReaderSettings cardReaderSettings)
            : base(
                sceneAuthorizationProcess,
                parentDefaultRouteProvider,
                cardReaderSettings)
        {
            var cardReaderDb = cardReaderSettings.CardReaderDb;

            var securityLevel = 
                (DB.SecurityLevel?)
                    cardReaderDb.SLForEnterToMenu;

            Guid guidStz;
            Guid guidSdp;

            if (securityLevel != null)
            {
                EntrySecurityLevel = securityLevel.Value;

                if (cardReaderDb.UseAccessGinForEnterToMenu)
                {
                    Gin = cardReaderDb.GIN;
                }
                else
                {
                    Gin = cardReaderDb.GinForEnterToMenu;
                }

                guidStz = cardReaderDb.GuidSecurityTimeZoneForEnterToMenu;
                guidSdp = cardReaderDb.GuidSecurityDailyPlanForEnterToMenu;
            }
            else
            {
                EntrySecurityLevel = DevicesAlarmSettings.Singleton.SecurityLevelForEnterToMenu;
                Gin = DevicesAlarmSettings.Singleton.GinForEnterToMenu;

                guidStz = DevicesAlarmSettings.Singleton.GuidSecurityTimeZoneForEnterToMenu;
                guidSdp = DevicesAlarmSettings.Singleton.GuidSecurityDailyPlanForEnterToMenu;
            }

            if (EntrySecurityLevel == DB.SecurityLevel.SecurityTimeZoneSecurityDailyPlan)
                EntrySecurityLevel =
                    ACardReaderSettings.GetSecurityLevelFromStzOrSdpState(
                        guidStz != Guid.Empty
                            ? SecurityTimeZones.Singleton.GetActualState(guidStz)
                            : SecurityDailyPlans.Singleton.GetActualState(guidSdp));
        }

        public DB.SecurityLevel EntrySecurityLevel
        {
            get;
            private set;
        }

        public string Gin
        {
            get;
            private set;
        }
    }
}