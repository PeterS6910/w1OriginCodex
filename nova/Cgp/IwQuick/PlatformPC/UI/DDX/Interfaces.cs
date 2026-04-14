using System;
using System.Windows.Forms;

namespace Contal.IwQuick.UI.DDX
{
    [Obsolete]
    public interface IDDXLink<T>
    {
        Control Control { get; }

        void Data2UI();
        void UI2Data();
    }

}
