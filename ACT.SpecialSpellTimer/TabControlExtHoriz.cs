namespace ACT.SpecialSpellTimer
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public class TabControlExtHoriz : TabControl
    {
        private const uint TCM_ADJUSTRECT = (TCM_FIRST + 40);

        // code from SRT Project (what?)
        private const int TCM_FIRST = 0x1300;

        public TabControlExtHoriz() : base()
        {
            Alignment = TabAlignment.Top;

            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            DoubleBuffered = true;

            ItemSize = new Size(100, 24);
            // SizeMode = TabSizeMode.Fixed;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.Clear(Parent.BackColor);
            e.Graphics.FillRectangle(Brushes.White, 4, 4, ItemSize.Height - 4, Height - 8);

            int inc = 0;

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(224, 224, 224)), new Rectangle(0, 0, Width, 24));

            foreach (TabPage tp in TabPages)
            {
                Color fore = Color.Black;
                Font fontF = Font;
                Rectangle tabrect = GetTabRect(inc), rect = new Rectangle(tabrect.X, tabrect.Y, tabrect.Width, tabrect.Height - 2), textrect = new Rectangle(tabrect.X, tabrect.Y, tabrect.Width, tabrect.Height - 2);

                StringFormat sf = new StringFormat();
                sf.LineAlignment = StringAlignment.Center;
                sf.Alignment = StringAlignment.Center;

                if (inc == SelectedIndex)
                {
                    e.Graphics.FillRectangle(Brushes.White, rect);
                    fontF = new Font(Font, FontStyle.Bold);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Transparent), rect);
                }

                e.Graphics.DrawString(tp.Text, fontF, new SolidBrush(fore), textrect, sf);
                inc++;
            }
        }

        protected override void OnTabIndexChanged(EventArgs e)
        {
            base.OnTabIndexChanged(e);
            Invalidate();
        }

        protected override void WndProc(ref Message m)
        {
            if ((m.Msg == TCM_ADJUSTRECT))
            {
                RECT rc = (RECT)m.GetLParam(typeof(RECT));
                // Adjust these values to suit, dependant upon Appearance
                rc.left -= 5;
                rc.right += 5;
                rc.top -= 5;
                rc.bottom += 5;
                Marshal.StructureToPtr(rc, m.LParam, true);
            }

            base.WndProc(ref m);
        }

        #region RECT structure

        [StructLayout(LayoutKind.Sequential)]
        internal struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public Rectangle Rect { get { return new Rectangle(this.left, this.top, this.right - this.left, this.bottom - this.top); } }

            public static RECT FromXYWH(int x, int y, int width, int height)
            {
                return new RECT(
                    x,
                    y,
                    x + width,
                    y + height);
            }

            public static RECT FromRectangle(Rectangle rect)
            {
                return new RECT(
                    rect.Left,
                    rect.Top,
                    rect.Right,
                    rect.Bottom);
            }
        }

        #endregion RECT structure
    }
}
