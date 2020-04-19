using System;
using System.Drawing;

namespace DotsAndBoxes
{
    class AmblemO : IAmblem
    {
        private SolidBrush brush;
        private Pen pen;

        public AmblemO(Color fill_c, Color symbol_c)
        {
            brush = new SolidBrush(fill_c);
            pen = new Pen(symbol_c);
        }
        public void draw(Graphics g, float x, float y, float a, int d)
        {
            pen.Width = d;

            float l = a/4;
            float w = a/2;

            g.FillRectangle(brush, x, y, a, a);
            g.DrawEllipse(pen, x + l, y + l, w, w);
        }

    }
}
