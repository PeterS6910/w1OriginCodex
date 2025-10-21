using Contal.IwQuick.Data;
using System;
using System.Windows.Forms;

namespace Contal.IwQuick.UI.Controls
{
    public class UiStatusReportControl<T> : IUiStatusReport where T : Control, IStatusReport
    {
        private T statusReportControl;

        public UiStatusReportControl(T statusReportControl)
        {
            this.statusReportControl = statusReportControl;
        }

        public void AddStatusMessage(StatusMessage data)
        {            
            if (statusReportControl.InvokeRequired)
            {
                statusReportControl.Invoke((Action)(() => statusReportControl.AddStatusMessage(data)));
            }
            else
            {
                statusReportControl.AddStatusMessage(data);
            }
        }

        public void AddStatusMessage(string message)
        {
            if (statusReportControl.InvokeRequired)
            {
                statusReportControl.Invoke((Action)(() => statusReportControl.AddStatusMessage(message)));
            }
            else
            {
                statusReportControl.AddStatusMessage(message);
            }
        }

        public void SetVisible(bool visible)
        {
            if (statusReportControl.InvokeRequired)
            {
                statusReportControl.Invoke((Action)(() => statusReportControl.Visible = visible));
            }
            else
            {
                statusReportControl.Visible = visible;
            }
        }
    }
}
