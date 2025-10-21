using System;
using System.Windows.Forms;

using Contal.IwQuick;

namespace Contal.Cgp.Client
{
    public partial class AlarmInstructionsForm : UserControl
    {
        public event DVoid2Void LocalInstructionTextChanged;
        public event DVoid2Void GlobalInstructionCreateClick;
        public event DVoid2Void GlobalInstructionInsertClick;
        public event DVoid2Void GlobalInstructionEditClick;
        public event DVoid2Void GlobalInstructionDeleteClick;
        public event Action<object> GlobalInstructionDragDrop;

        public AlarmInstructionsForm(string localInstruction, bool localAlarmInstructionsView,
            bool localAlarmInstructionsAdmin, DVoid2Void localInstructionTextChanged,
            DVoid2Void globalInstructionCreateClick, DVoid2Void globalInstructionInsertClick,
            DVoid2Void globalInstructionEditClick, DVoid2Void globalInstructionDeleteClick,
            Action<object> globalInstructionDragDrop)
        {
            InitializeComponent();
            this.Dock = DockStyle.Fill;

            _eLocalInstruction.Text = localInstruction;

            if (localAlarmInstructionsView || localAlarmInstructionsAdmin)
            {    
                _eLocalInstruction.ReadOnly = !localAlarmInstructionsAdmin;
            }
            else
            {
                _pLocalAlarmInstructions.Visible = false;
            }

            if (!GlobalAlarmInstructionsForm.Singleton.HasAccessView())
            {
                _pGlobalAlarmInstruction.Visible = false;
            }
            else
            {
                _bCreate.Enabled = GlobalAlarmInstructionsForm.Singleton.HasAccessInsert();
            }

            LocalInstructionTextChanged += localInstructionTextChanged;
            GlobalInstructionCreateClick += globalInstructionCreateClick;
            GlobalInstructionInsertClick += globalInstructionInsertClick;
            GlobalInstructionEditClick += globalInstructionEditClick;
            GlobalInstructionDeleteClick += globalInstructionDeleteClick;
            GlobalInstructionDragDrop += globalInstructionDragDrop;
        }

        public DataGridView GetDataGridViewGlobalinstructions()
        {
            return _dgGlobalInstructions;
        }

        public string GetLocalInstruction()
        {
            return _eLocalInstruction.Text;
        }

        private void _bCreate_Click(object sender, EventArgs e)
        {
            if (GlobalInstructionCreateClick != null)
            {
                try
                {
                    GlobalInstructionCreateClick();
                }
                catch { }
            }
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            if (GlobalInstructionInsertClick != null)
            {
                try
                {
                    GlobalInstructionInsertClick();
                }
                catch { }
            }
        }

        private void _eLocalInstruction_TextChanged(object sender, EventArgs e)
        {
            if (LocalInstructionTextChanged != null)
            {
                try
                {
                    LocalInstructionTextChanged();
                }
                catch { }
            }
        }

        private void _dgGlobalInstructions_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_dgGlobalInstructions.HitTest(e.X, e.Y).RowIndex == -1)
                return;

            if (GlobalInstructionEditClick != null)
            {
                try
                {
                    GlobalInstructionEditClick();
                }
                catch { }
            }
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (GlobalInstructionEditClick != null)
            {
                try
                {
                    GlobalInstructionEditClick();
                }
                catch { }
            }
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (GlobalInstructionDeleteClick != null)
            {
                try
                {
                    GlobalInstructionDeleteClick();
                }
                catch { }
            }
        }

        private void _dgGlobalInstructions_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.All;
        }

        private void _dgGlobalInstructions_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] output = e.Data.GetFormats();
                if (output == null) return;

                if (GlobalInstructionDragDrop != null)
                    GlobalInstructionDragDrop((object)e.Data.GetData(output[0]));
            }
            catch
            {
            }
        }
    }
}
