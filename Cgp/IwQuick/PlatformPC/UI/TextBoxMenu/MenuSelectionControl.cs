using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Contal.IwQuick.UI
{
    public enum MenuPosition
    {
        Left,        
        LeftTop,
        LeftBottom,
        Right,
        RightTop,        
        RightBottom,
        Top,
        Bottom
    }

    [ToolboxItem(false)]
    public partial class MenuSelectionControl : UserControl
    {
        private MenuPosition _menuPosition = MenuPosition.Right;
        private IWindowsFormsEditorService editorService = null;

        public MenuPosition MenuPosition
        {
            get
            {
                return this._menuPosition;
            }
            set
            {
                if (this._menuPosition != value)
                {
                    this._menuPosition = value;
                }
            }
        }   


        public MenuSelectionControl(MenuPosition menuPosition, IWindowsFormsEditorService editorService)
        {
            // This call is required by the designer.
            InitializeComponent();

            // Cache the menuposition value provided by the 
            // design-time environment.
            this._menuPosition = menuPosition;

            // Cache the reference to the editor service.
            this.editorService = editorService;
        }

        private void _bLeft_Click(object sender, EventArgs e)
        {
            this._menuPosition = MenuPosition.Left;
            this.Invalidate(false);
            this.editorService.CloseDropDown();
        }

        private void _bLeftTop_Click(object sender, EventArgs e)
        {
            this._menuPosition = MenuPosition.LeftTop;
            this.Invalidate(false);
            this.editorService.CloseDropDown();
        }

        private void _bLeftBottom_Click(object sender, EventArgs e)
        {
            this._menuPosition = MenuPosition.LeftBottom;
            this.Invalidate(false);
            this.editorService.CloseDropDown();
        }

        private void _bRight_Click(object sender, EventArgs e)
        {
            this._menuPosition = MenuPosition.Right;
            this.Invalidate(false);
            this.editorService.CloseDropDown();
        }

        private void _bRightTop_Click(object sender, EventArgs e)
        {
            this._menuPosition = MenuPosition.RightTop;
            this.Invalidate(false);
            this.editorService.CloseDropDown();
        }

        private void _bRightBottom_Click(object sender, EventArgs e)
        {
            this._menuPosition = MenuPosition.RightBottom;
            this.Invalidate(false);
            this.editorService.CloseDropDown();
        }
        
        private void _bTop_Click(object sender, EventArgs e)
        {
            this._menuPosition = MenuPosition.Top;
            this.Invalidate(false);
            this.editorService.CloseDropDown();
        }

        private void _bBottom_Click(object sender, EventArgs e)
        {
            this._menuPosition = MenuPosition.Bottom;
            this.Invalidate(false);
            this.editorService.CloseDropDown();
        }
     
       

        //void button2_Click(object sender, EventArgs e)
        //{
        //    this._menuPosition = MenuPosition.Right;

        //    this.Invalidate(false);

        //    this.editorService.CloseDropDown();
        //}

        //void button1_Click(object sender, EventArgs e)
        //{
        //    this._menuPosition = MenuPosition.Left;

        //    this.Invalidate(false);

        //    this.editorService.CloseDropDown();
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        // Be sure to unhook event handlers
        //        // to prevent "lapsed listener" leaks.
        //        this.button1.Click -=
        //            new EventHandler(button1_Click);
        //        this.button2.Click -=
        //            new EventHandler(button2_Click);

        //        if (components != null)
        //        {
        //            components.Dispose();
        //        }
        //    }
        //    base.Dispose(disposing);
        //}

        // LightShape is the property for which this control provides
        // a custom user interface in the Properties window.
           

        //protected override void OnPaint(PaintEventArgs e)
        //{
        //    base.OnPaint(e);

        //    using (
        //        Graphics gSquare = this.squarePanel.CreateGraphics(),
        //        gCircle = this.circlePanel.CreateGraphics())
        //    {
        //        // Draw a filled square in the client area of
        //        // the squarePanel control.
        //        gSquare.FillRectangle(
        //            Brushes.Red,
        //            0,
        //            0,
        //            this.squarePanel.Width,
        //            this.squarePanel.Height
        //            );

        //        // If the Square option has been selected, draw a 
        //        // border inside the squarePanel.
        //        if (this.lightShapeValue == MarqueeLightShape.Square)
        //        {
        //            gSquare.DrawRectangle(
        //                Pens.Black,
        //                0,
        //                0,
        //                this.squarePanel.Width - 1,
        //                this.squarePanel.Height - 1);
        //        }

        //        // Draw a filled circle in the client area of
        //        // the circlePanel control.
        //        gCircle.Clear(this.circlePanel.BackColor);
        //        gCircle.FillEllipse(
        //            Brushes.Blue,
        //            0,
        //            0,
        //            this.circlePanel.Width,
        //            this.circlePanel.Height
        //            );

        //        // If the Circle option has been selected, draw a 
        //        // border inside the circlePanel.
        //        if (this.lightShapeValue == MarqueeLightShape.Circle)
        //        {
        //            gCircle.DrawRectangle(
        //                Pens.Black,
        //                0,
        //                0,
        //                this.circlePanel.Width - 1,
        //                this.circlePanel.Height - 1);
        //        }
        //    }
        //}

        //private void squarePanel_Click(object sender, EventArgs e)
        //{
        //    this.lightShapeValue = MarqueeLightShape.Square;

        //    this.Invalidate(false);

        //    this.editorService.CloseDropDown();
        //}

        //private void circlePanel_Click(object sender, EventArgs e)
        //{
        //    this.lightShapeValue = MarqueeLightShape.Circle;

        //    this.Invalidate(false);

        //    this.editorService.CloseDropDown();
        //}
    }
}
