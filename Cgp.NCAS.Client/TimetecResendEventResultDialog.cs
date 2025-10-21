using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Contal.Cgp.Globals;
using Contal.Cgp.Client;
using WcfServiceNovaConnection;

namespace Contal.Cgp.NCAS.Client
{
    public partial class TimetecResendEventResultDialog : CgpTranslateForm
    {
        private class TimetecResendResultInfo
        {
            public string Result { get; private set; }
            public string TimetecErrorEventDescription { get; private set; }

            public TimetecResendResultInfo(
                string result,
                string timetecErrorEventDescription)
            {
                Result = result;
                TimetecErrorEventDescription = timetecErrorEventDescription;
            }
        }

        public TimetecResendEventResultDialog(
            IEnumerable<IShortObject> timetecErrorEventsDescriptions,
            Dictionary<int, TransitionAddResult> result)
            : base(NCASClient.LocalizationHelper)
        {
            InitializeComponent();

            _dgvData.DataSource = timetecErrorEventsDescriptions.Where(obj => result.ContainsKey((int) obj.Id))
                .Select(obj => new TimetecResendResultInfo(result[(int) obj.Id].ToString(), obj.Name)).ToArray();

            _dgvData.Columns[0].HeaderText = GetString("ColumnResult");
            _dgvData.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            _dgvData.Columns[1].HeaderText = GetString("ColumnTimetecErrorEventDescription");
            _dgvData.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
        }

        private void _bClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
