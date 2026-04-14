using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.Cgp.BaseLib;

namespace TestVisual.plugin
{
    public partial class TestVisualForm : PluginMainForm
    {
        public TestVisualForm()
            :base(false)
        {
            InitializeComponent();

            
        }

        private void TestVisualForm_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MdiParent == null)
                DockToMDI();
            else
                UndockFromMDI();
        }
    }
}
