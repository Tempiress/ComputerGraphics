using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace midpoint_displacement
{
    public partial class Form1 : Form
    {

        Point p;
        List<Point> polygonPoints = new List<Point>();
        Queue<Tuple<Point, int>> pointQueue = new Queue<Tuple<Point, int>>();
        
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Paint += new PaintEventHandler(PictureBox1_Paint);
            pictureBox1.MouseClick += new MouseEventHandler(pictureBox1_Click);
            
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            midpointAlg();
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e) 
        {
            if (polygonPoints.Count > 1) 
            {
                e.Graphics.DrawLines(Pens.Black, polygonPoints.ToArray());

            }

            //e.Graphics.DrawPolygon(Pens.Black, polygonPoints.ToArray());
            //g.DrawLine(Pens.Black, polygonPoints polygonPoints.ToArray());
        }


        private void midpointAlg() 
        {   
            var rand = new Random();
            double roughness = 0.3;

            double hX = (polygonPoints[0].X + polygonPoints[1].X) / 2;
            double hY = (polygonPoints[0].Y + polygonPoints[1].Y) / 2;

            int l = (int)(Math.Sqrt(Math.Pow(polygonPoints[1].X - polygonPoints[0].X, 2) + Math.Pow(polygonPoints[1].Y - polygonPoints[0].Y, 2)));
            double newH = hY - rand.Next((int)(-roughness * l), (int)(roughness * l));
            polygonPoints.Add(new Point((int)hX, (int)newH));
            polygonPoints.Sort((p1, p2) => 
            {
                int res = p1.X.CompareTo(p2.X);
                if (res == 0) 
                {
                    res = p1.Y.CompareTo(p2.Y); 
                }
                return res;
            });
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            if (polygonPoints.Count > 1) return;

            polygonPoints.Add(e.Location);

            if (polygonPoints.Count == 2)
            {
                if (polygonPoints[0].X < polygonPoints[1].X)
                    pointQueue.Enqueue(new Tuple<Point, int>((polygonPoints[1]), 1));
                else
                    pointQueue.Enqueue(new Tuple<Point, int>((polygonPoints[0]), 1));
            }

            pictureBox1.Invalidate();
        }


    }
}
