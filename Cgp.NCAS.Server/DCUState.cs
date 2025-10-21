using System;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.DB;
using Contal.Cgp.Server;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.NCAS.Server
{
    public class DCUState
    {
        private Guid _dcuId;
        private DateTime? _dateTimeLastDCUOnlineStateChanged;
        private OnlineState _onlineState;
        private DateTime? _dateTimeInputsSabotageStateChanged;
        private State _inputsSabotageState = State.Unknown;
        private DateTime? _dateTimeLastPhysicalAddressChanged;
        private string _physicalAddress;
        private string _firmwareVersion;

        public Guid DCUId { get { return _dcuId; } }
        public OnlineState OnlineState { get { return _onlineState; } }
        public State InputsSabotageState { get { return _inputsSabotageState; } }
        public string PhysicalAddress { get { return _physicalAddress; } }
        public string FirmwareVersion { get { return _firmwareVersion; } set { _firmwareVersion = value; } }

        public DCUState(DCU dcu, DateTime? dateTime, OnlineState onlineState)
        {
            if (dcu != null)
            {
                _dcuId = dcu.IdDCU;
                _dateTimeLastDCUOnlineStateChanged = dateTime;
                _onlineState = onlineState;

                ChangeSettingsForAlarmDCUOffline(dcu, false);
            }
        }

        public void SetNewID(Guid id)
        {
            _dcuId = id;
        }

        public bool SetOnlineState(DateTime? dateTime, OnlineState onlineState)
        {
            if (dateTime == null || _dateTimeLastDCUOnlineStateChanged == null || dateTime >= _dateTimeLastDCUOnlineStateChanged)
            {
                _dateTimeLastDCUOnlineStateChanged = dateTime;
                _onlineState = onlineState;

                return true;
            }

            return false;
        }

        public bool SetInputsSabotageState(DateTime? dateTime, State inputsSabotageState)
        {
            if (dateTime == null || _dateTimeInputsSabotageStateChanged == null || dateTime >= _dateTimeInputsSabotageStateChanged)
            {
                _dateTimeInputsSabotageStateChanged = dateTime;
                _inputsSabotageState = inputsSabotageState;

                return true;
            }

            return false;
        }

        public bool SetPhysicalAddress(DateTime? dateTime, string physicalAddress)
        {
            if (dateTime == null || _dateTimeLastPhysicalAddressChanged == null || dateTime >= _dateTimeLastPhysicalAddressChanged)
            {
                _dateTimeLastPhysicalAddressChanged = dateTime;
                _physicalAddress = physicalAddress;

                return true;
            }

            return false;
        }

        private bool? _enabledAlarmDCUOffline = null;
        private bool? _blockAlarmDCUOffline = null;
        private byte? _blockAlarmDCUOfflineObjectType = null;
        private Guid? _blockAlarmDCUOfflineObjectId = null;
        public void ChangeSettingsForAlarmDCUOffline(DCU dcu, bool updateAlarms)
        {
            bool? actEnabledkAlarmDCUOffline = dcu.AlarmOffline;
            bool? actBlockAlarmDCUOffline = dcu.BlockAlarmOffline;
            byte? actBlockAlarmDCUOfflineObjectType = dcu.ObjBlockAlarmOfflineObjectType;
            Guid? actBlockAlarmDCUOfflineObjectId = dcu.ObjBlockAlarmOfflineId;

            if (_enabledAlarmDCUOffline != actEnabledkAlarmDCUOffline ||
                _blockAlarmDCUOffline != actBlockAlarmDCUOffline ||
                _blockAlarmDCUOfflineObjectType != actBlockAlarmDCUOfflineObjectType ||
                _blockAlarmDCUOfflineObjectId != actBlockAlarmDCUOfflineObjectId)
            {
                _enabledAlarmDCUOffline = actEnabledkAlarmDCUOffline;
                _blockAlarmDCUOffline = actBlockAlarmDCUOffline;
                _blockAlarmDCUOfflineObjectType = actBlockAlarmDCUOfflineObjectType;
                _blockAlarmDCUOfflineObjectId = actBlockAlarmDCUOfflineObjectId;
            }
            else
            {
                return;
            }


            if (updateAlarms)
            {
                ChangeAlarmDcuOfflineDueCcuOffline(null, null);
            }
        }

        public void ChangeAlarmDcuOfflineDueCcuOffline(ObjectType? objectType, Guid? objectGuid)
        {
            if (objectType != null && objectGuid != null)
            {
                if (_blockAlarmDCUOffline == null)
                {
                    if (
                        !CCUAlarms.Singleton.IsRelevantObjectForBlockingAlarmDCUOffline(objectType.Value,
                            objectGuid.Value))
                    {
                        return;
                    }
                }
                else
                {
                    if (!_blockAlarmDCUOffline.Value ||
                        (byte?) objectType != _blockAlarmDCUOfflineObjectType ||
                        objectGuid != _blockAlarmDCUOfflineObjectId)
                    {
                        return;
                    }
                }
            }

            DCU dcu = DCUs.Singleton.GetById(_dcuId);
            if (dcu == null
                || dcu.CCU == null)
            {
                return;
            }

            CCUAlarms.UpdateAlarmDcuOfflineDueCcuOffline(
                dcu,
                CCUConfigurationHandler.Singleton.GetCCUState(dcu.CCU.IdCCU));
        }

        public bool IsBlockedAlarmDCUOffline()
        {
            if (_blockAlarmDCUOffline == null)
            {
                return CCUAlarms.Singleton.IsBlockedAlarmDCUOffline();
            }
            else
            {
                if (_blockAlarmDCUOffline.Value && _blockAlarmDCUOfflineObjectType != null &&
                    _blockAlarmDCUOfflineObjectId != null)
                {
                    switch (_blockAlarmDCUOfflineObjectType.Value)
                    {
                        case (byte)ObjectType.DailyPlan:
                            if (TimeAxis.Singleton.GetActualStatusDP(_blockAlarmDCUOfflineObjectId.Value) == TimeAxis.ON)
                            {
                                return true;
                            }
                            break;
                        case (byte)ObjectType.TimeZone:
                            if (TimeAxis.Singleton.GetActualStatusTZ(_blockAlarmDCUOfflineObjectId.Value) == TimeAxis.ON)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }

            return false;
        }
    }
}