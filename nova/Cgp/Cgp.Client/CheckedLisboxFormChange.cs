using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Contal.IwQuick.Data;
using Contal.IwQuick.Localization;
using Contal.IwQuick.Sys;

namespace Contal.Cgp.Client
{
    public partial class CheckedLisboxFormChange :
#if DESIGNER
    Form
#else
    CgpTranslateForm
#endif
    {
        private readonly SyncDictionary<object, bool> _isCheckedByObject;
        private bool _wasCanceled;
        private int _lastIndex = -1;

        public CheckedLisboxFormChange(
            string formText,
            IEnumerable<object> objects,
            Func<object, bool> lambdaIsChecked)
        {
            InitComponent(formText);

            _isCheckedByObject = new SyncDictionary<object, bool>();

            if (objects != null)
            {
                foreach (var o in objects)
                {
                    var isChecked = lambdaIsChecked != null
                                    && lambdaIsChecked(o);

                    _isCheckedByObject[o] = isChecked;
                }

                ShowObjects();
            }
        }

        private void InitComponent(string formText)
        {
            InitializeComponent();
            Text = formText;
        }

        public bool ShowDialog(out ICollection<object> checkedObjects)
        {
            ShowDialog();

            if (_wasCanceled)
            {
                checkedObjects = null;

                return false;
            }

            checkedObjects = new LinkedList<object>();

            foreach (var isCheckedByObject in _isCheckedByObject)
            {
                if (!isCheckedByObject.Value)
                    continue;

                checkedObjects.Add(isCheckedByObject.Key);
            }

            return true;
        }

        private void CheckedLisboxFormAdd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                _bCancel_Click(null, null);
        }

        private void _bOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            _wasCanceled = true;
            Close();
        }

        private void _bClear_Click(object sender, EventArgs e)
        {
            foreach (var o in _isCheckedByObject.KeysSnapshot)
            {
                _isCheckedByObject[o] = false;
            }

            ShowObjects();
        }

        private void _eFilter_KeyUp(object sender, KeyEventArgs e)
        {
            ShowObjects();
        }

        private void ShowObjects()
        {
            _chblbObjects.SuspendLayout();

            _chblbObjects.Items.Clear();

            var objectsCount = _isCheckedByObject.Count;
            int visibleObjectsCount = 0;

            foreach (var isCheckedByObject in _isCheckedByObject.PairsSnapshot)
            {
                var filterText = _eFilter.Text.ToLower();

                if (!string.IsNullOrEmpty(filterText)
                    && !isCheckedByObject.Key.ToString().ToLower().Contains(filterText))
                {
                    continue;
                }

                _chblbObjects.Items.Add(
                    isCheckedByObject.Key,
                    isCheckedByObject.Value);

                visibleObjectsCount++;
            }

            _chblbObjects.ResumeLayout();

            _lShownNumber.Text = string.Format(
                "{0}/{1}",
                visibleObjectsCount,
                objectsCount);
        }

        private void _chblbObjects_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            try
            {
                var changedObject = _chblbObjects.Items[e.Index];

                _isCheckedByObject[changedObject] = e.NewValue == CheckState.Checked;
            }
            catch (Exception error)
            {
                HandledExceptionAdapter.Examine(error);
            }
            
        }

        private void _chblbObjects_MouseClick(object sender, MouseEventArgs e)
        {
            var selectedIndex = _chblbObjects.SelectedIndex;

            if (_lastIndex == selectedIndex)
                return;

            _lastIndex = selectedIndex;

            if (e.Button != MouseButtons.Left)
                return;

            if (e.X > _chblbObjects.ItemHeight)
                return;

            _chblbObjects.SetItemChecked(
                selectedIndex,
                !_chblbObjects.GetItemChecked(selectedIndex));
        }
    }
}
