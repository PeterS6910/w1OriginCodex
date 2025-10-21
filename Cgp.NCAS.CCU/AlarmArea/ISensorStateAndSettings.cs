using System;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.Globals;

namespace Contal.Cgp.NCAS.CCU.AlarmArea
{
    internal interface ISensorStateAndSettings
    {
        Guid IdInput
        {
            get;
        }

        bool IsInAlarm
        {
            get;
        }

        bool IsNotAcknowledged
        {
            get;
        }

        bool IsInTamper
        {
            get;
        }

        SensorBlockingType SensorBlockingType
        {
            get;
        }

        Guid IdAlarmArea
        {
            get;
        }

        int SensorId
        {
            get;
        }

        string NickName
        {
            get;
        }

        BlockTemporarilyUntilType DefaultBlockTemporarilyUntilType
        {
            get;
        }

        void EnqueueSetSensorBlockingTypeRequest(
            SensorBlockingType sensorBlockingType,
            Guid idCardReader,
            AccessDataBase accessData);

        void AcknowledgeAlarms(
            Guid idCardReader,
            AccessDataBase accessData);

        void BlockTemporarilyAndAcknowledge(
            Guid idCardReader,
            AccessDataBase accessData);

        void BlockPermanentlyAndAcknowledge(
            Guid idCardReader,
            AccessDataBase accessData);
    }
}