using System.Drawing;

namespace DotsAndBoxes
{
    interface IAmblem
    {
        void draw(Graphics g, float x, float y, float a, int d);
    }
}
