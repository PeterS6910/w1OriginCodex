using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;

namespace Contal.IwQuick.UI
{
    public class UIComboBoxEditor : UITypeEditor
    {
        CheckBox cb = new CheckBox();
        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.None;
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
                cb.Checked = (bool)value;
                value = (bool)cb.Checked;
            }
            return value;
        }
        
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override void PaintValue(PaintValueEventArgs e)
        {
            ControlPaint.DrawCheckBox(e.Graphics, e.Bounds, (bool)e.Value ? ButtonState.Checked : ButtonState.Normal);
        }
    }
    //public class CustomIntConverter : DecimalConverter
    //{
    //    private int[] values;
    //    public CustomIntConverter()
    //    {
    //        values = new int[731];
    //        for (int i = 0; i < 731; i++)
    //        {
    //            values[i] = i;
    //        }
    //    }

    //    public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
    //    {
    //        return true;// show drop-down  
    //    }
    //    public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
    //    {
    //        return true;// true - not allow value set from editor
    //    }
    //    public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
    //    {
    //        return new StandardValuesCollection(values);
    //    }
    //}
}
