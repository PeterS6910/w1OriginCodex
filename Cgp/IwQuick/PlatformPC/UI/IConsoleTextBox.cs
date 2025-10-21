using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

namespace Contal.IwQuick.UI
{
    public interface IConsoleTextBox
    {
        bool AutoScroll { get; set; }
        bool RenderLocalInput { get; set; }
        bool HexMode { get; set; }
        int MaxLines { get; set; }
        void Push(string message);
        event DChar2Void OnInput;        
        void Clear();
    }
}
