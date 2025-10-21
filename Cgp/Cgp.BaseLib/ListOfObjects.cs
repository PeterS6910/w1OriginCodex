using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace Contal.Cgp.BaseLib
{
    public class ListOfObjects : IEnumerable, IEnumerator
    {
        public List<object> Objects = new List<object>();
        private int _currentPosition = -1;
        public int Count
        {
            get
            {
                return Objects.Count;
            }
        }
        public ListOfObjects() { }

        public ListOfObjects(List<object> objects)
        {
            Objects = objects;
        }

        public static string ToString<TElement>(ICollection<TElement> collection)
        {
            return 
                collection.Count == 1
                    ? collection.First().ToString()
                    : string.Format(
                        "Obj.:{0}",
                        collection.Count);
        }

        public override string ToString()
        {
            return ToString(Objects);
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            _currentPosition = -1;
            return this;
        }

        #endregion       

        public object this[int index]
        {
            get
            {
                return Objects[index];
            }
            set
            {
                Objects[index] = value;
            }
        }

        #region IEnumerator Members

        object IEnumerator.Current
        {
            get
            {
                return Objects[_currentPosition];
            }
        }

        bool IEnumerator.MoveNext()
        {            
            _currentPosition++;
            return (_currentPosition < Objects.Count);
        }

        void IEnumerator.Reset()
        {
            _currentPosition = 0;
        }

        #endregion
    }
}
