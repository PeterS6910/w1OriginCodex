using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans.Extern;

namespace Contal.Cgp.Client
{
    public class DbsSupport  
    { 
        public static AOrmObject GetTableObject(ObjectType objectType, Guid objectId)
        {
            return GetTableObject(objectType, objectId.ToString());
        }
         
        public static AOrmObject GetTableObject(ObjectType objectType, string strObjectId) 
        {
            if (CgpClient.Singleton.MainServerProvider == null)
                return null;

            return CgpClient.Singleton.MainServerProvider.GetTableObject(objectType, strObjectId);
        }

        public static bool ExistTableObject(ObjectType objectType, string strObjectId)
        {
            return GetTableObject(objectType, strObjectId) != null;
        }

        public static void OpenEditForm(AOrmObject dbObj)
        {
            if (dbObj == null) return;

            if (dbObj.GetObjectType() == ObjectType.Person)
            {
                PersonsForm.Singleton.OpenEditForm(dbObj as Person);
            }
            else if (dbObj.GetObjectType() == ObjectType.Calendar)
            {
                CalendarsForm.Singleton.OpenEditForm(dbObj as Calendar);
            }
            else if (dbObj.GetObjectType() == ObjectType.Card)
            {
                CardsForm.Singleton.OpenEditForm(dbObj as Card);
            }
            else if (dbObj.GetObjectType() == ObjectType.CardTemplate)
            {
                CardTemplatesForm.Singleton.EditSpecific(dbObj as CardTemplate);
            }
            else if (dbObj.GetObjectType() == ObjectType.CardSystem)
            {
                CardSystemsForm.Singleton.OpenEditForm(dbObj as CardSystem);
            }
            else if (dbObj.GetObjectType() == ObjectType.CisNGGroup)
            {
                CisNGGroupsForm.Singleton.OpenEditForm(dbObj as CisNGGroup);
            }
            else if (dbObj.GetObjectType() == ObjectType.CisNG)
            {
                CisNGsForm.Singleton.OpenEditForm(dbObj as CisNG);
            }
            else if (dbObj.GetObjectType() == ObjectType.DailyPlan)
            {
                DailyPlansForm.Singleton.OpenEditForm(dbObj as DailyPlan);
            }
            else if (dbObj.GetObjectType() == ObjectType.DayType)
            {
                DayTypesForm.Singleton.OpenEditForm(dbObj as DayType);
            }
            else if (dbObj.GetObjectType() == ObjectType.LoginGroup)
            {
                LoginGroupsForm.Singleton.OpenEditForm(dbObj as LoginGroup);
            }
            else if (dbObj.GetObjectType() == ObjectType.Login)
            {
                LoginsForm.Singleton.OpenEditForm(dbObj as Login);
            }
            else if (dbObj.GetObjectType() == ObjectType.PresentationFormatter)
            {
                PresentationFormattersForm.Singleton.OpenEditForm(dbObj as PresentationFormatter);
            }
            else if (dbObj.GetObjectType() == ObjectType.PresentationGroup)
            {
                PresentationGroupsForm.Singleton.OpenEditForm(dbObj as PresentationGroup);
            }
            else if (dbObj.GetObjectType() == ObjectType.TimeZone)
            {
                TimeZonesForm.Singleton.OpenEditForm(dbObj as Contal.Cgp.Server.Beans.TimeZone);
            }
            else if (dbObj.GetObjectType() == ObjectType.UserFoldersStructure)
            {
                var userFolder = dbObj as UserFoldersStructure;
                if (userFolder == null)
                    return;

                var objectPlacment = CgpClient.Singleton.MainServerProvider.StructuredSubSites.GetObjectPlacements(
                        userFolder.GetObjectType(), userFolder.GetIdString(),
                        @"\",
                        CgpClient.Singleton.LocalizationHelper.GetString("SelectStructuredSubSiteForm_RootNode"),
                        true).FirstOrDefault();

                StructuredSiteForm.Singleton.OpenFormAndSelectObject(
                    objectPlacment,
                    new IdAndObjectType(userFolder.GetId(), userFolder.GetObjectType()));
            }
            else if (dbObj.GetObjectType() == ObjectType.GlobalAlarmInstruction)
            {
                GlobalAlarmInstructionsForm.Singleton.OpenEditForm(dbObj as GlobalAlarmInstruction);
            }
            else
            {
                foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
                {
                    plugin._plugin.OpenDBSEdit(dbObj);
                }
            }
        }

        public static void OpenEditDialog(AOrmObject dbObj)
        {
            if (dbObj == null) return;

            if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.Person))
            {
                PersonsForm.Singleton.OpenEditDialog(dbObj as Person);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.Calendar))
            {
                CalendarsForm.Singleton.OpenEditDialog(dbObj as Calendar);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.Card))
            {
                CardsForm.Singleton.OpenEditDialog(dbObj as Card);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.CardSystem))
            {
                CardSystemsForm.Singleton.OpenEditDialog(dbObj as CardSystem);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.CisNGGroup))
            {
                CisNGGroupsForm.Singleton.OpenEditDialog(dbObj as CisNGGroup);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.CisNG))
            {
                CisNGsForm.Singleton.OpenEditDialog(dbObj as CisNG);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.DailyPlan))
            {
                DailyPlansForm.Singleton.OpenEditDialog(dbObj as DailyPlan);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.DayType))
            {
                DayTypesForm.Singleton.OpenEditDialog(dbObj as DayType);
            }
            else if (dbObj.GetType() == typeof(LoginGroup))
            {
                LoginGroupsForm.Singleton.OpenEditDialog(dbObj as LoginGroup);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.Login))
            {
                LoginsForm.Singleton.OpenEditDialog(dbObj as Login);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.PresentationFormatter))
            {
                PresentationFormattersForm.Singleton.OpenEditDialog(dbObj as PresentationFormatter);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.PresentationGroup))
            {
                PresentationGroupsForm.Singleton.OpenEditDialog(dbObj as PresentationGroup);
            }
            else if (dbObj.GetType() == typeof(Contal.Cgp.Server.Beans.TimeZone))
            {
                TimeZonesForm.Singleton.OpenEditDialog(dbObj as Contal.Cgp.Server.Beans.TimeZone);
            }
            else if (dbObj.GetType() == typeof(GlobalAlarmInstruction))
            {
                GlobalAlarmInstructionsForm.Singleton.OpenEditDialog(dbObj as GlobalAlarmInstruction);
            }
        }

        public static void OpenInsertForm(string strObjectTableType, Action<object> doAfterInsert)
        {
            if (strObjectTableType ==typeof(Person).Name + "s")
            {
                Person dbObj = new Person();
                PersonsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(Calendar).Name + "s")
            {
                Calendar dbObj = new Calendar();
                CalendarsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType ==typeof(Card).Name + "s")
            {
                Card dbObj = new Card();
                CardsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(CardSystem).Name + "s")
            {
                CardSystem dbObj = new CardSystem();
                CardSystemsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(CisNGGroup).Name + "s")
            {
                CisNGGroup dbObj = new CisNGGroup();
                CisNGGroupsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(CisNG).Name + "s")
            {
                CisNG dbObj = new CisNG();
                CisNGsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(DailyPlan).Name + "s")
            {
                DailyPlan dbObj = new DailyPlan();
                DailyPlansForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(DayType).Name + "s")
            {
                DayType dbObj = new DayType();
                DayTypesForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(LoginGroup).Name + "s")
            {
                var dbObj = new LoginGroup();
                LoginGroupsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(Login).Name + "s")
            {
                Login dbObj = new Login();
                LoginsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(PresentationFormatter).Name + "s")
            {
                PresentationFormatter dbObj = new PresentationFormatter();
                PresentationFormattersForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(PresentationGroup).Name + "s")
            {
                PresentationGroup dbObj = new PresentationGroup();
                PresentationGroupsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(Contal.Cgp.Server.Beans.TimeZone).Name + "s")
            {
                Contal.Cgp.Server.Beans.TimeZone dbObj = new Contal.Cgp.Server.Beans.TimeZone();
                TimeZonesForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(GlobalAlarmInstruction).Name + "s")
            {
                GlobalAlarmInstruction dbObj = new GlobalAlarmInstruction();
                GlobalAlarmInstructionsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else
            {
                foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
                {
                    plugin._plugin.OpenDBSInsert(strObjectTableType, doAfterInsert);
                }
            }
        }

        public static Icon GetIconForObjectType(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Person:
                    return PersonsForm.Singleton.Icon;
                case ObjectType.Calendar:
                    return CalendarsForm.Singleton.Icon;
                case ObjectType.Card:
                    return CardsForm.Singleton.Icon;
                case ObjectType.CardSystem:
                    return CardSystemsForm.Singleton.Icon;
                case ObjectType.CisNGGroup:
                    return CisNGGroupsForm.Singleton.Icon;
                case ObjectType.CisNG:
                    return CisNGsForm.Singleton.Icon;
                case ObjectType.DailyPlan:
                    return DailyPlansForm.Singleton.Icon;
                case ObjectType.DayType:
                    return DayTypesForm.Singleton.Icon;
                case ObjectType.LoginGroup:
                    return LoginGroupsForm.Singleton.Icon;
                case ObjectType.Login:
                    return LoginsForm.Singleton.Icon;
                case ObjectType.PresentationFormatter:
                    return PresentationFormattersForm.Singleton.Icon;
                case ObjectType.PresentationGroup:
                    return PresentationGroupsForm.Singleton.Icon;
                case ObjectType.TimeZone:
                    return TimeZonesForm.Singleton.Icon;
                case ObjectType.UserFoldersStructure:
                    return ResourceGlobal.IconFolderStruct16;
                case ObjectType.GlobalAlarmInstruction:
                    return ResourceGlobal.IconAlarmInstructions16;
            }

            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                var visualPlugin = plugin._plugin as ICgpVisualPlugin;

                if (visualPlugin != null)
                {
                    Icon icon = visualPlugin.GetIconForObjectType(objectType);
                    if (icon != null)
                        return icon;
                }
            }
            return null;
        }

        public static Image GetImageForObjectType(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Person:
                    return ResourceGlobal.Persons16;
                case ObjectType.Calendar:
                    return ResourceGlobal.Calendar16;
                case ObjectType.Card:
                    return ResourceGlobal.IconCardsNew16.ToBitmap();
                case ObjectType.CardTemplate:
                    return ResourceGlobal.IconCardTemplate16.ToBitmap();
                case ObjectType.CardSystem:
                    return ResourceGlobal.IconCardSystemNew16.ToBitmap();
                case ObjectType.CisNGGroup:
                    return CisNGGroupsForm.Singleton.Icon.ToBitmap();
                case ObjectType.CisNG:
                    return CisNGsForm.Singleton.Icon.ToBitmap();
                case ObjectType.DailyPlan:
                    return ResourceGlobal.DailyPLan16;
                case ObjectType.DayType:
                    return ResourceGlobal.DayType16;
                case ObjectType.Login:
                    return ResourceGlobal.Logins16;
                case ObjectType.PresentationFormatter:
                    return ResourceGlobal.Formater16;
                case ObjectType.PresentationGroup:
                    return ResourceGlobal.PresentationGroup16;
                case ObjectType.TimeZone:
                    return ResourceGlobal.TimeZone16;
                case ObjectType.UserFoldersStructure:
                    return ResourceGlobal.UserFoldersStructure;
                case ObjectType.GlobalAlarmInstruction:
                    return ResourceGlobal.IconAlarmInstructions16.ToBitmap();
            }

            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
                var visualPlugin = plugin._plugin as ICgpVisualPlugin;

                if (visualPlugin != null)
                {
                    Icon icon = visualPlugin.GetIconForObjectType(objectType);
                    if (icon != null)
                        return icon.ToBitmap();
                }
            }
            return null;
        }

        public static ICollection<IModifyObject> GetIModifyObjects(ObjectType objectType)
        {
            Exception error;

            switch (objectType)
            {
                case ObjectType.Person:
                    return CgpClient.Singleton.MainServerProvider.Persons.ListModifyObjects(out error);
                case ObjectType.Calendar:
                    return CgpClient.Singleton.MainServerProvider.Calendars.ListModifyObjects(out error);
                case ObjectType.Card:
                    return CgpClient.Singleton.MainServerProvider.Cards.ListModifyObjects(out error);
                case ObjectType.CardSystem:
                    return CgpClient.Singleton.MainServerProvider.CardSystems.ListModifyObjects(out error);
                case ObjectType.CisNGGroup:
                    return CgpClient.Singleton.MainServerProvider.CisNGGroups.ListModifyObjects(out error);
                case ObjectType.CisNG:
                    return CgpClient.Singleton.MainServerProvider.CisNGs.ListModifyObjects(out error);
                case ObjectType.DailyPlan:
                    return CgpClient.Singleton.MainServerProvider.DailyPlans.ListModifyObjects(out error);
                case ObjectType.DayType:
                    return CgpClient.Singleton.MainServerProvider.DayTypes.ListModifyObjects(out error);
                case ObjectType.Login:
                    return CgpClient.Singleton.MainServerProvider.Logins.ListModifyObjects(out error);
                case ObjectType.LoginGroup:
                    return CgpClient.Singleton.MainServerProvider.LoginGroups.ListModifyObjects(out error);
                case ObjectType.PresentationFormatter:
                    return CgpClient.Singleton.MainServerProvider.PresentationFormatters.ListModifyObjects(out error);
                case ObjectType.PresentationGroup:
                    return CgpClient.Singleton.MainServerProvider.PresentationGroups.ListModifyObjects(out error);
                case ObjectType.TimeZone:
                    return CgpClient.Singleton.MainServerProvider.TimeZones.ListModifyObjects(out error);
                case ObjectType.GlobalAlarmInstruction:
                    return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.ListModifyObjects(out error);
            }

            foreach (CgpClientPluginDescriptor plugin in CgpClient.Singleton.PluginManager)
            {
               return plugin._plugin.GetIModifyObjects(objectType);
            }
            return null;
        }

        public static void TranslateDailyPlans(IList<IModifyObject> listDailyPlans)
        {
            if (listDailyPlans == null) return;
            foreach (IModifyObject mo in listDailyPlans)
            {
                var dailyPlanModifyObj = mo as DailyPlanModifyObj;

                if (dailyPlanModifyObj == null)
                    continue;

                switch (dailyPlanModifyObj.FullName)
                {
                    case DailyPlan.IMPLICIT_DP_ALWAYS_OFF:
                        dailyPlanModifyObj.FullName = CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                        break;
                    case DailyPlan.IMPLICIT_DP_ALWAYS_ON:
                        dailyPlanModifyObj.FullName = CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_ON);
                        break;
                    case DailyPlan.IMPLICIT_DP_DAYLIGHT_ON:
                        dailyPlanModifyObj.FullName = CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                        break;
                    case DailyPlan.IMPLICIT_DP_NIGHT_ON:
                        dailyPlanModifyObj.FullName = CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_NIGHT_ON);
                        break;
                }
            }
            listDailyPlans = listDailyPlans.OrderBy(dp => dp.ToString()).ToList();
        }

        public static void TranslateCalendars(IList<IModifyObject> listCalendars)
        {
            if (listCalendars == null) return;
            foreach (IModifyObject mo in listCalendars)
            {
                var calendarModifyObj = mo as CalendarModifyObj;

                if (calendarModifyObj != null)
                {
                    switch (mo.ToString())
                    {
                        case Calendar.IMPLICIT_CALENDAR_DEFAULT:
                            calendarModifyObj.FullName = CgpClient.Singleton.LocalizationHelper.GetString(Calendar.IMPLICIT_CALENDAR_DEFAULT);
                            break;
                    }
                }
            }
            listCalendars = listCalendars.OrderBy(calendar => calendar.ToString()).ToList();
        }

        public static void CloseEditedForm(ObjectType objType, object objId)
        {
            switch (objType)
            {
                case ObjectType.DailyPlan:
                    DailyPlansForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.TimeZone:
                    TimeZonesForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.Calendar:
                    CalendarsForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.DayType:
                    DayTypesForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.Card:
                    CardsForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.CardSystem:
                    CardSystemsForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.Person:
                    PersonsForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.LoginGroup:
                    LoginGroupsForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.Login:
                    LoginsForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.PresentationGroup:
                    PresentationGroupsForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.CisNGGroup:
                    CisNGGroupsForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.PresentationFormatter:
                    PresentationFormattersForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.CisNG:
                    CisNGsForm.Singleton.CloseEditedFormById(objId);
                    break;
                case ObjectType.GlobalAlarmInstruction:
                    GlobalAlarmInstructionsForm.Singleton.CloseEditedFormById(objId);
                    break;
                default:
                    break;
            }
        }
    }

    public interface IListObjects
    {
        bool Contains(string expression);
        AOrmObject GetOrmObj();
    }

    public class DailyPlanList : IListObjects
    {
        private DailyPlan _dailyPlan = null;

        public DailyPlan DailyPlan
        {
            get { return _dailyPlan; }
        }

        public bool Contains(string expression)
        {
            if (_dailyPlan.Contains(expression))
                return true;
            if (this.ToString().ToLower().Contains(expression.ToLower()))
                return true;
            return false;
        }

        public AOrmObject GetOrmObj()
        {
            return _dailyPlan;
        }

        public DailyPlanList(DailyPlan dailyPlan)
        {
            _dailyPlan = dailyPlan;
        }

        public override string ToString()
        {
            switch (_dailyPlan.DailyPlanName)
            {
                case DailyPlan.IMPLICIT_DP_ALWAYS_ON:
                    return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_ON);
                case DailyPlan.IMPLICIT_DP_ALWAYS_OFF:
                    return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                case DailyPlan.IMPLICIT_DP_DAYLIGHT_ON:
                    return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                case DailyPlan.IMPLICIT_DP_NIGHT_ON:
                    return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_NIGHT_ON);
                default:
                    return _dailyPlan.DailyPlanName;
            }
        }
    }


    public class ListObjFS : IListObjects
    {
        private AOrmObject _ormObj = null;

        public bool Contains(string expression)
        {
            if (_ormObj.Contains(expression))
                return true;
            if (this.ToString().ToLower().Contains(expression.ToLower()))
                return true;
            return false;
        }

        public AOrmObject GetOrmObj()
        {
            return _ormObj;
        }

        public ListObjFS(AOrmObject ormObj)
        {
            _ormObj = ormObj;
        }

        public override string ToString()
        {
            if (_ormObj.GetObjectType() == ObjectType.DailyPlan)
            {
                switch (_ormObj.ToString())
                {
                    case DailyPlan.IMPLICIT_DP_ALWAYS_ON:
                        return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_ON);
                    case DailyPlan.IMPLICIT_DP_ALWAYS_OFF:
                        return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_ALWAYS_OFF);
                    case DailyPlan.IMPLICIT_DP_DAYLIGHT_ON:
                        return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_DAYLIGHT_ON);
                    case DailyPlan.IMPLICIT_DP_NIGHT_ON:
                        return CgpClient.Singleton.LocalizationHelper.GetString(DailyPlan.IMPLICIT_DP_NIGHT_ON);
                    default:
                        return _ormObj.ToString();
                }
            }
            return _ormObj.ToString();
        }
    }
}
