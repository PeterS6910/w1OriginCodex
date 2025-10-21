using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.CCU.AlarmArea;
using Contal.Cgp.NCAS.CCU.EventParameters;
using Contal.Cgp.NCAS.Globals;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes.Menu.AlarmAreas
{
    internal class UnsetAllAlarmAreasProcess
    {
        private readonly AccessDataBase _accessData;

        private readonly Guid _idCardReader;

        private readonly ICollection<CrAlarmAreasManager.CrAlarmAreaInfo> _alarmAreasToUnset;

        private readonly ICollection<Guid> _idAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod =
            new HashSet<Guid>();

        public UnsetAllAlarmAreasProcess(
            AccessDataBase accessData,
            Guid idCardReader,
            IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> alarmAreaInfos)
        {
            _accessData = accessData;
            _idCardReader = idCardReader;
            _alarmAreasToUnset = new List<CrAlarmAreasManager.CrAlarmAreaInfo>(alarmAreaInfos) ;

            TimeBuingRequired = _alarmAreasToUnset.Any(
                aaInf => aaInf.IsTimeBuyingPossible
                         && AlarmArea.AlarmAreas.Singleton.IsTimeBuyingEnabled(aaInf.IdAlarmArea));
        }

        public AccessDataBase AccessData
        {
            get
            {
                return _accessData;
            }
        }

        public int CountAlarmAreasToUnset
        {
            get { return _alarmAreasToUnset.Count; }
        }

        public int CountUnsetAlarmAreas { get; private set; }

        public int TimeToBuy { private get; set; }

        public IEnumerable<CrAlarmAreasManager.CrAlarmAreaInfo> AlarmAreaInfos
        {
            get { return _alarmAreasToUnset; }
        }

        public bool TimeBuingRequired { get; private set; }

        public ICollection<Guid> IdAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod
        {
            get { return _idAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod; }
        }

        private class UnsetAlarmAreasResult : AlarmAreaStateAndSettings.IAlarmAreaUnsetResult
        {
            private readonly UnsetAllAlarmAreasProcess _unsetAlarmAreasProcess;
            private readonly Guid _idAlarmArea;

            public UnsetAlarmAreasResult(
                UnsetAllAlarmAreasProcess unsetAlarmAreasProcess, 
                Guid idAlarmArea)
            {
                _unsetAlarmAreasProcess = unsetAlarmAreasProcess;
                _idAlarmArea = idAlarmArea;
            }

            public void OnFailed(AlarmAreaActionResult alarmAreaActionResult, int timeToBuy, int remainingTime)
            {
            }

            public void OnSucceded(
                int timeToBuy,
                int remainingTime,
                bool nonAcknowledgedAlarmDuringSetPeriod)
            {
                ++_unsetAlarmAreasProcess.CountUnsetAlarmAreas;

                Events.ProcessEvent(
                    new EventUnsetAlarmAreaFromCardReader(
                        _unsetAlarmAreasProcess._idCardReader,
                        _unsetAlarmAreasProcess.AccessData,
                        _idAlarmArea));

                if (nonAcknowledgedAlarmDuringSetPeriod)
                    _unsetAlarmAreasProcess._idAlarmAreasWithNonAcknowledgedAlarmDuringSetPeriod.Add(_idAlarmArea);
            }
        }

        public void Execute()
        {
            var setUnsetParams =
                new AlarmArea.AlarmAreas.SetUnsetParams(
                    _idCardReader,
                    AccessData,
                    Guid.Empty,
                    false,
                    false);

            foreach (var alarmArea in _alarmAreasToUnset)
            {
                var idAlarmArea = alarmArea.IdAlarmArea;

                if (TimeToBuy == 0
                    || !alarmArea.IsTimeBuyingPossible
                    || !AlarmArea.AlarmAreas.Singleton.IsTimeBuyingEnabled(idAlarmArea))
                {
                    TryUnsetAlarmAreas(
                        idAlarmArea,
                        AccessData.IdPerson,
                        setUnsetParams);
                }
                else
                {
                    TryTimeBuying(
                        idAlarmArea,
                        AccessData,
                        setUnsetParams);
                }
            }
        }

        private void TryUnsetAlarmAreas(
            Guid idAlarmArea,
            Guid idPerson, 
            AlarmArea.AlarmAreas.SetUnsetParams setUnsetParams)
        {
            if (idPerson == Guid.Empty
                || AlarmArea.AlarmAreas.Singleton.CheckUnsetRights(idAlarmArea, idPerson))
            {
                AlarmArea.AlarmAreas.Singleton.UnsetAlarmArea(
                    new UnsetAlarmAreasResult(
                        this,
                        idAlarmArea),
                    true,
                    idAlarmArea,
                    Guid.Empty,
                    idPerson,
                    0,
                    setUnsetParams);
            }
        }

        private void TryTimeBuying(
            Guid idAlarmArea,
            AccessDataBase accessData,
            AlarmArea.AlarmAreas.SetUnsetParams setUnsetParams)
        {
            if (!AlarmAreaAccessRightsManager.Singleton.CheckRigthsToTimeBuying(
                accessData,
                idAlarmArea))
                return;

            AlarmArea.AlarmAreas.Singleton.UnsetAlarmArea(
                new UnsetAlarmAreasResult(
                    this,
                    idAlarmArea),
                true,
                idAlarmArea,
                Guid.Empty,
                accessData.IdPerson,
                TimeToBuy,
                setUnsetParams);
        }
    }
}
