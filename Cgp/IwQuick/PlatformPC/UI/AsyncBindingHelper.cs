using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Reflection;

namespace Contal.IwQuick.UI
{
    /// <summary>
    /// A helper class for creating a binding on an object that may be changed
    /// asynchronously from the bound UI thread.
    /// </summary>    
    public class AsyncBindingHelper : INotifyPropertyChanged
    {
        /// <summary>
        /// Get a binding instance that can invoke a property change
        /// on the UI thread, regardless of the originating thread
        /// </summary>
        /// <param name="bindingControl">The UI control this binding is added to</param>
        /// <param name="propertyName">The property on the UI control to bind to</param>
        /// <param name="bindingSource">The source INotifyPropertyChanged to be
        /// observed for changes</param>
        /// <param name="dataMember">The property on the source to watch</param>
        /// <returns></returns>
        public static Binding GetBinding(Control bindingControl,
                                          string propertyName,
                                          INotifyPropertyChanged bindingSource,
                                          string dataMember)
        {
            AsyncBindingHelper helper
              = new AsyncBindingHelper(bindingControl, bindingSource, dataMember);
            return new Binding(propertyName, helper, "Value");
        }

        /// <summary>
        /// Get a binding instance that can invoke a property change
        /// on the UI thread, regardless of the originating thread
        /// </summary>
        /// <param name="bindingControl">The UI control this binding is added to</param>
        /// <param name="propertyName">The property on the UI control to bind to</param>
        /// <param name="bindingSource">The source INotifyPropertyChanged to be
        /// observed for changes</param>
        /// <param name="dataMember">The property on the source to watch</param>
        /// <param name="dataSourceUpdateMode">Specifies when will the UI control raise update event (OnValidation by default)</param>
        /// <returns></returns>
        public static Binding GetBinding(Control bindingControl,
                                          string propertyName,
                                          INotifyPropertyChanged bindingSource,
                                          string dataMember,
                                          DataSourceUpdateMode dataSourceUpdateMode)
        {
            AsyncBindingHelper helper
              = new AsyncBindingHelper(bindingControl, bindingSource, dataMember);
            return new Binding(propertyName, helper, "Value", false, dataSourceUpdateMode);
        }

        private readonly PropertyInfo _propertyInfo;
        private readonly Control _bindingControl;
        private readonly INotifyPropertyChanged _bindingSource;
        private readonly string _dataMember;

        private AsyncBindingHelper(Control bindingControl,
                                    INotifyPropertyChanged bindingSource,
                                    string dataMember)
        {
            _propertyInfo = bindingSource.GetType().GetProperty(dataMember);
            _bindingControl = bindingControl;
            _bindingSource = bindingSource;
            _dataMember = dataMember;
            bindingSource.PropertyChanged
              += OnBindingSourcePropertyChanged;
        }

        private void OnBindingSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null && e.PropertyName == _dataMember)
            {
                if (_bindingControl.InvokeRequired)
                {
                    _bindingControl.BeginInvoke(
                      new PropertyChangedEventHandler(OnBindingSourcePropertyChanged),
                      sender,
                      e);
                    return;
                }
                PropertyChanged(this, new PropertyChangedEventArgs("Value"));
            }
        }

        /// <summary>
        /// The current value of the data sources' datamember
        /// </summary>
        public object Value
        {
            get
            {
                return _propertyInfo.GetValue(_bindingSource, null);
            }
            set
            {
                try
                {
                    //it will first try to set property directly
                    try
                    {
                        _propertyInfo.SetValue(_bindingSource, value, null);
                    }
                    catch (Exception)
                    {
                        //if direct set failed, try to use conversion
                        if (_propertyInfo.PropertyType != value.GetType())
                        {
                            if (_propertyInfo.PropertyType == typeof(byte))
                            {
                                //passing int where byte is required
                                if (value is int)
                                    _propertyInfo.SetValue(_bindingSource, (byte)((int)value), null);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //let the reflection to apply default value
                    _propertyInfo.SetValue(_bindingSource, null, null);
                }
            }
        }
        #region INotifyPropertyChanged Members
        /// <summary>
        /// Event fired when the dataMember property on the data source is changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
