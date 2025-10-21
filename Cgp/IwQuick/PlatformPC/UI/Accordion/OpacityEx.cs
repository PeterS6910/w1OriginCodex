using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Contal.IwQuick.PlatformPC.UI.Accordion
{
    public static class OpacityEx
    {

        private static readonly Hashtable Ht = new Hashtable();
        private class Data
        {
            private PictureBox _pbox = new PictureBox { BorderStyle = BorderStyle.None };
            private Timer _fadeTimer = new Timer();
            private readonly Control _control;
            private Bitmap _bmpBack, _bmpFore;
            private float _blend = 1;
            private int _blendDir = 1;
            private float _step = 0.02f;

            public Data(Control control)
            {
                _control = control;
                _fadeTimer.Interval = 20;
                _fadeTimer.Tick += opacityTimer_Tick;
                _pbox.Paint += pbox_Paint;
                _blend = control.Visible ? 1 : 0;
            }

            public void Dispose()
            {
                if (_bmpBack != null)
                    _bmpBack.Dispose();

                if (_bmpFore != null)
                    _bmpFore.Dispose();

                if (_pbox != null)
                    _pbox.Dispose();

                if (_fadeTimer != null)
                    _fadeTimer.Dispose();

                _bmpBack = null;
                _bmpFore = null;
                _pbox = null;
                _fadeTimer = null;
            }

            void pbox_Paint(object sender, PaintEventArgs e)
            {
                if (_bmpFore == null || _bmpBack == null)
                    return;

                var rc = new Rectangle(Point.Empty, _control.Size);
                var cm = new ColorMatrix();
                var ia = new ImageAttributes();
                cm.Matrix33 = _blend;
                ia.SetColorMatrix(cm);
                e.Graphics.DrawImage(_bmpFore, rc, 0, 0, _bmpFore.Width, _bmpFore.Height, GraphicsUnit.Pixel, ia);
                cm.Matrix33 = 1f - _blend;
                ia.SetColorMatrix(cm);
                e.Graphics.DrawImage(_bmpBack, rc, 0, 0, _bmpBack.Width, _bmpBack.Height, GraphicsUnit.Pixel, ia);
                ia.Dispose();
            }

            public void FadeIn(int millis)
            {
                if (millis <= 0)
                {
                    _blend = 1;
                    StopFade();
                    return;
                }

                if (Math.Abs(_blend - 1) < float.Epsilon)
                    return;

                if (!_fadeTimer.Enabled)
                    CreateBitmaps();

                _step = 1f / (millis / 30f);
                StartFade(1);
            }

            public void FadeOut(int millis)
            {
                if (millis <= 0)
                {
                    _blend = 0;
                    StopFade(); // disabled timer
                    return;
                }

                if (Math.Abs(_blend) < float.Epsilon)
                    return;

                if (!_fadeTimer.Enabled)
                {
                    CreateBitmaps();
                    // fading out sets the control Visible to false
                    // which will cause a flicker if paint is not called
                    _pbox.Refresh(); // immediately calls pbox_Paint
                    //pbox.Invalidate();
                }

                _step = 1f / (millis / 30f);
                StartFade(-1);
            }

            private void StartFade(int dir)
            {
                _blendDir = dir;
                _fadeTimer.Enabled = true;
            }

            private void CreateBitmaps()
            {
                if (_bmpBack != null)
                    _bmpBack.Dispose();

                if (_bmpFore != null)
                    _bmpFore.Dispose();

                var r = _control.Bounds;
                var r2 = r;

                r.Location = Point.Empty;

                _bmpFore = new Bitmap(r.Width, r.Height);
                _control.DrawToBitmap(_bmpFore, r);

                if (_control.Visible)
                    _control.Visible = false;

                _bmpBack = CreateScreenCapture(_control.Parent, r2);
                _pbox.Size = r.Size;
                _control.Controls.Add(_pbox);
                _control.Controls.SetChildIndex(_pbox, 0);
                _control.Visible = true; // must set to visible when fading in from invisible, otherwise the pbox repaint isn't called
            }

            [DllImport("gdi32.dll", ExactSpelling = true, CharSet = CharSet.Auto, SetLastError = true)]
            private static extern bool BitBlt(IntPtr pHdc, int iX, int iY, int iWidth, int iHeight, IntPtr pHdcSource, int iXSource, int iYSource, Int32 dw);
            private const int Src = 0xCC0020;

            private static Bitmap CreateScreenCapture(Control c, Rectangle r)
            {
                Bitmap bmp;
                using (var g = c.CreateGraphics())
                {
                    bmp = new Bitmap(r.Width, r.Height, g);
                    var g2 = Graphics.FromImage(bmp);
                    var g2Hdc = g2.GetHdc();
                    var gHdc = g.GetHdc();
                    BitBlt(g2Hdc, 0, 0, r.Width, r.Height, gHdc, r.X, r.Y, Src);
                    g.ReleaseHdc(gHdc);
                    g2.ReleaseHdc(g2Hdc);
                    g2.Dispose();
                }
                return bmp;
            }

            void opacityTimer_Tick(object sender, EventArgs e)
            {
                _blend += _blendDir * _step;
                var done = false;
                if (_blend < 0) { done = true; _blend = 0; }
                if (_blend > 1) { done = true; _blend = 1; }
                if (done)
                    StopFade();
                else
                    _pbox.Invalidate();
            }

            private void StopFade()
            {
                _fadeTimer.Enabled = false;
                _control.Visible = Math.Abs(_blend - 1) < float.Epsilon;
                // timing issue. As a resize bar finishes fading out,
                // other control may already have been clicked. However,
                // when the pbox is removed, it causes the focus to change
                // to the first child control. So if a user clicked on
                // one resize bar, and then click on another resize bar
                // before the first one faded out, then the focus would
                // change to some other control, and the resize bar that
                // was just clicked is no longer the focused control.
                // So the focus is put back to the original window with
                // the focus.
                var hWnd = GetFocus();
                _control.Controls.Remove(_pbox);
                SetFocus(hWnd);
            }
        }

        private static Data GetData(Control control)
        {
            var d = (Data)Ht[control];
            if (d == null)
            {
                d = new Data(control);
                Ht[control] = d;
                control.Disposed += delegate
                {
                    Ht.Remove(control);
                    d.Dispose();
                };
            }
            return d;
        }

        public static void FadeIn(this Control control, int millis)
        {
            var d = GetData(control);
            d.FadeIn(millis);
        }

        public static void FadeOut(this Control control, int millis)
        {
            var d = GetData(control);
            d.FadeOut(millis);
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetFocus(); // returns the window handle with the active focus

        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);
    }

}