using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Contal.Cgp.Globals;
using JetBrains.Annotations;
using Button = System.Windows.Controls.Button;
using CheckBox = System.Windows.Controls.CheckBox;
using ListViewItem = System.Windows.Controls.ListViewItem;
using UserControl = System.Windows.Controls.UserControl;

namespace Contal.Cgp.Components
{
    /// <summary>
    /// Interaction logic for WpfObjectListView.xaml
    /// </summary>
    public partial class WpfObjectListView : UserControl
    {
        private class ObjectItem
        {
            public bool IsSelected { get; set; }
            public ImageSource Icon { get; set; }
            public object Id { get; set; }
            public string Name { get; set; }
        }

        private List<IShortObject> _items;
        private bool _enableCheckboxes;
        private bool _enableDeleteButtons;
        private bool _enableInsertButton;
        private readonly Dictionary<string, ImageSource> _icons = new Dictionary<string, ImageSource>();
        private readonly List<Button> _lastSelectedDeleteButtons = new List<Button>();

        public bool EnableCheckboxes 
        {
            get { return _enableCheckboxes; }
            set
            {
                _enableCheckboxes = value;
                _lwObjects.UpdateLayout();
            }
        }

        public bool DefaultCheckBoxesState { get; set; }

        public bool EnableDeleteButtons
        {
            get { return _enableDeleteButtons; }
            set
            {
                _enableDeleteButtons = value;
                _lwObjects.UpdateLayout();
            }
        }

        public bool EnableInsertButton
        {
            get { return _enableInsertButton; }
            set
            {
                _enableInsertButton = value;

                _bInsert.Visibility = _enableInsertButton
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
        }

        public event Action<ICollection<IShortObject>> DeleteAction;
        public event Action InsertAction;

        [NotNull]
        public List<IShortObject> Items
        {
            get { return _items; }
            set
            {
                _lastSelectedDeleteButtons.Clear();
                _items = value;
                UpdateItems();

                if (_lwObjects.Items.Count == 1)
                    _lwObjects.SelectedIndex = 0;
            }
        }

        public void UpdateItems()
        {
            var source = new List<ObjectItem>((_items.Select(
                item => new ObjectItem
                {
                    IsSelected = DefaultCheckBoxesState,
                    Icon = GetIcon(item.ObjectType.ToString()),
                    Id = item.Id,
                    Name = item.Name
                })));

            _lwObjects.ItemsSource = source;
        }

        public void Clear()
        {
            if (_items != null)
                _items.Clear();

            _lwObjects.ItemsSource = null;
        }

        public ImageList Icons 
        {
            set
            {
                _icons.Clear();

                foreach (var key in value.Images.Keys)
                {
                    _icons.Add(key, ConvertDrawingImageToWPFImage(value.Images[key]));
                }
            }
        }

        public void SelectAll()
        {
            SetCheckBoxesState(true);
        }

        public void UnselectAll()
        {
            SetCheckBoxesState(false);
        }

        private void SetCheckBoxesState(bool isChecked)
        {
            if (_lwObjects.ItemsSource == null
                || !EnableCheckboxes)
                return;

            DefaultCheckBoxesState = isChecked;
            UpdateItems();
        }

        public List<IShortObject> SelectedItems
        {
            get
            {
                HashSet<object> selectedItems;

                if (_enableCheckboxes)
                {
                    selectedItems = new HashSet<object>();

                    for (int i = 0; i < _lwObjects.Items.Count; i++)
                    {
                        var listViewItem = (ListViewItem) (_lwObjects.ItemContainerGenerator.ContainerFromIndex(i));

                        if (listViewItem == null)
                            continue;

                        var contentPresenter = FindVisualChild<ContentPresenter>(listViewItem);
                        var dataTemplate = contentPresenter.ContentTemplate;
                        var checkBox = (CheckBox) dataTemplate.FindName("_checkBox", contentPresenter);

                        if (checkBox.IsChecked != null
                            && checkBox.IsChecked.Value)
                        {
                            selectedItems.Add(((ObjectItem) _lwObjects.Items[i]).Id);
                        }
                    }
                }
                else
                    selectedItems = new HashSet<object>(
                        _lwObjects.SelectedItems.Cast<ObjectItem>().Select(obj => obj.Id));


                return _items.Where(obj => selectedItems.Contains(obj.Id)).ToList();
            }
        }

        public IShortObject SelectItem
        {
            set
            {
                int index = _items.IndexOf(value);

                if (index >= 0)
                    _lwObjects.SelectedIndex = index;
            }
        }

        public WpfObjectListView()
        {
            InitializeComponent();
            _lwObjects.LayoutUpdated += _lwObjects_LayoutUpdated;

            Dispatcher.BeginInvoke(DispatcherPriority.Background,
                new Action(() => _lwObjects.Focus()));
        }

        private ImageSource ConvertDrawingImageToWPFImage(System.Drawing.Image gdiImg)
        {
            var bmp = new System.Drawing.Bitmap(gdiImg);
            var hBitmap = bmp.GetHbitmap();
            
            var WpfBitmap =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());

            return WpfBitmap;
        }

        private ImageSource GetIcon(string objectType)
        {
            if (_icons == null
                || !_icons.ContainsKey(objectType))
                return null;

            return _icons[objectType];
        }

        private void _lwObjects_LayoutUpdated(object sender, EventArgs e)
        {
            foreach (var button in _lastSelectedDeleteButtons)
                    button.Visibility = Visibility.Hidden;
            
            _lastSelectedDeleteButtons.Clear();

            for (int i = 0; i < _lwObjects.Items.Count; i++)
            {
                var listViewItem = (ListViewItem)(_lwObjects.ItemContainerGenerator.ContainerFromIndex(i));

                if (listViewItem == null)
                    continue;

                var contentPresenter = FindVisualChild<ContentPresenter>(listViewItem);
                var dataTemplate = contentPresenter.ContentTemplate;

                var mainGrid = (Grid)dataTemplate.FindName("_mainGrid", contentPresenter);
                mainGrid.ColumnDefinitions[0].Width = new GridLength(_enableCheckboxes ? 22 : 0);
                mainGrid.ColumnDefinitions[1].Width = new GridLength(_enableDeleteButtons ? 22 : 0);

                if (_enableDeleteButtons
                    && _lwObjects.SelectedItems.Contains(_lwObjects.Items[i]))
                {
                    var deleteButton = (Button) dataTemplate.FindName("_bDelete", contentPresenter);
                    _lastSelectedDeleteButtons.Add(deleteButton);
                    deleteButton.Visibility = Visibility.Visible;
                }
            }
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);

                if (child is childItem)
                    return (childItem)child;

                var childOfChild = FindVisualChild<childItem>(child);

                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

        private void _bDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (!EnableDeleteButtons
                || DeleteAction == null)
            {
                return;
            }

            DeleteAction(SelectedItems);
        }

        private void _bInsert_OnClick(object sender, RoutedEventArgs e)
        {
            if (!_enableInsertButton
                || InsertAction == null)
            {
                return;
            }

            InsertAction();
        }
    }
}
