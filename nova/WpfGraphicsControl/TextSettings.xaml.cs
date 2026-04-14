using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

using Contal.Cgp.Client;
using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.WpfGraphicsControl;
using Contal.IwQuick.Threads;

namespace Cgp.NCAS.WpfGraphicsControl
{
    /// <summary>
    /// Interaction logic for TextSettings.xaml
    /// </summary>
    public partial class TextSettings : UserControl
    {
        private Text _text;
        private CanvasSerialization _canvasSerialization;

        public delegate void TextSettingsClosed(TextSettings sender);
        public event TextSettingsClosed Closed;
        public event Action ApplyClick;

        public TextSettings()
        {
            InitializeComponent();
            GraphicsScene.LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
            _lTextSettings.Content = GraphicsScene.LocalizationHelper.GetString("Graphics_TextSettings");
        }

        void LocalizationHelper_LanguageChanged()
        {
            GraphicsScene.LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this);
            _lTextSettings.Content = GraphicsScene.LocalizationHelper.GetString("Graphics_TextSettings");
        }

        public void DisposeLocalizationHelper()
        {
            GraphicsScene.LocalizationHelper.LanguageChanged -= LocalizationHelper_LanguageChanged;
        }

        public void LoadTextSettings(Text text, CanvasSerialization canvasSerialization)
        {
            _text = text;
            _canvasSerialization = canvasSerialization;
            
            if (_text == null)
                _bCancel_Click(this, null);

            FillFontFamilyComboBox();
            LoadTextParametres();            
            FillLayersComboBox();
            SafeThread.StartThread(LoadScenes);
        }

        private void _bCancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            if (Closed != null)
                Closed(this);
        }

        public Text GetEditText()
        {
            return _text;
        }

        private void TextSettings_OnLoaded(object sender, RoutedEventArgs e)
        {
            GraphicsScene.LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this);
            _lTextSettings.Content = GraphicsScene.LocalizationHelper.GetString("Graphics_TextSettings");
        }

        private void FillLayersComboBox()
        {
            _cbLayers.Items.Clear();

            foreach (Layer layer in _canvasSerialization.Layers.Values)
            {
                _cbLayers.Items.Add(layer);

                if (layer.Id == _text.GetLayerID())
                    _cbLayers.SelectedItem = layer;
            }

            if (_cbLayers.SelectedIndex == -1)
                _cbLayers.SelectedIndex = 0;
        }

        private void FillFontFamilyComboBox()
        {
            _cbFontFamily.Items.Clear();

            foreach (FontFamily fontFamily in Fonts.SystemFontFamilies)
            {
                _cbFontFamily.Items.Add(fontFamily);

                if (_text.FontFamily.ToString() == fontFamily.ToString())
                    _cbFontFamily.SelectedItem = fontFamily;
            }
        }

        private void LoadTextParametres()
        {
            _tbFontSize.Text = _text.FontSize.ToString("F2");
            _cpTextColor.SelectedColor = ((SolidColorBrush) _text.Foreground).Color;

            if (_text.Background != null)
                _cpBackgroundColor.SelectedColor = ((SolidColorBrush) _text.Background).Color;
            else
                _cpBackgroundColor.SelectedColor = Color.FromArgb(0, 0, 0, 0);

            _chbBold.IsChecked = _text.FontWeight == FontWeights.Bold;
            _chbItalic.IsChecked = _text.FontStyle == FontStyles.Italic;
            _chbUnderline.IsChecked = _text.TextDecorations == TextDecorations.Underline;
        }

        private void _bApply_Click(object sender, RoutedEventArgs e)
        {
            _text.UnSelect();
            SaveSettings();
            _text.Select(true);

            if (ApplyClick != null)
                ApplyClick();
        }

        private void SaveSettings()
        {
            _text.FontFamily = _cbFontFamily.SelectedItem as FontFamily;
            _text.FontSize = Double.Parse(_tbFontSize.Text);
            _text.Foreground = new SolidColorBrush(_cpTextColor.SelectedColor);
            _text.Background = new SolidColorBrush(_cpBackgroundColor.SelectedColor);

            if (_chbBold.IsChecked.Value)
                _text.FontWeight = FontWeights.Bold;
            else
                _text.FontWeight = FontWeights.Normal;

            if (_chbItalic.IsChecked.Value)
                _text.FontStyle = FontStyles.Italic;
            else
                _text.FontStyle = FontStyles.Normal;

            if (_chbUnderline.IsChecked.Value)
                _text.TextDecorations = TextDecorations.Underline;
            else
                _text.TextDecorations = null;

            var selectedLayer = _cbLayers.SelectedItem as Layer;

            if (selectedLayer != null)
                _text.SetLayerID(selectedLayer.Id);
            
            var selectedSceneShort = _cbScenes.SelectedItem as IShortObject;

            if (selectedSceneShort != null)
                _text.SetSceneId((Guid)(selectedSceneShort.Id));
            
            _text.UpdateLayout();
        }

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

                if (_text.GetSceneId() == (Guid) shortObject.Id)
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

        private void _bRemoveScene_Click(object sender, RoutedEventArgs e)
        {
            if (_text != null)
            {
                _text.SetSceneId(Guid.Empty);
                _cbScenes.SelectedIndex = -1;
            }
        }
    }
}
