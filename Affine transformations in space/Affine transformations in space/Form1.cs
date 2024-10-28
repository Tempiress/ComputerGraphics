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
        class Polygon
        {
            public List<Point> Vertices;

            public Polygon(List<Point> vertices) 
            {
                Vertices = vertices;
            }
        }
        class Polyhedron 
        {
            public List<Point> Verticles;
            public List<Point> Faces;

            public Polyhedron(List<Point> verticles, List<Point> faces) 
            {
                Verticles = verticles;
                Faces = faces;
            } 
        }
    }
}
