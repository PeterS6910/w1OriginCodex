using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Drawing.Imaging;


namespace Contal.IwQuick.UI
{
    public partial class ExtendedPropertyGrid : PropertyGrid
    {
        #region "Protected variables and objects"
        // CustomPropertyCollection assigned to MyBase.SelectedObject
        protected CustomPropertyCollection _customPropertyCollection;
        protected bool _showCustomProperties;

        // CustomPropertyCollectionSet assigned to MyBase.SelectedObjects
        protected CustomPropertyCollectionSet _customPropertyCollectionSet;
        protected bool _showCustomPropertiesSet;

        // Internal PropertyGrid Controls
        protected object _propertyGridView;
        protected object _hotCommands;
        protected object _docComment;
        public ToolStrip _toolStrip;

        // Internal PropertyGrid Fields
        protected Label _docCommentTitle;
        protected Label _docCommentDescription;
        protected FieldInfo _propertyGridEntries;

        // Properties variables
        protected bool _autoSizeProperties;
        protected bool _drawFlatToolbar;

        private bool _buttonCategorized = true;
        private bool _buttonAlphabetical = true;
        private bool _buttonsAddRemove = true;
        private bool _propertyPages = true;

        private bool _uniqueProperties = true;

        #endregion

        #region "Public Functions"
        //Events on adding and removing properties
        public delegate void AddPropEvenHandler(object sender, EventArgs e);
        public event AddPropEvenHandler PropertyAdd;
        public event AddPropEvenHandler PropertyRemove;

        public ExtendedPropertyGrid()
        {
            // This call is required by the Windows Form Designer.
             InitializeComponent();
            
            // Add any initialization after the InitializeComponent() call.
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);

            // Initialize collections
            _customPropertyCollection = new CustomPropertyCollection();
            _customPropertyCollectionSet = new CustomPropertyCollectionSet();

            // Attach internal controls
            _propertyGridView = base.GetType().BaseType.InvokeMember("gridView", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
            _hotCommands = base.GetType().BaseType.InvokeMember("hotcommands", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
            _toolStrip = (ToolStrip)base.GetType().BaseType.InvokeMember("toolStrip", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);
            _toolStrip.Name = "ExtendedPropertyToolStrip";
            _docComment = base.GetType().BaseType.InvokeMember("doccomment", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, this, null);

           
            
            //Add buttons Add/Remove property
            _toolStrip.Items.Add("Add property");
            if (_toolStrip.Items[_toolStrip.Items.Count - 1] != null)
            {
                _toolStrip.Items[_toolStrip.Items.Count - 1].Click += new EventHandler(EditablePropertyGrid_AddPropertyClick);
            }
            _toolStrip.Items.Add("Remove property");
            if (_toolStrip.Items[_toolStrip.Items.Count - 1] != null)
            {
                _toolStrip.Items[_toolStrip.Items.Count - 1].Click += new EventHandler(EditablePropertyGrid_RemovePropertyClick);
            }

            foreach (ToolStripItem item in _toolStrip.Items)
            {
                item.Name = item.Text;
            }         
            _toolStrip.Items[3].Name = "Separator";
            _toolStrip.Items[2].Select();
            _toolStrip.Refresh();

            // Attach DocComment internal fields
            if (_docComment != null)
            {
                _docCommentTitle = (Label)_docComment.GetType().InvokeMember("m_labelTitle", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, _docComment, null);
                _docCommentDescription = (Label)_docComment.GetType().InvokeMember("m_labelDesc", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance, null, _docComment, null);
            }

            // Attach PropertyGridView internal fields
            if (_propertyGridView != null)
            {
                _propertyGridEntries = _propertyGridView.GetType().GetField("allGridEntries", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            }

            // Apply Toolstrip style
            if (_toolStrip != null)
            {
                ApplyToolStripRenderMode(_drawFlatToolbar);
            }         
        }    
       

        void EditablePropertyGrid_AddPropertyClick(object sender, EventArgs e)
        {
            if (PropertyAdd != null)
            {
                PropertyAdd(sender, e);
            }
        }

        void EditablePropertyGrid_RemovePropertyClick(object sender, EventArgs e)
        {
            if (PropertyRemove != null)
            {
                PropertyRemove(sender, e);
            }
        }

        /// <summary>
        /// Removes selected property
        /// </summary>
        public void RemoveProperty()
        {
            if (SelectedGridItem != null)
            {
                if (SelectedGridItem.PropertyDescriptor != null)
                {
                    Items.Remove(SelectedGridItem.PropertyDescriptor.DisplayName);
                    Refresh();
                }
            }
        }

        public void MoveSplitterTo(int x)
        {
            _propertyGridView.GetType().InvokeMember("MoveSplitterTo", BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Instance, null, _propertyGridView, new object[] { x });
        }

        public override void Refresh()
        {
            if (_showCustomPropertiesSet)
            {
                base.SelectedObjects = (object[])_customPropertyCollectionSet.ToArray();
            }
            base.Refresh();
            if (_autoSizeProperties)
            {
                AutoSizeSplitter(32);
            }
        }

        public void SelectItemValue(string name)
        {
            foreach (GridItem item in SelectedGridItem.Parent.GridItems)
            {
                if (item.Label.Equals(name))
                {
                    this.SelectedGridItem = item;
                    SendKeys.Send("{tab}");
                }
            }           
        }

        public void SetComment(string title, string description)
        {
            MethodInfo method = _docComment.GetType().GetMethod("SetComment");
            method.Invoke(_docComment, new object[] { title, description });         
            //oDocComment.SetComment(title, description);
        }


        //single properties
        public CustomProperty AddSingleProperty(string name, PropertyObject value, bool isReadOnly)
        {
            return AddSingleProperty(name, value, isReadOnly, "", "", true, false);
        }

        public CustomProperty AddSingleProperty(string name, PropertyObject value, bool isReadOnly, string category, string description)
        {
            return AddSingleProperty(name, value, isReadOnly, category, description, true, false);
        }
        public CustomProperty AddSingleProperty(string name, PropertyObject value, bool isReadOnly, string category, string description
            , bool visible, bool isPassword)
        {
            value.Name = name;
            CustomProperty customProp = new CustomProperty(value, isReadOnly, category, description, visible);
            _customPropertyCollection.Add(customProp);
            _customPropertyCollection[_customPropertyCollection.Count - 1].IsPassword = isPassword;
            return customProp;
        }

        //File editor properties
        public CustomProperty AddFileEditorProperty(string name, PropertyObject value, bool isReadOnly)
        {
            return AddFileEditorProperty(name, value, isReadOnly, "", "", true, "");
        }
        public CustomProperty AddFileEditorProperty(string name, PropertyObject value, bool isReadOnly, string category, string description)
        {
            return AddFileEditorProperty(name, value, isReadOnly, category, description, true, "");
        }
        public CustomProperty AddFileEditorProperty(string name, PropertyObject value, bool isReadOnly, string category, string description, bool visible, string filter)
        {
            value.Name = name;
            CustomProperty customProp = new CustomProperty(name, value, isReadOnly, category, description, visible);
            _customPropertyCollection.Add(customProp);
            _customPropertyCollection[_customPropertyCollection.Count - 1].UseFileNameEditor = true;
            _customPropertyCollection[_customPropertyCollection.Count - 1].FileNameDialogType = UIFilenameEditor.FileDialogType.LoadFileDialog;
            _customPropertyCollection[_customPropertyCollection.Count - 1].FileNameFilter = filter;
            return customProp;
        }
        //Browsable property
        public CustomProperty AddExpandableProperty(string name, PropertyObject value, bool isReadOnly)
        {
            return AddExpandableProperty(name, value, isReadOnly, "", "", true, 1);
        }
        public CustomProperty AddExpandableProperty(string name, PropertyObject value, bool isReadOnly, string category, string description)
        {
            return AddExpandableProperty(name, value, isReadOnly, category, description, true, 1);
        }
        public CustomProperty AddExpandableProperty(string name, PropertyObject value, bool isReadOnly, string category, string description, bool visible, int visualBrowsableOption)
        {
            value.Name = name;
            CustomProperty customProp = new CustomProperty(name, value, isReadOnly, category, description, visible);
            _customPropertyCollection.Add(customProp);
            _customPropertyCollection[_customPropertyCollection.Count - 1].IsBrowsable = true;
            if (visualBrowsableOption < 2)
            {
                _customPropertyCollection[_customPropertyCollection.Count - 1].BrowsableLabelStyle = BrowsableTypeConverter.LabelStyle.lsEllipsis;
            }
            else if (visualBrowsableOption == 2)
            {
                _customPropertyCollection[_customPropertyCollection.Count - 1].BrowsableLabelStyle = BrowsableTypeConverter.LabelStyle.lsNormal;
            }
            else
            {
                _customPropertyCollection[_customPropertyCollection.Count - 1].BrowsableLabelStyle = BrowsableTypeConverter.LabelStyle.lsTypeName;
            }
            return customProp;
        }

        //Password roperty        
        public CustomProperty AddPasswordProperty(string name, PropertyObject value, bool isReadOnly, string category, string description, bool visible)
        {
            return AddSingleProperty(name, value, isReadOnly, category, description, visible, true);
        }

        //Custom editor property
        public CustomProperty AddCustomEditorProperty(string name, PropertyObject value, bool isReadOnly, string category, string description, bool visible, UITypeEditor customEditor)
        {
            AddSingleProperty(name, value, isReadOnly, category, description, visible, false);
            _customPropertyCollection[_customPropertyCollection.Count - 1].CustomEditor = customEditor;
            return _customPropertyCollection[_customPropertyCollection.Count - 1];
        }

        //Custom event editor property
        //public CustomProperty AddCustomEditorProperty(string name, object value, bool isReadOnly, string category, string description, bool visible, UITypeEditor customEditor)
        //{
        //    AddSingleProperty(name, value, isReadOnly, category, description, visible, false);
        //    oCustomPropertyCollection[oCustomPropertyCollection.Count - 1].CustomEditor = customEditor;
        //    return oCustomPropertyCollection[oCustomPropertyCollection.Count - 1];
        //}


        //Custom type converter property
        public CustomProperty AddCustomEditorProperty(string name, PropertyObject value, bool isReadOnly, string category, string description, bool visible, TypeConverter converter)
        {
            AddSingleProperty(name, value, isReadOnly, category, description, visible, false);
            _customPropertyCollection[_customPropertyCollection.Count - 1].CustomTypeConverter = converter;
            return _customPropertyCollection[_customPropertyCollection.Count - 1];
        }

        //Custom choises property
        public CustomProperty AddCustomEditorProperty(string name, PropertyObject value, bool isReadOnly, string category, string description, bool visible, string[] array, bool isSorted)
        {
            AddSingleProperty(name, value, isReadOnly, category, description, visible, false);
            _customPropertyCollection[_customPropertyCollection.Count - 1].Choices = new CustomChoices(array, isSorted);
            return _customPropertyCollection[_customPropertyCollection.Count - 1];
        }
        public CustomProperty AddCustomEditorProperty(string name, PropertyObject value, bool isReadOnly, string category, string description, bool visible, CustomChoices customChoices)
        {
            AddSingleProperty(name, value, isReadOnly, category, description, visible, false);
            _customPropertyCollection[_customPropertyCollection.Count - 1].Choices = customChoices;
            return _customPropertyCollection[_customPropertyCollection.Count - 1];
        }

        //Add dynamic property
        public CustomProperty AddDynamicProperty(string name, ref object refObject, string propertyName, bool isReadOnly, string category, string description, bool visible)
        {
            CustomProperty customProp = new CustomProperty(name, ref refObject, propertyName, isReadOnly, category, description, visible);
            _customPropertyCollection.Add(customProp);
            return customProp;
        }

        //Add databininding property        
        public CustomProperty AddDataBindingProperty(string name, PropertyObject value, bool isReadOnly, string category, string description, bool visible,
            string valueMember, string displayMember, object dataSource)
        {
            AddSingleProperty(name, value, isReadOnly, category, description, visible, false);
            _customPropertyCollection[_customPropertyCollection.Count - 1].ValueMember = valueMember;
            _customPropertyCollection[_customPropertyCollection.Count - 1].DisplayMember = displayMember;
            _customPropertyCollection[_customPropertyCollection.Count - 1].Datasource = dataSource;
            return _customPropertyCollection[_customPropertyCollection.Count - 1];
        }
        #endregion

        #region "Protected Functions"
        protected override void OnResize(System.EventArgs e)
        {
            base.OnResize(e);
            if (_autoSizeProperties)
            {
                AutoSizeSplitter(32);
            }
        }       

        //protected override void OnPropertyValueChanged(PropertyValueChangedEventArgs e)
        //{
        //    if (e.ChangedItem.PropertyDescriptor.Name == "Name")
        //    {
        //        GridItem item = e.ChangedItem;
        //        //Get CustomProperty that represents property
        //        while (e.ChangedItem.Parent != null && e.ChangedItem.Parent != item)
        //        {
        //            item = e.ChangedItem.Parent;
        //        }
        //        //Change name according to changed name
        //        if ((this.Items[e.ChangedItem.Value.ToString()] != null) && (!_uniqueProperties))
        //        {
        //            Dialog.Error("Property value is not valid");
        //        }
        //        else
        //        {
        //            this.Items[item.Label].Name = e.ChangedItem.Value.ToString();
        //        }
        //    }
        //    else
        //    {
        //        base.OnPropertyValueChanged(e);
        //    }
        //}

        protected void AutoSizeSplitter(int RightMargin)
        {

            GridItemCollection oItemCollection = (System.Windows.Forms.GridItemCollection)_propertyGridEntries.GetValue(_propertyGridView);
            if (oItemCollection == null)
            {
                return;
            }
            System.Drawing.Graphics oGraphics = System.Drawing.Graphics.FromHwnd(this.Handle);
            int CurWidth = 0;
            int MaxWidth = 0;

            foreach (GridItem oItem in oItemCollection)
            {
                if (oItem.GridItemType == GridItemType.Property)
                {
                    CurWidth = (int)oGraphics.MeasureString(oItem.Label, this.Font).Width + RightMargin;
                    if (CurWidth > MaxWidth)
                    {
                        MaxWidth = CurWidth;
                    }
                }
            }

            MoveSplitterTo(MaxWidth);
        }
        protected void ApplyToolStripRenderMode(bool value)
        {
            if (value)
            {
                _toolStrip.Renderer = new ToolStripSystemRenderer();
            }
            else
            {
                ToolStripProfessionalRenderer renderer = new ToolStripProfessionalRenderer(new CustomColorScheme());
                renderer.RoundedEdges = false;
                _toolStrip.Renderer = renderer;
            }
        }
        #endregion

        #region "Properties"

        [Category("Appearance"), DefaultValue(false),
        DescriptionAttribute("Add property button text")]
        public string ButtonTextAddProperty
        {
            get
            {
                if (_toolStrip.Items["Add property"] != null)
                {
                    return _toolStrip.Items["Add property"].Text;
                }
                return String.Empty;
            }
            set
            {
                if (_toolStrip.Items["Add property"] != null)
                {
                    _toolStrip.Items["Add property"].Text = value;
                }
            }
        }
        [Category("Appearance"), DefaultValue(false),
        DescriptionAttribute("Remove property button text")]
        public string ButtonTextRemoveProperty
        {
            get
            {
                if (_toolStrip.Items["Remove property"] != null)
                {
                    return _toolStrip.Items["Remove property"].Text;
                }
                return String.Empty;
            }
            set
            {
                if (_toolStrip.Items["Remove property"] != null)
                {
                    _toolStrip.Items["Remove property"].Text = value;
                }
            }
        }

        [Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        DescriptionAttribute("Set the collection of the CustomProperty. Set ShowCustomProperties to True to enable it."),
        RefreshProperties(RefreshProperties.Repaint)]
        public CustomPropertyCollection Items
        {
            get
            {
                return _customPropertyCollection;
            }
        }

        [Category("Behavior"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        DescriptionAttribute("Set the CustomPropertyCollectionSet. Set ShowCustomPropertiesSet to True to enable it."),
        RefreshProperties(RefreshProperties.Repaint)]
        public CustomPropertyCollectionSet ItemSet
        {
            get
            {
                return _customPropertyCollectionSet;
            }
        }

        [Category("Behavior"), DefaultValue(false),
        DescriptionAttribute("Move automatically the splitter to better fit all the properties shown.")]
        public bool AutoSizeProperties
        {
            get
            {
                return _autoSizeProperties;
            }
            set
            {
                _autoSizeProperties = value;
                if (value)
                {
                    AutoSizeSplitter(32);
                }
            }
        }

        [Category("Behavior"), DefaultValue(false),
        DescriptionAttribute("Use the custom properties collection as SelectedObject."),
        RefreshProperties(RefreshProperties.All)]
        public bool ShowCustomProperties
        {
            get
            {
                return _showCustomProperties;
            }
            set
            {
                if (value == true)
                {
                    _showCustomPropertiesSet = false;
                    base.SelectedObject = _customPropertyCollection;
                }
                _showCustomProperties = value;
            }
        }

        [Category("Behavior"), DefaultValue(false),
        DescriptionAttribute("Use the custom properties collections as SelectedObjects."),
        RefreshProperties(RefreshProperties.All)]
        public bool ShowCustomPropertiesSet
        {
            get
            {
                return _showCustomPropertiesSet;
            }
            set
            {
                if (value == true)
                {
                    _showCustomProperties = false;
                    base.SelectedObjects = (object[])_customPropertyCollectionSet.ToArray();
                }
                _showCustomPropertiesSet = value;
            }
        }

        [Category("Appearance"), DefaultValue(false),
        DescriptionAttribute("Draw a flat toolbar")]
        public new bool DrawFlatToolbar
        {
            get
            {
                return _drawFlatToolbar;
            }
            set
            {
                _drawFlatToolbar = value;
                ApplyToolStripRenderMode(_drawFlatToolbar);
            }
        }

        [Category("Appearance"), DisplayName("Toolstrip"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        DescriptionAttribute("Toolbar object"), Browsable(true)]
        public ToolStrip ToolStrip
        {
            get
            {
                return _toolStrip;
            }
        }

        [Category("Appearance"), DisplayName("Help"),
        DescriptionAttribute("DocComment object. Represent the comments area of the PropertyGrid."),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)]
        public Control DocComment
        {
            get
            {
                return (System.Windows.Forms.Control)_docComment;
            }
        }

        [Category("Appearance"), DisplayName("HelpTitle"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        DescriptionAttribute("Help Title Label."), Browsable(true)]
        public Label DocCommentTitle
        {
            get
            {
                return _docCommentTitle;
            }
        }

        [Category("Appearance"), DisplayName("HelpDescription"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        DescriptionAttribute("Help Description Label."), Browsable(true)]
        public Label DocCommentDescription
        {
            get
            {
                return _docCommentDescription;
            }
        }

        [Category("Appearance"), DisplayName("HelpImageBackground"), DescriptionAttribute("Help Image Background.")]
        public Image DocCommentImage
        {
            get
            {
                return ((Control)_docComment).BackgroundImage;
            }
            set
            {
                ((Control)_docComment).BackgroundImage = value;
            }
        }

        [Category("Appearance"), DefaultValue(true),
        DescriptionAttribute("Button Categorized visible")]
        public bool ButtonCategorized
        {
            get
            {
                return _buttonCategorized;
            }
            set
            {
                _buttonCategorized = value;
                if (!_buttonCategorized)
                {
                    _toolStrip.Items["Categorized"].Visible = false;
                }
                else
                {
                    _toolStrip.Items["Categorized"].Visible = true;
                }
            }
        }

        [Category("Appearance"), DefaultValue(true),
        DescriptionAttribute("Button Alphabetical visible")]
        public bool ButtonAlphabetical
        {
            get
            {
                return _buttonAlphabetical;
            }
            set
            {
                _buttonAlphabetical = value;
                if (!_buttonAlphabetical)
                {
                    _toolStrip.Items["Alphabetical"].Visible = false;
                }
                else
                {
                    _toolStrip.Items["Alphabetical"].Visible = true;                 
                }
            }
        }

        [Category("Appearance"), DefaultValue(true),
        DescriptionAttribute("Button Events and separator visible")]
        public bool ButtonPropertyPages
        {
            get
            {
                return _propertyPages;
            }
            set
            {
                _propertyPages = value;
                if (!_propertyPages)
                {
                    _toolStrip.Items["Separator"].Visible = false;
                    _toolStrip.Items["Property pages"].Visible = false;
                }
                else
                {
                    _toolStrip.Items["Separator"].Visible = true;
                    _toolStrip.Items["Property pages"].Visible = true;
                }
            }
        }

        [Category("Appearance"), DefaultValue(true),
        DescriptionAttribute("Button Add property and Remove property visible")]
        public bool ButtonsAddRemove
        {
            get
            {
                return _buttonsAddRemove;
            }
            set
            {
                _buttonsAddRemove = value;
                if (!_buttonsAddRemove)
                {
                    _toolStrip.Items["Add property"].Visible = false;
                    _toolStrip.Items["Remove property"].Visible = false;
                }
                else
                {
                    _toolStrip.Items["Add property"].Visible = true;
                    _toolStrip.Items["Remove property"].Visible = true;
                }
            }
        }
        [Category("Behavior"), DefaultValue(true),
        DescriptionAttribute("Define if properties can have same names")]
        public bool UniqueProperties
        {
            get
            {
                return _uniqueProperties;
            }
            set
            {
                _uniqueProperties = value;                
            }
        }

        #endregion
    }
    public partial class PropertyObject
    {
        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public PropertyObject()
        {
        
        }
    }
}
