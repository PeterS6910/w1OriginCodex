using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Input;
using Contal.Cgp.Globals;
using Contal.IwQuick.Localization;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace Cgp.Components
{
    public partial class DeleteDataGridItemsDialog : TranslateForm
    {
        public bool DeleteAll { get; private set; }

        public IShortObject SelectItem
        {
            set { _wpfObjectListView.SelectItem = value; }
        }

        public List<IShortObject> SelectedItems
        {
            get { return _wpfObjectListView.SelectedItems; }
        }

        public DeleteDataGridItemsDialog(ImageList imageList, List<IShortObject> items, LocalizationHelper localizationHelper)
            : base(localizationHelper)
        {
            InitializeComponent();

            if (items == null 
                || items.Count == 0)
                return;

            _wpfObjectListView.Icons = imageList;
            _wpfObjectListView.Items = items;
            _wpfObjectListView.KeyUp += (sender, e) => DeleteDataGridItemsDialog_KeyUp(e.Key);
        }

        protected override void AfterTranslateForm()
        {
            if (_wpfObjectListView.Items.Count == 1)
            {
                _bDeleteAll.Visible = false;
                _bDelete.Text = LocalizationHelper.GetString("General_bDelete");
                _bDelete.Left += _bDelete.Width - 81;
                _bDelete.Width = 81;
                _bDelete.Refresh();
            }
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            DeleteAll = false;
        }

        private void _bDeleteAll_Click(object sender, EventArgs e)
        {
            DeleteAll = true;
        }

        private void DeleteDataGridItemsDialog_KeyUp(object sender, KeyEventArgs e)
        {
            DeleteDataGridItemsDialog_KeyUp(e.KeyCode);
        }

        private void DeleteDataGridItemsDialog_KeyUp(Key key)
        {
            switch (key)
            {
                 case Key.Enter:
                    DeleteDataGridItemsDialog_KeyUp(Keys.Enter);
                    break;
                case Key.Escape:
                    DeleteDataGridItemsDialog_KeyUp(Keys.Escape);
                    break;
            }
        }

        private void DeleteDataGridItemsDialog_KeyUp(Keys key)
        {
            switch (key)
            {
                case Keys.Enter:

                    if (_bDeleteAll.Visible)
                        _bDeleteAll_Click(null, null);
                    else
                        _bDelete_Click(null, null);

                    DialogResult = DialogResult.OK;

                    break;

                case Keys.Escape:

                    DialogResult = DialogResult.Cancel;

                    break;
            }
        }
    }
}
