using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Graphics g;
        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        
        // ideally create abstract class Shape and have children classes (circle, square, triangle, etc.) inherit from Shape class and override the Draw() method
        public class Circle
        {
            public int Radius { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public Circle(int mouseX, int mouseY, int iRadius)
            {
                X = mouseX;
                Y = mouseY;
                Radius = iRadius;
            }
        }

        
        List<Rectangle> rectangles = new List<Rectangle>();
        List<Circle> circles = new List<Circle>();

        
        int orderNumShape; // variable to store which shape from the list is selected by mouseclick

        // idea for now is that 0 means Rectangle and 1 means Circle (may be improved with enum?)
        byte radioButtonShape; // which shape is chosen by the user
        byte rightClickCheckForm; // which shape was right clicked

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            g = CreateGraphics();

            
            int squareSide = 40;    // ensure it may be devided by 2 - or improve the code to make the check for this
            int circleRadius = 25;

            //creating shapes here to use them in the intersection check algorythms (which need improvement - bugs with circle intersection)
            Rectangle rectangleNew = new Rectangle(e.X - squareSide/2, e.Y - squareSide/2, squareSide, squareSide);
            Circle circleNew = new Circle(e.X - circleRadius, e.Y - circleRadius, circleRadius);


            
            if (e.Button == MouseButtons.Left && radioButtonShape == 0)
            {

                var intersect = false;

                foreach (var rect in rectangles)
                {
                    if (rectangleNew.IntersectsWith(rect))
                    {
                        intersect = true;
                        break;
                    }
                }

                if (!intersect)
                {
                    foreach (var circle in circles)
                    {
                        var distance = Math.Sqrt((Math.Pow(circleNew.X - circle.X, 2) + Math.Pow(circleNew.Y - circle.Y, 2)));
                        if (distance < (circleNew.Radius + circle.Radius))
                        {
                            intersect = true;
                            break;
                        }
                    }
                }


                if (intersect || (e.X < squareSide / 2) || (e.X > this.Width - squareSide) || (e.Y < squareSide / 2) || (e.Y > this.Height - squareSide - 20))
                {
                    MessageBox.Show("Error: intersection detected.");
                }
                else
                {
                    g.DrawRectangle(Pens.Blue, rectangleNew);
                    rectangles.Add(rectangleNew);
                }
            }





            // this solution must be improved. some repeating code is present.
            // also there are bugs with circle intersection check

            if (e.Button == MouseButtons.Left && radioButtonShape == 1)
            {

                var intersect = false;

                foreach (var circle in circles)
                {
                    var distance = Math.Sqrt((Math.Pow(circleNew.X - circle.X, 2) + Math.Pow(circleNew.Y - circle.Y, 2)));
                    if (distance < (circleNew.Radius + circle.Radius))
                    {
                        intersect = true;
                        break;
                    }
                }

                if (!intersect)
                {
                    foreach (var rect in rectangles)
                    {
                        if (rectangleNew.IntersectsWith(rect))
                        {
                            intersect = true;
                            break;
                        }
                    }
                }

                if (intersect || (e.X < circleRadius) || (e.X > this.Width - circleRadius - 20) || (e.Y < circleRadius) || (e.Y > this.Height - circleRadius - 40))
                {
                    MessageBox.Show("Error: intersection detected.");
                }
                else
                {
                    g.DrawEllipse(Pens.Blue, circleNew.X, circleNew.Y, circleNew.Radius * 2, circleNew.Radius * 2);
                    circles.Add(circleNew);
                }
            }

            else if (e.Button == MouseButtons.Right)
            {
                var hitShape = false;

                for (int i = 0; i < rectangles.Count; i++)
                {
                    if (e.X > rectangles[i].X && e.X < (rectangles[i].X + rectangles[i].Width)
                        && e.Y > rectangles[i].Y && e.Y < (rectangles[i].Y + rectangles[i].Height))
                    {
                        hitShape = true;
                        orderNumShape = i;
                        rightClickCheckForm = 0;

                        contextMenuStrip1.Show(this, new Point(e.X, e.Y));
                    }
                }

                // check if Circle was hit only if Square is not hit
                // though right now, due to enlargement there may be intersection and therefore multiple shapes may be hit
                // either add intersection check for enlargement or improve this code
                if (!hitShape)
                {
                    for (int i = 0; i < circles.Count; i++)
                    {
                        var distance = Math.Sqrt((Math.Pow(e.X - circles[i].Radius - circles[i].X, 2) + Math.Pow(e.Y - circles[i].Radius - circles[i].Y, 2)));
                        if (distance < circles[i].Radius)
                        {
                            hitShape = true;
                            orderNumShape = i;
                            rightClickCheckForm = 1;

                            contextMenuStrip1.Show(this, new Point(e.X, e.Y));
                        }
                    }
                }


                if (!hitShape)
                {
                    contextMenuStrip2.Show(this, new Point(e.X, e.Y));
                }
            }
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            // 0 means Rectangle (may be improved with enum?)
            radioButtonShape = 0;
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            // 1 means Circle (may be improved with enum?)
            radioButtonShape = 1;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // check to ensure that shape does not exceed 100
            if ((rightClickCheckForm == 0 && rectangles[orderNumShape].Height > 100)
                || (rightClickCheckForm == 1 && circles[orderNumShape].Radius*2 > 100))
            {
                enlargeToolStripMenuItem.Enabled = false;
            }
            else
            {
                enlargeToolStripMenuItem.Enabled = true;
            }

            // check to ensure that shape can not be decreased into non-existence
            if (rightClickCheckForm == 0 && rectangles[orderNumShape].Height <= 11
                || (rightClickCheckForm == 1 && circles[orderNumShape].Radius <= 7))
            {
                decreaseToolStripMenuItem.Enabled = false;
            }
            else
            {
                decreaseToolStripMenuItem.Enabled = true;
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rightClickCheckForm == 0)
            {
                g.DrawRectangle(Pens.LightGray, rectangles[orderNumShape]);

                rectangles.Remove(rectangles[orderNumShape]);
            }

            else if (rightClickCheckForm == 1)
            {
                g.DrawEllipse(Pens.LightGray, circles[orderNumShape].X, circles[orderNumShape].Y, circles[orderNumShape].Radius*2, circles[orderNumShape].Radius*2);

                circles.Remove(circles[orderNumShape]);
            }

        }

        private void enlargeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ideally should have a check for insersection here as well

            if (rightClickCheckForm == 0)
            {
                g.DrawRectangle(Pens.LightGray, rectangles[orderNumShape]);

                rectangles[orderNumShape] = new Rectangle(rectangles[orderNumShape].X, rectangles[orderNumShape].Y, rectangles[orderNumShape].Width + 11, rectangles[orderNumShape].Height + 11);

                g.DrawRectangle(Pens.Blue, rectangles[orderNumShape]);
            }

            else if (rightClickCheckForm == 1)
            {
                g.DrawEllipse(Pens.LightGray, circles[orderNumShape].X, circles[orderNumShape].Y, circles[orderNumShape].Radius*2, circles[orderNumShape].Radius*2);

                circles[orderNumShape] = new Circle(circles[orderNumShape].X - 7, circles[orderNumShape].Y - 7, circles[orderNumShape].Radius + 7);

                g.DrawEllipse(Pens.Blue, circles[orderNumShape].X, circles[orderNumShape].Y, circles[orderNumShape].Radius*2, circles[orderNumShape].Radius*2);
            }         
        }

        private void decreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (rightClickCheckForm == 0)
            {
                g.DrawRectangle(Pens.LightGray, rectangles[orderNumShape]);

                rectangles[orderNumShape] = new Rectangle(rectangles[orderNumShape].X, rectangles[orderNumShape].Y, rectangles[orderNumShape].Width - 11, rectangles[orderNumShape].Height - 11);

                g.DrawRectangle(Pens.Blue, rectangles[orderNumShape]);
            }

            else if (rightClickCheckForm == 1)
            {
                g.DrawEllipse(Pens.LightGray, circles[orderNumShape].X, circles[orderNumShape].Y, circles[orderNumShape].Radius*2, circles[orderNumShape].Radius*2);

                circles[orderNumShape] = new Circle(circles[orderNumShape].X + 7, circles[orderNumShape].Y + 7, circles[orderNumShape].Radius - 7);

                g.DrawEllipse(Pens.Blue, circles[orderNumShape].X, circles[orderNumShape].Y, circles[orderNumShape].Radius*2, circles[orderNumShape].Radius*2);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // not implemented

            
            MessageBox.Show("not implemented");

            //var result = string.Join("|", circles);
            //System.IO.File.WriteAllText("SavedShapes.txt", result);

        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // not implemented
            MessageBox.Show("not implemented");
        }
    }

}
