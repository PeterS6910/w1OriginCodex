using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Contal.IwQuick.Localization;

namespace Contal.Cgp.Components
{
    /// <summary>
    /// Interaction logic for WpfUpgradeFilesViewer.xaml
    /// </summary>
    public partial class WpfUpgradeFilesViewer : UserControl
    {
        private List<object> _files;
        private Button _lastSelectedDeleteButton;
        private Button _lastSelectedUpgradeButton;
        private Button _lastSelectedFavoriteButton;
        private Button _favoriteButton;
        private bool _enableUpgrade = true;
        private bool _enableFavorite;
        private object _lastFavoriteValue;

        public List<object> Files
        {
            set
            {
                _lastSelectedDeleteButton = null;
                _files = value;
                _lwObjects.ItemsSource = _files;

                if (_lwObjects.Items.Count > 0)
                    _lwObjects.SelectedIndex = 0;
            }
            get { return _files; }
        }

        public bool Enable
        {
            set
            {
                IsEnabled = value;
                Opacity = value ? 1.0 : 0.5;
            }
        }

        public bool EnableUpgrade 
        {
            get { return _enableUpgrade; }
            set
            {
                _enableUpgrade = value;
                _lwObjects.UpdateLayout();
            }
        }

        public bool EnableFavorite
        {
            get { return _enableFavorite; }
            set
            {
                _enableFavorite = value;
                _lwObjects.UpdateLayout();
            }
        }

        public bool EnableInsertFile
        {
            set
            {
                _bInsert.IsEnabled = value;
                _bInsert.Opacity = value ? 1.0 : 0.5;
            }
        }

        public LocalizationHelper LocalizationHelper { get; set; }

        public Action<object> DeleteAction;
        public Action InsertAction;
        public Action<object> UpgradeAction;
        public Action<object> FavoriteAction;

        public WpfUpgradeFilesViewer()
        {
            InitializeComponent();

            _lwObjects.LayoutUpdated += _lwObjects_LayoutUpdated;
        }

        private void SetFavoriteColumnVisibility()
        {
            for (int i = 0; i < _lwObjects.Items.Count; i++)
            {
                var listViewItem = (ListViewItem)(_lwObjects.ItemContainerGenerator.ContainerFromIndex(i));

                if (listViewItem == null)
                    continue;

                var contentPresenter = FindVisualChild<ContentPresenter>(listViewItem);
                var dataTemplate = contentPresenter.ContentTemplate;
                var mainGrid = (Grid)dataTemplate.FindName("_mainGrid", contentPresenter);
                mainGrid.ColumnDefinitions[2].Width = new GridLength(_enableFavorite ? 22 : 0);

                if (_favoriteButton != null
                    && _lwObjects.Items[i].ToString() == _lastFavoriteValue.ToString())
                {
                    _favoriteButton = (Button) dataTemplate.FindName("_bFavorite", contentPresenter);
                    _favoriteButton.Visibility = Visibility.Visible;
                }
            }
        }

        void _lwObjects_LayoutUpdated(object sender, EventArgs e)
        {
            SetFavoriteColumnVisibility();

            if (_lwObjects.SelectedIndex == -1)
                return;

            if (_lastSelectedDeleteButton != null)
                _lastSelectedDeleteButton.Visibility = Visibility.Hidden;

            if (_lastSelectedUpgradeButton != null)
                _lastSelectedUpgradeButton.Visibility = Visibility.Hidden;

            if (_lastSelectedFavoriteButton != null
                && _lastSelectedFavoriteButton != _favoriteButton)
            {
                _lastSelectedFavoriteButton.Visibility = Visibility.Hidden;
            }

            var listViewItem = (ListViewItem)(_lwObjects.ItemContainerGenerator.ContainerFromIndex(_lwObjects.SelectedIndex));

            if (listViewItem == null)
                return;

            var contentPresenter = FindVisualChild<ContentPresenter>(listViewItem);
            var dataTemplate = contentPresenter.ContentTemplate;

            _lastSelectedDeleteButton = (Button)dataTemplate.FindName("_bDelete", contentPresenter);
            _lastSelectedDeleteButton.Visibility = Visibility.Visible;
            _lastSelectedDeleteButton.ToolTip = LocalizationHelper.GetString("General_bDelete");

            if (EnableUpgrade)
            {
                _lastSelectedUpgradeButton = (Button) dataTemplate.FindName("_bUpgrade", contentPresenter);
                _lastSelectedUpgradeButton.Visibility = Visibility.Visible;
                _lastSelectedUpgradeButton.ToolTip = LocalizationHelper.GetString("General_bUpgrade");
            }

            if (EnableFavorite)
            {
                _lastSelectedFavoriteButton = (Button)dataTemplate.FindName("_bFavorite", contentPresenter);
                _lastSelectedFavoriteButton.Visibility = Visibility.Visible;
                _lastSelectedFavoriteButton.ToolTip = LocalizationHelper.GetString("General_bFavourite");

                var mainGrid = (Grid)dataTemplate.FindName("_mainGrid", contentPresenter);
                mainGrid.ColumnDefinitions[2].Width = new GridLength(22);
            }
            else
            {
                var mainGrid = (Grid)dataTemplate.FindName("_mainGrid", contentPresenter);
                mainGrid.ColumnDefinitions[2].Width = new GridLength(0);
            }
        }

        private void _bInsert_OnClick(object sender, RoutedEventArgs e)
        {
            if (InsertAction != null)
                InsertAction();
        }

        private void _bUpgrade_OnClick(object sender, RoutedEventArgs e)
        {
            if (UpgradeAction != null
                && _lwObjects.SelectedItem != null)
                UpgradeAction(_lwObjects.SelectedItem);
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                
                if (child is childItem)
                    return (childItem) child;

                var childOfChild = FindVisualChild<childItem>(child);
                
                if (childOfChild != null)
                    return childOfChild;
            }

            return null;
        }

        private void _bDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (DeleteAction != null)
                DeleteAction(_lwObjects.SelectedItem);
        }

        private void _bFavorite_OnClick(object sender, RoutedEventArgs e)
        {
            if (_favoriteButton != null)
                _favoriteButton.Visibility = Visibility.Hidden;

            _favoriteButton = (Button) sender;
            _lastFavoriteValue = _lwObjects.SelectedItem;

            if (FavoriteAction != null)
                FavoriteAction(_lwObjects.SelectedItem);
        }
    }
}
