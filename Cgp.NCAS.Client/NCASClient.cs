using System;
using System.Collections.Generic;
using System.Linq;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Client.Common;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.BaseLib;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Globals;
using System.Windows.Forms;

using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys.Microsoft;

using System.Drawing;
using Contal.IwQuick;
using Microsoft.Win32;
using ICgpEditForm = Contal.Cgp.Client.ICgpEditForm;

namespace Contal.Cgp.NCAS.Client
{
    public class NCASClient : ACgpVisualPlugin<NCASClient>
    {
        private readonly NCASClientCore _ncasClientCore =
            new NCASClientCore(
                CgpClient.Singleton,
                LocalizationHelper);

        public const bool INTER_CCU_COMMUNICATION = false;
        public const string MULTI_OBJECTS_STRING_KEY = "MultiObjects";

        private NCASClientForm _mainForm;

        private readonly List<PluginMainForm<NCASClient>> _listSubForms =
            new List<PluginMainForm<NCASClient>>();

        private readonly List<PluginMainForm<NCASClient>> _listHideSubForms =
            new List<PluginMainForm<NCASClient>>();

        private ImageList _objectImages;

        private Dictionary<ObjectType, Func<AOrmObject, string>> _namesLocalizationByObjectType;

        private Dictionary<ObjectType, Func<AOrmObject, string>> NamesLocalizationByObjectType
        {
            get
            {
                return _namesLocalizationByObjectType
                       ?? (_namesLocalizationByObjectType = new Dictionary<ObjectType, Func<AOrmObject, string>>
                       {
                           {
                               ObjectType.DevicesAlarmSettingAlarmArc,
                               DevicesAlarmSettingAlarmArcNameLocalization
                           },
                           {
                               ObjectType.AlarmAreaAlarmArc,
                               AlarmAreaAlarmArcNameLocalization
                           },
                           {
                               ObjectType.CardReaderAlarmArc,
                               CardReaderAlarmArcNameLocalization
                           },
                           {
                               ObjectType.CcuAlarmArc,
                               CcuAlarmArcNameLocalization
                           },
                           {
                               ObjectType.DcuAlarmArc,
                               DcuAlarmArcNameLocalization
                           },
                           {
                               ObjectType.DoorEnvironmentAlarmArc,
                               DoorEnvironmentAlarmArcNameLocalization
                           },
                           {
                               ObjectType.DevicesAlarmSetting,
                               DevicesAlarmSettingsNameLocalization
                           }
                       });
            }
        }

        private static bool EnabledSystemLanguage
        {
            get
            {
                RegistryKey registryKey;

                if (RegistryHelper.TryParseKey(CgpClientGlobals.RegPathClientSettings, true, out registryKey))
                {
                    try
                    {
                        var result = (int) registryKey.GetValue(CgpClientGlobals.CGP_ENABLED_SYSTEM_LANGUAGE);
                        return result == 1;
                    }
                    catch
                    {
                        return false;
                    }
                }

                return false;
            }
        }

        private static LocalizationHelper _localizationHelper;
        public static LocalizationHelper LocalizationHelper
        {
            get
            {
                return _localizationHelper
                    ?? (_localizationHelper = new LocalizationHelper(typeof(NCASClient).Assembly, EnabledSystemLanguage));
            }
        }

        public ImageList ObjectImages
        {
            get
            {
                if (_objectImages == null)
                    AddObjectsImages();

                return _objectImages;
            }
            set
            {
                _objectImages = value;
            }
        }

        public Image GetImageForListOfObject(ListOfObjects objects)
        {
            if (objects == null || objects.Count == 0)
                return null;

            Image image = null;

            if (objects.Count > 1)
            {
                if (IsSameType(objects))
                {
                    var aOrmObject = objects[0] as AOrmObject;

                    if (aOrmObject != null)
                        image = ObjectImages.Images[aOrmObject.GetObjectType()
                            .ToString()];
                }
                else
                    image = ObjectImages.Images[MULTI_OBJECTS_STRING_KEY];
            }
            else if (objects.Count == 1)
            {
                var aOrmObject = objects[0] as AOrmObject;

                if (aOrmObject != null)
                    image = ObjectImages.Images[aOrmObject.GetObjectType().ToString()];
            }

            return image;
        }

        private static bool IsSameType(ListOfObjects objects)
        {
            ObjectType type = ((AOrmObject)objects[0]).GetObjectType();

            return objects.Cast<AOrmObject>()
                .All(obj => obj.GetObjectType() == type);
        }

        public Image GetImageForObjectType(ObjectType objectType)
        {
            return
                ObjectImages.Images[objectType.ToString()]
                    ?? ObjectImageList.Singleton.GetImageForObjectType(objectType);
        }

        public Image GetImageForObjectType(string objectType)
        {
            return
                ObjectImages.Images[objectType]
                    ?? ObjectImageList.Singleton.GetImageForObjectType(objectType);
        }

        public Image GetImageForAOrmObject(AOrmObject obj)
        {
            if (obj == null)
                return null;

            return
                ObjectImages.Images[obj.GetObjectType().ToString()]
                    ?? ObjectImageList.Singleton.GetImageForAOrmObject(obj);
        }

        public override ImageList GetPluginObjectsImages()
        {
            return ObjectImages;
        }

        private void AddObjectsImages()
        {
            _objectImages = 
                new ImageList
                {
                    ColorDepth = ColorDepth.Depth32Bit
                };

            _objectImages.Images.Add(
                ObjectType.AccessControlList.ToString(), 
                ResourceGlobal.IconACL16);

            _objectImages.Images.Add(
                ObjectType.ACLGroup.ToString(), 
                ResourceGlobal.IconACLGroup16);

            _objectImages.Images.Add(
                ObjectType.AlarmArea.ToString(), 
                ResourceGlobal.IconAlarmAreas16);

            _objectImages.Images.Add(
                ObjectType.AntiPassBackZone.ToString(),
                ResourceGlobal.IconAntiPassBackZone16);

            _objectImages.Images.Add(
                ObjectType.CardReader.ToString(), 
                ResourceGlobal.IconCardreader16);

            _objectImages.Images.Add(
                ObjectType.LprCamera.ToString(),
                ResourceGlobal.IconLprCamera16);

            _objectImages.Images.Add(
                ObjectType.CCU.ToString(), 
                ResourceGlobal.ccu_16);

            _objectImages.Images.Add(
                ObjTypeHelper.CCUOffline,
                ResourceGlobal.ccu_offline_16);

            _objectImages.Images.Add(
                ObjectType.DCU.ToString(), 
                ResourceGlobal.dcu_16);

            _objectImages.Images.Add(
                ObjTypeHelper.DCUOffline,
                ResourceGlobal.dcu_offline_16);

            _objectImages.Images.Add(
                ObjectType.DoorEnvironment.ToString(),
                ResourceGlobal.IconDoorenvironment16);

            _objectImages.Images.Add(
                ObjectType.Input.ToString(), 
                ResourceGlobal.IconInputs16);

            _objectImages.Images.Add(
                ObjectType.Output.ToString(), 
                ResourceGlobal.IconOutputs16);

            _objectImages.Images.Add(
                ObjectType.SecurityDailyPlan.ToString(), 
                ResourceGlobal.IconSecurityDailyPLan16);

            _objectImages.Images.Add(
                ObjectType.SecurityTimeZone.ToString(),
                ResourceGlobal.IconSecurityTimeZone16);

            _objectImages.Images.Add(
                ObjTypeHelper.CardReaderBlocked, 
                ResourceGlobal.IconCardreaderBlocked_16);

            _objectImages.Images.Add(
                MULTI_OBJECTS_STRING_KEY, 
                ResourceGlobal.IconMultiObjects16);

            _objectImages.Images.Add(
                ObjectType.MultiDoor.ToString(),
                ResourceGlobal.IconMultiDoor16);

            _objectImages.Images.Add(
                ObjectType.MultiDoorElement.ToString(),
                ResourceGlobal.IconMultiDoorElement16);

            _objectImages.Images.Add(
                ObjectType.Floor.ToString(),
                ResourceGlobal.IconFloor16);

            _objectImages.Images.Add(
                ObjectType.AlarmTransmitter.ToString(),
                ResourceGlobal.IconCat16);

            _objectImages.Images.Add(
                ObjectType.AlarmArc.ToString(),
                ResourceGlobal.IconAlarmArc16);

            _objectImages.Images.Add(
                ObjectType.DevicesAlarmSettingAlarmArc.ToString(),
                ResourceGlobal.IconAlarmSettingsNew16);

            _objectImages.Images.Add(
                ObjectType.AlarmAreaAlarmArc.ToString(),
                ResourceGlobal.IconAlarmAreas16);


            _objectImages.Images.Add(
                ObjectType.DevicesAlarmSetting.ToString(),
                ResourceGlobal.IconAlarmSettingsNew16);

            _objectImages.Images.Add(
                ObjectType.CardReaderAlarmArc.ToString(),
                ResourceGlobal.IconCardreader16);

            _objectImages.Images.Add(
                ObjectType.CcuAlarmArc.ToString(),
                ResourceGlobal.ccu_16);

            _objectImages.Images.Add(
                ObjectType.DcuAlarmArc.ToString(),
                ResourceGlobal.dcu_16);

            _objectImages.Images.Add(
                ObjectType.DoorEnvironmentAlarmArc.ToString(),
                ResourceGlobal.IconDoorenvironment16);

            _objectImages.Images.Add(
                "Delete",
                ResourceGlobal.Delete);

            _objectImages.Images.Add(
                "Edit",
                ResourceGlobal.Edit);

            _objectImages.Images.Add(
                "Insert",
                ResourceGlobal.insert);
        }

        protected override NCASClient This { get { return this; } }

        protected override PluginMainForm<NCASClient> CreateMainForm()
        {
            return _mainForm ?? (_mainForm = new NCASClientForm());
        }

        public override void SetLanguage(string language)
        {
            _ncasClientCore.SetLanguage(language);
        }

        public override string GetTranslateString(string name, params object[] args)
        {
            return _ncasClientCore.GetTranslateString(name, args);
        }

        public override string GetTranslateTableObjectTypeName(string name)
        {
            return LocalizationHelper.GetString("ObjectType_" + name.Substring(0, name.Length - 1));
        }

        protected override IEnumerable<PluginMainForm<NCASClient>> CreateSubForms()
        {
            _listSubForms.Clear();
            _listSubForms.Add(NCASCardReadersForm.Singleton);
            _listSubForms.Add(NCASLprCamerasForm.Singleton);
            _listSubForms.Add(NCASCCUsForm.Singleton);
            _listSubForms.Add(NCASDCUsForm.Singleton);
            _listSubForms.Add(NCASInputsForm.Singleton);
            _listSubForms.Add(NCASOutputsForm.Singleton);
            _listSubForms.Add(NCASDoorEnvironmentsForm.Singleton);
            _listSubForms.Add(NCASAccessControlListsForm.Singleton);
            _listSubForms.Add(NCASACLGroupsForm.Singleton);
            _listSubForms.Add(NCASAlarmAreasForm.Singleton);
            _listSubForms.Add(NCASSecurityDailyPlansForm.Singleton);
            _listSubForms.Add(NCASSecurityTimeZonesForm.Singleton);
            //_listSubForms.Add(NCASExpressionsForm.Singleton);
            _listSubForms.Add(NCASDevicesAlarmSettingsForm.Singleton);
#if DEBUG
            _listSubForms.Add(NCASGraphicsViewsForm.Singleton);
#endif
            _listSubForms.Add(NCASScenesForm.Singleton);
            _listSubForms.Add(NCASAntiPassBackZonesForm.Singleton);
            _listSubForms.Add(NCASMultiDoorsForm.Singleton);
            _listSubForms.Add(NCASMultiDoorElementsForm.Singleton);
            _listSubForms.Add(NCASFloorsForm.Singleton);
            _listSubForms.Add(NCASAlarmTransmittersForm.Singleton);
            _listSubForms.Add(NCASAlarmArcsForm.Singleton);
            _listSubForms.Add(NCASTimetecSettingsForm.Singleton);
            return _listSubForms;
        }

        protected override IEnumerable<PluginMainForm<NCASClient>> CreateHideSubForms()
        {
            _listHideSubForms.Clear();
            //_listHideSubForms.Add(NCASInputsForm.Singleton);
            //_listHideSubForms.Add(NCASOutputsForm.Singleton);
            //_listHideSubForms.Add(NCASDoorEnvironmentForm.Singleton);
            return _listHideSubForms;
        }

        public override string FriendlyName
        {
            get { return "NCAS plugin"; }
        }

        public override string Description
        {
            get { return "NCAS"; }
        }

        public override string[] FriendPlugins
        {
            get { return null; }
        }

        private readonly ExtendedVersion _version =
            new ExtendedVersion(typeof(NCASClient), true,
#if !DEBUG
#if RELEASE_SPECIAL
                DevelopmentStage.Testing
#else
                DevelopmentStage.Release
#endif
#else
 DevelopmentStage.Testing
#endif
);

        public override ExtendedVersion Version
        {
            get
            {
                return _version;
            }
        }

        public override void OnDispose()
        {

        }

        public override void Initialize()
        {

        }

        public override Type GetRemotingProviderInterfaceType()
        {
            return _ncasClientCore.GetRemotingProviderInterfaceType();
        }

        public override void SetRemotingProviderInterface(object remotingProviderInterface)
        {
            _ncasClientCore.SetRemotingProviderInterface(remotingProviderInterface);
        }

        public override void PreRegisterAttachCallbackHandlers()
        {
            _ncasClientCore.PreRegisterAttachCallbackHandlers();
        }

        public ICgpNCASRemotingProvider MainServerProvider
        {
            get { return _ncasClientCore.MainServerProvider; }
        }

        public override Dictionary<string, List<AccessPresentation>> GetPluginAccessList()
        {
            return NCASAccess.GetAccessList();
        }

        public override bool LoadPluginControlToForm(
            object obj,
            Control control,
            IExtendedCgpEditForm cgpEditForm,
            bool allowEdit)
        {
            return LoadPluginControlToForm(
                obj,
                null,
                control,
                cgpEditForm,
                allowEdit);
        }

        public override bool LoadPluginControlToForm(
            object obj,
            Dictionary<string, bool> filter,
            Control control,
            IExtendedCgpEditForm cgpEditForm,
            bool allowEdit)
        {
            PluginMainForm<NCASClient> pluginMainForm = null;

            switch (control.Name)
            {
                case "_tpAddAccessControlList":

                    var person = obj as Person;

                    if (person == null)
                        return false;

                    if (filter != null && filter.ContainsKey("AccessControlList") &&
                        filter["AccessControlList"] == false)
                        person = new Person();

                    if (
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesView)) ||
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesAdmin)))
                    {
                        pluginMainForm =
                            new NCASACLPersonEditForm(
                                person,
                                control,
                                allowEdit);
                    }

                    break;

                case "_tpAccessZone":

                    person = obj as Person;

                    if (person == null)
                        return false;

                    if (filter != null && filter.ContainsKey("AccessZone") && filter["AccessZone"] == false)
                        person = new Person();

                    if (
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesView)) ||
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.PersonsAccessControlListsAccessZonesAdmin)))
                    {
                        pluginMainForm =
                            new NCASAccessZonePersonEditForm(
                                person,
                                control,
                                allowEdit);
                    }
                    break;

                case "_tpActualAccess":

                    person = obj as Person;

                    if (person == null)
                        return false;

                    if (
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.PersonsActualAccessListView)))
                    {
                        pluginMainForm =
                            new NCASActualListAccessedCR(
                                person,
                                control);
                    }
                    break;

                case "_tpSms":

                    if (
                        CgpClient.Singleton.MainServerProvider.HasAccess(
                            BaseAccess.GetAccess(LoginAccess.PersonsActualAccessListView)))
                    {
                        var pg = obj as PresentationGroup;

                        if (pg == null)
                            return false;

                        pluginMainForm =
                            new NCASCatSmsConfiguration(
                                control,
                                cgpEditForm,
                                pg);

                        pluginMainForm.Show();
                    }
                    break;
            }

            return pluginMainForm != null;
        }

        public override void SaveAfterInsertWithData(object obj, object clonedObj)
        {
            var person = obj as Person;

            if (person == null)
                return;

            var clonedPerson = clonedObj as Person;

            if (clonedPerson == null)
                return;

            NCASACLPersonEditForm.SaveAfterInsertWithData(
                this,
                person,
                clonedPerson);

            NCASAccessZonePersonEditForm.SaveAfterInsertWithData(
                this,
                person,
                clonedPerson);
        }

        public override string GetLocalizedObjectName(AOrmObject aOrmObject)
        {
            Func<AOrmObject, string> nameLocalization;

            if (!NamesLocalizationByObjectType.TryGetValue(
                aOrmObject.GetObjectType(),
                out nameLocalization))
            {
                return null;
            }

            return nameLocalization(aOrmObject);
        }

        private static string DevicesAlarmSettingAlarmArcNameLocalization(AOrmObject aOrmObject)
        {
            var devicesAlarmSettingAlarmArc = aOrmObject as DevicesAlarmSettingAlarmArc;

            if (devicesAlarmSettingAlarmArc == null)
                return null;

            var alarmType = (AlarmType) devicesAlarmSettingAlarmArc.AlarmType;

            return string.Format(
                "{0}: {1}",
                LocalizationHelper.GetString("NCASDevicesAlarmSettingsFormNCASDevicesAlarmSettingsForm"),
                LocalizationHelper.GetString(
                    alarmType == AlarmType.AlarmArea_Alarm
                        ? "NCASDeviceAlarmSettingsForm_chbAlarmReceptionCenterSettings"
                        : string.Format(
                            "AlarmType_{0}",
                            (AlarmType) devicesAlarmSettingAlarmArc.AlarmType)));
        }

        private static string AlarmAreaAlarmArcNameLocalization(AOrmObject aOrmObject)
        {
            var alarmAreaAlarmArc = aOrmObject as AlarmAreaAlarmArc;

            if (alarmAreaAlarmArc == null)
                return null;

            return string.Format(
                "{0}: {1}",
                alarmAreaAlarmArc.AlarmArea != null
                    ? alarmAreaAlarmArc.AlarmArea.ToString()
                    : string.Empty,
                LocalizationHelper.GetString(
                    string.Format(
                        "AlarmType_{0}",
                        (AlarmType) alarmAreaAlarmArc.AlarmType)));
        }

        private static string CardReaderAlarmArcNameLocalization(AOrmObject aOrmObject)
        {
            var cardReaderAlarmArc = aOrmObject as CardReaderAlarmArc;

            if (cardReaderAlarmArc == null)
                return null;

            return string.Format(
                "{0}: {1}",
                cardReaderAlarmArc.CardReader != null
                    ? cardReaderAlarmArc.CardReader.ToString()
                    : string.Empty,
                LocalizationHelper.GetString(
                    string.Format(
                        "AlarmType_{0}",
                        (AlarmType)cardReaderAlarmArc.AlarmType)));
        }

        private static string CcuAlarmArcNameLocalization(AOrmObject aOrmObject)
        {
            var ccuAlarmArc = aOrmObject as CcuAlarmArc;

            if (ccuAlarmArc == null)
                return null;

            return string.Format(
                "{0}: {1}",
                ccuAlarmArc.Ccu != null
                    ? ccuAlarmArc.Ccu.ToString()
                    : string.Empty,
                LocalizationHelper.GetString(
                    string.Format(
                        "AlarmType_{0}",
                        (AlarmType)ccuAlarmArc.AlarmType)));
        }

        private static string DcuAlarmArcNameLocalization(AOrmObject aOrmObject)
        {
            var dcuAlarmArc = aOrmObject as DcuAlarmArc;

            if (dcuAlarmArc == null)
                return null;

            return string.Format(
                "{0}: {1}",
                dcuAlarmArc.Dcu != null
                    ? dcuAlarmArc.Dcu.ToString()
                    : string.Empty,
                LocalizationHelper.GetString(
                    string.Format(
                        "AlarmType_{0}",
                        (AlarmType)dcuAlarmArc.AlarmType)));
        }

        private static string DoorEnvironmentAlarmArcNameLocalization(AOrmObject aOrmObject)
        {
            var doorEnvironmentAlarmArc = aOrmObject as DoorEnvironmentAlarmArc;

            if (doorEnvironmentAlarmArc == null)
                return null;

            return string.Format(
                "{0}: {1}",
                doorEnvironmentAlarmArc.DoorEnvironment != null
                    ? doorEnvironmentAlarmArc.DoorEnvironment.ToString()
                    : string.Empty,
                LocalizationHelper.GetString(
                    string.Format(
                        "AlarmType_{0}",
                        (AlarmType)doorEnvironmentAlarmArc.AlarmType)));
        }

        private static string DevicesAlarmSettingsNameLocalization(AOrmObject aOrmObject)
        {
            return LocalizationHelper.GetString("NCASDevicesAlarmSettingsFormNCASDevicesAlarmSettingsForm");
        }

        public override void OpenDBSEdit(AOrmObject dbObj)
        {
            if (dbObj == null) return;

            if (dbObj.GetObjectType() == ObjectType.CCU)
            {
                NCASCCUsForm.Singleton.OpenEditForm(dbObj as CCU);
            }
            else if (dbObj.GetObjectType() == ObjectType.DCU)
            {
                NCASDCUsForm.Singleton.OpenEditForm(dbObj as DCU);
            }
            else if (dbObj.GetObjectType() == ObjectType.AlarmArea)
            {
                NCASAlarmAreasForm.Singleton.OpenEditForm(dbObj as AlarmArea);
            }
            else if (dbObj.GetObjectType() == ObjectType.CardReader)
            {
                NCASCardReadersForm.Singleton.OpenEditForm(dbObj as CardReader);
            }
            else if (dbObj.GetObjectType() == ObjectType.LprCamera)
            {
                NCASLprCamerasForm.Singleton.Show();
            }
            else if (dbObj.GetObjectType() == ObjectType.Input)
            {
                NCASInputsForm.Singleton.OpenEditForm(dbObj as Input);
            }
            else if (dbObj.GetObjectType() == ObjectType.Output)
            {
                NCASOutputsForm.Singleton.OpenEditForm(dbObj as Output);
            }
            else if (dbObj.GetObjectType() == ObjectType.SecurityDailyPlan)
            {
                NCASSecurityDailyPlansForm.Singleton.OpenEditForm(dbObj as SecurityDailyPlan);
            }
            else if (dbObj.GetObjectType() == ObjectType.SecurityTimeZone)
            {
                NCASSecurityTimeZonesForm.Singleton.OpenEditForm(dbObj as SecurityTimeZone);
            }
            else if (dbObj.GetObjectType() == ObjectType.DoorEnvironment)
            {
                NCASDoorEnvironmentsForm.Singleton.OpenEditForm(dbObj as DoorEnvironment);
            }
            else if (dbObj.GetObjectType() == ObjectType.AccessControlList)
            {
                NCASAccessControlListsForm.Singleton.OpenEditForm(dbObj as AccessControlList);
            }
            else if (dbObj.GetObjectType() == ObjectType.ACLGroup)
            {
                NCASACLGroupsForm.Singleton.OpenEditForm(dbObj as ACLGroup);
            }
            else if (dbObj.GetObjectType() == ObjectType.DevicesAlarmSetting)
            {
                NCASDevicesAlarmSettingsForm.Singleton.Show();
            }
            else if (dbObj.GetObjectType() == ObjectType.Scene)
            {
                NCASScenesForm.Singleton.OpenEditForm(dbObj as Scene);
            }
            else if (dbObj.GetObjectType() == ObjectType.AntiPassBackZone)
            {
                NCASAntiPassBackZonesForm.Singleton.OpenEditForm(dbObj as AntiPassBackZone);
            }
            else if (dbObj.GetObjectType() == ObjectType.MultiDoor)
            {
                NCASMultiDoorsForm.Singleton.OpenEditForm(dbObj as MultiDoor);
            }
            else if (dbObj.GetObjectType() == ObjectType.MultiDoorElement)
            {
                NCASMultiDoorElementsForm.Singleton.OpenEditForm(dbObj as MultiDoorElement);
            }
            else if (dbObj.GetObjectType() == ObjectType.Floor)
            {
                NCASFloorsForm.Singleton.OpenEditForm(dbObj as Floor);
            }
            else if (dbObj.GetObjectType() == ObjectType.AlarmTransmitter)
            {
                NCASAlarmTransmittersForm.Singleton.OpenEditForm(dbObj as AlarmTransmitter);
            }
            else if (dbObj.GetObjectType() == ObjectType.AlarmArc)
            {
                NCASAlarmArcsForm.Singleton.OpenEditForm(dbObj as AlarmArc);
            }
            else if (dbObj.GetObjectType() == ObjectType.DevicesAlarmSettingAlarmArc)
            {
                var devicesAlarmSettingAlarmArc = dbObj as DevicesAlarmSettingAlarmArc;

                if (devicesAlarmSettingAlarmArc == null)
                    return;

                NCASDevicesAlarmSettingsForm.Singleton.Show((AlarmType) devicesAlarmSettingAlarmArc.AlarmType);
            }
            else if (dbObj.GetObjectType() == ObjectType.AlarmAreaAlarmArc)
            {
                var alarmAreaAlarmArc = dbObj as AlarmAreaAlarmArc;

                if (alarmAreaAlarmArc == null
                    || alarmAreaAlarmArc.AlarmArea == null)
                {
                    return;
                }

                NCASAlarmAreasForm.Singleton.OpenEditForm(alarmAreaAlarmArc.AlarmArea);
            }
            else if (dbObj.GetObjectType() == ObjectType.CardReaderAlarmArc)
            {
                var cardReaderAlarmArc = dbObj as CardReaderAlarmArc;

                if (cardReaderAlarmArc == null
                    || cardReaderAlarmArc.CardReader == null)
                {
                    return;
                }

                NCASCardReadersForm.Singleton.OpenEditForm(cardReaderAlarmArc.CardReader);
            }
            else if (dbObj.GetObjectType() == ObjectType.CcuAlarmArc)
            {
                var ccuAlarmArc = dbObj as CcuAlarmArc;

                if (ccuAlarmArc == null
                    || ccuAlarmArc.Ccu == null)
                {
                    return;
                }

                NCASCCUsForm.Singleton.OpenEditForm(ccuAlarmArc.Ccu);
            }
            else if (dbObj.GetObjectType() == ObjectType.DcuAlarmArc)
            {
                var dcuAlarmArc = dbObj as DcuAlarmArc;

                if (dcuAlarmArc == null
                    || dcuAlarmArc.Dcu == null)
                {
                    return;
                }

                NCASDCUsForm.Singleton.OpenEditForm(dcuAlarmArc.Dcu);
            }
            else if (dbObj.GetObjectType() == ObjectType.DoorEnvironmentAlarmArc)
            {
                var doorEnvironmentAlarmArc = dbObj as DoorEnvironmentAlarmArc;

                if (doorEnvironmentAlarmArc == null
                    || doorEnvironmentAlarmArc.DoorEnvironment == null)
                {
                    return;
                }

                NCASDoorEnvironmentsForm.Singleton.OpenEditForm(doorEnvironmentAlarmArc.DoorEnvironment);
            }
        }

        public override void OpenDBSInsert(string strObjectTableType, Action<object> doAfterInsert)
        {
            if (strObjectTableType == typeof(CCU).Name + "s")
            {
                var dbObj = new CCU();
                NCASCCUsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(DCU).Name + "s")
            {
                var dbObj =
                    new DCU
                    {
                        EnableParentInFullName = MainServerProvider.GetEnableParentInFullName()
                    };

                NCASDCUsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(AlarmArea).Name + "s")
            {
                var dbObj = new AlarmArea();
                NCASAlarmAreasForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(CardReader).Name + "s")
            {
                var dbObj =
                    new CardReader
                    {
                        EnableParentInFullName = MainServerProvider.GetEnableParentInFullName()
                    };

                NCASCardReadersForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(Input).Name + "s")
            {
                var dbObj =
                    new Input
                    {
                        EnableParentInFullName = MainServerProvider.GetEnableParentInFullName()
                    };

                NCASInputsForm.Singleton.OpenInsertFromEdit(ref dbObj, doAfterInsert);
            }
            else if (strObjectTableType == typeof(Output).Name + "s")
            {
                var dbObj =
                    new Output
                    {
                        EnableParentInFullName = MainServerProvider.GetEnableParentInFullName()
                    };

                NCASOutputsForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(SecurityDailyPlan).Name + "s")
            {
                var dbObj = new SecurityDailyPlan();

                NCASSecurityDailyPlansForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(SecurityTimeZone).Name + "s")
            {
                var dbObj = new SecurityTimeZone();

                NCASSecurityTimeZonesForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(DoorEnvironment).Name + "s")
            {
                var dbObj = new DoorEnvironment();

                NCASDoorEnvironmentsForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(AccessControlList).Name + "s")
            {
                var dbObj = new AccessControlList();

                NCASAccessControlListsForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(ACLGroup).Name + "s")
            {
                var dbObj = new ACLGroup();

                NCASACLGroupsForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(AntiPassBackZone).Name + "s")
            {
                var dbObj = new AntiPassBackZone();

                NCASAntiPassBackZonesForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(MultiDoor).Name + "s")
            {
                var dbObj = new MultiDoor();

                NCASMultiDoorsForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(MultiDoorElement).Name + "s")
            {
                var dbObj = new MultiDoorElement();

                NCASMultiDoorElementsForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(Floor).Name + "s")
            {
                var dbObj = new Floor();

                NCASFloorsForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(AlarmTransmitter).Name + "s")
            {
                var dbObj = new AlarmTransmitter();

                NCASAlarmTransmittersForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
            else if (strObjectTableType == typeof(AlarmArc).Name + "s")
            {
                var dbObj = new AlarmArc();

                NCASAlarmArcsForm.Singleton.OpenInsertFromEdit(
                    ref dbObj,
                    doAfterInsert);
            }
        }

        public override Icon GetIconForObjectType(ObjectType objectType)
        {
            if (objectType == ObjectType.CCU)
                return NCASCCUsForm.Singleton.Icon;

            if (objectType == ObjectType.DCU)
                return NCASDCUsForm.Singleton.Icon;

            if (objectType == ObjectType.AlarmArea)
                return NCASAlarmAreasForm.Singleton.Icon;

            if (objectType == ObjectType.CardReader)
                return NCASCardReadersForm.Singleton.Icon;

            if (objectType == ObjectType.LprCamera)
                return NCASLprCamerasForm.Singleton.Icon;

            if (objectType == ObjectType.Input)
                return NCASInputsForm.Singleton.Icon;

            if (objectType == ObjectType.Output)
                return NCASOutputsForm.Singleton.Icon;

            if (objectType == ObjectType.SecurityDailyPlan)
                return NCASSecurityDailyPlansForm.Singleton.Icon;

            if (objectType == ObjectType.SecurityTimeZone)
                return NCASSecurityTimeZonesForm.Singleton.Icon;

            if (objectType == ObjectType.DoorEnvironment)
                return NCASDoorEnvironmentsForm.Singleton.Icon;

            if (objectType == ObjectType.AccessControlList)
                return NCASAccessControlListsForm.Singleton.Icon;

            if (objectType == ObjectType.ACLGroup)
                return NCASACLGroupsForm.Singleton.Icon;

            if (objectType == ObjectType.DevicesAlarmSetting)
                return NCASDevicesAlarmSettingsForm.Singleton.Icon;

            if (objectType == ObjectType.AntiPassBackZone)
                return NCASAntiPassBackZonesForm.Singleton.Icon;

            if (objectType == ObjectType.Scene)
                return NCASScenesForm.Singleton.Icon;

            if (objectType == ObjectType.MultiDoor)
                return NCASMultiDoorsForm.Singleton.Icon;

            if (objectType == ObjectType.MultiDoorElement)
                return NCASMultiDoorElementsForm.Singleton.Icon;

            if (objectType == ObjectType.Floor)
                return NCASFloorsForm.Singleton.Icon;

            if (objectType == ObjectType.AlarmTransmitter)
                return NCASAlarmTransmittersForm.Singleton.Icon;

            if (objectType == ObjectType.AlarmArc)
                return NCASFloorsForm.Singleton.Icon;

            if (objectType == ObjectType.DevicesAlarmSettingAlarmArc)
                return NCASDevicesAlarmSettingsForm.Singleton.Icon;

            if (objectType == ObjectType.AlarmAreaAlarmArc)
                return NCASAlarmAreasForm.Singleton.Icon;

            if (objectType == ObjectType.CardReaderAlarmArc)
                return NCASCardReadersForm.Singleton.Icon;

            if (objectType == ObjectType.CcuAlarmArc)
                return NCASCCUsForm.Singleton.Icon;

            if (objectType == ObjectType.DcuAlarmArc)
                return NCASDCUsForm.Singleton.Icon;

            if (objectType == ObjectType.DoorEnvironmentAlarmArc)
                return NCASDoorEnvironmentsForm.Singleton.Icon;

            return null;
        }

        public override ICollection<IModifyObject> GetIModifyObjects(ObjectType objectType)
        {
            return _ncasClientCore.GetIModifyObjects(objectType);
        }

        public override Dictionary<string, bool> GetDefaultCloneFilterForObjectType(ObjectType objectType)
        {
            switch (objectType)
            {
                case ObjectType.Person:
                    var result = new Dictionary<string, bool>
                    {
                        {"AccessControlList", true},
                        {"AccessZone", true}
                    };

                    return result;

                default:
                    return null;
            }
        }

        public override void LoadAndShowOpenedForms(out Form selectedForm)
        {
            selectedForm = null;

            if (CgpClient.Singleton.MainServerProvider == null ||
                CgpClient.Singleton.IsConnectionLost(false))
            {
                return;
            }

            ICollection<UserOpenedWindow> userOpenedWindows =
                CgpClient.Singleton.MainServerProvider.GetUserOpenedWindows();

            if (userOpenedWindows == null)
                return;

            foreach (UserOpenedWindow openedWindow in userOpenedWindows.OrderBy(x => x.WindowIndex))
            {
                try
                {
                    Form formToOpen = null;
                    //classic forms
                    if (openedWindow.FormName == typeof(NCASAccessControlListsForm).Name)
                    {
                        formToOpen = NCASAccessControlListsForm.Singleton;
                    }
                    if (openedWindow.FormName == typeof(NCASACLGroupsForm).Name)
                    {
                        formToOpen = NCASACLGroupsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASAlarmAreasForm).Name)
                    {
                        formToOpen = NCASAlarmAreasForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASCardReadersForm).Name)
                    {
                        formToOpen = NCASCardReadersForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASCCUsForm).Name)
                    {
                        formToOpen = NCASCCUsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASDCUsForm).Name)
                    {
                        formToOpen = NCASDCUsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASDevicesAlarmSettingsForm).Name)
                    {
                        formToOpen = NCASDevicesAlarmSettingsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASDoorEnvironmentsForm).Name)
                    {
                        formToOpen = NCASDoorEnvironmentsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASInputsForm).Name)
                    {
                        formToOpen = NCASInputsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASOutputsForm).Name)
                    {
                        formToOpen = NCASOutputsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASSecurityDailyPlansForm).Name)
                    {
                        formToOpen = NCASSecurityDailyPlansForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASSecurityTimeZonesForm).Name)
                    {
                        formToOpen = NCASSecurityTimeZonesForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof (NCASScenesForm).Name)
                    {
                        formToOpen = NCASScenesForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASAntiPassBackZonesForm).Name)
                    {
                        formToOpen = NCASAntiPassBackZonesForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASMultiDoorsForm).Name)
                    {
                        formToOpen = NCASMultiDoorsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASMultiDoorElementsForm).Name)
                    {
                        formToOpen = NCASMultiDoorElementsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASFloorsForm).Name)
                    {
                        formToOpen = NCASFloorsForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASAlarmTransmittersForm).Name)
                    {
                        formToOpen = NCASAlarmTransmittersForm.Singleton;
                    }
                    else if (openedWindow.FormName == typeof(NCASAlarmArcsForm).Name)
                    {
                        formToOpen = NCASAlarmArcsForm.Singleton;
                    }
                    //edit forms
                    else
                    {
                        bool editAllowed;
                        AOrmObject objForEdit;

                        if (openedWindow.FormName == typeof (NCASAccessControlListEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.AccessControlLists.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen =
                                        NCASAccessControlListsForm.Singleton.OpenEditForm(
                                            objForEdit as AccessControlList, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new NCASAccessControlListEditForm(new AccessControlList(),
                                        (ShowOptionsEditForm) (openedWindow.ShowOption),
                                        NCASAccessControlListsForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof (NCASACLGruopEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.AclGroups.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = NCASACLGroupsForm.Singleton.OpenEditForm(objForEdit as ACLGroup,
                                        editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new NCASACLGruopEditForm(new ACLGroup(),
                                        (ShowOptionsEditForm) (openedWindow.ShowOption), NCASACLGroupsForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof (NCASAlarmAreaEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.AlarmAreas.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = NCASAlarmAreasForm.Singleton.OpenEditForm(objForEdit as AlarmArea,
                                        editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new NCASAlarmAreaEditForm(new AlarmArea(),
                                        (ShowOptionsEditForm) (openedWindow.ShowOption), NCASAlarmAreasForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof (NCASCardReaderEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.CardReaders.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = NCASCardReadersForm.Singleton.OpenEditForm(objForEdit as CardReader,
                                        editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    var newCr =
                                        new CardReader
                                        {
                                            EnableParentInFullName =
                                                MainServerProvider.GetEnableParentInFullName()
                                        };

                                    formToOpen = new NCASCardReaderEditForm(newCr,
                                        (ShowOptionsEditForm) (openedWindow.ShowOption), NCASCardReadersForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(NCASAntiPassBackZoneEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:

                                    objForEdit =
                                        MainServerProvider.AntiPassBackZones.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), 
                                            out editAllowed);

                                    if (objForEdit == null)
                                        continue;

                                    formToOpen = 
                                        NCASAntiPassBackZonesForm.Singleton.OpenEditForm(
                                            objForEdit as AntiPassBackZone,
                                            editAllowed, 
                                            false);

                                    break;

                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:

                                    formToOpen = 
                                        new NCASAntiPassBackZoneEditForm(
                                            new AntiPassBackZone(),
                                            (ShowOptionsEditForm)(openedWindow.ShowOption),
                                            NCASAntiPassBackZonesForm.Singleton);

                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(NCASCCUEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.CCUs.GetObjectForEditById(new Guid(openedWindow.ObjectId),
                                            out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = NCASCCUsForm.Singleton.OpenEditForm(objForEdit as CCU, editAllowed,
                                        false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new NCASCCUEditForm(new CCU(),
                                        (ShowOptionsEditForm) (openedWindow.ShowOption), NCASCCUsForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof (NCASDCUEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.DCUs.GetObjectForEditById(new Guid(openedWindow.ObjectId),
                                            out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = NCASDCUsForm.Singleton.OpenEditForm(objForEdit as DCU, editAllowed,
                                        false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new NCASDCUEditForm(new DCU(),
                                        (ShowOptionsEditForm) (openedWindow.ShowOption), NCASDCUsForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof (NCASDoorEnvironmentEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.DoorEnvironments.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen =
                                        NCASDoorEnvironmentsForm.Singleton.OpenEditForm(objForEdit as DoorEnvironment,
                                            editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new NCASDoorEnvironmentEditForm(new DoorEnvironment(),
                                        (ShowOptionsEditForm) (openedWindow.ShowOption),
                                        NCASDoorEnvironmentsForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof (NCASInputEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.Inputs.GetObjectForEditById(new Guid(openedWindow.ObjectId),
                                            out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = NCASInputsForm.Singleton.OpenEditForm(objForEdit as Input, editAllowed,
                                        false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    var newInput =
                                        new Input
                                        {
                                            EnableParentInFullName =
                                                MainServerProvider.GetEnableParentInFullName()
                                        };

                                    formToOpen = new NCASInputEditForm(newInput,
                                        (ShowOptionsEditForm) (openedWindow.ShowOption), NCASInputsForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof (NCASOutputEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.Outputs.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen = NCASOutputsForm.Singleton.OpenEditForm(objForEdit as Output,
                                        editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    var newOutput =
                                        new Output
                                        {
                                            EnableParentInFullName =
                                                MainServerProvider.GetEnableParentInFullName()
                                        };

                                    formToOpen = new NCASOutputEditForm(newOutput,
                                        (ShowOptionsEditForm) (openedWindow.ShowOption), NCASOutputsForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof (NCASSecurityDailyPlanEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.SecurityDailyPlans.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen =
                                        NCASSecurityDailyPlansForm.Singleton.OpenEditForm(
                                            objForEdit as SecurityDailyPlan, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new NCASSecurityDailyPlanEditForm(new SecurityDailyPlan(),
                                        (ShowOptionsEditForm) (openedWindow.ShowOption),
                                        NCASSecurityDailyPlansForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof (NCASSecurityTimeZoneEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm) (openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.SecurityTimeZones.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;
                                    formToOpen =
                                        NCASSecurityTimeZonesForm.Singleton.OpenEditForm(
                                            objForEdit as SecurityTimeZone, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new NCASSecurityTimeZoneEditForm(new SecurityTimeZone(),
                                        (ShowOptionsEditForm) (openedWindow.ShowOption),
                                        NCASSecurityTimeZonesForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(NCASSceneEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:
                                    objForEdit =
                                        MainServerProvider.Scenes.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId), out editAllowed);
                                    if (objForEdit == null)
                                        continue;

                                    if (openedWindow.ShowOption == (int)ShowOptionsEditForm.View)
                                        editAllowed = false;

                                    formToOpen =
                                        NCASScenesForm.Singleton.OpenEditForm(
                                            objForEdit as Scene, editAllowed, false);
                                    break;
                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:
                                    formToOpen = new NCASSceneEditForm(new Scene(),
                                        (ShowOptionsEditForm)(openedWindow.ShowOption),
                                        NCASScenesForm.Singleton);
                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(NCASMultiDoorEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:

                                    objForEdit =
                                        MainServerProvider.MultiDoors.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId),
                                            out editAllowed);

                                    if (objForEdit == null)
                                        continue;

                                    formToOpen =
                                        NCASMultiDoorsForm.Singleton.OpenEditForm(
                                            objForEdit as MultiDoor,
                                            editAllowed,
                                            false);

                                    break;

                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:

                                    formToOpen =
                                        new NCASMultiDoorEditForm(
                                            new MultiDoor(),
                                            (ShowOptionsEditForm)(openedWindow.ShowOption),
                                            NCASMultiDoorsForm.Singleton);

                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(NCASMultiDoorElementEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:

                                    objForEdit =
                                        MainServerProvider.MultiDoorElements.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId),
                                            out editAllowed);

                                    if (objForEdit == null)
                                        continue;

                                    formToOpen =
                                        NCASMultiDoorElementsForm.Singleton.OpenEditForm(
                                            objForEdit as MultiDoorElement,
                                            editAllowed,
                                            false);

                                    break;

                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:

                                    formToOpen =
                                        new NCASMultiDoorElementEditForm(
                                            new MultiDoorElement(),
                                            (ShowOptionsEditForm)(openedWindow.ShowOption),
                                            NCASMultiDoorElementsForm.Singleton);

                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(NCASFloorEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:

                                    objForEdit =
                                        MainServerProvider.Floors.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId),
                                            out editAllowed);

                                    if (objForEdit == null)
                                        continue;

                                    formToOpen =
                                        NCASFloorsForm.Singleton.OpenEditForm(
                                            objForEdit as Floor,
                                            editAllowed,
                                            false);

                                    break;

                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:

                                    formToOpen =
                                        new NCASFloorEditForm(
                                            new Floor(),
                                            (ShowOptionsEditForm)(openedWindow.ShowOption),
                                            NCASFloorsForm.Singleton);

                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(NCASAlarmTransmitterEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:

                                    objForEdit =
                                        MainServerProvider.AlarmTransmitters.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId),
                                            out editAllowed);

                                    if (objForEdit == null)
                                        continue;

                                    formToOpen =
                                        NCASAlarmTransmittersForm.Singleton.OpenEditForm(
                                            objForEdit as AlarmTransmitter,
                                            editAllowed,
                                            false);

                                    break;

                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:

                                    formToOpen =
                                        new NCASAlarmTransmitterEditForm(
                                            new AlarmTransmitter(), 
                                            (ShowOptionsEditForm)(openedWindow.ShowOption),
                                            NCASAlarmTransmittersForm.Singleton);

                                    break;
                            }
                        }
                        else if (openedWindow.FormName == typeof(NCASAlarmArcEditForm).Name)
                        {
                            switch ((ShowOptionsEditForm)(openedWindow.ShowOption))
                            {
                                case ShowOptionsEditForm.View:
                                case ShowOptionsEditForm.Edit:

                                    objForEdit =
                                        MainServerProvider.AlarmArcs.GetObjectForEditById(
                                            new Guid(openedWindow.ObjectId),
                                            out editAllowed);

                                    if (objForEdit == null)
                                        continue;

                                    formToOpen =
                                        NCASAlarmArcsForm.Singleton.OpenEditForm(
                                            objForEdit as AlarmArc,
                                            editAllowed,
                                            false);

                                    break;

                                case ShowOptionsEditForm.InsertDialog:
                                case ShowOptionsEditForm.InsertWithData:
                                case ShowOptionsEditForm.Insert:

                                    formToOpen =
                                        new NCASAlarmArcEditForm(
                                            new AlarmArc(),
                                            (ShowOptionsEditForm)(openedWindow.ShowOption),
                                            NCASAlarmArcsForm.Singleton);

                                    break;
                            }
                        }
                    }

                    if (formToOpen == null)
                        continue;

                    var icgpTableForm = formToOpen as ICgpTableForm;
                    if (icgpTableForm != null && !icgpTableForm.HasAccessView())
                        continue;

                    if (openedWindow.HasParent == false)
                    {
                        DllUser32.SendMessage(CgpClientMainForm.Singleton.Handle, (uint)CgpClientMainForm.Singleton.WM_SETREDRAW, 0, 0);
                        CgpClientMainForm.SetVisualFormParameters(formToOpen, openedWindow);
                        selectedForm = openedWindow.Selected ? formToOpen : selectedForm;
                        DllUser32.SendMessage(CgpClientMainForm.Singleton.Handle, (uint)CgpClientMainForm.Singleton.WM_SETREDRAW, 1, 0);
                        CgpClientMainForm.Singleton.Refresh();
                    }
                    else
                    {
                        formToOpen.Text = LocalizationHelper.GetString(formToOpen.Name + formToOpen.Name);
                        var icgpEditForm = formToOpen as ICgpEditForm;

                        if (icgpEditForm != null)
                        {
                            object editingObject = icgpEditForm.GetEditingObject();
                            CgpClientMainForm.Singleton.AddToOpenWindows(formToOpen, editingObject == null ? "" : editingObject.ToString());
                        }
                        else
                            CgpClientMainForm.Singleton.AddToOpenWindows(formToOpen);

                        CgpClientMainForm.SetVisualFormParameters(formToOpen, openedWindow);
                        formToOpen.Visible = false;
                        selectedForm = openedWindow.Selected ? formToOpen : selectedForm;
                    }
                }
                catch { }
            }
        }

        public override void RestartCardReaderCommunication()
        {
            _ncasClientCore.RestartCardReaderCommunication();
        }

        public override void SendCardReaderCommand(CardReaderSceneType crCommanad)
        {
            _ncasClientCore.SendCardReaderCommand(crCommanad);
        }

        public void AddToRecentList(object dbObj)
        {
            if (dbObj == null) return;

            if (dbObj.GetType() == typeof(CCU))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASCCUsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(DCU))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASDCUsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(AlarmArea))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASAlarmAreasForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(CardReader))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASCardReadersForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(Input))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASInputsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(Output))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASOutputsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(SecurityDailyPlan))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASSecurityDailyPlansForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(SecurityTimeZone))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASSecurityTimeZonesForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(DoorEnvironment))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASDoorEnvironmentsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(AccessControlList))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASAccessControlListsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(ACLGroup))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASACLGroupsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(DevicesAlarmSetting))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASDevicesAlarmSettingsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(AntiPassBackZone))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASAntiPassBackZonesForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(MultiDoor))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASMultiDoorsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(MultiDoorElement))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASMultiDoorElementsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(Floor))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASFloorsForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(AlarmTransmitter))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASAlarmTransmittersForm.Singleton, true);
            }
            else if (dbObj.GetType() == typeof(AlarmArc))
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj, NCASAlarmArcsForm.Singleton, true);
            }
            else
            {
                CgpClientMainForm.Singleton.AddToRecentList(dbObj);
            }
        }

        public override IList<AlarmType> GetPluginAlarmTypes()
        {
            return _ncasClientCore.GetPluginAlarmTypes();
        }

        public override IPluginMainForm GetEditPluginForm()
        {
            PluginMainForm<NCASClient> result = null;
            try
            {
                result = new PersonAclAssignment();
            }
            catch { }
            return result;
        }
    }
}
