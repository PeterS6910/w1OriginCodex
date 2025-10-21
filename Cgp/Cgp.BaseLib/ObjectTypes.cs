using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.Cgp.BaseLib
{
    [LwSerialization.LwSerialize()]
    public enum ObjectTypes
    {
        DailyPlan = 0,
        TimeZone = 1,
        Calendar = 2,
        CalendarDateSetting = 3,
        DayInterval = 4,
        TimeZoneDateSetting = 5,
        DayType = 6,
        Expression = 7,
        OutputDCU = 8,
        InputDCU = 9,
        ACLSetting = 10,
        ACLSettingAA = 11,
        ACLPerson = 12,
        AccessControlList = 13,
        Card = 14,
        CardSystem = 15,
        AlarmArea = 16,
        AACardReader = 17,
        AccessZone = 18,
        CCU = 19,
        AAInput = 20,
        NotSupport = 201
    }
}
