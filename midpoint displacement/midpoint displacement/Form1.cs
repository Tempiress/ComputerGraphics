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
        List<Point> PolPol = new List<Point>();
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
            if (PolPol.Count > 0) 
            {
                e.Graphics.DrawEllipse(Pens.Black, PolPol[0].X, PolPol[0].Y, 5, 5);
            }
            //e.Graphics.DrawPolygon(Pens.Black, polygonPoints.ToArray());
            //g.DrawLine(Pens.Black, polygonPoints polygonPoints.ToArray());
        }

        private void midpointAlg() 
        {
            //float h = (polygonPoints[0].X + polygonPoints[1].X + polygonPoints[0].Y + polygonPoints[1].Y) / 2;
            double q = (polygonPoints[0].X + polygonPoints[1].X) / 2;
            double v = (polygonPoints[0].Y + polygonPoints[1].Y) / 2;
            
            PolPol.Add(new Point((int)q,(int)v));
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Click(object sender, MouseEventArgs e)
        {
            polygonPoints.Add(e.Location);
            pictureBox1.Invalidate();
        }
    }
}
