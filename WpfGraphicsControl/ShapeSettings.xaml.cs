using System;
using System.Collections.Generic;
using System.IO;
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
using Contal.IwQuick.Threads;
using Contal.IwQuick.UI;
using Microsoft.Win32;

using Color = System.Windows.Media.Color;

namespace Cgp.NCAS.WpfGraphicsControl
{
    /// <summary>
    /// Interaction logic for MyShapeSettings.xaml
    /// </summary>
    public partial class ShapeSettings : UserControl
    {
        private UIElement _editElement;
        private bool _fillByColor;
        private CanvasSerialization _canvasSerialization;
        private GraphicsObjectType _graphicsObjectType;

        public List<Category> Categories = new List<Category>();
        public delegate void ShapeSettingsClosed(ShapeSettings sender);
        public event ShapeSettingsClosed Closed;
        public event Action ApplyClick;

        public ShapeSettings()
        {
            InitializeComponent();
            GraphicsScene.LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
        }

        public void DisposeLocalizationHelper()
        {
            GraphicsScene.LocalizationHelper.LanguageChanged -= LocalizationHelper_LanguageChanged;
        }

        void LocalizationHelper_LanguageChanged()
        {
            GraphicsScene.LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this);

            if (!(_editElement is ILiveObject))
                _lObjectName.Content = GraphicsScene.LocalizationHelper.GetString("Graphics_ShapeSettings");
            else
                _lObjectName.Content = _editingObjectName;
        }

        public void LoadSettings(UIElement element, CanvasSerialization canvasSerialization)
        {
            _tbImageSource.Text = string.Empty;
            _editElement = element;
            _canvasSerialization = canvasSerialization;
            var shape = _editElement as Shape;
            _graphicsObjectType = (_editElement as IGraphicsObject).GetGraphicsObjectType();

            if (shape is ISceneLink)
            {
                myGrid.RowDefinitions[6].Height = new GridLength(80);
                SafeThread.StartThread(LoadScenes);
            }
            else
                myGrid.RowDefinitions[6].Height = new GridLength(0);

            if (_graphicsObjectType == GraphicsObjectType.Line
                || _graphicsObjectType == GraphicsObjectType.Polyline
                || _graphicsObjectType == GraphicsObjectType.RectangleAndEllipse)
            {
                SetShowingModeForShape();
            }
            else
            {
                SetShowingModeForObject();
                
                //Loading Nova objects
                DoLoadSettings();
            }

            if (shape != null)
            {
                _tbLineWidth.Text = shape.StrokeThickness.ToString();
                _cpLineColor.SelectedColor = ((SolidColorBrush)shape.Stroke).Color;

                var solidColorBrush = shape.Fill as SolidColorBrush;

                if (solidColorBrush != null)
                {
                    var shapeColor = solidColorBrush.Color;
                    _cpBackgroundColor.SelectedColor = shapeColor;
                }
                else
                    _cpBackgroundColor.SelectedColor = Color.FromArgb(0, 0, 0, 0);
            }

            _cbLayers.Items.Clear();

            foreach (var layer in _canvasSerialization.Layers.Values)
            {
                _cbLayers.Items.Add(layer);

                if (layer.Id == ((IGraphicsObject)_editElement).GetLayerID())
                    _cbLayers.SelectedItem = layer;
            }

            if (_cbLayers.SelectedIndex == -1)
                _cbLayers.SelectedIndex = 0;

            if (!(_editElement is ILiveObject))
            {
                _fillByColor = false;
                return;
            }

            FillListOfCategories();
            _fillByColor = false;
        }

        private void DoLoadSettings()
        {
            var graphicsCr = _editElement as GraphicsCardReader;

            if (graphicsCr != null)
            {
                SafeThread<ObjectType>.StartThread(
                    LoadObjects, 
                    ObjectType.CardReader);

                return;
            }

            var graphicsPolygon = _editElement as GraphicsPolygon;

            if (graphicsPolygon != null)
            {
                SafeThread<ObjectType>.StartThread(
                    LoadObjects, 
                    ObjectType.AlarmArea);

                return;
            }

            var graphicsIo = _editElement as GraphicsIO;

            if (graphicsIo != null)
            {
                SafeThread<ObjectType>.StartThread(
                    LoadObjects,
                    graphicsIo.IOType == IOType.Output
                        ? ObjectType.Output
                        : ObjectType.Input);

                return;
            }

            var graphicsDoorEnvironment = _editElement as GraphicsDoorEnvironment;

            if (graphicsDoorEnvironment != null)
            {
                SafeThread<ObjectType>.StartThread(
                    LoadObjects,
                    ObjectType.DoorEnvironment);

                return;
            }

            var graphicsCcu = _editElement as GraphicsCcu;

            if (graphicsCcu != null)
            {
                SafeThread<ObjectType>.StartThread(
                    LoadObjects,
                    ObjectType.CCU);

                return;
            }

            var graphicsDcu = _editElement as GraphicsDcu;

            if (graphicsDcu != null)
            {
                SafeThread<ObjectType>.StartThread(
                    LoadObjects,
                    ObjectType.DCU);
            }
        }

        public UIElement GetEditElement()
        {
            return _editElement;
        }

        private void _bOk_Click(object sender, RoutedEventArgs e)
        {
            if (_graphicsObjectType == GraphicsObjectType.Line
                || _graphicsObjectType == GraphicsObjectType.Polyline
                || _graphicsObjectType == GraphicsObjectType.RectangleAndEllipse)
            {
                var rect = _editElement as RectangleAndEllipse;
                var shape = _editElement as Shape;

                if (shape != null)
                {
                    try
                    {
                        shape.StrokeThickness = Double.Parse(_tbLineWidth.Text);
                    }
                    catch
                    {
                    }

                    shape.Stroke = new SolidColorBrush(_cpLineColor.SelectedColor);

                    if (_fillByColor)
                    {
                        shape.Fill = new SolidColorBrush(_cpBackgroundColor.SelectedColor);

                        if (rect != null)
                            rect.BackgroundImage = null;
                    }

                    if (!string.IsNullOrEmpty(_tbImageSource.Text))
                    {
                        if (File.Exists(_tbImageSource.Text))
                        {
                            try
                            {
                                var src = new BitmapImage();

                                src.BeginInit();

                                src.UriSource = new Uri(_tbImageSource.Text, UriKind.Absolute);
                                src.CacheOption = BitmapCacheOption.OnLoad;

                                src.EndInit();

                                shape.Fill =
                                    new ImageBrush(src)
                                    {
                                        Stretch = Stretch.UniformToFill
                                    };

                                if (rect != null)
                                    rect.BackgroundImage = File.ReadAllBytes(_tbImageSource.Text);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Error. " + ex);
                            }
                        }
                        else
                            Dialog.Error(GraphicsScene.LocalizationHelper.GetString("ErrorFileNotFound"));
                    }
                }
            }

            ((IGraphicsObject)_editElement).SetLayerID(((Layer)_cbLayers.SelectedItem).Id);

            if (((Layer)_cbLayers.SelectedItem).Enabled)
                _editElement.Visibility = Visibility.Visible;
            else
            {
                ((IGraphicsObject)_editElement).UnSelect();
                _editElement.Visibility = Visibility.Hidden;
            }

            var liveObject = _editElement as ILiveObject;

            if (liveObject != null)
            {
                FillCategories();

                if (Categories.Count == 0)
                {
                    Dialog.Warning(GraphicsScene.LocalizationHelper.GetString("WarningSelectLeastOneCategory"));
                    return;
                }

                if (_cbObjects.SelectedItem != null)
                    liveObject.SetObjectGuid(
                        (Guid) (((IShortObject)_cbObjects.SelectedItem).Id));

                liveObject.SetCategories(Categories);
            }

            var sceneLink = _editElement as ISceneLink;
            var sceneShort = _cbScenes.SelectedItem as SceneShort;

            if (sceneLink != null
                && sceneShort != null)
            {
                sceneLink.SetSceneId(sceneShort.IdScene);
            }

            if (ApplyClick != null)
                ApplyClick();
        }

        private void _bCancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;

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
            var liveObject = (ILiveObject)_editElement;

            if (liveObject.GetCategories() == null)
                return;

            if (liveObject.GetCategories().Contains(Category.AccessControl))
                _chbAccessControl.IsChecked = true;

            if (liveObject.GetCategories().Contains(Category.Fire))
                _chbFire.IsChecked = true;

            if (liveObject.GetCategories().Contains(Category.BurglarAlarm))
                _chBurglarAlarm.IsChecked = true;

            if (liveObject.GetCategories().Contains(Category.Accessories))
                _chbAccessories.IsChecked = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            GraphicsScene.LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this);

            if (!(_editElement is ILiveObject))
                _lObjectName.Content = GraphicsScene.LocalizationHelper.GetString("Graphics_ShapeSettings");
        }

        private void SetShowingModeForShape()
        {
            myGrid.RowDefinitions[4].Height = new GridLength(0);
            myGrid.RowDefinitions[5].Height = new GridLength(0);
            myGrid.RowDefinitions[1].Height = new GridLength(80);
            
            if (_graphicsObjectType == GraphicsObjectType.RectangleAndEllipse)
                myGrid.RowDefinitions[2].Height = new GridLength(65);
            else
                myGrid.RowDefinitions[2].Height = new GridLength(0);

            if (_graphicsObjectType == GraphicsObjectType.RectangleAndEllipse
                || (_graphicsObjectType == GraphicsObjectType.Polyline
                    && (_editElement as GraphicsPolygon).IsClosed))
            {
                _lBackgroundColor.IsEnabled = true;
                _cpBackgroundColor.IsEnabled = true;
            }
            else
            {
                _lBackgroundColor.IsEnabled = false;
                _cpBackgroundColor.IsEnabled = false;
            }

            if (!(_editElement is ILiveObject))
                _lObjectName.Content = GraphicsScene.LocalizationHelper.GetString("Graphics_ShapeSettings");
        }

        private void SetShowingModeForObject()
        {
            myGrid.RowDefinitions[1].Height = new GridLength(0);
            myGrid.RowDefinitions[2].Height = new GridLength(0);
            myGrid.RowDefinitions[4].Height = new GridLength(90);
            myGrid.RowDefinitions[5].Height = new GridLength(80);
        }

        private void LoadObjects(ObjectType type)
        {
            var allObjects = MainServerProvider.GetObjects(type);
            var objects = new LinkedList<IShortObject>();

            foreach (var obj in allObjects)
                if (CgpClient.Singleton.MainServerProvider.HasAccessView(obj.ObjectType, obj.Id))
                    objects.AddLast(obj);

            var selectedIndex = -1;
            var count = -1;

            foreach (var shortObject in objects)
            {
                count++;

                if (((ILiveObject)_editElement).GetObjectGuid() == (Guid)shortObject.Id)
                {
                    selectedIndex = count;  
                    break;
                }
            }

            Dispatcher.Invoke(
                DispatcherPriority.Normal, 
                new Action(() =>
                {
                    _cbObjects.ItemsSource = objects;

                    if (selectedIndex > -1)
                        _cbObjects.SelectedIndex = selectedIndex;

                    var shortObject = (_cbObjects.SelectedItem as IShortObject);

                    _lObjectName.Content = 
                        shortObject != null
                            ? shortObject.Name
                            : GraphicsScene.LocalizationHelper.GetString("Graphics_None");

                    if (shortObject != null)
                        _editingObjectName = shortObject.Name;
                }));
        }

        private string _editingObjectName = string.Empty;

        private void LoadScenes()
        {
            var allObjects = MainServerProvider.GetObjects(ObjectType.Scene);
            var objects = new LinkedList<IShortObject>();

            foreach (var obj in allObjects)
                if (CgpClient.Singleton.MainServerProvider.HasAccessView(obj.ObjectType, obj.Id))
                    objects.AddLast(obj);

            var selectedIndex = -1;
            var count = -1;

            foreach (var shortObject in objects)
            {
                count++;

                if (((ISceneLink)_editElement).GetSceneId() == (Guid)shortObject.Id)
                {
                    selectedIndex = count;
                    break;
                }
            }

            Dispatcher.Invoke(
                DispatcherPriority.Normal,
                new Action(() =>
                {
                    _cbScenes.ItemsSource = objects;

                    if (selectedIndex > -1)
                        _cbScenes.SelectedIndex = selectedIndex;
                }));
        }

        private void _bAddImageSource_Click(object sender, RoutedEventArgs e)
        {
            var dlg = 
                new OpenFileDialog
                {
                    DefaultExt = ".jpg, .png",
                    Filter = "Image formats |*.jpg;*.png"
                };

            if (dlg.ShowDialog() == true)
                _tbImageSource.Text = dlg.FileName;
        }

        private void _cpBackgroundColor_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color> e)
        {
            _fillByColor = true;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        private void _bRemoveObject_OnClick(object sender, RoutedEventArgs e)
        {
            var liveObject = _editElement as ILiveObject;

            if (liveObject != null)
            {
                liveObject.SetObjectGuid(Guid.Empty);
                _cbObjects.SelectedIndex = -1;
            }
        }

        private void _bRemoveScene_OnClick(object sender, RoutedEventArgs e)
        {
            var sceneLink = _editElement as ISceneLink;

            if (sceneLink != null)
            {
                sceneLink.SetSceneId(Guid.Empty);
                _cbScenes.SelectedIndex = -1;
            }
        }
    }
}
