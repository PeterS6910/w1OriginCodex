using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Data;
using System.Xml.Serialization;

namespace Contal.IwQuick.UI
{
    [Serializable(), XmlRootAttribute("CustomProperty")]
    public class CustomProperty
    {				
		#region "Private variables"
		
		// Common properties
        protected string _name = String.Empty;
        

		protected object _value = null;
		protected bool _isReadOnly = false;
		protected bool _visible = true;
		protected string _description = String.Empty;
		protected string _category = String.Empty;
		protected bool _isPassword = false;
		protected bool _isPercentage = false;
		protected bool _isParenthesize = false;
		
		// Filename editor properties
		protected string _filter = null;
		protected UIFilenameEditor.FileDialogType _dialogType = UIFilenameEditor.FileDialogType.LoadFileDialog;
		protected bool _useFileNameEditor = false;
		
		// Custom choices properties
		protected CustomChoices _choices = null;
		
		// Browsable properties
		protected bool _isBrowsable = false;
		protected BrowsableTypeConverter.LabelStyle _browsablePropertyLabel = BrowsableTypeConverter.LabelStyle.lsEllipsis;
		
		// Dynamic properties
		protected bool _isRef = false;
		protected object _ref = null;
		protected string _prop = String.Empty;
		
		// Databinding properties
		protected object _datasource = null;
		protected string _displayMember = null;
		protected string _valueMember = null;
		protected object _selectedValue = null;
		protected object _selectedItem = null;
		protected bool _isDropdownResizable = false;
		
		// 3-dots button event handler
		protected UICustomEventEditor.OnClick MethodDelegate;
		
		// Extended Attributes
		[NonSerialized()]protected AttributeCollection _customAttributes = null;
		protected object _tag = null;
		protected object _defaultValue = null;
		protected Type _defaultType = null;
		
		// Custom Editor and Custom Type Converter
		[NonSerialized()]protected UITypeEditor oCustomEditor = null;
		[NonSerialized()]protected TypeConverter oCustomTypeConverter = null;
		
		#endregion
		
		#region "Public methods"
		
		public CustomProperty()
		{
			_name = "New Property";
			_value = new string(' ',0);
		}

        public CustomProperty(string strName, object objValue, bool boolIsReadOnly, string strCategory, string strDescription, bool boolVisible)
        {
            _name = strName;
            _value = objValue;
            _isReadOnly = boolIsReadOnly;
            _description = strDescription;
            _category = strCategory;
            _visible = boolVisible;            
            if (_value != null)
            {
                _defaultValue = _value;
            }
        }
		
		public CustomProperty(string strName, object objValue, bool boolIsReadOnly, string strCategory, string strDescription, bool boolVisible, object tag)
		{
			_name = strName;
			_value = objValue;
			_isReadOnly = boolIsReadOnly;
			_description = strDescription;
			_category = strCategory;
			_visible = boolVisible;
            _tag = tag;
			if (_value != null)
			{
				_defaultValue = _value;
			}
		}
        public CustomProperty(PropertyObject objValue, bool boolIsReadOnly, string strCategory, string strDescription, bool boolVisible)
        {
            _name = objValue.Name;            
            _value = objValue;
            _isReadOnly = boolIsReadOnly;
            _description = strDescription;
            _category = strCategory;
            _visible = boolVisible;
            if (_value != null)
            {
                _defaultValue = _value;
            }
        }
		
		public CustomProperty(string strName, ref object objRef, string strProp, bool boolIsReadOnly, string strCategory, string strDescription, bool boolVisible)
		{
			_name = strName;
			_isReadOnly = boolIsReadOnly;
			_description = strDescription;
			_category = strCategory;
			_visible = boolVisible;
			_isRef = true;
			_ref = objRef;
			_prop = strProp;
			if (Value != null)
			{
				_defaultValue = Value;
			}
		}

        public void RebuildAttributes()
        {
            if (_useFileNameEditor)
            {
                BuildAttributes_FilenameEditor();
            }
            else if (_choices != null)
            {
                BuildAttributes_CustomChoices();
            }
            else if (_datasource != null)
            {
                BuildAttributes_ListboxEditor();
            }
            else if (_isBrowsable)
            {
                BuildAttributes_BrowsableProperty();
            }
        }
		
		#endregion
		
		#region "Private methods"
		
		private void BuildAttributes_FilenameEditor()
		{
			ArrayList attrs = new ArrayList();
			UIFilenameEditor.FileDialogFilterAttribute FilterAttribute = new UIFilenameEditor.FileDialogFilterAttribute(_filter);
			UIFilenameEditor.SaveFileAttribute SaveDialogAttribute = new UIFilenameEditor.SaveFileAttribute();
			Attribute[] attrArray;
			attrs.Add(FilterAttribute);
			if (_dialogType == UIFilenameEditor.FileDialogType.SaveFileDialog)
			{
				attrs.Add(SaveDialogAttribute);
			}
			attrArray =  (System.Attribute[]) attrs.ToArray(typeof(Attribute));
			_customAttributes = new AttributeCollection(attrArray);
		}
		
		private void BuildAttributes_CustomChoices()
		{
			if (_choices != null)
			{
				CustomChoices.CustomChoicesAttributeList list = 
                    new CustomChoices.CustomChoicesAttributeList(_choices.Items);
				ArrayList attrs = new ArrayList();
				Attribute[] attrArray;
				attrs.Add(list);
				attrArray =  (System.Attribute[]) attrs.ToArray(typeof(Attribute));
				_customAttributes = new AttributeCollection(attrArray);
			}
		}
		
		private void BuildAttributes_ListboxEditor()
		{
			if (_datasource != null)
			{
				UIListboxEditor.UIListboxDatasource ds = new UIListboxEditor.UIListboxDatasource(ref _datasource);
				UIListboxEditor.UIListboxValueMember vm = new UIListboxEditor.UIListboxValueMember(_valueMember);
				UIListboxEditor.UIListboxDisplayMember dm = new UIListboxEditor.UIListboxDisplayMember(_displayMember);
				UIListboxEditor.UIListboxIsDropDownResizable ddr = null;
				ArrayList attrs = new ArrayList();
				attrs.Add(ds);
				attrs.Add(vm);
				attrs.Add(dm);
				if (_isDropdownResizable)
				{
					ddr = new UIListboxEditor.UIListboxIsDropDownResizable();
					attrs.Add(ddr);
				}
				Attribute[] attrArray;
				attrArray =  (System.Attribute[]) attrs.ToArray(typeof(Attribute));
				_customAttributes = new AttributeCollection(attrArray);
			}
		}

        private void BuildAttributes_BrowsableProperty()
        {
            BrowsableTypeConverter.BrowsableLabelStyleAttribute style = 
                new BrowsableTypeConverter.BrowsableLabelStyleAttribute(_browsablePropertyLabel);
            _customAttributes = new AttributeCollection(new Attribute[] { style });
        }
		
		private void BuildAttributes_CustomEventProperty()
		{
			UICustomEventEditor.DelegateAttribute attr = new UICustomEventEditor.DelegateAttribute(MethodDelegate);
			_customAttributes = new AttributeCollection(new Attribute[] {attr});
		}

        private object DataColumn
        {
            get
            {
                DataRow oRow = (System.Data.DataRow)_ref;
                if (oRow.RowState != DataRowState.Deleted)
                {
                    if (_datasource == null)
                    {
                        return oRow[_prop];
                    }
                    else
                    {
                        DataTable oLookupTable = _datasource as DataTable;
                        if (oLookupTable != null)
                        {
                            return oLookupTable.Select(_valueMember + "=" + oRow[_prop])[0][_displayMember];
                        }
                        else
                        {
                            Information.Err().Raise(Constants.vbObjectError + 513, null, 
                                "Bind of DataRow with a DataSource that is not a DataTable is not possible", null, null);
                            return null;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            set
            {
                DataRow oRow = (System.Data.DataRow)_ref;
                if (oRow.RowState != DataRowState.Deleted)
                {
                    if (_datasource == null)
                    {
                        oRow[_prop] = value;
                    }
                    else
                    {
                        DataTable oLookupTable = _datasource as DataTable;
                        if (oLookupTable != null)
                        {
                            if (oLookupTable.Columns[_displayMember].DataType.Equals(System.Type.GetType("System.String")))
                            {

                                oRow[_prop] = oLookupTable.Select(oLookupTable.Columns[_displayMember].ColumnName + 
                                    " = \'" + value + "\'")[0][_valueMember];
                            }
                            else
                            {
                                oRow[_prop] = oLookupTable.Select(oLookupTable.Columns[_displayMember].ColumnName + 
                                    " = " + value)[0][_valueMember];
                            }
                        }
                        else
                        {
                            Information.Err().Raise(Constants.vbObjectError + 514, null,
                                "Bind of DataRow with a DataSource that is not a DataTable is impossible", null, null);
                        }
                    }
                }
            }
        }
		
		#endregion
		
		#region "Public properties"

        [Category("Appearance"), DisplayName("Name"),
        DescriptionAttribute("Display Name of the CustomProperty."),
        ParenthesizePropertyName(true), XmlElementAttribute("Name")]
        public string Name
        {
            get { return _name; }
            set
            {
                Validator.CheckNullString(value);
                _name = value;
            }
        }    

        [Category("Appearance"), DisplayName("ReadOnly"), 
        DescriptionAttribute("Set read only attribute of the CustomProperty."),   XmlElementAttribute("ReadOnly")]
        public bool IsReadOnly
        {
			get
			{
				return _isReadOnly;
			}
			set
			{
				_isReadOnly = value;
			}
		}

        [Category("Appearance"), 
        DescriptionAttribute("Set visibility attribute of the CustomProperty.")]
        public bool Visible
        {
			get
			{
				return _visible;
			}
			set
			{
				_visible = value;
			}
		}

        [Category("Appearance"), DescriptionAttribute("Represent the Value of the CustomProperty.")]
        public object Value
        {
            get
            {
                if (_isRef)
                {
                    if (_ref.GetType() == typeof(DataRow) || _ref.GetType().IsSubclassOf(typeof(DataRow)))
                        return this.DataColumn;
                    else
                        return Interaction.CallByName(_ref, _prop, CallType.Get, null);
                }
                else
                {
                    return _value;
                }
            }
            set
            {
                if (_isRef)
                {
                    if (_ref.GetType() == typeof(DataRow) || _ref.GetType().IsSubclassOf(typeof(DataRow)))
                        this.DataColumn = value;
                    else
                        Interaction.CallByName(_ref, _prop, CallType.Set, value);
                }
                else
                {
                    _value = value;
                }
            }
        }

        [Category("Appearance"), DescriptionAttribute("Set description associated with the CustomProperty.")]
        public string Description
        {
			get
			{
				return _description;
			}
			set
			{
				_description = value;
			}
		}

        [Category("Appearance"), DescriptionAttribute("Set category associated with the CustomProperty.")]
        public string Category
        {
			get
			{
				return _category;
			}
			set
			{
				_category = value;
			}
		}

        [XmlIgnore()]public System.Type Type
		{
			get
			{
				if (Value != null)
				{
					return Value.GetType();
				}
				else
				{
					if (_defaultValue != null)
					{
						return _defaultValue.GetType();
					}
					else
					{
						return _defaultType;
					}
				}
            }
		}

        [XmlIgnore()]public AttributeCollection Attributes
		{
			get
			{
				return _customAttributes;
			}
			set
			{
				_customAttributes = value;
			}
		}

        [Category("Behavior"), DescriptionAttribute("Indicates if the property is browsable or not."), 
        XmlElementAttribute(IsNullable = false)]
        public bool IsBrowsable
		{
			get
			{
				return _isBrowsable;
			}
			set
			{
				_isBrowsable = value;
				if (value == true)
				{
                    BuildAttributes_BrowsableProperty();
                }
			}
		}

		[Category("Appearance"), DisplayName("Parenthesize"), 
        DescriptionAttribute("Indicates whether the name of the associated property is displayed with parentheses in the Properties window."),
        DefaultValue(false), XmlElementAttribute("Parenthesize")]
        public bool Parenthesize
		{
			get
			{
				return _isParenthesize;
			}
			set
			{
				_isParenthesize = value;
			}
		}

        [Category("Behavior"), DescriptionAttribute("Indicates the style of the label when a property is browsable."),
        XmlElementAttribute(IsNullable = false)]
        public BrowsableTypeConverter.LabelStyle BrowsableLabelStyle
        {
			get
			{
				return _browsablePropertyLabel;
			}
			set
			{
				bool Update = false;
				if (value != _browsablePropertyLabel)
				{
					Update = true;
				}
				_browsablePropertyLabel = value;
				if (Update)
				{
					BrowsableTypeConverter.BrowsableLabelStyleAttribute style = new BrowsableTypeConverter.BrowsableLabelStyleAttribute(value);
					_customAttributes = new AttributeCollection(new Attribute[] {style});
				}
			}
		}

        [Category("Behavior"), DescriptionAttribute("Indicates if the property is masked or not."), XmlElementAttribute(IsNullable = false)]public bool IsPassword
        {
			get
			{
				return _isPassword;
			}
			set
			{
				_isPassword = value;
			}
		}

        [Category("Behavior"), DescriptionAttribute("Indicates if the property represents a value in percentage."),
        XmlElementAttribute(IsNullable = false)]
        public bool IsPercentage
        {
			get
			{
				return _isPercentage;
			}
			set
			{
				_isPercentage = value;
			}
		}

        [Category("Behavior"), DescriptionAttribute("Indicates if the property uses a FileNameEditor converter."), 
        XmlElementAttribute(IsNullable = false)]
        public bool UseFileNameEditor
        {
			get
			{
				return _useFileNameEditor;
			}
			set
			{
				_useFileNameEditor = value;
			}
		}

        [Category("Behavior"), DescriptionAttribute("Apply a filter to FileNameEditor converter."),
        XmlElementAttribute(IsNullable = false)]
        public string FileNameFilter
        {
			get
			{
				return _filter;
			}
			set
			{
				bool UpdateAttributes = false;
				if (value != _filter)
				{
					UpdateAttributes = true;
				}
				_filter = value;
				if (UpdateAttributes)
				{
					BuildAttributes_FilenameEditor();
				}
			}
		}

        [Category("Behavior"), DescriptionAttribute("DialogType of the FileNameEditor."), 
        XmlElementAttribute(IsNullable = false)]
        public UIFilenameEditor.FileDialogType FileNameDialogType
        {
			get
			{
				return _dialogType;
			}
			set
			{
				bool UpdateAttributes = false;
				if (value != _dialogType)
				{
					UpdateAttributes = true;
				}
				_dialogType = value;
				if (UpdateAttributes)
				{
					BuildAttributes_FilenameEditor();
				}
			}
		}

        [Category("Behavior"), DescriptionAttribute("Custom Choices list."), XmlIgnore()]
        public CustomChoices Choices
        {
			get
			{
				return _choices;
			}
			set
			{
				_choices = value;
				BuildAttributes_CustomChoices();
			}
		}

        [Category("Databinding"), XmlIgnore()]public object Datasource
        {
			get
			{
				return _datasource;
			}
			set
			{
				_datasource = value;
				BuildAttributes_ListboxEditor();
			}
		}

        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public string ValueMember
		{
			get
			{
				return _valueMember;
			}
			set
			{
				_valueMember = value;
				BuildAttributes_ListboxEditor();
			}
		}

        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public string DisplayMember
		{
			get
			{
				return _displayMember;
			}
			set
			{
				_displayMember = value;
				BuildAttributes_ListboxEditor();
			}
		}

        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public object SelectedValue
		{
			get
			{
				return _selectedValue;
			}
			set
			{
				_selectedValue = value;
			}
		}

        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public object SelectedItem
		{
			get
			{
				return _selectedItem;
			}
			set
			{
				_selectedItem = value;
			}
		}

        [Category("Databinding"), XmlElementAttribute(IsNullable = false)]
        public bool IsDropdownResizable
		{
			get
			{
				return _isDropdownResizable;
			}
			set
			{
				_isDropdownResizable = value;
				BuildAttributes_ListboxEditor();
			}
		}

        [XmlIgnore()]public UITypeEditor CustomEditor
		{
			get
			{
				return oCustomEditor;
			}
			set
			{
				oCustomEditor = value;
			}
		}

        [XmlIgnore()]public TypeConverter CustomTypeConverter
		{
			get
			{
				return oCustomTypeConverter;
			}
			set
			{
				oCustomTypeConverter = value;
			}
		}
				
		[XmlIgnore()]public object Tag
		{
			get
			{
				return _tag;
			}
			set
			{
				_tag = value;
			}
		}
		
		[XmlIgnore()]public object DefaultValue
		{
			get
			{
				return _defaultValue;
			}
			set
			{
				_defaultValue = value;
			}
		}
		
		[XmlIgnore()]public Type DefaultType
		{
			get
			{
				return _defaultType;
			}
			set
			{
				_defaultType = value;
			}
		}
		
		[XmlIgnore()]public UICustomEventEditor.OnClick OnClick
		{
			get
			{
				return MethodDelegate;
			}
			set
			{
				MethodDelegate = value;
				BuildAttributes_CustomEventProperty();
			}
		}
		
		#endregion
		
		#region "CustomPropertyDescriptor"
		public class CustomPropertyDescriptor : PropertyDescriptor
		{
			
			protected CustomProperty oCustomProperty;
			
			public CustomPropertyDescriptor(CustomProperty myProperty, Attribute[] attrs) 
                : base(myProperty.Name, attrs)
			{
				if (myProperty == null)
				{
					oCustomProperty = null;
				}
				else
				{
					
					oCustomProperty = myProperty;
				}
			}
			
			public override bool CanResetValue(object component)
			{
				if ((oCustomProperty.DefaultValue != null)|| (oCustomProperty.DefaultType != null))
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			
			public override System.Type ComponentType
			{
				get
				{
					return this.GetType();
				}
			}
			
			public override object GetValue(object component)
			{
				return oCustomProperty.Value;
			}
			
			public override bool IsReadOnly
			{
				get
				{
					return oCustomProperty.IsReadOnly;
				}
			}
			
			public override System.Type PropertyType
			{
				get
				{
					return oCustomProperty.Type;
				}
			}
			
			public override void ResetValue(object component)
			{
				oCustomProperty.Value = oCustomProperty.DefaultValue;
				this.OnValueChanged(component, EventArgs.Empty);
			}
			
			public override void SetValue(object component, object value)
			{
				oCustomProperty.Value = value;
				this.OnValueChanged(component, EventArgs.Empty);
			}
			
			public override bool ShouldSerializeValue(object component)
			{
				object oValue = oCustomProperty.Value;
				if ((oCustomProperty.DefaultValue != null)&& (oValue != null))
				{
					return ! oValue.Equals(oCustomProperty.DefaultValue);
				}
				else
				{
					return false;
				}
			}
			
			public override string Description
			{
				get
				{
					return oCustomProperty.Description;
				}
			}
			
			public override string Category
			{
				get
				{
					return oCustomProperty.Category;
				}
			}
			
			public override string DisplayName
			{
				get
				{
					return oCustomProperty.Name;
				}
			}
			
			public override bool IsBrowsable
			{
				get
				{
					return oCustomProperty.IsBrowsable;
				}
			}
			
			public object CustomProperty
			{
				get
				{
					return oCustomProperty;
				}
			}
		}
		#endregion		
	}	
}