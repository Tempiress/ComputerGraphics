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
        public Form1()
        {
            InitializeComponent();
        }
        class point 
        {
            public double X, Y, Z;

            public point(double x, double y, double z) 
            {
                X = x;
                Y = y;
                Z = z;
            }

        }
        class polygon
        {
            public List<point> Vertices;

            public polygon(List<point> vertices) 
            {
                Vertices = vertices;
            }
        }
        class polyhedron 
        {
            public List<point> Verticles;
            public List<point> Faces;

            public polyhedron(List<point> verticles, List<point> faces) 
            {
                Verticles = verticles;
                Faces = faces;
            }

            public void drawTetraedr() 
            {

                point v1 = new point(0, 0, 0);
                point v2 = new point(0, 0, 0);
                point v3 = new point(0, 0, 0);
                point v4 = new point(0, 0, 0);

                polygon firstPol =  new polygon(new List<point> {v1, v2, v3 });
                polygon secondPol = new polygon(new List<point> {v1, v2, v4 });
                polygon thirdPol = new polygon(new List<point> { });
                polygon fourthPol;


            }
        }
    }
}
