using System.Windows;
using System.Windows.Controls;
using Contal.Cgp.NCAS.Server.Beans;

namespace Cgp.NCAS.WpfGraphicsControl
{
    /// <summary>
    /// Interaction logic for GraphicsViewSettings.xaml
    /// </summary>
    public partial class GraphicsViewSettings : UserControl
    {
        private readonly GraphicsView _graphicsView;

        public delegate void DOkClick();
        public event DOkClick OkClickEvent;

        public GraphicsViewSettings(GraphicsView graphicsView)
        {
            InitializeComponent();

            if (graphicsView != null)
            {
                _graphicsView = graphicsView;
                _tbName.Text = graphicsView.Name;
                _tbDescription.Text = graphicsView.Description;
            }
        }

        public void Show()
        {
            Visibility = Visibility.Visible;

            if (_graphicsView != null)
            {
                _tbName.Text = _graphicsView.Name;
                _tbDescription.Text = _graphicsView.Description;
            }
        }

        private void _bOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_tbName.Text))
            {
                Contal.IwQuick.UI.Dialog.Error("Insert name.");
                return;
            }

            _graphicsView.Name = _tbName.Text;
            _graphicsView.Description = _tbDescription.Text;

            if (OkClickEvent != null)
                OkClickEvent();

            Visibility = Visibility.Collapsed;
        }

        private void _bCancel_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
    }
}
