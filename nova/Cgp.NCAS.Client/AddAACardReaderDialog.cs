using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;
using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class AddAACardReaderDialog : CgpTranslateForm
    {
        private readonly NCASClient _plugin;
        private readonly AlarmArea _alarmArea;
        private readonly Guid _guidImplicitCCU;

        public ListOfObjects ActCardReaderObjects { get; private set; }

        public bool AASet 
        {
            get { return _cbAlarmAreaSet.Checked;}
        }

        public bool AAUnset 
        {
            get { return _cbAlarmAreaUnset.Checked;}
        }

        public bool AAUnconditionalSet 
        {
            get { return _cbUnconditionalSet.Checked; }
        }

        public bool PermanentlyUnlock 
        {
            get { return !_cbImplicitMember.Checked; }
        }

        public bool EnableEventlog 
        {
            get { return _cbEnableEventlog.Checked; }
        } 

        public AddAACardReaderDialog(NCASClient plugin, AlarmArea alarmArea)
            : base(NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            _plugin = plugin;
            _alarmArea = alarmArea;
            var implicitCcu = _plugin.MainServerProvider.AlarmAreas.GetImplicitCCUForAlarmArea(_alarmArea.IdAlarmArea);
            _guidImplicitCCU = implicitCcu != null
                ? implicitCcu.IdCCU
                : Guid.Empty;

            _tbmCardReader.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
            _tbmCardReader.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
        }

        private void _tbmCardReader_DoubleClick(object sender, EventArgs e)
        {
            if (ActCardReaderObjects != null && ActCardReaderObjects.Count == 1)
            {
                NCASCardReadersForm.Singleton.OpenEditForm(ActCardReaderObjects[0] as CardReader);
            }
        }

        private void _tbmCardReader_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item.Name == "_tsiModify")
            {
                ModifyCardReader();
            }
        }

        private void ModifyCardReader()
        {
            try
            {
                Exception error;
                IList<IModifyObject> listCrModObj;

                if (!NCASClient.INTER_CCU_COMMUNICATION)
                {
                    listCrModObj = _plugin.MainServerProvider.CardReaders.ListModifyObjects(
                        true,
                        out error,
                        _guidImplicitCCU);
                }
                else
                {
#pragma warning disable
                    listCrModObj = _plugin.MainServerProvider.CardReaders.ListModifyObjects(true, out error);
#pragma warning enable
                }

                if (error != null)
                    throw error;

                var formAdd = new ListboxFormAdd(listCrModObj, GetString("NCASCardReadersFormNCASCardReadersForm"));
                ListOfObjects outCardReaders;

                ActCardReaderObjects = new ListOfObjects();
                formAdd.ShowDialogMultiSelect(out outCardReaders);
                if (outCardReaders == null)
                {
                    return;
                }
                foreach (var cr in outCardReaders)
                {
                    var crMo = (CardReaderModifyObj)cr;
                    var cardReader = _plugin.MainServerProvider.CardReaders.GetObjectById(crMo.GetId);
                    ActCardReaderObjects.Objects.Add(cardReader);
                }
                RefreshCardReader();
            }
            catch
            {
            }
        }

        private void RefreshCardReader()
        {
            if (ActCardReaderObjects != null)
            {
                _tbmCardReader.Text = ActCardReaderObjects.ToString();
                _tbmCardReader.TextImage = _plugin.GetImageForListOfObject(ActCardReaderObjects);

                if (ActCardReaderObjects.Objects.Count > 1)
                {
                    var disableImplicitMember = false;

                    foreach (var obj in ActCardReaderObjects.Objects)
                    {
                        var alarmArea = _plugin.MainServerProvider.AACardReaders.GetImplicitAlarmArea(obj as CardReader);
                        if (alarmArea != null && !_alarmArea.Compare(alarmArea))
                        {
                            disableImplicitMember = true;
                        }
                    }

                    if (disableImplicitMember)
                    {
                        _cbImplicitMember.Enabled = false;
                        _cbImplicitMember.Checked = false;
                    }
                    else
                    {
                        _cbImplicitMember.Enabled = true;
                    }
                }
                else if (ActCardReaderObjects.Objects.Count == 1)
                {
                    var alarmArea = _plugin.MainServerProvider.AACardReaders.GetImplicitAlarmArea(ActCardReaderObjects.Objects[0] as CardReader);
                    if (alarmArea != null && !_alarmArea.Compare(alarmArea))
                    {
                        _cbImplicitMember.Checked = false;
                        _cbImplicitMember.Enabled = false;
                    }
                    else
                    {
                        _cbImplicitMember.Enabled = true;
                    }
                }
            }
            else
            {
                _tbmCardReader.Text = string.Empty;
            }
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            if (ActCardReaderObjects == null
                || ActCardReaderObjects.Count == 0)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader,
                    GetString("ErrorEntryCardReader"), ControlNotificationSettings.Default);

                return;
            }

            DialogResult = DialogResult.OK;
        }

        private void _tbmCardReader_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();
                if (output == null) return;
                AddCardReader(e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }

        private void AddCardReader(object newCardReader)
        {
            try
            {
                if (newCardReader.GetType() == typeof(CardReader))
                {
                    var cardReader = newCardReader as CardReader;
                    if (cardReader != null)
                    {
                        string errorMessage;

                        if (!CheckCardReaderBeforeAdding(
                            cardReader,
                            out errorMessage))
                        {
                            ControlNotification.Singleton.Error(
                                NotificationPriority.JustOne,
                                _tbmCardReader,
                                errorMessage,
                                ControlNotificationSettings.Default);

                            return;
                        }

                        ActCardReaderObjects = new ListOfObjects();
                        ActCardReaderObjects.Objects.Add(cardReader);
                        _plugin.AddToRecentList(newCardReader);
                    }
                }
                else
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _tbmCardReader,
                       CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"), ControlNotificationSettings.Default);
                }
            }
            catch
            { }
        }

        private bool CheckCardReaderBeforeAdding(
            CardReader cardReader,
            out string errorMessage)
        {
            if (!NCASClient.INTER_CCU_COMMUNICATION)
            {
                if (
                    !_plugin.MainServerProvider.AlarmAreas.SetCardReaderToAlarmArea(cardReader.IdCardReader,
                        _guidImplicitCCU))
                {
                    errorMessage = GetString("InterCCUCommunicationNotEnabled");
                    return false;
                }
            }

            if (_alarmArea.AACardReaders == null)
            {
                errorMessage = null;
                return true;
            }

            if (_alarmArea.AACardReaders.Any(
                actAaCardReader =>
                    actAaCardReader.CardReader != null
                    && actAaCardReader.CardReader.Compare(cardReader)))
            {
                errorMessage = GetString("ErrorCardReaderAlreadyInAlarmArea");
                return false;
            }

            errorMessage = null;
            return true;
        }

        private void _tbmCardReader_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }
    }
}
