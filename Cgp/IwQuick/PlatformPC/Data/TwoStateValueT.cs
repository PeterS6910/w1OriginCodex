using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.Data
{
    public class TwoStateValue<T>
    {
        private T _defaultValue;
        private bool _defaultSet = false;

        private T _currentValue;
        private bool _currentSet = false;

        public void SetDefault(T value)
        {
            _defaultValue = value;
            _defaultSet = true;
        }

        public void UnsetDefault()
        {
            _defaultSet = false;
        }

        public void SetCurrent(T value)
        {
            _currentValue = value;
            _currentSet = true;
        }

        public void UnsetCurrent()
        {
            _currentSet = false;
        }

        public TwoStateValue(T defaultValue,T currentValue)
        {
            SetDefault(defaultValue);
            SetCurrent(currentValue);
        }

        public TwoStateValue(T defaultValue)
        {
            SetDefault(defaultValue);
        }

        public TwoStateValue()
        {
        }

        public T Get()
        {
            if (_currentSet)
                return _currentValue;

            if (_defaultSet)
                return _defaultValue;

            return default(T);
        }

        public T GetDefault()
        {
            if (_defaultSet)
                return _defaultValue;
            else
                return default(T);
        }

        public T GetCurrent()
        {
            if (_currentSet)
                return _currentValue;
            else
                return default(T);
        }

        public bool TryGet(out T o_aValue)
        {
            o_aValue = default(T);
            if (!_currentSet && !_defaultSet)
                return false;

            if (_currentSet)
                o_aValue = _currentValue;

            if (_defaultSet)
                o_aValue = _defaultValue;

            return true;
        }

        public void ResetDefault()
        {
            UnsetCurrent();
        }

        public void Unset()
        {
            UnsetCurrent();
            UnsetDefault();
        }

    }
}
