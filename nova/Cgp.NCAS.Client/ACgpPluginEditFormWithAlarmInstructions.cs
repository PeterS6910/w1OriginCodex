using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Contal.Cgp.Client.PluginSupport;
using Contal.IwQuick;
using Contal.Cgp.Server.Beans;
using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public abstract class ACgpPluginEditFormWithAlarmInstructions<TObject> : ACgpPluginEditForm<NCASClient, TObject> where TObject : AOrmObject
    {
        protected ACgpPluginEditFormWithAlarmInstructions(
                TObject editingObject,
                ShowOptionsEditForm showOption,
                ICgpClientMainForm cgpClientMainForm,
                PluginMainForm<NCASClient> myTableForm,
                LocalizationHelper localizationHelper)
            : base(
                editingObject,
                showOption,
                cgpClientMainForm,
                myTableForm,
                localizationHelper)
        {
        }

        public override bool IsEditForm()
        {
            return true;
        }

        #region AlarmInstructions

        private AlarmInstructionsForm _alarmInstructionsForm = null;
        private BindingSource _alarmInstructionsBindingSource;
        protected override void CreateAlarmInstructionsTabPage()
        {
            bool exist = false;
            var localAlarmInstructionsView = LocalAlarmInstructionsView();
            var localAlarmInstructionsAdmin = LocalAlarmInstructionsAdmin();

            if (_whTabConrol != null &&
                (localAlarmInstructionsView ||
                 localAlarmInstructionsAdmin ||
                 GlobalAlarmInstructionsForm.Singleton.HasAccessView()))
            {
                TabPage alarmInstructions = null;

                if (_alarmInstructionsForm != null)
                {
                    // If _alarmInstructionsFrom exist it means that TabPage exist too
                    foreach (var tabControl in _whTabConrol.TabPages)
                    {
                        var tabPage = tabControl as TabPage;
                        if (tabPage != null
                            && tabPage.Name == "_tpAlarmInstructions")
                        {
                            alarmInstructions = tabPage;
                            alarmInstructions.Controls.Remove(_alarmInstructionsForm);
                            exist = true;
                            break;
                        }
                    }
                }

                if (alarmInstructions == null)
                {
                    alarmInstructions =
                        new TabPage
                        {
                            Name = "_tpAlarmInstructions",
                            Text = "Alarm instructions",
                            BackColor = SystemColors.Control
                        };
                }

                _alarmInstructionsForm = new AlarmInstructionsForm(GetLocalAlarmInstruction(),
                    localAlarmInstructionsView,
                    localAlarmInstructionsAdmin,
                    LocalAlarmInstructionTextChanged,
                    GlobalAlarmInstructionsCreateClick,
                    GlobalAlarmInstructionsInsertClick,
                    GlobalAlarmInstructionsEditClick,
                    GlobalAlarmInstructionsDeleteClick,
                    GlobalAlarmInstructionsDragDrop);

                AlarmInstructionsReloadData();

                alarmInstructions.Controls.Add(_alarmInstructionsForm);

                if(!exist)
                    _whTabConrol.TabPages.Add(alarmInstructions);

                CgpClient.Singleton.LocalizationHelper.TranslateControl(_alarmInstructionsForm);
                CgpClient.Singleton.LocalizationHelper.TranslateControl(_whTabConrol);
            }
        }

        protected virtual bool LocalAlarmInstructionsView()
        {
            return false;
        }

        protected virtual bool LocalAlarmInstructionsAdmin()
        {
            return false;
        }

        protected virtual string GetLocalAlarmInstruction()
        {
            return string.Empty;
        }

        private void AlarmInstructionsReloadData()
        {
            try
            {
                var position = 0;
                if (_alarmInstructionsBindingSource != null && _alarmInstructionsBindingSource.Count > 0)
                    position = _alarmInstructionsBindingSource.Position;

                var dgGlobalInstructions = _alarmInstructionsForm.GetDataGridViewGlobalinstructions();
                var globalAlarmInstructions = GetGlobalAlarmInstructions();

                if (globalAlarmInstructions == null || globalAlarmInstructions.Count == 0)
                {
                    _alarmInstructionsBindingSource = null;
                    dgGlobalInstructions.DataSource = null;
                }
                else
                {
                    _alarmInstructionsBindingSource = 
                        new BindingSource
                        {
                            DataSource = globalAlarmInstructions
                        };

                    if (position < _alarmInstructionsBindingSource.Count)
                        _alarmInstructionsBindingSource.Position = position;
                    else
                        _alarmInstructionsBindingSource.Position = _alarmInstructionsBindingSource.Count - 1;

                    dgGlobalInstructions.DataSource = _alarmInstructionsBindingSource;

                    dgGlobalInstructions.AutoGenerateColumns = false;
                    dgGlobalInstructions.AllowUserToAddRows = false;

                    if (dgGlobalInstructions.Columns.Contains(GlobalAlarmInstruction.COLUMN_NAME))
                        dgGlobalInstructions.Columns[GlobalAlarmInstruction.COLUMN_NAME].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;

                    if (dgGlobalInstructions.Columns.Contains(GlobalAlarmInstruction.COLUMN_INSTRUCTIONS))
                        dgGlobalInstructions.Columns[GlobalAlarmInstruction.COLUMN_INSTRUCTIONS].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                    HideColumnDgw(dgGlobalInstructions, GlobalAlarmInstruction.COLUMN_ID_GLOBAL_ALARM_INSTRUCTION);
                    HideColumnDgw(dgGlobalInstructions, GlobalAlarmInstruction.COLUMN_OBJECT_TYPE);
                    HideColumnDgw(dgGlobalInstructions, GlobalAlarmInstruction.COLUMN_DESCRIPTION);

                    CgpClient.Singleton.LocalizationHelper.TranslateDataGridViewColumnsHeaders(dgGlobalInstructions);
                }
            }
            catch { }
        }

        protected string GetNewLocalAlarmInstruction()
        {
            if (_alarmInstructionsForm != null)
                return _alarmInstructionsForm.GetLocalInstruction();

            return string.Empty;
        }

        private ICollection<GlobalAlarmInstruction> GetGlobalAlarmInstructions()
        {
            if (CgpClient.Singleton.IsConnectionLost(true))
                return null;

            try
            {
                return CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetGlobalAlarmInstructionsForObject(_editingObject.GetObjectType(), _editingObject.GetIdString());
            }
            catch { }

            return null;
        }

        private void LocalAlarmInstructionTextChanged()
        {
            EditTextChanger(null, null);
        }

        private void GlobalAlarmInstructionsCreateClick()
        {
            try
            {
                var globalAlarmInstruction = new GlobalAlarmInstruction();
                if (GlobalAlarmInstructionsForm.Singleton.OpenInsertDialg(ref globalAlarmInstruction))
                {
                    if (CgpClient.Singleton.IsConnectionLost(true))
                        return;

                    GlobalAlarmInstructionsInsert(globalAlarmInstruction);
                }
            }
            catch { }
        }

        private void GlobalAlarmInstructionsInsertClick()
        {
            try
            {
                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                var listModObj = new List<IModifyObject>();

                Exception error;
                var listGlobalAlarmInstructionsFromDatabase = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.ListModifyObjects(out error);
                if (error != null) throw error;
                listModObj.AddRange(listGlobalAlarmInstructionsFromDatabase);

                var formAdd = new ListboxFormAdd(listModObj, CgpClient.Singleton.LocalizationHelper.GetString("GlobalAlarmInstructionsFormGlobalAlarmInstructionsForm"));
                IModifyObject outModObj;
                formAdd.ShowDialog(out outModObj);
                if (outModObj != null)
                {
                    var globalAlarmInstruction = CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.GetObjectById(outModObj.GetId);

                    GlobalAlarmInstructionsInsert(globalAlarmInstruction);
                }
            }
            catch
            {
                Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertGlobalAlarmInstructionToObjectFailed"));
            }
        }

        private void GlobalAlarmInstructionsInsert(GlobalAlarmInstruction globalAlarmInstruction)
        {
            try
            {
                Exception error;
                if (!CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.AddReference(globalAlarmInstruction.IdGlobalAlarmInstruction, _editingObject.GetObjectType(), _editingObject.GetIdString(), out error))
                {
                    throw error;
                }
                AlarmInstructionsReloadData();
                CgpClientMainForm.Singleton.AddToRecentList(globalAlarmInstruction);
            }
            catch (Exception exception)
            {
                if (exception is SqlUniqueException)
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorGlobalAlarmInstructionIsAlreadAddedToThisObject"));
                else
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorInsertGlobalAlarmInstructionToObjectFailed"));
            }
        }

        private void GlobalAlarmInstructionsEditClick()
        {
            try
            {
                if (_alarmInstructionsBindingSource != null && _alarmInstructionsBindingSource.Count > 0)
                {
                    var globalAlarmInstruction = (GlobalAlarmInstruction)_alarmInstructionsBindingSource.List[_alarmInstructionsBindingSource.Position];
                    if (globalAlarmInstruction != null)
                        GlobalAlarmInstructionsForm.Singleton.OpenEditForm(globalAlarmInstruction);
                }
            }
            catch { }
        }

        private void GlobalAlarmInstructionsDeleteClick()
        {
            try
            {
                if (!Dialog.Question(CgpClient.Singleton.LocalizationHelper.GetString("QuestionDeleteConfirm")))
                    return;

                if (CgpClient.Singleton.IsConnectionLost(true))
                    return;

                if (_alarmInstructionsBindingSource != null && _alarmInstructionsBindingSource.Count > 0)
                {
                    var globalAlarmInstruction = (GlobalAlarmInstruction)_alarmInstructionsBindingSource.List[_alarmInstructionsBindingSource.Position];
                    if (globalAlarmInstruction != null)
                    {
                        CgpClient.Singleton.MainServerProvider.GlobalAlarmInstructions.RemoveReference(globalAlarmInstruction.IdGlobalAlarmInstruction, _editingObject.GetObjectType(), _editingObject.GetIdString());
                        AlarmInstructionsReloadData();
                    }
                }
            }
            catch { }
        }

        private void GlobalAlarmInstructionsDragDrop(object dragDropObject)
        {
            try
            {
                var globalAlarmInstruction = dragDropObject as GlobalAlarmInstruction;
                if (globalAlarmInstruction != null)
                {
                    if (CgpClient.Singleton.IsConnectionLost(true))
                        return;

                    GlobalAlarmInstructionsInsert(globalAlarmInstruction);
                }
                else
                {
                    Dialog.Error(CgpClient.Singleton.LocalizationHelper.GetString("ErrorWrongObjectType"));
                }
            }
            catch { }
        }

        #endregion
    }
}
