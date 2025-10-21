using System;
using System.Collections.Generic;
using System.Linq;
using Contal.Cgp.NCAS.CCU.CardReaderMechanism.Scenes;
using Contal.Cgp.NCAS.CCU.DB;
using Contal.Cgp.Globals;
using Contal.IwQuick;
using Contal.IwQuick.Data;

namespace Contal.Cgp.NCAS.CCU
{
    public sealed class CardAccessRightsManager :
        ASingleton<CardAccessRightsManager>
    {
        private CardAccessRightsManager()
            : base(null)
        {
        }

        public bool HasAccess(
            AccessDataBase accessData,
            Guid idCardReader,
            Guid idDoorEnvironment,
            Guid idDcu)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool CardAccessForCardReaders.HasAccess(Guid idPerson, Guid idCardReader): [{0}, {1}]",
                    accessData.IdPerson,
                    idCardReader));

            var idPerson = accessData.IdPerson;

            if (Database.ConfigObjectsEngine.AccessZonesStorage.ExistsAccessZonesForPerson(idPerson))
            {
                if (HasAccess(
                    Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForCrObject(
                        idPerson,
                        ObjectType.CardReader,
                        idCardReader)))
                {
                    return true;
                }

                if (idDoorEnvironment != Guid.Empty
                    && HasAccess(
                        Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForCrObject(
                            idPerson,
                            ObjectType.DoorEnvironment,
                            idDoorEnvironment)))
                {
                    return true;
                }

                if (idDcu != Guid.Empty
                    && HasAccess(
                        Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForCrObject(
                            idPerson,
                            ObjectType.DCU,
                            idDcu)))
                {
                    return true;
                }

                if (HasAccess(
                    Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForAlarmAreas(
                        idPerson,
                        idCardReader)))
                {
                    return true;
                }
            }

            var acls = Database.ConfigObjectsEngine.AclPersonsStorage.GetAclIdsForPerson(idPerson).ToArray();

            if (acls.Length == 0)
                return false;

            if (acls.Any(idAcl =>
                HasAccess(
                    Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForCardReaderObject(
                        ObjectType.CardReader,
                        idCardReader,
                        idAcl))))
            {
                return true;
            }

            if (idDoorEnvironment != Guid.Empty
                && acls.Any(idAcl =>
                    HasAccess(
                        Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForCardReaderObject(
                            ObjectType.DoorEnvironment,
                            idDoorEnvironment,
                            idAcl))))
            {
                return true;
            }

            if (idDcu != Guid.Empty
                && acls.Any(idAcl =>
                    HasAccess(
                        Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForCardReaderObject(
                            ObjectType.DCU,
                            idDcu,
                            idAcl))))
            {
                return true;
            }

            if (acls.Any(idAcl =>
                HasAccess(
                    Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForAlarmAreas(
                        idCardReader,
                        idAcl))))
            {
                return true;
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL, () => "bool CardAccessForCardReaders.HasAccess return false");
            return false;
        }

        private static bool HasAccess(
            IEnumerable<Guid> timeZoneIds)
        {
            return timeZoneIds
                .Any(idTimeZone =>
                    idTimeZone == Guid.Empty
                    || TimeZones.Singleton.GetActualState(idTimeZone) == State.On);
        }

        public bool HasAccessTest(
            ICard card,
            Guid idCardReader,
            Guid idDoorEnvironment,
            Guid idDcu,
            out string additionalInfo)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool CardAccessForCardReaders.HasAccessTest(CardData card, Guid guidCardReader, out string additionalInfo): [{0}, {1}]",
                    card,
                    idCardReader));

            try
            {
                additionalInfo = string.Empty;
                var idPerson = card.GuidPerson;

                if (Database.ConfigObjectsEngine.AccessZonesStorage.ExistsAccessZonesForPerson(idPerson))
                {
                    if (HasAccess(
                        Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForCrObject(
                            idPerson,
                            ObjectType.CardReader,
                            idCardReader)))
                    {
                        CcuCore.DebugLog.Info(
                            Log.NORMAL_LEVEL,
                            () => "bool CardAccessForCardReaders.HasAccessTest return true");

                        return true;
                    }

                    if (HasAccess(
                        Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForCrObject(
                            idPerson,
                            ObjectType.DoorEnvironment,
                            idDoorEnvironment)))
                    {
                        CcuCore.DebugLog.Info(
                            Log.NORMAL_LEVEL,
                            () => "bool CardAccessForCardReaders.HasAccessTest return true");

                        return true;
                    }

                    if (idDcu != Guid.Empty)
                    {
                        if (HasAccess(
                            Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForCrObject(
                                idPerson,
                                ObjectType.DCU,
                                idDcu)))
                        {
                            CcuCore.DebugLog.Info(
                                Log.NORMAL_LEVEL,
                                () => "bool CardAccessForCardReaders.HasAccessTest return true");

                            return true;
                        }
                    }

                    if (HasAccess(
                        Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForAlarmAreas(
                            idPerson,
                            idCardReader)))
                    {
                        CcuCore.DebugLog.Info(
                            Log.NORMAL_LEVEL,
                            () => "bool CardAccessForCardReaders.HasAccessTest return true");

                        return true;
                    }
                }

                var acls = Database.ConfigObjectsEngine.AclPersonsStorage.GetAclIdsForPerson(idPerson).ToArray();

                if (acls.Length == 0)
                {
                    CcuCore.DebugLog.Info(
                        Log.NORMAL_LEVEL,
                        () => "bool CardAccessForCardReaders.HasAccessTest return false");

                    return false;
                }

                if (acls.Any(idAcl =>
                    HasAccess(
                        Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForCardReaderObject(
                            ObjectType.CardReader,
                            idCardReader,
                            idAcl))))
                {
                    CcuCore.DebugLog.Info(
                        Log.NORMAL_LEVEL,
                        () => "bool CardAccessForCardReaders.HasAccessTest return true");

                    return true;
                }

                if (acls.Any(idAcl =>
                    HasAccess(
                        Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForCardReaderObject(
                            ObjectType.DoorEnvironment,
                            idDoorEnvironment,
                            idAcl))))
                {
                    CcuCore.DebugLog.Info(
                        Log.NORMAL_LEVEL,
                        () => "bool CardAccessForCardReaders.HasAccessTest return true");

                    return true;
                }

                if (idDcu != Guid.Empty)
                {
                    if (acls.Any(idAcl =>
                        HasAccess(
                            Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForCardReaderObject(
                                ObjectType.DCU,
                                idDcu,
                                idAcl))))
                    {
                        CcuCore.DebugLog.Info(
                            Log.NORMAL_LEVEL,
                            () => "bool CardAccessForCardReaders.HasAccessTest return true");

                        return true;
                    }
                }

                if (acls.Any(idAcl =>
                    HasAccess(
                        Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForAlarmAreas(
                            idCardReader,
                            idAcl))))
                {
                    CcuCore.DebugLog.Info(
                        Log.NORMAL_LEVEL,
                        () => "bool CardAccessForCardReaders.HasAccessTest return true");

                    return true;
                }

                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () => "bool CardAccessForCardReaders.HasAccessTest return false");

                return false;
            }
            catch (Exception error)
            {
                Console.WriteLine("CardAccessForCardReader HasAccessTest exception: " + error);
                additionalInfo = "Exception: " + error;
            }

            return false;
        }

        public ICollection<Guid> HasAccessMultiDoor(
            AccessDataBase accessData,
            Guid idMultiDoor,
            ICollection<Guid> elementIds)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "ICollection<Guid> CardAccessForCardReaders.HasAccessMultiDoor(Guid idPerson, Guid idMutliDoor): [{0}, {1}]",
                    accessData.IdPerson,
                    idMultiDoor));

            var idPerson = accessData.IdPerson;

            var existsAccessZonesForPerson = Database.ConfigObjectsEngine.AccessZonesStorage.ExistsAccessZonesForPerson(idPerson);

            if (existsAccessZonesForPerson)
            {
                if (HasAccess(
                    Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForCrObject(
                        idPerson,
                        ObjectType.MultiDoor,
                        idMultiDoor)))
                {
                    CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                        () =>
                            string.Format(
                                "ICollection<Guid> CardAccessForCardReaders.HasAccessMultiDoor return {0}",
                                Log.GetStringFromParameters(elementIds)));

                    return elementIds;
                }
            }

            var acls = Database.ConfigObjectsEngine.AclPersonsStorage.GetAclIdsForPerson(idPerson).ToArray();

            if (acls.Any(idAcl =>
                HasAccess(
                    Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForCardReaderObject(
                        ObjectType.MultiDoor,
                        idMultiDoor,
                        idAcl))))
            {
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                    () =>
                        string.Format("ICollection<Guid> CardAccessForCardReaders.HasAccessMultiDoor return {0}",
                            Log.GetStringFromParameters(elementIds)));

                return elementIds;
            }

            var multiDoorElements = new HashSet<Guid>();

            foreach (var id in elementIds)
            {
                var elementId = id;

                if ((existsAccessZonesForPerson
                     && (HasAccess(
                         Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForMultiDoorElement(
                             idPerson,
                             elementId))
                         || HasAccess(
                             Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForFloor(
                                 idPerson,
                                 elementId))))
                    || acls.Any(idAcl =>
                        HasAccess(
                            Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForMultiDoorElement(
                                elementId,
                                idAcl)))
                    || acls.Any(idAcl =>
                        HasAccess(
                            Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForFloor(
                                elementId,
                                idAcl))))
                {
                    multiDoorElements.Add(elementId);
                }
            }

            if (multiDoorElements.Count > 0)
            {
                CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                    () =>
                        string.Format("ICollection<Guid> CardAccessForCardReaders.HasAccessMultiDoor return {0}",
                            Log.GetStringFromParameters(multiDoorElements)));

                return multiDoorElements;
            }

            CcuCore.DebugLog.Info(Log.NORMAL_LEVEL,
                () =>
                    "ICollection<Guid> CardAccessForCardReaders.HasAccessMultiDoor return null");

            return null;
        }

        public bool HasAccessMultiDoorTest(
            ICard card,
            Guid idMultiDoorElement,
            out string additionalInfo)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool CardAccessForCardReaders.HasAccessMultiDoorTest(CardData card, Guid idMultiDoorElement, out string additionalInfo): [{0}, {1}]",
                    card,
                    idMultiDoorElement));

            additionalInfo = string.Empty;

            if (card != null
                && HasAccessMultiDoorTest(
                    card.GuidPerson,
                    idMultiDoorElement,
                    ref additionalInfo))
            {
                CcuCore.DebugLog.Info(
                    Log.NORMAL_LEVEL,
                    () =>
                        "bool CardAccessForCardReaders.HasAccessMultiDoorTest return true");

                return true;
            }

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () =>
                    "bool CardAccessForCardReaders.HasAccessMultiDoorTest return false");

            return false;
        }

        public bool HasAccessMultiDoorTest(
            Guid idPerson,
            Guid idMultiDoorElement,
            ref string additionalInfo)
        {
            var multiDoorElelemnt = Database.ConfigObjectsEngine.GetFromDatabase(
                ObjectType.MultiDoorElement,
                idMultiDoorElement) as MultiDoorElement;

            if (multiDoorElelemnt == null)
            {
                additionalInfo = "Faild to load multi door element from database";
                return false;
            }

            var existsAccessZonesForPerson = Database.ConfigObjectsEngine.AccessZonesStorage.ExistsAccessZonesForPerson(idPerson);

            if (existsAccessZonesForPerson)
            {
                if (HasAccess(
                    Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForCrObject(
                        idPerson,
                        ObjectType.MultiDoor,
                        multiDoorElelemnt.MultiDoorId)))
                {
                    return true;
                }
            }

            var acls = Database.ConfigObjectsEngine.AclPersonsStorage.GetAclIdsForPerson(idPerson).ToArray();

            if (acls.Any(idAcl =>
                HasAccess(
                    Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForCardReaderObject(
                        ObjectType.MultiDoor,
                        multiDoorElelemnt.MultiDoorId,
                        idAcl))))
            {
                return true;
            }

            var multiDoorElements = new HashSet<Guid>();

            if ((existsAccessZonesForPerson
                 && (HasAccess(
                     Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForMultiDoorElement(
                         idPerson,
                         idMultiDoorElement))
                     || HasAccess(
                         Database.ConfigObjectsEngine.AccessZonesStorage.GetTimeZoneIdsForFloor(
                             idPerson,
                             idMultiDoorElement))))
                || acls.Any(idAcl =>
                    HasAccess(
                        Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForMultiDoorElement(
                            idMultiDoorElement,
                            idAcl)))
                || acls.Any(idAcl =>
                    HasAccess(
                        Database.ConfigObjectsEngine.AclSettingsStorage.GetTimeZoneIdsForFloor(
                            idMultiDoorElement,
                            idAcl))))
            {
                multiDoorElements.Add(idMultiDoorElement);
            }

            if (multiDoorElements.Count > 0)
            {
                if (multiDoorElements.Contains(idMultiDoorElement))
                    return true;
            }

            additionalInfo = "Person has no access for the multi door element";
            return false;
        }
    }
}
