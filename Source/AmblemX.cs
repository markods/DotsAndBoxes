using System.Drawing;

namespace DotsAndBoxes
{
    class AmblemX : IAmblem
    {
        private SolidBrush brush;
        private Pen pen;

        public AmblemX(Color fill_c, Color symbol_c)
        {
            brush = new SolidBrush(fill_c);
            pen = new Pen(symbol_c);
        }
        public void draw(Graphics g, float x, float y, float a, int d)
        {
            pen.Width = d;

            float l = a * 1/5;
            float r = a * 4/5;

            g.FillRectangle(brush, x, y, a, a);
            g.DrawLine(pen, x + l, y + l, x + r, y + r);    // llrr
            g.DrawLine(pen, x + r, y + l, x + l, y + r);    // rllr -- right shifted by one position
        }

    }
}
