using System.Drawing;
using System.Windows.Forms;

namespace Contal.IwQuick.PlatformPC.UI
{
    public class ProgressBarWithText : ProgressBar
    {
        const int WmPaint = 15;

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            switch (m.Msg)
            {
                case WmPaint:
                    string text = (int)(((double)Value / Maximum) * 100) + "%";

                    using (var graphics = Graphics.FromHwnd(Handle))
                    {
                        var textSize = graphics.MeasureString(text, Font);

                        using (var textBrush = new SolidBrush(Color.Black))
                            graphics.DrawString(text,
                                Font,
                                textBrush,
                                (Width / 2) - (textSize.Width / 2),
                                (Height / 2) - (textSize.Height / 2));
                    }
                    break;
            }
        }
    }
}
