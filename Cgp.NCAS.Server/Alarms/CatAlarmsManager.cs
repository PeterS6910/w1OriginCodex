using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

using Contal.CatCom;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.NCAS.Server.ServerAlarms;
using Contal.Cgp.Server.Alarms;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;
using Contal.IwQuick;
using Contal.IwQuick.Data;
using Contal.IwQuick.Sys;
using JetBrains.Annotations;

namespace Contal.Cgp.NCAS.Server.Alarms
{
    public sealed class CatAlarmsManager : ASingleton<CatAlarmsManager>
    {
        private class SpecificAlarmArcsManager
        {
            private readonly SyncDictionary<AlarmType, ICollection<Guid>> _alarmArcIdsByAlarmType =
                new SyncDictionary<AlarmType, ICollection<Guid>>();

            public SpecificAlarmArcsManager(
                [NotNull]
                ICollection<AlarmType> supportedAlarmTypes,
                IEnumerable<IAlarmArcForAlarmType> alarmArcForAlarmTypes)
            {
                _alarmArcIdsByAlarmType.Clear();

                if (alarmArcForAlarmTypes == null)
                    return;

                foreach (var alarmArcForAlarmType in alarmArcForAlarmTypes)
                {
                    var alarmType = (AlarmType) alarmArcForAlarmType.AlarmType;

                    if (!supportedAlarmTypes.Contains(alarmType))
                        continue;

                    ICollection<Guid> alarmArcIds;

                    if (!_alarmArcIdsByAlarmType.TryGetValue(
                        alarmType,
                        out alarmArcIds))
                    {
                        alarmArcIds = new LinkedList<Guid>();

                        _alarmArcIdsByAlarmType.Add(
                            alarmType,
                            alarmArcIds);
                    }

                    alarmArcIds.Add(alarmArcForAlarmType.IdAlarmArc);
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

        private class CatAlarmAndTransmitterIp
        {
            public CatAlarm CatAlarm
            {
                get;
                private set;
            }

            public string TransmitterIp
            {
                get;
                private set;
            }

            public CatAlarmAndTransmitterIp(
                CatAlarm catAlarm,
                string transmitterIp)
            {
                CatAlarm = catAlarm;
                TransmitterIp = transmitterIp;
            }
        }

        private delegate IEnumerable<CatAlarmAndTransmitterIp> DCreateCatAlarm(
            ServerAlarm serverAlarm,
            bool isAlarm,
            ICollection<string> alarmArcs);

        private readonly SyncDictionary<AlarmType, DCreateCatAlarm> _createCatAlarmByAlarmType;

        private readonly HashSet<AlarmType> _supportedAlarmTypes;

        private CatAlarmsManager()
            : base(null)
        {
            _createCatAlarmByAlarmType = new SyncDictionary<AlarmType, DCreateCatAlarm>
            {
                {
                    AlarmType.CCU_Offline,
                    CreateCatAlarmCcuOffline
                },
                {
                    AlarmType.CCU_Unconfigured,
                    CreateCatAlarmCcuUnconfigured
                },
                {
                    AlarmType.CCU_ClockUnsynchronized,
                    CreateCatAlarmCcuClockUnsynchronized
                },
                {
                    AlarmType.CCU_OutdatedFirmware,
                    CreateCatAlarmCcuOutdatedFirmware
                },
                {
                    AlarmType.CCU_DataChannelDistrupted,
                    CreateCatAlarmCcuDataChannelDistrupted
                },
                {
                    AlarmType.DCU_WaitingForUpgrade,
                    CreateCatAlarmDcuWaitingForUpgrade
                },
                {
                    AlarmType.DCU_OutdatedFirmware,
                    CreateCatAlarmDcuOutdatedFirmware
                },
                {
                    AlarmType.DCU_Offline_Due_CCU_Offline,
                    CreateCatAlarmDcuOfflineDueCcuOffline
                },
                {
                    AlarmType.CardReader_Offline_Due_CCU_Offline,
                    CreateCatAlarmCrOfflineDueCcuOffline
                }
            };

            _supportedAlarmTypes = new HashSet<AlarmType>(_createCatAlarmByAlarmType.Keys);
        }

        public void ConfigureGeneralAlarmArcs(IEnumerable<IAlarmArcForAlarmType> alarmArcsForAlarmTypes)
        {
            lock (_alarmArcIdsByAlarmType)
            {
                _alarmArcIdsByAlarmType.Clear();

                if (alarmArcsForAlarmTypes == null)
                    return;

                foreach (var alarmArcForAlarmType in alarmArcsForAlarmTypes)
                {
                    var alarmType = (AlarmType) alarmArcForAlarmType.AlarmType;

                    if (!_supportedAlarmTypes.Contains(alarmType))
                        continue;

                    ICollection<Guid> alarmArcIds;

                    if (!_alarmArcIdsByAlarmType.TryGetValue(
                        alarmType,
                        out alarmArcIds))
                    {
                        alarmArcIds = new LinkedList<Guid>();

                        _alarmArcIdsByAlarmType.Add(
                            alarmType,
                            alarmArcIds);
                    }

                    alarmArcIds.Add(alarmArcForAlarmType.IdAlarmArc);
                }
            }
        }

        public void ConfigureSpecificAlarmArcs(
            IdAndObjectType alarmObject,
            IEnumerable<IAlarmArcForAlarmType> alarmArcsForAlarmTypes)
        {
            if (alarmObject == null)
                return;

            _specificAlarmArcsManagersByAlarmObject[alarmObject] =
                new SpecificAlarmArcsManager(
                    _supportedAlarmTypes,
                    alarmArcsForAlarmTypes);
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

            return _alarmArcIdsByAlarmType.TryGetValue(
                    alarmType,
                    out generalAlarmArcs)
                ? GetAlarmArcNames(generalAlarmArcs)
                : null;
        }

        private static ICollection<string> GetAlarmArcNames(IEnumerable<Guid> alarmArcIds)
        {
            if (alarmArcIds == null)
                return null;

            return new LinkedList<string>(
                alarmArcIds.Select(
                    alarmArcId =>
                        AlarmArcs.Singleton.GetById(alarmArcId))
                    .Where(
                        alarmArc =>
                            alarmArc != null)
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
            ServerAlarm serverAlarm)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null
                || serverAlarm.ServerAlarmCore.Alarm == null)
            {
                return;
            }

            var alarmKey = serverAlarm.ServerAlarmCore.Alarm.AlarmKey;

            if (alarmKey == null)
                return;

            var alarmArcs = GetAlarmArcs(
                alarmKey.AlarmType,
                alarmKey.AlarmObject);

            if (alarmArcs == null)
                return;

            DCreateCatAlarm createCatAlarm;

            if (!_createCatAlarmByAlarmType.TryGetValue(
                alarmKey.AlarmType,
                out createCatAlarm))
            {
                return;
            }

           
            var catAlarms = createCatAlarm(
                serverAlarm,
                !serverAlarm.ServerAlarmCore.Alarm.IsBlocked
                && serverAlarm.ServerAlarmCore.Alarm.AlarmState == AlarmState.Alarm,
                alarmArcs);

            if (catAlarms == null)
            {
                return;
            }

            try
            {
                foreach (var catAlarmAndTransmitterIp in catAlarms)
                {
                    IPAddress ipAddress;

                    if (!IPAddress.TryParse(
                        catAlarmAndTransmitterIp.TransmitterIp,
                        out ipAddress))
                    {
                        continue;
                    }

                    CatComClient.Singleton.SendAlarm(
                        ipAddress,
                        catAlarmAndTransmitterIp.CatAlarm);
                }
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        public void Init()
        {
            var alarmsManager = NCASServer.Singleton.GetAlarmsQueue();

            if (alarmsManager != null)
            {
                alarmsManager.ServerAlarmsOwner.AlarmAdded += SendAlarm;
                alarmsManager.ServerAlarmsOwner.AlarmStopped += SendAlarm;
            }

            CatComClient.Singleton.RegisterEventHandler(new CatComClientEventHandler());
        }

        #region Creatign cat alarms

        private static IEnumerable<CatAlarmAndTransmitterIp> CreateCatAlarmCcuOffline(
            ServerAlarm serverAlarm,
            bool isAlarm,
            IEnumerable<string> alarmArcs)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null)
            {
                yield break;
            }

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm == null)
            {
                yield break;
            }

            var alarmKey = alarm.AlarmKey;

            if (alarmKey == null)
            {
                yield break;
            }

            var alarmObject = alarmKey.AlarmObject;

            if (alarmObject == null)
            {
                yield break;
            }

            var ccu = CCUs.Singleton.GetById(alarmObject.Id);

            if (ccu == null
                || ccu.AlarmTransmitter == null)
            {
                yield break;
            }

            yield return new CatAlarmAndTransmitterIp(
                new CatAlarm
                {
                    AlarmEventCode = AlarmEventCode.DeviceOffline,
                    Section = string.Format(
                        "CCU{0}",
                        ccu.IPAddress),
                    AlarmState = isAlarm,
                    ArcList = alarmArcs,
                    AdditionalInfo = string.Format(
                        "Unit {0}",
                        ccu.IndexCCU.ToString("D3")),
                    Tag = serverAlarm
                },
                ccu.AlarmTransmitter.IpAddress);
        }

        private static IEnumerable<CatAlarmAndTransmitterIp> CreateCatAlarmCcuUnconfigured(
            ServerAlarm serverAlarm,
            bool isAlarm,
            ICollection<string> alarmArcs)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null)
            {
                yield break;
            }

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm == null)
            {
                yield break;
            }

            var alarmKey = alarm.AlarmKey;

            if (alarmKey == null)
            {
                yield break;
            }

            var alarmObject = alarmKey.AlarmObject;

            if (alarmObject == null)
            {
                yield break;
            }

            var ccu = CCUs.Singleton.GetById(alarmObject.Id);

            if (ccu == null
                || ccu.AlarmTransmitter == null)
            {
                yield break;
            }

            yield return
                new CatAlarmAndTransmitterIp(
                    new CatAlarm
                    {
                        AlarmEventCode = AlarmEventCode.DeviceUnconfigured,
                        Section = string.Format(
                            "CCU{0}",
                            ccu.IPAddress),
                        AlarmState = isAlarm,
                        ArcList = alarmArcs,
                        AdditionalInfo = string.Format(
                            "Unit {0}",
                            ccu.IndexCCU.ToString("D3")),
                        Tag = serverAlarm
                    },
                    ccu.AlarmTransmitter.IpAddress);
        }

        private static IEnumerable<CatAlarmAndTransmitterIp> CreateCatAlarmCcuClockUnsynchronized(
            ServerAlarm serverAlarm,
            bool isAlarm,
            ICollection<string> alarmArcs)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null)
            {
                yield break;
            }

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm == null)
            {
                yield break;
            }

            var alarmKey = alarm.AlarmKey;

            if (alarmKey == null)
            {
                yield break;
            }

            var alarmObject = alarmKey.AlarmObject;

            if (alarmObject == null)
            {
                yield break;
            }

            var ccu = CCUs.Singleton.GetById(alarmObject.Id);

            if (ccu == null
                || ccu.AlarmTransmitter == null)
            {
                yield break;
            }

            yield return new CatAlarmAndTransmitterIp(
                new CatAlarm
                {
                    AlarmEventCode = AlarmEventCode.DeviceClockUnsynchronized,
                    Section = string.Format(
                        "CCU{0}",
                        ccu.IPAddress),
                    AlarmState = isAlarm,
                    ArcList = alarmArcs,
                    AdditionalInfo = string.Format(
                        "Unit {0}",
                        ccu.IndexCCU.ToString("D3")),
                    Tag = serverAlarm
                },
                ccu.AlarmTransmitter.IpAddress);
        }

        private static IEnumerable<CatAlarmAndTransmitterIp> CreateCatAlarmCcuOutdatedFirmware(
            ServerAlarm serverAlarm,
            bool isAlarm,
            ICollection<string> alarmArcs)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null)
            {
                yield break;
            }

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm == null)
            {
                yield break;
            }

            var alarmKey = alarm.AlarmKey;

            if (alarmKey == null)
            {
                yield break;
            }

            var alarmObject = alarmKey.AlarmObject;

            if (alarmObject == null)
            {
                yield break;
            }

            var ccu = CCUs.Singleton.GetById(alarmObject.Id);

            if (ccu == null
                || ccu.AlarmTransmitter == null)
            {
                yield break;
            }

            yield return
                new CatAlarmAndTransmitterIp(
                    new CatAlarm
                    {
                        AlarmEventCode = AlarmEventCode.DeviceOutdated,
                        Section = string.Format(
                            "CCU{0}",
                            ccu.IPAddress),
                        AlarmState = isAlarm,
                        ArcList = alarmArcs,
                        AdditionalInfo = string.Format(
                            "Unit {0}",
                            ccu.IndexCCU.ToString("D3")),
                        Tag = serverAlarm
                    },
                    ccu.AlarmTransmitter.IpAddress);
        }

        private static IEnumerable<CatAlarmAndTransmitterIp> CreateCatAlarmCcuDataChannelDistrupted(
            ServerAlarm serverAlarm,
            bool isAlarm,
            ICollection<string> alarmArcs)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null)
            {
                yield break;
            }

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm == null)
            {
                yield break;
            }

            var alarmKey = alarm.AlarmKey;

            if (alarmKey == null)
            {
                yield break;
            }

            var alarmObject = alarmKey.AlarmObject;

            if (alarmObject == null)
            {
                yield break;
            }

            var ccu = CCUs.Singleton.GetById(alarmObject.Id);

            if (ccu == null
                || ccu.AlarmTransmitter == null)
            {
                yield break;
            }

            yield return new CatAlarmAndTransmitterIp(
                new CatAlarm
                {
                    AlarmEventCode = AlarmEventCode.DeviceFailure,
                    Section = string.Format(
                        "CCU{0}",
                        ccu.IPAddress),
                    AlarmState = isAlarm,
                    ArcList = alarmArcs,
                    AdditionalInfo = string.Format(
                        "Unit {0}",
                        ccu.IndexCCU.ToString("D3")),
                    Tag = serverAlarm
                },
                ccu.AlarmTransmitter.IpAddress);
        }

        private static IEnumerable<CatAlarmAndTransmitterIp> CreateCatAlarmDcuWaitingForUpgrade(
            ServerAlarm serverAlarm,
            bool isAlarm,
            ICollection<string> alarmArcs)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null)
            {
                yield break;
            }

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm == null)
            {
                yield break;
            }

            var alarmKey = alarm.AlarmKey;

            if (alarmKey == null)
            {
                yield break;
            }

            var alarmObject = alarmKey.AlarmObject;

            if (alarmObject == null)
            {
                yield break;
            }

            var dcu = DCUs.Singleton.GetById(alarmObject.Id);

            if (dcu == null
                || dcu.CCU == null
                || dcu.CCU.AlarmTransmitter == null)
            {
                yield break;
            }

            yield return new CatAlarmAndTransmitterIp(
                new CatAlarm
                {
                    AlarmEventCode = AlarmEventCode.DeviceUnconfigured,
                    Section = string.Format(
                        "CCU{0}/DCU{1}",
                        dcu.CCU.IPAddress,
                        dcu.LogicalAddress),
                    AlarmState = isAlarm,
                    ArcList = alarmArcs,
                    AdditionalInfo = alarmKey.AlarmType.ToString(),
                    Tag = serverAlarm
                },
                dcu.CCU.AlarmTransmitter.IpAddress);
        }

        private static IEnumerable<CatAlarmAndTransmitterIp> CreateCatAlarmDcuOutdatedFirmware(
            ServerAlarm serverAlarm,
            bool isAlarm,
            ICollection<string> alarmArcs)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null)
            {
                yield break;
            }

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm == null)
            {
                yield break;
            }

            var alarmKey = alarm.AlarmKey;

            if (alarmKey == null)
            {
                yield break;
            }

            var alarmObject = alarmKey.AlarmObject;

            if (alarmObject == null)
            {
                yield break;
            }

            var dcu = DCUs.Singleton.GetById(alarmObject.Id);

            if (dcu == null
                || dcu.CCU == null
                || dcu.CCU.AlarmTransmitter == null)
            {
                yield break;
            }


            yield return new CatAlarmAndTransmitterIp(
                new CatAlarm
                {
                    AlarmEventCode = AlarmEventCode.DeviceOutdated,
                    Section = string.Format(
                        "CCU{0}/DCU{1}",
                        dcu.CCU.IPAddress,
                        dcu.LogicalAddress),
                    AlarmState = isAlarm,
                    ArcList = alarmArcs,
                    AdditionalInfo = alarmKey.AlarmType.ToString(),
                    Tag = serverAlarm
                },
                dcu.CCU.AlarmTransmitter.IpAddress);
        }

        private static IEnumerable<CatAlarmAndTransmitterIp> CreateCatAlarmDcuOfflineDueCcuOffline(
            ServerAlarm serverAlarm,
            bool isAlarm,
            ICollection<string> alarmArcs)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null)
            {
                yield break;
            }

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm == null)
            {
                yield break;
            }

            var alarmKey = alarm.AlarmKey;

            if (alarmKey == null)
            {
                yield break;
            }

            var alarmObject = alarmKey.AlarmObject;

            if (alarmObject == null)
            {
                yield break;
            }

            var dcu = DCUs.Singleton.GetById(alarmObject.Id);

            if (dcu == null
                || dcu.CCU == null
                || dcu.CCU.AlarmTransmitter == null)
            {
                yield break;
            }

            yield return new CatAlarmAndTransmitterIp(
                new CatAlarm
                {
                    AlarmEventCode = AlarmEventCode.SlaveDeviceOffline,
                    Section = string.Format(
                        "CCU{0}/DCU{1}",
                        dcu.CCU.IPAddress,
                        dcu.LogicalAddress),
                    AlarmState = isAlarm,
                    ArcList = alarmArcs,
                    AdditionalInfo = alarmKey.AlarmType.ToString(),
                    Tag = serverAlarm
                },
                dcu.CCU.AlarmTransmitter.IpAddress);
        }

        private static IEnumerable<CatAlarmAndTransmitterIp> CreateCatAlarmCrOfflineDueCcuOffline(
            ServerAlarm serverAlarm,
            bool isAlarm,
            ICollection<string> alarmArcs)
        {
            if (serverAlarm == null
                || serverAlarm.ServerAlarmCore == null)
            {
                yield break;
            }

            var alarm = serverAlarm.ServerAlarmCore.Alarm;

            if (alarm == null)
            {
                yield break;
            }

            var alarmKey = alarm.AlarmKey;

            if (alarmKey == null)
            {
                yield break;
            }

            var alarmObject = alarmKey.AlarmObject;

            if (alarmObject == null)
            {
                yield break;
            }

            var cardReader = CardReaders.Singleton.GetById(alarmObject.Id);

            if (cardReader == null)
            {
                yield break;
            }

            var ccu = cardReader.DCU != null
                ? cardReader.DCU.CCU
                : cardReader.CCU;

            if (ccu == null)
            {
                yield break;
            }

            yield return new CatAlarmAndTransmitterIp(
                new CatAlarm
                {
                    AlarmEventCode = AlarmEventCode.SlaveDeviceOffline,
                    Section = cardReader.DCU != null
                        ? string.Format(
                            "CCU{0}/DCU{1}/CR{2}",
                            ccu.IPAddress,
                            cardReader.DCU.LogicalAddress,
                            cardReader.Address)
                        : string.Format(
                            "CCU{0}/CR{1}",
                            ccu.IPAddress,
                            cardReader.Address),
                    AlarmState = isAlarm,
                    ArcList = alarmArcs,
                    AdditionalInfo = alarmKey.AlarmType.ToString(),
                    Tag = serverAlarm
                },
                ccu.AlarmTransmitter.IpAddress);
        }

        #endregion
    }

    public class CatComClientEventHandler : ICatComClientEventHandler
    {
        public void CatUnreachable(
            IPEndPoint remoteEndPoint,
            Exception error,
            CatAlarm alarm)
        {
            AlarmTransmitters.Singleton.ChangeOnlineState(
                remoteEndPoint.Address.ToString(),
                OnlineState.Offline);

            if (!DevicesAlarmSettings.Singleton.AlarmCcuCatUnreachable())
                return;

            var referencedAlarm = alarm != null
                ? alarm.Tag as ServerAlarm
                : null;

            CcuCatUnreachableServerAlarm.AddAlarm(
                remoteEndPoint.Address.ToString(),
                referencedAlarm);
        }

        public void CatReachable(IPEndPoint remoteEndPoint)
        {
            AlarmTransmitters.Singleton.ChangeOnlineState(
                remoteEndPoint.Address.ToString(),
                OnlineState.Online);

            CcuCatUnreachableServerAlarm.StopAlarm(remoteEndPoint.Address.ToString());
        }

        public void TransferToArcTimedOut(IPEndPoint remoteEndPoint, string arcName, CatAlarm alarm)
        {
            if (!DevicesAlarmSettings.Singleton.AlarmCcuTransferToArcTimedOut())
                return;

            var referencedAlarm = alarm.Tag as ServerAlarm;

            if (referencedAlarm == null)
                return;

            CcuTransferToArcTimedOutServerAlarm.AddAlarm(
                remoteEndPoint.Address.ToString(),
                arcName,
                referencedAlarm);
        }

        public void AllArcNamesReceived(IPEndPoint remoteEndPoint, IEnumerable<string> arcNames)
        {
            AlarmTransmitters.Singleton.AllArcNamesReceived(
                remoteEndPoint.Address.ToString(),
                arcNames);
        }

        public void ArcPresenceReceived(IPEndPoint remoteEndPoint, string arcName, bool arcExists)
        {

        }
    }
}
