using System.Collections.Generic;
using System.Drawing;

namespace EasyShapes
{
    public partial class Form1
    {
        class Square : Shape
        {
            public Square(int mouseX, int mouseY, int iSide)
            {
                X = mouseX;
                Y = mouseY;
                Side = iSide;
            }

            public override void Draw(Graphics g)
            {
                g.DrawRectangle(Pens.Blue, this.X, this.Y, this.Side, this.Side);
            }

            public override bool checkIntersect(List<Shape> shapes)
            {
                foreach (var checkSquare in shapes)
                {
                    if (((checkSquare.X + checkSquare.Side) >= X && checkSquare.X <= (X + Side)) &&
                        ((checkSquare.Y + checkSquare.Side) >= Y && checkSquare.Y <= (Y + Side)))
                    {
                        return true;
                    }
                }
                return false;
            }

            public override int checkHitShape(int mouseX, int mouseY, List<Shape> shapes)
            {
                for (int i = 0; i < shapes.Count; i++)
                {
                    if (mouseX > shapes[i].X && mouseX < (shapes[i].X + shapes[i].Side)
                        && mouseY > shapes[i].Y && mouseY < (shapes[i].Y + shapes[i].Side))
                    {
                        return i;
                    }
                }
                return -1;
            }

        }
    }

}
