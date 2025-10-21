using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public sealed class ObjectImageList : ASingleton<ObjectImageList>
    {
        public const string ALARM_INSTRUCTION_STRING_KEY = "GlobalAlarmInstruction";

        private ImageList _clientObjectImages;

        private ObjectImageList() : base(null)
        {
        }

        public ImageList ClientObjectImages
        {
            get
            {
                if (_clientObjectImages == null)
                    AddObjectsImages();

                return _clientObjectImages;
            }

            set
            {
                _clientObjectImages = value;
            }
        }

        public Image GetImageForObjectType(ObjectType objectType)
        {
            return ClientObjectImages.Images[objectType.ToString()];
        }

        public Image GetImageForObjectType(string objectType)
        {
            return ClientObjectImages.Images[objectType];
        }

        public Image GetImageForAOrmObject(AOrmObject obj)
        {
            return
                obj != null
                    ? ClientObjectImages.Images[obj.GetObjectType().ToString()]
                    : null;
        }

        public ImageList GetClientObjectsImages()
        {
            return ClientObjectImages;
        }

        public ImageList GetPluginImages(string pluginFriendlyName)
        {
            return
                CgpClient.Singleton.PluginManager.GetVisualPlugins()
                    .Where(item => item.FriendlyName == pluginFriendlyName)
                    .Select(item => item.GetPluginObjectsImages())
                    .FirstOrDefault();
        }

        private ImageList _allObjectImages;

        //get client and all plugin objects images
        public ImageList GetAllObjectImages()
        {
            if (_allObjectImages != null)
                return _allObjectImages;

            _allObjectImages = new ImageList();
            _allObjectImages.ColorDepth = ColorDepth.Depth32Bit;

            ImageList tempImages = ClientObjectImages;
            tempImages.ColorDepth = ColorDepth.Depth32Bit;

            foreach (string key in _clientObjectImages.Images.Keys)
                _allObjectImages.Images.Add(
                    key,
                    _clientObjectImages.Images[key]);

            foreach (ICgpVisualPlugin item in CgpClient.Singleton.PluginManager.GetVisualPlugins())
            {
                tempImages = item.GetPluginObjectsImages();

                foreach (string key in tempImages.Images.Keys)
                    _allObjectImages.Images.Add(key, tempImages.Images[key]);
            }

            return _allObjectImages;
        }

        //fill with client and all plugin objects images
        public void FillWithClientAndPluginsObjectImages(ImageList imageList)
        {
            ImageList tempImages = GetClientObjectsImages();

            foreach (string key in _clientObjectImages.Images.Keys)
                imageList.Images.Add(
                    key,
                    _clientObjectImages.Images[key]);

            foreach (ICgpVisualPlugin item in CgpClient.Singleton.PluginManager.GetVisualPlugins())
            {
                tempImages = item.GetPluginObjectsImages();

                foreach (string key in tempImages.Images.Keys)
                    imageList.Images.Add(
                        key,
                        tempImages.Images[key]);
            }
        }

        private void AddObjectsImages()
        {
            _clientObjectImages = new ImageList();
            _clientObjectImages.ImageSize = new Size(18, 18);
            _clientObjectImages.ColorDepth = ColorDepth.Depth32Bit;

            _clientObjectImages.Images.Add(ObjectType.Scene.ToString(), ResourceGlobal.Scenes16);
            _clientObjectImages.Images.Add(ObjectType.Calendar.ToString(), ResourceGlobal.IconCalendar16);
            _clientObjectImages.Images.Add(ObjectType.Card.ToString(), ResourceGlobal.IconCardsNew16);
            _clientObjectImages.Images.Add(CardState.Active.ToString(), ResourceGlobal.CardBlank128);
            _clientObjectImages.Images.Add(CardState.Blocked.ToString(), ResourceGlobal.CardNewNO_128);
            _clientObjectImages.Images.Add(CardState.Lost.ToString(), ResourceGlobal.CardNewNO_128);
            _clientObjectImages.Images.Add(CardState.Unused.ToString(), ResourceGlobal.CardNewUNKNOWN_128);
            _clientObjectImages.Images.Add(CardState.Destroyed.ToString(), ResourceGlobal.CardNewNO_128);
            _clientObjectImages.Images.Add(CardState.TemporarilyBlocked.ToString(), ResourceGlobal.CardNewNO_128);
            _clientObjectImages.Images.Add(CardState.HybridActive.ToString(), ResourceGlobal.HybridCardBlank128);
            _clientObjectImages.Images.Add(CardState.HybridBlocked.ToString(), ResourceGlobal.HybridCardNewNO_128);
            _clientObjectImages.Images.Add(CardState.HybridLost.ToString(), ResourceGlobal.HybridCardNewNO_128);
            _clientObjectImages.Images.Add(CardState.HybridUnused.ToString(), ResourceGlobal.HybridCardNewUNKNOWN_128);
            _clientObjectImages.Images.Add(CardState.HybridDestroyed.ToString(), ResourceGlobal.HybridCardNewNO_128);
            _clientObjectImages.Images.Add(CardState.HybridTemporarilyBlocked.ToString(), ResourceGlobal.HybridCardNewNO_128);
            //   blocked = 1,
            _clientObjectImages.Images.Add(ObjectType.CardSystem.ToString(), ResourceGlobal.IconCardSystemNew16);
            _clientObjectImages.Images.Add(ObjectType.CisNG.ToString(), ResourceGlobal.IconCisNG16);
            _clientObjectImages.Images.Add(ObjectType.CisNGGroup.ToString(), ResourceGlobal.IconCisNGGrpup16);
            _clientObjectImages.Images.Add(ObjectType.Car.ToString(), ResourceGlobal.Car16);
            _clientObjectImages.Images.Add(ObjectType.DailyPlan.ToString(), ResourceGlobal.IconDailyPLan16);
            _clientObjectImages.Images.Add(ObjectType.DayType.ToString(), ResourceGlobal.IconDayType16);
            _clientObjectImages.Images.Add(ObjectType.Eventlog.ToString(), ResourceGlobal.EventLogsNew16);
            _clientObjectImages.Images.Add(ObjectType.Login.ToString(), ResourceGlobal.IconLogins16);
            _clientObjectImages.Images.Add(ObjectType.LoginGroup.ToString(), ResourceGlobal.IconLoginGroup16);
            _clientObjectImages.Images.Add(ObjectType.Person.ToString(), ResourceGlobal.IconPersonsNew16);
            _clientObjectImages.Images.Add(ObjectType.PresentationFormatter.ToString(), ResourceGlobal.IconFormater16);
            _clientObjectImages.Images.Add(ObjectType.PresentationGroup.ToString(), ResourceGlobal.IconNewPresentationGroup16);
            _clientObjectImages.Images.Add(ObjectType.SystemEvent.ToString(), ResourceGlobal.SystemEvents16);
            _clientObjectImages.Images.Add(ObjectType.TimeZone.ToString(), ResourceGlobal.IconTimeZone16);
            _clientObjectImages.Images.Add(ObjectType.UserFoldersStructure.ToString(), ResourceGlobal.IconFolderStruct16);
            _clientObjectImages.Images.Add(ObjectType.StructuredSubSite.ToString(), ResourceGlobal.UserFoldersStructure);
            _clientObjectImages.Images.Add(ObjectType.CardTemplate.ToString(), ResourceGlobal.IconCardTemplate16);
            _clientObjectImages.Images.Add(ALARM_INSTRUCTION_STRING_KEY, ResourceGlobal.IconAlarmInstructions16);

            _clientObjectImages.Images.Add("Delete", ResourceGlobal.delete);
            _clientObjectImages.Images.Add("Edit", ResourceGlobal.Edit);
            _clientObjectImages.Images.Add("Insert", ResourceGlobal.insert);
        }
    }
}
