using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Contal.IwQuick.UI
{  
	
	public class UIListboxEditor : UITypeEditor
	{				
		private bool _isDropDownResizable = false;
		private ListBox _list = new ListBox();
		private object _selectedValue = null;
		private IWindowsFormsEditorService _editorService;
		
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			if (context != null&& context.Instance != null)
			{
				UIListboxIsDropDownResizable attribute =  (UIListboxIsDropDownResizable) context.PropertyDescriptor.Attributes[typeof(UIListboxIsDropDownResizable)];
				if (attribute != null)
				{
					_isDropDownResizable = true;
				}
				return UITypeEditorEditStyle.DropDown;
			}
			return UITypeEditorEditStyle.None;
		}
		
		public override bool IsDropDownResizable
		{
			get
			{
				return _isDropDownResizable;
			}
		}
		
		[RefreshProperties(RefreshProperties.All)]public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
		{
			if (context == null || provider == null || context.Instance == null)
			{
				return base.EditValue(provider, value);
			}
			
			_editorService =  (System.Windows.Forms.Design.IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
			if (_editorService != null)
			{
				
				// Get the Back reference to the Custom Property
				CustomProperty.CustomPropertyDescriptor oDescriptor =  (CustomProperty.CustomPropertyDescriptor) context.PropertyDescriptor;
				CustomProperty cp =  (CustomProperty) oDescriptor.CustomProperty;
				
				// Declare attributes
				UIListboxDatasource datasource;
				UIListboxValueMember valuemember;
				UIListboxDisplayMember displaymember;
				
				// Get attributes
				datasource =  (UIListboxDatasource) context.PropertyDescriptor.Attributes[typeof(UIListboxDatasource)];
				valuemember =  (UIListboxValueMember) context.PropertyDescriptor.Attributes[typeof(UIListboxValueMember)];
				displaymember =  (UIListboxDisplayMember) context.PropertyDescriptor.Attributes[typeof(UIListboxDisplayMember)];
				
				_list.BorderStyle = BorderStyle.None;
				_list.IntegralHeight = true;
				
				if (datasource != null)
				{
					_list.DataSource = datasource.Value;
				}
				
				if (displaymember != null)
				{
					_list.DisplayMember = displaymember.Value;
				}
				
				if (valuemember != null)
				{
					_list.ValueMember = valuemember.Value;
				}
				
				if (value != null)
				{
					if (value.GetType().Name == "String")
					{
						_list.Text =  (string) value;
					}
					else
					{
						_list.SelectedItem = value;
					}
				}
				
				
				_list.SelectedIndexChanged += new System.EventHandler(this.SelectedItem);
				
				_editorService.DropDownControl(_list);
				if (_list.SelectedIndices.Count == 1)
				{
					cp.SelectedItem = _list.SelectedItem;
					cp.SelectedValue = _selectedValue;
					value = _list.Text;
				}
				_editorService.CloseDropDown();
			}
			else
			{
				return base.EditValue(provider, value);
			}
			
			return value;
			
		}
		
		private void SelectedItem(object sender, EventArgs e)
		{
			if (_editorService != null)
			{
				if (_list.SelectedValue != null)
				{
					_selectedValue = _list.SelectedValue;
				}
				_editorService.CloseDropDown();
			}
		}

        [AttributeUsage(AttributeTargets.Property)]
		public class UIListboxDatasource : Attribute
		{
			
			private object oDataSource;
			public UIListboxDatasource(ref object Datasource)
			{
				oDataSource = Datasource;
			}
			public object Value
			{
				get
				{
					return oDataSource;
				}
			}
		}

        [AttributeUsage(AttributeTargets.Property)]
		public class UIListboxValueMember : Attribute
		{
			
			private string sValueMember;
			public UIListboxValueMember(string ValueMember)
			{
				sValueMember = ValueMember;
			}
			public string Value
			{
				get
				{
					return sValueMember;
				}
				set
				{
					sValueMember = value;
				}
			}
		}

        [AttributeUsage(AttributeTargets.Property)]
		public class UIListboxDisplayMember : Attribute
		{
			
			private string sDisplayMember;
			public UIListboxDisplayMember(string DisplayMember)
			{
				sDisplayMember = DisplayMember;
			}
			public string Value
			{
				get
				{
					return sDisplayMember;
				}
				set
				{
					sDisplayMember = value;
				}
			}
			
		}

        [AttributeUsage(AttributeTargets.Property)]
		public class UIListboxIsDropDownResizable : Attribute
		{
			
		}	
	}	
}
