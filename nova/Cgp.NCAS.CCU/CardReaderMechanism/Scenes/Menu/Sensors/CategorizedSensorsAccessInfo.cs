using System;
using System.Collections.Generic;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Common;
using Contal.Cgp.NCAS.CCU.DB;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.Sensors
{
    internal class CategorizedSensorsAccessInfo : 
        AAlarmAreaAccessInfoBase
    {
        public CategorizedSensorsAccessInfo(
            CrAlarmAreasManager.CrAlarmAreaInfo crAlarmAreaInfo,
            IAlarmAreaAccessManager alarmAreaAccessManager)
            : base(
                crAlarmAreaInfo,
                alarmAreaAccessManager)
        {
        }

        protected override bool CheckPersonAccessRights(IAlarmAreaAccessManager accessManager)
        {
            return accessManager.CheckRigthsToSensorHandling(CrAlarmAreaInfo.IdAlarmArea);
        }
    }
}