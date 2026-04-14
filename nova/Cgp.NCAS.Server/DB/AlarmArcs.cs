using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.Server.Beans.ModifyObjects;
using Contal.Cgp.NCAS.Server.Beans.Shorts;
using Contal.Cgp.NCAS.Server.CcuDataReplicator;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Server.DB;

namespace Contal.Cgp.NCAS.Server.DB
{
    public sealed class AlarmArcs :
        ANcasBaseOrmTable<AlarmArcs, AlarmArc>,
        IAlarmArcs
    {
        private AlarmArcs()
            : base(
                  null,
                  new CudPreparationForObjectWithVersion<AlarmArc>())
        {
        }

        public override bool HasAccessView(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.AlarmArcs),
                login);
        }

        public override bool HasAccessInsert(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AlarmArcsInsertDeletePerform),
                login);
        }

        public override bool HasAccessUpdate(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccessesForGroup(AccessNcasGroups.AlarmArcs),
                login);
        }

        public override bool HasAccessDelete(Login login)
        {
            return AccessChecker.HasAccessControl(
                NCASAccess.GetAccess(AccessNCAS.AlarmArcsInsertDeletePerform),
                login);
        }

        public ICollection<AlarmArcShort> ShortSelectByCriteria(
            IList<FilterSettings> filterSettings,
            out Exception error)
        {
            var list = SelectByCriteria(filterSettings, out error);

            if (list != null)
            {
                return
                    new LinkedList<AlarmArcShort>(
                        list.Select(alarmArc => new AlarmArcShort(alarmArc)));
            }

            return null;
        }

        public ICollection<IModifyObject> ListModifyObjects(out Exception error)
        {
            var alarmArcs = List(out error);

            if (alarmArcs == null)
                return null;

            return
                new LinkedList<IModifyObject>(
                    alarmArcs.Select(alarmArc => new AlarmArcModifyObj(alarmArc))
                        .OrderBy(alarmArcModifyObj => alarmArcModifyObj.ToString())
                        .Cast<IModifyObject>());
        }

        #region SearchFunctions

        public override ICollection<AOrmObject> GetSearchList(string name, bool single)
        {
            if (single && string.IsNullOrEmpty(name))
            {
                var alarmArcs = List();

                return alarmArcs != null
                    ? new LinkedList<AOrmObject>(
                        alarmArcs.OrderBy(alarmArc => alarmArc.Name).Cast<AOrmObject>())
                    : null;
            }

            if (string.IsNullOrEmpty(name))
                return null;

            var linqResult =
                single
                    ? SelectLinq<AlarmArc>(alarmArc => alarmArc.Name.IndexOf(name) >= 0)
                    : SelectLinq<AlarmArc>(
                        alarmArc =>
                            alarmArc.Name.IndexOf(name) >= 0 ||
                            alarmArc.Description.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(alarmArc => alarmArc.Name).Cast<AOrmObject>())
                : null;
        }

        public override ICollection<AOrmObject> FulltextSearch(string name)
        {
            ICollection<AlarmArc> linqResult;

            if (!string.IsNullOrEmpty(name))
            {
                linqResult =
                    SelectLinq<AlarmArc>(
                        alarmArc =>
                            alarmArc.Name.IndexOf(name) >= 0 ||
                            alarmArc.Description.IndexOf(name) >= 0);
            }
            else
            {
                linqResult = null;
            }

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(alarmArc => alarmArc.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override ICollection<AOrmObject> ParametricSearch(string name)
        {
            var linqResult =
                string.IsNullOrEmpty(name)
                    ? List()
                    : SelectLinq<AlarmArc>(
                        alarmArc => alarmArc.Name.IndexOf(name) >= 0);

            return linqResult != null
                ? new LinkedList<AOrmObject>(
                    linqResult.OrderBy(alarmArc => alarmArc.Name).Cast<AOrmObject>())
                : new LinkedList<AOrmObject>();
        }

        public override bool ImplementsSearchFunctions()
        {
            return true;
        }

        #endregion

        public override ObjectType ObjectType
        {
            get { return ObjectType.AlarmArc; }
        }

        protected override IList<AOrmObject> GetReferencedObjectsInternal(object idObj)
        {
            var result = new List<AOrmObject>();

            var devicesAlarmSettingAlarmArcs = DevicesAlarmSettings.Singleton.DevicesAlarmSettingAlarmArcs();

            if (devicesAlarmSettingAlarmArcs != null)
            {
                var addeDdevicesAlarmSettingIds = new HashSet<Guid>();

                result.AddRange(
                    devicesAlarmSettingAlarmArcs
                        .Where(
                            devicesAlarmSettingAlarmArc =>
                                devicesAlarmSettingAlarmArc.IdAlarmArc.Equals(idObj))
                        .Select(devicesAlarmSettingAlarmArc =>
                            devicesAlarmSettingAlarmArc.DevicesAlarmSetting)
                        .Where(devicesAlarmSetting =>
                            addeDdevicesAlarmSettingIds.Add(devicesAlarmSetting.IdDevicesAlarmSetting))
                        .Cast<AOrmObject>());
            }

            var alarmAreas = AlarmAreas.Singleton.List();

            if (alarmAreas != null)
            {
                var alarmAreaAlarmArcs = alarmAreas
                    .SelectMany(
                        alarmArea =>
                            alarmArea.AlarmAreaAlarmArcs ?? Enumerable.Empty<AlarmAreaAlarmArc>())
                    .Where(
                        alarmAreaAlarmArc =>
                            alarmAreaAlarmArc.IdAlarmArc.Equals(idObj));

                var addedAlarmAreaIds = new HashSet<Guid>();

                result.AddRange(alarmAreaAlarmArcs
                    .Select(
                        alarmAreaAlarmArc =>
                            alarmAreaAlarmArc.AlarmArea)
                    .Where(
                        alarmArea =>
                            addedAlarmAreaIds.Add(alarmArea.IdAlarmArea))
                    .Cast<AOrmObject>());
            }

            var ccus = CCUs.Singleton.List();

            if (ccus != null)
            {
                var ccuAlarmArcs = ccus
                    .SelectMany(
                        ccu =>
                            ccu.CcuAlarmArcs ?? Enumerable.Empty<CcuAlarmArc>())
                    .Where(
                        ccuAlarmArc =>
                            ccuAlarmArc.IdAlarmArc.Equals(idObj));

                var addedCcuIds = new HashSet<Guid>();

                result.AddRange(ccuAlarmArcs
                    .Select(
                        ccuAlarmArc =>
                            ccuAlarmArc.Ccu)
                    .Where(
                        ccu =>
                            addedCcuIds.Add(ccu.IdCCU))
                    .Cast<AOrmObject>());
            }

            var dcus = DCUs.Singleton.List();

            if (dcus != null)
            {
                var dcuAlarmArcs = dcus
                    .SelectMany(
                        dcu =>
                            dcu.DcuAlarmArcs ?? Enumerable.Empty<DcuAlarmArc>())
                    .Where(
                        dcuAlarmArc =>
                            dcuAlarmArc.IdAlarmArc.Equals(idObj));

                var addedDcuIds = new HashSet<Guid>();

                result.AddRange(dcuAlarmArcs
                    .Select(
                        dcuAlarmArc =>
                            dcuAlarmArc.Dcu)
                    .Where(
                        dcu =>
                            addedDcuIds.Add(dcu.IdDCU))
                    .Cast<AOrmObject>());
            }

            var cardReaders = CardReaders.Singleton.List();

            if (cardReaders != null)
            {
                var cardReaderAlarmArcs = cardReaders
                    .SelectMany(
                        cardReader =>
                            cardReader.CardReaderAlarmArcs ?? Enumerable.Empty<CardReaderAlarmArc>())
                    .Where(
                        cardReaderAlarmArc =>
                            cardReaderAlarmArc.IdAlarmArc.Equals(idObj));

                var addedCardReaderIds = new HashSet<Guid>();

                result.AddRange(cardReaderAlarmArcs
                    .Select(
                        cardReaderAlarmArc =>
                            cardReaderAlarmArc.CardReader)
                    .Where(
                        cardReader =>
                            addedCardReaderIds.Add(cardReader.IdCardReader))
                    .Cast<AOrmObject>());
            }

            var doorEnvironments = DoorEnvironments.Singleton.List();

            if (doorEnvironments != null)
            {
                var doorEnvironmentAlarmArcs = doorEnvironments
                    .SelectMany(
                        doorEnvironment =>
                            doorEnvironment.DoorEnvironmentAlarmArcs ?? Enumerable.Empty<DoorEnvironmentAlarmArc>())
                    .Where(
                        doorEnvironmentAlarmArc =>
                            doorEnvironmentAlarmArc.AlarmArc.IdAlarmArc.Equals(idObj));

                var addedDoorEnvironmentIds = new HashSet<Guid>();

                result.AddRange(doorEnvironmentAlarmArcs
                    .Select(
                        doorEnvironmentAlarmArc =>
                            doorEnvironmentAlarmArc.DoorEnvironment)
                    .Where(
                        doorEnvironment =>
                            addedDoorEnvironmentIds.Add(doorEnvironment.IdDoorEnvironment))
                    .Cast<AOrmObject>());
            }

            return result;
        }

        public IList<AOrmObject> GetBackReferences(Guid idAlarmArc)
        {
            var result = Enumerable.Empty<AOrmObject>();

            var devicesAlarmSettingAlarmArcs = DevicesAlarmSettings.Singleton.DevicesAlarmSettingAlarmArcs();

            if (devicesAlarmSettingAlarmArcs != null)
            {
                result = result.Concat(
                    devicesAlarmSettingAlarmArcs
                        .Where(
                            devicesAlarmSettingAlarmArc =>
                                devicesAlarmSettingAlarmArc.IdAlarmArc.Equals(idAlarmArc))
                        .Cast<AOrmObject>());
            }

            var alarmAreas = AlarmAreas.Singleton.List();

            if (alarmAreas != null)
            {
                result = result.Concat(
                    alarmAreas
                        .SelectMany(
                            alarmArea =>
                                alarmArea.AlarmAreaAlarmArcs ?? Enumerable.Empty<AlarmAreaAlarmArc>())
                        .Where(
                            alarmAreaAlarmArc =>
                                alarmAreaAlarmArc.IdAlarmArc.Equals(idAlarmArc))
                        .Select(alarmAreaAlarmArc =>
                            AlarmAreaAlarmArcs.Singleton.GetById(alarmAreaAlarmArc.IdAlarmAreaAlarmArc))
                        .Cast<AOrmObject>());
            }

            var ccus = CCUs.Singleton.List();

            if (ccus != null)
            {
                result = result.Concat(
                    ccus
                        .SelectMany(
                            ccu =>
                                ccu.CcuAlarmArcs ?? Enumerable.Empty<CcuAlarmArc>())
                        .Where(
                            ccuAlarmArc =>
                                ccuAlarmArc.IdAlarmArc.Equals(idAlarmArc))
                        .Select(
                            ccuAlarmArc =>
                                CcuAlarmArcs.Singleton.GetById(ccuAlarmArc.IdCcuAlarmArc))
                        .Cast<AOrmObject>());
            }

            var dcus = DCUs.Singleton.List();

            if (dcus != null)
            {
                result = result.Concat(
                    dcus
                        .SelectMany(
                            dcu =>
                                dcu.DcuAlarmArcs ?? Enumerable.Empty<DcuAlarmArc>())
                        .Where(
                            dcuAlarmArc =>
                                dcuAlarmArc.IdAlarmArc.Equals(idAlarmArc))
                        .Select(
                            dcuAlarmArc =>
                                DcuAlarmArcs.Singleton.GetById(dcuAlarmArc.IdDcuAlarmArc))
                        .Cast<AOrmObject>());
            }

            var cardReaders = CardReaders.Singleton.List();

            if (cardReaders != null)
            {
                result = result.Concat(
                    cardReaders
                        .SelectMany(
                            cardReader =>
                                cardReader.CardReaderAlarmArcs ?? Enumerable.Empty<CardReaderAlarmArc>())
                        .Where(
                            cardReaderAlarmArc =>
                                cardReaderAlarmArc.IdAlarmArc.Equals(idAlarmArc))
                        .Select(
                            cardReaderAlarmArc =>
                                CardReaderAlarmArcs.Singleton.GetById(cardReaderAlarmArc.IdCardReaderAlarmArc))
                        .Cast<AOrmObject>());
            }

            var doorEnvironments = DoorEnvironments.Singleton.List();

            if (doorEnvironments != null)
            {
                result = result.Concat(
                    doorEnvironments
                        .SelectMany(
                            doorEnvironment =>
                                doorEnvironment.DoorEnvironmentAlarmArcs ?? Enumerable.Empty<DoorEnvironmentAlarmArc>())
                        .Where(
                            doorEnvironmentAlarmArc =>
                                doorEnvironmentAlarmArc.AlarmArc.IdAlarmArc.Equals(idAlarmArc))
                        .Select(
                            doorEnvironmentAlarmArc =>
                                DoorEnvironmentAlarmArcs.Singleton.GetById(
                                    doorEnvironmentAlarmArc.IdDoorEnvironmentAlarmArc))
                        .Cast<AOrmObject>());
            }

            return new List<AOrmObject>(result);
        }

        public AlarmArc GetAlarmArcByName(string alarmArcName)
        {
            var alarmArcs = SelectLinq<AlarmArc>(
                alarmArc =>
                    alarmArc.Name.Equals(alarmArcName));

            if (alarmArcs == null)
                return null;

            return alarmArcs.FirstOrDefault();
        }

        public void CreateNotExistsAlarmArcs(
            IEnumerable<string> alarmArcNames,
            int? idStructuredSubSite)
        {
            if (alarmArcNames == null)
                return;

            var notExistsAlarmArcNames =
                alarmArcNames.Where(
                    alarmArcName =>
                        GetAlarmArcByName(alarmArcName) == null);

            foreach (var alarmArcName in notExistsAlarmArcNames)
            {
                var alarmArc = new AlarmArc
                {
                    Name = alarmArcName
                };

                Insert(
                    ref alarmArc,
                    idStructuredSubSite);
            }
        }

        public override void CUDSpecial(AlarmArc alarmArc, ObjectDatabaseAction objectDatabaseAction)
        {
            if (alarmArc == null)
                return;
            
            if (objectDatabaseAction == ObjectDatabaseAction.Update)
            {
                DataReplicationManager.Singleton.SendModifiedObjectToCcus(alarmArc);
            }
        }
    }
}
