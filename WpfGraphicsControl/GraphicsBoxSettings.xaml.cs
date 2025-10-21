using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.IwQuick.Threads;

namespace Cgp.NCAS.WpfGraphicsControl
{
    /// <summary>
    /// Interaction logic for GraphicsBoxSettings.xaml
    /// </summary>
    public partial class GraphicsBoxSettings : UserControl
    {
        private GraphicsSceneBox _graphicsSceneBox;
        private bool _savingSettings;

        public GraphicsSceneBox SceneBox
        {
            get { return _graphicsSceneBox; }
            set
            {
                if (_graphicsSceneBox != null)
                {
                    _graphicsSceneBox.PositionChanged -= _graphicsSceneBox_PositionChanged;
                    _graphicsSceneBox.SizeChanged -= _graphicsSceneBox_SizeChanged;
                }

                _graphicsSceneBox = value;
                _graphicsSceneBox.PositionChanged += _graphicsSceneBox_PositionChanged;
                _graphicsSceneBox.SizeChanged += _graphicsSceneBox_SizeChanged;
                LoadSettings();
                IsEnabled = true;
            }
        }

        void _graphicsSceneBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_savingSettings)
                return;

            _tbWidth.Text = e.NewSize.Width.ToString("F1");
            _tbHeight.Text = e.NewSize.Height.ToString("F1");
        }

        void _graphicsSceneBox_PositionChanged(double left, double top)
        {
            if (_savingSettings)
                return;

            _tbLeft.Text = _graphicsSceneBox.GetLeft().ToString("F1");
            _tbTop.Text = _graphicsSceneBox.GetTop().ToString("F1");
        }

        public GraphicsBoxSettings()
        {
            InitializeComponent();
            IsEnabled = false;
            GraphicsScene.LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
        }

        public void DisposeLocalizationHelper()
        {
            GraphicsScene.LocalizationHelper.LanguageChanged -= LocalizationHelper_LanguageChanged;
        }

        void LocalizationHelper_LanguageChanged()
        {
            //GraphicsScene.LocalizationHelper.TranslateWpfControl(this);
        }

        private void GraphicsBoxSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            //GraphicsScene.LocalizationHelper.TranslateWpfControl(this);
        }

        private void LoadSettings()
        {
            if (_graphicsSceneBox == null)
                return;

            //set name
            _lGraphicsBoxName.Content = _graphicsSceneBox.BoxName;
            _tbSceneBoxName.Text = _graphicsSceneBox.BoxName;

            //set horizontal alignment
            _cbHorizontalAlignment.Items.Clear();
            int i = 0;

            foreach (var hAlignment in Enum.GetValues(typeof (GraphicsBoxHorizontalAlignment)))
            {
                _cbHorizontalAlignment.Items.Add(hAlignment);

                if (hAlignment.ToString() == _graphicsSceneBox.HAlignment.ToString())
                {
                    _cbHorizontalAlignment.SelectedIndex = i;
                }

                i++;
            }

            //set vertical alignment
            _cbVerticalAlignment.Items.Clear();
            i = 0;

            foreach (var vAlignment in Enum.GetValues(typeof(GraphicsBoxVerticalAlignment)))
            {
                _cbVerticalAlignment.Items.Add(vAlignment);

                if (vAlignment.ToString() == _graphicsSceneBox.VAlignment.ToString())
                {
                    _cbVerticalAlignment.SelectedIndex = i;
                }

                i++;
            }

            //set position
            _tbLeft.Text = _graphicsSceneBox.GetLeft().ToString("F0");
            _tbTop.Text = _graphicsSceneBox.GetTop().ToString("F0");

            //set size
            _tbWidth.Text = _graphicsSceneBox.GetWidth().ToString("F0");
            _tbHeight.Text = _graphicsSceneBox.GetHeight().ToString("F0");
           
            _chbPercentuallyWidth.IsChecked = _graphicsSceneBox.EnablePercentuallyWidth;            
            _chbPercentuallyHeight.IsChecked = _graphicsSceneBox.EnablePercentuallyHeight;

            //set border width and color
            _tbBorderWidth.Value = _graphicsSceneBox.BorderWidth;
            _cpBorderColor.SelectedColor = _graphicsSceneBox.BorderColor != null
                ? _graphicsSceneBox.BorderColor.GetColor()
                : Colors.Black;

            //set scene
            SafeThread.StartThread(LoadScenes);

            //set scene link
            var sceneBoxs = _graphicsSceneBox.GetAllSceneBoxs();

            if (sceneBoxs != null)
            {
                _cbSceneBoxs.Items.Clear();
                _cbSceneBoxs.Items.Add(string.Empty);
                _cbSceneBoxs.SelectedIndex = 0;
                i = 0;

                foreach (var sceneBox in sceneBoxs)
                {
                    i++;
                    _cbSceneBoxs.Items.Add(sceneBox.BoxName);

                    if (sceneBox.BoxName == _graphicsSceneBox.SceneLinkTarget)
                        _cbSceneBoxs.SelectedIndex = i;
                }
            }
        }

        private void LoadScenes()
        {
            Exception ex;
            var scenesShort = GraphicsScene.MainServerProvider.Scenes.ShortSelectByCriteria(null, out ex);

            if (ex != null)
                return;

            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() =>
            {
                _cbScenes.ItemsSource = scenesShort;
                int i = 0;

                foreach (var sceneShort in scenesShort)
                {
                    if (sceneShort.IdScene == _graphicsSceneBox.GetSceneId())
                    {
                        _cbScenes.SelectedIndex = i;
                        break;
                    }

                    i++;
                }
            }));
        }

        private void _bApply_Click(object sender, RoutedEventArgs e)
        {
            ApplySettings();
        }

        private void ApplySettings()
        {
            if (_graphicsSceneBox != null)
            {
                _graphicsSceneBox._textBlock.UpdateLayout();
                _savingSettings = true;

                //set name
                if (_graphicsSceneBox.BoxName != _tbSceneBoxName.Text)
                {
                    var boxes = _graphicsSceneBox.GetAllSceneBoxs();

                    if (boxes.SingleOrDefault((box) => box.BoxName == _tbSceneBoxName.Text) == null)
                    {
                        foreach (var box in boxes.Where((box) => box.SceneLinkTarget == _graphicsSceneBox.BoxName))
                            box.SceneLinkTarget = _tbSceneBoxName.Text;

                        _graphicsSceneBox.BoxName = _tbSceneBoxName.Text;
                    }
                    else
                    {
                        Contal.IwQuick.UI.Dialog.Error("This name is already used.");
                    }
                }

                //set horizontal alignment
                _graphicsSceneBox.HAlignment = (GraphicsBoxHorizontalAlignment)_cbHorizontalAlignment.SelectedIndex;

                //set vertical alignment
                _graphicsSceneBox.VAlignment = (GraphicsBoxVerticalAlignment)_cbVerticalAlignment.SelectedIndex;

                //set positon and size
                double left;

                if (Double.TryParse(_tbLeft.Text, out left))
                    _graphicsSceneBox.SetLeft(left);
                else
                    Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("ErrorLeftValueMustBeNumber"));

                double top;

                if (Double.TryParse(_tbTop.Text, out top))
                    _graphicsSceneBox.SetTop(top);
                else
                    Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("ErrorTopValueMustBeNumber"));

                double width;

                if (Double.TryParse(_tbWidth.Text, out width))
                    _graphicsSceneBox.SetWidth(width);
                else
                    Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("ErrorWidthValueMustBeNumber"));

                double height;

                if (Double.TryParse(_tbHeight.Text, out height))
                    _graphicsSceneBox.SetHeight(height);
                else
                    Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("ErrorHeightValueMustBeNumber"));

                if (_chbPercentuallyWidth.IsChecked != null && _chbPercentuallyWidth.IsChecked.Value)
                    _graphicsSceneBox.EnablePercentuallyWidth = true;
                else
                    _graphicsSceneBox.EnablePercentuallyWidth = false;

                if (_chbPercentuallyHeight.IsChecked != null && _chbPercentuallyHeight.IsChecked.Value)
                    _graphicsSceneBox.EnablePercentuallyHeight = true;
                else
                    _graphicsSceneBox.EnablePercentuallyHeight = false;

                //set scene
                var selectedSceneShort = _cbScenes.SelectedItem as SceneShort;

                if (selectedSceneShort != null)
                    _graphicsSceneBox.SetSceneId(selectedSceneShort.IdScene);

                //set scene target link
                var selectedSceneBoxName = _cbSceneBoxs.SelectedItem as string;

                if (selectedSceneBoxName != null)
                    _graphicsSceneBox.SceneLinkTarget = selectedSceneBoxName;

                //set border width and color
                int borderWidth = 0;

                try
                {
                    borderWidth = (int) _tbBorderWidth.Value;
                }
                catch
                {
                }
                
                _graphicsSceneBox.BorderWidth = borderWidth;
                _graphicsSceneBox.BorderColor = new SerializableColor(_cpBorderColor.SelectedColor);
                _savingSettings = false;
            }
        }
    }
}
