using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Contal.IwQuick.PlatformPC.Properties;

namespace Contal.IwQuick.UI
{
    public partial class ImageTextBox : UserControl
    {
        private const int TEXT_BOX_WIDTH_ADJUST = 3;

        public new event EventHandler TextChanged;
        public new event EventHandler DoubleClick;

        private bool _useImage = false;
        private bool _noTextNoImage = true;
        //private readonly Image _defaultImage = Resources.IconDefaultImage.ToBitmap();

        public override string Text
        {
            get { return _tbTextBox.Text; }
            set
            {
                _tbTextBox.Text = value;
                if ((_noTextNoImage) && string.IsNullOrEmpty(_tbTextBox.Text))
                {
                    SetImageVisibility(false);
                }
                else
                {
                    SetImageVisibility(true);
                }
            }
        }

        public override Cursor Cursor
        {
            get { return _tbTextBox.Cursor; }
            set { /* Cursor is changed internally */ }
        }

        public bool ReadOnly
        {
            get { return _tbTextBox.ReadOnly; }
            set
            {
                _tbTextBox.ReadOnly = value;
            }
        }

        public bool UseImage
        {
            get { return _useImage; }
            set
            {
                _useImage = value;
                if ((_noTextNoImage) && string.IsNullOrEmpty(_tbTextBox.Text))
                {
                    SetImageVisibility(false);
                }
                else
                {
                    SetImageVisibility(_useImage);
                }
            }
        }

        public bool NoTextNoImage
        {
            get { return _noTextNoImage; }
            set
            {
                _noTextNoImage = value;
                if (!_noTextNoImage)
                {
                    SetImageVisibility(true);
                }
                else
                {
                    if (string.IsNullOrEmpty(_tbTextBox.Text))
                    {
                        SetImageVisibility(false);
                    }
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public TextBox TextBox
        {
            get { return _tbTextBox; }
            set { _tbTextBox = value; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public PictureBox PictureBox
        {
            get { return _pbPictureBox; }
            set { _pbPictureBox = value; }
        }

        public Image Image
        {
            get { return _pbPictureBox.Image; }
            set
            {
                _pbPictureBox.Image = value;
            }
        }

        public ImageTextBox()
        {
            InitializeComponent();
            //Image = _defaultImage;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle &= (~NativeMethods.WS_EX_CLIENTEDGE);
                cp.Style &= (~NativeMethods.WS_BORDER);

                switch (BorderStyle)
                {
                    case BorderStyle.Fixed3D:
                        //used to drow border similar to textBox border   
                        cp.ExStyle |= NativeMethods.WS_EX_STATICEDGE;
                        break;

                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                }
                return cp;
            }
        }

        public void SetImageVisibility(bool visible)
        {
            if ((visible) && (_useImage))
            {
                _pbPictureBox.Visible = true;
                _tbTextBox.Size = new Size(
                    Width - (_pbPictureBox.Width + _pbPictureBox.Location.X) - TEXT_BOX_WIDTH_ADJUST, _tbTextBox.Height);

                _tbTextBox.Location = new Point(
                    _pbPictureBox.Location.X + _pbPictureBox.Width + 2, _tbTextBox.Location.Y);

                _tbTextBox.Cursor = Cursors.Hand;
            }
            else
            {
                _pbPictureBox.Visible = false;
                _tbTextBox.Size = new Size(Width - 2, _tbTextBox.Height);
                _tbTextBox.Location = new Point(1, _tbTextBox.Location.Y);

                _tbTextBox.Cursor = Cursors.Default;
            }
        }

        private void SetBackgroundColor(Color color)
        {
            _pbPictureBox.BackColor = color;
            _tbTextBox.BackColor = color;
        }

        private void ImageTextBox_BackColorChanged(object sender, EventArgs e)
        {
            SetBackgroundColor(BackColor);
        }

        private void _tbTextBox_TextChanged(object sender, EventArgs e)
        {
            if (TextChanged != null)
            {
                TextChanged(sender, e);
            }
        }

        private void _tbTextBox_DoubleClick(object sender, EventArgs e)
        {
            if (DoubleClick != null)
            {
                DoubleClick(sender, e);
            }
        }

        private void _pbPictureBox_DoubleClick(object sender, EventArgs e)
        {
            if (DoubleClick != null)
            {
                DoubleClick(sender, e);
            }
        }
    }

    public class NativeMethods
    {
        public const int WS_POPUP = unchecked((int)0x80000000);
        public const int WS_CHILD = unchecked(0x40000000);
        public const int WS_MINIMIZE = unchecked(0x20000000);
        public const int WS_VISIBLE = unchecked(0x10000000);
        public const int WS_DISABLED = unchecked(0x08000000);
        public const int WS_CLIPSIBLINGS = unchecked(0x04000000);
        public const int WS_CLIPCHILDREN = unchecked(0x02000000);
        public const int WS_MAXIMIZE = unchecked(0x01000000);
        public const int WS_CAPTION = unchecked(0x00C00000);
        public const int WS_BORDER = unchecked(0x00800000);
        public const int WS_DLGFRAME = unchecked(0x00400000);
        public const int WS_VSCROLL = unchecked(0x00200000);
        public const int WS_HSCROLL = unchecked(0x00100000);
        public const int WS_SYSMENU = unchecked(0x00080000);
        public const int WS_THICKFRAME = unchecked(0x00040000);
        public const int WS_TABSTOP = unchecked(0x00010000);
        public const int WS_MINIMIZEBOX = unchecked(0x00020000);
        public const int WS_MAXIMIZEBOX = unchecked(0x00010000);
        public const int WS_EX_DLGMODALFRAME = unchecked(0x00000001);
        public const int WS_EX_MDICHILD = unchecked(0x00000040);
        public const int WS_EX_TOOLWINDOW = unchecked(0x00000080);
        public const int WS_EX_CLIENTEDGE = unchecked(0x00000200);
        public const int WS_EX_CONTEXTHELP = unchecked(0x00000400);
        public const int WS_EX_RIGHT = unchecked(0x00001000);
        public const int WS_EX_LEFT = unchecked(0x00000000);
        public const int WS_EX_RTLREADING = unchecked(0x00002000);
        public const int WS_EX_LEFTSCROLLBAR = unchecked(0x00004000);
        public const int WS_EX_CONTROLPARENT = unchecked(0x00010000);
        public const int WS_EX_STATICEDGE = unchecked(0x00020000);
        public const int WS_EX_APPWINDOW = unchecked(0x00040000);
        public const int WS_EX_LAYERED = unchecked(0x00080000);
        public const int WS_EX_TOPMOST = unchecked(0x00000008);
        public const int WS_EX_NOPARENTNOTIFY = unchecked(0x00000004);
    }
}
