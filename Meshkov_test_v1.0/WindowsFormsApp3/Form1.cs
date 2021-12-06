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
        readonly int SquareSide = 40;    // ensure it may be devided by 2 - or improve the code to make a check for this
        readonly int CircleRadius = 25;

        readonly int EnlargeDecrSideStep = 11;
        readonly int EnlargeDecrRadiusStep = 7;
        readonly int EnlargeLimit = 100;

        List<Shape> shapes = new List<Shape>();
        int orderNumShape;

        enum ShapeChosen
        {
            Square,
            Circle
        }

        byte radioButtonShape;

        Graphics g;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            drawAllShapes(shapes);
        }

        private void drawAllShapes(List<Shape> shapes)
        {
            g = CreateGraphics();

            foreach (var shape in shapes)
            {
                shape.Draw(g);
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //creating both shapes here to use them in the intersection check algorythms (which need improvement - bugs with circle intersection)
            Shape squareNew = new Square(e.X - SquareSide / 2, e.Y - SquareSide / 2, SquareSide);
            Shape circleNew = new Circle(e.X - CircleRadius, e.Y - CircleRadius, CircleRadius);

            if (e.Button == MouseButtons.Left && radioButtonShape == 0)
            {
                if (squareNew.checkIntersect(shapes) || circleNew.checkIntersect(shapes) ||
                    (e.X < SquareSide / 2) || (e.X > this.Width - SquareSide) || (e.Y < SquareSide / 2) || (e.Y > this.Height - SquareSide - 20))
                {
                    MessageBox.Show("Error: intersection detected.");
                }
                else
                {
                    squareNew.Draw(g);
                    shapes.Add(squareNew);
                }
            }

            if (e.Button == MouseButtons.Left && radioButtonShape == 1)
            {
                if (squareNew.checkIntersect(shapes) || circleNew.checkIntersect(shapes) ||
                    (e.X < CircleRadius) || (e.X > this.Width - CircleRadius - 20) || (e.Y < CircleRadius) || (e.Y > this.Height - CircleRadius - 40))
                {
                    MessageBox.Show("Error: intersection detected.");
                }
                else
                {
                    circleNew.Draw(g);
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
            radioButtonShape = (byte)ShapeChosen.Square;

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            radioButtonShape = (byte)ShapeChosen.Circle;
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            // check to ensure that shape does not exceed enlargeLimit
            if ((shapes[orderNumShape].Side > EnlargeLimit - EnlargeDecrSideStep)
                || (shapes[orderNumShape].Radius * 2 > EnlargeLimit - EnlargeDecrRadiusStep * 2))
            {
                enlargeToolStripMenuItem.Enabled = false;
            }
            else
            {
                enlargeToolStripMenuItem.Enabled = true;
            }

            // check to ensure that shape can not be decreased into non-existence
            if (shapes[orderNumShape].Side != 0 && shapes[orderNumShape].Side <= EnlargeDecrSideStep
                || (shapes[orderNumShape].Radius != 0 && shapes[orderNumShape].Radius <= EnlargeDecrRadiusStep))
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
            shapes.Remove(shapes[orderNumShape]);
            Refresh();
            drawAllShapes(shapes);
        }

        private void enlargeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ideally should have a check for intersection here as well

            if (shapes[orderNumShape].Side != 0)
            {
                shapes[orderNumShape] = new Square(shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Side + EnlargeDecrSideStep);
            }

            else if (shapes[orderNumShape].Radius != 0)
            {
                shapes[orderNumShape] = new Circle(shapes[orderNumShape].X - EnlargeDecrRadiusStep, shapes[orderNumShape].Y - EnlargeDecrRadiusStep, shapes[orderNumShape].Radius + EnlargeDecrRadiusStep);
            }

            Refresh();
            drawAllShapes(shapes);
        }

        private void decreaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (shapes[orderNumShape].Side != 0)
            {
                shapes[orderNumShape] = new Square(shapes[orderNumShape].X, shapes[orderNumShape].Y, shapes[orderNumShape].Side - EnlargeDecrSideStep);
            }

            else if (shapes[orderNumShape].Radius != 0)
            {
                shapes[orderNumShape] = new Circle(shapes[orderNumShape].X + EnlargeDecrRadiusStep, shapes[orderNumShape].Y + EnlargeDecrRadiusStep, shapes[orderNumShape].Radius - EnlargeDecrRadiusStep);
            }

            Refresh();
            drawAllShapes(shapes);
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
            Refresh();

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

            drawAllShapes(shapes);
        }
    }

}
