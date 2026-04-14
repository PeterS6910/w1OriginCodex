using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Contal.IwQuick;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;

namespace Contal.Cgp.Client.PluginSupport
{
    public abstract class PluginMainForm<TCgpVisualPlugin> :
        MdiChildForm,
        IPluginMainForm
        where TCgpVisualPlugin : ICgpVisualPlugin
    {
        private readonly ICgpClientMainForm _cgpClientMainForm;
        private readonly bool _dockToMdi;
        public Image FormImage { get; set; }

        public EventHandler OnClose { get; set; }

        protected PluginMainForm(
            ICgpClientMainForm cgpClientMainForm,
            bool dockToMdi,
            LocalizationHelper localizationHelper)
            : base(localizationHelper)
        {
            _cgpClientMainForm = cgpClientMainForm;
            _dockToMdi = dockToMdi;
        }

        protected PluginMainForm(
            LocalizationHelper localizationHelper,
            ICgpClientMainForm cgpClientMainForm)
            : base(localizationHelper)
        {
            _cgpClientMainForm = cgpClientMainForm;
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
                        menu.ImageTextBox.ForeColor = _cgpClientMainForm.GetDragDropTextColor;
                        menu.ImageTextBox.BackColor = _cgpClientMainForm.GetDragDropBackgroundColor;
                    }
                    else
                    {
                        control.ForeColor = _cgpClientMainForm.GetDragDropTextColor;
                        control.BackColor = _cgpClientMainForm.GetDragDropBackgroundColor;
                    }
                }
                else if (control.Tag != null && control.Tag.ToString() == "Reference")
                {
                    control.ForeColor = _cgpClientMainForm.GetReferenceTextColor;
                    control.BackColor = _cgpClientMainForm.GetReferenceBackgroundColor;
                }
            }
        }

        public abstract TCgpVisualPlugin Plugin { get; }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.MdiFormClosing)
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);

            try
            {
                if (e.CloseReason == CloseReason.UserClosing)
                {
                    e.Cancel = true;
                    Hide();
                }

                if (OnClose != null)
                    OnClose(this, null);
            }
            catch(Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (_dockToMdi && MdiParent != Plugin.InvokingParent)
                MdiParent = Plugin.InvokingParent;
        }

        public virtual bool HasAccessView()
        {
            return false;
        }

        public virtual void SpecialAction(List<Object> objects)
        {
        }

        protected T GetObjectFromDragDrop<T>(DragEventArgs e)
            where T : class
        {
            return GetObjectsFromDragDrop<T>(e, 0);
        }

        protected T GetObjectsFromDragDrop<T>(DragEventArgs e, int index)
            where T : class
        {
            string[] formats = e.Data.GetFormats();

            if (formats == null ||
                formats.Length <= index)
                return null;

            return e.Data.GetData(formats[index]) as T;
        }
    }
}
