using System.Collections.Generic;
using System.Windows.Controls;

namespace Contal.Cgp.NCAS.Client
{
    /// <summary>
    /// Interaction logic for GraphicSymbolsViewer.xaml
    /// </summary>
    public partial class GraphicSymbolsViewer : UserControl
    {
        private List<GraphicSymbolItem> _graphicSymbols;

        public GraphicSymbolItem SelectedGraphicSymbol { get; private set; }

        public GraphicSymbolsViewer()
        {
            InitializeComponent();
        }

        public List<GraphicSymbolItem> GraphicSymbols
        {
            get { return _graphicSymbols; }
            set
            {
                _graphicSymbols = value;
                _lvGraphicSymbols.ItemsSource = null;
                _lvGraphicSymbols.ItemsSource = _graphicSymbols;
            }

        }

        private void _lvGraphicSymbols_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedGraphicSymbol = ((ListView)sender).SelectedItem as GraphicSymbolItem;
        }
    }
}
