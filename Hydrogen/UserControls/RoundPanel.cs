using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Hydrogen.UserControls
{
    public partial class RoundPanel : Control
    {
        public int CornerRadius { get; set; } = 10;
        public int BorderThickness { get; set; } = 2;
        public Color BorderColor { get; set; } = Color.Gray;

        public RoundPanel()
        {
            this.ResizeRedraw = true; // 리사이즈 시 자동 리프레시
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.OptimizedDoubleBuffer |
                          ControlStyles.ResizeRedraw |
                          ControlStyles.UserPaint, true);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            SetRoundedRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = new Rectangle(
                BorderThickness / 2,
                BorderThickness / 2,
                this.Width - BorderThickness,
                this.Height - BorderThickness);

            using (GraphicsPath path = GetRoundedPath(rect, CornerRadius))
            using (Pen pen = new Pen(BorderColor, BorderThickness))
            using (Brush brush = new SolidBrush(this.BackColor))
            {
                // 배경 채우기
                e.Graphics.FillPath(brush, path);
                // 테두리 그리기
                e.Graphics.DrawPath(pen, path);
            }
        }

        private void SetRoundedRegion()
        {
            using (GraphicsPath path = GetRoundedPath(new Rectangle(0, 0, this.Width, this.Height), CornerRadius))
            {
                this.Region = new Region(path);
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle rect, int radius)
        {
            int r = radius * 2; // 직경
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, r, r, 180, 90);
            path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
            path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
            path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}