using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.UI;
using JetBrains.Annotations;

namespace Contal.Cgp.Client
{
    public class MdiChildForm : CgpTranslateForm
    {
        private static List<MdiChildForm> _openedForms = new List<MdiChildForm>();
        private static MdiChildForm _activeForm = null;
        //private bool _runOnEnter = false;

        [DllImport("UxTheme.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        private Action<Form> _formOnEnter = null;

        public Action<Form> FormOnEnter
        {
            get { return _formOnEnter; }
            set { _formOnEnter = value; }
        }

        public virtual void CallEscape()
        {
        }

        private DVoid2Void _FormOnLeave = null;

        public DVoid2Void FormOnLeave
        {
            get { return _FormOnLeave; }
            set { _FormOnLeave = value; }
        }

        public MdiChildForm(LocalizationHelper localizationHelper)
            : base(localizationHelper)
        {
            this.Enter += new EventHandler(MdiChildForm_Enter);

            CgpClientMainForm.Singleton.ShowFormLoadProgress(this);
        }

        private void _titlebarButton_TitleBarButtonClicked(object sender, EventArgs e)
        {

        }

        private void MdiChildForm_Enter(object sender, EventArgs e)
        {
            SetActiveForm(this);
        }

        public MdiChildForm()
        {
            this.Enter += new EventHandler(MdiChildForm_Enter);
        }

        private void SetActiveForm(MdiChildForm actForm)
        {
            if (_activeForm != actForm)
            {
                _activeForm = actForm;

                if (actForm != null)
                {
                    if (actForm._formOnEnter != null)
                        actForm._formOnEnter(actForm);

                    if (_openedForms.Count > 1)
                    {
                        MdiChildForm lastActiveForm = _openedForms[_openedForms.Count - 1];

                        if (lastActiveForm != actForm && lastActiveForm._FormOnLeave != null)
                            lastActiveForm._FormOnLeave();
                    }

                    _openedForms.Remove(actForm);
                    _openedForms.Add(actForm);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
            _openedForms.Remove(this);
            if (_openedForms.Count > 0)
            {
                MdiChildForm lastActiveForm = _openedForms[_openedForms.Count - 1];
                if (lastActiveForm != null)
                    SetActiveForm(lastActiveForm);
            }
            else
            {
                SetActiveForm(null);
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            //_runOnEnter = true;
        }

        public virtual bool IsEditForm()
        {
            return false;
        }

        protected static bool SelectStructuredSubSite(bool multiSelectEnabled,
            out ICollection<int> selectedSubSiteIds)
        {
            var selectStructuredSubSiteForm = new SelectStructuredSubSiteForm();
            return selectStructuredSubSiteForm.SelectStructuredSubSites(
                multiSelectEnabled,
                out selectedSubSiteIds);
        }

        protected static void InsertStructuredSubSiteObject(ObjectType objectType, string idString, bool isReference,
            ICollection<int> structuredSubSiteIds)
        {
            if (structuredSubSiteIds == null)
                return;

            foreach (var structuredSubSiteId in structuredSubSiteIds)
            {
                if (structuredSubSiteId == -1)
                {
                    if (isReference)
                        InsertStructuredSubSiteObject(null, objectType, idString, true);

                    continue;
                }

                var structuredSubSite =
                    CgpClient.Singleton.MainServerProvider.StructuredSubSites.GetObjectById(structuredSubSiteId);

                if (structuredSubSite != null)
                    InsertStructuredSubSiteObject(structuredSubSite, objectType, idString, isReference);
            }
        }

        private static void InsertStructuredSubSiteObject(StructuredSubSite structuredSubSite, ObjectType objectType,
            string idString, bool isReference)
        {
            var structuredSubSiteObject = new StructuredSubSiteObject
            {
                StructuredSubSite = structuredSubSite,
                ObjectType = objectType,
                ObjectId = idString,
                IsReference = isReference
            };

            if (!CgpClient.Singleton.MainServerProvider.StructuredSubSites.
                    InsertStructuredSubSiteObject(structuredSubSiteObject))
            {
                throw new Exception();
            }
        }

        protected static bool SelectSubSitesEnabled(AOrmObject ormObject)
        {
            return !CgpClient.Singleton.MainServerProvider.HasObjectParent(ormObject)
                   && CgpClient.Singleton.MainServerProvider.IsObjectTypeRelevantForStructuredSites(
                       ormObject.GetObjectType());
        }

        public void SetReferenceEditColors()
        {
            SetReferenceEditColors(this);
        }

        private void SetReferenceEditColors(Control mainControl)
        {
            foreach (Control cnt in mainControl.Controls)
            {
                if (cnt is TextBox || cnt is ListBox || cnt is ImageTextBox)
                {
                    DoAdd(cnt);
                }
                else if (cnt is TextBoxMenu)
                {
                    DoAdd((cnt as TextBoxMenu));
                }
                if ((cnt.Controls != null) && (!(cnt is TextBoxMenu)))
                    SetReferenceEditColors(cnt);
            }
        }

        private void DoAdd(Control control)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action<Control>(DoAdd), control);
            }
            else
            {
                if (control.AllowDrop)
                {
                    var menu = control as TextBoxMenu;
                    if (menu != null)
                    {
                        menu.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
                        menu.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
                    }
                    else
                    {
                        control.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
                        control.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
                    }
                }
                else if (control.Tag != null && control.Tag.ToString() == "Reference")
                {
                    control.ForeColor = CgpClientMainForm.Singleton.GetReferenceTextColor;
                    control.BackColor = CgpClientMainForm.Singleton.GetReferenceBackgroundColor;
                }
            }
        }

        protected void ErrorNotificationOverControl(
            [NotNull] Control control, 
            string errorSymbolName, 
            params object[] translationParameters)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ReferenceEquals(control,null))
                // ReSharper disable once HeuristicUnreachableCode
                return;

            ControlNotification.Singleton.Error(
                NotificationPriority.JustOne,
                control,
                GetString(errorSymbolName, translationParameters),  
                GetString("Error"),              
                CgpClient.Singleton.ClientControlNotificationSettings);
        }

        protected void ErrorNotificationOverControl(
            string errorMessage,
            [NotNull] Control control)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (ReferenceEquals(control, null))
                // ReSharper disable once HeuristicUnreachableCode
                return;

            ControlNotification.Singleton.Error(
                NotificationPriority.JustOne,
                control,
                errorMessage,
                GetString("Error"),
                CgpClient.Singleton.ClientControlNotificationSettings);
        }

        public new void Show()
        {
            if (InvokeRequired)
            {
                Invoke(new DVoid2Void(Show));
                return;
            }
            base.Show();

            CgpClientMainForm.Singleton.HideFormLoadProgress(this);
        }
    }
}
