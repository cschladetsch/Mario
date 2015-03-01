using System.Windows.Forms;

namespace LiquidSim
{
    public delegate void PaintHandler(PaintEventArgs pe);

    public class DoubleBufferedControl : Control
    {
        public DoubleBufferedControl()
        {
            this.SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
        }

        public event PaintHandler PaintRequested;

        public Liquid Liquid;

        protected override void OnPaint(PaintEventArgs pe)
        {
            if (PaintRequested != null)
                PaintRequested(pe);
        }

        private bool _mouseDown;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_mouseDown)
                Liquid.MouseDragged(e.X, e.Y);
            else
                Liquid.MouseMoved(e.X, e.Y);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _mouseDown = false;
            Liquid.MouseReleased();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            _mouseDown = true;
            Liquid.MousePressed();
        }

    }
}

//EOF
