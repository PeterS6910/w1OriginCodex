using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Contal.IwQuick.UI
{
    public class ComponentList : ComponentList<Control>
    {
        public ComponentList()
            : base()
        {

        }
    }

    public partial class ComponentList<T> : UserControl where T : Control
    {
        private readonly ComponentListCollection<T> _items;
        private readonly RowStyle _lastItemStyle = new RowStyle(SizeType.AutoSize);
        private RowStyle _defaultRowStyle = new RowStyle(SizeType.Absolute, 40F);
        private readonly int _vertScrollWidth = SystemInformation.VerticalScrollBarWidth;
        private bool _visibleScrollbar;

        public ComponentList()
        {
            InitializeComponent();
            _tlPanel.RowStyles.Clear();
            _items = new ComponentList<T>.ComponentListCollection<T>(this);
            _tlPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            _tlPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _tlPanel.AutoScroll = true;
            _items.OnItemAdded += AddOccured;
            _items.OnItemRemoved += RemoveOccured;
            _items.OnItemsCleared += ClearOccured;
            _visibleScrollbar = _tlPanel.VerticalScroll.Visible;
            _tlPanel.Padding = new Padding(0, 0, 1, 0);
        }

        private void AddOccured(T item)
        {
            if (!_tlPanel.Controls.Contains(item))
            {
                _tlPanel.RowCount = _items.Count + 1;

                item.Dock = DockStyle.Fill;
                try
                {
                    var autoSize = typeof(T).GetProperty("AutoSize");
                    if (autoSize != null)
                        autoSize.SetValue(item, AutoSizeMode.GrowAndShrink, null);
                }
                catch (System.Exception ex)
                {
                    Console.WriteLine(ex);
                }

                int count = _tlPanel.RowStyles.Count;
                if (count > 0)
                {
                    _tlPanel.RowStyles.RemoveAt(count - 1);
                }

                _tlPanel.RowStyles.Add(new RowStyle(_defaultRowStyle.SizeType, _defaultRowStyle.Height));
                _tlPanel.RowStyles.Add(_lastItemStyle);
                _tlPanel.Controls.Add(item, 0, _items.IndexOf(item));
            }

            if (_visibleScrollbar != _tlPanel.VerticalScroll.Visible)
            {
                // Hide horizontal scrollbar
                _tlPanel.Padding = new Padding(0, 0, _vertScrollWidth, 0);
                _tlPanel.Padding = new Padding(0, 0, 1, 0);
                
                _visibleScrollbar = _tlPanel.VerticalScroll.Visible;
            }
            
        }

        private void RemoveOccured(T item)
        {
            if (_tlPanel.Controls.Contains(item))
            {
                int index = _tlPanel.Controls.IndexOf(item);
                
                for (int i = index + 1; i < _items.Count + 1; i++)
                {
                    _tlPanel.SetRow(_tlPanel.Controls[i], i - 1);
                }
                _tlPanel.Controls.Remove(item);
                //_tlPanel.RowStyles.RemoveAt(index);
                _tlPanel.RowCount = _items.Count + 1;
                
                
                
                if (!item.IsDisposed)
                    item.Dispose();
            }
        }

        private void ClearOccured(T[] items)
        {
            _tlPanel.RowCount = 1;
            _tlPanel.Controls.Clear();
            _tlPanel.RowStyles.Clear();
            _tlPanel.RowStyles.Add(_lastItemStyle);

            if (_items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    if(!items[i].IsDisposed)
                        items[i].Dispose();
                }
            }
        }

        public RowStyle DefaultRowStyle
        {
            get { return _defaultRowStyle; }
            set { _defaultRowStyle = value; }
        }

        [DefaultValue(TableLayoutPanelCellBorderStyle.Single)]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            get { return _tlPanel.CellBorderStyle; }
            set { _tlPanel.CellBorderStyle = value; }
        }

        public ComponentListCollection<T> Items
        {
            get { return _items; }
        }

        public class ComponentListCollection<T_ITEM> : IList<T_ITEM>, ICollection<T_ITEM>, IEnumerable<T_ITEM> where T_ITEM : Control
        {
            private const string ITEM_EXIST = "Item already exist.";

            public event Action<T_ITEM> OnItemRemoved;
            public event Action<T_ITEM> OnItemAdded;
            public event Action<T_ITEM[]> OnItemsCleared;

            List<T_ITEM> _list;
            Object _lock = new Object();

            internal ComponentListCollection(ComponentList<T_ITEM> parent)
            {
                _list = new List<T_ITEM>();
            }

            #region ICollection<T> Members

            public void Add(T_ITEM item)
            {
                lock (_lock)
                {
                    if (!Contains(item))
                    {
                        _list.Add(item);
                        if (OnItemAdded != null)
                            OnItemAdded(item);
                    }
                }
            }

            public void Clear()
            {
                lock(_lock)
                {
                    if (OnItemsCleared != null)
                        OnItemsCleared(_list.ToArray());
                    _list.Clear();
                }
            }

            public bool Contains(T_ITEM item)
            {
                lock (_lock)
                {
                    bool contains = false;
                    foreach (var i in _list)
                    {
                        if (item.Equals(i))
                        {
                            contains = true;
                            break;
                        }
                    }
                    return contains;
                }
            }

            public void CopyTo(T_ITEM[] array, int arrayIndex)
            {
                lock (_lock)
                {
                    _list.CopyTo(array, arrayIndex);
                }
            }

            public int Count
            {
                get { lock (_lock) { return _list.Count; } }
            }

            public bool IsReadOnly
            {
                get { lock (_lock) { return false; } }
            }

            public bool Remove(T_ITEM item)
            {
                lock (_lock)
                {
                    bool success = _list.Remove(item);
                    if(success && OnItemRemoved != null)
                        OnItemRemoved(item);
                    return success;
                }
            }

            #endregion

            #region IList<T> Members

            public int IndexOf(T_ITEM item)
            {
                lock (_lock)
                {
                    return _list.IndexOf(item);
                }
            }

            public void Insert(int index, T_ITEM item)
            {
                lock (_lock)
                {
                    if (Contains(item))
                        throw new ArgumentException(ITEM_EXIST);

                    _list.Insert(index, item);

                    if (OnItemAdded != null)
                        OnItemAdded(item);
                }
            }

            public void RemoveAt(int index)
            {
                lock (_lock)
                {
                    var item = _list[index];
                    _list.RemoveAt(index);
                    if (OnItemRemoved != null)
                        OnItemRemoved(item);
                }
            }

            public T_ITEM this[int index]
            {
                get
                {
                    lock (_lock)
                    {
                        return _list[index];
                    }
                }
                set
                {
                    lock (_lock)
                    {
                        if (!Contains(value))
                        {
                            _list[index] = value;
                        }
                        else
                            throw new ArgumentException(ITEM_EXIST);
                    }
                }
            }

            #endregion

            #region IEnumerable<T_ITEM> Members

            IEnumerator<T_ITEM> IEnumerable<T_ITEM>.GetEnumerator()
            {
                lock (_lock)
                {
                    T_ITEM[] temp = new T_ITEM[_list.Count];
                    _list.CopyTo(temp);
                    return (IEnumerator <T_ITEM>) temp.GetEnumerator();
                }
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                lock (_lock)
                {
                    T_ITEM[] temp = new T_ITEM[_list.Count];
                    _list.CopyTo(temp);
                    return temp.GetEnumerator();
                }
            }

            #endregion

            public void ForEach(Action<T_ITEM> action)
            {
                lock (_lock)
                {
                    foreach (T_ITEM item in _list)
                        action(item);
                }
            }
        }
    }
}
