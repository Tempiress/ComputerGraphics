using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Affine_transformations_in_space
{
    public partial class Form1 : Form
    {

        polyhedron pop;
        public Form1()
        {
            InitializeComponent();
            
    
        }


        public void DrawTetrahedron()
        {
            pop =  polyhedron.drawTetraedr();
            pictureBox1.Invalidate();

           
        }


        public class point 
        {
            public double X, Y, Z;

            public point(double x, double y, double z) 
            {
                X = x;
                Y = y;
                Z = z;
            }

        }
        public class polygon
        {
            public List<point> Vertices;

            public polygon(List<point> vertices) 
            {
                Vertices = vertices;
            }
        }


        public class polyhedron
        {
            public List<point> Verticles;
            public List<polygon> Faces;

            public polyhedron(List<point> verticles , List<polygon> faces)
            {
                Verticles = verticles;
                Faces = faces;
            }

            public static polyhedron drawTetraedr()
            {

                point v1 = new point(0, 0, 0);
                point v2 = new point(0, 0, 0);
                point v3 = new point(0, 0, 0);
                point v4 = new point(0, 0, 0);

                polygon firstPol = new polygon(new List<point> { v1, v2, v3 });
                polygon secondPol = new polygon(new List<point> { v1, v2, v4 });
                polygon thirdPol = new polygon(new List<point> {v1, v3, v4 });
                polygon fourthPol = new polygon(new List<point> {v2, v3, v4 });

                return new polyhedron(new List<point> { v1, v2, v3, v4 }, new List<polygon> {firstPol, secondPol, thirdPol, fourthPol });
                
            }
        }



        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(pop != null)
            MessageBox.Show(pop.Faces.Count.ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DrawTetrahedron();
        }
    }
}
