using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Contal.IwQuick.PlatformPC.UI.Accordion
{

    ///<summary>Different options for rendering the resize bar.</summary>
    public enum ResizeBarVisualStyle
    {

        ///<summary>The default renderer is not used.</summary>
        Custom = -1,

        ///<summary>Non-themed colors, typically gray, are used to render the bar.</summary>
        ClassicSubtle = 0,

        ///<summary>Non-themed colors, typically gray, are used to render the bar.</summary>
        ClassicStrong = 1,

        ///<summary>Non-themed colors, typically gray, are used to render the bar.</summary>
        ClassicNone = 2,

        ///<summary>Themed rendering is used, where only the grip handle changes colors with mouse interaction.</summary>
        ModernSubtle = 10,

        ///<summary>Themed rendering is used, where the entire bar changes colors with mouse interaction.</summary>
        ModernStrong = 11,

        ///<summary>Themed rendering is used. Colors do not change with mouse interaction.</summary>
        ModernNone = 12,
    }

    ///<summary>A visual cue for the user that another control can be resized.</summary>
    public sealed class ResizeBar : UserControl
    {

        ///<summary>An option to show or hide the handle that appears in the middle of the resize bar. The default value is true.</summary>
        public bool ShowGrip { get; set; }

        ///<summary>Different options for rendering this widget. The default value is ModernStrong.</summary>
        public ResizeBarVisualStyle VisualStyle { get; set; }

        private Renderer _renderer;
        private TrackBarThumbState _state = TrackBarThumbState.Normal;

        public ResizeBar()
        {
            ShowGrip = true;
            Cursor = Cursors.SizeNS;
            MinimumSize = new Size(0, 13); // doesn't render properly at smaller heights
            using (var g = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpiY = g.DpiY;
                Size = new Size(0, Math.Max(13, (int)(13 * dpiY / 120)));
            }

            VisualStyle = ResizeBarVisualStyle.ModernStrong; // ClassicNone
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
            _renderer = new Renderer(this);
        }

        private class Renderer
        {
            private readonly VisualStyleRenderer _rGrip0;
            private readonly VisualStyleRenderer _rBar1;
            private readonly VisualStyleRenderer _rBarDisabled;
            private readonly SolidBrush _brushDisabled = new SolidBrush(Color.FromArgb(231, 234, 234));
            private readonly SolidBrush _brushNormal = new SolidBrush(Color.FromArgb(230, 230, 228));

            private ResizeBar Owner { get; set; }
            private readonly bool _renderWithVisualStyles;

            public Renderer(ResizeBar owner)
            {
                _renderWithVisualStyles = (Application.RenderWithVisualStyles && VisualStyleRenderer.IsSupported && VisualStyleInformation.IsEnabledByUser);

                Owner = owner;
                if (_renderWithVisualStyles)
                {
                    _rBarDisabled = new VisualStyleRenderer(VisualStyleElement.TrackBar.Track.Normal);
                    _rGrip0 = new VisualStyleRenderer(VisualStyleElement.Rebar.Gripper.Normal);
                    _rBar1 = new VisualStyleRenderer(VisualStyleElement.TrackBar.Thumb.Normal);
                }
            }

            public void Dispose()
            {
                _brushNormal.Dispose();
                _brushDisabled.Dispose();
            }

            public void Draw(Graphics g, Rectangle bounds, Rectangle grip, TrackBarThumbState state)
            {
                ResizeBarVisualStyle style = Owner.VisualStyle;
                if (!_renderWithVisualStyles)
                {
                    if (style == ResizeBarVisualStyle.ModernStrong)
                        style = ResizeBarVisualStyle.ClassicStrong;
                    else if (style == ResizeBarVisualStyle.ModernSubtle)
                        style = ResizeBarVisualStyle.ClassicSubtle;
                    else if (style == ResizeBarVisualStyle.ModernNone)
                        style = ResizeBarVisualStyle.ClassicNone;
                }

                if (style == ResizeBarVisualStyle.ModernStrong || style == ResizeBarVisualStyle.ModernNone)
                {
                    if (state == TrackBarThumbState.Disabled)
                        _rBarDisabled.DrawBackground(g, bounds);
                    else
                    {
                        if (style == ResizeBarVisualStyle.ModernNone)
                            _rBar1.DrawBackground(g, bounds);
                        else
                            TrackBarRenderer.DrawHorizontalThumb(g, bounds, state);
                    }

                    if (Owner.ShowGrip)
                        DrawGrip(g, grip, 0);
                }
                else if (style == ResizeBarVisualStyle.ModernSubtle)
                {
                    if (state == TrackBarThumbState.Disabled)
                    {
                        _rBarDisabled.DrawBackground(g, bounds);
                        if (Owner.ShowGrip)
                        {
                            _rBarDisabled.DrawBackground(g, grip);
                            DrawGrip(g, grip, -5);
                        }
                    }
                    else
                    {
                        _rBar1.DrawBackground(g, bounds);
                        if (Owner.ShowGrip)
                        {
                            TrackBarRenderer.DrawHorizontalThumb(g, grip, state);
                            DrawGrip(g, grip, -5);
                        }
                    }
                }
                else if (style == ResizeBarVisualStyle.ClassicStrong || style == ResizeBarVisualStyle.ClassicSubtle)
                {
                    Brush bg = _brushNormal;
                    if (state == TrackBarThumbState.Disabled)
                        bg = _brushDisabled;
                    else
                    {
                        if (style == ResizeBarVisualStyle.ClassicStrong)
                        {
                            if (state == TrackBarThumbState.Hot)
                                bg = SystemBrushes.GradientInactiveCaption;
                            else if (state == TrackBarThumbState.Pressed)
                                bg = SystemBrushes.GradientActiveCaption;
                        }
                    }

                    g.FillRectangle(bg, bounds);
                    ControlPaint.DrawBorder(g, bounds, SystemColors.ActiveBorder, ButtonBorderStyle.Solid);

                    if (Owner.ShowGrip)
                    {
                        var r = grip;
                        if (style == ResizeBarVisualStyle.ClassicSubtle)
                        {
                            bg = (state == TrackBarThumbState.Disabled ? _brushDisabled : _brushNormal);
                            if (state == TrackBarThumbState.Hot)
                                bg = SystemBrushes.GradientInactiveCaption;
                            else if (state == TrackBarThumbState.Pressed)
                                bg = SystemBrushes.GradientActiveCaption;

                            g.FillRectangle(bg, grip);
                            ControlPaint.DrawBorder(g, grip, SystemColors.ActiveBorder, ButtonBorderStyle.Solid);

                            r.Width -= r.Height;
                            r.X += r.Height / 2;
                        }

                        int numLines = Math.Max(2, bounds.Height / 10);
                        int h = 3 * numLines + (numLines - 1);
                        int dy = (bounds.Height - h) / 2;

                        for (int i = 0; i < numLines; i++)
                        {
                            g.DrawLine(SystemPens.ControlLight, r.X, r.Y + dy, r.X + r.Width, r.Y + dy);
                            g.DrawLine(SystemPens.ControlDark, r.X, r.Y + dy + 2, r.X + r.Width, r.Y + dy + 2);
                            dy += 4;
                        }
                    }
                }
                else if (style == ResizeBarVisualStyle.ClassicNone)
                {

                    int x1 = bounds.X;
                    int y1 = bounds.Y;
                    int x2 = bounds.X + bounds.Width;
                    int y2 = bounds.Y + bounds.Height;

                    g.FillRectangle(SystemBrushes.Control, bounds);

                    g.DrawLine(SystemPens.ControlLight, x1 - 1, y1, x1 - 1, y2);
                    g.DrawLine(SystemPens.ControlLightLight, x1, y1, x1, y2);

                    g.DrawLine(SystemPens.ScrollBar, x1, y2 - 2, x2, y2 - 2);
                    g.DrawLine(SystemPens.ControlDark, x1, y2 - 1, x2, y2 - 1);

                    g.DrawLine(SystemPens.ControlLight, x1, y1, x2, y1);
                    g.DrawLine(SystemPens.ControlLightLight, x1, y1 + 1, x2, y1 + 1);

                    g.DrawLine(SystemPens.ScrollBar, x2 - 2, y1, x2 - 2, y2 - 2);
                    g.DrawLine(SystemPens.ControlDark, x2 - 1, y1, x2 - 1, y2);
                }
            }

            private void DrawGrip(Graphics g, Rectangle grip, int adjustWidth)
            {
                var r = grip;
                r.Y += 2;
                r.Height -= 4;
                r.Width += adjustWidth;

                // pixel adjustments
                if (adjustWidth != 0)
                {
                    if (r.Width % 5 == 4)
                        r.X++;
                    else if (r.Width % 5 == 3)
                    {
                        r.Width++;
                        r.X++;
                    }
                    else
                        r.X += 2;
                }
                else
                {
                    if (r.Width % 5 == 3)
                        r.Width++;
                    else if (r.Width % 5 == 2)
                        r.Width--;
                }

                if (r.Height % 4 == 2)
                {
                    r.Y++;
                    r.Height--;
                }
                else if (r.Height % 4 == 3)
                {
                    r.Y--;
                }

                _rGrip0.DrawBackground(g, r);
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _state = TrackBarThumbState.Hot;
            Refresh();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _state = TrackBarThumbState.Normal;
            Refresh();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _state = TrackBarThumbState.Pressed;
            Refresh();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _state = TrackBarThumbState.Hot;
            Refresh();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            Refresh();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            Refresh();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_renderer != null)
            {
                Size s = Size;
                var g = e.Graphics;
                var r = new Rectangle(Point.Empty, s);
                int handleLength = Math.Max(20, 3 * s.Height);
                var r2 = new Rectangle((s.Width - handleLength) / 2, 0, handleLength, s.Height);
                var st = _state;
                if (!Enabled)
                    st = TrackBarThumbState.Disabled;
                else if (Focused)
                    st = TrackBarThumbState.Pressed;

                _renderer.Draw(g, r, r2, st);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                if (_renderer != null)
                    _renderer.Dispose();
                _renderer = null;
            }
        }
    }

}