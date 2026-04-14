using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ClipArtViewer;

using Contal.Cgp.Globals;
using Contal.Cgp.NCAS.RemotingCommon;
using Contal.Cgp.NCAS.Server.Beans;

namespace Contal.Cgp.NCAS.Client
{
    public partial class InsertGraphicsSymbolDialogForm : Form
    {
        private List<GraphicSymbolTemplate> _templates;
        private List<ImageItem> _images = new List<ImageItem>();
        private ImageItem _selectedImageSource;
        private GraphicSymbol _graphicSymbol;

        public static ICgpNCASRemotingProvider MainServerProvider { get; set; }

        public InsertGraphicsSymbolDialogForm()
        {
            InitializeComponent();
            LoadSymbolTypeItems();
            LoadSymbolStateItems();
            LoadTemplates();
        }

        public InsertGraphicsSymbolDialogForm(Guid IdGS)
        {
            InitializeComponent();
            GetGraphicSymbolById(IdGS);
            LoadSymbolTypeItems();
            LoadSymbolStateItems();
            LoadTemplates();
        }

        private void GetGraphicSymbolById(Guid Id)
        {
            Exception ex = null;
            bool b;

            _graphicSymbol = MainServerProvider.GraphicSymbols.GetObjectForEditById(Id, out b);

            if (ex != null)
                throw ex;
        }

        private void LoadSymbolTypeItems()
        {
            foreach (SymbolType value in Enum.GetValues(typeof(SymbolType)))
            {
                SymbolTypeItem item = new SymbolTypeItem(value.ToString(), value);
                _cbSymbolType.Items.Add(item);

                if (_graphicSymbol != null && _graphicSymbol.SymbolType == value)
                    _cbSymbolType.SelectedItem = item;
            }
            
            if (_graphicSymbol == null)
                _cbSymbolType.SelectedIndex = 0;
        }

        private void LoadSymbolStateItems()
        {
            foreach (State value in Enum.GetValues(typeof(State)))
            {
                SymbolStateItem item = new SymbolStateItem(value.ToString(), value);
                _cbSymbolState.Items.Add(item);

                if (_graphicSymbol != null && _graphicSymbol.SymbolState == value)
                    _cbSymbolState.SelectedItem = item;
            }

            if (_graphicSymbol == null)
                _cbSymbolState.SelectedIndex = 0;
        }

        private void LoadTemplates()
        {
            Exception ex = null;

            _templates = MainServerProvider.GraphicSymbolTemplates.List(out ex).ToList();

            if (ex != null)
                throw ex;

            _cbSelectTemplate.DataSource = _templates;

            foreach (GraphicSymbolTemplate template in _templates)
                if (_graphicSymbol != null && template.Id == _graphicSymbol.IdTemplate)
                {
                    _cbSelectTemplate.SelectedItem = template;
                    break;
                }
        }

        private void LoadImageSources()
        {
            Exception ex = null;

            ICollection<GraphicSymbolRawData> rawDatas = MainServerProvider.GraphicSymbolRawDatas.List(out ex);

            if (ex != null)
                throw ex;

            _images.Clear();

            foreach (GraphicSymbolRawData rawData in rawDatas)
            {
                ImageItem imageItem = new ImageItem();
                imageItem.Id = rawData.Id;

                using (MemoryStream stream = new MemoryStream(rawData.RawData))
                {
                    switch (rawData.DataType)
                    {
                        case SymbolDataType.Vector:
                            SVGRender svgRender = new SVGRender();
                            DrawingGroup dg = svgRender.LoadDrawing(stream);
                            DrawingImage svgSymbol = new DrawingImage(dg);
                            imageItem.Source = svgSymbol;
                            break;

                        case SymbolDataType.Raster:
                            BitmapImage bmpSymbol = new BitmapImage();
                            bmpSymbol.BeginInit();
                            bmpSymbol.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                            bmpSymbol.CacheOption = BitmapCacheOption.OnLoad;
                            bmpSymbol.StreamSource = stream;
                            bmpSymbol.EndInit();
                            imageItem.Source = bmpSymbol;
                            break;
                    }
                }

                _images.Add(imageItem);
            }
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(_tbFileName.Text) || _selectedImageSource != null || _graphicSymbol.IdRawData != null)
            {
                try
                {
                    Exception ex = null;
                    GraphicSymbol gs = null;

                    if (_graphicSymbol == null)
                        gs = new GraphicSymbol();
                    else
                        gs = _graphicSymbol;

                    if (_selectedImageSource == null && (_graphicSymbol == null || _graphicSymbol.IdRawData == null || !string.IsNullOrEmpty(_tbFileName.Text)))
                    {
                        GraphicSymbolRawData newRawData = new GraphicSymbolRawData();
                        string extension = Path.GetExtension(openFileDialog.FileName);

                        if (extension == ".svg")
                            newRawData.DataType = SymbolDataType.Vector;
                        else if (extension == ".bmp" || extension == ".png" || extension == ".jpg" || extension == ".gif")
                            newRawData.DataType = SymbolDataType.Raster;
                        else
                            return;

                        byte[] data = File.ReadAllBytes(openFileDialog.FileName);
                        newRawData.RawData = data;
                        MainServerProvider.GraphicSymbolRawDatas.Insert(ref newRawData, out ex);

                        if (ex != null)
                            throw ex;

                        gs.IdRawData = newRawData.Id;
                    }
                    else if (_selectedImageSource != null)
                        gs.IdRawData = _selectedImageSource.Id;

                    gs.SymbolType = (_cbSymbolType.SelectedItem as SymbolTypeItem).GraphicsSymbolType;
                    gs.SymbolState = (_cbSymbolState.SelectedItem as SymbolStateItem).GraphicsSymbolState;
                    gs.IdTemplate = (_cbSelectTemplate.SelectedItem as GraphicSymbolTemplate).Id;

                    if (_chbUseFilterKey.Checked)
                        gs.FilterKey = _cbFilterKeys.SelectedItem.ToString();
                    else
                        gs.FilterKey = string.Empty;

                    if (_graphicSymbol == null)
                        MainServerProvider.GraphicSymbols.Insert(ref gs, out ex);
                    else
                        MainServerProvider.GraphicSymbols.Update(gs, out ex);

                    if (ex != null)
                        throw ex;
                    MessageBox.Show("The Graphics symbol has been saved.");

                    DialogResult = DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
            else
            {
                MessageBox.Show("You must select a file.");
            }
        }

        private void _bSelectFile_Click(object sender, EventArgs e)
        {
            DialogResult dr = openFileDialog.ShowDialog();

            if (dr == DialogResult.OK)
            {
                _tbFileName.Text = openFileDialog.FileName;
            }
        }

        private void _bClear_Click(object sender, EventArgs e)
        {
            _tbTemplateName.Text = string.Empty;
            _tbTemplateDescription.Text = string.Empty;
        }

        private void InsertGraphicsSymbolDialogForm_Load(object sender, EventArgs e)
        {
            //load object type and fill combobox
            foreach (ObjectType value in Enum.GetValues(typeof(ObjectType)))
            {
                _cbFilterKeys.Items.Add(value.ToString());
            }

            _cbFilterKeys.SelectedIndex = 0;

            if (_graphicSymbol == null || string.IsNullOrEmpty(_graphicSymbol.FilterKey))
                _chbUseFilterKey.Checked = false;
            else
            {
                _chbUseFilterKey.Checked = true;

                foreach (string type in _cbFilterKeys.Items)
                {
                    if (type == _graphicSymbol.FilterKey)
                    {
                        _cbFilterKeys.SelectedItem = type;
                        break;
                    }
                }
            }

            LoadImageSources();
        }

        private void _bInsertTemplate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_tbTemplateName.Text))
            {
                MessageBox.Show("You must insert template name.");
                return;
            }

            Exception ex = null;

            GraphicSymbolTemplate newTemplate = new GraphicSymbolTemplate();
            newTemplate.Name = _tbTemplateName.Text;
            newTemplate.Description = _tbTemplateDescription.Text;

            MainServerProvider.GraphicSymbolTemplates.Insert(ref newTemplate, out ex);

            if (ex != null)
                throw ex;
            LoadTemplates();
            MessageBox.Show("The template was inserted.");
        }

        private void _chbUseFilterKey_CheckedChanged(object sender, EventArgs e)
        {
            _cbFilterKeys.Enabled = (sender as CheckBox).Checked;
        }

        private void _bSelectExistingImage_Click(object sender, EventArgs e)
        {
            SelectImageSourceForm imageSourceDialog = new SelectImageSourceForm(MainServerProvider, _images);

            if (imageSourceDialog.ShowDialog() == DialogResult.OK)
            {
                _tbFileName.Text = string.Empty;
                _selectedImageSource = imageSourceDialog.SelectedImageSource;
            }
        }
    }

    public class SymbolTypeItem
    {
        public string Text { get; set; }
        public SymbolType GraphicsSymbolType { get; set; }

        public SymbolTypeItem(string text, SymbolType symbolType)
        {
            Text = text;
            GraphicsSymbolType = symbolType;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class ImageItem
    {
        public Guid Id { get; set; }
        public ImageSource Source { get; set; }
    }

    public class SymbolStateItem
    {
        public string Text { get; set; }
        public State GraphicsSymbolState { get; set; }

        public SymbolStateItem(string text, State symbolState)
        {
            Text = text;
            GraphicsSymbolState = symbolState;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
