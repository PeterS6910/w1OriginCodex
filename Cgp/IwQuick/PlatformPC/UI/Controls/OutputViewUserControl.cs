using Contal.IwQuick.Data;
using Contal.IwQuick.PlatformPC.Properties;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Contal.IwQuick.UI.Controls
{
    public partial class OutputViewUserControl : UserControl, IStatusReport
    {
        private BindingList<StatusMessage> outputDatas = new BindingList<StatusMessage>();
        private int dataGridViewHeight = 0;

        public OutputViewUserControl()
        {
            InitializeComponent();
            
            bindingSource.DataSource = outputDatas;
        }

        public void AddStatusMessage(StatusMessage data)
        {
            if (outputDatas.Count > 1000)
            {
                outputDatas.RemoveAt(outputDatas.Count - 1);
            }

            outputDatas.Insert(0, data);
        }

        public void AddStatusMessage(string message)
        {
            AddStatusMessage(new StatusMessage(DateTime.Now, StatusReport.AppName, message));
        }

        public void Clear()
        {
            outputDatas.Clear();
        }

        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs eventArgs)
        {
            if (eventArgs.ColumnIndex == 0)
            {
                eventArgs.Value = imageList.Images[(int)eventArgs.Value];
            }
        }

        private void OnShowHideClicked(object sender, EventArgs e)
        {
            var isVisible = dataGridView.Visible;

            if (isVisible)
            {
                if (dataGridViewHeight == 0)
                {
                    dataGridViewHeight = dataGridView.Height;
                }

                this.Height -= dataGridViewHeight;
                pictureBoxShowHide.Image = Resources.Show16;
            }
            else
            {
                this.Height += dataGridViewHeight;
                pictureBoxShowHide.Image = Resources.Hide16;
            }

            dataGridView.Visible = !isVisible;
        }
    }
}