using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick;
using Contal.IwQuick.CrossPlatform.Data;
using Contal.IwQuick.CrossPlatform.Threads;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU.Alarms
{
    public sealed class CatAlarmsManager :
        ASingleton<CatAlarmsManager>,
        IDbObjectRemovalListener
    {
        private class SpecificAlarmArcsManager
        {
            private readonly SyncDictionary<AlarmType, ICollection<Guid>> _alarmArcIdsByAlarmType =
                new SyncDictionary<AlarmType, ICollection<Guid>>();

            public SpecificAlarmArcsManager(
                IEnumerable<AlarmTypeAndIdAlarmArc> alarmTypeAndIdAlarmArcs)
            {
                _alarmArcIdsByAlarmType.Clear();

                if (alarmTypeAndIdAlarmArcs == null)
                    return;

                foreach (var alarmTypeAndIdAlarmArc in alarmTypeAndIdAlarmArcs)
                {
                    ICollection<Guid> alarmArcIds;

                    if (!_alarmArcIdsByAlarmType.TryGetValue(
                        alarmTypeAndIdAlarmArc.AlarmType,
                        out alarmArcIds))
                    {
                        alarmArcIds = new LinkedList<Guid>();

                        _alarmArcIdsByAlarmType.Add(
                            alarmTypeAndIdAlarmArc.AlarmType,
                            alarmArcIds);
                    }

                    alarmArcIds.Add(alarmTypeAndIdAlarmArc.IdAlarmArc);
                }
            }

            public bool GetAlarmArcs(
                AlarmType alarmType,
                out ICollection<Guid> alarmArcs)
            {
                if (_alarmArcIdsByAlarmType.TryGetValue(
                    alarmType,
                    out alarmArcs))
                {
                    return true;
                }

                alarmArcs = null;
                return false;
            }
        }

        private readonly SyncDictionary<AlarmType, ICollection<Guid>> _alarmArcIdsByAlarmType =
            new SyncDictionary<AlarmType, ICollection<Guid>>();

        private readonly SyncDictionary<IdAndObjectType, SpecificAlarmArcsManager>
            _specificAlarmArcsManagersByAlarmObject =
                new SyncDictionary<IdAndObjectType, SpecificAlarmArcsManager>();

        private readonly ThreadPoolQueue<CatAlarmsCollection> _sendCatAlarmsQueue;

        private CatAlarmsManager()
            : base(null)
        {
            _sendCatAlarmsQueue = new ThreadPoolQueue<CatAlarmsCollection>(ThreadPoolGetter.Get());
        }

        public void ConfigureGeneralAlarmArcs(IEnumerable<AlarmTypeAndIdAlarmArc> alarmTypeAndIdAlarmArcs)
        {
            lock (_alarmArcIdsByAlarmType)
            {
                _alarmArcIdsByAlarmType.Clear();

                if (alarmTypeAndIdAlarmArcs == null)
                    return;

                foreach (var alarmTypeAndIdAlarmArc in alarmTypeAndIdAlarmArcs)
                {
                    ICollection<Guid> alarmArcIds;

                    if (!_alarmArcIdsByAlarmType.TryGetValue(
                        alarmTypeAndIdAlarmArc.AlarmType,
                        out alarmArcIds))
                    {
                        alarmArcIds = new LinkedList<Guid>();

                        _alarmArcIdsByAlarmType.Add(
                            alarmTypeAndIdAlarmArc.AlarmType,
                            alarmArcIds);
                    }

                    alarmArcIds.Add(alarmTypeAndIdAlarmArc.IdAlarmArc);
                }
            }
        }

        public void ConfigureSpecificAlarmArcs(
            IdAndObjectType alarmObject,
            IEnumerable<AlarmTypeAndIdAlarmArc> alarmTypeAndIdAlarmArcs)
        {
            if (alarmObject == null)
                return;
            
            _specificAlarmArcsManagersByAlarmObject[alarmObject] =
                new SpecificAlarmArcsManager(
                    alarmTypeAndIdAlarmArcs);
        }

        public ICollection<string> GetAlarmArcs(
            AlarmType alarmType,
            IdAndObjectType alarmObject)
        {
            if (alarmObject != null)
            {
                SpecificAlarmArcsManager specificAlarmArcsManager;

                if (_specificAlarmArcsManagersByAlarmObject.TryGetValue(
                    alarmObject,
                    out specificAlarmArcsManager))
                {
                    ICollection<Guid> specificAlarmArcs;

                    if (specificAlarmArcsManager.GetAlarmArcs(
                        alarmType,
                        out specificAlarmArcs))
                    {
                        return GetAlarmArcNames(specificAlarmArcs);
                    }
                }
            }

            ICollection<Guid> generalAlarmArcs;

            if (!_alarmArcIdsByAlarmType.TryGetValue(
                alarmType,
                out generalAlarmArcs))
            {
                return null;
            }

            return GetAlarmArcNames(generalAlarmArcs);
        }

        private ICollection<string> GetAlarmArcNames(IEnumerable<Guid> alarmArcIds)
        {
            if (alarmArcIds == null)
                return null;

            return new LinkedList<string>(
                alarmArcIds.Select(
                    alarmArcId =>
                        Database.ConfigObjectsEngine.GetFromDatabase(
                            ObjectType.AlarmArc,
                            alarmArcId))
                    .OfType<AlarmArc>()
                    .Select(
                        alarmArc =>
                            alarmArc.Name));
        }

        public void RemoveSpecificAlarmArcsConfiguration(
            IdAndObjectType alarmObject)
        {
            _specificAlarmArcsManagersByAlarmObject.Remove(alarmObject);
        }

        private void SendAlarm(
            Alarm alarm,
            bool alarmAcknowledged)
        {
            if (alarm.IsBlocked)
                return;

            var createCatAlarms = alarm as ICreateCatAlarms;

            if (createCatAlarms == null)
                return;

            SendCatAlarms(createCatAlarms.CreateCatAlarms(alarmAcknowledged));
        }

        internal IPAddress GetAlarmTransmitterIpAddress()
        {
            var ccu = Ccus.Singleton.GetCCU();

            if (ccu == null)
                return null;

            if (ccu.IdAlarmTransmitter == Guid.Empty)
                return null;

            var alarmTransmitter = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.AlarmTransmitter,
                ccu.IdAlarmTransmitter) as AlarmTransmitter;

            if (alarmTransmitter == null)
                return null;

            try
            {
                return IPAddress.Parse(alarmTransmitter.IpAddress);
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
                return null;
            }
        }

        public void SendCatAlarms(
            ICollection<CatAlarm> catAlarms)
        {
            _sendCatAlarmsQueue.Enqueue(new CatAlarmsCollection(this,catAlarms));
        }

        public void Init()
        {
            AlarmsManager.Singleton.AlarmAdded +=
                alarm =>
                    SendAlarm(
                        alarm,
                        false);

            AlarmsManager.Singleton.AlarmStopped +=
                alarm =>
                    SendAlarm(
                        alarm,
                        false);

            AlarmsManager.Singleton.AlarmAcknowledged +=
                (alarm, acknowledgeSources) =>
                    SendAlarm(
                        alarm,
                        true);

            CatComClient.Singleton.RegisterEventHandler(new CatComClientEventHandler());
        }

        #region Creatign cat alarms

        public static CatAlarm CreateSensorAlarm(
            Alarm alarm,
            bool isAlarm,
            Input input,
            DB.AlarmArea alarmArea,
            SensorPurpose sensorPurpose,
            bool isBlocked,
            IEnumerable<string> alarmArcs)
        {
            AlarmEventCode alarmEventCode;

            switch (sensorPurpose)
            {
                case SensorPurpose.FireAlarm:
                    alarmEventCode = isBlocked
                        ? AlarmEventCode.FireBlocking
                        :AlarmEventCode.FireAlarm;

                    break;

                case SensorPurpose.BurglaryAlarm:
                    alarmEventCode = isBlocked
                        ? AlarmEventCode.BurglarBlocking
                        : AlarmEventCode.BurglarAlarm;

                    break;

                case SensorPurpose.FreezeAlarm:
                    alarmEventCode = isBlocked
                        ? AlarmEventCode.FreezeBlocking
                        : AlarmEventCode.FreezeAlarm;

                    break;

                case SensorPurpose.GasAlarm:
                    alarmEventCode = isBlocked
                        ? AlarmEventCode.GasBlocking
                        : AlarmEventCode.GasAlarm;

                    break;

                case SensorPurpose.HeatAlarm:
                    alarmEventCode = isBlocked
                        ? AlarmEventCode.HeatBlocking
                        : AlarmEventCode.HeatAlarm;

                    break;

                case SensorPurpose.HoldUpAlarm:
                    alarmEventCode = isBlocked
                        ? AlarmEventCode.HoldupBlocking
                        : AlarmEventCode.HoldupAlarm;

                    break;

                case SensorPurpose.PanicAlarm:
                    alarmEventCode = isBlocked
                        ? AlarmEventCode.PanicBlocking
                        : AlarmEventCode.PanicAlarm;

                    break;

                case SensorPurpose.SprinklerAlarm:
                    alarmEventCode = isBlocked
                        ? AlarmEventCode.SprinklerBlocking
                        : AlarmEventCode.SprinklerAlarm;

                    break;

                default:
                    alarmEventCode = isBlocked
                        ? AlarmEventCode.WaterBlocking
                        : AlarmEventCode.WaterAlarm;

                    break;
            }

            var sensorId = AlarmAreas.Singleton.GetSensorId(
                alarmArea.IdAlarmArea,
                input.IdInput);

            var additionalInfo = new StringBuilder();

            additionalInfo.AppendFormat(
                "{0}, {1}{2} {3}, Unit {4}",
                alarmArea.ToString(),
                alarmArea.Id.ToString("D2"),
                sensorId.ToString("D2"),
                input.NickName,
                Ccus.Singleton.GetIndex().ToString("D3"));

            if (input.GuidDCU != Guid.Empty)
            {
                var inputDcu = Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DCU,
                    input.GuidDCU) as DCU;

                if (inputDcu != null)
                    additionalInfo.AppendFormat(
                        " {0}",
                        inputDcu.LogicalAddress.ToString("D2"));
            }

            additionalInfo.AppendFormat(
                " {0}",
                (input.InputNumber + 1).ToString("D2"));

            return new CatAlarm
            {
                AlarmEventCode = alarmEventCode,
                Section = string.Format(
                    "{0}{1}",
                    alarmArea.Id.ToString("D2"),
                    sensorId.ToString("D2")),
                TransmitterArea = alarmArea.Id.ToString("D2"),
                AlarmState = isAlarm,
                ArcList = alarmArcs,
                AdditionalInfo = additionalInfo.ToString(),
                Tag = alarm
            };
        }

        public static CatAlarm CreateSensorTamperAlarm(
            Alarm alarm,
            bool isAlarm,
            Input input,
            DB.AlarmArea alarmArea,
            IEnumerable<string> alarmArcs)
        {
            var sensorId = AlarmAreas.Singleton.GetSensorId(
                alarmArea.IdAlarmArea,
                input.IdInput);

            var additionalInfo = new StringBuilder();

            additionalInfo.AppendFormat(
                "{0}, {1}{2} {3}, Unit {4}",
                alarmArea.ToString(),
                alarmArea.Id.ToString("D2"),
                sensorId.ToString("D2"),
                input.NickName,
                Ccus.Singleton.GetIndex().ToString("D3"));

            if (input.GuidDCU != Guid.Empty)
            {
                var inputDcu = Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DCU,
                    input.GuidDCU) as DCU;

                if (inputDcu != null)
                    additionalInfo.AppendFormat(
                        " {0}",
                        inputDcu.LogicalAddress.ToString("D2"));
            }

            additionalInfo.AppendFormat(
                " {0}",
                (input.InputNumber + 1).ToString("D2"));

            return new CatAlarm
            {
                AlarmEventCode = AlarmEventCode.SensorSabotage,
                Section = string.Format(
                    "{0}{1}",
                    alarmArea.Id.ToString("D2"),
                    sensorId.ToString("D2")),
                TransmitterArea = alarmArea.Id.ToString("D2"),
                AlarmState = isAlarm,
                ArcList = alarmArcs,
                AdditionalInfo = additionalInfo.ToString(),
                Tag = alarm
            };
        }

        public static CatAlarm CreateAaSetByOnOffObjectFailedAlarm(
            Alarm alarm,
            bool isAlarm,
            DB.AlarmArea alarmArea,
            IEnumerable<string> alarmArcs)
        {
            var additionalInfo = new StringBuilder();

            additionalInfo.AppendFormat(
                "{0}, Unit {1}",
                alarmArea.ToString(),
                Ccus.Singleton.GetIndex().ToString("D3"));

            return new CatAlarm
            {
                AlarmEventCode = AlarmEventCode.ActivationFailed,
                Section = alarmArea.Id.ToString("D2"),
                TransmitterArea = alarmArea.Id.ToString("D2"),
                AlarmState = isAlarm,
                ArcList = alarmArcs,
                AdditionalInfo = additionalInfo.ToString(),
                Tag = alarm
            };
        }

        public static CatAlarm CreateCcuAlarm(
            AlarmEventCode alarmEventCode,
            Alarm alarm,
            AlarmState alarmState,
            AlarmType alarmType,
            IEnumerable<string> alarmArcs)
        {
            return new CatAlarm
            {
                AlarmEventCode = alarmEventCode,
                Section = "CCU#",
                AlarmState = alarmState == AlarmState.Alarm,
                ArcList = alarmArcs,
                AdditionalInfo = string.Format("Unit {0}",
                    Ccus.Singleton.GetIndex().ToString("D3")),
                Tag = alarm
            };
        }

        public static CatAlarm CreateDcuAlarm(
            AlarmEventCode alarmEventCode,
            Alarm alarm,
            DCU dcu,
            AlarmState alarmState,
            AlarmType alarmType,
            IEnumerable<string> alarmArcs)
        {
            return new CatAlarm
            {
                AlarmEventCode = alarmEventCode,
                Section = string.Format(
                    "CCU#/DCU{0}",
                    dcu.LogicalAddress),
                AlarmState = alarmState == AlarmState.Alarm,
                ArcList = alarmArcs,
                AdditionalInfo = string.Format("Unit {0} {1}",
                    Ccus.Singleton.GetIndex().ToString("D3"),
                    dcu.LogicalAddress.ToString("D2")),
                Tag = alarm
            };
        }

        public static CatAlarm CreateDeAlarm(
            AlarmEventCode alarmEventCode,
            Alarm alarm,
            DoorEnvironment doorEnvironment,
            AlarmState alarmState,
            AlarmType alarmType,
            IEnumerable<string> alarmArcs)
        {
            DCU dcu = null;

            if (doorEnvironment.GuidDCU != Guid.Empty)
            {
                dcu = Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DCU,
                    doorEnvironment.GuidDCU) as DCU;
            }

            var section = dcu != null
                ? string.Format(
                    "CCU#/DCU{0}",
                    dcu.LogicalAddress)
                : "CCU#";

            return new CatAlarm
            {
                AlarmEventCode = alarmEventCode,
                Section = section,
                AlarmState = alarmState == AlarmState.Alarm,
                ArcList = alarmArcs,
                AdditionalInfo = string.Format("Unit {0} {1}",
                    Ccus.Singleton.GetIndex().ToString("D3"),
                    dcu != null
                        ? dcu.LogicalAddress.ToString("D2")
                        : doorEnvironment.Number.ToString("D2")),
                Tag = alarm
            };
        }

        public static CatAlarm CreateCrAlarm(
            AlarmEventCode alarmEventCode,
            Alarm alarm,
            CardReader cardReader,
            AlarmState alarmState,
            AlarmType alarmType,
            IEnumerable<string> alarmArcs)
        {
            return CreateCrAlarm(
                alarmEventCode,
                alarm,
                cardReader,
                null,
                alarmState,
                alarmType,
                alarmArcs);
        }

        public static CatAlarm CreateCrAlarm(
            AlarmEventCode alarmEventCode,
            Alarm alarm,
            CardReader cardReader,
            string fullCardNumber,
            AlarmState alarmState,
            AlarmType alarmType,
            IEnumerable<string> alarmArcs)
        {
            var section = new StringBuilder();

            section.Append("CCU#");

            DCU dcu = null;

            if (cardReader.GuidDCU != Guid.Empty)
            {
                dcu = Database.ConfigObjectsEngine.GetFromDatabase(
                    ObjectType.DCU,
                    cardReader.GuidDCU) as DCU;

                if (dcu != null)
                {
                    section.AppendFormat(
                        "/DCU{0}",
                        dcu.LogicalAddress);
                }
            }

            section.AppendFormat(
                "/CR{0}",
                cardReader.Address);

            var additionalInfo = new StringBuilder();

            additionalInfo.Append(string.Format(
                "Unit {0}",
                Ccus.Singleton.GetIndex().ToString("D3")));

            if (dcu != null)
            {
                additionalInfo.Append(string.Format(
                    " {0}",
                    dcu.LogicalAddress.ToString("D2")));
            }

            additionalInfo.Append(string.Format(
                " {0}",
                cardReader.Address.ToString("D2")));

            if (!string.IsNullOrEmpty(fullCardNumber))
                additionalInfo.AppendFormat(
                    " {0}",
                    fullCardNumber);

            return new CatAlarm
            {
                AlarmEventCode = alarmEventCode,
                Section = section.ToString(),
                AlarmState = alarmState == AlarmState.Alarm,
                ArcList = alarmArcs,
                AdditionalInfo = additionalInfo.ToString(),
                Tag = alarm
            };
        }

        #endregion

        public void PrepareObjectDelete(
            Guid idObject,
            ObjectType objectType)
        {
            RemoveSpecificAlarmArcsConfiguration(
                new IdAndObjectType(
                    idObject,
                    objectType));
        }
    }
}
