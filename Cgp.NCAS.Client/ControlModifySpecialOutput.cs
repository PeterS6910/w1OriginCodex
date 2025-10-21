using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Server.Beans;
using Contal.IwQuick.Sys;
using Contal.IwQuick.UI;

namespace Contal.Cgp.NCAS.Client
{
    public partial class ControlModifySpecialOutput : UserControl
    {
        public NCASClient Plugin { get; set; }

        public Action EditTextChanger;

        private Output _specialOutput;

        public Output SpecialOutput
        {
            get { return _specialOutput; }
            set
            {
                _specialOutput = value;
                ShowSpecialOutput();

                if (EditTextChanger != null)
                    EditTextChanger();
            }
        }

        private Guid _idParentCcu;

        public ControlModifySpecialOutput()
        {
            InitializeComponent();

            _tbmSpecialOutput.MaximumSize = new Size(0, 0);

            _tbmSpecialOutput.ImageTextBox.ForeColor = CgpClientMainForm.Singleton.GetDragDropTextColor;
            _tbmSpecialOutput.ImageTextBox.BackColor = CgpClientMainForm.Singleton.GetDragDropBackgroundColor;
        }

        public void SetParentCcu(Guid idCcu)
        {
            _idParentCcu = idCcu;
        }

        private void ShowSpecialOutput()
        {
            if (_specialOutput == null)
            {
                _tbmSpecialOutput.Text = string.Empty;
                return;
            }

            _tbmSpecialOutput.Text = _specialOutput.ToString();

            if (Plugin != null)
                _tbmSpecialOutput.TextImage = Plugin.GetImageForObjectType(ObjectType.Output);
        }

        private void _tbmSpecialOutput_ButtonPopupMenuItemClick(ToolStripItem item, int index)
        {
            if (item == _tsiModify)
            {
                ModifySpecialOutput();
                return;
            }

            SpecialOutput = null;
        }

        private void ModifySpecialOutput()
        {
            if (Plugin == null
                || CgpClient.Singleton.IsConnectionLost(true))
            {
                return;
            }

            try
            {
                var outputs =
                    Plugin.MainServerProvider.Outputs.FilterOutputsFromActivators(
                        false,
                        _idParentCcu,
                        false,
                        Guid.Empty);

                if (outputs == null)
                    return;

                var formAdd = new ListboxFormAdd(
                    new List<AOrmObject>(
                        outputs.Cast<AOrmObject>()),
                    CgpClient.Singleton.GetLocalizedString("NCASOutputsFormNCASOutputsForm"));


                object selectedObject;
                formAdd.ShowDialog(out selectedObject);

                var output = selectedObject as Output;

                if (output != null)
                {
                    SpecialOutput = output;
                    Plugin.AddToRecentList(output);
                }
            }
            catch (Exception error)
            {
                Dialog.Error(error);
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void AddSpecialOutput(object newSpecialOutput)
        {
            var specialOutput = newSpecialOutput as Output;

            if (specialOutput == null)
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmSpecialOutput.ImageTextBox,
                    Plugin != null
                        ? Plugin.GetTranslateString("ErrorWrongObjectType")
                        : string.Empty,
                    ControlNotificationSettings.Default);

                return;
            }

            if (Plugin == null
                || CgpClient.Singleton.IsConnectionLost(true))
            {
                return;
            }

            var outputs =
                Plugin.MainServerProvider.Outputs.FilterOutputsFromActivators(
                    false,
                    _idParentCcu,
                    false,
                    Guid.Empty);

            if (!Contains(
                outputs,
                specialOutput))
            {
                ControlNotification.Singleton.Error(
                    NotificationPriority.JustOne,
                    _tbmSpecialOutput.ImageTextBox,
                    Plugin.GetTranslateString("ErrorOutputCannotBeAssignThisRole", specialOutput.Name),
                    ControlNotificationSettings.Default);

                return;
            }

            SpecialOutput = specialOutput;

            Plugin.AddToRecentList(specialOutput);
        }

        private bool Contains(IEnumerable<Output> outputs, Output output)
        {
            if (outputs == null)
                return false;

            return outputs.Any(
                outputFromCollection =>
                    outputFromCollection.Compare(output));
        }

        private void _tbmSpecialOutput_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var output = e.Data.GetFormats();

                if (output == null)
                    return;

                AddSpecialOutput(e.Data.GetData(output[0]));
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
        }

        private void _tbmSpecialOutput_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _tbmSpecialOutput_ImageTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (SpecialOutput != null)
                NCASOutputsForm.Singleton.OpenEditForm(SpecialOutput);
        }
    }
}
