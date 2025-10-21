using System;
using System.Collections.Generic;

using Contal.Cgp.BaseLib;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Globals;

namespace Contal.Cgp.NCAS.RemotingCommon
{
    public interface IAlarmAreas : IBaseOrmTable<AlarmArea>
    {
        AlarmAreaActionResult SetAlarmArea(Guid alarmAreaGuid, bool noPrewarning);
        AlarmAreaActionResult UnsetAlarmArea(Guid alarmAreaGuid, int timeToBuy);
        AlarmAreaActionResult UnconditionalSetAlarmArea(Guid alarmAreaGuid, bool noPrewarning);
        ActivationState GetAlarmAreaActivationState(Guid alarmAreaGuid);
        RequestActivationState GetAlarmAreaRequestActivationState(Guid alarmAreaGuid);
        AlarmAreaAlarmState GetAlarmAreaAlarmState(Guid alarmAreaGuid);
        State GetAlarmAreaSabotageState(Guid alarmAreaGuid);
        CCU GetImplicitCCUForAlarmArea(Guid guidAlarmArea);
        IList<Output> GetSpecialOutputs(Guid alarmAreaGuid);

        ICollection<AlarmAreaShort> ShortSelectByCriteria(IList<FilterSettings> filterSettings, out Exception error);

        ICollection<IModifyObject> ListModifyObjects(out Exception error);
        ICollection<IModifyObject> ListModifyObjects(Guid? ccuId, out Exception error);

        bool HasAccessForAlarmAreaActionFromCurrentLogin(AccessNCAS action, Guid alarmAreaGuid);

        bool SetCardReaderToAlarmArea(Guid guidCardReader, Guid guidImplicitCCU);
        bool SetSensorToAlarmArea(Guid guidInput, Guid guidImplicitCCU);

        bool IsOutputUsedAsSiren(Guid outputGuid);

        bool ShowExtendedTimeBuying { get; }
        int GetNewId();

        State? GetSensorState(Guid idAlarmArea, Guid idInput);
        
        SensorBlockingType? GetSensorBlockingType(Guid idAlarmArea, Guid idInput);
        void SetAlarmAreaSensorBlockingType(Guid idAlarmArea, Guid idInput, SensorBlockingType sensorBlockingType);
        AlarmArea GetAlarmAreaBySectionId(int sectionId);
        bool ReplaceAlarmAreasSectionId(AlarmArea existingAlarmArea, AlarmArea editingAlarmArea);
        TimeBuyingMatrixState? GetTimeBuyingMatrixState(Guid idAlarmArea);
    }
}
