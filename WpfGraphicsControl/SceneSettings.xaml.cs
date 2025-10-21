using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Contal.Cgp.NCAS.Server.Beans;
using Contal.Cgp.NCAS.WpfGraphicsControl;

namespace Cgp.NCAS.WpfGraphicsControl
{
    /// <summary>
    /// Interaction logic for SceneSettings.xaml
    /// </summary>
    public partial class SceneSettings : UserControl
    {
        private readonly List<GraphicSymbolTemplate> _templates;
        private readonly CanvasSerialization _canvasSerialization;

        public string SceneName { get; set; }
        public string SceneDescription { get; set; }
        public bool InsertScaleSpecified { get; set; }

        public delegate void SceneSettingsOkClickDelegate(SceneSettings sender);
        public event SceneSettingsOkClickDelegate SceneSettingsOkClick;

        public delegate void SceneSettingsCloseClickDelegate(SceneSettings sender);
        public event SceneSettingsCloseClickDelegate SceneSettingsCloseClick;

        public SceneSettings(List<GraphicSymbolTemplate> templates, CanvasSerialization canvasSerialization)
        {
            InitializeComponent();
            _templates = templates;
            _canvasSerialization = canvasSerialization;
            InsertScaleSpecified = false;
            GraphicsScene.LocalizationHelper.LanguageChanged += LocalizationHelper_LanguageChanged;
        }

        public void DisposeLocalizationHelper()
        {
            GraphicsScene.LocalizationHelper.LanguageChanged -= LocalizationHelper_LanguageChanged;
        }

        void LocalizationHelper_LanguageChanged()
        {
            GraphicsScene.LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this);
        }

        private void _bOk_Click(object sender, RoutedEventArgs e)
        {
            if (!ControlValues())
                return;

            SceneName = _tbSceneName.Text;
            SceneDescription = _tbSceneDescription.Text;

            double lineWidth = 1.0;

            try
            {
                lineWidth = Double.Parse(_tbLineWidth.Text);
            }
            catch (Exception)
            {
                return;
            }

            _canvasSerialization.CanvasSettings.defaultLineWidth = lineWidth;
            _canvasSerialization.CanvasSettings.defaultLineColor = new SerializableColor(_cpLineColor.SelectedColor);
            _canvasSerialization.CanvasSettings.defaultBackgroundColor = new SerializableColor(_cpBackgroundColor.SelectedColor);
            _canvasSerialization.CanvasSettings.DefaultSymbolSize = ((GraphicsScene.SymbolSizeMode)_cbSymbolSize.SelectedItem).Value;
            _canvasSerialization.CanvasSettings.UseTemplateId =
                (_cbSymbolTemplate.SelectedItem as GraphicSymbolTemplate).Id;

            int implicityScale = 50;

            if (_tbImplicityScale.Value != null)
                implicityScale = (int)_tbImplicityScale.Value;

            _canvasSerialization.CanvasSettings.ImplicityScaleOfInsertedSymbols = implicityScale;

            if (InsertScaleSpecified)
            {
                try
                {
                    _canvasSerialization.CanvasSettings.ModelLength = Double.Parse(_tbSpecifiedScaleLength.Text);
                }
                catch
                {
                    _canvasSerialization.CanvasSettings.ModelLength = 0;
                }
            }

            SceneSettingsOkClick(this);
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
            Window_Loaded(this, null);
        }

        private void _bCancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
            SceneSettingsCloseClick(this);
            DisposeLocalizationHelper();
        }

        private bool ControlValues()
        {
            if (string.IsNullOrEmpty(_tbSceneName.Text))
            {
                Contal.IwQuick.UI.Dialog.Error(GraphicsScene.LocalizationHelper.GetString("WarningInsertSceneName"));
                return false;
            }

            if (InsertScaleSpecified)
            {
                if (string.IsNullOrEmpty(_tbSpecifiedScaleLength.Text) || !IsNumber(_tbSpecifiedScaleLength.Text))
                {
                    MessageBox.Show("The model height must be number");
                    return false;
                }
            }

            return true;
        }

        private bool IsNumber(string number)
        {
            try
            {
                double x = double.Parse(number);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _tbSceneName.Text = SceneName;
            _tbSceneDescription.Text = SceneDescription;
            _tbLineWidth.Text = _canvasSerialization.CanvasSettings.defaultLineWidth.ToString();
            _cpLineColor.SelectedColor = _canvasSerialization.CanvasSettings.defaultLineColor.GetColor();
            _cpBackgroundColor.SelectedColor = _canvasSerialization.CanvasSettings.defaultBackgroundColor.GetColor();
            _tbImplicityScale.Value = _canvasSerialization.CanvasSettings.ImplicityScaleOfInsertedSymbols;
            _cbSymbolSize.Items.Clear();
            _cbSymbolTemplate.Items.Clear();

            //load all symbol type
            foreach (SymbolSize symbolSize in Enum.GetValues(typeof (SymbolSize)))
            {
                var item = new GraphicsScene.SymbolSizeMode(symbolSize);
                _cbSymbolSize.Items.Add(item);

                if (symbolSize == _canvasSerialization.CanvasSettings.DefaultSymbolSize)
                    _cbSymbolSize.SelectedItem = item;
                else
                    _cbSymbolSize.SelectedIndex = 0;
            }

            //load all templates
            foreach (GraphicSymbolTemplate template in _templates)
            {
                _cbSymbolTemplate.Items.Add(template);

                if (template.Id == _canvasSerialization.CanvasSettings.UseTemplateId)
                    _cbSymbolTemplate.SelectedItem = template;
                else
                    _cbSymbolTemplate.SelectedIndex = 0;
            }

            if (InsertScaleSpecified)
            {
                _tbSpecifiedScaleLength.IsEnabled = true;
                _tbSpecifiedScaleLength.Text = _canvasSerialization.CanvasSettings.ModelLength.ToString("F2");
            }
            else
            {
                _tbSpecifiedScaleLength.IsEnabled = false;
            }

            GraphicsScene.LocalizationHelper.TranslateWpfControl("NCASSceneEditForm", this);
        }
    }
}
