using System;
using System.Collections.Generic;
using System.Text;

namespace Contal.IwQuick.Parsing
{
    public class TokenString
    {
        protected List<string> _strings;
        protected string _separator = String.Empty;
        protected bool _caseSens = false;

        protected void Allocs(int i_iSize)
        {
            if (i_iSize <= 0)
                _strings = new List<string>(8);
            else
                _strings = new List<string>(i_iSize);
        }

        public TokenString()
        {
            Allocs(0);
        }

        public TokenString(String i_strSeparator)
        {
            Allocs(0);
            if (i_strSeparator != null)
                _separator = i_strSeparator;
        }

        public TokenString(String i_strSeparator, int i_iPreallocate)
        {
            Allocs(i_iPreallocate);
            if (i_strSeparator != null)
                _separator = i_strSeparator;
        }

        public TokenString(int i_iPreallocate)
        {
            Allocs(i_iPreallocate);
        }

        public bool SetSeparator(String i_strSeparator)
        {
            if (i_strSeparator == null || i_strSeparator.Length == 0)
                return false;

            _separator = i_strSeparator;
            return true;
        }

        public void SetCaseSensitive(bool isInput)
        {
            _caseSens = isInput;
        }

        public String Get(int i_iPos)
        {
            if (i_iPos < 0 || i_iPos >= _strings.Count)
                return null;

            return _strings[i_iPos];
        }

        public String Set(int i_iPos, String i_strValue)
        {
            if (i_strValue == null)
                return null;

            if (i_iPos < 0 || i_iPos >= _strings.Count)
                return null;

            _strings[i_iPos] = i_strValue;
            return _strings[i_iPos];
        }

        public String this[int i_iPos]
        {
            get
            {
                return Get(i_iPos);
            }

            set
            {
                Set(i_iPos, value);
            }
        }

        public void AddUnique(String input)
        {
            if (input == null || input.Length == 0)
                throw new ArgumentException();

            if (input.IndexOf(_separator) >= 0)
                throw new ArgumentException();

            bool bFound = false;
            string strBuf;
            for (int i = 0; i < _strings.Count; i++)
                if (_caseSens)
                {
                    if (_strings[i].CompareTo(input) == 0)
                    {
                        bFound = true;
                        break;
                    }
                }
                else
                {
                    strBuf = _strings[i];
                    if (strBuf.ToLower().CompareTo(input.ToLower()) == 0)
                    {
                        bFound = true;
                        break;
                    }
                }

            if (!bFound)
                _strings.Add(input);
            else
                throw new AlreadyExistsException();
        }

        public void DelUnique(String input)
        {
            if (input == null || input.Length == 0)
                throw new ArgumentException();

            if (input.IndexOf(_separator) >= 0)
                throw new ArgumentException();

            int iFound = -1;
            string strBuf;
            for (int i = 0; i < _strings.Count; i++)
                if (_caseSens)
                {
                    if (_strings[i].CompareTo(input) == 0)
                    {
                        iFound = i;
                        break;
                    }
                }
                else
                {
                    strBuf = _strings[i];
                    strBuf.ToLower();
                    if (strBuf.CompareTo(input.ToLower()) == 0)
                    {
                        iFound = i;
                        break;
                    }
                }

            if (iFound >= 0 && iFound < _strings.Count)
                _strings.RemoveAt(iFound);
            else
                throw new DoesNotExistException();
        }

        public void Load(String input)
        {
            if (_strings != null)
                _strings.Clear();
            else
                Allocs(0);

            int iFound = -1;
            int iLastFound = -1;
            int iTo = -1;
            do
            {
                iLastFound = iFound;
                iFound = input.IndexOf(_separator, iFound + _separator.Length);
                if (iFound >= 0)
                    iTo = iFound;
                else
                    iTo = input.Length;

                _strings.Add(input.Substring(iLastFound + _separator.Length,
                        iTo - (iLastFound + _separator.Length)));
            }
            while (iFound >= 0);
        }

        public String Save()
        {
            string strBuf = String.Empty;
            for (int i = 0; i < _strings.Count; i++)
            {
                strBuf += _strings[i];
                if (i < _strings.Count - 1)
                    strBuf += _separator;
            }

            return strBuf;
        }

        public int GetCount()
        {
            return _strings.Count;
        }

        public String GetSeparator()
        {
            return _separator;
        }

    }
}
