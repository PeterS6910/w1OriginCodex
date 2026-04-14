using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.NCAS.CCU.DB;
using Contal.IwQuick;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.NCAS.CCU
{
    public static class Database
    {
        public static IConfigObjectsEngine ConfigObjectsEngine;
        public static IAutonomousEventsEngine AutonomousEventsEngine;

        public const string DATABASE_DIRECTORY_NAME = @"Database\";

        public const string DELETE_DATABASE = "DeleteDatabase";

        static Database()
        {
            ConfigObjectsEngine = new SqlCeDbEngine.SqlCeDbConfigObjectsEngine();
            AutonomousEventsEngine = new SqlCeDbEngine.SqlCeDbAutonomousEventsEngine();
        }

        /// <summary>
        /// Save database object to Database
        /// </summary>
        /// <param name="objectsToSend"></param>
        public static bool SaveToDatabase(ObjectsToSend objectsToSend)
        {
            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => string.Format(
                    "bool Database.SaveToDatabase(ObjectsToSend objectsToSend): [{0}]",
                    Log.GetStringFromParameters(objectsToSend)));

            try
            {
                ConfigObjectsEngine.SaveObjectsToDatabase(
                    objectsToSend.ObjectType,
                    objectsToSend.Objects.OfType<IDbObject>());
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);

                CcuCore.DebugLog.Warning(
                    Log.LOW_LEVEL,
                    "bool Database.SaveToDatabase return false");

                return false;
            }

            CcuCore.DebugLog.Info(
                Log.NORMAL_LEVEL,
                () => "bool Database.SaveToDatabase return true");

            return true;
        }
    }
}
