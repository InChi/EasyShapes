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

namespace EasyShapes
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



        abstract class Shape
        {
            public int Side { get; set; }
            public int Radius { get; set; }
            public int X { get; set; }
            public int Y { get; set; }

            public abstract void Draw();
            public abstract bool checkIntersect(List<Shape> shapes);
            public abstract int checkHitShape(int mouseX, int mouseY, List<Shape> shapes);
        }

        class Square : Shape
        {
            public Square(int mouseX, int mouseY, int iSide)
            {
                X = mouseX;
                Y = mouseY;
                Side = iSide;
            }

            public override void Draw()
            {
                //g.DrawRectangle(Pens.Blue, this.X, this.Y, this.Side, this.Side);
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

        
        class Circle : Shape
        {
            public Circle(int mouseX, int mouseY, int iRadius)
            {
                X = mouseX;
                Y = mouseY;
                Radius = iRadius;
            }

            public override void Draw()
            {
                //g.DrawEllipse(Pens.Blue, this.X, this.Y, this.Radius * 2, this.Radius * 2);
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


        List<Shape> shapes = new List<Shape>();
              
        int orderNumShape;
        
        byte radioButtonShape; // 0 means Square and 1 means Circle (may be improved with enum?)

        int squareSide = 40;    // ensure it may be devided by 2 - or improve the code to make a check for this
        int circleRadius = 25;
        
        int enlargeOrDecreaseSideStep = 11;
        int enlargeOrDecreaseRadiusStep = 7;
        int enlargeLimit = 100;

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            g = CreateGraphics();       

            //creating both shapes here to use them in the intersection check algorythms (which need improvement - bugs with circle intersection)
            Shape squareNew = new Square(e.X - squareSide / 2, e.Y - squareSide / 2, squareSide);
            Shape circleNew = new Circle(e.X - circleRadius, e.Y - circleRadius, circleRadius);

            if (e.Button == MouseButtons.Left && radioButtonShape == 0)
            {
                if (squareNew.checkIntersect(shapes) || circleNew.checkIntersect(shapes) ||
                    (e.X < squareSide / 2) || (e.X > this.Width - squareSide) || (e.Y < squareSide / 2) || (e.Y > this.Height - squareSide - 20))
                {
                    MessageBox.Show("Error: intersection detected.");
                }
                else
                {
                    g.DrawRectangle(Pens.Blue, squareNew.X, squareNew.Y, squareNew.Side, squareNew.Side);
                    shapes.Add(squareNew);
                }
            }
            
            if (e.Button == MouseButtons.Left && radioButtonShape == 1)
            {
                if (squareNew.checkIntersect(shapes) ||
                    circleNew.checkIntersect(shapes) ||
                    (e.X < circleRadius) || (e.X > this.Width - circleRadius - 20) || (e.Y < circleRadius) || (e.Y > this.Height - circleRadius - 40))
                {
                    MessageBox.Show("Error: intersection detected.");
                }
                else
                {
                    g.DrawEllipse(Pens.Blue, circleNew.X, circleNew.Y, circleNew.Radius * 2, circleNew.Radius * 2);
                    shapes.Add(circleNew);
                }
            }

            else if (e.Button == MouseButtons.Right)
            {
                // kind of a bug: right now, due to enlargement there may be intersection and therefore multiple shapes may be hit
                // either add intersection check for enlargement or improve this code to make user decide what to choose

                if (squareNew.checkHitShape(e.X, e.Y, shapes) != -1)
                {
                    orderNumShape = squareNew.checkHitShape(e.X, e.Y, shapes);
                    contextMenuStrip1.Show(this, new Point(e.X, e.Y));
                }
                else if (circleNew.checkHitShape(e.X, e.Y, shapes) != -1)
                {
                    orderNumShape = circleNew.checkHitShape(e.X, e.Y, shapes);
                    contextMenuStrip1.Show(this, new Point(e.X, e.Y));
                }
                else
                {
                    contextMenuStrip2.Show(this, new Point(e.X, e.Y));
                }
            }
        }


        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {            
            radioButtonShape = 0; // 0 means Square (may be improved with enum?)
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {            
            radioButtonShape = 1; // 1 means Circle (may be improved with enum?)
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // check to ensure that shape does not exceed enlargeLimit
            if ((shapes[orderNumShape].Side > enlargeLimit - enlargeOrDecreaseSideStep)
                || (shapes[orderNumShape].Radius * 2 > enlargeLimit - enlargeOrDecreaseRadiusStep * 2))
            {
                enlargeToolStripMenuItem.Enabled = false;
            }
            else
            {
                enlargeToolStripMenuItem.Enabled = true;
            }

            // check to ensure that shape can not be decreased into non-existence
            if (shapes[orderNumShape].Side != 0 && shapes[orderNumShape].Side <= enlargeOrDecreaseSideStep
                || (shapes[orderNumShape].Radius != 0 && shapes[orderNumShape].Radius <= enlargeOrDecreaseRadiusStep))
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
            if (shapes[orderNumShape].Side != 0)
            {
                g.DrawRectangle(Pens.LightGray, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Side, shapes[orderNumShape].Side);

                shapes.Remove(shapes[orderNumShape]);
            }

            else if (shapes[orderNumShape].Radius != 0)
            {
                g.DrawEllipse(Pens.LightGray, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Radius * 2, shapes[orderNumShape].Radius * 2);

                shapes.Remove(shapes[orderNumShape]);
            }

        }

        private void enlargeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ideally should have a check for intersection here as well

            if (shapes[orderNumShape].Side != 0)
            {
                g.DrawRectangle(Pens.LightGray, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Side, shapes[orderNumShape].Side);

                shapes[orderNumShape] = new Square(shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Side + enlargeOrDecreaseSideStep);

                g.DrawRectangle(Pens.Blue, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Side, shapes[orderNumShape].Side);
            }

            else if (shapes[orderNumShape].Radius != 0)
            {
                g.DrawEllipse(Pens.LightGray, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Radius * 2, shapes[orderNumShape].Radius * 2);

                shapes[orderNumShape] = new Circle(shapes[orderNumShape].X - enlargeOrDecreaseRadiusStep, shapes[orderNumShape].Y - enlargeOrDecreaseRadiusStep, shapes[orderNumShape].Radius + enlargeOrDecreaseRadiusStep);

                g.DrawEllipse(Pens.Blue, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Radius * 2, shapes[orderNumShape].Radius * 2);
            }
        }

        private void decreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (shapes[orderNumShape].Side != 0)
            {
                g.DrawRectangle(Pens.LightGray, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Side, shapes[orderNumShape].Side);

                shapes[orderNumShape] = new Square(shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Side - enlargeOrDecreaseSideStep);

                g.DrawRectangle(Pens.Blue, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Side, shapes[orderNumShape].Side);
            }

            else if (shapes[orderNumShape].Radius != 0)
            {
                g.DrawEllipse(Pens.LightGray, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Radius * 2, shapes[orderNumShape].Radius * 2);

                shapes[orderNumShape] = new Circle(shapes[orderNumShape].X + enlargeOrDecreaseRadiusStep, shapes[orderNumShape].Y + enlargeOrDecreaseRadiusStep, shapes[orderNumShape].Radius - enlargeOrDecreaseRadiusStep);

                g.DrawEllipse(Pens.Blue, shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Radius * 2, shapes[orderNumShape].Radius * 2);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var resultShapes = new List<string>();

            foreach (var shape in shapes)
            {
                if (shape.Radius != 0)
                {
                    resultShapes.Add("1:" + shape.X + ":" + shape.Y + ":" + shape.Radius);
                }
                else if (shape.Side != 0)
                {
                    resultShapes.Add("0:" + shape.X + ":" + shape.Y + ":" + shape.Side);
                }
                
            }

            //add possibility to browse for the file to load or at least get user input
            File.WriteAllText("SavedShapes.txt", string.Join("|", resultShapes));
            MessageBox.Show("Saved to SavedShapes.txt next to the exe file");
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //add possibility to browse for the file to load or at least get user input
            var savedShapes = File.ReadAllText("SavedShapes.txt");
            MessageBox.Show("Loading shapes from SavedShapes.txt next to the exe file");
            var savedShapesArray = savedShapes.Split('|');

            shapes.Clear();
            this.Refresh();

            foreach (var shape in savedShapesArray)
            {
                var shapeData = shape.Split(':');

                if (shapeData[0] == "0")
                {
                    Shape squareNew = new Square(Convert.ToInt32(shapeData[1]), Convert.ToInt32(shapeData[2]), Convert.ToInt32(shapeData[3]));

                    shapes.Add(squareNew);
                }
                else if (shapeData[0] == "1")
                {
                    Shape circleNew = new Circle(Convert.ToInt32(shapeData[1]), Convert.ToInt32(shapeData[2]), Convert.ToInt32(shapeData[3]));
                    shapes.Add(circleNew);
                }
                else
                {
                    MessageBox.Show("Error reading input file");
                    break;
                }

            }

            foreach (var shape in shapes)
            {
                if (shape.Side != 0)
                {
                    g.DrawRectangle(Pens.Blue, shape.X, shape.Y, shape.Side, shape.Side);
                }
                else if (shape.Radius !=0)
                {
                    g.DrawEllipse(Pens.Blue, shape.X, shape.Y, shape.Radius * 2, shape.Radius * 2);
                }
                
            }
        }
    }

}
