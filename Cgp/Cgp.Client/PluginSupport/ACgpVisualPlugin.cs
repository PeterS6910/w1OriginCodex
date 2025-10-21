using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client.Common;
using Contal.Cgp.Globals;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick;

namespace Contal.Cgp.Client.PluginSupport
{
    public abstract class ACgpVisualPlugin<TCgpVisualPlugin> : 
        ACgpClientPlugin<TCgpVisualPlugin>,
        ICgpVisualPlugin
        where TCgpVisualPlugin : ACgpVisualPlugin<TCgpVisualPlugin>
    {
        private PluginMainForm<TCgpVisualPlugin> _mainForm;

        public IPluginMainForm MainForm
        {
            get { return _mainForm; }
        }

        private IEnumerable<PluginMainForm<TCgpVisualPlugin>> _subForms;

        public IEnumerable<IPluginMainForm> SubForms
        {
            get { return _subForms.Cast<IPluginMainForm>(); }
        }

        private IEnumerable<PluginMainForm<TCgpVisualPlugin>> _hideSubForms;

        public IEnumerable<IPluginMainForm> HideSubForms
        {
            get { return _hideSubForms.Cast<IPluginMainForm>(); }
        }

        private Action<ICgpClientPlugin, Form> _eventEnter;
        private Action<ICgpClientPlugin, Form> _eventShow;
        private Action<ICgpClientPlugin, Form> _eventClose;

        private Form _invokingParent;

        public Form InvokingParent
        {
            get { return _invokingParent; }
        }

        protected abstract TCgpVisualPlugin This { get; }

        protected abstract PluginMainForm<TCgpVisualPlugin> CreateMainForm();
        protected abstract IEnumerable<PluginMainForm<TCgpVisualPlugin>> CreateSubForms();
        protected abstract IEnumerable<PluginMainForm<TCgpVisualPlugin>> CreateHideSubForms();

        public abstract IPluginMainForm GetEditPluginForm();

        public abstract ImageList GetPluginObjectsImages();

        public void InitializeUI(Form invokingParent)
        {
            if (null == invokingParent)
                throw new ArgumentNullException();

            _invokingParent = invokingParent;

            _subForms = CreateSubForms();
            _hideSubForms = CreateHideSubForms();
            _mainForm = CreateMainForm();
        }

        private void MainForm_Enter(Form form)
        {
            if (_eventEnter != null)
                _eventEnter(this, form);
        }

        public void AddToOnEnter(Action<ICgpClientPlugin, Form> enter)
        {
            if (_mainForm == null)
                return;

            _mainForm.FormOnEnter += MainForm_Enter;

            if (_subForms != null)
                foreach (PluginMainForm<TCgpVisualPlugin> subForm in _subForms)
                    subForm.FormOnEnter += MainForm_Enter;

            _eventEnter += enter;
        }

        private void MainForm_Show(object sender, EventArgs e)
        {
            if (_eventShow != null)
                _eventShow(this, (sender as Form));
        }

        public void AddToOnShow(Action<ICgpClientPlugin, Form> show)
        {
            if (_mainForm == null)
                return;

            _mainForm.Shown += MainForm_Show;

            if (_subForms != null)
                foreach (PluginMainForm<TCgpVisualPlugin> subForm in _subForms)
                    subForm.Shown += MainForm_Show;

            _eventShow += show;
        }

        private void MainForm_Close(object sender, EventArgs e)
        {
            if (_eventClose != null)
                _eventClose(this, sender as Form);
        }

        public void AddToOnClose(Action<ICgpClientPlugin, Form> close)
        {
            if (_mainForm == null)
                return;

            _mainForm.OnClose += MainForm_Close;

            if (_subForms != null)
                foreach (PluginMainForm<TCgpVisualPlugin> subForm in _subForms)
                    subForm.OnClose += MainForm_Close;

            _eventClose += close;
        }

        public abstract System.Drawing.Icon GetIconForObjectType(ObjectType objectType);

        public abstract string GetTranslateString(string name, params object[] args);

        public string GetTranslateNameForm(Form form)
        {
            string translateNameForm = string.Empty;

            if (form != null)
            {
                try
                {
                    translateNameForm = GetTranslateString(form.Name + form.Name);
                }
                catch
                {
                }

                if (translateNameForm == string.Empty)
                    translateNameForm = form.Text;
            }

            return translateNameForm;
        }

        public abstract string GetTranslateTableObjectTypeName(string name);

        public virtual bool LoadPluginControlToForm(
            Object obj,
            Control control,
            IExtendedCgpEditForm cgpEditForm,
            bool allowEdit)
        {            
            return false;
        }

        public virtual bool LoadPluginControlToForm(
            Object obj,
            Dictionary<string, bool> filter,
            Control control,
            IExtendedCgpEditForm cgpEditForm,
            bool allowEdit)
        {
            return false;
        }

        /// <summary>
        /// Loads and show opened windows for specific user
        /// </summary>
        /// <param name="selectedForm">Selected form</param>
        public abstract void LoadAndShowOpenedForms(out Form selectedForm);

        public abstract string GetLocalizedObjectName(AOrmObject aOrmObject);

        public virtual Dictionary<string, bool> GetDefaultCloneFilterForObjectType(ObjectType objectType)
        {
            return null;
        }
    }
}
