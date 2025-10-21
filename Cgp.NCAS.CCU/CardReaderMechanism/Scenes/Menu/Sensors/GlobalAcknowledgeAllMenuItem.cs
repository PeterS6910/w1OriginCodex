using System;
using System.Collections.Generic;

using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;

using CrSceneFrameworkCF;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class GlobalAcknowledgeAllMenuItemBase<TMenuItemsProvider> : 
        ALocalizedMenuItem<TMenuItemsProvider>
        where TMenuItemsProvider : IAlarmAreasMenuItemsProvider
    {
        private class RouteProvider : IInstanceProvider<ACrSceneRoute, TMenuItemsProvider>
        {
            public ACrSceneRoute GetInstance(TMenuItemsProvider menuItemsProvider)
            {
                AcknowledgeAllSensors(menuItemsProvider);
                return CrSceneGroupReturnRoute.Default;
            }

            private static void AcknowledgeAllSensors(TMenuItemsProvider menuItemsProvider)
            {
                var idCardReader = menuItemsProvider.CardReaderSettings.Id;

                var sceneGroup = menuItemsProvider.SceneGroup;

                foreach (var alarmAreaInfo in menuItemsProvider.AlarmAreaInfos)
                    alarmAreaInfo.AcknowledgeAllSensorAlarms(
                        idCardReader,
                        sceneGroup.AccessData);
            }
        }

        protected GlobalAcknowledgeAllMenuItemBase()
            : base(new RouteProvider())
        {
        }

        protected override string GetLocalizationKey(TMenuItemsProvider menuItemsProvider)
        {
            return CardReaderConstants.MENUSENSORACKNOWLEDGEALLALARMAREAS;
        }

        protected override IEnumerable<CrIconSymbol> GetInlinedIcons(TMenuItemsProvider menuItemsProvider)
        {
            yield return CrIconSymbol.AcknowledgeAll;
        }
    }
}
