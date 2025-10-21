using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.Cgp.BaseLib;
using Contal.IwQuick.UI;
using System.Xml;

namespace Contal.Cgp.Client
{
    public partial class CardTemplatesForm :
#if DESIGNER
        Form
#else
 ACgpTableForm<CardTemplate, CardTemplateShort>
#endif
    {
        private static volatile CardTemplatesForm _singleton = null;
        private static object _syncRoot = new object();

        public static CardTemplatesForm Singleton
        {
            get
            {
                if (null == _singleton)
                    lock (_syncRoot)
                    {
                        if (null == _singleton)
                        {
                            _singleton = new CardTemplatesForm();
                            _singleton.MdiParent = CgpClientMainForm.Singleton;
                        }
                    }
                return _singleton;
            }
        }

        private bool _openedForEdit = false;
        private Guid _editTemplateId = Guid.Empty;

        public CardTemplatesForm()
        {
            InitializeComponent();
            CardIdManagementManager.Singleton.CardComposerFinished += new CardIdManagementManager.CardComposerFinishedHandler(CardComposerFinished);
            CardIdManagementManager.Singleton.CardJobFinished += new CardIdManagementManager.CardJobFinishedHandler(CardJobFinished);
            CardIdManagementManager.Singleton.CardPreviewFinished += new CardIdManagementManager.CardPreviewFinishedHandler(CardPreviewFinished);
            CardIdManagementManager.Singleton.ConfigurationFinished += new CardIdManagementManager.ConfigurationFinishedHandler(ConfigurationFinished);
            CardIdManagementManager.Singleton.ImageCaptureFinished += new CardIdManagementManager.ImageCaptureFinishedHandler(ImageCaptureFinished);
            InitCGPDataGridView();
        }

        private void InitCGPDataGridView()
        {
            _cdgvData.LocalizationHelper = CgpClient.Singleton.LocalizationHelper;
            _cdgvData.ImageList = ObjectImageList.Singleton.GetClientObjectsImages();
            _cdgvData.CgpDataGridEvents = this;
            _cdgvData.BeforeGridModified += _cdgvData_BeforeGridModified;
        }

        void _cdgvData_BeforeGridModified(BindingSource bindingSource)
        {
            if (bindingSource == null)
                return;

            foreach (CardTemplateShort ct in bindingSource)
            {
                ct.Symbol = _cdgvData.GetDefaultImage(ct);
            }
        }

        void ImageCaptureFinished(string result, string resultXML)
        {

        }

        void ConfigurationFinished(string result)
        {

        }

        void CardPreviewFinished(string result, string templateData)
        {

        }

        void CardJobFinished(string result, CardTemplate template)
        {

        }

        void CardComposerFinished(string result, CardTemplate template)
        {
            if (result == "Success")
            {
                if (!CgpClient.Singleton.IsConnectionLost(false))
                {
                    Exception ex = null;
                    if (_openedForEdit)
                    {
                        CardTemplate templateForEdit = CgpClient.Singleton.MainServerProvider.CardTemplates.GetObjectForEdit(_editTemplateId, out ex);
                        if (templateForEdit == null || ex != null)
                        {
                            Dialog.Error(GetString("ErrorInsertFailed"));
                            return;
                        }

                        templateForEdit.Name = template.Name;
                        templateForEdit.TemplateData = template.TemplateData;
                        if (!CgpClient.Singleton.MainServerProvider.CardTemplates.Update(templateForEdit, out ex) || ex != null)
                        {
                            Dialog.Error(GetString("ErrorInsertFailed"));
                            return;
                        }
                        else
                        {
                            CgpClientMainForm.Singleton.DeleteFromRecentList(templateForEdit);
                            CgpClientMainForm.Singleton.AddToRecentList(templateForEdit);
                        }
                    }
                    else
                    {
                        ICollection<int> structuredSubSiteIds;
                        var selectSubSitesEnabled = SelectSubSitesEnabled(template);

                        if (selectSubSitesEnabled)
                        {
                            if (!SelectStructuredSubSite(
                                false,
                                out structuredSubSiteIds))
                            {
                                return;
                            }
                        }
                        else
                        {
                            structuredSubSiteIds = null;
                        }

                        if (!CgpClient.Singleton.MainServerProvider.CardTemplates.Insert(ref template, out ex) || ex != null)
                        {
                            if (ex is IwQuick.SqlUniqueException)
                            {
                                if (Dialog.ErrorQuestion(GetString("ErrorQuestionSameTemplateName")))
                                {
                                    CardIdManagementManager.Singleton.AnyProductionWindowOpened = false;
                                    if (!CardIdManagementManager.Singleton.RunTemplateEdit(template, out ex))
                                    {
                                        if (ex == null)
                                            Dialog.Error(GetString("FailedToOpenIdManagementDialog"));
                                        else
                                            Dialog.Error(GetString("ExceptionOccured") + ": " + ex.Message);
                                    }
                                }
                            }
                            else
                                Dialog.Error(GetString("ErrorInsertFailed"));
                            return;
                        }

                        InsertStructuredSubSiteObject(
                            template.GetObjectType(),
                            template.GetIdString(),
                            false,
                            structuredSubSiteIds);
                        
                        CgpClientMainForm.Singleton.AddToRecentList(template);
                    }

                    ShowData();
                }
            }
            else if (result != "User Canceled")
                Dialog.Error(GetString("CardComposerOpenFailed"));
        }

        protected override ICollection<CardTemplateShort> GetData()
        {
            Exception error;
            ICollection<CardTemplateShort> list = CgpClient.Singleton.MainServerProvider.CardTemplates.ShortSelectByCriteria(_filterSettings, out error);

            if (error != null)
                throw (error);

            CheckAccess();
            _lRecordCount.BeginInvoke(new Action(
            () =>
            {
                _lRecordCount.Text = string.Format("{0} : {1}",
                                                        GetString("TextRecordCount"),
                                                        list == null
                                                            ? 0
                                                            : list.Count);
            }));
            return list;
        }

        private void CheckAccess()
        {
            if (InvokeRequired)
                BeginInvoke(new DVoid2Void(CheckAccess));
            else
            {
                if (HasAccessInsert())
                {
                    _cdgvData.EnabledInsertButton = true;
                    _eCloneName.Enabled = true;
                    _bClone.Enabled = true;
                }
                else
                {
                    _cdgvData.EnabledInsertButton = false;
                    _eCloneName.Enabled = false;
                    _bClone.Enabled = false;
                }

                //_bEdit.Enabled = HasAccessUpdate();
                _cdgvData.EnabledDeleteButton = HasAccessDelete();
            }
        }

        public override bool HasAccessView()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CardTemplates.HasAccessView();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessView(CardTemplate cardTemplate)
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CardTemplates.HasAccessViewForObject(cardTemplate);

                return false;
            }
            catch
            {
                return false;
            }
        }

        public override bool HasAccessInsert()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CardTemplates.HasAccessInsert();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAccessUpdate()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CardTemplates.HasAccessUpdate();

                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool HasAccessDelete()
        {
            try
            {
                if (CgpClient.Singleton.IsLoggedIn)
                    return CgpClient.Singleton.MainServerProvider.CardTemplates.HasAccessDelete();

                return false;
            }
            catch
            {
                return false;
            }
        }

        protected override void ModifyGridView(BindingSource bindingSource)
        {
            _cdgvData.ModifyGridView(bindingSource, CardTemplateShort.COLUMN_SYMBOL, CardTemplateShort.COLUMNNAME);
        }

        protected override void RemoveGridView()
        {
            _cdgvData.RemoveDataSource();
        }

        protected override ACgpEditForm<CardTemplate> CreateEditForm(CardTemplate obj, ShowOptionsEditForm showOption)
        {
            return null;
        }

        protected override CardTemplate GetObjectForEdit(CardTemplateShort listObj, out bool editEnabled)
        {
            return CgpClient.Singleton.MainServerProvider.CardTemplates.GetObjectForEditById(listObj.Id, out editEnabled);
        }

        protected override CardTemplate GetFromShort(CardTemplateShort listObj)
        {
            return CgpClient.Singleton.MainServerProvider.CardTemplates.GetObjectById(listObj.Id);
        }

        protected override CardTemplate GetById(object idObj)
        {
            return CgpClient.Singleton.MainServerProvider.CardTemplates.GetObjectById(idObj);
        }

        protected override bool Compare(CardTemplate obj1, CardTemplate obj2)
        {
            return obj1.Compare(obj2);
        }

        protected override void DeleteObj(CardTemplate obj)
        {
            Exception ex;
            if (!CgpClient.Singleton.MainServerProvider.CardTemplates.DeleteById(obj.Id, out ex))
                throw ex;
        }

        protected override void DeleteObjById(object idObj)
        {
            Exception ex;
            if (!CgpClient.Singleton.MainServerProvider.CardTemplates.DeleteById(idObj, out ex))
                throw ex;
        }

        protected override void SetFilterSettings()
        {
            _filterSettings.Clear();
            if (!string.IsNullOrEmpty(_eFilterName.Text))
            {
                FilterSettings filterSettingsName = new FilterSettings(CardTemplate.COLUMNNAME, _eFilterName.Text, ComparerModes.LIKEBOTH);
                _filterSettings.Add(filterSettingsName);
            }
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            _openedForEdit = false;
            Exception ex = null;
            if (!CardIdManagementManager.Singleton.RunTemplateInsert(out ex))
                Dialog.Error(GetString("CardComposerOpenFailed"));
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            EditCurrent();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (_cdgvData.SelectedRows.Count == 1)
            {
                Delete_Click();
            }
            else
            {
                MultiselectDeleteClic(_cdgvData.SelectedRows);
            }
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            _eFilterName.Text = string.Empty;
        }

        #region Filters
        private void RunFilterClick(object sender, EventArgs e)
        {
            RunFilter();
        }

        private void FilterClearClick(object sender, EventArgs e)
        {
            FilterClear_Click();
        }

        protected override void ClearFilterEdits()
        {
            _eFilterName.Text = string.Empty;
        }

        #endregion

        private void EditCurrent()
        {
            if ((_cdgvData.DataGrid.DataSource as BindingSource).Current != null)
            {
                CardTemplateShort templateShort = (_cdgvData.DataGrid.DataSource as BindingSource).Current as CardTemplateShort;
                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;

                CardTemplate fullCardTemplate = null;
                fullCardTemplate = CgpClient.Singleton.MainServerProvider.CardTemplates.GetObjectById(templateShort.Id);
                if (fullCardTemplate == null)
                    return;

                _openedForEdit = true;
                _editTemplateId = fullCardTemplate.Id;
                Exception ex;
                if (!CardIdManagementManager.Singleton.RunTemplateEdit(fullCardTemplate, out ex))
                {
                    if (ex == null)
                        Dialog.Error(GetString("FailedToOpenIdManagementDialog"));
                    else
                        Dialog.Error(GetString("ExceptionOccured") + ": " + ex.Message);
                }
                else
                    CgpClientMainForm.Singleton.AddToRecentList(fullCardTemplate);
            }
        }

        public bool EditSpecific(CardTemplate cardTemplate)
        {
            if (cardTemplate == null)
                return false;

            _openedForEdit = true;
            _editTemplateId = cardTemplate.Id;
            Exception ex = null;
            if (!CardIdManagementManager.Singleton.RunTemplateEdit(cardTemplate, out ex))
            {
                if (ex == null)
                    Dialog.Error(GetString("FailedToOpenIdManagementDialog"));
                else
                    Dialog.Error(GetString("ExceptionOccured") + ": " + ex.Message);
                return false;
            }
            else
                return true;
        }

        private void _bClone_Click(object sender, EventArgs e)
        {
            if (_eCloneName.Text == null || _eCloneName.Text == string.Empty)
            {
                ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eCloneName, GetString("ErrorInsertName"), ControlNotificationSettings.Default);
                return;
            }

            if ((_cdgvData.DataGrid.DataSource as BindingSource).Current != null)
            {
                CardTemplateShort templateShort = (_cdgvData.DataGrid.DataSource as BindingSource).Current as CardTemplateShort;
                if (CgpClient.Singleton.IsConnectionLost(false))
                    return;

                CardTemplate fullCardTemplate = null;
                fullCardTemplate = CgpClient.Singleton.MainServerProvider.CardTemplates.GetObjectById(templateShort.Id);
                if (fullCardTemplate == null)
                    return;

                if (IsTemplateNameAlreadyDefined(_eCloneName.Text))
                {
                    ControlNotification.Singleton.Error(NotificationPriority.JustOne, _eCloneName, GetString("ErrorTemplateNameAlreadyUsed"), ControlNotificationSettings.Default);
                    return;
                }

                CardTemplate newTemplate = new CardTemplate()
                {
                    Name = _eCloneName.Text,
                    TemplateData = RenameTemplateNameInTemplateData(_eCloneName.Text, fullCardTemplate.TemplateData)
                };

                Exception ex;
                if (!CgpClient.Singleton.MainServerProvider.CardTemplates.Insert(ref newTemplate, out ex) || ex != null)
                    Dialog.Error(GetString("ErrorInsertFailed"));
                else
                    ShowData();
            }
        }

        private byte[] RenameTemplateNameInTemplateData(string newTemplateName, byte[] templateData)
        {
            if (newTemplateName == null || newTemplateName == string.Empty)
                return templateData;

            if (templateData == null)
                return templateData;

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(Encoding.UTF8.GetString(templateData));

            try
            {
                xmlDoc.GetElementsByTagName("card")[0].Attributes["name"].Value = newTemplateName;
            }
            catch
            {
                return templateData;
            }

            return Encoding.UTF8.GetBytes(xmlDoc.OuterXml);
        }

        private bool IsTemplateNameAlreadyDefined(string templateName)
        {
            if (templateName == null || templateName == string.Empty)
                return false;

            if (CgpClient.Singleton.IsConnectionLost(false))
                return false;

            string[] templateNames = CgpClient.Singleton.MainServerProvider.CardTemplates.GetAllCardTemplateNames();

            if (templateNames != null && templateNames.Length > 0)
            {
                foreach (string name in templateNames)
                {
                    if (name.ToLower().Equals(templateName.ToLower()))
                        return true;
                }
            }

            return false;
        }

        public override void DeleteClick()
        {
            _bDelete_Click(null, null);
        }

        public override void EditClick()
        {
            _bEdit_Click(null, null);
        }

        public override void InsertClick()
        {
            _bInsert_Click(null, null);
        }
    }
}
