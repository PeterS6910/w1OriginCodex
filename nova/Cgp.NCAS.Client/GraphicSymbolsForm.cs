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
    public partial class GraphicSymbolsForm : Form
    {
        private List<GraphicSymbolItem> _symbols = new List<GraphicSymbolItem>();
        private Dictionary<Guid, string> _templates = new Dictionary<Guid, string>();
        private ICgpNCASRemotingProvider _mainServerProvider;


        public GraphicSymbolsForm(ICgpNCASRemotingProvider mainServerProvider)
        {
            InitializeComponent();
            _mainServerProvider = mainServerProvider;
        }

        private void GraphicSymbolsForm_Load(object sender, EventArgs e)
        {
            LoadTemplates();
            LoadSymbols();
        }

        private void LoadTemplates()
        {
            if (_mainServerProvider == null)
                return;

            _templates.Clear();
            Exception ex = null;

            List<GraphicSymbolTemplate> templates = _mainServerProvider.GraphicSymbolTemplates.List(out ex).ToList();

            if (ex != null)
                throw ex;

            foreach (GraphicSymbolTemplate template in templates)
                _templates.Add(template.Id, template.Name);

            templates.Clear();
        }

        private void LoadSymbols()
        {
            try
            {
                _symbols.Clear();
                Exception ex = null;

                //load graphic symbol 
                ICollection<GraphicSymbol> graphicSymbols = _mainServerProvider.GraphicSymbols.List(out ex);

                if (ex != null)
                    throw ex;

                if (graphicSymbols == null)
                    return;

                //load image sources of symbols
                foreach (GraphicSymbol gs in graphicSymbols)
                {
                    GraphicSymbolItem item = new GraphicSymbolItem();
                    item.Id = gs.Id;
                    item.SymbolState = gs.SymbolState;
                    item.SymbolType = gs.SymbolType;
                    item.FilterKey = gs.FilterKey;
                    item.Template = _templates[gs.IdTemplate];
                    _symbols.Add(item);

                    GraphicSymbolRawData rawData = _mainServerProvider.GraphicSymbolRawDatas.GetObjectById(gs.IdRawData);

                    if (rawData == null)
                        continue;

                    byte[] RawData = rawData.RawData;

                    using (MemoryStream stream = new MemoryStream(RawData))
                    {
                        switch (rawData.DataType)
                        {
                            case SymbolDataType.Vector:
                                SVGRender svgRender = new SVGRender();
                                DrawingGroup dg = svgRender.LoadDrawing(stream);
                                DrawingImage svgSymbol = new DrawingImage(dg);
                                item.Picture = svgSymbol;
                                break;

                            case SymbolDataType.Raster:
                                BitmapImage bmpSymbol = new BitmapImage();
                                bmpSymbol.BeginInit();
                                bmpSymbol.CreateOptions = BitmapCreateOptions.PreservePixelFormat;
                                bmpSymbol.CacheOption = BitmapCacheOption.OnLoad;
                                bmpSymbol.StreamSource = stream;
                                bmpSymbol.EndInit();
                                item.Picture = bmpSymbol;
                                break;
                        }
                    }
                }

                graphicSymbolsViewer1.GraphicSymbols = _symbols;
            }
            catch (Exception ex)
            {
                Close();
            }
        }

        private void _bInsert_Click(object sender, EventArgs e)
        {
            InsertGraphicsSymbolDialogForm.MainServerProvider = _mainServerProvider;
            InsertGraphicsSymbolDialogForm insertSymbolForm = new InsertGraphicsSymbolDialogForm();
            insertSymbolForm.ShowDialog();
        }

        private void _bEdit_Click(object sender, EventArgs e)
        {
            if (graphicSymbolsViewer1.SelectedGraphicSymbol == null)
                return;

            InsertGraphicsSymbolDialogForm.MainServerProvider = _mainServerProvider;
            InsertGraphicsSymbolDialogForm insertSymbolForm = new InsertGraphicsSymbolDialogForm(graphicSymbolsViewer1.SelectedGraphicSymbol.Id);
            
            if (insertSymbolForm.ShowDialog() == DialogResult.OK)
                LoadSymbols();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (graphicSymbolsViewer1.SelectedGraphicSymbol == null)
                return;

            if (MessageBox.Show("Do you want to delete this graphic symbol", "?", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {

                Exception ex = null;

                _mainServerProvider.GraphicSymbols.DeleteById(graphicSymbolsViewer1.SelectedGraphicSymbol.Id, out ex);

                if (ex != null)
                    throw ex;
                
                LoadSymbols();
            }
        }
    }

    public class GraphicSymbolItem
    {
        public Guid Id { get; set; }
        public SymbolType SymbolType { get; set; }
        public State SymbolState { get; set; }
        public string Template { get; set; }
        public string FilterKey { get; set; }
        public ImageSource Picture { get; set; }
    }
}
