using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Contal.Cgp.NCAS.RemotingCommon;

namespace Contal.Cgp.NCAS.Client
{
    public partial class SelectImageSourceForm : Form
    {
        private List<ImageItem> _imageSources;
        private ICgpNCASRemotingProvider _mainServerProvider;

        public ImageItem SelectedImageSource = null;

        public SelectImageSourceForm(ICgpNCASRemotingProvider mainServerProvider, List<ImageItem> imageSources)
        {
            InitializeComponent();
            _mainServerProvider = mainServerProvider;
            _imageSources = imageSources;
        }

        private void SelectImageSourceForm_Load(object sender, EventArgs e)
        {
            imageSourceViewer1.ImageSources = _imageSources;
        }

        private void _bCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void _bSelect_Click(object sender, EventArgs e)
        {
            if (imageSourceViewer1.SelectImageSource == null)
            {
                MessageBox.Show("You must first select a image source.");
                return;
            }

            SelectedImageSource = imageSourceViewer1.SelectImageSource;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void _bDelete_Click(object sender, EventArgs e)
        {
            if (imageSourceViewer1.SelectImageSource == null)
                return;

            if (MessageBox.Show("Do you want to delete this image source?", "?", MessageBoxButtons.YesNo) ==
                DialogResult.Yes)
            {

                Exception ex = null;

                _mainServerProvider.GraphicSymbolRawDatas.DeleteById(imageSourceViewer1.SelectImageSource.Id, out ex);

                if (ex != null)
                    throw ex;

                _imageSources.Remove(imageSourceViewer1.SelectImageSource);
                imageSourceViewer1.ImageSources = _imageSources;
            }
        }
    }
}
