using System;

using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.Client;

namespace Contal.Cgp.NCAS.Client
{
    public partial class SelectDCUHWTypeDialog : CgpTranslateForm
    {
        private DCUHWVersion _selectedDCUHWType = DCUHWVersion.Unknown;

        public DCUHWVersion SelectedDCUHWType
        {
            get { return _selectedDCUHWType; }
        }    

        public SelectDCUHWTypeDialog()
        {
            InitializeComponent();
        }

        private void SelectDCUHWTypeDialog_Load(object sender, EventArgs e)
        {
            var enumValues = Enum.GetValues(typeof(DCUHWVersion));
            
            foreach (object enumValue in enumValues)
            {
                _cbHWType.Items.Add(enumValue);
            }

            _cbHWType.SelectedIndex = 0;
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            if (_cbHWType.SelectedItem != null)
            {
                _selectedDCUHWType = (DCUHWVersion)_cbHWType.SelectedItem;
            }
        }
    }
}
