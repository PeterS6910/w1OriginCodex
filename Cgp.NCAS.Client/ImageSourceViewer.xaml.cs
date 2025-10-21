using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Contal.Cgp.NCAS.Client
{
    /// <summary>
    /// Interaction logic for ImageSourceViewer.xaml
    /// </summary>
    public partial class ImageSourceViewer : UserControl
    {
        private List<ImageItem> _imageSources;

        public ImageItem SelectImageSource { get; private set; }

        public ImageSourceViewer()
        {
            InitializeComponent();
        }

        public List<ImageItem> ImageSources
        {
            get { return _imageSources; }
            set
            {
                _imageSources = value;

                if (_imageSources != null)
                {
                    _lvImageSources.ItemsSource = null;
                    _lvImageSources.ItemsSource = _imageSources;
                }
            }
        }

        private void _lvImageSources_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectImageSource = (ImageItem)((sender as ListView).SelectedItem);
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }
    }
}
