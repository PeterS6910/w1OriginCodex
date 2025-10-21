using System.Collections.Generic;

using Contal.IwQuick.Data;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;

namespace Contal.Cgp.Server.DB
{
    public static class ObjectTypeTablesORM
    {
        private static readonly IDictionary<ObjectType, ITableORM> _objectTypeTablesORM;

        static ObjectTypeTablesORM()
        {
            _objectTypeTablesORM =
                new SyncDictionary<ObjectType, ITableORM>
                {
                    {
                        ObjectType.AccessControl,
                        AccessControls.Singleton
                    },
                    {
                        ObjectType.AlarmPriority,
                        AlarmPrioritiesDbs.Singleton
                    },
                    {
                        ObjectType.CalendarDateSetting,
                        CalendarDateSettings.Singleton
                    },
                    {
                        ObjectType.Calendar,
                        Calendars.Singleton
                    },
                    {
                        ObjectType.CardPair,
                        CardPairs.Singleton
                    },
                    {
                        ObjectType.Card,
                        Cards.Singleton
                    },
                    {
                        ObjectType.CardSystem,
                        CardSystems.Singleton
                    },
                    {
                        ObjectType.CardTemplate,
                        CardTemplates.Singleton
                    },
                    {
                        ObjectType.CentralNameRegister,
                        CentralNameRegisters.Singleton
                    },
                    {
                        ObjectType.CisNGGroup,
                        CisNGGroups.Singleton
                    },
                    {
                        ObjectType.CisNG,
                        CisNGs.Singleton
                    },
                    {
                        ObjectType.CSVImportColumn,
                        CSVImportColumns.Singleton
                    },
                    {
                        ObjectType.CSVImportSchema,
                        CSVImportSchemas.Singleton
                    },
                    {
                        ObjectType.CustomAccessControl,
                        CustomAccesControls.Singleton
                    },
                    {
                        ObjectType.DailyPlan,
                        DailyPlans.Singleton
                    },
                    {
                        ObjectType.DayInterval,
                        DayIntervals.Singleton
                    },
                    {
                        ObjectType.DayType,
                        DayTypes.Singleton
                    },
                    {
                        ObjectType.GlobalAlarmInstruction,
                        GlobalAlarmInstructions.Singleton
                    },
                    {
                        ObjectType.LoginGroup,
                        LoginGroups.Singleton
                    },
                    {
                        ObjectType.Login,
                        Logins.Singleton
                    },
                    {
                        ObjectType.Person,
                        Persons.Singleton
                    },
                    {
                        ObjectType.Car,
                        Cars.Singleton
                    },
                    {
                        ObjectType.PresentationFormatter,
                        PresentationFormatters.Singleton
                    },
                    {
                        ObjectType.PresentationGroup,
                        PresentationGroups.Singleton
                    },
                    {
                        ObjectType.RelationshipGlobalAlarmInstructionObject,
                        RelationshipGlobalAlarmInstructionObjects.Singleton
                    },
                    {
                        ObjectType.SecuritySetting,
                        SecuritySettings.Singleton
                    },
                    {
                        ObjectType.ServerGeneralOptionsDB,
                        ServerGeneralOptionsDBs.Singleton
                    },
                    {
                        ObjectType.StructuredSubSiteObject,
                        StructuredSubSiteObjects.Singleton
                    },
                    {
                        ObjectType.StructuredSubSite,
                        StructuredSubSites.Singleton
                    },
                    {
                        ObjectType.SystemEvent,
                        SystemEvents.Singleton
                    },
                    {
                        ObjectType.TimeZoneDateSetting,
                        TimeZoneDateSettings.Singleton
                    },
                    {
                        ObjectType.TimeZone,
                        TimeZones.Singleton
                    },
                    {
                        ObjectType.UserFoldersStructureObject,
                        UserFoldersStructureObjects.Singleton
                    },
                    {
                        ObjectType.UserFoldersStructure,
                        UserFoldersStructures.Singleton
                    }
                };
        }

        public static ITableORM GetTableOrm(ObjectType objectType)
        {
            ITableORM result;

            _objectTypeTablesORM.TryGetValue(
                objectType,
                out result);

            return result;
        }
    }
}
