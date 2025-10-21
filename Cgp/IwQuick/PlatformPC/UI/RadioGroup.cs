using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;

using System.Diagnostics;

namespace Contal.IwQuick.UI
{
    public class RadioGroup : ListControl
    {
        private GroupBox _radioGroup = null;
        private bool _reverseOrder = true;

        private int _refCount = -1;
        private int _selectedIndex = -1;

        private List<RadioButton> _radioButtons = new List<RadioButton>(4);

        public RadioGroup(GroupBox groupBox)
            : this(groupBox, true)
        {

        }

        public RadioGroup(GroupBox groupBox, bool isReverseOrder)
        {
            if (null == groupBox)
                throw new ArgumentNullException("groupBox");

            _radioGroup = groupBox;
            // must be set before reindexation
            _reverseOrder = isReverseOrder;

            int iReindexCount = Count;
            int iReindexSelectIndex = SelectedIndex;
        }

        public int Count
        {
            get
            {
                if (_refCount == _radioGroup.Controls.Count)
                    return _radioButtons.Count;

                // refresh, if the referencing count does not match the group controls count

                // unregister events for old radiobutton set
                foreach (RadioButton aRadio in _radioButtons)
                    try { aRadio.CheckedChanged -= LocalCheckedChange; }
                    catch { }

                _radioButtons.Clear();
                foreach (Control aControl in _radioGroup.Controls)
                    if (aControl is RadioButton)
                    {
                        RadioButton aRadio = (RadioButton)aControl;

                        aRadio.CheckedChanged += LocalCheckedChange;

                        if (_reverseOrder)
                            _radioButtons.Insert(0, aRadio);
                        else
                            _radioButtons.Add(aRadio);
                    }

                _refCount = _radioGroup.Controls.Count;
                return _radioButtons.Count;
            }
        }

        private void LocalCheckedChange(Object sender, EventArgs e)
        {
            if (_previousData2UIOverride)
            {
                return;
            }

            // automatically reacts only for UI checked to true
            Debug.Assert(null != sender);
            if (null != CheckedChange)
            {
                RadioButton aRadio = (RadioButton)sender;
                if (aRadio.Checked)
                    CheckedChange(this, e);
            }
        }

        public event EventHandler CheckedChange;

        protected override void RefreshItem(int index)
        {

        }

        private bool _previousData2UIOverride = false;
        protected void LockRecursiveUIRead()
        {
            if (_previousData2UIOverride)
                return;

            _previousData2UIOverride = true;

        }

        protected void UnlockRecursiveUIRead()
        {
            if (!_previousData2UIOverride)
                return;

            _previousData2UIOverride = false;
        }

        public override int SelectedIndex
        {
            get
            {
                int iRet = -1;

                foreach (RadioButton aRadio in _radioButtons)
                {
                    iRet++;
                    if (aRadio.Checked)
                    {
                        _selectedIndex = iRet;
                        return iRet++;
                    }
                }

                _selectedIndex = -1;
                return -1;
            }
            set
            {
                LockRecursiveUIRead();

                if (value < 0 ||
                    value >= Count)
                {
                    for (int i = 0; i < _radioButtons.Count; i++)
                        _radioButtons[i].Checked = false;

                    _selectedIndex = -1;
                }
                else
                {
                    _radioButtons[value].Checked = true;
                    _selectedIndex = value;
                }

                UnlockRecursiveUIRead();
            }
        }

        protected override void SetItemsCore(System.Collections.IList items)
        {

        }
    }
}
