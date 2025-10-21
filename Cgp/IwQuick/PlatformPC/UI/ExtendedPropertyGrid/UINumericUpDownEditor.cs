using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Contal.IwQuick.UI
{
    public class UINumericUpDownEditor : UITypeEditor
    {
        private NumericUpDown nup = new NumericUpDown();

        public UINumericUpDownEditor()
        {
            nup.Maximum = 730;
        }

        public UINumericUpDownEditor(
            int minimum,
            int maximum)
        {
            nup.Minimum = minimum;
            nup.Maximum = maximum;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;
            if (provider != null)
            {
                editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }

            if (editorService != null)
            {
                nup.Value = Decimal.Parse(value.ToString());
                editorService.DropDownControl(nup);
                value = nup.Value.ToString();
            }
            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
