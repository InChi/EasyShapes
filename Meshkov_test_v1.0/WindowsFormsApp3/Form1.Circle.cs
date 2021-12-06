using System;
using System.Collections.Generic;
using System.Drawing;

namespace EasyShapes
{
    public partial class Form1
    {
        class Circle : Shape
        {
            public Circle(int mouseX, int mouseY, int iRadius)
            {
                X = mouseX;
                Y = mouseY;
                Radius = iRadius;
            }

            public override void Draw(Graphics g)
            {
                g.DrawEllipse(Pens.Blue, this.X, this.Y, this.Radius * 2, this.Radius * 2);
            }

            // there are bugs with circle intersection check
            public override bool checkIntersect(List<Shape> shapes)
            {
                foreach (var circle in shapes)
                {
                    var distance = Math.Sqrt((Math.Pow(X - circle.X, 2) + Math.Pow(Y - circle.Y, 2)));
                    if (distance < (Radius + circle.Radius))
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
                    var distance = Math.Sqrt((Math.Pow(mouseX - shapes[i].Radius - shapes[i].X, 2) + Math.Pow(mouseY - shapes[i].Radius - shapes[i].Y, 2)));
                    if (distance < shapes[i].Radius)
                    {
                        return i;
                    }
                }
                return -1;
            }
        }
    }

}
