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
    }
}
