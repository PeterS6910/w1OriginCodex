using System.Collections.Generic;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public sealed class CloneFilterValues : ASingleton<CloneFilterValues>
    {
        public readonly Dictionary<ObjectType, Dictionary<string, bool>> _cloneFilterValues =
            new Dictionary<ObjectType, Dictionary<string, bool>>();

        private CloneFilterValues() : base(null)
        {
        }

        public Dictionary<string, bool> GetCloneFilterValues(ObjectType objectType)
        {
            Dictionary<string, bool> filterValues;

            if (!_cloneFilterValues.TryGetValue(objectType, out filterValues))
            {
                filterValues =
                    GetDefaultCloneFilterForObjectType(objectType);

                if (filterValues != null)
                    if (filterValues.Count > 0)
                        _cloneFilterValues.Add(
                            objectType,
                            filterValues);
                    else
                        filterValues = null;
            }

            return filterValues;
        }

        public void SetDefaultCloneFilterForObjectType(ObjectType objectType, Dictionary<string, bool> values)
        {
            if (!_cloneFilterValues.ContainsKey(objectType))
                _cloneFilterValues.Add(objectType, values);
            else
                _cloneFilterValues[objectType] = values;
        }

        public void ClearCloneFilterValues()
        {
            if (_cloneFilterValues != null)
                _cloneFilterValues.Clear();
        }

        private static Dictionary<string, bool> GetDefaultCloneFilterForObjectType(
            ObjectType objectType)
        {
            var result = new Dictionary<string, bool>();

            switch (objectType)
            {
                case ObjectType.Person:
                    result.Add("Company", true);
                    result.Add("Role", true);
                    result.Add("Address", true); // SB
                    result.Add("Email", false);
                    result.Add("Phone", false);
                    result.Add("Department", true);
                    result.Add("Relative_superior", false);
                    result.Add("Relative_superiorsPhoneNumber", false);
                    result.Add("CostCenter", true);
                    result.Add("EmploymentBeginningDate", false);
                    result.Add("EmpoymentEndDate", false);
                    break;
                case ObjectType.DailyPlan:
                case ObjectType.TimeZone:
                case ObjectType.Calendar:
                case ObjectType.CalendarDateSetting:
                case ObjectType.DayInterval:
                case ObjectType.TimeZoneDateSetting:
                case ObjectType.SecurityDayInterval:
                case ObjectType.SecurityDailyPlan:
                case ObjectType.DayType:
                case ObjectType.Output:
                case ObjectType.Input:
                case ObjectType.ACLSetting:
                case ObjectType.ACLSettingAA:
                case ObjectType.ACLPerson:
                case ObjectType.AccessControlList:
                case ObjectType.Card:
                case ObjectType.CardSystem:
                case ObjectType.AlarmArea:
                case ObjectType.AACardReader:
                case ObjectType.AccessZone:
                case ObjectType.CCU:
                case ObjectType.AAInput:
                case ObjectType.DCU:
                case ObjectType.DoorEnvironment:
                case ObjectType.CardReader:
                case ObjectType.DevicesAlarmSetting:
                case ObjectType.SecurityTimeZone:
                case ObjectType.SecurityTimeZoneDateSetting:
                case ObjectType.Login:
                case ObjectType.AccessControl:
                case ObjectType.PresentationGroup:
                case ObjectType.SecuritySetting:
                case ObjectType.SystemEvent:
                case ObjectType.CisNGGroup:
                case ObjectType.PresentationFormatter:
                case ObjectType.CisNG:
                case ObjectType.CustomAccessControl:
                case ObjectType.Eventlog:
                case ObjectType.UserFoldersStructure:
                case ObjectType.UserFoldersStructureObject:
                case ObjectType.CentralNameRegister:
                case ObjectType.ServerGeneralOptionsDB:
                case ObjectType.NotSupport:
                    return null;
                default:
                    return null;
            }

            foreach (ICgpVisualPlugin plugin in CgpClient.Singleton.PluginManager.GetVisualPlugins())
            {
                Dictionary<string, bool> pluginCloneFilters = plugin.GetDefaultCloneFilterForObjectType(objectType);
                if (pluginCloneFilters != null && pluginCloneFilters.Count > 0)
                    foreach (KeyValuePair<string, bool> item in pluginCloneFilters)
                    {
                        result.Add(item.Key, item.Value);
                    }
            }

            return result;
        }
    }
}
