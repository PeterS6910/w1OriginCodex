using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.PluginSupport;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Globals;
using Contal.IwQuick;

namespace Contal.Cgp.NCAS.Client
{
    public partial class NCASActualListAccessedCR :
#if DESIGNER    
        Form
#else
 PluginMainForm<NCASClient>
#endif
    {
        private readonly Person _actPerson;
        DVoid2Void _dAfterTranslateForm;

        public override NCASClient Plugin
        {
            get { return NCASClient.Singleton; }
        }

        public NCASActualListAccessedCR(Person person, Control control)
            : base(NCASClient.LocalizationHelper, CgpClientMainForm.Singleton)
        {
            InitializeComponent();       

            LocalizationHelper.TranslateForm(this);
            _dAfterTranslateForm = AfterTranslateForm;
            _pBack.Parent = control;
            _actPerson = person;
            control.Enter += RunOnEnter;
            control.Disposed += RunOnDisposed;

            _tvActualAccessObjects.ImageList = ObjectImageList.Singleton.GetPluginImages(NCASClient.Singleton.FriendlyName);
            _tvActualAccessObjects.ImageList.ColorDepth = ColorDepth.Depth32Bit;
        }

        protected override void AfterTranslateForm()
        {
            LocalizationHelper.TranslateControl(_pBack);
        }

        void RunOnEnter(object sender, EventArgs e)
        {
            if (CgpClient.Singleton.IsConnectionLost(false)) return;
            if (_dAfterTranslateForm == null)
            {
                _dAfterTranslateForm = AfterTranslateForm;
                LocalizationHelper.LanguageChanged += _dAfterTranslateForm;
            }

            if (CgpClient.Singleton.IsConnectionLost(true)) return;

            LoadActualAccessObjects();
        }

        void RunOnDisposed(object sender, EventArgs e)
        {
            LocalizationHelper.LanguageChanged -= _dAfterTranslateForm;
        }

        private void LoadActualAccessObjects()
        {
            try
            {
                Exception error;
                Dictionary<Guid, object> objectStates;

                _tvActualAccessObjects.Nodes.Clear();

                var activeAOrmObjects =
                    Plugin.MainServerProvider.ACLPersons.GetActualAccessAOrmObjects(_actPerson, out objectStates, out error);

                var alarmAreas = new LinkedList<AlarmArea>();
                var dcus = new LinkedList<DCU>();
                var doorEnvironments = new LinkedList<DoorEnvironment>();
                var cardReaders = new LinkedList<CardReader>();
                var multiDoors = new LinkedList<MultiDoor>();
                var floors = new LinkedList<Floor>();
                var multiDoorElements = new LinkedList<MultiDoorElement>();

                foreach (var ormObj in activeAOrmObjects.Values)
                {
                    switch (ormObj.GetObjectType())
                    {
                        case ObjectType.AlarmArea: alarmAreas.AddLast((ormObj as AlarmArea));
                            break;

                        case ObjectType.DCU: dcus.AddLast(ormObj as DCU);
                            break;

                        case ObjectType.DoorEnvironment: doorEnvironments.AddLast((ormObj as DoorEnvironment));
                            break;

                        case ObjectType.CardReader: cardReaders.AddLast((ormObj as CardReader));
                            break;

                        case ObjectType.MultiDoor: multiDoors.AddLast((ormObj as MultiDoor));
                            break;

                        case ObjectType.Floor: floors.AddLast((ormObj as Floor));
                            break;

                        case ObjectType.MultiDoorElement: multiDoorElements.AddLast((ormObj as MultiDoorElement));
                            break;
                    }
                }

                AddAlarmAreaObjects(alarmAreas, objectStates);
                AddDcuObjects(dcus, objectStates);
                AddDoorEnvironmentObjects(doorEnvironments, objectStates);
                AddCardReaderObjects(cardReaders, objectStates);
                AddMultiDoorObjects(multiDoors, objectStates);
                AddFloorObjects(floors, objectStates);
                AddMultiDoorElementObjects(multiDoorElements, objectStates);

                ExpandAllNodes();

                _lLastRefreshDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            }
            catch
            {
                _lLastRefreshDateTime.Text = string.Empty;
            }
        }

        private void AddAlarmAreaObjects(
            IEnumerable<AlarmArea> alarmAreas,
            Dictionary<Guid, object> objectStates)
        {
            alarmAreas = alarmAreas.OrderBy(aa => aa.Name);

            foreach (var alarmArea in alarmAreas)
            {
                var treeNode = new TreeNode(alarmArea.ToString());
                
                AddObjectNode(
                    _tvActualAccessObjects,
                    objectStates,
                    alarmArea,
                    treeNode);

                foreach (var aaCardReader in alarmArea.AACardReaders)
                {
                    if (aaCardReader.CardReader != null && aaCardReader.CardReader != null)
                    {
                        AddChildCardReaderNode(
                            objectStates,
                            aaCardReader.CardReader,
                            ref treeNode);
                    }
                }
            }
        }

        private void AddDcuObjects(
            IEnumerable<DCU> dcus,
            Dictionary<Guid, object> objectStates)
        {
            dcus = dcus.OrderBy(dcu => dcu.Name);

            foreach (var dcu in dcus)
            {
                var treeNode = new TreeNode(dcu.ToString());

                AddObjectNode(
                    _tvActualAccessObjects,
                    objectStates,
                    dcu,
                    treeNode);

                foreach (var cardReader in dcu.CardReaders)
                {
                    if (cardReader != null)
                    {
                        AddChildCardReaderNode(
                            objectStates,
                            cardReader,
                            ref treeNode);
                    }
                }
            }
        }

        private void AddDoorEnvironmentObjects(
            IEnumerable<DoorEnvironment> doorEnvironments,
            Dictionary<Guid, object> objectStates)
        {
            doorEnvironments = doorEnvironments.OrderBy(doorEnvironment => doorEnvironment.Name);

            foreach (var doorEnv in doorEnvironments)
            {
                var treeNode = new TreeNode(doorEnv.ToString());
                
                AddObjectNode(
                    _tvActualAccessObjects,
                    objectStates,
                    doorEnv,
                    treeNode);

                if (doorEnv.CardReaderExternal != null)
                {
                    AddChildCardReaderNode(
                        objectStates,
                        doorEnv.CardReaderExternal,
                        ref treeNode);
                }
                if (doorEnv.CardReaderInternal != null)
                {
                    AddChildCardReaderNode(
                        objectStates,
                        doorEnv.CardReaderInternal,
                        ref treeNode);
                }
            }
        }

        private void AddMultiDoorObjects(
            IEnumerable<MultiDoor> multiDoors,
            Dictionary<Guid, object> objectStates)
        {
            multiDoors = multiDoors.OrderBy(multiDoor => multiDoor.Name);

            foreach (var multiDoor in multiDoors)
            {
                var treeNode = new TreeNode(multiDoor.ToString());

                AddObjectNode(
                    _tvActualAccessObjects,
                    objectStates,
                    multiDoor,
                    treeNode);

                if (multiDoor.CardReader != null)
                {
                    AddChildCardReaderNode(
                        objectStates,
                        multiDoor.CardReader,
                        ref treeNode);
                }
            }
        }

        private void AddMultiDoorElementObjects(
            IEnumerable<MultiDoorElement> multiDoorElements,
            Dictionary<Guid, object> objectStates)
        {
            multiDoorElements = multiDoorElements.OrderBy(multiDoorElement => multiDoorElement.Name);

            foreach (var multiDoorElement in multiDoorElements)
            {
                var treeNode = new TreeNode(multiDoorElement.ToString());

                AddObjectNode(
                    _tvActualAccessObjects,
                    objectStates,
                    multiDoorElement,
                    treeNode);

                if (multiDoorElement.MultiDoor == null
                    || multiDoorElement.MultiDoor.CardReader == null)
                {
                    continue;
                }

                var cardReader =
                    Plugin.MainServerProvider.CardReaders.GetObjectById(
                        multiDoorElement.MultiDoor.CardReader.IdCardReader);

                if (cardReader == null)
                    continue;

                AddChildCardReaderNode(
                    objectStates,
                    cardReader,
                    ref treeNode);
            }
        }

        private void AddFloorObjects(
            IEnumerable<Floor> floors,
            Dictionary<Guid, object> objectStates)
        {
            floors = floors.OrderBy(floor => floor.Name);

            foreach (var floor in floors)
            {
                var treeNode = new TreeNode(floor.ToString());

                AddObjectNode(
                    _tvActualAccessObjects,
                    objectStates,
                    floor,
                    treeNode);

                if (floor.Doors == null)
                    continue;

                foreach (var door in floor.Doors)
                {
                    if (door.MultiDoor == null
                        || door.MultiDoor.CardReader == null)
                    {
                        continue;
                    }

                    var cardReader =
                        Plugin.MainServerProvider.CardReaders.GetObjectById(
                            door.MultiDoor.CardReader.IdCardReader);

                    if (cardReader == null)
                        continue;

                    AddChildCardReaderNode(
                        objectStates,
                        cardReader,
                        ref treeNode);
                }
            }
        }

        private void AddCardReaderObjects(
            IEnumerable<CardReader> cardReaders,
            Dictionary<Guid, object> objectStates)
        {
            cardReaders = cardReaders.OrderBy(cr => cr.Name);

            foreach (var cardReader in cardReaders)
            {
                var treeNode = new TreeNode(ReturnCardReaderName(cardReader));
                AddObjectNode(_tvActualAccessObjects, objectStates, cardReader, treeNode);
            }
        }

        private static void AddObjectNode(TreeView treeView, Dictionary<Guid, object> objectStates, AOrmObject aOrmObject, TreeNode treeNode)
        {
            var objectType = aOrmObject.GetObjectType();
            treeNode.Tag = new TagInfoAormObj((Guid)aOrmObject.GetId(), objectType);
            SetNodeIcon(objectStates, treeNode, aOrmObject, objectType.ToString());
            treeView.Nodes.Add(treeNode);
        }

        private void AddChildCardReaderNode(Dictionary<Guid, object> objectStates, CardReader cardReader, ref TreeNode treeNode)
        {
            var node =
                new TreeNode(ReturnCardReaderName(cardReader))
                {
                    Tag = new TagInfoAormObj(cardReader.IdCardReader, ObjectType.CardReader)
                };

            SetNodeIcon(objectStates, node, cardReader, ObjectType.CardReader.ToString());
            treeNode.Nodes.Add(node);
        }

        private void ExpandAllNodes()
        {
            try
            {
                foreach (TreeNode item in _tvActualAccessObjects.Nodes)
                {
                    item.Expand();
                }
            }
            catch { }
        }

        private void _bRefresh_Click(object sender, EventArgs e)
        {
            LoadActualAccessObjects();
        }

        private string ReturnCardReaderName(CardReader cardReader)
        {
            try
            {
                SecurityLevel? actualSecurityLevel;
                SecurityLevel4SLDP? securityLeve4SlDp;

                Plugin.MainServerProvider.CardReaders.GetActualSecurityLevel(
                    cardReader.IdCardReader,
                    out actualSecurityLevel,
                    out securityLeve4SlDp);

                if (actualSecurityLevel == SecurityLevel.SecurityTimeZoneSecurityDailyPlan && securityLeve4SlDp != null)
                {
                    SecurityLevel securityLeveStzSdp;
                    switch (securityLeve4SlDp.Value)
                    {
                        case SecurityLevel4SLDP.togglecard:
                        case SecurityLevel4SLDP.togglecardpin:
                        case SecurityLevel4SLDP.card:
                            securityLeveStzSdp = SecurityLevel.CARD;
                            break;
                        case SecurityLevel4SLDP.cardpin:
                            securityLeveStzSdp = SecurityLevel.CARDPIN;
                            break;
                        case SecurityLevel4SLDP.code:
                            securityLeveStzSdp = SecurityLevel.CODE;
                            break;
                        case SecurityLevel4SLDP.codeorcard:
                            securityLeveStzSdp = SecurityLevel.CODEORCARD;
                            break;
                        case SecurityLevel4SLDP.codeorcardpin:
                            securityLeveStzSdp = SecurityLevel.CODEORCARDPIN;
                            break;
                        case SecurityLevel4SLDP.unlocked:
                            securityLeveStzSdp = SecurityLevel.Unlocked;
                            break;
                        default:
                            securityLeveStzSdp = SecurityLevel.Locked;
                            break;
                    }

                    return string.Format("{0} ({1} - {2})",
                        cardReader.ToString(),
                        GetString(string.Format("SecurityLevelStates_{0}", actualSecurityLevel.ToString())),
                        GetString(string.Format("SecurityLevelStates_{0}", securityLeveStzSdp.ToString())));
                }

                return string.Format("{0} ({1})",
                    cardReader.ToString(),
                    GetString(string.Format("SecurityLevelStates_{0}", actualSecurityLevel.ToString())));
            }
            catch (Exception)
            {
                
            }

            return cardReader.ToString();
        }

        private static void SetNodeIcon(Dictionary<Guid, object> objectStates, TreeNode node, AOrmObject aOrmObject, string key)
        {
            if (aOrmObject.GetObjectType() == ObjectType.CardReader)
            {
                try
                {
                    object state;
                    objectStates.TryGetValue(((CardReader)aOrmObject).IdCardReader, out state);
                    if (((OnlineState)state != OnlineState.Online))
                    {
                        key = ObjTypeHelper.CardReaderBlocked;
                    }
                }
                catch { }             
            }
            node.ImageKey = key;
            node.SelectedImageKey = key;
            node.StateImageKey = key;
        }

        private void _tvActualCR_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null)
            {
                var tagInfo = e.Node.Tag as TagInfoAormObj;
                if (tagInfo != null)
                {
                    var ormObject = DbsSupport.GetTableObject(tagInfo.ObjectType, tagInfo.Id);
                    DbsSupport.OpenEditForm(ormObject);
                }
            }
        }
    }

    class TagInfoAormObj
    {
        public Guid Id { get; private set; }
        public ObjectType ObjectType { get; private set; }

        public TagInfoAormObj(Guid id, ObjectType objType)
        {
            Id = id;
            ObjectType = objType;
        }
    }
}
