using System;
using System.Windows;
using System.Windows.Controls;

namespace Contal.IwQuick.PlatformPC.UI
{
    /// <summary>
    /// Interaction logic for WpfTreeview.xaml
    /// </summary>
    public abstract partial class WpfTreeview
    {
        public abstract ItemCollection Items { get; }

        protected WpfTreeview()
        {
            InitializeComponent();
        }
    }

    public class WpfCheckBoxesTreeview : WpfTreeview
    {
        private readonly CheckBoxTreeView _checkBoxTreeView;

        public event Action<CheckBoxTreeViewItem> ItemCheckedChanged;

        public WpfCheckBoxesTreeview()
        {
            _checkBoxTreeView = new CheckBoxTreeView
            {
                Margin = new Thickness(0),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            _checkBoxTreeView.ItemCheckedChanged += DoItemCheckedChanged;

            mainGrid.Children.Add(_checkBoxTreeView);

        }

        private void DoItemCheckedChanged(CheckBoxTreeViewItem item)
        {
            if (ItemCheckedChanged != null)
                ItemCheckedChanged(item);
        }

        public override ItemCollection Items
        {
            get { return _checkBoxTreeView.Items; }
        }
    }

    public interface ICheckBoxCheckedChanged
    {
        void ProcessCheckBoxCheckedChanged(CheckBoxTreeViewItem item);
    }

    public class CheckBoxTreeView :
        TreeView,
        ICheckBoxCheckedChanged
    {
        public event Action<CheckBoxTreeViewItem> ItemCheckedChanged;

        public void ProcessCheckBoxCheckedChanged(CheckBoxTreeViewItem item)
        {
            if (ItemCheckedChanged != null)
                ItemCheckedChanged(item);
        }
    }

    public class CheckBoxTreeViewItem :
        TreeViewItem,
        ICheckBoxCheckedChanged
    {
        private bool _isChecked;
        private string _text;

        private readonly CheckBox _checkBox = new CheckBox();
        private readonly TextBlock _textBlock = new TextBlock();

        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                _checkBox.IsChecked = value;
            }
        }
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _textBlock.Text = value;
            }
        }

        public CheckBoxTreeViewItem()
        {
            var stackPanel = new StackPanel { Orientation = Orientation.Horizontal };
            stackPanel.Children.Add(_checkBox);
            stackPanel.Children.Add(_textBlock);
            _checkBox.Margin = new Thickness(2);
            _textBlock.Margin = new Thickness(2);
            Header = stackPanel;

            _checkBox.Checked += DoCheckedChanged;
            _checkBox.Unchecked += DoCheckedChanged;
        }

        void DoCheckedChanged(object sender, RoutedEventArgs e)
        {
            IsSelected = true;
            _isChecked = _checkBox.IsChecked ?? false;

            var iCheckBoxCheckedChanged = Parent as ICheckBoxCheckedChanged;

            if (iCheckBoxCheckedChanged == null)
                return;

            iCheckBoxCheckedChanged.ProcessCheckBoxCheckedChanged(this);
        }

        public void ProcessCheckBoxCheckedChanged(CheckBoxTreeViewItem item)
        {
            var iCheckBoxCheckedChanged = Parent as ICheckBoxCheckedChanged;

            if (iCheckBoxCheckedChanged == null)
                return;

            iCheckBoxCheckedChanged.ProcessCheckBoxCheckedChanged(item);
        }
    }
}
