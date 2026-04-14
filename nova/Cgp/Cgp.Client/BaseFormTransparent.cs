using System;
using System.Drawing;
using System.Windows.Forms;

namespace Contal.Cgp.Client
{
    public partial class BaseFormTransparent : Form
    {
#if DESIGNER
        public static Color _TransparentColor = Color.Red;
#else
        public static Color _TransparentColor = Color.Transparent;
#endif        
        public BaseFormTransparent()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }
    }
}
