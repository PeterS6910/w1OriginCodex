using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cgp.Components
{
    public class CgpTabToolStripItemButton : ToolStripButton
    {
        public const int ImageSize = 16;

        public event EventHandler<EventArgs> CloseImageClick;

        Image _closeImage, _closeImageOn;
        Rectangle _closeImageRectangle = new Rectangle(0, 0, ImageSize, ImageSize);
        bool _isClosing = false;

        public CgpTabToolStripItemButton(string text, Image image, Image closeImage, Image closeImageOn)
        {
            Text = text;
            Image = image;

            _closeImage = closeImage;
            _closeImageOn = closeImageOn;

            //MouseMove += _MouseMove;
            MouseDown += _MouseDown;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Calculate the rectangle where to draw the Close image
            _closeImageRectangle.X = ContentRectangle.Width - ImageSize - Padding.Right - Padding.Left;
            _closeImageRectangle.Y = (ContentRectangle.Height - ImageSize) / 2;

            // Rectangle around whole control
            //e.Graphics.DrawRectangle(new Pen(Color.LightBlue), ContentRectangle);

            e.Graphics.DrawImage(BackColor == Color.LightBlue ? _closeImageOn : _closeImage, _closeImageRectangle);
            //e.Graphics.DrawImage(_isClosing ? _closeImageOn : _closeImage, _closeImageRectangle);
        }

        private void _MouseMove(object sender, MouseEventArgs e)
        {
            // If cursor is moved over the area of Close icon highlight it
            _isClosing = _closeImageRectangle.Contains(new Point(e.X, e.Y));
        }

        private void _MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _closeImageRectangle.Contains(new Point(e.X, e.Y)))
            {
                // Close the current window
                CloseImageClick(this, e);
            }
        }
    }
}
