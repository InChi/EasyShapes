using System.Collections.Generic;
using System.Drawing;

namespace EasyShapes
{
    public partial class Form1
    {
        abstract class Shape
        {
            public int Side { get; set; }
            public int Radius { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public abstract void Draw(Graphics g);
            public abstract bool checkIntersect(List<Shape> shapes);
            public abstract int checkHitShape(int mouseX, int mouseY, List<Shape> shapes);
        }
    }

}
