using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections;
using System.Reflection;

namespace Contal.IwQuick.Data
{
    public class SortableBindingList<K> : BindingList<K>
    {
        private class PropertyComparer : IComparer<K>
        {
            private readonly IComparer _comparer;
            private PropertyDescriptor _propertyDescriptor;
            private int _reverse;

            public PropertyComparer(PropertyDescriptor property, ListSortDirection direction)
            {
                _propertyDescriptor = property;
                if (typeof(IComparer).IsAssignableFrom(property.PropertyType))
                    _comparer = Activator.CreateInstance(property.PropertyType, false) as IComparer;
                else
                    _comparer = Comparer.Default;

                this.SetListSortDirection(direction);
            }

            #region IComparer<K> Members

            public int Compare(K x, K y)
            {
                try
                {
                    return this._reverse * this._comparer.Compare(this._propertyDescriptor.GetValue(x), this._propertyDescriptor.GetValue(y));
                }
                catch
                {
                    return 0;
                }
            }

            #endregion

            private void SetPropertyDescriptor(PropertyDescriptor descriptor)
            {
                _propertyDescriptor = descriptor;
            }

            private void SetListSortDirection(ListSortDirection direction)
            {
                _reverse = direction == ListSortDirection.Ascending ? 1 : -1;
            }

            public void SetPropertyAndDirection(PropertyDescriptor descriptor, ListSortDirection direction)
            {
                SetPropertyDescriptor(descriptor);
                SetListSortDirection(direction);
            }
        }

        private readonly Dictionary<Type, PropertyComparer> _comparers;
        private bool _isSorted;
        private ListSortDirection _listSortDirection;
        private PropertyDescriptor _propertyDescriptor;

        public SortableBindingList()
            : base(new List<K>())
        {
            _comparers = new Dictionary<Type, PropertyComparer>();
        }

        public SortableBindingList(IEnumerable<K> enumeration)
            : base(new List<K>(enumeration))
        {
            _comparers = new Dictionary<Type, PropertyComparer>();
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _propertyDescriptor; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return _listSortDirection; }
        }

        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }

        protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
        {
            List<K> itemsList = (List<K>)Items;

            Type propertyType = property.PropertyType;
            PropertyComparer comparer;
            if (!_comparers.TryGetValue(propertyType, out comparer))
            {
                comparer = new PropertyComparer(property, direction);
                _comparers.Add(propertyType, comparer);
            }            

            comparer.SetPropertyAndDirection(property, direction);
            itemsList.Sort(comparer);

            _propertyDescriptor = property;
            _listSortDirection = direction;
            _isSorted = true;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override void RemoveSortCore()
        {
            _isSorted = false;
            _propertyDescriptor = base.SortPropertyCore;
            _listSortDirection = base.SortDirectionCore;

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        protected override int FindCore(PropertyDescriptor property, object key)
        {
            int count = this.Count;
            for (int i = 0; i < count; ++i)
            {
                K element = this[i];
                if (property.GetValue(element).Equals(key))
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
