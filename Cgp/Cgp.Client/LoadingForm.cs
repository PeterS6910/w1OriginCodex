using Contal.Cgp.Components;
using Contal.IwQuick.Localization;
using System.Windows.Forms;

namespace Contal.Cgp.Client
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();

            Width = 24;
            Height = 24;
        }

        private void _pictureBoxLoading_Click(object sender, System.EventArgs e)
        {
            MethodInvoker methodInvokerDelegate = delegate ()
            {
                Close();
            };

            if (InvokeRequired)
                Invoke(methodInvokerDelegate);
            else
                methodInvokerDelegate();
        }
    }
}
