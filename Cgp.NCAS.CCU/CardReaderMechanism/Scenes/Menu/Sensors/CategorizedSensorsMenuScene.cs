using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;

using CrSceneFrameworkCF;
using CrSceneFrameworkCF.Scenes;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class CategorizedSensorsMenuScene<TCategorizedSensorsMenuSceneGroup, TMenuItemsProvider> :
        CrMenuScene
        where TCategorizedSensorsMenuSceneGroup :
            class,
            IAuthorizedSceneGroup,
            IInstanceProvider<TCategorizedSensorsMenuSceneGroup>
        where TMenuItemsProvider : 
            CategorizedSensorsMenuScene<
                TCategorizedSensorsMenuSceneGroup, 
                TMenuItemsProvider>.AMenuItemsProvider
    {
        public abstract class AMenuItemsProvider : 
            AAlarmAreasMenuItemsProviderBase<
                TMenuItemsProvider,
                CategorizedSensorsAccessManager.IEventHandler,
                TCategorizedSensorsMenuSceneGroup>,
            CategorizedSensorsAccessManager.IEventHandler
        {
            protected class AlarmAreaMenuItem
                : AlarmAreaMenuItemBase
            {
                protected AlarmAreaMenuItem(
                    AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                    IInstanceProvider<ACrSceneRoute, TMenuItemsProvider> routeProvider)
                    : base(alarmAreaAccessInfo,
                        routeProvider)
                {
                }

                protected override IEnumerable<CrIconSymbol> GetAdditionalInlinedIcons()
                {
                    var crAlarmAreaInfo = AccessInfo.CrAlarmAreaInfo;

                    if (crAlarmAreaInfo.IsInSabotage)
                    {
                        yield return CrIconSymbol.SensorsInSabotage;
                        yield break;
                    }

                    if (crAlarmAreaInfo.IsAnySensorInAlarm)
                    {
                        yield return 
                            crAlarmAreaInfo.IsAnySensorNotAcknowledged
                                ? CrIconSymbol.SensorsInAlarmNotAcknowledged
                                : CrIconSymbol.SensorsInAlarm;

                        yield break;
                    }

                    if (crAlarmAreaInfo.IsAnySensorNotAcknowledged)
                        yield return CrIconSymbol.SensorsNormalNotAcknowledged;
                }
            }

            protected AMenuItemsProvider(
                IInstanceProvider<TCategorizedSensorsMenuSceneGroup> sceneGroupProvider,
                IInstanceProvider<CrMenuScene> menuSceneProvider)
                : base(
                    sceneGroupProvider,
                    menuSceneProvider)
            {
            }

            public virtual void OnAnySensorInAlarmChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                UpdateAlarmArea(alarmAreaAccessInfo);
            }

            public virtual void OnAnySensorInTamperChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                UpdateAlarmArea(alarmAreaAccessInfo);
            }

            public virtual void OnAnySensorNotAcknowledgedChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                UpdateAlarmArea(alarmAreaAccessInfo);
            }

            public virtual void OnAnySensorPermanentlyBlockedChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                UpdateAlarmArea(alarmAreaAccessInfo);
            }

            public virtual void OnAnySensorTemporarilyBlockedChanged(
                AAlarmAreaAccessInfoBase alarmAreaAccessInfo,
                bool value)
            {
                UpdateAlarmArea(alarmAreaAccessInfo);
            }
        }

        public CategorizedSensorsMenuScene(
            TCategorizedSensorsMenuSceneGroup categorizedSensorsMenuSceneGroup,
            TMenuItemsProvider menuItemsProvider)
            : base(
                menuItemsProvider,
                MenuConfigurations.GetAvailableSensorsMenuConfiguration(categorizedSensorsMenuSceneGroup.CardReaderSettings.CardReader),
                categorizedSensorsMenuSceneGroup.DefaultGroupExitRoute,
                categorizedSensorsMenuSceneGroup.TimeOutGroupExitRoute)
        {
        }
    }
}
