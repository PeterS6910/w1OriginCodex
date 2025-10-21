using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Windows.Forms;

using System.Diagnostics;

namespace Contal.IwQuick.UI
{
    public class ColorPair
    {
        private Color _foreColor;
        private bool _foreColorSet = false;

        public void UnsetForeColor()
        {
            _foreColorSet = false;
        }


        /// <summary>
        /// <exception cref="EValueNotSet">If the Fore color is not set</exception>
        /// </summary>
        /// <returns></returns>
        public Color ForeColor
        {
            get
            {
                if (!_foreColorSet)
                {
                    Debug.Assert(false, "Color value is not set, but was read instead");
                    return default(Color);
                }

                return _foreColor;
            }
            set
            {
                _foreColor = value;
                _foreColorSet = true;
            }
        }


        private Color _backColor;
        private bool _backColorSet = false;

        public void UnsetBackColor()
        {
            _backColorSet = false;
        }


        /// <summary>
        /// <exception cref="EValueNotSet">If the Back color is not set</exception>
        /// </summary>
        /// <returns></returns>
        public Color BackColor
        {
            get
            {
                if (!_backColorSet)
                {
                    Debug.Assert(false, "Color value is not set, but was read instead");
                    return default(Color);
                }

                return _backColor;
            }
            set
            {
                _backColor = value;
                _backColorSet = true;
            }
        }

        public ColorPair(Color foreColor, Color backColor)
        {
            ForeColor = foreColor;
            BackColor = backColor;
        }

        public ColorPair()
        {
        }

        public ColorPair(bool isForeground, Color color)
        {
            if (isForeground)
                ForeColor = color;
            else
                BackColor = color;
        }

        public ColorPair(Control control)
        {
            if (!Pick(control))
                throw new ArgumentException();
        }

        public bool Pick(Control control)
        {
            if (null == control)
                return false;
            try
            {
                lock (control)
                {
                    ForeColor = control.ForeColor;
                    BackColor = control.BackColor;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool Apply(Control control)
        {
            if (null == control)
                return false;

            try
            {
                lock (control)
                {
                    if (_foreColorSet)
                        control.ForeColor = _foreColor;

                    if (_backColorSet)
                        control.BackColor = _backColor;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
