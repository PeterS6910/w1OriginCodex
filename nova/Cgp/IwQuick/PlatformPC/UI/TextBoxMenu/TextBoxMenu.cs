using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Contal.IwQuick.PlatformPC.Properties;

namespace Contal.IwQuick.UI
{
    public partial class TextBoxMenu : UserControl
    {
        private Color _buttonHoverColor = SystemColors.GradientActiveCaption;
        private Color _buttonBaseColor;

        private readonly ContextMenuStrip _buttonPopupMenu;
        private MenuPosition _buttonPosition = MenuPosition.Right;

        public delegate void DPopupMenuHandler(ToolStripItem item, int index);
        public event DPopupMenuHandler ButtonPopupMenuItemClick;

        private int _buttonSizeWidth = 20;
        private int _buttonSizeHeight = 20;

        private string _buttonText = string.Empty;
        private bool _buttonShowImage = true;
        private bool _buttonDefaultBehaviour = true;
        private int _hoverTime = 500;

        private Image _buttonImage = Resources.PopupMenu;
        private Timer _timer;


        #region Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ImageTextBox ImageTextBox
        {
            get { return _itbTextBox; }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ButtonMenu Button
        {
            get { return _bMenu; }
        }

        [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ContextMenuStrip ButtonPopupMenu
        {
            get { return _buttonPopupMenu; }
        }

        [Browsable(true), Editor(typeof(MenuPositionEditor), typeof(UITypeEditor))]
        public MenuPosition ButtonPosition
        {
            get
            {
                return _buttonPosition;
            }
            set
            {
                _buttonPosition = value;
                SetMenuPosition();
            }
        }

        public Image TextImage
        {
            get { return _itbTextBox.PictureBox.Image; }
            set
            {
                _itbTextBox.PictureBox.Image = value;               
            }
        }

        public new string Text
        {
            get { return _itbTextBox.Text; }
            set
            {
                _itbTextBox.Text = value;               
            }
        }

        private Size _size = new Size(120, 20);
        public new Size Size
        {
            get { return _size; }
            set
            {
                _size = value;
                SetControlsSize();
            }
        }

        private void SetControlsSize()
        {
            base.Size = _size;
            _itbTextBox.Width = _size.Width - _buttonSizeWidth;
        }

        public bool ButtonDefaultBehaviour
        {
            get { return _buttonDefaultBehaviour; }
            set
            {
                _buttonDefaultBehaviour = value;
                if (_buttonDefaultBehaviour)
                {
                    _buttonPopupMenu.ItemClicked += _popupMenu_ItemClicked;
                    _buttonPopupMenu.MouseLeave += _popupMenu_MouseLeave;
                    _bMenu.Click += _bMenu_Click;
                    _bMenu.MouseLeave += _bMenu_MouseLeave;
                    _bMenu.MouseHover += _bMenu_MouseHover;
                    _timer = new Timer();
                    _timer.Tick += _timer_Tick;
                }
            }
        }

        public Color ButtonBaseColor
        {
            get { return _buttonBaseColor; }
            set
            {
                _buttonBaseColor = value;
                _bMenu.BackColor = _buttonBaseColor;
            }
        }

        public Color ButtonHoverColor
        {
            get { return _buttonHoverColor; }
            set
            {
                _buttonHoverColor = value;
            }
        }

        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                _buttonText = value;
                if (_buttonText == string.Empty)
                {
                    if ((_buttonImage == null) && (_buttonShowImage))
                    {
                        _bMenu.Image = Resources.PopupMenu;
                    }
                    else if (_buttonImage != null && (_buttonShowImage))
                    {
                        _bMenu.Image = _buttonImage;
                    }
                }
                else
                {
                    _bMenu.Image = null;
                }
                _bMenu.Text = _buttonText;
            }
        }

        public Image ButtonImage
        {
            get { return _buttonImage; }
            set
            {
                _buttonImage = value;
                SetImage();
            }
        }

        public bool ButtonShowImage
        {
            get { return _buttonShowImage; }
            set
            {
                _buttonShowImage = value;
                SetImage();
            }
        }

        public int ButtonSizeWidth
        {
            get { return _buttonSizeWidth; }
            set
            {
                if (value < _itbTextBox.Width)
                {
                    _buttonSizeWidth = value;
                    SetButtonWidth();
                }
                else
                {
                    throw new Exception("Button width must be lower than textbox width");
                }
            }
        }

        public int ButtonSizeHeight
        {
            get { return _buttonSizeHeight; }
            set
            {
                _buttonSizeHeight = value;
                SetButtonHeight();
            }
        }

        [Description("Time in milliseconds to show menu after mouse hover event. (ms)")]
        public int HoverTime
        {
            get { return _hoverTime; }
            set
            {
                _hoverTime = value;
                if (_timer != null)
                {
                    _timer.Interval = _hoverTime;
                }
            }
        }

        public override Cursor Cursor
        {
            get { return _itbTextBox.Cursor; }
            set
            {
                /* Cursor is changed internally */
                _bMenu.Cursor = value;
            }
        }
        #endregion

        public TextBoxMenu()
        {
            InitializeComponent();
            InitButtonSettings();
            Text = string.Empty;
            _buttonPopupMenu = new ContextMenuStrip();
            _itbTextBox.ContextMenuStrip = _buttonPopupMenu;
            Cursor = Cursors.Default;
        }

        void _bMenu_MouseLeave(object sender, EventArgs e)
        {
            _timer.Stop();
        }

        void _bMenu_Click(object sender, EventArgs e)
        {
            if ((_buttonPopupMenu != null))
            {
                if (_buttonPopupMenu.Items.Count > 0)
                {
                    _bMenu.BackColor = _buttonHoverColor;
                    _buttonPopupMenu.Show(new Point(Cursor.Position.X - 5, Cursor.Position.Y - 5));
                }
            }
        }

        void _bMenu_MouseHover(object sender, EventArgs e)
        {
            if (!_timer.Enabled)
            {
                _timer.Start();
            }
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            if ((_buttonPopupMenu != null))
            {
                if (_buttonPopupMenu.Items.Count > 0)
                {
                    _bMenu.BackColor = _buttonHoverColor;
                    _buttonPopupMenu.Show(new Point(Cursor.Position.X - 5, Cursor.Position.Y - 5));
                }
            }
            _timer.Stop();
        }

        void _popupMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (ButtonPopupMenuItemClick != null)
            {
                ButtonPopupMenuItemClick(e.ClickedItem, _buttonPopupMenu.Items.IndexOf(e.ClickedItem));
            }
            _bMenu.BackColor = _buttonBaseColor;
            _timer.Stop();
        }

        void _popupMenu_MouseLeave(object sender, EventArgs e)
        {
            _buttonPopupMenu.Close();
            _bMenu.BackColor = _buttonBaseColor;
            _timer.Stop();
        }

        private void InitButtonSettings()
        {
            _buttonBaseColor = _bMenu.BackColor;
            _buttonHoverColor = Color.Empty;
            _buttonSizeHeight = _bMenu.Height;
            _buttonSizeWidth = _bMenu.Width;
            _buttonImage = _bMenu.Image;
            _buttonText = _bMenu.Text;
        }

        private void SetImage()
        {
            if ((_buttonImage == null) && (_buttonText == string.Empty) && (_buttonShowImage))
            {
                _bMenu.Image = Resources.PopupMenu;
            }
            else if ((_buttonImage != null) && (_buttonText == string.Empty) && (_buttonShowImage))
            {
                _bMenu.Image = _buttonImage;
            }
            else if (!_buttonShowImage)
            {
                _bMenu.Image = null;
            }
        }

        private void SetMenuPosition()
        {
            _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            _bMenu.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            _bMenu.Size = new Size(_buttonSizeWidth, _buttonSizeHeight);

            MinimumSize = new Size(0, 0);
            MaximumSize = new Size(1200, 1200);

            switch (_buttonPosition)
            {
                //case MenuPosition.Left:
                //    {
                //        _bMenu.Location = new Point(0, 0);
                //        _itbTextBox.Location = new Point(_bMenu.Width, 0);
                //        this.Size = new Size(_itbTextBox.Width + _bMenu.Width + 1,
                //            (_bMenu.Height > _itbTextBox.Height ? _bMenu.Height + 2 : _itbTextBox.Height + 2));
                //        _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                //        _bMenu.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                //    }
                //    break;
                //case MenuPosition.LeftTop:
                //    {
                //        _bMenu.Location = new Point(0, 0);
                //        _itbTextBox.Location = new Point(0, _bMenu.Height + 1);
                //        this.Size = new Size(_itbTextBox.Width, _itbTextBox.Height + 2 + _bMenu.Height);
                //        _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                //        _bMenu.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                //    }
                //    break;
                //case MenuPosition.LeftBottom:
                //    {
                //        _bMenu.Location = new Point(0, _itbTextBox.Height + 1);
                //        _itbTextBox.Location = new Point(0, 0);
                //        this.Size = new Size(_itbTextBox.Width, _itbTextBox.Height + 2 + _bMenu.Height);
                //        _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                //        _bMenu.Anchor = AnchorStyles.Left | AnchorStyles.Top;
                //    }
                //    break;
                case MenuPosition.Right:
                    {
                        _bMenu.Location = new Point(_itbTextBox.Width, 0);
                        _itbTextBox.Location = new Point(0, 0);
                        Size = new Size(_itbTextBox.Width + _bMenu.Width,
                            (_bMenu.Height > _itbTextBox.Height ? _bMenu.Height + 2 : _itbTextBox.Height));
                        _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                        _bMenu.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    }
                    break;
                //case MenuPosition.RightTop:
                //    {
                //        _bMenu.Location = new Point(_itbTextBox.Width - _bMenu.Width, 1);
                //        _itbTextBox.Location = new Point(0, _bMenu.Height + 1);
                //        this.Size = new Size(_itbTextBox.Size.Width, _itbTextBox.Height + 2 + _bMenu.Height);
                //        _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                //        _bMenu.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                //    }
                //    break;
                //case MenuPosition.RightBottom:
                //    {
                //        _bMenu.Location = new Point(_itbTextBox.Width - _bMenu.Width, _itbTextBox.Height + 1);
                //        _itbTextBox.Location = new Point(0, 0);
                //        this.Size = new Size(_itbTextBox.Size.Width, _itbTextBox.Height + 2 + _bMenu.Height);
                //        _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                //        _bMenu.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                //    }
                //    break;
                //case MenuPosition.Top:
                //    {
                //        _bMenu.Location = new Point(0, 0);
                //        _itbTextBox.Location = new Point(0, _bMenu.Height + 1);
                //        this.Size = new Size(_itbTextBox.Width, _itbTextBox.Height + 2 + _bMenu.Height);
                //        _bMenu.Size = new Size(_itbTextBox.Width, _bMenu.Height);//stretch
                //        _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
                //        _bMenu.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                //    }
                //    break;
                //case MenuPosition.Bottom:
                //    {
                //        _bMenu.Location = new Point(0, _itbTextBox.Height + 1);
                //        _itbTextBox.Location = new Point(0, 0);
                //        this.Size = new Size(_itbTextBox.Width, _itbTextBox.Height + 2 + _bMenu.Height);
                //        _bMenu.Size = new Size(_itbTextBox.Width, _bMenu.Height);//stretch
                //        _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                //        _bMenu.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                //    }
                //    break;
                default:
                    {
                        _bMenu.Location = new Point(_itbTextBox.Width, 0);
                        _itbTextBox.Location = new Point(0, 0);
                        Size = new Size(_itbTextBox.Width + _bMenu.Width,
                            (_bMenu.Height > _itbTextBox.Height ? _bMenu.Height + 2 : _itbTextBox.Height));
                        _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                        _bMenu.Anchor = AnchorStyles.Right | AnchorStyles.Top;
                    }
                    break;
            }

            MinimumSize = new Size(_bMenu.Width + 1, Height);
            MaximumSize = new Size(1200, Height);
        }

        private void SetButtonWidth()
        {
            //this.MinimumSize = new System.Drawing.Size(0, 0);
            //if (_buttonPosition == MenuPosition.LeftTop || _buttonPosition == MenuPosition.LeftBottom)
            //{
            //    _bMenu.Width = _buttonSizeWidth;
            //}
            //else if (_buttonPosition == MenuPosition.RightTop || _buttonPosition == MenuPosition.RightBottom)
            //{
            //    _bMenu.Location = new Point(_itbTextBox.Width - _buttonSizeWidth, _bMenu.Location.Y);
            //    _bMenu.Width = _buttonSizeWidth;
            //}
            //else if (_buttonPosition == MenuPosition.Left)
            //{
            //    _itbTextBox.Anchor = AnchorStyles.Right;
            //    _bMenu.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            //    this.Size = new Size(_itbTextBox.Width + _buttonSizeWidth + 1,
            //        (_bMenu.Height > _itbTextBox.Height ? _buttonSizeHeight + 2 : _itbTextBox.Height + 2));
            //    _bMenu.Width = _buttonSizeWidth;
            //    _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            //    _bMenu.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            //}
            //else if (_buttonPosition == MenuPosition.Right)
            //{
            _itbTextBox.Anchor = AnchorStyles.Left;
            _bMenu.Anchor = AnchorStyles.Right;
            Size = new Size(_itbTextBox.Width + _buttonSizeWidth,
                (_bMenu.Height > _itbTextBox.Height ? _buttonSizeHeight + 2 : _itbTextBox.Height));
            _bMenu.Width = _buttonSizeWidth;
            _itbTextBox.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            _bMenu.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            //}
            //else if (_buttonPosition == MenuPosition.Top || _buttonPosition == MenuPosition.Bottom)
            //{
            //    //dont set it automaticly stretch
            //}

            MinimumSize = new Size(_bMenu.Width + 1, Height);
        }

        private void SetButtonHeight()
        {
            MinimumSize = new Size(0, 0);
            MaximumSize = new Size(1200, 1200);
            if (!(_buttonPosition == MenuPosition.Left || _buttonPosition == MenuPosition.Right))
            {
                Height = _buttonSizeHeight + _itbTextBox.Height;
            }
            else
            {
                Height = (_buttonSizeHeight > _itbTextBox.Height ? _buttonSizeHeight : _itbTextBox.Height);
            }
            _bMenu.Height = _buttonSizeHeight;

            //if (GetDPI() == 120)
            //{
            //    _bMenu.Height = _bMenu.Height + 2;
            //}

            MinimumSize = new Size(_bMenu.Width + 1, Height);
            MaximumSize = new Size(1200, Height);
        }
    }

    /// <summary>
    /// Transparent panel
    /// </summary>
    [ToolboxItem(false)]
    public class TransparentPanel : Panel
    {
        //public TransparentPanel() { }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams createParams = base.CreateParams;
                createParams.ExStyle |= 0x00000020; // WS_EX_TRANSPARENT
                return createParams;
            }
        }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Do not paint background.
        }
    }

    internal class MenuPositionEditor : UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService editorService = null;
// ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (provider != null)
            {
                editorService = provider.GetService(typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;
            }

            if (editorService != null)
            {
                if (value != null)
                {
                    MenuSelectionControl selectionControl = new MenuSelectionControl((MenuPosition)value, editorService);
                    editorService.DropDownControl(selectionControl);
                    value = selectionControl.MenuPosition;
                }
            }

// ReSharper disable once AssignNullToNotNullAttribute
            return value;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }

    //Inherited controls to hide some of their properties   
    [ToolboxItem(false)]
    public class ButtonMenu : Button
    {
        [Browsable(false)]
        public new Size Size
        {
            get { return base.Size; }
            set { base.Size = value; }
        }

        [Browsable(false)]
        public override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }

        /// <summary>
        /// Do NOT use Transparent color - causes exception when showing Form in Visual Studio Designer!
        /// </summary>
        [Browsable(false)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }

        [Browsable(false)]
        public new Image Image
        {
            get { return base.Image; }
            set { base.Image = value; }
        }

        [Browsable(false)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get { return base.ContextMenuStrip; }
            set { base.ContextMenuStrip = value; }
        }
    }
}
