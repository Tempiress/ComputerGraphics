using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GrehemAlgorithm
{
    public partial class Form1 : Form
    {
        List<Point> points  = new List<Point>();
        
        public Form1()
        {
            InitializeComponent();

            pictureBox1.MouseClick += drawpoints;
           
        }


        private void pictureBox1_Click(object sender, EventArgs e)
        {
            
        }

        private void drawpoints(object sender, MouseEventArgs e) 
        {
            
            points.Add(e.Location);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (points.Count != 0) 
            {
                Brush brush = new SolidBrush(Color.Black);
                for (int i = 0; i < points.Count; i++) 
                {
                    e.Graphics.FillRectangle(brush, points[i].X, points[i].Y, 2, 2);
                }
                
                
                
            }

        }

        private double rotate(Point A, Point B, Point C)
        {
            return (B.X - A.X) * (C.Y - B.Y) - (B.Y - A.Y) * (C.X - B.X);
        }

        private void Grahamscan(List<Point> pnts)
        {

            int n = pnts.Count;
            List<int> P = new List<int>(n);
            for (int x = 0; x < n; n++)
                P.Add(x);

            for (int i = 0; i < n; i++)
            {

                if (pnts[P[i]].X < pnts[P[0]].X)
                {
                    (P[i], P[0]) = (P[i], P[i]); //Меняем местами номера строк 
                }
            }

            for (int j = 1; j < n; j++)
            {
                int k = j;

                while (k > 1 && (rotate(pnts[P[0]], pnts[P[k - 1]], pnts[P[k]]) < 0))
                {
                    (P[k], P[k - 1]) = (P[k - 1], P[k]);
                    j -= 1;
                }
            }
        }

    }
}
