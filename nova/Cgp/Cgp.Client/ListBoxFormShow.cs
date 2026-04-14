using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Contal.IwQuick.Localization;

namespace Contal.Cgp.Client
{
    public partial class ListboxFormShow : CgpTranslateForm
    {
        private List<object> _data = null;

        public ListboxFormShow(LocalizationHelper localizationHelper)
            : base(localizationHelper)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Only for designer
        /// </summary>
        public ListboxFormShow()
        {
            InitializeComponent();
        }

        private void _bAdd_Click(object sender, EventArgs e)
        {
            if (AddObjectToListBox())
                ShowDataInListBox();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            Object obj = _lbData.SelectedItem;

            if (obj != null)
            {
                if (DeleteOjectsFromListBox(obj))
                    ShowDataInListBox();
            }
        }

        private void _bRunFilter_Click(object sender, EventArgs e)
        {
            ShowData(_eFilter.Text);
        }

        private void _bFilterClear_Click(object sender, EventArgs e)
        {
            _eFilter.Text = "";
            ShowData(_eFilter.Text);
        }

        private void _eFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ShowData(_eFilter.Text);
            }
        }

        protected void ShowDataInListBox()
        {
            _data = GetDataForListBox();
            ShowData(_eFilter.Text);
        }

        private void ShowData(string filter)
        {
            _lbData.Items.Clear();
            foreach (Object obj in _data)
                if (filter == "" || obj.ToString().IndexOf(filter) == 0)
                    _lbData.Items.Add(obj);
        }

        protected virtual List<object> GetDataForListBox()
        {
            return null;
        }

        protected virtual bool DeleteOjectsFromListBox(Object obj)
        {
            return false;
        }

        protected virtual bool AddObjectToListBox()
        {
            return false;
        }
    }
}
