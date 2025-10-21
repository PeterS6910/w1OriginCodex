using System;
using System.Collections;

namespace Contal.IwQuick.Data
{
    public class StringKeyArray
    {

        class ValueElement
        {
            public Object _key = null;
            public Object _value = null;
            public ValueElement _previous = null;
            public ValueElement _next = null;
        }

        private Hashtable _keyedData = null;
        private Boolean _autoDefine = true;

        private ValueElement _historyRoot = null;
        private ValueElement _historyLast = null;

        private Boolean _historyEnumInit = true;
        private ValueElement _historyActual = null;

        private IDictionaryEnumerator m_rValueEnum = null;

        public StringKeyArray(int i_iStartCount, Single i_fLoadFactor)
        {
            _keyedData = new Hashtable(i_iStartCount, i_fLoadFactor);
            m_rValueEnum = _keyedData.GetEnumerator();
        }

        public StringKeyArray()
            : this(16, (Single)Math.Log(2)) // ln 2 = cca 0.693
        {
        }

        public StringKeyArray(int i_iStartCount)
            : this(i_iStartCount, (Single)Math.Log(2))
        {

        }


        public void SetAutoDefinition(Boolean i_bSwitchedOn)
        {
            _autoDefine = i_bSwitchedOn;
        }

        private Boolean AddHistory(ValueElement element)
        {
            if (element == null)
                return false;

            if (_historyRoot == null)
                _historyRoot = element;
            else
                if (_historyLast != null)
                {
                    _historyLast._next = element;
                    element._previous = _historyLast;
                }

            _historyLast = element;
            return true;
        }

        private Boolean RemoveHistory(ValueElement element)
        {
            if (element == null)
                return false;

            if (element._previous != null)
                element._previous._next = element._next;

            if (element._next != null)
                element._next._previous = element._previous;

            if (Object.ReferenceEquals(element, _historyLast))
                _historyLast = element._previous;

            if (Object.ReferenceEquals(element, _historyRoot))
                _historyRoot = element._next;

            if (Object.ReferenceEquals(element, _historyActual))
                _historyActual = element._next;

            return true;
        }

        public Boolean Add(String key, Object value)
        {
            if (key == null || key.Length == 0)
                return false;

            ValueElement aPom = new ValueElement();
            if (aPom == null)
                return false;

            aPom._key = key;
            aPom._value = value;

            try
            {
                _keyedData.Add(aPom._key, aPom);
            }
            catch (ArgumentException)
            {
                return false;
            }

            AddHistory(aPom);
            return true;
        }

        public Boolean Remove(String key)
        {
            if (key == null || key.Length == 0)
                return false;

            ValueElement aPom = (ValueElement)_keyedData[key];
            if (aPom == null)
                return false;


            try
            {
                _keyedData.Remove(aPom._key);
                RemoveHistory(aPom);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public Object Get(String key)
        {
            if (key == null || key.Length == 0)
                return null;

            ValueElement aPom = (ValueElement)_keyedData[key];
            if (aPom == null)
                return null;

            return aPom._value;
        }

        public Boolean Set(String key, Object value)
        {
            if (key == null || key.Length == 0)
                return false;

            ValueElement aPom = (ValueElement)_keyedData[key];

            if (aPom == null)
            {
                if (!_autoDefine)
                    return false;

                Add(key, value);
            }
            else
                aPom._value = value;

            return true;

        }

        public Object this[String key]
        {
            get
            {
                return Get(key);
            }

            set
            {
                Set(key, value);
            }
        }

        public void ResetEnumHistory()
        {
            _historyEnumInit = true;
        }

        public void ResetEnum()
        {
            m_rValueEnum = _keyedData.GetEnumerator();
        }

        public Boolean GetNext(out Object o_aValue)
        {
            o_aValue = null;

            if (!m_rValueEnum.MoveNext())
                return false;

            ValueElement aPom = (ValueElement)m_rValueEnum.Value;
            if (aPom == null)
                return false;

            o_aValue = aPom._value;
            return true;
        }

        public Boolean GetNextHistory(out Object o_aValue)
        {
            o_aValue = null;

            if (_historyEnumInit)
            {
                _historyEnumInit = false;
                _historyActual = _historyRoot;
            }
            else
            {
                _historyActual = _historyActual._next;
            }

            if (_historyActual == null)
                return false;
            else
            {
                o_aValue = _historyActual._value;
                return true;
            }
        }

        public int Count
        {
            get
            {
                return _keyedData.Count;
            }
        }

        public int GetCount()
        {
            return _keyedData.Count;
        }


    }
}
