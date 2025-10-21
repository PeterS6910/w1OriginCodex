using System;
using System.Collections.Generic;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.CCU.Alarms;
using Contal.Cgp.NCAS.CCU.DB;

namespace Contal.Cgp.NCAS.CCU
{
    public interface IConfigObjectsEngine
    {
        ICardsStorage CardsStorage { get; }
        IApbzStorage ApbzStorage { get; }
        ICrEventlogEventsStorage CrEventlogEventsStorage { get; }
        IAlarmsStorage AlarmsStorage { get; }
        IPersonsStorage PersonsStorage { get; }
        IAclPersonsStorage AclPersonsStorage { get; }
        IAclSettingsStorage AclSettingsStorage { get; }
        IDoorEnvironmentsStorage DoorEnvironmentsStorage { get; }
        IAclSettingAAsStorage AclSettingAasStorage { get; }
        IAccessZonesStorage AccessZonesStorage { get; }
        IAaCardReadersStorage AaCardReadersStorage { get; }

        void ReadObjectsFromFiles();

        /// <summary>
        /// Save database object to Database
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="objectsToSave"></param>
        void SaveObjectsToDatabase(ObjectType objectType, IEnumerable<IDbObject> objectsToSave);

        void SaveMaxObjectTypeVersion(ObjectType objectType, int version);

        /// <summary>
        /// Get the object form database
        /// </summary>
        /// <param name="objectType">type of object by enum ObjectTypes</param>
        /// <param name="guid">Guid of object</param>
        /// <returns>database object</returns>
        object GetFromDatabase(ObjectType objectType, Guid guid);

        bool Exists(ObjectType objectType, Guid guid);
        MaximumVersionAndIds GetMaximumVersionAndIds(ObjectType objectType);
        void DeleteFromDatabase(ObjectType objectType, IEnumerable<Guid> objectGuids);
        void DeleteAllFromDatabase();
        void DeleteAllFromDatabase(ObjectType objectType);

        /// <summary>
        /// Get all guids of objects defined type from database
        /// </summary>
        /// <param name="objectType">type of required objects</param>
        /// <returns>List Guids</returns>
        ICollection<Guid> GetPrimaryKeysForObjectType(ObjectType objectType);

        bool ContainsAnyObjects(ObjectType objectType);

        ICollection<Guid> GetIdsOfRecentlySavedObjects(ObjectType objectType);

        void OnApplyChangesDone();
    }
}