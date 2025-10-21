/*---------------------------------------------------------------------------
*	Namespace   : Cgp.Components
*   Assembly    : Cgp.Components.dll
*   
*   Class       : TitleBarButton
*  
*	Description : Class creates a new Title Bar Button, whitch is added
*	              to the parent window Form.    
*
*   Requirement key :  R_2275  
*
*
*	Copyright (C) 2013 CONTAL OK s.r.o. All Rights Reserved.
-----------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Contal.IwQuick.Sys.Microsoft;
using System.Runtime.InteropServices;
using Contal.IwQuick.Sys;

namespace Cgp.Components
{
    /// <summary>
    /// Delegate that holds a reference to the title bar button event handler.
    /// </summary>
    public delegate void TitleBarButtonClickedEventHandler(object sender, EventArgs e);
    
    /// <summary>
    /// Class creates a new Title Bar Button, whitch is added
    /// to the parent Form.
    /// </summary>
    /// 
    /// <devdoc>Req_key : R_2275 </devdoc>
    public class TitleBarButton : NativeWindow
    {
        #region Private Member Variables
        private bool _pressed  = false; //flag, true if button was pressed  
        private bool _docked   = true;  //flag, determines whether the control borders are docked to the parrent
        private Size _windowSize = new Size();
        private Form _parent;
        // Current title bar button positions
        private int _xPositionAdjust = 0;
        private int _yPositionAdjust = 0;
        private int _width = 21;
        private int _height = 18;
        #endregion
       
        #region Constructors
        
        /// <summary>
        ///     The class constructor. 
        /// </summary>
        /// <param name="parent">Represents the parent Form where button is added.</param>
        /// <param name="xPositionAdjust">X-coordinate of the button possition.</param>
        /// <param name="yPositionAdjust">Y-coordinate of the button possition.</param>
        /// <param name="width">The button width.</param>
        /// <param name="height">The button height.</param>
        /// <exception cref="System.ArgumentException">Thrown when parent is null.</exception>
        public TitleBarButton(Form parent, int xPositionAdjust, int yPositionAdjust, int width, int height)
        {
            if (parent == null)
                throw new ArgumentException("Parameter cannot be null", "parent");

            _xPositionAdjust = xPositionAdjust;
            _yPositionAdjust = yPositionAdjust;
            _width = width;
            _height = height;
            //registered delegates 
            parent.HandleCreated   += new EventHandler(this.OnHandleCreated);
            parent.HandleDestroyed += new EventHandler(this.OnHandleDestroyed);
            parent.LostFocus += new EventHandler(this.OnLostFocus);

            this._parent = parent;
        }
        #endregion
        
        #region Properties
        /// <summary>
        /// 	Gets or sets the flag that indicates whether the control borders are docked to the parent.
        /// </summary>
        public bool Docked
        {
            get { return _docked; }
            set
            {
                _docked = value;
                _pressed = false;
            }
        }
        /// <summary>
        /// 	Gets or sets the width of the button.
        /// </summary>
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        /// <summary>
        /// 	Gets or sets the height of the button.
        /// </summary>
        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }
        /// <summary>
        /// 	Gets or sets the button's Y-position in the client coordinates system.
        /// </summary>
        public int Y_PositionAdjust
        {
            get { return _yPositionAdjust; }
            set { _yPositionAdjust = value; }
        }

        /// <summary>
        /// 	Gets or sets button's X-position in the client coordinates system.
        /// </summary>
        public int X_positionAdjust
        {
            get { return _xPositionAdjust; }
            set { _xPositionAdjust = value; }
        }
        #endregion

        #region Events and Handlers
        /// <summary>
        ///     The event is raised when the title bar button is clicked. 
        /// </summary>
        public event TitleBarButtonClickedEventHandler ButtonClicked;

        /// <summary>
        /// Listen for the control's window creation and then hook into it.
        /// </summary>
        protected void OnHandleCreated(object sender, EventArgs e)
        {
            // Window is now created, assign handle to NativeWindow.
            AssignHandle(((Form)sender).Handle);
        }

        /// <summary>
        /// Listen for the destroy of window.
        /// </summary>
        protected void OnHandleDestroyed(object sender, EventArgs e)
        {
            // Window was destroyed, release hook.
            ReleaseHandle();
        }

        /// <summary>
        /// Listen if the parent window lost focus.
        /// </summary>
        protected void OnLostFocus(object sender, EventArgs e)
        {
            // Invalidate the region of  the control
           _parent.Invalidate();//force WM_PAINT to redraw button   
        }
  
        #endregion

        #region Win32 API Imports
        [DllImport("user32")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        #endregion

        #region WndProc
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
        protected override void WndProc(ref Message m)
        {
            // Change the Pressed-State of the Button when the User pressed the
            // left mouse button and moves the cursor over the Button.
            if (m.Msg == DllUser32.WM_MOUSEMOVE)
            {
                if (DllUser32.GetCapture() != 0)
                {
                    if (_pressed)
                    {
                        if (!MouseOverBtn())
                        {
                            _pressed = false; //reset the flag, but control has still captured the mouse
                            DrawButton();
                        }
                    }
                    else
                    {
                        //Determines whether the left mouse button is currently pressed,test the MSB bit. 
                        if ((GetAsyncKeyState(DllUser32.VK_LBUTTON) & (0x8000)) != 0)
                        {
                            if (MouseOverBtn())
                            {
                                _pressed = true;
                                DrawButton();
                            }
                        }
                    }
                }
            }

            // Button released and eventually clicked.
            if (m.Msg == DllUser32.WM_LBUTTONUP)
            {
                DllUser32.ReleaseCapture();
                if (_pressed)
                {
                    _pressed = false;
                    DrawButton();
                    if (MouseOverBtn())
                    {
                        if (ButtonClicked != null)
                            ButtonClicked(this, new EventArgs());
                        return;
                    }
                }
            }

            // Clicking the Button - capture the mouse and await until the User relases the button again
            if (m.Msg == DllUser32.WM_NCLBUTTONDOWN)
            {
                Point pnt = new Point((int)m.LParam);
                if (MouseInBtnDown(pnt))
                {
                    _pressed = true;
                    DrawButton();
                    DllUser32.SetCapture((int)_parent.Handle);//directs all mouse messages to parent's hwnd
                    return;
                }
            }

            // Drawing the Button and getting the Real Size of the Window.
            switch (m.Msg)
            {
                case DllUser32.WM_ACTIVATE:
                case DllUser32.WM_SIZE:
                case DllUser32.WM_SYNCPAINT:
                case DllUser32.WM_NCACTIVATE:
                case DllUser32.WM_NCCREATE:
                case DllUser32.WM_NCHITTEST:
                case DllUser32.WM_PAINT:
                    {
                        if (m.Msg == DllUser32.WM_SIZE) 
                            _windowSize = new Size(new Point((int)m.LParam));
                        DrawButton();
                    }
                    break;
            }
            base.WndProc(ref m);
        }
        #endregion

        #region Button-Specific Functions
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int X1; // x position of upper-left corner  
            public int Y1; // y position of upper-left corner  
            public int X2; // x position of lower-right corner  
            public int Y2; // y position of lower-right corner  
        }

        /// <summary>
        /// Determines whether mouse cursor possition is over the button.
        /// The parent's hwnd already captured all mouse messages.
        /// </summary>
        /// <returns>True if mouse is over.</returns>
        private bool MouseOverBtn()
        {
            RECT rec;
            GetWindowRect(_parent.Handle, out rec);
            int xCursorPosition = Cursor.Position.X;
            int yCursorPoaition = Cursor.Position.Y;
            // returns true if cursor position is over button area           
            return (xCursorPosition >= rec.X2 - _xPositionAdjust - 17 && xCursorPosition <= rec.X2 + _width - _xPositionAdjust - 17 &&
                yCursorPoaition >= rec.Y1 + _yPositionAdjust && yCursorPoaition <= rec.Y1 + _height + _yPositionAdjust);
        }

        /// <summary>
        /// Determines whether mouse cursor possition is over the button.
        /// </summary>
        /// <param name="clickPosition">Mouse position in the client coordinates system.</param>
        /// <returns>True if mouse is over.</returns>
        private bool MouseInBtnDown(Point clickPosition)
        {
            RECT rec;
            GetWindowRect(_parent.Handle, out rec);
            // returns true if the cursor position is over the button
            return (clickPosition.X >= rec.X2 - _xPositionAdjust - 17 && clickPosition.X <= rec.X2 + _width - _xPositionAdjust - 17 &&
                clickPosition.Y >= rec.Y1 + _yPositionAdjust && clickPosition.Y <= rec.Y1 + _height + _yPositionAdjust);
        }

        /// <summary>
        /// Draws a title bar button.
        /// </summary>
        private void DrawButton()
        {
            try
            {
               Graphics g = Graphics.FromHdc((IntPtr)DllUser32.GetWindowDC((int)_parent.Handle));
               DrawButton(g, _pressed);
            }
            catch (Exception e)
            {
                HandledExceptionAdapter.Examine(e);
            }
        }

        /// <summary>
        /// Draws the Dock/Undock button. 
        /// </summary>
        /// <param name="g">GDI+ drawing surface.</param>
        /// <param name="pressed">Indicates the Pressed-State of the Button.</param>
        private void DrawButton(Graphics g, bool pressed)
        {
            RECT rec;
            GetWindowRect(_parent.Handle, out rec);
            Point pos = new Point(_windowSize.Width - _xPositionAdjust, _yPositionAdjust);

            Color light = SystemColors.ControlLightLight;
            Color icon = SystemColors.ControlText;
            Color background = SystemColors.Control;
            Color shadow1 = SystemColors.ControlDark;
            Color shadow2 = SystemColors.ControlDarkDark;

            Color tmp1, tmp2;

            if (pressed)
            {
                tmp1 = shadow2;
                tmp2 = light;
            }
            else
            {
                tmp1 = light;
                tmp2 = shadow2;
            }

            g.DrawLine(new Pen(tmp1), pos, new Point(pos.X + _width - 1, pos.Y));
            g.DrawLine(new Pen(tmp1), pos, new Point(pos.X, pos.Y + _height - 1));

            if (pressed)
            {
                g.DrawLine(new Pen(shadow1), pos.X + 1, pos.Y + 1, pos.X + _width - 2, pos.Y + 1);
                g.DrawLine(new Pen(shadow1), pos.X + 1, pos.Y + 1, pos.X + 1, pos.Y + _height - 2);
            }
            else
            {
                g.DrawLine(new Pen(shadow1), pos.X + _width - 2, pos.Y + 1, pos.X + _width - 2, pos.Y + _height - 2);
                g.DrawLine(new Pen(shadow1), pos.X + 1, pos.Y + _height - 2, pos.X + _width - 2, pos.Y + _height - 2);
            }

            g.DrawLine(new Pen(tmp2), pos.X + _width - 1, pos.Y + 0, pos.X + _width - 1, pos.Y + _height - 1);
            g.DrawLine(new Pen(tmp2), pos.X + 0, pos.Y + _height - 1, pos.X + _width - 1, pos.Y + _height - 1);

            g.FillRectangle(new SolidBrush(background), pos.X + 1 + Convert.ToInt32(pressed), pos.Y + 1 + Convert.ToInt32(pressed),
                _width - 3, _height - 3);

            if (_docked)
            {
                g.FillRectangle(new SolidBrush(icon), pos.X + 5 + Convert.ToInt32(pressed), pos.Y + 5 + Convert.ToInt32(pressed), _width - 9, _height - 9);
                g.FillRectangle(new SolidBrush(background), pos.X + 6 + Convert.ToInt32(pressed), pos.Y + 7 + Convert.ToInt32(pressed), _width - 11, _height - 12);

                g.FillRectangle(new SolidBrush(icon), pos.X + 3 + Convert.ToInt32(pressed), pos.Y + 3 + Convert.ToInt32(pressed), _width - 12, _height - 11);
                g.FillRectangle(new SolidBrush(background), pos.X + 4 + Convert.ToInt32(pressed), pos.Y + 5 + Convert.ToInt32(pressed), _width - 14, _height - 14);
            }
            else
            {
                g.FillRectangle(new SolidBrush(icon), pos.X + 2 + Convert.ToInt32(pressed), pos.Y + 2 + Convert.ToInt32(pressed), _width - 5, _height - 5);
                g.FillRectangle(new SolidBrush(background), pos.X + 3 + Convert.ToInt32(pressed), pos.Y + 4 + Convert.ToInt32(pressed), _width - 7, _height - 8);

                g.FillRectangle(new SolidBrush(icon), pos.X + 4 + Convert.ToInt32(pressed), pos.Y + 5 + Convert.ToInt32(pressed), _width - 12, _height - 11);
                g.FillRectangle(new SolidBrush(background), pos.X + 5 + Convert.ToInt32(pressed), pos.Y + 7 + Convert.ToInt32(pressed), _width - 14, _height - 14);
            }
        }
        #endregion
    }
}
