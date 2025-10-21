using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.Server.Beans;

namespace Cgp.NCAS.WpfGraphicsControl
{
    /// <summary>
    /// Interaction logic for SelectIOSymbol.xaml
    /// </summary>
    public partial class SelectSymbolDialog: Window
    {
        private bool _firstLoad = true;
        private string _filterKey = string.Empty;
        private readonly List<ObjectItem> _objectItems = new List<ObjectItem>();
        private readonly Dictionary<string, SymbolParemeter> _symbols;
        private readonly Dictionary<string, string> _filterKeysOfSymbol;
        private readonly List<ObjectTypeItem> _objectTypes = new List<ObjectTypeItem>(); 
        private readonly Dictionary<ObjectType, ImageSource> _images = new Dictionary<ObjectType, ImageSource>();
        private AOrmObject _existingObject;

        public SymbolType SelectedSymbol { get; private set; }
        public List<Category> Categories = new List<Category>();

        public AOrmObject ExistingObject
        {
            get
            {
                return _existingObject;
            }
            set
            {
                _existingObject = value;

                if (_existingObject != null)
                {
                    foreach (ObjectTypeItem item in _cbObjectType.Items)
                    {
                        if (item.objectType == _existingObject.GetObjectType())
                        {
                            _cbObjectType.SelectedItem = item;
                            break;
                        }
                    }
                }
            }
        }

        public ObjectType SelectedObjectType { get; private set; }
        public Guid? SelectedObjectGuid { get; private set; }

        public SelectSymbolDialog(Dictionary<string, SymbolParemeter> symbols, Dictionary<string, string> filterKeysOfSymbol)
        {
            InitializeComponent();
            _symbols = symbols;
            _filterKeysOfSymbol = filterKeysOfSymbol;

            _images.Add(ObjectType.CardReader, new BitmapImage(new Uri("Resources/Cardreader48.png", UriKind.Relative)));
            _images.Add(ObjectType.AlarmArea, new BitmapImage(new Uri("Resources/AlarmAreasNew48.png", UriKind.Relative)));
            _images.Add(ObjectType.Output, new BitmapImage(new Uri("Resources/Outputs48.png", UriKind.Relative)));
            _images.Add(ObjectType.DoorEnvironment, new BitmapImage(new Uri("Resources/Doorenvironment48.png", UriKind.Relative)));
            _images.Add(ObjectType.Input, new BitmapImage(new Uri("Resources/Inputs48.png", UriKind.Relative)));
            _images.Add(ObjectType.CCU, new BitmapImage(new Uri("Resources/CCU48.png", UriKind.Relative)));
            _images.Add(ObjectType.DCU, new BitmapImage(new Uri("Resources/NewDcu48.png", UriKind.Relative)));
            
            _objectTypes.Add(new ObjectTypeItem(ObjectType.AlarmArea, _images[ObjectType.AlarmArea]));
            _objectTypes.Add(new ObjectTypeItem(ObjectType.CardReader, _images[ObjectType.CardReader]));
            _objectTypes.Add(new ObjectTypeItem(ObjectType.Output, _images[ObjectType.Output]));
            _objectTypes.Add(new ObjectTypeItem(ObjectType.DoorEnvironment, _images[ObjectType.DoorEnvironment]));
            _objectTypes.Add(new ObjectTypeItem(ObjectType.Input, _images[ObjectType.Input]));
            _objectTypes.Add(new ObjectTypeItem(ObjectType.CCU, _images[ObjectType.CCU]));
            _objectTypes.Add(new ObjectTypeItem(ObjectType.DCU, _images[ObjectType.DCU]));

            foreach (ObjectTypeItem item in _objectTypes)
            {
                _cbObjectType.Items.Add(item);
            }

            _cbObjectType.SelectedIndex = 0;
            GraphicsScene.LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
        }

        void LocalizationHelper_LanguageChanged()
        {
            GraphicsScene.LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this);
        }

        public void DisposeLocalizationHelper()
        {
            GraphicsScene.LocalizationHelper.LanguageChanged -= LocalizationHelper_LanguageChanged;
        }

        private void LoadSymbols()
        {
            if (_symbols == null)
                return;

            foreach (string symbolType in _symbols.Keys)
            {
                if (_filterKey.ToLower() == _filterKeysOfSymbol[symbolType].ToLower())
                {
                    var item = new SymbolItem(_symbols[symbolType].ImageSource, _symbols[symbolType].SymbolType);
                    _lvSymbols.Items.Add(item);
                }
            }

            if (_lvSymbols.Items.Count > 0)
                _lvSymbols.SelectedIndex = 0;
        }

        private void _bSelect_Click(object sender, RoutedEventArgs e)
        {
            if (_chbExistingObject.IsChecked.Value && (_lvObjectList.SelectedItem as ObjectItem) == null)
            {
                Contal.IwQuick.UI.Dialog.Warning(GraphicsScene.LocalizationHelper.GetString("WarningSelectLeastOneObject"));
                return;
            }

            if (_lvSymbols.SelectedItem == null && _lvSymbols.Items.Count > 0)
                _lvSymbols.SelectedIndex = 0;

            FillCategories();

            if (Categories.Count == 0)
            {
                Contal.IwQuick.UI.Dialog.Warning(GraphicsScene.LocalizationHelper.GetString("WarningSelectLeastOneCategory"));
                return;
            }

            if (_lvSymbols.SelectedItem == null
                || _cbObjectType.SelectedItem == null)
                return;

            SelectedSymbol = ((SymbolItem) _lvSymbols.SelectedItem).TypeOfSymbol;
            SelectedObjectType = ((ObjectTypeItem) _cbObjectType.SelectedItem).objectType;

            if (!_chbOnlySymbol.IsChecked.Value)
                SelectedObjectGuid = ((ObjectItem) _lvObjectList.SelectedItem).Id;
            else
                SelectedObjectGuid = null;
            
            DialogResult = true;
        }

        private void _bCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void _cbObjectType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _lvSymbols.Items.Clear();
            IsEnableControls(false);
            Contal.IwQuick.Threads.SafeThread<ObjectType>.StartThread(LoadObjects, 
                ((ObjectTypeItem) _cbObjectType.SelectedItem).objectType);

            _filterKey = ((ObjectTypeItem) _cbObjectType.SelectedItem).objectType.ToString();

            if (_filterKey.ToLower() == ObjectType.Input.ToString().ToLower() || 
                _filterKey.ToLower() == ObjectType.Output.ToString().ToLower())
            {
                _filterKey = ObjectType.Input.ToString().ToLower();
                LoadSymbols();

                _filterKey = ObjectType.Output.ToString().ToLower();
                LoadSymbols();
            }
            else
                LoadSymbols();
        }

        private void IsEnableControls(bool enable)
        {
            int count = VisualTreeHelper.GetChildrenCount(MainGrid);

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(MainGrid, i) as Control;

                if (child != null)
                    child.IsEnabled = enable;
            }
        }

        private readonly AutoResetEvent _autoResetEvent = new AutoResetEvent(true);

        private void LoadObjects(ObjectType objectType)
        {
            try
            {
                _autoResetEvent.WaitOne();
                IEnumerable<IShortObject> objects = MainServerProvider.GetObjects(objectType);
                _objectItems.Clear();

                foreach (IShortObject obj in objects)
                { 
                    ObjectItem item = new ObjectItem((Guid)obj.Id, obj.Name, _images[obj.ObjectType]);
                    _objectItems.Add(item);
                }

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    if (objects.Count() == 0)
                    {
                        _chbOnlySymbol.IsChecked = true;
                        return;
                    }

                    if (_objectItems != null)
                    {
                        _tbFilter.Text = string.Empty;
                        _lvObjectList.ItemsSource = null;
                        _lvObjectList.ItemsSource = _objectItems;
                        _lvObjectList.SelectedIndex = 0;

                        if (ExistingObject != null && 
                            _firstLoad &&
                            ExistingObject.GetObjectType() == objectType)
                        {
                            _firstLoad = false;
                            int index = 0;

                            foreach (ObjectItem obj in _lvObjectList.Items)
                            {
                                if (obj.Id == (Guid)ExistingObject.GetId())
                                {
                                    _lvObjectList.SelectedItem = obj;
                                    _lvObjectList.ScrollIntoView(obj);
                                    break;
                                }

                                index++;
                            }
                        }
                    }
                }));
            }
            catch
            {
            }
            finally
            {
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
                {
                    IsEnableControls(true);

                    if (_chbOnlySymbol.IsChecked.Value)
                        _lvObjectList.IsEnabled = false;
                }));

                _autoResetEvent.Set();
            }
        }

        public void FillCategories()
        {
            if (_chbAccessControl.IsChecked.Value)
                Categories.Add(Category.AccessControl);

            if (_chbFire.IsChecked.Value)
                Categories.Add(Category.Fire);

            if (_chBurglarAlarm.IsChecked.Value)
                Categories.Add(Category.BurglarAlarm);

            if (_chbAccessories.IsChecked.Value)
                Categories.Add(Category.Accessories);
        }

        private void _chbExistingObject_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                _chbOnlySymbol.IsChecked = false;
                _lvObjectList.IsEnabled = true;
            }
            catch
            {
            }
        }

        private void _chbOnlySymbol_OnChecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _chbExistingObject.IsChecked = false;
                _lvObjectList.IsEnabled = false;
            }
            catch
            {
            }
        }

        private void _chbOnlySymbol_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _chbExistingObject.IsChecked = true;
                _lvObjectList.IsEnabled = true;
            }
            catch
            {
            }
        }

        private void _chbExistingObject_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                _chbOnlySymbol.IsChecked = true;
                _lvObjectList.IsEnabled = false;
            }
            catch
            {
            }
        }

        private void _tbFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            _lvObjectList.ItemsSource = RunFilter((sender as TextBox).Text);
        }

        private IEnumerable<ObjectItem> RunFilter(string text)
        {
            if (string.IsNullOrEmpty(text))
                return _objectItems;

            return _objectItems.Where(x => x.Name.ToLower().Contains(text.ToLower()));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GraphicsScene.LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this);
        }

        private void SelectSymbolDialog_OnUnloaded(object sender, RoutedEventArgs e)
        {
            DisposeLocalizationHelper();
        }
    }

    public class SymbolItem
    {
        public ImageSource Picture { get; set; }
        public SymbolType TypeOfSymbol { get; set; }
        public Image image;
        public string Text { private set; get; }

        public SymbolItem(ImageSource picture, SymbolType symbolType)
        {
            Picture = picture;
            TypeOfSymbol = symbolType;
            Text = ToString();
        }

        public override string ToString()
        {
            return GraphicsScene.LocalizationHelper.GetString(string.Format("SymbolType_{0}", TypeOfSymbol.ToString()));
        }
    }

    public class ObjectTypeItem
    {
        public ObjectType objectType { get; set; }
        public string Text { get; private set; }
        public ImageSource Picture { get; set; }

        public ObjectTypeItem(ObjectType type, ImageSource source)
        {
            objectType = type;
            Text = GraphicsScene.LocalizationHelper.GetString("ObjectType_" + objectType);
            Picture = source;
        }

        public override string ToString()
        {
            return objectType.ToString();
        }
    }

    public class ObjectItem
    {
        public string Name { get; set; }
        public Guid Id { get; set; }
        public ImageSource Picture { get; set; }

        public ObjectItem(Guid id, string name, ImageSource source)
        {
            Id = id;
            Name = name;
            Picture = source;
        }
    }

    public enum Category
    {
        AccessControl, Fire, BurglarAlarm, Accessories
    }
}
