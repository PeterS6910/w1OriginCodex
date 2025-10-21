using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick.Localization;

namespace Contal.Cgp.DBSCreator
{
    public partial class FormConfirmation : TranslateForm
    {
        bool _cancled = false;
        bool _goBack = false;

        #region Properties
        public bool WasCanceled
        {
            get { return _cancled; }
        }

        public bool GoBack
        {
            get { return _goBack; }
        }
        #endregion

        public FormConfirmation(LocalizationHelper localizationHelper)
            : base(localizationHelper)
        {
            InitializeComponent();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _cancled = true;
            _goBack = false;
            Close();
        }

        private void _bNext_Click(object sender, EventArgs e)
        {
            _cancled = false;
            _goBack = false;
            Close();
        }

        private void _bBack_Click(object sender, EventArgs e)
        {
            _cancled = false;
            _goBack = true;
            Close();
        }
    }
}
