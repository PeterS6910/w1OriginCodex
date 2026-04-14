using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.Cgp.Server.Beans;
using Color = System.Windows.Media.Color;

namespace Cgp.NCAS.WpfGraphicsControl
{
    /// <summary>
    /// Interaction logic for MyShapeSettings.xaml
    /// </summary>
    public partial class MyShapeSettings : UserControl
    {
        //Title="Shape settings" Width="360"  WindowStyle="ToolWindow" Loaded="Window_Loaded" ResizeMode="NoResize" SizeToContent="WidthAndHeight"
        private UIElement _editElement;
        private bool _fillByColor = false;
        private CanvasSerialization CanvasSerialization;

        public List<Category> Categories = new List<Category>();
        public delegate void ShapeSettingsClosed(MyShapeSettings sender);
        public event ShapeSettingsClosed Closed;

        public MyShapeSettings(UIElement element, CanvasSerialization canvasSerialization)
        {
            InitializeComponent();
            _editElement = element;
            CanvasSerialization = canvasSerialization;
        }

        public UIElement GetEditElement()
        {
            return _editElement;
        }

        private void _bOk_Click(object sender, RoutedEventArgs e)
        {
            myRectangleAndEllipse shape = _editElement as myRectangleAndEllipse;

            if (shape != null)
            {
                shape.StrokeThickness = Double.Parse(_tbLineWidth.Text);
                shape.Stroke = new SolidColorBrush(_cpLineColor.SelectedColor);

                if (_fillByColor)
                {
                    shape.Fill = new SolidColorBrush(_cpBackgroundColor.SelectedColor);
                    shape.BackgroundImage = null;
                }

                if (!string.IsNullOrEmpty(_tbImageSource.Text))
                {
                    if (File.Exists(_tbImageSource.Text))
                    {
                        try
                        {
                            BitmapImage src = new BitmapImage();
                            src.BeginInit();
                            src.UriSource = new Uri(_tbImageSource.Text, UriKind.Absolute);
                            src.CacheOption = BitmapCacheOption.OnLoad;
                            src.EndInit();

                            ImageBrush brush = new ImageBrush(src);
                            brush.Stretch = Stretch.UniformToFill;
                            shape.Fill = brush;
                            shape.BackgroundImage = File.ReadAllBytes(_tbImageSource.Text);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error. " + ex.ToString());
                        }
                    }
                    else
                        MessageBox.Show("File not found.");
                }
            }

            (_editElement as ImyShape).SetLayerID((_cbLayers.SelectedItem as Layer).Id);

            if ((_cbLayers.SelectedItem as Layer).Enabled)
                _editElement.Visibility = Visibility.Visible;
            else
            {
                (_editElement as ImyShape).UnSelect();
                _editElement.Visibility = Visibility.Hidden;
            }

            if (_editElement is ImyLiveObject)
            {
                FillCategories();

                if (Categories.Count == 0)
                {
                    MessageBox.Show("You must select least one Category.");
                    return;
                }

                if (_cbObjects.SelectedItem != null)
                    (_editElement as ImyLiveObject).SetObjectGuid(
                        (Guid) ((_cbObjects.SelectedItem as IShortObject).Id));

                (_editElement as ImyLiveObject).SetCategories(Categories);
            }

            this.Visibility = Visibility.Collapsed;
            
            if (Closed != null)
                Closed(this);
        }

        private void _bCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;

            if (Closed != null)
                Closed(this);
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

        private void FillListOfCategories()
        {
            if ((_editElement as ImyLiveObject).GetCategories() == null)
                return;

            if ((_editElement as ImyLiveObject).GetCategories().Contains(Category.AccessControl))
                _chbAccessControl.IsChecked = true;

            if ((_editElement as ImyLiveObject).GetCategories().Contains(Category.Fire))
                _chbFire.IsChecked = true;

            if ((_editElement as ImyLiveObject).GetCategories().Contains(Category.BurglarAlarm))
                _chBurglarAlarm.IsChecked = true;

            if ((_editElement as ImyLiveObject).GetCategories().Contains(Category.Accessories))
                _chbAccessories.IsChecked = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Shape shape = _editElement as Shape;

            if (shape is myRectangleAndEllipse)
                SetShowingModeForShape();
            else
                SetShowingModeForObject();
 
            if (shape != null)
            {              
                _tbLineWidth.Text = shape.StrokeThickness.ToString();
                _cpLineColor.SelectedColor = (shape.Stroke as SolidColorBrush).Color;

                if ((shape.Fill as SolidColorBrush) != null)
                {
                    Color shapeColor = (shape.Fill as SolidColorBrush).Color;
                    _cpBackgroundColor.SelectedColor = shapeColor;
                }
                else
                    _cpBackgroundColor.SelectedColor = Color.FromArgb(0, 0, 0, 0);
            }

            foreach (Layer layer in CanvasSerialization.Layers.Values)
            {
                _cbLayers.Items.Add(layer);

                if (layer.Id == (_editElement as ImyShape).GetLayerID())
                    _cbLayers.SelectedItem = layer; 
            }

            if (_cbLayers.SelectedIndex == -1)
                _cbLayers.SelectedIndex = 0;

            if (_editElement is ImyLiveObject)
            {
                //Loading Nova objects
                if (_editElement is myCardReader)
                    Contal.IwQuick.Threads.SafeThread<ObjectType>.StartThread(LoadObjects, ObjectType.CardReader);
                else if (_editElement is myPolygon)
                    Contal.IwQuick.Threads.SafeThread<ObjectType>.StartThread(LoadObjects, ObjectType.AlarmArea);
                else if (_editElement is myIO)
                {
                    if ((_editElement as myIO).IOType == IOType.Output)
                        Contal.IwQuick.Threads.SafeThread<ObjectType>.StartThread(LoadObjects, ObjectType.Output);
                    else
                        Contal.IwQuick.Threads.SafeThread<ObjectType>.StartThread(LoadObjects, ObjectType.Input);
                }
                else if (_editElement is myDoorEnvironment)
                    Contal.IwQuick.Threads.SafeThread<ObjectType>.StartThread(LoadObjects, ObjectType.DoorEnvironment);

                FillListOfCategories();
            }

            _fillByColor = false;
        }

        private void SetShowingModeForShape()
        {
            myGrid.RowDefinitions[4].Height = new GridLength(0);
            myGrid.RowDefinitions[5].Height = new GridLength(0);
            _lObjectName.Content = "Shape";
        }

        private void SetShowingModeForObject()
        {
            myGrid.RowDefinitions[1].Height = new GridLength(0);
            myGrid.RowDefinitions[2].Height = new GridLength(0);
        }

        private void LoadObjects(ObjectType type)
        {
            IEnumerable<IShortObject> allObjects = MainServerProvider.GetObjects(type);
            LinkedList<IShortObject> objects = new LinkedList<IShortObject>();

            foreach (IShortObject obj in allObjects)
            {
                if (CgpClient.Singleton.MainServerProvider.HasAccessView(obj.ObjectType, obj.Id))
                    objects.AddLast(obj);
            }

            int selectedIndex = -1;
            int count = -1;

            if (objects == null)
                return;

            foreach (var shortObject in objects)
            {
                count++;

                if ((_editElement as ImyLiveObject).GetObjectGuid() == (Guid)shortObject.Id)
                {
                    selectedIndex = count;  
                    break;
                }
            }

            this.Dispatcher.Invoke(DispatcherPriority.Normal, new System.Action(() =>
            {
                if (selectedIndex > -1)
                    _cbObjects.SelectedIndex = selectedIndex;

                _cbObjects.ItemsSource = objects;

                if ((_cbObjects.SelectedItem as IShortObject) != null)
                    _lObjectName.Content = (_cbObjects.SelectedItem as IShortObject).Name;
                else
                    _lObjectName.Content = "None";
            }));
        }

        private void _bAddImageSource_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg, .png";
            dlg.Filter = "Image formats |*.jpg;*.png";

            if (dlg.ShowDialog() == true)
            {
                _tbImageSource.Text = dlg.FileName;
            }
        }

        private void _cpBackgroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            _fillByColor = true;
        }

        public void Show()
        {
            this.Visibility = Visibility.Visible;
        }

        private void _bRemoveObject_OnClick(object sender, RoutedEventArgs e)
        {
            if ((_editElement as ImyLiveObject) != null)
            {
                (_editElement as ImyLiveObject).SetObjectGuid(Guid.Empty);
                _cbObjects.SelectedIndex = -1;
            }
        }
    }
}
