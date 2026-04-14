using System;
using System.Drawing;
using System.Windows.Forms;
using Contal.IwQuick.PlatformPC.Properties;

namespace Contal.IwQuick.UI
{
    #region Enums
    public enum TextBoxBrowserMode
    {
        Open = 1,
        Save = 2
    }      

    public enum TextBoxBrowserEntity
    {
        File = 1,
        Directory = 2
    }
    #endregion        

    /// <summary>
    /// User control combining TextBox and Button for browsing files (Open/Save Filedialog), folders (FileBrowserDialog), with path appearing in TextBox    
    /// </summary>    
    public partial class TextBoxBrowser : UserControl
    {
        //Control atributes and properties
        #region Control

        int _buttonWidth = 23;
        int _buttonHeight = 23; 
        bool _readOnly = false;
        private bool _multiline = false;
        string _buttonText = string.Empty;
        //string _textBoxText = string.Empty;
      
        Image _backgroundButtonImage;
        ImageLayout _backgroundButtonImageLayout = ImageLayout.Zoom;
        Color _textBoxBackColor = Color.White;  

        TextBoxBrowserMode _mode = TextBoxBrowserMode.Open;       
        TextBoxBrowserEntity _entity = TextBoxBrowserEntity.File;

        //Event on Text change in textBoxPath
        public delegate void UserControlEventHandler(Object sender, EventArgs e);      
        public event UserControlEventHandler TextBoxTextChanged;
        public event UserControlEventHandler ButtonClicked;
        

        /// <summary>
        /// Gets or sets button width
        /// </summary>
        public int ButtonWidth
        {
            get { return _buttonWidth; }
            set
            {
                _buttonWidth = value;                
                textBoxPath.Size = new Size((Size.Width - (buttonBrowse.Width + 3)), textBoxPath.Size.Height);
                textBoxPath.Anchor = AnchorStyles.Left;
                buttonBrowse.Anchor = AnchorStyles.Left | AnchorStyles.Right;            
                Size = new Size(textBoxPath.Size.Width + (value + 3), Size.Height);
                buttonBrowse.Size = new Size(Size.Width - (textBoxPath.Size.Width + 3), buttonBrowse.Size.Height);
                buttonBrowse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                textBoxPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
                MinimumSize = new Size((buttonBrowse.Size.Width + 6), 22);
            }
        }

        /// <summary>
        /// Gets or sets button height
        /// </summary>
        public int ButtonHeight
        {
            get { return _buttonHeight; }
            set
            {
                _buttonHeight = value;
                buttonBrowse.Size = new Size(buttonBrowse.Size.Width, _buttonHeight);
                Size = new Size(Width, _buttonHeight - 2 > textBoxPath.Height ? _buttonHeight - 1 : textBoxPath.Height + 2);
                if (WinFormsHelper.GetCurrentDPI() == ScreenDpi.Dpi120)
                {
                    buttonBrowse.Height = ButtonHeight + 1;
                }
            }
        }
    
        /// <summary>
        /// Gets or sets if textBox in control is readOnly
        /// </summary>
        public bool ReadOnly
        {
            get { return _readOnly; }
            set
            {
                _readOnly = value;
                if (_readOnly)
                {
                    textBoxPath.ReadOnly = true;
                    textBoxPath.BackColor = Color.White;
                }
                else
                {
                    textBoxPath.ReadOnly = false;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets button text, when text is empty button have default picture
        /// </summary>
        public string ButtonText
        {
            get { return _buttonText; }
            set
            {
                _buttonText = value;
                if (_buttonText == string.Empty)
                {
                    if (_backgroundButtonImage==null)
                    {                   
                        buttonBrowse.BackgroundImage = buttonBrowse.BackgroundImage = Resources.IconFolderSearch.ToBitmap();                        
                    }                    
                    buttonBrowse.Text = value;
                }
                else
                {
                    buttonBrowse.BackgroundImage = null;
                    buttonBrowse.Text = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets textBox text
        /// </summary>
        public string TextBoxText
        {
            get
            {
                return textBoxPath.Text;
            }
            set 
            {
                if (value == null)
                    value = string.Empty;

                //_textBoxText = value;
                textBoxPath.Text = value;
            }
        }

        public override string Text
        {
            get
            {
                return textBoxPath.Text;
            }
            set
            {
                Validator.CheckForNull(value,"value");

                //_textBoxText = value;
                textBoxPath.Text = value;
            }
        }

        /// <summary>
        /// Gets or sets button background image, if it is not set and buttonText property is empty button will have default background picture
        /// </summary>
        public Image BackgroundButtonImage
        {
            get { return _backgroundButtonImage; }
            set 
            {
                _backgroundButtonImage = value;             
                if ((_backgroundButtonImage == null) && (_buttonText == string.Empty))
                {                    
                    buttonBrowse.BackgroundImage = buttonBrowse.BackgroundImage = Resources.IconFolderSearch.ToBitmap();                    
                }
                else if ((_backgroundButtonImage != null) && (_buttonText == string.Empty))
                {               
                    buttonBrowse.BackgroundImage = _backgroundButtonImage;                    
                }
            }
        }
        
        /// <summary>
        /// Gets or sets button background image layout
        /// </summary>
        public ImageLayout BackgroundButtonImageLayout
        {
            get { return _backgroundButtonImageLayout; }
            set 
            { 
                _backgroundButtonImageLayout = value;
                buttonBrowse.BackgroundImageLayout = _backgroundButtonImageLayout;
            }
        }

        /// <summary>
        /// Gets or sets back color on textBox  
        /// </summary>
        public Color TextBoxBackColor
        {
            get { return _textBoxBackColor; }
            set
            {
                _textBoxBackColor = value;
                
            }
        }

        /// <summary>
        /// Gets text from textBox returning null or string path after browse  
        /// </summary>
        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(textBoxPath.Text))
                {
                    return null;
                }
                return textBoxPath.Text;
            }
        }
       
        /// <summary>
        /// Gets or sets if mode is Open(OpenFileDialog) or Save(SaveFileDialog)
        /// </summary>
        public TextBoxBrowserMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        /// <summary>
        /// Gets or sets if Directory (FolderBrowserDialog) or File Open(Open/Save FileDialog) will be open
        /// </summary>
        public TextBoxBrowserEntity Entity
        {
            get { return _entity; }
            set { _entity = value; }
        }

        /// <summary>
        /// Gets or sets if textBox is multiline
        /// </summary>
        public bool Multiline
        {
            get { return _multiline; }
            set
            {
                _multiline = value;
                textBoxPath.Multiline = value;
            }
        }

        #endregion
        //Open/Save dialog atributes and properties
        #region OpenSaveDialog

        bool _addExtension = true;
        bool _autoUpgradeEnabled = true;
        bool _checkFileExists = true;
        bool _checkPathExists = true;
        string _defaultExt = string.Empty;
        bool _dereferenceLinks = true;
        string _fileName = string.Empty;
        string _filter = string.Empty;
        int _filterIndex = 1;
        string _initialDirectory = string.Empty;
        bool _readOnlyChecked = false;       
        bool _restoreDirectory = false;
        bool _showHelp = false;       
        bool _showReadOnly = false;
        string _title = string.Empty;

        #region OpenSaveProperties

        public bool AddExtension
        {
            get { return _addExtension; }
            set { _addExtension = value; }
        }

        public bool AutoUpgradeEnabled
        {
            get { return _autoUpgradeEnabled; }
            set { _autoUpgradeEnabled = value; }
        }

        public bool CheckFileExists
        {
            get { return _checkFileExists; }
            set { _checkFileExists = value; }
        }

        public bool CheckPathExists
        {
            get { return _checkPathExists; }
            set { _checkPathExists = value; }
        }

        public string DefaultExt
        {
            get { return _defaultExt; }
            set { _defaultExt = value; }
        }

        public bool DereferenceLinks
        {
            get { return _dereferenceLinks; }
            set { _dereferenceLinks = value; }
        }

        public string FileName
        {
            get { return textBoxPath.Text; }
            set { _fileName = value; }
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                try
                {
                    OpenFileDialog openDialog = new OpenFileDialog {Filter = value};
                    openDialog.Dispose();
                    _filter = value;
                }
                catch (Exception)
                {
                    throw new Exception();
                }
            }
        }

        public int FilterIndex
        {
            get { return _filterIndex; }
            set { _filterIndex = value; }
        }

        public string InitialDirectory
        {
            get { return _initialDirectory; }
            set { _initialDirectory = value; }
        }
        
        public bool ReadOnlyChecked
        {
            get { return _readOnlyChecked; }
            set { _readOnlyChecked = value; }
        }

        public bool RestoreDirectory
        {
            get { return _restoreDirectory; }
            set { _restoreDirectory = value; }
        }

        public bool ShowHelp
        {
            get { return _showHelp; }
            set { _showHelp = value; }
        }

        public bool ShowReadOnly
        {
            get { return _showReadOnly; }
            set { _showReadOnly = value; }
        }
        
        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        #endregion

        #endregion

        public TextBoxBrowser()
        {
            InitializeComponent();           
        }

// ReSharper disable once UnusedMember.Local
        private void SetButtonHeight(int value)
        {
            buttonBrowse.Height = value;
        }    

        private void Browse()
        {
            if (_entity == TextBoxBrowserEntity.File)
            {
                if (_mode == TextBoxBrowserMode.Open)
                {
                    OpenFileDialog openDialog = new OpenFileDialog();
                    InitOpenFileDialog(openDialog);
                    if (openDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBoxPath.Text = openDialog.FileName;                      
                        Text = textBoxPath.Text;
                    }
                }
                else
                {
                    SaveFileDialog saveDialog = new SaveFileDialog();                    
                    InitSaveFileDialog(saveDialog);
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        textBoxPath.Text = saveDialog.FileName;                      
                        Text = textBoxPath.Text;
                    }
                }
            }
            else
            {
                FolderBrowserDialog folderDialog = new FolderBrowserDialog();              
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxPath.Text = folderDialog.SelectedPath;                  
                    Text = textBoxPath.Text;
                }
            }
        }

        private void InitOpenFileDialog(OpenFileDialog openDialog)
        {
            openDialog.AddExtension = _addExtension;
            openDialog.AutoUpgradeEnabled = _autoUpgradeEnabled;
            openDialog.CheckFileExists = _checkFileExists;
            openDialog.CheckPathExists = _checkPathExists;
            openDialog.DefaultExt = _defaultExt;
            openDialog.DereferenceLinks = _dereferenceLinks;
            openDialog.FileName = _fileName;
            openDialog.Filter = _filter;
            openDialog.FilterIndex = _filterIndex;
            openDialog.InitialDirectory = _initialDirectory;
            openDialog.ReadOnlyChecked = _readOnlyChecked;
            openDialog.RestoreDirectory = _restoreDirectory;
            openDialog.ShowHelp = _showHelp;
            openDialog.ShowReadOnly = _showReadOnly;
            openDialog.Title = _title;
        }
        private void InitSaveFileDialog(SaveFileDialog saveDialog)
        {
            saveDialog.AddExtension = _addExtension;
            saveDialog.AutoUpgradeEnabled = _autoUpgradeEnabled;
            saveDialog.CheckFileExists = _checkFileExists;
            saveDialog.CheckPathExists = _checkPathExists;
            saveDialog.DefaultExt = _defaultExt;
            saveDialog.DereferenceLinks = _dereferenceLinks;
            saveDialog.FileName = _fileName;
            saveDialog.Filter = _filter;
            saveDialog.FilterIndex = _filterIndex;
            saveDialog.InitialDirectory = _initialDirectory;           
            saveDialog.RestoreDirectory = _restoreDirectory;
            saveDialog.ShowHelp = _showHelp;
            saveDialog.Title = _title;           
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            Browse();
            if (ButtonClicked != null)
            {
                ButtonClicked(sender, e);
            }
        }

        private void textBoxPath_DoubleClick(object sender, EventArgs e)
        {
            Browse();
        }

        private void textBoxPath_TextChanged(object sender, EventArgs e)
        {
            if (TextBoxTextChanged != null)
                TextBoxTextChanged(sender, e);
        }         
    }    
}
