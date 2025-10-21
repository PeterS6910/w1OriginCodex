using System;
using System.Globalization;

namespace Contal.IwQuick.UI.DDX
{
    [Obsolete]
    public class DDXIntLink: ADDXLink<int>
    {
        public DDXIntLink(ref int externalValue, System.Windows.Forms.Control control)
            :base(ref externalValue,control)
        {
        }

        protected override void SetData2UI(int data, System.Windows.Forms.Control control)
        {
            control.Text = data.ToString(CultureInfo.InvariantCulture);
        }

        protected override bool SetUI2Data(System.Windows.Forms.Control control, ref int data)
        {
            if (control.Text.Length == 0)
                return false;

            return int.TryParse(control.Text,out data);
        }
    }
}
