using Contal.IwQuick.Localization;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contal.Cgp.Client
{
    public enum PleaseWaitMode
    {
        SplashAbout = 1,
        EditMdiForms = 2
    }

    public class PleaseWaitMonitor : IDisposable
    {
        PleaseWaitMode _pwMode;
        Form _pwForm;
        Point _location;

        public PleaseWaitMonitor(Point location, PleaseWaitMode pwMode)
        {
            _pwMode = pwMode;
            _location = location;

            Thread thread = new Thread(new ThreadStart(DoWorkerThread));
            thread.IsBackground = true;
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public string Tag { get; set; }

        public void Dispose()
        {
            if (_pwForm != null)
                _pwForm.Invoke(new MethodInvoker(DoStopThread));
        }
        private void DoStopThread()
        {
            _pwForm.Close();
            _pwForm = null;
        }
        private void DoWorkerThread()
        {
            if (_pwForm == null)
            {
                switch(_pwMode)
                {
                    case PleaseWaitMode.SplashAbout:
                        _pwForm = new AboutForm(CgpClient.Singleton.LocalizationHelper);
                        break;
                    case PleaseWaitMode.EditMdiForms:
                        _pwForm = new LoadingForm();
                        break;
                }

                _pwForm.StartPosition = FormStartPosition.Manual;
                _pwForm.Location = _location;
                _pwForm.TopMost = true;

                Application.Run(_pwForm);
            }
            else
                Dispose();
        }
    }
}
